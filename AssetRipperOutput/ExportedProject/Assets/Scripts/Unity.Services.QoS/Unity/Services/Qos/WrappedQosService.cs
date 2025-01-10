using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Telemetry.Internal;
using Unity.Services.Qos.Apis.QosDiscovery;
using Unity.Services.Qos.Internal;
using Unity.Services.Qos.Models;
using Unity.Services.Qos.QosDiscovery;
using Unity.Services.Qos.Runner;

namespace Unity.Services.Qos
{
	internal class WrappedQosService : IQosService
	{
		private const string ResultLatencyMetricName = "qos_result_latency_ms";

		private const string ResultPacketLossMetricName = "qos_result_packet_loss";

		private const string MetricServiceNameLabelName = "qos_service_name";

		private const string MetricServiceRegionLabelName = "qos_service_region";

		private const string MetricClientCountryLabelName = "qos_client_country";

		private const string MetricClientRegionLabelName = "qos_client_region";

		private const string MetricClientBestResultLabelName = "qos_best_result";

		private const string MetricClientBestResultLabelTrueValue = "true";

		private IQosDiscoveryApiClient _qosDiscoveryApiClient;

		private IQosRunner _qosRunner;

		private IAccessToken _accessToken;

		private IMetrics _metrics;

		internal WrappedQosService(IQosDiscoveryApiClient qosDiscoveryApiClient, IQosRunner qosRunner, IAccessToken accessToken, IMetrics metrics)
		{
			_qosDiscoveryApiClient = qosDiscoveryApiClient;
			_qosRunner = qosRunner;
			_accessToken = accessToken;
			_metrics = metrics;
		}

		public async Task<IList<IQosResult>> GetSortedQosResultsAsync(string service, IList<string> regions)
		{
			return (await GetSortedInternalQosResultsAsync(service, regions)).Select(MapToPublicQosResult).ToList();
		}

		internal async Task<IList<Unity.Services.Qos.Internal.QosResult>> GetSortedInternalQosResultsAsync(string service, IList<string> regions)
		{
			if (string.IsNullOrEmpty(_accessToken.AccessToken))
			{
				throw new Exception("Access token not available, please sign in with the Authentication Service.");
			}
			List<string> list = regions as List<string>;
			if (list == null && regions != null)
			{
				list = new List<string>(regions);
			}
			Response<QosServersResponseBody> httpResp = await _qosDiscoveryApiClient.GetServersAsync(new GetServersRequest(list, service));
			List<QosServer> servers = httpResp.Result.Data.Servers;
			if (!servers.Any())
			{
				return new List<Unity.Services.Qos.Internal.QosResult>();
			}
			List<Unity.Services.Qos.Internal.QosResult> list2 = SortResults(await _qosRunner.MeasureQosAsync(servers));
			SendResultsMetrics(list2, service, httpResp);
			return list2;
		}

		private List<Unity.Services.Qos.Internal.QosResult> SortResults(IList<Unity.Services.Qos.Internal.QosResult> results)
		{
			return (from q in results
				orderby q.AverageLatencyMs, q.PacketLossPercent
				select q).ToList();
		}

		public async Task<IList<IQosAnnotatedResult>> GetSortedRelayQosResultsAsync(IList<string> regions)
		{
			return await GetSortedInternalServiceQosResultsAsync("relay", regions, null);
		}

		public async Task<IList<IQosAnnotatedResult>> GetSortedMultiplayQosResultsAsync(IList<string> fleet)
		{
			return await GetSortedInternalServiceQosResultsAsync("multiplay", null, fleet);
		}

		internal async Task<IList<IQosAnnotatedResult>> GetSortedInternalServiceQosResultsAsync(string service, IList<string> regions, IList<string> fleet)
		{
			if (string.IsNullOrEmpty(_accessToken.AccessToken))
			{
				throw new Exception("Access token not available, please sign in with the Authentication Service.");
			}
			List<string> list = regions as List<string>;
			if (list == null && regions != null)
			{
				list = new List<string>(regions);
			}
			List<string> list2 = fleet as List<string>;
			if (list2 == null && fleet != null)
			{
				list2 = new List<string>(fleet);
			}
			Response<QosServiceServersResponseBody> httpResp = await _qosDiscoveryApiClient.GetServiceServersAsync(new GetServiceServersRequest(service, list, list2));
			List<QosServiceServer> servers = httpResp.Result.Data.Servers;
			if (!servers.Any())
			{
				return new List<IQosAnnotatedResult>();
			}
			List<IQosAnnotatedResult> list3 = SortServiceResults(await _qosRunner.MeasureQosAsync(servers));
			SendResultsMetrics(list3.Cast<IQosResult>().ToList(), service, httpResp);
			return list3;
		}

		private List<IQosAnnotatedResult> SortServiceResults(IList<QosAnnotatedResult> results)
		{
			return (from q in (from q in results
					where q.AverageLatencyMs != int.MaxValue && (double)q.PacketLossPercent >= 0.0 && (double)q.PacketLossPercent < 1.0
					group q by q.Region).Select((Func<IGrouping<string, QosAnnotatedResult>, IQosAnnotatedResult>)((IGrouping<string, QosAnnotatedResult> q) => new QosResult(q.Key, (int)Math.Round(q.Select((QosAnnotatedResult x) => x.AverageLatencyMs).Average()), q.Select((QosAnnotatedResult x) => x.PacketLossPercent).Average(), q.Select((QosAnnotatedResult x) => x.Annotations).First()))).ToList()
				orderby q.AverageLatencyMs, q.PacketLossPercent
				select q).ToList();
		}

		private void SendResultsMetrics(IList<Unity.Services.Qos.Internal.QosResult> sortedResults, string service, Response discoveryResponse)
		{
			if (sortedResults.Count != 0)
			{
				for (int i = 0; i < sortedResults.Count; i++)
				{
					Unity.Services.Qos.Internal.QosResult qosResult = sortedResults[i];
					SendResultMetrics(service, discoveryResponse, qosResult.Region, qosResult.AverageLatencyMs, qosResult.PacketLossPercent, i == 0);
				}
			}
		}

		private void SendResultsMetrics(IList<IQosResult> sortedResults, string service, Response discoveryResponse)
		{
			if (sortedResults.Count != 0)
			{
				for (int i = 0; i < sortedResults.Count; i++)
				{
					IQosResult qosResult = sortedResults[i];
					SendResultMetrics(service, discoveryResponse, qosResult.Region, qosResult.AverageLatencyMs, qosResult.PacketLossPercent, i == 0);
				}
			}
		}

		private void SendResultMetrics(string service, Response discoveryResponse, string region, int averageLatencyMs, float packetLossPercent, bool isBest)
		{
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("qos_service_name", service);
			dictionary.Add("qos_service_region", region);
			if (discoveryResponse.Headers.TryGetValue("X-Client-Country", out var value))
			{
				dictionary.Add("qos_client_country", value);
			}
			if (discoveryResponse.Headers.TryGetValue("X-Client-Region", out var value2))
			{
				dictionary.Add("qos_client_region", value2);
			}
			if (isBest)
			{
				dictionary.Add("qos_best_result", "true");
			}
			_metrics.SendHistogramMetric("qos_result_latency_ms", averageLatencyMs, dictionary);
			_metrics.SendHistogramMetric("qos_result_packet_loss", packetLossPercent, dictionary);
		}

		private IQosResult MapToPublicQosResult(Unity.Services.Qos.Internal.QosResult internalQosResult)
		{
			return new QosResult(internalQosResult.Region, internalQosResult.AverageLatencyMs, internalQosResult.PacketLossPercent);
		}
	}
}
