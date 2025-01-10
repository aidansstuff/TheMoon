using Unity.Services.Core.Internal;
using UnityEngine.Scripting;

namespace Unity.Services.Wire.Internal
{
	[RequireImplementors]
	public interface IWire : IServiceComponent
	{
		IChannel CreateChannel(IChannelTokenProvider tokenProvider);
	}
}
