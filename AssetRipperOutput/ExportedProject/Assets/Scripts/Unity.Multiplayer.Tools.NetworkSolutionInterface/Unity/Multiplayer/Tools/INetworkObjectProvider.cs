using UnityEngine;

namespace Unity.Multiplayer.Tools
{
	internal interface INetworkObjectProvider
	{
		Object GetNetworkObject(ulong networkObjectId);
	}
}
