using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class SignInWithExternalTokenRequest
	{
		[JsonProperty("idProvider")]
		public string IdProvider;

		[JsonProperty("token")]
		public string Token;

		[JsonProperty("signInOnly")]
		public bool SignInOnly;

		[Preserve]
		internal SignInWithExternalTokenRequest()
		{
		}
	}
}
