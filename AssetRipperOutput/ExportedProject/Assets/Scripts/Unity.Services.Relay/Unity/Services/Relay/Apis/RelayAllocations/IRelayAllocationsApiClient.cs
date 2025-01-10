using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Unity.Services.Relay.RelayAllocations;

namespace Unity.Services.Relay.Apis.RelayAllocations
{
	internal interface IRelayAllocationsApiClient
	{
		Task<Response<AllocateResponseBody>> CreateAllocationAsync(CreateAllocationRequest request, Configuration operationConfiguration = null);

		Task<Response<JoinCodeResponseBody>> CreateJoincodeAsync(CreateJoincodeRequest request, Configuration operationConfiguration = null);

		Task<Response<JoinResponseBody>> JoinRelayAsync(JoinRelayRequest request, Configuration operationConfiguration = null);

		Task<Response<RegionsResponseBody>> ListRegionsAsync(ListRegionsRequest request, Configuration operationConfiguration = null);
	}
}
