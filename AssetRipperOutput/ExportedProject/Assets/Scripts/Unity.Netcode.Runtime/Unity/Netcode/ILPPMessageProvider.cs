using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Unity.Netcode
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct ILPPMessageProvider : INetworkMessageProvider
	{
		internal static readonly List<NetworkMessageManager.MessageWithHandler> __network_message_types = new List<NetworkMessageManager.MessageWithHandler>();

		public List<NetworkMessageManager.MessageWithHandler> GetMessages()
		{
			return __network_message_types;
		}
	}
}
