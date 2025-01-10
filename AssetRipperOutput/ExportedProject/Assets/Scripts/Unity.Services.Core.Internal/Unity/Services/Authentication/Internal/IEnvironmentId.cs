using Unity.Services.Core.Internal;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication.Internal
{
	[RequireImplementors]
	public interface IEnvironmentId : IServiceComponent
	{
		string EnvironmentId { get; }
	}
}
