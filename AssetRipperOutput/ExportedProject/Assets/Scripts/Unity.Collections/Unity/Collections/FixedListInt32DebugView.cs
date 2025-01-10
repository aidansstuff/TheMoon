using System;

namespace Unity.Collections
{
	[Obsolete("FixedListInt32DebugView is deprecated. (UnityUpgradable) -> FixedList32BytesDebugView<int>", true)]
	internal sealed class FixedListInt32DebugView
	{
		private FixedList32Bytes<int> m_List;

		public int[] Items => m_List.ToArray();

		public FixedListInt32DebugView(FixedList32Bytes<int> list)
		{
			m_List = list;
		}
	}
}
