using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class AuthenticationErrorResponse
	{
		[JsonProperty("title")]
		public string Title;

		[JsonProperty("detail")]
		public string Detail;

		[JsonProperty("status")]
		public int Status;

		[Preserve]
		public AuthenticationErrorResponse()
		{
		}
	}
}
