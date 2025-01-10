using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class PlayerInfoResponse
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

		[JsonProperty("usernamepassword")]
		[CanBeNull]
		public UsernameInfo UsernamePassword;

		[Preserve]
		public PlayerInfoResponse()
		{
		}
	}
}
