using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class SignInWithOculusRequest : SignInWithExternalTokenRequest
	{
		[JsonProperty("oculusConfig")]
		public OculusConfig OculusConfig;

		[Preserve]
		internal SignInWithOculusRequest()
		{
		}
	}
}
