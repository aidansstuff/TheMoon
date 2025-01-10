namespace Unity.Services.Authentication
{
	internal class NetworkConfiguration : INetworkConfiguration
	{
		private const int k_DefaultRetries = 2;

		private const int k_DefaultTimeout = 10;

		public int Retries { get; set; } = 2;


		public int Timeout { get; set; } = 10;

	}
}
