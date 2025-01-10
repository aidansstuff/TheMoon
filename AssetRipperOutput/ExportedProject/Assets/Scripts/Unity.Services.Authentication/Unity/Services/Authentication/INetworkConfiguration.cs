namespace Unity.Services.Authentication
{
	internal interface INetworkConfiguration
	{
		int Retries { get; }

		int Timeout { get; }
	}
}
