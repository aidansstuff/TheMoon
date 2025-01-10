using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	internal class UsernameInfo
	{
		[JsonProperty("username")]
		public string Username;

		[JsonProperty("createdAt")]
		public string CreatedAt;

		[JsonProperty("lastLoginAt")]
		public string LastLoginAt;

		[JsonProperty("passwordUpdatedAt")]
		public string PasswordUpdatedAt;

		[Preserve]
		public UsernameInfo()
		{
		}
	}
}
