using System;

namespace Unity.Collections
{
	[Obsolete("FixedListInt64DebugView is deprecated. (UnityUpgradable) -> FixedList64BytesDebugView<int>", true)]
	internal sealed class FixedListInt64DebugView
	{
		private FixedList64Bytes<int> m_List;

		public int[] Items => m_List.ToArray();

		public FixedListInt64DebugView(FixedList64Bytes<int> list)
		{
			m_List = list;
		}
	}
}
