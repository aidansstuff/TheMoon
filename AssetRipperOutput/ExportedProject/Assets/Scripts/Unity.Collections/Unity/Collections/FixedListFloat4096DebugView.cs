using System;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat4096DebugView is deprecated. (UnityUpgradable) -> FixedList4096BytesDebugView<float>", true)]
	internal sealed class FixedListFloat4096DebugView
	{
		private FixedList4096Bytes<float> m_List;

		public float[] Items => m_List.ToArray();

		public FixedListFloat4096DebugView(FixedList4096Bytes<float> list)
		{
			m_List = list;
		}
	}
}
