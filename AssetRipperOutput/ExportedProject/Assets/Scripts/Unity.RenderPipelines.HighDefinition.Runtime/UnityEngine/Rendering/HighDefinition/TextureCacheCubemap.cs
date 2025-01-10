using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class TextureCacheCubemap : TextureCache
	{
		private RenderTexture m_Cache;

		private const int k_NbFace = 6;

		private Texture2DArray m_CacheNoCubeArray;

		private RenderTexture[] m_StagingRTs;

		private int m_NumPanoMipLevels;

		private Material m_CubeBlitMaterial;

		private int m_CubeMipLevelPropName;

		private int m_cubeSrcTexPropName;

		private Material m_BlitCubemapFaceMaterial;

		private MaterialPropertyBlock m_BlitCubemapFaceProperties;

		public TextureCacheCubemap(string cacheName = "", int sliceSize = 1)
			: base(cacheName, sliceSize)
		{
			if (HDRenderPipeline.isReady)
			{
				m_BlitCubemapFaceMaterial = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.blitCubeTextureFacePS);
			}
			m_BlitCubemapFaceProperties = new MaterialPropertyBlock();
		}

		public override bool IsCreated()
		{
			return m_Cache.IsCreated();
		}

		protected override bool TransferToSlice(CommandBuffer cmd, int sliceIndex, Texture[] textureArray)
		{
			if (!TextureCache.supportsCubemapArrayTextures)
			{
				return TransferToPanoCache(cmd, sliceIndex, textureArray);
			}
			if (textureArray == null || textureArray.Length == 0)
			{
				return false;
			}
			for (int i = 1; i < textureArray.Length; i++)
			{
				if (textureArray[i].width != textureArray[0].width || textureArray[i].height != textureArray[0].height)
				{
					Debug.LogWarning("All the sub-textures should have the same dimensions to be handled by the texture cache.");
					return false;
				}
			}
			bool flag = m_Cache.width != textureArray[0].width || m_Cache.height != textureArray[0].height;
			if (textureArray[0] is Cubemap)
			{
				flag |= m_Cache.graphicsFormat != (textureArray[0] as Cubemap).graphicsFormat;
			}
			for (int j = 0; j < textureArray.Length; j++)
			{
				if (flag)
				{
					m_BlitCubemapFaceProperties.SetTexture(HDShaderIDs._InputTex, textureArray[j]);
					m_BlitCubemapFaceProperties.SetFloat(HDShaderIDs._LoD, 0f);
					for (int k = 0; k < 6; k++)
					{
						m_BlitCubemapFaceProperties.SetFloat(HDShaderIDs._FaceIndex, k);
						CoreUtils.SetRenderTarget(cmd, m_Cache, ClearFlag.None, Color.black, 0, CubemapFace.Unknown, 6 * (m_SliceSize * sliceIndex + j) + k);
						CoreUtils.DrawFullScreen(cmd, m_BlitCubemapFaceMaterial, m_BlitCubemapFaceProperties);
					}
				}
				else
				{
					for (int l = 0; l < 6; l++)
					{
						cmd.CopyTexture(textureArray[j], l, m_Cache, 6 * (m_SliceSize * sliceIndex + j) + l);
					}
				}
			}
			return true;
		}

		public override Texture GetTexCache()
		{
			if (TextureCache.supportsCubemapArrayTextures)
			{
				return m_Cache;
			}
			return m_CacheNoCubeArray;
		}

		public bool AllocTextureArray(int numCubeMaps, int width, GraphicsFormat format, bool isMipMapped, Material cubeBlitMaterial)
		{
			bool result = AllocTextureArray(numCubeMaps);
			m_NumMipLevels = GetNumMips(width, width);
			if (!TextureCache.supportsCubemapArrayTextures)
			{
				m_CubeBlitMaterial = cubeBlitMaterial;
				int num = 4 * width;
				int num2 = 2 * width;
				Texture2DArray obj = new Texture2DArray(num, num2, numCubeMaps, format, isMipMapped ? TextureCreationFlags.MipChain : TextureCreationFlags.None)
				{
					hideFlags = HideFlags.HideAndDontSave,
					wrapMode = TextureWrapMode.Repeat,
					wrapModeV = TextureWrapMode.Clamp,
					filterMode = FilterMode.Trilinear,
					anisoLevel = 0
				};
				int depth = numCubeMaps;
				obj.name = CoreUtils.GetTextureAutoName(num, num2, format, TextureDimension.Tex2DArray, m_CacheName, mips: false, depth);
				m_CacheNoCubeArray = obj;
				m_NumPanoMipLevels = ((!isMipMapped) ? 1 : GetNumMips(num, num2));
				m_StagingRTs = new RenderTexture[m_NumPanoMipLevels];
				for (int i = 0; i < m_NumPanoMipLevels; i++)
				{
					m_StagingRTs[i] = new RenderTexture(Mathf.Max(1, num >> i), Mathf.Max(1, num2 >> i), 0, format)
					{
						hideFlags = HideFlags.HideAndDontSave
					};
					m_StagingRTs[i].name = CoreUtils.GetRenderTargetAutoName(Mathf.Max(1, num >> i), Mathf.Max(1, num2 >> i), 1, format, $"PanaCache{i}");
				}
				if ((bool)m_CubeBlitMaterial)
				{
					m_CubeMipLevelPropName = Shader.PropertyToID("_cubeMipLvl");
					m_cubeSrcTexPropName = Shader.PropertyToID("_srcCubeTexture");
				}
			}
			else
			{
				RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(width, width, format, 0);
				renderTextureDescriptor.dimension = TextureDimension.CubeArray;
				renderTextureDescriptor.volumeDepth = numCubeMaps * 6;
				renderTextureDescriptor.autoGenerateMips = false;
				renderTextureDescriptor.useMipMap = isMipMapped;
				renderTextureDescriptor.msaaSamples = 1;
				RenderTextureDescriptor desc = renderTextureDescriptor;
				RenderTexture obj2 = new RenderTexture(desc)
				{
					hideFlags = HideFlags.HideAndDontSave,
					wrapMode = TextureWrapMode.Clamp,
					filterMode = FilterMode.Trilinear,
					anisoLevel = 0
				};
				TextureDimension dimension = desc.dimension;
				int depth = numCubeMaps;
				obj2.name = CoreUtils.GetTextureAutoName(width, width, format, dimension, m_CacheName, isMipMapped, depth);
				m_Cache = obj2;
				ClearCache();
				m_Cache.Create();
			}
			return result;
		}

		internal void ClearCache()
		{
			RenderTextureDescriptor descriptor = m_Cache.descriptor;
			int num = ((!descriptor.useMipMap) ? 1 : GetNumMips(descriptor.width, descriptor.height));
			for (int i = 0; i < num; i++)
			{
				Graphics.SetRenderTarget(m_Cache, i, CubemapFace.Unknown, -1);
				GL.Clear(clearDepth: false, clearColor: true, Color.clear);
			}
		}

		public void Release()
		{
			if ((bool)m_CacheNoCubeArray)
			{
				CoreUtils.Destroy(m_CacheNoCubeArray);
				for (int i = 0; i < m_NumPanoMipLevels; i++)
				{
					m_StagingRTs[i].Release();
				}
				m_StagingRTs = null;
				CoreUtils.Destroy(m_CubeBlitMaterial);
			}
			CoreUtils.Destroy(m_BlitCubemapFaceMaterial);
			CoreUtils.Destroy(m_Cache);
		}

		private bool TransferToPanoCache(CommandBuffer cmd, int sliceIndex, Texture[] textureArray)
		{
			for (int i = 0; i < textureArray.Length; i++)
			{
				m_CubeBlitMaterial.SetTexture(m_cubeSrcTexPropName, textureArray[i]);
				for (int j = 0; j < m_NumPanoMipLevels; j++)
				{
					m_CubeBlitMaterial.SetInt(m_CubeMipLevelPropName, Mathf.Min(m_NumMipLevels - 1, j));
					cmd.Blit(null, m_StagingRTs[j], m_CubeBlitMaterial, 0);
				}
				for (int k = 0; k < m_NumPanoMipLevels; k++)
				{
					cmd.CopyTexture(m_StagingRTs[k], 0, 0, m_CacheNoCubeArray, m_SliceSize * sliceIndex + i, k);
				}
			}
			return true;
		}

		internal static long GetApproxCacheSizeInByte(int nbElement, int resolution, int sliceSize)
		{
			return (long)((float)((long)nbElement * (long)resolution * resolution * 6 * 2 * 4) * 1.33f * (float)sliceSize);
		}

		internal static int GetMaxCacheSizeForWeightInByte(long weight, int resolution, int sliceSize)
		{
			return Mathf.Clamp(Mathf.FloorToInt((float)weight / ((float)((long)resolution * (long)resolution * 6 * 2 * 4) * 1.33f * (float)sliceSize)), 1, 250);
		}
	}
}
