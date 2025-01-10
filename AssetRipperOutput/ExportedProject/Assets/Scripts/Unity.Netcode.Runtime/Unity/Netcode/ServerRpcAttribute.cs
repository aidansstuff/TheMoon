using System;

namespace Unity.Netcode
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ServerRpcAttribute : RpcAttribute
	{
		public bool RequireOwnership = true;
	}
}
