using System;

namespace Unity.Collections
{
	[Obsolete("FixedListByte32DebugView is deprecated. (UnityUpgradable) -> FixedList32BytesDebugView<byte>", true)]
	internal sealed class FixedListByte32DebugView
	{
		private FixedList32Bytes<byte> m_List;

		public byte[] Items => m_List.ToArray();

		public FixedListByte32DebugView(FixedList32Bytes<byte> list)
		{
			m_List = list;
		}
	}
}
