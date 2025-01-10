using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication.Internal;
using Unity.Services.Qos.Http;
using Unity.Services.Qos.Models;
using Unity.Services.Qos.QosDiscovery;

namespace Unity.Services.Qos.Apis.QosDiscovery
{
	internal class QosDiscoveryApiClient : BaseApiClient, IQosDiscoveryApiClient
	{
		private IAccessToken _accessToken;

		private const int _baseTimeout = 10;

		private Configuration _configuration;

		public Configuration Configuration
		{
			get
			{
				Configuration b = new Configuration("https://qos-discovery.services.api.unity.com", 10, 4, null);
				return Configuration.MergeConfigurations(_configuration, b);
			}
			set
			{
				_configuration = value;
			}
		}

		public QosDiscoveryApiClient(IHttpClient httpClient, IAccessToken accessToken, Configuration configuration = null)
			: base(httpClient)
		{
			_configuration = configuration;
			_accessToken = accessToken;
		}

		public async Task<Response<QosServersResponseBody>> GetServersAsync(GetServersRequest request, Configuration operationConfiguration = null)
		{
			Dictionary<string, Type> statusCodeToTypeMap = new Dictionary<string, Type>
			{
				{
					"200",
					typeof(QosServersResponseBody)
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
			HttpClientResponse obj = await HttpClient.MakeRequestAsync("GET", request.ConstructUrl(configuration.BasePath), request.ConstructBody(), request.ConstructHeaders(_accessToken, configuration), configuration.RequestTimeout ?? 10);
			QosServersResponseBody result = ResponseHandler.HandleAsyncResponse<QosServersResponseBody>(obj, statusCodeToTypeMap);
			return new Response<QosServersResponseBody>(obj, result);
		}

		public async Task<Response<QosServiceServersResponseBody>> GetServiceServersAsync(GetServiceServersRequest request, Configuration operationConfiguration = null)
		{
			Dictionary<string, Type> statusCodeToTypeMap = new Dictionary<string, Type>
			{
				{
					"200",
					typeof(QosServiceServersResponseBody)
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
			HttpClientResponse obj = await HttpClient.MakeRequestAsync("GET", request.ConstructUrl(configuration.BasePath), request.ConstructBody(), request.ConstructHeaders(_accessToken, configuration), configuration.RequestTimeout ?? 10);
			QosServiceServersResponseBody result = ResponseHandler.HandleAsyncResponse<QosServiceServersResponseBody>(obj, statusCodeToTypeMap);
			return new Response<QosServiceServersResponseBody>(obj, result);
		}
	}
}
