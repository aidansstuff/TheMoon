using System;
using Unity.Services.Core.Internal;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication.Internal
{
	[RequireImplementors]
	public interface IAccessTokenObserver : IServiceComponent
	{
		event Action<string> AccessTokenChanged;
	}
}
