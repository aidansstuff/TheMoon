using System;

namespace Unity.Collections
{
	[Obsolete("FixedListInt4096DebugView is deprecated. (UnityUpgradable) -> FixedList4096BytesDebugView<int>", true)]
	internal sealed class FixedListInt4096DebugView
	{
		private FixedList4096Bytes<int> m_List;

		public int[] Items => m_List.ToArray();

		public FixedListInt4096DebugView(FixedList4096Bytes<int> list)
		{
			m_List = list;
		}
	}
}
