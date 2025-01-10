using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class LightCookieManager
	{
		private HDRenderPipelineAsset m_RenderPipelineAsset;

		internal static readonly int s_texSource = Shader.PropertyToID("_SourceTexture");

		internal static readonly int s_texCubeSource = Shader.PropertyToID("_SourceCubeTexture");

		internal static readonly int s_sourceMipLevel = Shader.PropertyToID("_SourceMipLevel");

		internal static readonly int s_sourceSize = Shader.PropertyToID("_SourceSize");

		internal static readonly int s_uvLimits = Shader.PropertyToID("_UVLimits");

		internal const int k_MinCookieSize = 2;

		private readonly Material m_MaterialFilterAreaLights;

		private MaterialPropertyBlock m_MPBFilterAreaLights = new MaterialPropertyBlock();

		private RenderTexture m_TempRenderTexture0;

		private RenderTexture m_TempRenderTexture1;

		private PowerOfTwoTextureAtlas m_CookieAtlas;

		private bool m_2DCookieAtlasNeedsLayouting;

		private bool m_NoMoreSpace;

		private readonly int cookieAtlasLastValidMip;

		private readonly GraphicsFormat cookieFormat;

		public RTHandle atlasTexture => m_CookieAtlas.AtlasTexture;

		public PowerOfTwoTextureAtlas atlas => m_CookieAtlas;

		public LightCookieManager(HDRenderPipelineAsset hdAsset, int maxCacheSize)
		{
			m_RenderPipelineAsset = hdAsset;
			HDRenderPipelineRuntimeResources renderPipelineResources = HDRenderPipelineGlobalSettings.instance.renderPipelineResources;
			GlobalLightLoopSettings lightLoopSettings = hdAsset.currentPlatformRenderPipelineSettings.lightLoopSettings;
			m_MaterialFilterAreaLights = CoreUtils.CreateEngineMaterial(renderPipelineResources.shaders.filterAreaLightCookiesPS);
			int cookieAtlasSize = (int)lightLoopSettings.cookieAtlasSize;
			cookieFormat = (GraphicsFormat)lightLoopSettings.cookieFormat;
			cookieAtlasLastValidMip = lightLoopSettings.cookieAtlasLastValidMip;
			m_CookieAtlas = new PowerOfTwoTextureAtlas(cookieAtlasSize, lightLoopSettings.cookieAtlasLastValidMip, cookieFormat, FilterMode.Point, "Cookie Atlas (Punctual Lights)");
		}

		public void NewFrame()
		{
			m_CookieAtlas.ResetRequestedTexture();
			m_2DCookieAtlasNeedsLayouting = false;
			m_NoMoreSpace = false;
		}

		public void Release()
		{
			CoreUtils.Destroy(m_MaterialFilterAreaLights);
			if (m_TempRenderTexture0 != null)
			{
				m_TempRenderTexture0.Release();
				m_TempRenderTexture0 = null;
			}
			if (m_TempRenderTexture1 != null)
			{
				m_TempRenderTexture1.Release();
				m_TempRenderTexture1 = null;
			}
			if (m_CookieAtlas != null)
			{
				m_CookieAtlas.Release();
				m_CookieAtlas = null;
			}
		}

		private void ReserveTempTextureIfNeeded(CommandBuffer cmd, int mipMapCount)
		{
			if (m_TempRenderTexture0 == null)
			{
				int width = m_CookieAtlas.AtlasTexture.rt.width;
				int height = m_CookieAtlas.AtlasTexture.rt.height;
				string name = m_CookieAtlas.AtlasTexture.name;
				m_TempRenderTexture0 = new RenderTexture(width, height, 1, cookieFormat)
				{
					hideFlags = HideFlags.HideAndDontSave,
					useMipMap = true,
					autoGenerateMips = false,
					name = name + "TempAreaLightRT0"
				};
				for (int i = 0; i < mipMapCount; i++)
				{
					cmd.SetRenderTarget(m_TempRenderTexture0, i);
					cmd.ClearRenderTarget(clearDepth: false, clearColor: true, Color.clear);
				}
				m_TempRenderTexture1 = new RenderTexture(width >> 1, height, 1, cookieFormat)
				{
					hideFlags = HideFlags.HideAndDontSave,
					useMipMap = true,
					autoGenerateMips = false,
					name = name + "TempAreaLightRT1"
				};
				for (int j = 0; j < mipMapCount - 1; j++)
				{
					cmd.SetRenderTarget(m_TempRenderTexture1, j);
					cmd.ClearRenderTarget(clearDepth: false, clearColor: true, Color.clear);
				}
			}
		}

		private Texture FilterAreaLightTexture(CommandBuffer cmd, Texture source, int finalWidth, int finalHeight)
		{
			if (m_MaterialFilterAreaLights == null)
			{
				Debug.LogError("FilterAreaLightTexture has an invalid shader. Can't filter area light cookie.");
				return null;
			}
			int num = m_CookieAtlas.AtlasTexture.rt.width;
			int num2 = m_CookieAtlas.AtlasTexture.rt.height;
			int num3 = finalWidth;
			int num4 = finalHeight;
			int num5 = 1 + Mathf.FloorToInt(Mathf.Log(Mathf.Max(source.width, source.height), 2f));
			ReserveTempTextureIfNeeded(cmd, num5);
			using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.AreaLightCookieConvolution)))
			{
				int num6 = num;
				int num7 = num2;
				if (source.dimension == TextureDimension.Cube)
				{
					m_MPBFilterAreaLights.SetInt(s_sourceMipLevel, 0);
					m_MPBFilterAreaLights.SetTexture(s_texCubeSource, source);
					cmd.SetRenderTarget(m_TempRenderTexture0, 0);
					cmd.SetViewport(new Rect(0f, 0f, num3, num4));
					cmd.DrawProcedural(Matrix4x4.identity, m_MaterialFilterAreaLights, 3, MeshTopology.Triangles, 3, 1, m_MPBFilterAreaLights);
				}
				else
				{
					m_MPBFilterAreaLights.SetInt(s_sourceMipLevel, 0);
					m_MPBFilterAreaLights.SetTexture(s_texSource, source);
					int num8 = 1;
					cmd.SetRenderTarget(m_TempRenderTexture0, 0);
					cmd.SetViewport(new Rect(0f, 0f, num3 + num8, num4 + num8));
					m_MPBFilterAreaLights.SetVector(s_sourceSize, new Vector4(num3, num4, (float)(num3 + num8) / (float)num3, (float)(num4 + num8) / (float)num4));
					cmd.DrawProcedural(Matrix4x4.identity, m_MaterialFilterAreaLights, 0, MeshTopology.Triangles, 3, 1, m_MPBFilterAreaLights);
				}
				Vector4 zero = Vector4.zero;
				for (int i = 1; i < num5; i++)
				{
					zero.Set((float)num3 / (float)num * 1f, (float)num4 / (float)num2, 1f / (float)num, 1f / (float)num2);
					Vector4 value = new Vector4(0f, 0f, (float)num3 / (float)num, (float)num4 / (float)num2);
					num3 = Mathf.Max(1, num3 >> 1);
					num6 = Mathf.Max(1, num6 >> 1);
					m_MPBFilterAreaLights.SetTexture(s_texSource, m_TempRenderTexture0);
					m_MPBFilterAreaLights.SetInt(s_sourceMipLevel, i - 1);
					m_MPBFilterAreaLights.SetVector(s_sourceSize, zero);
					m_MPBFilterAreaLights.SetVector(s_uvLimits, value);
					cmd.SetRenderTarget(m_TempRenderTexture1, i - 1);
					cmd.SetViewport(new Rect(0f, 0f, num3, num4));
					cmd.DrawProcedural(Matrix4x4.identity, m_MaterialFilterAreaLights, 1, MeshTopology.Triangles, 3, 1, m_MPBFilterAreaLights);
					num = num6;
					zero.Set((float)num3 / (float)num, (float)num4 / (float)num2 * 1f, 1f / (float)num, 1f / (float)num2);
					Vector4 value2 = new Vector4(0f, 0f, (float)num3 / (float)num, (float)num4 / (float)num2);
					num4 = Mathf.Max(1, num4 >> 1);
					num7 = Mathf.Max(1, num7 >> 1);
					m_MPBFilterAreaLights.SetTexture(s_texSource, m_TempRenderTexture1);
					m_MPBFilterAreaLights.SetInt(s_sourceMipLevel, i - 1);
					m_MPBFilterAreaLights.SetVector(s_sourceSize, zero);
					m_MPBFilterAreaLights.SetVector(s_uvLimits, value2);
					cmd.SetRenderTarget(m_TempRenderTexture0, i);
					cmd.SetViewport(new Rect(0f, 0f, num3, num4));
					cmd.DrawProcedural(Matrix4x4.identity, m_MaterialFilterAreaLights, 2, MeshTopology.Triangles, 3, 1, m_MPBFilterAreaLights);
					num2 = num7;
				}
			}
			return m_TempRenderTexture0;
		}

		public void LayoutIfNeeded()
		{
			if (m_2DCookieAtlasNeedsLayouting && !m_CookieAtlas.RelayoutEntries())
			{
				Debug.LogError("No more space in the 2D Cookie Texture Atlas. To solve this issue, increase the resolution of the cookie atlas in the HDRP settings.");
				m_NoMoreSpace = true;
			}
		}

		public Vector4 Fetch2DCookie(CommandBuffer cmd, Texture cookie, Texture ies)
		{
			int num = Mathf.Max(cookie.width, ies.height);
			int num2 = Mathf.Max(cookie.width, ies.height);
			if (num < 2 || num2 < 2)
			{
				return Vector4.zero;
			}
			if (!m_CookieAtlas.IsCached(out var scaleOffset, m_CookieAtlas.GetTextureID(cookie, ies)) && !m_NoMoreSpace)
			{
				Debug.LogError($"Unity cannot fetch the 2D Light cookie texture: {cookie} because it is not on the cookie atlas. To resolve this, open your HDRP Asset and increase the resolution of the cookie atlas.");
			}
			if (m_CookieAtlas.NeedsUpdate(cookie, ies))
			{
				m_CookieAtlas.BlitTexture(cmd, scaleOffset, ies, new Vector4(1f, 1f, 0f, 0f), blitMips: false, m_CookieAtlas.GetTextureID(cookie, ies));
				m_CookieAtlas.BlitTextureMultiply(cmd, scaleOffset, cookie, new Vector4(1f, 1f, 0f, 0f), blitMips: false, m_CookieAtlas.GetTextureID(cookie, ies));
			}
			return scaleOffset;
		}

		public Vector4 Fetch2DCookie(CommandBuffer cmd, Texture cookie)
		{
			if (cookie.width < 2 || cookie.height < 2)
			{
				return Vector4.zero;
			}
			if (!m_CookieAtlas.IsCached(out var scaleOffset, m_CookieAtlas.GetTextureID(cookie)) && !m_NoMoreSpace)
			{
				Debug.LogError($"Unity cannot fetch the 2D Light cookie texture: {cookie} because it is not on the cookie atlas. To resolve this, open your HDRP Asset and increase the resolution of the cookie atlas.");
			}
			if (m_CookieAtlas.NeedsUpdate(cookie))
			{
				m_CookieAtlas.BlitTexture(cmd, scaleOffset, cookie, new Vector4(1f, 1f, 0f, 0f), blitMips: false);
			}
			return scaleOffset;
		}

		public Vector4 FetchAreaCookie(CommandBuffer cmd, Texture cookie)
		{
			if (cookie.width < 2 || cookie.height < 2)
			{
				return Vector4.zero;
			}
			if (!m_CookieAtlas.IsCached(out var scaleOffset, cookie) && !m_NoMoreSpace)
			{
				Debug.LogError($"Area Light cookie texture {cookie} can't be fetched without having reserved. You can try to increase the cookie atlas resolution in the HDRP settings.");
			}
			int textureID = m_CookieAtlas.GetTextureID(cookie);
			if (m_CookieAtlas.NeedsUpdate(cookie, needMips: true))
			{
				Texture texture = FilterAreaLightTexture(cmd, cookie, cookie.width, cookie.height);
				Vector4 sourceScaleOffset = new Vector4(((float)cookie.width - 0.5f) / (float)atlasTexture.rt.width, ((float)cookie.height - 0.5f) / (float)atlasTexture.rt.height, 0f, 0f);
				m_CookieAtlas.BlitTexture(cmd, scaleOffset, texture, sourceScaleOffset, blitMips: true, textureID);
			}
			return scaleOffset;
		}

		public Vector4 FetchAreaCookie(CommandBuffer cmd, Texture cookie, Texture ies)
		{
			int num = Mathf.Max(cookie.width, ies.height);
			int num2 = Mathf.Max(cookie.width, ies.height);
			if (num < 2 || num2 < 2)
			{
				return Vector4.zero;
			}
			int num3 = 2 * (int)Mathf.Max((float)cookie.width, (float)ies.width);
			if (!m_CookieAtlas.IsCached(out var scaleOffset, cookie, ies) && !m_NoMoreSpace)
			{
				Debug.LogError($"Area Light cookie texture {cookie} & {ies} can't be fetched without having reserved. You can try to increase the cookie atlas resolution in the HDRP settings.");
			}
			if (m_CookieAtlas.NeedsUpdate(cookie, ies, needMips: true))
			{
				Vector4 sourceScaleOffset = new Vector4((float)num3 / (float)atlasTexture.rt.width, (float)num3 / (float)atlasTexture.rt.height, 0f, 0f);
				Texture texture = FilterAreaLightTexture(cmd, cookie, num3, num3);
				m_CookieAtlas.BlitOctahedralTexture(cmd, scaleOffset, texture, sourceScaleOffset, blitMips: true, m_CookieAtlas.GetTextureID(cookie, ies));
				texture = FilterAreaLightTexture(cmd, ies, num3, num3);
				m_CookieAtlas.BlitOctahedralTextureMultiply(cmd, scaleOffset, texture, sourceScaleOffset, blitMips: true, m_CookieAtlas.GetTextureID(cookie, ies));
			}
			return scaleOffset;
		}

		public void ReserveSpace(Texture cookieA, Texture cookieB)
		{
			if (!(cookieA == null) && !(cookieB == null))
			{
				int num = Mathf.Max(cookieA.width, cookieB.height);
				int num2 = Mathf.Max(cookieA.width, cookieB.height);
				if (num >= 2 && num2 >= 2 && !m_CookieAtlas.ReserveSpace(cookieA, cookieB, num, num2))
				{
					m_2DCookieAtlasNeedsLayouting = true;
				}
			}
		}

		public void ReserveSpace(Texture cookie)
		{
			if (!(cookie == null) && cookie.width >= 2 && cookie.height >= 2 && !m_CookieAtlas.ReserveSpace(cookie))
			{
				m_2DCookieAtlasNeedsLayouting = true;
			}
		}

		public void ReserveSpaceCube(Texture cookie)
		{
			if (!(cookie == null))
			{
				int num = 2 * cookie.width;
				if (num >= 2 && !m_CookieAtlas.ReserveSpace(cookie, num, num))
				{
					m_2DCookieAtlasNeedsLayouting = true;
				}
			}
		}

		public void ReserveSpaceCube(Texture cookieA, Texture cookieB)
		{
			if (!(cookieA == null) || !(cookieB == null))
			{
				int num = 2 * Mathf.Max(cookieA.width, cookieB.width);
				if (num >= 2 && !m_CookieAtlas.ReserveSpace(cookieA, cookieB, num, num))
				{
					m_2DCookieAtlasNeedsLayouting = true;
				}
			}
		}

		public Vector4 FetchCubeCookie(CommandBuffer cmd, Texture cookie)
		{
			int num = 2 * cookie.width;
			if (num < 2)
			{
				return Vector4.zero;
			}
			if (!m_CookieAtlas.IsCached(out var scaleOffset, cookie) && !m_NoMoreSpace)
			{
				Debug.LogError($"Unity cannot fetch the Cube cookie texture: {cookie} because it is not on the cookie atlas. To resolve this, open your HDRP Asset and increase the resolution of the cookie atlas.");
			}
			if (m_CookieAtlas.NeedsUpdate(cookie, needMips: true))
			{
				Vector4 sourceScaleOffset = new Vector4((float)num / (float)atlasTexture.rt.width, (float)num / (float)atlasTexture.rt.height, 0f, 0f);
				Texture texture = FilterAreaLightTexture(cmd, cookie, num, num);
				m_CookieAtlas.BlitOctahedralTexture(cmd, scaleOffset, texture, sourceScaleOffset, blitMips: true, m_CookieAtlas.GetTextureID(cookie));
			}
			return scaleOffset;
		}

		public Vector4 FetchCubeCookie(CommandBuffer cmd, Texture cookie, Texture ies)
		{
			int num = 2 * cookie.width;
			if (num < 2)
			{
				return Vector4.zero;
			}
			if (!m_CookieAtlas.IsCached(out var scaleOffset, cookie, ies) && !m_NoMoreSpace)
			{
				Debug.LogError($"Unity cannot fetch the Cube cookie texture: {cookie} because it is not on the cookie atlas. To resolve this, open your HDRP Asset and increase the resolution of the cookie atlas.");
			}
			if (m_CookieAtlas.NeedsUpdate(cookie, ies, needMips: true))
			{
				Vector4 sourceScaleOffset = new Vector4((float)num / (float)atlasTexture.rt.width, (float)num / (float)atlasTexture.rt.height, 0f, 0f);
				Texture texture = FilterAreaLightTexture(cmd, cookie, num, num);
				m_CookieAtlas.BlitOctahedralTexture(cmd, scaleOffset, texture, sourceScaleOffset, blitMips: true, m_CookieAtlas.GetTextureID(cookie, ies));
				texture = FilterAreaLightTexture(cmd, ies, num, num);
				m_CookieAtlas.BlitOctahedralTextureMultiply(cmd, scaleOffset, texture, sourceScaleOffset, blitMips: true, m_CookieAtlas.GetTextureID(cookie, ies));
			}
			return scaleOffset;
		}

		public void ResetAllocator()
		{
			m_CookieAtlas.ResetAllocator();
		}

		public void ClearAtlasTexture(CommandBuffer cmd)
		{
			m_CookieAtlas.ClearTarget(cmd);
		}

		public Vector4 GetCookieAtlasSize()
		{
			return new Vector4(m_CookieAtlas.AtlasTexture.rt.width, m_CookieAtlas.AtlasTexture.rt.height, 1f / (float)m_CookieAtlas.AtlasTexture.rt.width, 1f / (float)m_CookieAtlas.AtlasTexture.rt.height);
		}

		public Vector4 GetCookieAtlasDatas()
		{
			float num = Mathf.Pow(2f, m_CookieAtlas.mipPadding) * 2f;
			return new Vector4(m_CookieAtlas.AtlasTexture.rt.width, num / (float)m_CookieAtlas.AtlasTexture.rt.width, cookieAtlasLastValidMip, 0f);
		}
	}
}
