using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	internal class LinkWithSteamRequest : LinkWithExternalTokenRequest
	{
		[JsonProperty("steamConfig")]
		public SteamConfig SteamConfig;

		[Preserve]
		internal LinkWithSteamRequest()
		{
		}
	}
}
