using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class SessionTokenRequest
	{
		[JsonProperty("sessionToken")]
		public string SessionToken;

		[Preserve]
		public SessionTokenRequest()
		{
		}
	}
}
