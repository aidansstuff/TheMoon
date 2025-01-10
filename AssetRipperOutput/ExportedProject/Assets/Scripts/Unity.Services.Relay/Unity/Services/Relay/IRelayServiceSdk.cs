using Unity.Services.Authentication.Internal;
using Unity.Services.Qos.Internal;
using Unity.Services.Relay.Apis.RelayAllocations;

namespace Unity.Services.Relay
{
	internal interface IRelayServiceSdk
	{
		IRelayAllocationsApiClient AllocationsApi { get; set; }

		Configuration Configuration { get; set; }

		IAccessToken AccessToken { get; set; }

		IQosResults QosResults { get; set; }
	}
	public interface IRelayServiceSDK : IRelayService
	{
	}
}
