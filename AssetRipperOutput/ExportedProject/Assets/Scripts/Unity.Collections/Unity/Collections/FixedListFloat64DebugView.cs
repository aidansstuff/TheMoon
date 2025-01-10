using System;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat64DebugView is deprecated. (UnityUpgradable) -> FixedList64BytesDebugView<float>", true)]
	internal sealed class FixedListFloat64DebugView
	{
		private FixedList64Bytes<float> m_List;

		public float[] Items => m_List.ToArray();

		public FixedListFloat64DebugView(FixedList64Bytes<float> list)
		{
			m_List = list;
		}
	}
}
