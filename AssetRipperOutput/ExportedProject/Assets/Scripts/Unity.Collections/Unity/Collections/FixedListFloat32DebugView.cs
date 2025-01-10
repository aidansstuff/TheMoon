using System;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat32DebugView is deprecated. (UnityUpgradable) -> FixedList32BytesDebugView<float>", true)]
	internal sealed class FixedListFloat32DebugView
	{
		private FixedList32Bytes<float> m_List;

		public float[] Items => m_List.ToArray();

		public FixedListFloat32DebugView(FixedList32Bytes<float> list)
		{
			m_List = list;
		}
	}
}
