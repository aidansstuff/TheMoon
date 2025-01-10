using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class ReflectionProbeTextureCache
	{
		private IBLFilterBSDF[] m_IBLFiltersBSDF;

		private int m_AtlasWidth;

		private int m_AtlasHeight;

		private GraphicsFormat m_AtlasFormat;

		private int m_AtlasMipCount;

		private int m_AtlasSlicesCount;

		private RTHandle m_AtlasTexture;

		private Texture2DAtlasDynamic m_Atlas;

		private int m_CubeMipPadding;

		private int m_CubeTexelPadding;

		private int m_PlanarMipPadding;

		private int m_PlanarTexelPadding;

		private bool m_DecreaseResToFit;

		private int m_CubeFrameFetchIndex;

		private int m_PlanarFrameFetchIndex;

		private Dictionary<int, (uint, uint)> m_TextureLRUAndHash = new Dictionary<int, (uint, uint)>();

		private List<(int, uint)> m_TextureLRUSorted = new List<(int, uint)>();

		private Material m_ConvertTextureMaterial;

		private MaterialPropertyBlock m_ConvertTexturePropertyBlock = new MaterialPropertyBlock();

		private uint m_CurrentRender;

		private bool m_NoMoreSpaceErrorLogged;

		private RenderTexture m_ConvolvedPlanarReflectionTexture;

		private const int k_MaxFramesTmpUsage = 60;

		private int m_TempCubeTexturesLastFrameUsed;

		private int m_TmpTextureConvertedSize;

		private int m_TmpTextureConvolvedWidth;

		private int m_TmpTextureConvolvedHeight;

		private FilterMode m_TmpConvolvedFilterMode;

		private GraphicsFormat m_TmpTextureConvertedFormat;

		private GraphicsFormat m_TmpTextureConvolvedFormat;

		private FilterMode m_TmpTextureConvertedFilterMode;

		private FilterMode m_TmpTextureConvolvedFilterMode;

		private RenderTexture m_TempConvertedReflectionProbeTexture;

		private RenderTexture m_TempConvolvedReflectionProbeTexture;

		public ReflectionProbeTextureCache(HDRenderPipelineRuntimeResources defaultResources, IBLFilterBSDF[] iblFiltersBSDF, int width, int height, GraphicsFormat format, bool decreaseResToFit, int lastValidCubeMip, int lastValidPlanarMip)
		{
			m_IBLFiltersBSDF = iblFiltersBSDF;
			m_AtlasWidth = width;
			m_AtlasHeight = height;
			m_AtlasFormat = format;
			m_AtlasMipCount = Mathf.FloorToInt(Mathf.Log(Math.Max(m_AtlasWidth, m_AtlasHeight), 2f)) + 1;
			m_AtlasSlicesCount = m_IBLFiltersBSDF.Length;
			m_AtlasTexture = RTHandles.Alloc(width, height, m_AtlasSlicesCount, DepthBits.None, format, FilterMode.Trilinear, TextureWrapMode.Clamp, TextureDimension.Tex2DArray, enableRandomWrite: false, useMipMap: true, autoGenerateMips: false, isShadowMap: false, 1, 0f, MSAASamples.None, bindTextureMS: false, useDynamicScale: false, RenderTextureMemoryless.None, VRTextureUsage.None, "ReflectionProbeCacheTextureAtlas");
			m_Atlas = new Texture2DAtlasDynamic(width, height, 2048, m_AtlasTexture);
			m_CubeMipPadding = Mathf.Clamp(lastValidCubeMip, 0, 6);
			m_CubeTexelPadding = (1 << m_CubeMipPadding) * 2;
			m_PlanarMipPadding = Mathf.Clamp(lastValidPlanarMip, 0, 6);
			m_PlanarTexelPadding = (1 << m_PlanarMipPadding) * 2;
			m_DecreaseResToFit = decreaseResToFit;
			m_ConvertTextureMaterial = CoreUtils.CreateEngineMaterial(defaultResources.shaders.blitCubeTextureFacePS);
		}

		private static int GetTextureID(HDProbe probe)
		{
			int textureSize;
			return GetTextureIDAndSize(probe, out textureSize);
		}

		private static int GetTextureIDAndSize(HDProbe probe, out int textureSize)
		{
			textureSize = GetTextureSizeInAtlas(probe);
			int instanceID = probe.texture.GetInstanceID();
			return 31 * instanceID + textureSize;
		}

		private static int GetTextureSizeInAtlas(HDProbe probe)
		{
			int num = probe.texture.width;
			if (probe.type == ProbeSettings.ProbeType.ReflectionProbe)
			{
				num = Mathf.Min(num, (int)probe.cubeResolution);
				num = GetReflectionProbeSizeInAtlas(num);
			}
			return num;
		}

		public static int GetReflectionProbeSizeInAtlas(int textureSize)
		{
			textureSize = Mathf.Max(textureSize, 32);
			textureSize = ((textureSize >= 512) ? (textureSize * 2) : (textureSize * 4));
			return textureSize;
		}

		private static Vector2 GetTextureSizeWithoutPadding(int textureWidth, int textureHeight, int texelPadding)
		{
			int num = Mathf.Max(textureWidth - texelPadding, 1);
			int num2 = Mathf.Max(textureHeight - texelPadding, 1);
			return new Vector2(num, num2);
		}

		internal static long GetApproxCacheSizeInByte(int elementsCount, int width, int height, GraphicsFormat format)
		{
			return (long)((double)(elementsCount * width * height) * 1.33 * (double)GraphicsFormatUtility.GetBlockSize(format));
		}

		private RenderTexture EnsureConvolvedPlanarReflectionTexture(int textureSize)
		{
			if (m_ConvolvedPlanarReflectionTexture == null || m_ConvolvedPlanarReflectionTexture.width < textureSize)
			{
				m_ConvolvedPlanarReflectionTexture?.Release();
				m_ConvolvedPlanarReflectionTexture = new RenderTexture(textureSize, textureSize, 0, m_AtlasFormat);
				m_ConvolvedPlanarReflectionTexture.hideFlags = HideFlags.HideAndDontSave;
				m_ConvolvedPlanarReflectionTexture.dimension = TextureDimension.Tex2D;
				m_ConvolvedPlanarReflectionTexture.useMipMap = true;
				m_ConvolvedPlanarReflectionTexture.autoGenerateMips = false;
				m_ConvolvedPlanarReflectionTexture.filterMode = FilterMode.Point;
				m_ConvolvedPlanarReflectionTexture.name = CoreUtils.GetRenderTargetAutoName(textureSize, textureSize, 0, m_AtlasFormat, "ConvolvedPlanarReflectionTexture", mips: true);
				m_ConvolvedPlanarReflectionTexture.enableRandomWrite = true;
				m_ConvolvedPlanarReflectionTexture.Create();
			}
			return m_ConvolvedPlanarReflectionTexture;
		}

		private void LogErrorNoMoreSpaceOnce()
		{
			if (!m_NoMoreSpaceErrorLogged)
			{
				m_NoMoreSpaceErrorLogged = true;
				Debug.LogError("No more space in Reflection Probe Atlas. To solve this issue, increase the size of the Reflection Probe Atlas in the HDRP settings.");
			}
		}

		private bool NeedsUpdate(int textureId, uint textureHash, ref Vector4 scaleOffset)
		{
			bool result = false;
			(uint, uint) value;
			if (!m_Atlas.IsCached(out scaleOffset, textureId))
			{
				result = true;
			}
			else if (!m_TextureLRUAndHash.TryGetValue(textureId, out value) || value.Item2 != textureHash)
			{
				result = true;
			}
			m_TextureLRUAndHash[textureId] = (m_CurrentRender, textureHash);
			return result;
		}

		private RenderTexture GetTempConvertedReflectionProbeTexture(Texture texture, int cubeSize)
		{
			if (m_TempConvertedReflectionProbeTexture == null || m_TmpTextureConvertedSize != cubeSize || m_TmpTextureConvertedFormat != m_AtlasFormat || m_TmpTextureConvertedFilterMode != texture.filterMode)
			{
				if (m_TempConvertedReflectionProbeTexture != null)
				{
					RenderTexture.ReleaseTemporary(m_TempConvertedReflectionProbeTexture);
				}
				RenderTexture temporary = RenderTexture.GetTemporary(cubeSize, cubeSize, 0, m_AtlasFormat);
				temporary.dimension = TextureDimension.Cube;
				temporary.filterMode = texture.filterMode;
				temporary.useMipMap = true;
				temporary.autoGenerateMips = false;
				temporary.name = CoreUtils.GetRenderTargetAutoName(cubeSize, cubeSize, 0, m_AtlasFormat, "ConvertedReflectionProbeTemp", mips: true);
				temporary.Create();
				m_TempConvertedReflectionProbeTexture = temporary;
				m_TmpTextureConvertedSize = cubeSize;
				m_TmpTextureConvertedFormat = m_AtlasFormat;
				m_TmpTextureConvertedFilterMode = texture.filterMode;
			}
			m_TempCubeTexturesLastFrameUsed = (int)m_CurrentRender;
			return m_TempConvertedReflectionProbeTexture;
		}

		private RenderTexture PrepareCubeReflectionProbeTexture(CommandBuffer cmd, Texture texture, int textureSize)
		{
			RenderTexture renderTexture = texture as RenderTexture;
			Cubemap cubemap = texture as Cubemap;
			using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.ConvertReflectionProbe)))
			{
				int num = Math.Max(textureSize, (int)Mathf.Pow(2f, 6f));
				if ((texture.graphicsFormat != m_AtlasFormat) | ((bool)cubemap && cubemap.mipmapCount == 1) | ((bool)renderTexture && !renderTexture.useMipMap) | (texture.width != num))
				{
					RenderTexture tempConvertedReflectionProbeTexture = GetTempConvertedReflectionProbeTexture(texture, num);
					m_ConvertTexturePropertyBlock.SetTexture(HDShaderIDs._InputTex, texture);
					m_ConvertTexturePropertyBlock.SetFloat(HDShaderIDs._LoD, 0f);
					for (int i = 0; i < 6; i++)
					{
						m_ConvertTexturePropertyBlock.SetFloat(HDShaderIDs._FaceIndex, i);
						CoreUtils.SetRenderTarget(cmd, tempConvertedReflectionProbeTexture, ClearFlag.None, Color.black, 0, (CubemapFace)i);
						CoreUtils.DrawFullScreen(cmd, m_ConvertTextureMaterial, m_ConvertTexturePropertyBlock);
					}
					cmd.GenerateMips(tempConvertedReflectionProbeTexture);
					return tempConvertedReflectionProbeTexture;
				}
				if ((bool)renderTexture && !renderTexture.autoGenerateMips)
				{
					cmd.GenerateMips(renderTexture);
				}
			}
			return null;
		}

		private RenderTexture GetTempConvolveReflectionProbeTexture(Texture texture)
		{
			if (m_TempConvolvedReflectionProbeTexture == null || m_TmpTextureConvolvedWidth != texture.width || m_TmpTextureConvolvedHeight != texture.height || m_TmpTextureConvolvedFormat != m_AtlasFormat || m_TmpTextureConvolvedFilterMode != texture.filterMode)
			{
				if (m_TempConvolvedReflectionProbeTexture != null)
				{
					RenderTexture.ReleaseTemporary(m_TempConvolvedReflectionProbeTexture);
				}
				RenderTexture temporary = RenderTexture.GetTemporary(texture.width, texture.height, 0, m_AtlasFormat);
				temporary.dimension = TextureDimension.Cube;
				temporary.filterMode = texture.filterMode;
				temporary.useMipMap = true;
				temporary.autoGenerateMips = false;
				temporary.anisoLevel = 0;
				temporary.name = "ConvolvedReflectionProbeTemp";
				temporary.Create();
				m_TempConvolvedReflectionProbeTexture = temporary;
				m_TmpTextureConvolvedWidth = texture.width;
				m_TmpTextureConvolvedHeight = texture.height;
				m_TmpConvolvedFilterMode = texture.filterMode;
				m_TmpTextureConvolvedFormat = m_AtlasFormat;
				m_TmpTextureConvolvedFilterMode = texture.filterMode;
			}
			m_TempCubeTexturesLastFrameUsed = (int)m_CurrentRender;
			return m_TempConvolvedReflectionProbeTexture;
		}

		private RenderTexture ConvolveCubeReflectionProbeTexture(CommandBuffer cmd, Texture texture, IBLFilterBSDF filter)
		{
			RenderTexture tempConvolveReflectionProbeTexture = GetTempConvolveReflectionProbeTexture(texture);
			filter.FilterCubemap(cmd, texture, tempConvolveReflectionProbeTexture);
			return tempConvolveReflectionProbeTexture;
		}

		private RenderTexture ConvolvePlanarReflectionProbeTexture(CommandBuffer cmd, Texture texture, ref IBLFilterBSDF.PlanarTextureFilteringParameters planarTextureFilteringParameters)
		{
			RenderTexture source = texture as RenderTexture;
			RenderTexture renderTexture = EnsureConvolvedPlanarReflectionTexture(texture.width);
			using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.ConvolvePlanarReflectionProbe)))
			{
				((IBLFilterGGX)m_IBLFiltersBSDF[0]).FilterPlanarTexture(cmd, source, ref planarTextureFilteringParameters, renderTexture);
				return renderTexture;
			}
		}

		private void BlitTextureCube(CommandBuffer cmd, Vector4 scaleOffset, Texture texture, int arraySlice)
		{
			using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.BlitTextureToReflectionProbeAtlas)))
			{
				int num = m_CubeTexelPadding;
				int textureWidth = Mathf.CeilToInt(scaleOffset.x * (float)m_AtlasWidth);
				int textureHeight = Mathf.CeilToInt(scaleOffset.y * (float)m_AtlasHeight);
				Vector2 textureSizeWithoutPadding = GetTextureSizeWithoutPadding(textureWidth, textureHeight, num);
				bool bilinear = texture.filterMode != FilterMode.Point;
				for (int i = 0; i < m_AtlasMipCount; i++)
				{
					if (i > m_CubeMipPadding)
					{
						num *= 2;
					}
					cmd.SetRenderTarget(m_AtlasTexture, i, CubemapFace.Unknown, arraySlice);
					Blitter.BlitCubeToOctahedral2DQuadWithPadding(cmd, texture, textureSizeWithoutPadding, scaleOffset, i, bilinear, num);
				}
			}
		}

		private void BlitTexture2D(CommandBuffer cmd, Vector4 scaleOffset, Vector4 sourceScaleOffset, Texture texture, int arraySlice)
		{
			using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.BlitTextureToReflectionProbeAtlas)))
			{
				int planarTexelPadding = m_PlanarTexelPadding;
				int textureWidth = Mathf.CeilToInt(scaleOffset.x * (float)m_AtlasWidth);
				int textureHeight = Mathf.CeilToInt(scaleOffset.y * (float)m_AtlasHeight);
				Vector2 textureSizeWithoutPadding = GetTextureSizeWithoutPadding(textureWidth, textureHeight, planarTexelPadding);
				bool bilinear = texture.filterMode != FilterMode.Point;
				for (int i = 0; i < m_AtlasMipCount; i++)
				{
					cmd.SetRenderTarget(m_AtlasTexture, i, CubemapFace.Unknown, arraySlice);
					Blitter.BlitQuadWithPadding(cmd, texture, textureSizeWithoutPadding, sourceScaleOffset, scaleOffset, i, bilinear, planarTexelPadding);
				}
			}
		}

		private bool RelayoutTextureAtlas()
		{
			List<(int, Vector4)> value;
			using (ListPool<(int, Vector4)>.Get(out value))
			{
				value.Capacity = m_TextureLRUAndHash.Count;
				foreach (KeyValuePair<int, (uint, uint)> item in m_TextureLRUAndHash)
				{
					if (m_Atlas.IsCached(out var scaleOffset, item.Key))
					{
						value.Add((item.Key, scaleOffset));
					}
				}
				value.Sort(((int textureId, Vector4 scaleOffset) a, (int textureId, Vector4 scaleOffset) b) => b.scaleOffset.x.CompareTo(a.scaleOffset.x));
				m_Atlas.ResetAllocator();
				bool result = true;
				foreach (var item2 in value)
				{
					int width = Mathf.CeilToInt(item2.Item2.x * (float)m_AtlasWidth);
					int height = Mathf.CeilToInt(item2.Item2.y * (float)m_AtlasHeight);
					if (m_Atlas.EnsureTextureSlot(out var _, out var scaleOffset2, item2.Item1, width, height))
					{
						Vector2Int vector2Int = new Vector2Int(Mathf.FloorToInt(item2.Item2.z * (float)m_AtlasWidth), Mathf.FloorToInt(item2.Item2.w * (float)m_AtlasHeight));
						Vector2Int vector2Int2 = new Vector2Int(Mathf.FloorToInt(scaleOffset2.z * (float)m_AtlasWidth), Mathf.FloorToInt(scaleOffset2.w * (float)m_AtlasHeight));
						if (vector2Int != vector2Int2)
						{
							(uint, uint) value2 = m_TextureLRUAndHash[item2.Item1];
							value2.Item2 = 0u;
							m_TextureLRUAndHash[item2.Item1] = value2;
						}
					}
					else
					{
						m_TextureLRUAndHash.Remove(item2.Item1);
						result = false;
					}
				}
				return result;
			}
		}

		private bool TryAllocateTexture(int textureId, int textureSize, ref Vector4 scaleOffset)
		{
			if (m_Atlas.EnsureTextureSlot(out var isUploadNeeded, out scaleOffset, textureId, textureSize, textureSize))
			{
				return true;
			}
			for (int num = m_TextureLRUSorted.Count - 1; num >= 0; num--)
			{
				(int, uint) tuple = m_TextureLRUSorted[num];
				if (m_CurrentRender - tuple.Item2 <= 1)
				{
					break;
				}
				m_Atlas.ReleaseTextureSlot(tuple.Item1);
				m_TextureLRUAndHash.Remove(tuple.Item1);
				m_TextureLRUSorted.RemoveAt(num);
				if (m_Atlas.EnsureTextureSlot(out isUploadNeeded, out scaleOffset, textureId, textureSize, textureSize))
				{
					return true;
				}
			}
			if (m_DecreaseResToFit && m_Atlas.EnsureTextureSlot(out isUploadNeeded, out scaleOffset, textureId, textureSize / 2, textureSize / 2))
			{
				return true;
			}
			m_TextureLRUAndHash.Remove(textureId);
			return false;
		}

		private bool UpdateTexture(CommandBuffer cmd, HDProbe probe, ref Vector4 scaleOffset)
		{
			using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.UpdateReflectionProbeAtlas)))
			{
				Texture texture = probe.texture;
				int textureSize;
				int textureIDAndSize = GetTextureIDAndSize(probe, out textureSize);
				if (!m_Atlas.IsCached(out scaleOffset, textureIDAndSize) && !TryAllocateTexture(textureIDAndSize, textureSize, ref scaleOffset))
				{
					return false;
				}
				RenderTexture renderTexture = PrepareCubeReflectionProbeTexture(cmd, texture, textureSize);
				for (int i = 0; i < m_IBLFiltersBSDF.Length; i++)
				{
					RenderTexture texture2 = ConvolveCubeReflectionProbeTexture(cmd, renderTexture ? renderTexture : texture, m_IBLFiltersBSDF[i]);
					BlitTextureCube(cmd, scaleOffset, texture2, i);
				}
				return true;
			}
		}

		private bool UpdateTexture(CommandBuffer cmd, HDProbe probe, ref IBLFilterBSDF.PlanarTextureFilteringParameters planarTextureFilteringParameters, ref Vector4 scaleOffset)
		{
			using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.UpdateReflectionProbeAtlas)))
			{
				Texture texture = probe.texture;
				int textureSize;
				int textureIDAndSize = GetTextureIDAndSize(probe, out textureSize);
				if (!m_Atlas.IsCached(out scaleOffset, textureIDAndSize) && !TryAllocateTexture(textureIDAndSize, textureSize, ref scaleOffset))
				{
					return false;
				}
				RenderTexture renderTexture = ConvolvePlanarReflectionProbeTexture(cmd, texture, ref planarTextureFilteringParameters);
				float num = (float)texture.width / (float)renderTexture.width;
				BlitTexture2D(sourceScaleOffset: new Vector4(num, num, 0f, 0f), cmd: cmd, scaleOffset: scaleOffset, texture: renderTexture, arraySlice: 0);
				return true;
			}
		}

		public Vector4 GetTextureAtlasCubeData()
		{
			return new Vector4((float)m_CubeTexelPadding / (float)m_AtlasWidth, (float)m_CubeTexelPadding / (float)m_AtlasHeight, m_CubeMipPadding, 0f);
		}

		public Vector4 GetTextureAtlasPlanarData()
		{
			return new Vector4((float)m_PlanarTexelPadding / (float)m_AtlasWidth, (float)m_PlanarTexelPadding / (float)m_AtlasHeight, 1f / (float)m_AtlasWidth, 1f / (float)m_AtlasHeight);
		}

		public Texture GetAtlasTexture()
		{
			return m_AtlasTexture;
		}

		public int GetEnvSliceSize()
		{
			return m_AtlasSlicesCount;
		}

		public void Release()
		{
			IBLFilterBSDF[] iBLFiltersBSDF = m_IBLFiltersBSDF;
			for (int i = 0; i < iBLFiltersBSDF.Length; i++)
			{
				iBLFiltersBSDF[i].Cleanup();
			}
			m_IBLFiltersBSDF = null;
			m_AtlasTexture.Release();
			m_AtlasTexture = null;
			m_Atlas.Release();
			m_Atlas = null;
			m_TextureLRUAndHash = null;
			m_ConvertTextureMaterial = null;
			m_ConvolvedPlanarReflectionTexture?.Release();
			if (m_TempConvertedReflectionProbeTexture != null)
			{
				RenderTexture.ReleaseTemporary(m_TempConvertedReflectionProbeTexture);
				m_TempConvertedReflectionProbeTexture = null;
			}
			if (m_TempConvolvedReflectionProbeTexture != null)
			{
				RenderTexture.ReleaseTemporary(m_TempConvolvedReflectionProbeTexture);
				m_TempConvolvedReflectionProbeTexture = null;
			}
		}

		public Vector4 FetchCubeReflectionProbe(CommandBuffer cmd, HDProbe probe, out int fetchIndex)
		{
			_ = probe.texture;
			fetchIndex = m_CubeFrameFetchIndex++;
			Vector4 scaleOffset = Vector4.zero;
			int textureID = GetTextureID(probe);
			if (NeedsUpdate(textureID, probe.GetTextureHash(), ref scaleOffset) && !UpdateTexture(cmd, probe, ref scaleOffset))
			{
				LogErrorNoMoreSpaceOnce();
			}
			return scaleOffset;
		}

		public Vector4 FetchPlanarReflectionProbe(CommandBuffer cmd, HDProbe probe, ref IBLFilterBSDF.PlanarTextureFilteringParameters planarTextureFilteringParameters, out int fetchIndex)
		{
			_ = probe.texture;
			fetchIndex = m_PlanarFrameFetchIndex++;
			Vector4 scaleOffset = Vector4.zero;
			int textureID = GetTextureID(probe);
			if (NeedsUpdate(textureID, probe.GetTextureHash(), ref scaleOffset) && !UpdateTexture(cmd, probe, ref planarTextureFilteringParameters, ref scaleOffset))
			{
				LogErrorNoMoreSpaceOnce();
			}
			return scaleOffset;
		}

		public void ReserveReflectionProbeSlot(HDProbe probe)
		{
			_ = probe.texture;
			int textureSize;
			int textureIDAndSize = GetTextureIDAndSize(probe, out textureSize);
			if (!m_Atlas.IsCached(out var _, textureIDAndSize))
			{
				Vector4 scaleOffset2 = Vector4.zero;
				if (!TryAllocateTexture(textureIDAndSize, textureSize, ref scaleOffset2) && RelayoutTextureAtlas())
				{
					TryAllocateTexture(textureIDAndSize, textureSize, ref scaleOffset2);
				}
			}
		}

		public void NewFrame()
		{
			m_CubeFrameFetchIndex = 0;
			m_PlanarFrameFetchIndex = 0;
		}

		public void NewRender()
		{
			m_NoMoreSpaceErrorLogged = false;
			m_CurrentRender++;
			m_TextureLRUSorted.Clear();
			foreach (KeyValuePair<int, (uint, uint)> item in m_TextureLRUAndHash)
			{
				m_TextureLRUSorted.Add((item.Key, item.Value.Item1));
			}
			m_TextureLRUSorted.Sort(((int, uint) a, (int, uint) b) => b.Item2.CompareTo(a.Item2));
		}

		public void GarbageCollectTmpResources()
		{
			if (Math.Max((int)m_CurrentRender - m_TempCubeTexturesLastFrameUsed, 0) > 60)
			{
				m_TempConvertedReflectionProbeTexture?.Release();
				m_TempConvolvedReflectionProbeTexture?.Release();
				m_TempConvertedReflectionProbeTexture = null;
				m_TempConvolvedReflectionProbeTexture = null;
			}
		}

		public void ClearAtlasAllocator()
		{
			m_Atlas.ResetAllocator();
			m_TextureLRUAndHash.Clear();
			m_TextureLRUSorted.Clear();
		}

		public void Clear(CommandBuffer cmd)
		{
			ClearAtlasAllocator();
			for (int i = 0; i < m_AtlasSlicesCount; i++)
			{
				for (int j = 0; j < m_AtlasMipCount; j++)
				{
					cmd.SetRenderTarget(m_AtlasTexture, j, CubemapFace.Unknown, i);
					Blitter.BlitQuad(cmd, Texture2D.blackTexture, new Vector4(1f, 1f, 0f, 0f), new Vector4(1f, 1f, 0f, 0f), j, bilinear: true);
				}
			}
		}
	}
}
