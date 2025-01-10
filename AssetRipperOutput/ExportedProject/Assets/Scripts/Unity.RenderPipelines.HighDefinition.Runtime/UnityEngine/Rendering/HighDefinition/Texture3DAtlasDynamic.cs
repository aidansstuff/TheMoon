using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class Texture3DAtlasDynamic
	{
		private struct Texture3DAtlasScaleBias
		{
			public Vector3 scale;

			public Vector3 bias;
		}

		private RTHandle m_AtlasTexture;

		private bool isAtlasTextureOwner;

		private int m_Width;

		private int m_Height;

		private int m_Depth;

		private GraphicsFormat m_Format;

		private Atlas3DAllocatorDynamic m_AtlasAllocator;

		private Dictionary<int, Texture3DAtlasScaleBias> m_AllocationCache;

		public RTHandle AtlasTexture => m_AtlasTexture;

		public Texture3DAtlasDynamic(int width, int height, int depth, int capacity, GraphicsFormat format)
		{
			m_Width = width;
			m_Height = height;
			m_Depth = depth;
			m_Format = format;
			m_AtlasTexture = RTHandles.Alloc(m_Width, m_Height, m_Depth, DepthBits.None, m_Format, FilterMode.Point, TextureWrapMode.Clamp, TextureDimension.Tex3D, enableRandomWrite: false, useMipMap: true, autoGenerateMips: false);
			isAtlasTextureOwner = true;
			m_AtlasAllocator = new Atlas3DAllocatorDynamic(width, height, depth, capacity);
			m_AllocationCache = new Dictionary<int, Texture3DAtlasScaleBias>(capacity);
		}

		public Texture3DAtlasDynamic(int width, int height, int depth, int capacity, RTHandle atlasTexture)
		{
			m_Width = width;
			m_Height = height;
			m_Depth = depth;
			m_Format = atlasTexture.rt.graphicsFormat;
			m_AtlasTexture = atlasTexture;
			isAtlasTextureOwner = false;
			m_AtlasAllocator = new Atlas3DAllocatorDynamic(width, height, depth, capacity);
			m_AllocationCache = new Dictionary<int, Texture3DAtlasScaleBias>(capacity);
		}

		public void Release()
		{
			ResetAllocator();
			if (isAtlasTextureOwner)
			{
				RTHandles.Release(m_AtlasTexture);
			}
		}

		public void ResetAllocator()
		{
			m_AtlasAllocator.Release();
			m_AllocationCache.Clear();
		}

		public bool TryGetScaleBias(out Vector3 scale, out Vector3 bias, int key)
		{
			if (m_AllocationCache.TryGetValue(key, out var value))
			{
				scale = value.scale;
				bias = value.bias;
				return true;
			}
			scale = Vector3.zero;
			bias = Vector3.zero;
			return false;
		}

		public bool EnsureTextureSlot(out bool isUploadNeeded, out Vector3 scale, out Vector3 bias, int key, int width, int height, int depth)
		{
			isUploadNeeded = false;
			if (m_AllocationCache.TryGetValue(key, out var value))
			{
				scale = value.scale;
				bias = value.bias;
				return true;
			}
			if (!m_AtlasAllocator.Allocate(out scale, out bias, key, width, height, depth))
			{
				return false;
			}
			isUploadNeeded = true;
			scale.Scale(new Vector3(1f / (float)m_Width, 1f / (float)m_Height, 1f / (float)m_Depth));
			bias.Scale(new Vector3(1f / (float)m_Width, 1f / (float)m_Height, 1f / (float)m_Depth));
			m_AllocationCache.Add(key, new Texture3DAtlasScaleBias
			{
				scale = scale,
				bias = bias
			});
			return true;
		}

		public void ReleaseTextureSlot(int key)
		{
			m_AtlasAllocator.Release(key);
			m_AllocationCache.Remove(key);
		}

		public string DebugStringFromRoot(int depthMax = -1)
		{
			return m_AtlasAllocator.DebugStringFromRoot(depthMax);
		}
	}
}
