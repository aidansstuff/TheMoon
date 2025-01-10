using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class UnlinkRequest
	{
		[JsonProperty("idProvider")]
		public string IdProvider;

		[JsonProperty("externalId")]
		public string ExternalId;

		[Preserve]
		internal UnlinkRequest()
		{
		}
	}
}
