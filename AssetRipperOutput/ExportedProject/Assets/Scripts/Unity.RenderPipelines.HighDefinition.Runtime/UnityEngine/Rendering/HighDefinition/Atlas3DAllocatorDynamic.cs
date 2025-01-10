using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class Atlas3DAllocatorDynamic
	{
		private class Atlas3DNodePool
		{
			public Atlas3DNode[] m_Nodes;

			private short m_Next;

			private short m_FreelistHead;

			public Atlas3DNodePool(short capacity)
			{
				m_Nodes = new Atlas3DNode[capacity];
				m_Next = 0;
				m_FreelistHead = -1;
			}

			public void Dispose()
			{
				Clear();
				m_Nodes = null;
			}

			public void Clear()
			{
				m_Next = 0;
				m_FreelistHead = -1;
			}

			public short Atlas3DNodeCreate(short parent)
			{
				if (m_FreelistHead != -1)
				{
					short freelistNext = m_Nodes[m_FreelistHead].m_FreelistNext;
					m_Nodes[m_FreelistHead] = new Atlas3DNode(m_FreelistHead, parent);
					short freelistHead = m_FreelistHead;
					m_FreelistHead = freelistNext;
					return freelistHead;
				}
				m_Nodes[m_Next] = new Atlas3DNode(m_Next, parent);
				return m_Next++;
			}

			public void Atlas3DNodeFree(short index)
			{
				m_Nodes[index].m_FreelistNext = m_FreelistHead;
				m_FreelistHead = index;
			}
		}

		private struct Atlas3DNode
		{
			private enum Atlas3DNodeFlags : uint
			{
				IsOccupied = 1u
			}

			public short m_Self;

			public short m_Parent;

			public short m_LeftChild;

			public short m_RightChild;

			public short m_FreelistNext;

			public ushort m_Flags;

			public Vector3 m_RectSize;

			public Vector3 m_RectOffset;

			public Atlas3DNode(short self, short parent)
			{
				m_Self = self;
				m_Parent = parent;
				m_LeftChild = -1;
				m_RightChild = -1;
				m_Flags = 0;
				m_FreelistNext = -1;
				m_RectSize = Vector3.zero;
				m_RectOffset = Vector3.zero;
			}

			public bool IsOccupied()
			{
				return (m_Flags & 1) > 0;
			}

			public void SetIsOccupied()
			{
				ushort num = 1;
				m_Flags |= num;
			}

			public void ClearIsOccupied()
			{
				ushort num = 1;
				m_Flags &= (ushort)(~num);
			}

			public bool IsLeafNode()
			{
				return m_LeftChild == -1;
			}

			public short Allocate(Atlas3DNodePool pool, int width, int height, int depth)
			{
				if (Mathf.Min(Mathf.Min(width, height), depth) < 1)
				{
					return -1;
				}
				if (!IsLeafNode())
				{
					short num = pool.m_Nodes[m_LeftChild].Allocate(pool, width, height, depth);
					if (num == -1)
					{
						num = pool.m_Nodes[m_RightChild].Allocate(pool, width, height, depth);
					}
					return num;
				}
				if (IsOccupied())
				{
					return -1;
				}
				if ((float)width > m_RectSize.x || (float)height > m_RectSize.y || (float)depth > m_RectSize.z)
				{
					return -1;
				}
				m_LeftChild = pool.Atlas3DNodeCreate(m_Self);
				m_RightChild = pool.Atlas3DNodeCreate(m_Self);
				float num2 = m_RectSize.x - (float)width;
				float num3 = m_RectSize.y - (float)height;
				float num4 = m_RectSize.z - (float)depth;
				if (num2 >= num3 && num2 >= num4)
				{
					pool.m_Nodes[m_LeftChild].m_RectSize = new Vector3(width, m_RectSize.y, m_RectSize.z);
					pool.m_Nodes[m_LeftChild].m_RectOffset = m_RectOffset;
					pool.m_Nodes[m_RightChild].m_RectSize = new Vector3(num2, m_RectSize.y, m_RectSize.z);
					pool.m_Nodes[m_RightChild].m_RectOffset = new Vector3(m_RectOffset.x + (float)width, m_RectOffset.y, m_RectOffset.z);
					if (Mathf.Max(num3, num4) < 1f)
					{
						pool.m_Nodes[m_LeftChild].SetIsOccupied();
						return m_LeftChild;
					}
					short num5 = pool.m_Nodes[m_LeftChild].Allocate(pool, width, height, depth);
					if (num5 >= 0)
					{
						pool.m_Nodes[num5].SetIsOccupied();
					}
					return num5;
				}
				if (num3 >= num2 && num3 >= num4)
				{
					pool.m_Nodes[m_LeftChild].m_RectSize = new Vector3(m_RectSize.x, height, m_RectSize.z);
					pool.m_Nodes[m_LeftChild].m_RectOffset = m_RectOffset;
					pool.m_Nodes[m_RightChild].m_RectSize = new Vector3(m_RectSize.x, num3, m_RectSize.z);
					pool.m_Nodes[m_RightChild].m_RectOffset = new Vector3(m_RectOffset.x, m_RectOffset.y + (float)height, m_RectOffset.z);
					if (Math.Max(num2, num4) < 1f)
					{
						pool.m_Nodes[m_LeftChild].SetIsOccupied();
						return m_LeftChild;
					}
					short num6 = pool.m_Nodes[m_LeftChild].Allocate(pool, width, height, depth);
					if (num6 >= 0)
					{
						pool.m_Nodes[num6].SetIsOccupied();
					}
					return num6;
				}
				pool.m_Nodes[m_LeftChild].m_RectSize = new Vector3(m_RectSize.x, m_RectSize.y, depth);
				pool.m_Nodes[m_LeftChild].m_RectOffset = m_RectOffset;
				pool.m_Nodes[m_RightChild].m_RectSize = new Vector3(m_RectSize.x, m_RectSize.y, num4);
				pool.m_Nodes[m_RightChild].m_RectOffset = new Vector3(m_RectOffset.x, m_RectOffset.y, m_RectOffset.z + (float)depth);
				if (Math.Max(num2, num3) < 1f)
				{
					pool.m_Nodes[m_LeftChild].SetIsOccupied();
					return m_LeftChild;
				}
				short num7 = pool.m_Nodes[m_LeftChild].Allocate(pool, width, height, depth);
				if (num7 >= 0)
				{
					pool.m_Nodes[num7].SetIsOccupied();
				}
				return num7;
			}

			public void ReleaseChildren(Atlas3DNodePool pool)
			{
				if (!IsLeafNode())
				{
					pool.m_Nodes[m_LeftChild].ReleaseChildren(pool);
					pool.m_Nodes[m_RightChild].ReleaseChildren(pool);
					pool.Atlas3DNodeFree(m_LeftChild);
					pool.Atlas3DNodeFree(m_RightChild);
					m_LeftChild = -1;
					m_RightChild = -1;
				}
			}

			public void ReleaseAndMerge(Atlas3DNodePool pool)
			{
				short num = m_Self;
				do
				{
					pool.m_Nodes[num].ReleaseChildren(pool);
					pool.m_Nodes[num].ClearIsOccupied();
					num = pool.m_Nodes[num].m_Parent;
				}
				while (num >= 0 && pool.m_Nodes[num].IsMergeNeeded(pool));
			}

			public bool IsMergeNeeded(Atlas3DNodePool pool)
			{
				if (pool.m_Nodes[m_LeftChild].IsLeafNode() && !pool.m_Nodes[m_LeftChild].IsOccupied() && pool.m_Nodes[m_RightChild].IsLeafNode())
				{
					return !pool.m_Nodes[m_RightChild].IsOccupied();
				}
				return false;
			}
		}

		private int m_Width;

		private int m_Height;

		private int m_Depth;

		private Atlas3DNodePool m_Pool;

		private short m_Root;

		private Dictionary<int, short> m_NodeFromID;

		public Atlas3DAllocatorDynamic(int width, int height, int depth, int capacityAllocations)
		{
			int num = capacityAllocations * 2;
			m_Pool = new Atlas3DNodePool((short)num);
			m_NodeFromID = new Dictionary<int, short>(capacityAllocations);
			short parent = -1;
			m_Root = m_Pool.Atlas3DNodeCreate(parent);
			m_Pool.m_Nodes[m_Root].m_RectSize = new Vector3(width, height, depth);
			m_Pool.m_Nodes[m_Root].m_RectOffset = Vector3.zero;
			m_Width = width;
			m_Height = height;
			m_Depth = depth;
		}

		public bool Allocate(out Vector3 resultSize, out Vector3 resultOffset, int key, int width, int height, int depth)
		{
			short num = m_Pool.m_Nodes[m_Root].Allocate(m_Pool, width, height, depth);
			if (num >= 0)
			{
				resultSize = m_Pool.m_Nodes[num].m_RectSize;
				resultOffset = m_Pool.m_Nodes[num].m_RectOffset;
				m_NodeFromID.Add(key, num);
				return true;
			}
			resultSize = Vector3.zero;
			resultOffset = Vector3.zero;
			return false;
		}

		public void Release(int key)
		{
			if (m_NodeFromID.TryGetValue(key, out var value))
			{
				m_Pool.m_Nodes[value].ReleaseAndMerge(m_Pool);
				m_NodeFromID.Remove(key);
			}
		}

		public void Release()
		{
			m_Pool.Clear();
			m_Root = m_Pool.Atlas3DNodeCreate(-1);
			m_Pool.m_Nodes[m_Root].m_RectSize = new Vector3(m_Width, m_Height, m_Depth);
			m_Pool.m_Nodes[m_Root].m_RectOffset = Vector3.zero;
			m_NodeFromID.Clear();
		}

		public string DebugStringFromRoot(int depthMax = -1)
		{
			string res = "";
			DebugStringFromNode(ref res, m_Root, 0, depthMax);
			return res;
		}

		private void DebugStringFromNode(ref string res, short n, int depthCurrent = 0, int depthMax = -1)
		{
			res = res + "{[" + depthCurrent + "], isOccupied = " + (m_Pool.m_Nodes[n].IsOccupied() ? "true" : "false") + ", self = " + m_Pool.m_Nodes[n].m_Self + ", " + m_Pool.m_Nodes[n].m_RectSize.x + "," + m_Pool.m_Nodes[n].m_RectSize.y + ", " + m_Pool.m_Nodes[n].m_RectSize.z + ", " + m_Pool.m_Nodes[n].m_RectOffset.x + ", " + m_Pool.m_Nodes[n].m_RectOffset.y + ", " + m_Pool.m_Nodes[n].m_RectOffset.z + "}\n";
			if (depthMax == -1 || depthCurrent < depthMax)
			{
				if (m_Pool.m_Nodes[n].m_LeftChild >= 0)
				{
					DebugStringFromNode(ref res, m_Pool.m_Nodes[n].m_LeftChild, depthCurrent + 1, depthMax);
				}
				if (m_Pool.m_Nodes[n].m_RightChild >= 0)
				{
					DebugStringFromNode(ref res, m_Pool.m_Nodes[n].m_RightChild, depthCurrent + 1, depthMax);
				}
			}
		}
	}
}
