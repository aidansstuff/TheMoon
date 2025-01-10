using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class SignInWithSteamRequest : SignInWithExternalTokenRequest
	{
		[JsonProperty("steamConfig")]
		public SteamConfig SteamConfig;

		[Preserve]
		internal SignInWithSteamRequest()
		{
		}
	}
}
