namespace Unity.Networking.Transport.Relay
{
	internal static class ConnectionAddressExtensions
	{
		public unsafe static ref RelayAllocationId AsRelayAllocationId(this ref NetworkInterfaceEndPoint address)
		{
			fixed (byte* ptr = address.data)
			{
				return ref *(RelayAllocationId*)ptr;
			}
		}
	}
}
