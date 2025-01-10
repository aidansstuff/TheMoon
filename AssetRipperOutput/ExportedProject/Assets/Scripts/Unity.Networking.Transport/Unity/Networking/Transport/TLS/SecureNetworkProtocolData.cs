using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Networking.Transport.TLS
{
	internal struct SecureNetworkProtocolData
	{
		public UnsafeHashMap<NetworkInterfaceEndPoint, SecureClientState> SecureClients;

		public FixedString4096Bytes Pem;

		public FixedString4096Bytes Rsa;

		public FixedString4096Bytes RsaKey;

		public FixedString32Bytes Hostname;

		public uint Protocol;

		public uint SSLReadTimeoutMs;

		public uint SSLHandshakeTimeoutMax;

		public uint SSLHandshakeTimeoutMin;

		public uint ClientAuth;

		public long LastUpdate;

		public long LastHalfOpenPrune;
	}
}
