using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class LinkWithExternalTokenRequest
	{
		[JsonProperty("idProvider")]
		public string IdProvider;

		[JsonProperty("token")]
		public string Token;

		[JsonProperty("forceLink")]
		public bool ForceLink;

		[Preserve]
		internal LinkWithExternalTokenRequest()
		{
		}
	}
}
