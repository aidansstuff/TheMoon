using Unity.Services.Core.Internal;
using UnityEngine.Scripting;

namespace Unity.Services.Vivox.Internal
{
	[RequireImplementors]
	public interface IVivox : IServiceComponent
	{
		void RegisterTokenProvider(IVivoxTokenProviderInternal tokenProvider);
	}
}
