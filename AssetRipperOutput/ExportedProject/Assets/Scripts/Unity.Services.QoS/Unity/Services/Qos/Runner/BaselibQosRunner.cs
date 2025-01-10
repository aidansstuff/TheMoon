using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Unity.Jobs;
using Unity.Networking.QoS;
using Unity.Services.Qos.Internal;
using Unity.Services.Qos.Models;
using UnityEngine;

namespace Unity.Services.Qos.Runner
{
	internal class BaselibQosRunner : IQosRunner
	{
		private QosJobProvider _qosJobProvider = (IList<UcgQosServer> servers, string title) => new QosJob(servers, title, 5u, 10000uL, 500uL);

		private DnsResolver _dnsResolver = Dns.GetHostAddressesAsync;

		public BaselibQosRunner(QosJobProvider qosJobProvider = null, DnsResolver dnsResolver = null)
		{
			if (qosJobProvider != null)
			{
				_qosJobProvider = qosJobProvider;
			}
			if (dnsResolver != null)
			{
				_dnsResolver = dnsResolver;
			}
		}

		public async Task<List<Unity.Services.Qos.Internal.QosResult>> MeasureQosAsync(IList<QosServer> servers)
		{
			List<UcgQosServer> convertedServers = (from s in await Task.WhenAll(servers.Select(ToUcgFormat))
				where s.HasValue
				select s.Value).ToList();
			List<Unity.Services.Qos.Internal.QosResult> results = new List<Unity.Services.Qos.Internal.QosResult>();
			IQosJob qosJob = await RunQosJob(convertedServers);
			if (servers.Count() == qosJob.QosResults.Count())
			{
				results = ParseResults(qosJob.QosResults, servers);
			}
			qosJob.Dispose();
			qosJob.QosResults.Dispose();
			return results;
		}

		public async Task<List<QosAnnotatedResult>> MeasureQosAsync(IList<QosServiceServer> servers)
		{
			List<UcgQosServer> convertedServers = (from s in await Task.WhenAll(servers.Select(ToUcgFormat))
				where s.HasValue
				select s.Value).ToList();
			List<QosAnnotatedResult> results = new List<QosAnnotatedResult>();
			IQosJob qosJob = await RunQosJob(convertedServers);
			if (servers.Count() == qosJob.QosResults.Count())
			{
				results = ParseResults(qosJob.QosResults, servers);
			}
			qosJob.Dispose();
			qosJob.QosResults.Dispose();
			return results;
		}

		private async Task<IQosJob> RunQosJob(List<UcgQosServer> convertedServers)
		{
			string title = "QoS request";
			IQosJob job = _qosJobProvider(convertedServers, title);
			JobHandle handle = job.Schedule<QosJob>();
			while (!handle.IsCompleted)
			{
				await Task.Yield();
			}
			handle.Complete();
			return job;
		}

		private async Task<UcgQosServer?> ToUcgFormat(QosServer server)
		{
			string serverEndpoint = server.Endpoints[0];
			string region = server.Region;
			return await ToUcgFormat(serverEndpoint, region);
		}

		private async Task<UcgQosServer?> ToUcgFormat(QosServiceServer server)
		{
			string serverEndpoint = server.Endpoints[0];
			string region = server.Region;
			return await ToUcgFormat(serverEndpoint, region);
		}

		private async Task<UcgQosServer?> ToUcgFormat(string serverEndpoint, string serverRegion)
		{
			if (!Uri.TryCreate("udp://" + serverEndpoint, UriKind.Absolute, out var uri))
			{
				Debug.LogError("Could not create address from endpoint: '" + serverEndpoint + "'");
				return null;
			}
			if (uri.Port == -1)
			{
				Debug.LogError("Missing or invalid port in endpoint: '" + serverEndpoint + "'");
				return null;
			}
			IPAddress[] array = await _dnsResolver(uri.Host);
			if (array.Length == 0)
			{
				Debug.LogError("No addresses could be resolved for host " + uri.Host);
				return null;
			}
			IPAddress iPAddress = array[0];
			UcgQosServer ucgQosServer = default(UcgQosServer);
			ucgQosServer.regionid = serverRegion;
			ucgQosServer.ipv6 = null;
			ucgQosServer.port = Convert.ToUInt16(uri.Port);
			ucgQosServer.BackoffUntilUtc = default(DateTime);
			UcgQosServer value = ucgQosServer;
			if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				value.ipv4 = iPAddress.ToString();
			}
			else if (iPAddress.AddressFamily == AddressFamily.InterNetworkV6)
			{
				value.ipv6 = iPAddress.ToString();
			}
			return value;
		}

		private static List<Unity.Services.Qos.Internal.QosResult> ParseResults(IEnumerable<InternalQosResult> ucgResults, IEnumerable<QosServer> servers)
		{
			List<Unity.Services.Qos.Internal.QosResult> list = new List<Unity.Services.Qos.Internal.QosResult>();
			using IEnumerator<QosServer> enumerator2 = servers.GetEnumerator();
			foreach (InternalQosResult ucgResult in ucgResults)
			{
				enumerator2.MoveNext();
				if (enumerator2.Current == null)
				{
					break;
				}
				int averageLatencyMs = (int)((ucgResult.AverageLatencyMs > int.MaxValue) ? int.MaxValue : ucgResult.AverageLatencyMs);
				list.Add(new Unity.Services.Qos.Internal.QosResult
				{
					Region = enumerator2.Current.Region,
					AverageLatencyMs = averageLatencyMs,
					PacketLossPercent = ucgResult.PacketLoss
				});
			}
			return list;
		}

		private static List<QosAnnotatedResult> ParseResults(IEnumerable<InternalQosResult> ucgResults, IEnumerable<QosServiceServer> servers)
		{
			List<QosAnnotatedResult> list = new List<QosAnnotatedResult>();
			using IEnumerator<QosServiceServer> enumerator2 = servers.GetEnumerator();
			foreach (InternalQosResult ucgResult in ucgResults)
			{
				enumerator2.MoveNext();
				if (enumerator2.Current == null)
				{
					break;
				}
				int averageLatencyMs = (int)((ucgResult.AverageLatencyMs > int.MaxValue) ? int.MaxValue : ucgResult.AverageLatencyMs);
				list.Add(new QosAnnotatedResult
				{
					Region = enumerator2.Current.Region,
					AverageLatencyMs = averageLatencyMs,
					PacketLossPercent = ucgResult.PacketLoss,
					Annotations = enumerator2.Current.Annotations
				});
			}
			return list;
		}
	}
}
