using System.Threading.Tasks;
using Unity.Services.Qos.Models;
using Unity.Services.Qos.QosDiscovery;

namespace Unity.Services.Qos.Apis.QosDiscovery
{
	internal interface IQosDiscoveryApiClient
	{
		Task<Response<QosServersResponseBody>> GetServersAsync(GetServersRequest request, Configuration operationConfiguration = null);

		Task<Response<QosServiceServersResponseBody>> GetServiceServersAsync(GetServiceServersRequest request, Configuration operationConfiguration = null);
	}
}
