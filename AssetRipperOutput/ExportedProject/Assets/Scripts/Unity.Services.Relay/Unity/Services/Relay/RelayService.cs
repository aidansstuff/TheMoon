using System;

namespace Unity.Services.Relay
{
	public static class RelayService
	{
		private static IRelayService service;

		public static IRelayService Instance
		{
			get
			{
				if (service != null)
				{
					return service;
				}
				service = new WrappedRelayService(RelayServiceSdk.Instance ?? throw new InvalidOperationException("Attempting to call Relay Services requires initializing Core Registry. Call 'UnityServices.InitializeAsync' first!"));
				return service;
			}
		}
	}
}
