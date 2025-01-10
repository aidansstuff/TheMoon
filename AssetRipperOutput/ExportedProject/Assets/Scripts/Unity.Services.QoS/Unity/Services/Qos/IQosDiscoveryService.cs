using Unity.Services.Qos.Apis.QosDiscovery;

namespace Unity.Services.Qos
{
	internal interface IQosDiscoveryService
	{
		IQosDiscoveryApiClient QosDiscoveryApi { get; set; }

		Configuration Configuration { get; set; }
	}
}
