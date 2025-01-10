using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class User
	{
		[JsonProperty("id")]
		public string Id;

		[JsonProperty("createdAt")]
		public string CreatedAt;

		[JsonProperty("externalIds")]
		public List<ExternalIdentity> ExternalIds;

		[JsonProperty("username")]
		[CanBeNull]
		public string Username;

		[JsonProperty("UsernameInfo")]
		[CanBeNull]
		public UsernameInfo UsernameInfo;

		[Preserve]
		public User()
		{
		}
	}
}
