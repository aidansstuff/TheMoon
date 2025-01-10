using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class CullingGroupManager
	{
		private static CullingGroupManager m_Instance;

		private Stack<CullingGroup> m_FreeList = new Stack<CullingGroup>();

		public static CullingGroupManager instance
		{
			get
			{
				if (m_Instance == null)
				{
					m_Instance = new CullingGroupManager();
				}
				return m_Instance;
			}
		}

		public CullingGroup Alloc()
		{
			CullingGroup cullingGroup;
			if (m_FreeList.Count > 0)
			{
				cullingGroup = m_FreeList.Pop();
				cullingGroup.enabled = true;
			}
			else
			{
				cullingGroup = new CullingGroup();
			}
			return cullingGroup;
		}

		public void Free(CullingGroup group)
		{
			group.enabled = false;
			m_FreeList.Push(group);
		}

		public void Cleanup()
		{
			foreach (CullingGroup free in m_FreeList)
			{
				free.Dispose();
			}
			m_FreeList.Clear();
		}
	}
}
