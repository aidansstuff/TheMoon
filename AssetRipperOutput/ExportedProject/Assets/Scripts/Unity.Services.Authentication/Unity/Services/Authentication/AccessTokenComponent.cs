using System;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Internal;

namespace Unity.Services.Authentication
{
	internal class AccessTokenComponent : IAccessToken, IServiceComponent
	{
		public string AccessToken { get; internal set; }

		public DateTime? ExpiryTime { get; internal set; }

		internal AccessTokenComponent()
		{
		}

		internal void Clear()
		{
			AccessToken = null;
			ExpiryTime = null;
		}
	}
}
