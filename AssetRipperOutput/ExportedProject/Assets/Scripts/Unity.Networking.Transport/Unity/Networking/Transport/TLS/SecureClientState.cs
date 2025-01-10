using Unity.TLS.LowLevel;

namespace Unity.Networking.Transport.TLS
{
	internal struct SecureClientState
	{
		public unsafe Binding.unitytls_client* ClientPtr;

		public unsafe Binding.unitytls_client_config* ClientConfig;

		public SessionIdToken ReceiveToken;

		public long LastHandshakeUpdate;
	}
}
