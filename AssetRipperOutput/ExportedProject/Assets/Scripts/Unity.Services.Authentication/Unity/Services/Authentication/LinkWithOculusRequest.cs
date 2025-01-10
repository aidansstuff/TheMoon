using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class LinkWithOculusRequest : LinkWithExternalTokenRequest
	{
		[JsonProperty("oculusConfig")]
		public OculusConfig OculusConfig;

		[Preserve]
		internal LinkWithOculusRequest()
		{
		}
	}
}
