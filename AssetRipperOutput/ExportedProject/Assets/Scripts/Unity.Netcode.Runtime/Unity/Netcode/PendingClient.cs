using UnityEngine;

namespace Unity.Netcode
{
	public class PendingClient
	{
		public enum State
		{
			PendingConnection = 0,
			PendingApproval = 1
		}

		internal Coroutine ApprovalCoroutine;

		public ulong ClientId { get; internal set; }

		public State ConnectionState { get; internal set; }
	}
}
