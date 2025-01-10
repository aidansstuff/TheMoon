using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class UsernamePasswordRequest
	{
		[JsonProperty("username")]
		public string Username;

		[JsonProperty("password")]
		public string Password;

		[Preserve]
		internal UsernamePasswordRequest()
		{
		}
	}
}
