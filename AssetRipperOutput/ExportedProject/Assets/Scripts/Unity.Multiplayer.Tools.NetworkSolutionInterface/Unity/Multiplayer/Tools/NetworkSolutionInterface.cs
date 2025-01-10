namespace Unity.Multiplayer.Tools
{
	internal static class NetworkSolutionInterface
	{
		private static NetworkSolutionInterfaceParameters s_Parameters;

		internal static INetworkObjectProvider NetworkObjectProvider => s_Parameters.NetworkObjectProvider;

		public static void SetInterface(NetworkSolutionInterfaceParameters parameters)
		{
			ref INetworkObjectProvider networkObjectProvider = ref parameters.NetworkObjectProvider;
			if (networkObjectProvider == null)
			{
				networkObjectProvider = new NullNetworkObjectProvider();
			}
			s_Parameters = parameters;
		}
	}
}
