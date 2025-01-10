using System.Collections.Generic;
using Unity.Services.Authentication.Internal;
using Unity.Services.Relay.Models;
using Unity.Services.Relay.Scheduler;
using UnityEngine;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.RelayAllocations
{
	[Preserve]
	internal class JoinRelayRequest : RelayAllocationsApiBaseRequest
	{
		private string PathAndQueryParams;

		[Preserve]
		public JoinRequest JoinRequest { get; }

		[Preserve]
		public JoinRelayRequest(JoinRequest joinRequest)
		{
			JoinRequest = joinRequest;
			PathAndQueryParams = "/v1/join";
		}

		public string ConstructUrl(string requestBasePath)
		{
			return requestBasePath + PathAndQueryParams;
		}

		public byte[] ConstructBody()
		{
			return ConstructBody(JoinRequest);
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
			string[] contentTypes = new string[1] { "application/json" };
			string[] accepts = new string[2] { "application/json", "application/problem+json" };
			string value = GenerateAcceptHeader(accepts);
			if (!string.IsNullOrEmpty(value))
			{
				dictionary.Add("Accept", value);
			}
			string text = "POST";
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
