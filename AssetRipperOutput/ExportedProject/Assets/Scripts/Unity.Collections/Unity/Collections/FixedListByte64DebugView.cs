using System;

namespace Unity.Collections
{
	[Obsolete("FixedListByte64DebugView is deprecated. (UnityUpgradable) -> FixedList64BytesDebugView<byte>", true)]
	internal sealed class FixedListByte64DebugView
	{
		private FixedList64Bytes<byte> m_List;

		public byte[] Items => m_List.ToArray();

		public FixedListByte64DebugView(FixedList64Bytes<byte> list)
		{
			m_List = list;
		}
	}
}
