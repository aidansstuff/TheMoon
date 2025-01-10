using Unity.Services.Core.Internal;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication.Internal
{
	[RequireImplementors]
	public interface IAccessToken : IServiceComponent
	{
		string AccessToken { get; }
	}
}
