using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class OculusConfig
	{
		[JsonProperty("userId")]
		public string UserId;

		[Preserve]
		internal OculusConfig()
		{
		}
	}
}
