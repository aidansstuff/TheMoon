using System;

namespace Unity.Networking.Transport.Protocols
{
	internal struct UdpCHeader
	{
		[Flags]
		public enum HeaderFlags : byte
		{
			HasConnectToken = 1,
			HasPipeline = 2
		}

		public const int Length = 10;

		public byte Type;

		public HeaderFlags Flags;

		public SessionIdToken SessionToken;
	}
}
