using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	[Serializable]
	internal class SignInWithAppleGameCenterRequest : SignInWithExternalTokenRequest
	{
		[JsonProperty("appleGameCenterConfig")]
		public AppleGameCenterConfig AppleGameCenterConfig;

		[Preserve]
		internal SignInWithAppleGameCenterRequest()
		{
		}
	}
}
