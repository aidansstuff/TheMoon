using System.Runtime.InteropServices;

namespace Unity.Networking.Transport
{
	internal struct ProcessPacketCommand
	{
		[StructLayout(LayoutKind.Explicit)]
		public struct ProcessPacketCommandAs
		{
			public struct AsAddressUpdate
			{
				public NetworkInterfaceEndPoint NewAddress;
			}

			public struct AsConnectionAccept
			{
				public SessionIdToken ConnectionToken;
			}

			public struct AsData
			{
				public int Offset;

				public int Length;

				public byte HasPipelineByte;

				public bool HasPipeline => HasPipelineByte != 0;
			}

			public struct AsDataWithImplicitConnectionAccept
			{
				public int Offset;

				public int Length;

				public byte HasPipelineByte;

				public SessionIdToken ConnectionToken;

				public bool HasPipeline => HasPipelineByte != 0;
			}

			public struct AsProtocolStatusUpdate
			{
				public int Status;
			}

			[FieldOffset(0)]
			public AsAddressUpdate AddressUpdate;

			[FieldOffset(0)]
			public AsConnectionAccept ConnectionAccept;

			[FieldOffset(0)]
			public AsData Data;

			[FieldOffset(0)]
			public AsDataWithImplicitConnectionAccept DataWithImplicitConnectionAccept;

			[FieldOffset(0)]
			public AsProtocolStatusUpdate ProtocolStatusUpdate;
		}

		public ProcessPacketCommandType Type;

		public NetworkInterfaceEndPoint Address;

		public SessionIdToken SessionId;

		public ProcessPacketCommandAs As;
	}
}
