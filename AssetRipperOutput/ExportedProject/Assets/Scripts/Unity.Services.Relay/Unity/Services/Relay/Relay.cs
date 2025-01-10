namespace Unity.Services.Relay
{
	public static class Relay
	{
		public static IRelayServiceSDK Instance => (IRelayServiceSDK)RelayService.Instance;
	}
}
