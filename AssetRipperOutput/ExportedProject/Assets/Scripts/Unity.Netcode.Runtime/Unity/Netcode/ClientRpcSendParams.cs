using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Netcode
{
	public struct ClientRpcSendParams
	{
		public IReadOnlyList<ulong> TargetClientIds;

		public NativeArray<ulong>? TargetClientIdsNativeArray;
	}
}
