using System;

namespace Unity.Collections
{
	[Obsolete("FixedListInt512DebugView is deprecated. (UnityUpgradable) -> FixedList512BytesDebugView<int>", true)]
	internal sealed class FixedListInt512DebugView
	{
		private FixedList512Bytes<int> m_List;

		public int[] Items => m_List.ToArray();

		public FixedListInt512DebugView(FixedList512Bytes<int> list)
		{
			m_List = list;
		}
	}
}
