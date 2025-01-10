using System;

namespace Unity.Networking.Transport
{
	public struct NetworkInterfaceSendHandle
	{
		public IntPtr data;

		public int capacity;

		public int size;

		public int id;

		public SendHandleFlags flags;
	}
}
