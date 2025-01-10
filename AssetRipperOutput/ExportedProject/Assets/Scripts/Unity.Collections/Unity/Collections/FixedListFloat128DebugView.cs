using System;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat128DebugView is deprecated. (UnityUpgradable) -> FixedList128BytesDebugView<float>", true)]
	internal sealed class FixedListFloat128DebugView
	{
		private FixedList128Bytes<float> m_List;

		public float[] Items => m_List.ToArray();

		public FixedListFloat128DebugView(FixedList128Bytes<float> list)
		{
			m_List = list;
		}
	}
}
