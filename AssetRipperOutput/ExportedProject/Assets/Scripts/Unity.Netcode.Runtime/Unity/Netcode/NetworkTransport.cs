using System;
using UnityEngine;

namespace Unity.Netcode
{
	public abstract class NetworkTransport : MonoBehaviour
	{
		public delegate void TransportEventDelegate(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime);

		internal INetworkMetrics NetworkMetrics;

		public abstract ulong ServerClientId { get; }

		public virtual bool IsSupported => true;

		public event TransportEventDelegate OnTransportEvent;

		protected void InvokeOnTransportEvent(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime)
		{
			this.OnTransportEvent?.Invoke(eventType, clientId, payload, receiveTime);
		}

		public abstract void Send(ulong clientId, ArraySegment<byte> payload, NetworkDelivery networkDelivery);

		public abstract NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime);

		public abstract bool StartClient();

		public abstract bool StartServer();

		public abstract void DisconnectRemoteClient(ulong clientId);

		public abstract void DisconnectLocalClient();

		public abstract ulong GetCurrentRtt(ulong clientId);

		public abstract void Shutdown();

		public abstract void Initialize(NetworkManager networkManager = null);
	}
}
