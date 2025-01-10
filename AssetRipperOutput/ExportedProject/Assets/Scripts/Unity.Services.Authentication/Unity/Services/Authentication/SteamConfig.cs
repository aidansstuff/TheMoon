using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	internal class SteamConfig
	{
		[JsonProperty("identity")]
		public string identity;

		[Preserve]
		internal SteamConfig()
		{
		}
	}
}
