using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Internal;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication.Server.Internal
{
	[RequireImplementors]
	public interface IServerAccessToken : IAccessToken, IServiceComponent, IAccessTokenObserver
	{
	}
}
