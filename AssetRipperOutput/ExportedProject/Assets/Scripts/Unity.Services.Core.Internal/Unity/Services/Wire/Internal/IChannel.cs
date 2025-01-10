using System;
using System.Threading.Tasks;

namespace Unity.Services.Wire.Internal
{
	public interface IChannel : IDisposable
	{
		event Action<string> MessageReceived;

		event Action<byte[]> BinaryMessageReceived;

		event Action KickReceived;

		event Action<SubscriptionState> NewStateReceived;

		event Action<string> ErrorReceived;

		Task SubscribeAsync();

		Task UnsubscribeAsync();
	}
}
