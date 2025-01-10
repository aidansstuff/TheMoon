using Unity.Collections;

namespace Unity.Networking.Transport.TLS
{
	public struct SecureNetworkProtocolParameter : INetworkParameter
	{
		public FixedString4096Bytes Pem;

		public FixedString4096Bytes Rsa;

		public FixedString4096Bytes RsaKey;

		public FixedString32Bytes Hostname;

		public SecureTransportProtocol Protocol;

		public SecureClientAuthPolicy ClientAuthenticationPolicy;

		public uint SSLReadTimeoutMs;

		public uint SSLHandshakeTimeoutMax;

		public uint SSLHandshakeTimeoutMin;

		public bool Validate()
		{
			return true;
		}
	}
}
