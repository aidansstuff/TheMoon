using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class TextureCache2D : TextureCache
	{
		private RenderTexture m_Cache;

		public TextureCache2D(string cacheName = "")
			: base(cacheName)
		{
		}

		private bool TextureHasMipmaps(Texture texture)
		{
			if (texture is Texture2D)
			{
				return ((Texture2D)texture).mipmapCount > 1;
			}
			return ((RenderTexture)texture).useMipMap;
		}

		public override bool IsCreated()
		{
			return m_Cache.IsCreated();
		}

		protected override bool TransferToSlice(CommandBuffer cmd, int sliceIndex, Texture[] textureArray)
		{
			if (textureArray == null || (textureArray.Length == 0 && !(textureArray[0] is RenderTexture) && !(textureArray[0] is Texture2D)))
			{
				return false;
			}
			for (int i = 1; i < textureArray.Length; i++)
			{
				if (textureArray[i].width != textureArray[0].width || textureArray[i].height != textureArray[0].height || (!(textureArray[0] is RenderTexture) && !(textureArray[0] is Texture2D)))
				{
					Debug.LogWarning("All the sub-textures should have the same dimensions to be handled by the texture cache.");
					return false;
				}
			}
			bool flag = m_Cache.width != textureArray[0].width || m_Cache.height != textureArray[0].height;
			if (textureArray[0] is Texture2D)
			{
				flag |= m_Cache.graphicsFormat != (textureArray[0] as Texture2D).graphicsFormat;
			}
			for (int j = 0; j < textureArray.Length; j++)
			{
				if (!TextureHasMipmaps(textureArray[j]))
				{
					Debug.LogWarning("The texture '" + textureArray[j]?.ToString() + "' should have mipmaps to be handled by the cookie texture array");
				}
				if (flag)
				{
					cmd.Blit(textureArray[j], m_Cache, 0, m_SliceSize * sliceIndex + j);
				}
				else
				{
					cmd.CopyTexture(textureArray[j], 0, m_Cache, m_SliceSize * sliceIndex + j);
				}
			}
			return true;
		}

		public override Texture GetTexCache()
		{
			return m_Cache;
		}

		public bool AllocTextureArray(int numTextures, int width, int height, GraphicsFormat format, bool isMipMapped)
		{
			bool result = AllocTextureArray(numTextures);
			m_NumMipLevels = GetNumMips(width, height);
			RenderTextureDescriptor desc = new RenderTextureDescriptor(width, height, format, 0)
			{
				dimension = TextureDimension.Tex2DArray,
				volumeDepth = numTextures,
				useMipMap = isMipMapped,
				msaaSamples = 1
			};
			RenderTexture obj = new RenderTexture(desc)
			{
				hideFlags = HideFlags.HideAndDontSave,
				wrapMode = TextureWrapMode.Clamp
			};
			obj.name = CoreUtils.GetTextureAutoName(width, height, format, TextureDimension.Tex2DArray, m_CacheName, mips: false, numTextures);
			m_Cache = obj;
			ClearCache();
			m_Cache.Create();
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
			CoreUtils.Destroy(m_Cache);
		}

		internal static long GetApproxCacheSizeInByte(int nbElement, int resolution, int sliceSize)
		{
			return (long)((float)((long)nbElement * (long)resolution * resolution * 2 * 4) * 1.33f * (float)sliceSize);
		}

		internal static int GetMaxCacheSizeForWeightInByte(int weight, int resolution, int sliceSize)
		{
			return Mathf.Clamp(Mathf.FloorToInt((float)weight / ((float)((long)resolution * (long)resolution * 2 * 4) * 1.33f * (float)sliceSize)), 1, 250);
		}
	}
}
