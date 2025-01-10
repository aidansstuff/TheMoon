using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class UnlinkResponse
	{
		[JsonProperty("user")]
		public User User;

		[Preserve]
		public UnlinkResponse()
		{
		}
	}
}
