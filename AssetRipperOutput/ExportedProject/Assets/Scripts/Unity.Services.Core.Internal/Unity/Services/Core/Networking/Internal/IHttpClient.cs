using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Networking.Internal
{
	internal interface IHttpClient : IServiceComponent
	{
		string GetBaseUrlFor(string serviceId);

		HttpOptions GetDefaultOptionsFor(string serviceId);

		HttpRequest CreateRequestForService(string serviceId, string resourcePath);

		IAsyncOperation<ReadOnlyHttpResponse> Send(HttpRequest request);
	}
}
