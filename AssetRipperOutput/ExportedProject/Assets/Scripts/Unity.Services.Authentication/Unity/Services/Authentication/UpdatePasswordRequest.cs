using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class UpdatePasswordRequest
	{
		[JsonProperty("password")]
		public string Password;

		[JsonProperty("newPassword")]
		public string NewPassword;

		[Preserve]
		internal UpdatePasswordRequest()
		{
		}
	}
}
