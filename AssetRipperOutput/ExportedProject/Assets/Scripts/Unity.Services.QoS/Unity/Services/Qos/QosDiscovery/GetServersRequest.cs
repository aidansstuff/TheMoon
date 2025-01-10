using System.Collections.Generic;
using System.Linq;
using Unity.Services.Authentication.Internal;
using Unity.Services.Qos.Scheduler;
using UnityEngine;
using UnityEngine.Scripting;

namespace Unity.Services.Qos.QosDiscovery
{
	[Preserve]
	internal class GetServersRequest : QosDiscoveryApiBaseRequest
	{
		private string PathAndQueryParams;

		[Preserve]
		public List<string> Region { get; }

		[Preserve]
		public string Service { get; }

		[Preserve]
		public GetServersRequest(List<string> region = null, string service = null)
		{
			Region = region;
			Service = service;
			PathAndQueryParams = "/v1/servers";
			List<string> list = new List<string>();
			if (Region != null)
			{
				List<string> values = Region.Select((string v) => v.ToString()).ToList();
				list = AddParamsToQueryParams(list, "region", values, "form", explode: true);
			}
			if (!string.IsNullOrEmpty(Service))
			{
				list = AddParamsToQueryParams(list, "service", Service);
			}
			if (list.Count > 0)
			{
				PathAndQueryParams = PathAndQueryParams + "?" + string.Join("&", list);
			}
		}

		public string ConstructUrl(string requestBasePath)
		{
			return requestBasePath + PathAndQueryParams;
		}

		public byte[] ConstructBody()
		{
			return null;
		}

		public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken, Configuration operationConfiguration = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(accessToken.AccessToken))
			{
				dictionary.Add("authorization", "Bearer " + accessToken.AccessToken);
			}
			dictionary.Add("Unity-Client-Version", Application.unityVersion);
			dictionary.Add("Unity-Client-Mode", EngineStateHelper.IsPlaying ? "play" : "edit");
			string[] contentTypes = new string[0];
			string[] accepts = new string[2] { "application/json", "application/problem+json" };
			string value = GenerateAcceptHeader(accepts);
			if (!string.IsNullOrEmpty(value))
			{
				dictionary.Add("Accept", value);
			}
			string text = "GET";
			string value2 = GenerateContentTypeHeader(contentTypes);
			if (!string.IsNullOrEmpty(value2))
			{
				dictionary.Add("Content-Type", value2);
			}
			else if (text == "POST" || text == "PATCH")
			{
				dictionary.Add("Content-Type", "application/json");
			}
			if (operationConfiguration != null && operationConfiguration.Headers != null)
			{
				foreach (KeyValuePair<string, string> header in operationConfiguration.Headers)
				{
					dictionary[header.Key] = header.Value;
				}
			}
			return dictionary;
		}
	}
}
