using System;

namespace Unity.Collections
{
	[Obsolete("FixedListByte4096DebugView is deprecated. (UnityUpgradable) -> FixedList4096BytesDebugView<byte>", true)]
	internal sealed class FixedListByte4096DebugView
	{
		private FixedList4096Bytes<byte> m_List;

		public byte[] Items => m_List.ToArray();

		public FixedListByte4096DebugView(FixedList4096Bytes<byte> list)
		{
			m_List = list;
		}
	}
}
