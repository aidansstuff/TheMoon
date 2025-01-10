using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class AppleGameCenterConfig
	{
		[JsonProperty("teamPlayerId")]
		public string TeamPlayerId;

		[JsonProperty("publicKeyUrl")]
		public string PublicKeyURL;

		[JsonProperty("salt")]
		public string Salt;

		[JsonProperty("timestamp")]
		public ulong Timestamp;

		[Preserve]
		internal AppleGameCenterConfig()
		{
		}
	}
}
