using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class LinkResponse
	{
		[JsonProperty("user")]
		public User User;

		[Preserve]
		public LinkResponse()
		{
		}
	}
}
