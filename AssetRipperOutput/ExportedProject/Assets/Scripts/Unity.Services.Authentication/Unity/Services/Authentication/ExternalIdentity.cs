using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class ExternalIdentity
	{
		[JsonProperty("providerId")]
		public string ProviderId;

		[JsonProperty("externalId")]
		public string ExternalId;

		[Preserve]
		public ExternalIdentity()
		{
		}
	}
}
