using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	internal abstract class TextureCache
	{
		private struct SliceEntry
		{
			public uint texId;

			public uint countLRU;

			public uint sliceEntryHash;
		}

		protected string m_CacheName;

		protected int m_NumMipLevels;

		protected int m_SliceSize;

		private int m_NumTextures;

		private Dictionary<uint, int> m_LocatorInSliceDictionnary;

		private SliceEntry[] m_SliceArray;

		private int[] m_SortedIdxArray;

		private Texture[] m_autoContentArray = new Texture[1];

		private static uint g_MaxFrameCount = uint.MaxValue;

		private static uint g_InvalidTexID = 0u;

		protected const int k_FP16SizeInByte = 2;

		protected const int k_NbChannel = 4;

		protected const float k_MipmapFactorApprox = 1.33f;

		internal const int k_MaxSupported = 250;

		private static List<int> s_TempIntList = new List<int>();

		public static bool isMobileBuildTarget => Application.isMobilePlatform;

		public static bool supportsCubemapArrayTextures => !GraphicsSettings.HasShaderDefine(BuiltinShaderDefine.UNITY_NO_CUBEMAP_ARRAY);

		protected TextureCache(string cacheName, int sliceSize = 1)
		{
			m_CacheName = cacheName;
			m_SliceSize = sliceSize;
			m_NumTextures = 0;
			m_NumMipLevels = 0;
		}

		public virtual bool IsCreated()
		{
			return true;
		}

		public string GetCacheName()
		{
			return m_CacheName;
		}

		public int GetNumMipLevels()
		{
			return m_NumMipLevels;
		}

		protected bool AllocTextureArray(int numTextures)
		{
			if (numTextures >= m_SliceSize)
			{
				m_SliceArray = new SliceEntry[numTextures];
				m_SortedIdxArray = new int[numTextures];
				m_LocatorInSliceDictionnary = new Dictionary<uint, int>();
				m_NumTextures = numTextures / m_SliceSize;
				for (int i = 0; i < m_NumTextures; i++)
				{
					m_SliceArray[i].countLRU = g_MaxFrameCount;
					m_SliceArray[i].texId = g_InvalidTexID;
					m_SortedIdxArray[i] = i;
				}
			}
			return numTextures >= m_SliceSize;
		}

		public abstract Texture GetTexCache();

		public int ReserveSlice(Texture texture, uint textureHash, out bool needUpdate)
		{
			needUpdate = false;
			if (texture == null)
			{
				return -1;
			}
			uint instanceID = (uint)texture.GetInstanceID();
			if (instanceID == g_InvalidTexID)
			{
				return -1;
			}
			int value = -1;
			if (m_LocatorInSliceDictionnary.TryGetValue(instanceID, out value))
			{
				needUpdate |= m_SliceArray[value].sliceEntryHash != textureHash;
			}
			else
			{
				bool flag = false;
				int num = 0;
				int num2 = 0;
				while (!flag && num < m_NumTextures)
				{
					num2 = m_SortedIdxArray[num];
					if (m_SliceArray[num2].countLRU == 0)
					{
						num++;
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					needUpdate = true;
					if (m_SliceArray[num2].texId != g_InvalidTexID)
					{
						m_LocatorInSliceDictionnary.Remove(m_SliceArray[num2].texId);
					}
					m_LocatorInSliceDictionnary.Add(instanceID, num2);
					m_SliceArray[num2].texId = instanceID;
					value = num2;
				}
			}
			if (value != -1)
			{
				m_SliceArray[value].countLRU = 0u;
			}
			needUpdate |= !IsCreated();
			return value;
		}

		public bool UpdateSlice(CommandBuffer cmd, int sliceIndex, Texture[] contentArray, uint textureHash)
		{
			SetSliceHash(sliceIndex, textureHash);
			return TransferToSlice(cmd, sliceIndex, contentArray);
		}

		public bool UpdateSlice(CommandBuffer cmd, int sliceIndex, Texture texture, uint textureHash)
		{
			SetSliceHash(sliceIndex, textureHash);
			m_autoContentArray[0] = texture;
			return TransferToSlice(cmd, sliceIndex, m_autoContentArray);
		}

		public void SetSliceHash(int sliceIndex, uint hash)
		{
			m_SliceArray[sliceIndex].sliceEntryHash = hash;
		}

		protected abstract bool TransferToSlice(CommandBuffer cmd, int sliceIndex, Texture[] textureArray);

		public int FetchSlice(CommandBuffer cmd, Texture texture, uint textureHash, bool forceReinject = false)
		{
			bool needUpdate = false;
			int num = ReserveSlice(texture, textureHash, out needUpdate);
			bool flag = forceReinject || needUpdate;
			if (num != -1 && flag)
			{
				m_autoContentArray[0] = texture;
				UpdateSlice(cmd, num, m_autoContentArray, textureHash);
			}
			return num;
		}

		public void NewFrame()
		{
			int num = 0;
			s_TempIntList.Clear();
			for (int i = 0; i < m_NumTextures; i++)
			{
				s_TempIntList.Add(m_SortedIdxArray[i]);
				if (m_SliceArray[m_SortedIdxArray[i]].countLRU != 0)
				{
					num++;
				}
			}
			int num2 = 0;
			int num3 = 0;
			for (int j = 0; j < m_NumTextures; j++)
			{
				if (m_SliceArray[s_TempIntList[j]].countLRU == 0)
				{
					m_SortedIdxArray[num3 + num] = s_TempIntList[j];
					num3++;
				}
				else
				{
					m_SortedIdxArray[num2] = s_TempIntList[j];
					num2++;
				}
			}
			for (int k = 0; k < m_NumTextures; k++)
			{
				if (m_SliceArray[k].countLRU < g_MaxFrameCount)
				{
					m_SliceArray[k].countLRU++;
				}
			}
		}

		public void RemoveEntryFromSlice(Texture texture)
		{
			uint instanceID = (uint)texture.GetInstanceID();
			if (instanceID == g_InvalidTexID || !m_LocatorInSliceDictionnary.ContainsKey(instanceID))
			{
				return;
			}
			int num = m_LocatorInSliceDictionnary[instanceID];
			bool flag = false;
			int num2 = 0;
			while (!flag && num2 < m_NumTextures)
			{
				if (m_SortedIdxArray[num2] == num)
				{
					flag = true;
				}
				else
				{
					num2++;
				}
			}
			if (flag)
			{
				for (int i = 0; i < num2; i++)
				{
					m_SortedIdxArray[i + 1] = m_SortedIdxArray[i];
				}
				m_SortedIdxArray[0] = num;
				m_LocatorInSliceDictionnary.Remove(instanceID);
				m_SliceArray[num].countLRU = g_MaxFrameCount;
				m_SliceArray[num].texId = g_InvalidTexID;
			}
		}

		protected int GetNumMips(int width, int height)
		{
			return GetNumMips((width > height) ? width : height);
		}

		protected int GetNumMips(int dim)
		{
			uint num = (uint)dim;
			int num2 = 0;
			while (num != 0)
			{
				num2++;
				num >>= 1;
			}
			return num2;
		}
	}
}
