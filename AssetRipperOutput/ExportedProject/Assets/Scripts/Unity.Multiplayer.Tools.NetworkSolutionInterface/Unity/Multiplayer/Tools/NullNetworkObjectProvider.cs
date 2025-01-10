using UnityEngine;

namespace Unity.Multiplayer.Tools
{
	internal class NullNetworkObjectProvider : INetworkObjectProvider
	{
		Object INetworkObjectProvider.GetNetworkObject(ulong networkObjectId)
		{
			return null;
		}
	}
}
