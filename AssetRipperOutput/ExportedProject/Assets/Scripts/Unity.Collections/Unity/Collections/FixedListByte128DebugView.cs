using System;

namespace Unity.Collections
{
	[Obsolete("FixedListByte128DebugView is deprecated. (UnityUpgradable) -> FixedList128BytesDebugView<byte>", true)]
	internal sealed class FixedListByte128DebugView
	{
		private FixedList128Bytes<byte> m_List;

		public byte[] Items => m_List.ToArray();

		public FixedListByte128DebugView(FixedList128Bytes<byte> list)
		{
			m_List = list;
		}
	}
}
