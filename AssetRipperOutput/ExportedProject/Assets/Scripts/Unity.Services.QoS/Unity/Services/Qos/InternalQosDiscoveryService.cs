using Unity.Services.Authentication.Internal;
using Unity.Services.Qos.Apis.QosDiscovery;
using Unity.Services.Qos.Http;

namespace Unity.Services.Qos
{
	internal class InternalQosDiscoveryService : IQosDiscoveryService
	{
		private const int RequestTimeout = 10;

		private const int NumRetries = 4;

		public IQosDiscoveryApiClient QosDiscoveryApi { get; set; }

		public Configuration Configuration { get; set; }

		internal InternalQosDiscoveryService(string host, HttpClient httpClient, IAccessToken accessToken = null)
		{
			Configuration = new Configuration(host, 10, 4, null);
			QosDiscoveryApi = new QosDiscoveryApiClient(httpClient, accessToken, Configuration);
		}
	}
}
