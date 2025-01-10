using System;

namespace Unity.Collections
{
	[Obsolete("FixedListInt128DebugView is deprecated. (UnityUpgradable) -> FixedList128BytesDebugView<int>", true)]
	internal sealed class FixedListInt128DebugView
	{
		private FixedList128Bytes<int> m_List;

		public int[] Items => m_List.ToArray();

		public FixedListInt128DebugView(FixedList128Bytes<int> list)
		{
			m_List = list;
		}
	}
}
