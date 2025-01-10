using System.Runtime.InteropServices;

namespace Unity.Networking.Transport
{
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkEvent
	{
		public enum Type : short
		{
			Empty = 0,
			Data = 1,
			Connect = 2,
			Disconnect = 3
		}

		[FieldOffset(0)]
		internal Type type;

		[FieldOffset(2)]
		internal short pipelineId;

		[FieldOffset(4)]
		internal int connectionId;

		[FieldOffset(8)]
		internal int status;

		[FieldOffset(8)]
		internal int offset;

		[FieldOffset(12)]
		internal int size;
	}
}
