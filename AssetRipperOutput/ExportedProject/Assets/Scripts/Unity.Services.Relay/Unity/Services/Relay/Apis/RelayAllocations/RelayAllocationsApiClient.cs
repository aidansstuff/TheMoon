using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication.Internal;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Services.Relay.RelayAllocations;

namespace Unity.Services.Relay.Apis.RelayAllocations
{
	internal class RelayAllocationsApiClient : BaseApiClient, IRelayAllocationsApiClient
	{
		private IAccessToken _accessToken;

		private const int _baseTimeout = 10;

		private Configuration _configuration;

		public Configuration Configuration
		{
			get
			{
				Configuration b = new Configuration("https://relay-allocations.services.api.unity.com", 10, 4, null);
				return Configuration.MergeConfigurations(_configuration, b);
			}
			set
			{
				_configuration = value;
			}
		}

		public RelayAllocationsApiClient(IHttpClient httpClient, IAccessToken accessToken, Configuration configuration = null)
			: base(httpClient)
		{
			_configuration = configuration;
			_accessToken = accessToken;
		}

		public async Task<Response<AllocateResponseBody>> CreateAllocationAsync(CreateAllocationRequest request, Configuration operationConfiguration = null)
		{
			Dictionary<string, Type> statusCodeToTypeMap = new Dictionary<string, Type>
			{
				{
					"201",
					typeof(AllocateResponseBody)
				},
				{
					"400",
					typeof(ErrorResponseBody)
				},
				{
					"401",
					typeof(ErrorResponseBody)
				},
				{
					"403",
					typeof(ErrorResponseBody)
				},
				{
					"429",
					typeof(ErrorResponseBody)
				},
				{
					"500",
					typeof(ErrorResponseBody)
				},
				{
					"503",
					typeof(ErrorResponseBody)
				}
			};
			Configuration configuration = Configuration.MergeConfigurations(operationConfiguration, Configuration);
			HttpClientResponse obj = await HttpClient.MakeRequestAsync("POST", request.ConstructUrl(configuration.BasePath), request.ConstructBody(), request.ConstructHeaders(_accessToken, configuration), configuration.RequestTimeout ?? 10);
			AllocateResponseBody result = ResponseHandler.HandleAsyncResponse<AllocateResponseBody>(obj, statusCodeToTypeMap);
			return new Response<AllocateResponseBody>(obj, result);
		}

		public async Task<Response<JoinCodeResponseBody>> CreateJoincodeAsync(CreateJoincodeRequest request, Configuration operationConfiguration = null)
		{
			Dictionary<string, Type> statusCodeToTypeMap = new Dictionary<string, Type>
			{
				{
					"200",
					typeof(JoinCodeResponseBody)
				},
				{
					"201",
					typeof(JoinCodeResponseBody)
				},
				{
					"400",
					typeof(ErrorResponseBody)
				},
				{
					"401",
					typeof(ErrorResponseBody)
				},
				{
					"403",
					typeof(ErrorResponseBody)
				},
				{
					"429",
					typeof(ErrorResponseBody)
				},
				{
					"500",
					typeof(ErrorResponseBody)
				}
			};
			Configuration configuration = Configuration.MergeConfigurations(operationConfiguration, Configuration);
			HttpClientResponse obj = await HttpClient.MakeRequestAsync("POST", request.ConstructUrl(configuration.BasePath), request.ConstructBody(), request.ConstructHeaders(_accessToken, configuration), configuration.RequestTimeout ?? 10);
			JoinCodeResponseBody result = ResponseHandler.HandleAsyncResponse<JoinCodeResponseBody>(obj, statusCodeToTypeMap);
			return new Response<JoinCodeResponseBody>(obj, result);
		}

		public async Task<Response<JoinResponseBody>> JoinRelayAsync(JoinRelayRequest request, Configuration operationConfiguration = null)
		{
			Dictionary<string, Type> statusCodeToTypeMap = new Dictionary<string, Type>
			{
				{
					"200",
					typeof(JoinResponseBody)
				},
				{
					"400",
					typeof(ErrorResponseBody)
				},
				{
					"401",
					typeof(ErrorResponseBody)
				},
				{
					"403",
					typeof(ErrorResponseBody)
				},
				{
					"404",
					typeof(ErrorResponseBody)
				},
				{
					"429",
					typeof(ErrorResponseBody)
				},
				{
					"500",
					typeof(ErrorResponseBody)
				}
			};
			Configuration configuration = Configuration.MergeConfigurations(operationConfiguration, Configuration);
			HttpClientResponse obj = await HttpClient.MakeRequestAsync("POST", request.ConstructUrl(configuration.BasePath), request.ConstructBody(), request.ConstructHeaders(_accessToken, configuration), configuration.RequestTimeout ?? 10);
			JoinResponseBody result = ResponseHandler.HandleAsyncResponse<JoinResponseBody>(obj, statusCodeToTypeMap);
			return new Response<JoinResponseBody>(obj, result);
		}

		public async Task<Response<RegionsResponseBody>> ListRegionsAsync(ListRegionsRequest request, Configuration operationConfiguration = null)
		{
			Dictionary<string, Type> statusCodeToTypeMap = new Dictionary<string, Type>
			{
				{
					"200",
					typeof(RegionsResponseBody)
				},
				{
					"400",
					typeof(ErrorResponseBody)
				},
				{
					"401",
					typeof(ErrorResponseBody)
				},
				{
					"403",
					typeof(ErrorResponseBody)
				},
				{
					"429",
					typeof(ErrorResponseBody)
				},
				{
					"500",
					typeof(ErrorResponseBody)
				}
			};
			Configuration configuration = Configuration.MergeConfigurations(operationConfiguration, Configuration);
			HttpClientResponse obj = await HttpClient.MakeRequestAsync("GET", request.ConstructUrl(configuration.BasePath), request.ConstructBody(), request.ConstructHeaders(_accessToken, configuration), configuration.RequestTimeout ?? 10);
			RegionsResponseBody result = ResponseHandler.HandleAsyncResponse<RegionsResponseBody>(obj, statusCodeToTypeMap);
			return new Response<RegionsResponseBody>(obj, result);
		}
	}
}
