using System;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat512DebugView is deprecated. (UnityUpgradable) -> FixedList512BytesDebugView<float>", true)]
	internal sealed class FixedListFloat512DebugView
	{
		private FixedList512Bytes<float> m_List;

		public float[] Items => m_List.ToArray();

		public FixedListFloat512DebugView(FixedList512Bytes<float> list)
		{
			m_List = list;
		}
	}
}
