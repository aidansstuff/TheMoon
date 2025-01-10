using System;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Configuration
{
	internal class ExternalUserId : IExternalUserId, IServiceComponent
	{
		public string UserId => UnityServices.ExternalUserIdProperty.UserId;

		public event Action<string> UserIdChanged
		{
			add
			{
				UnityServices.ExternalUserIdProperty.UserIdChanged += value;
			}
			remove
			{
				UnityServices.ExternalUserIdProperty.UserIdChanged -= value;
			}
		}
	}
}
