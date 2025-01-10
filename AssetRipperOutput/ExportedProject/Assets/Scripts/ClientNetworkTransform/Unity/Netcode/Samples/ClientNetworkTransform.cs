using Unity.Netcode.Components;
using UnityEngine;

namespace Unity.Netcode.Samples
{
	[DisallowMultipleComponent]
	public class ClientNetworkTransform : NetworkTransform
	{
		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			base.CanCommitToTransform = base.IsOwner;
		}

		protected override void Update()
		{
			base.CanCommitToTransform = base.IsOwner;
			base.Update();
			if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsListening) && base.CanCommitToTransform)
			{
				TryCommitTransformToServer(base.transform, base.NetworkManager.LocalTime.Time);
			}
		}

		protected override void __initializeVariables()
		{
			base.__initializeVariables();
		}

		protected internal override string __getTypeName()
		{
			return "ClientNetworkTransform";
		}
	}
}
