using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

namespace Netcode.Transports.Facepunch
{
	public class FacepunchTransport : NetworkTransport, IConnectionManager, ISocketManager
	{
		private class Client
		{
			public SteamId steamId;

			public Connection connection;
		}

		private ConnectionManager connectionManager;

		private SocketManager socketManager;

		private Dictionary<ulong, Client> connectedClients;

		[Space]
		[Tooltip("The Steam App ID of your game. Technically you're not allowed to use 480, but Valve doesn't do anything about it so it's fine for testing purposes.")]
		[SerializeField]
		private uint steamAppId = 480u;

		[Tooltip("The Steam ID of the user targeted when joining as a client.")]
		[SerializeField]
		public ulong targetSteamId;

		[Header("Info")]
		[ReadOnly]
		[Tooltip("When in play mode, this will display your Steam ID.")]
		[SerializeField]
		private ulong userSteamId;

		private byte[] payloadCache = new byte[4096];

		private LogLevel LogLevel => NetworkManager.Singleton.LogLevel;

		public override ulong ServerClientId => 0uL;

		private void Awake()
		{
			try
			{
				SteamClient.Init(steamAppId, asyncCallbacks: false);
			}
			catch (Exception arg)
			{
				if (LogLevel <= LogLevel.Error)
				{
					Debug.LogError(string.Format("[{0}] - Caught an exeption during initialization of Steam client: {1}", "FacepunchTransport", arg));
				}
			}
			finally
			{
				StartCoroutine(InitSteamworks());
			}
		}

		private void Update()
		{
			SteamClient.RunCallbacks();
		}

		private void OnDestroy()
		{
			SteamClient.Shutdown();
		}

		public override void DisconnectLocalClient()
		{
			connectionManager?.Connection.Close();
			if (LogLevel <= LogLevel.Developer)
			{
				Debug.Log("[FacepunchTransport] - Disconnecting local client.");
			}
		}

		public override void DisconnectRemoteClient(ulong clientId)
		{
			if (connectedClients.TryGetValue(clientId, out var value))
			{
				value.connection.Flush();
				value.connection.Close();
				connectedClients.Remove(clientId);
				if (LogLevel <= LogLevel.Developer)
				{
					Debug.Log(string.Format("[{0}] - Disconnecting remote client with ID {1}.", "FacepunchTransport", clientId));
				}
			}
			else if (LogLevel <= LogLevel.Normal)
			{
				Debug.LogWarning(string.Format("[{0}] - Failed to disconnect remote client with ID {1}, client not connected.", "FacepunchTransport", clientId));
			}
		}

		public override ulong GetCurrentRtt(ulong clientId)
		{
			return 0uL;
		}

		public override void Initialize(NetworkManager networkManager = null)
		{
			connectedClients = new Dictionary<ulong, Client>();
		}

		private SendType NetworkDeliveryToSendType(NetworkDelivery delivery)
		{
			return delivery switch
			{
				NetworkDelivery.Reliable => SendType.Reliable, 
				NetworkDelivery.ReliableFragmentedSequenced => SendType.Reliable, 
				NetworkDelivery.ReliableSequenced => SendType.Reliable, 
				NetworkDelivery.Unreliable => SendType.Unreliable, 
				NetworkDelivery.UnreliableSequenced => SendType.Unreliable, 
				_ => SendType.Reliable, 
			};
		}

		public override void Shutdown()
		{
			try
			{
				if (LogLevel <= LogLevel.Developer)
				{
					Debug.Log("[FacepunchTransport] - Shutting down.");
				}
				connectionManager?.Close();
				socketManager?.Close();
			}
			catch (Exception arg)
			{
				if (LogLevel <= LogLevel.Error)
				{
					Debug.LogError(string.Format("[{0}] - Caught an exception while shutting down: {1}", "FacepunchTransport", arg));
				}
			}
		}

		public override void Send(ulong clientId, ArraySegment<byte> data, NetworkDelivery delivery)
		{
			SendType sendType = NetworkDeliveryToSendType(delivery);
			Client value;
			if (clientId == ServerClientId)
			{
				connectionManager.Connection.SendMessage(data.Array, data.Offset, data.Count, sendType);
			}
			else if (connectedClients.TryGetValue(clientId, out value))
			{
				value.connection.SendMessage(data.Array, data.Offset, data.Count, sendType);
			}
			else if (LogLevel <= LogLevel.Normal)
			{
				Debug.LogWarning(string.Format("[{0}] - Failed to send packet to remote client with ID {1}, client not connected.", "FacepunchTransport", clientId));
			}
		}

		public override NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime)
		{
			connectionManager?.Receive();
			socketManager?.Receive();
			clientId = 0uL;
			receiveTime = Time.realtimeSinceStartup;
			payload = default(ArraySegment<byte>);
			return NetworkEvent.Nothing;
		}

		public override bool StartClient()
		{
			if (LogLevel <= LogLevel.Developer)
			{
				Debug.Log("[FacepunchTransport] - Starting as client.");
			}
			connectionManager = SteamNetworkingSockets.ConnectRelay<ConnectionManager>(targetSteamId);
			connectionManager.Interface = this;
			return true;
		}

		public override bool StartServer()
		{
			if (LogLevel <= LogLevel.Developer)
			{
				Debug.Log("[FacepunchTransport] - Starting as server.");
			}
			socketManager = SteamNetworkingSockets.CreateRelaySocket<SocketManager>();
			socketManager.Interface = this;
			return true;
		}

		private void EnsurePayloadCapacity(int size)
		{
			if (payloadCache.Length < size)
			{
				payloadCache = new byte[Math.Max(payloadCache.Length * 2, size)];
			}
		}

		void IConnectionManager.OnConnecting(ConnectionInfo info)
		{
			if (LogLevel <= LogLevel.Developer)
			{
				Debug.Log(string.Format("[{0}] - Connecting with Steam user {1}.", "FacepunchTransport", info.Identity.SteamId));
			}
		}

		void IConnectionManager.OnConnected(ConnectionInfo info)
		{
			InvokeOnTransportEvent(NetworkEvent.Connect, ServerClientId, default(ArraySegment<byte>), Time.realtimeSinceStartup);
			if (LogLevel <= LogLevel.Developer)
			{
				Debug.Log(string.Format("[{0}] - Connected with Steam user {1}.", "FacepunchTransport", info.Identity.SteamId));
			}
		}

		void IConnectionManager.OnDisconnected(ConnectionInfo info)
		{
			InvokeOnTransportEvent(NetworkEvent.Disconnect, ServerClientId, default(ArraySegment<byte>), Time.realtimeSinceStartup);
			if (LogLevel <= LogLevel.Developer)
			{
				Debug.Log(string.Format("[{0}] - Disconnected Steam user {1}.", "FacepunchTransport", info.Identity.SteamId));
			}
		}

		unsafe void IConnectionManager.OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
		{
			EnsurePayloadCapacity(size);
			fixed (byte* destination = payloadCache)
			{
				UnsafeUtility.MemCpy(destination, (void*)data, size);
			}
			InvokeOnTransportEvent(NetworkEvent.Data, ServerClientId, new ArraySegment<byte>(payloadCache, 0, size), Time.realtimeSinceStartup);
		}

		void ISocketManager.OnConnecting(Connection connection, ConnectionInfo info)
		{
			if (LogLevel <= LogLevel.Developer)
			{
				Debug.Log(string.Format("[{0}] - Accepting connection from Steam user {1}.", "FacepunchTransport", info.Identity.SteamId));
			}
			connection.Accept();
		}

		void ISocketManager.OnConnected(Connection connection, ConnectionInfo info)
		{
			if (!connectedClients.ContainsKey(connection.Id))
			{
				connectedClients.Add(connection.Id, new Client
				{
					connection = connection,
					steamId = info.Identity.SteamId
				});
				InvokeOnTransportEvent(NetworkEvent.Connect, connection.Id, default(ArraySegment<byte>), Time.realtimeSinceStartup);
				if (LogLevel <= LogLevel.Developer)
				{
					Debug.Log(string.Format("[{0}] - Connected with Steam user {1}.", "FacepunchTransport", info.Identity.SteamId));
				}
			}
			else if (LogLevel <= LogLevel.Normal)
			{
				Debug.LogWarning(string.Format("[{0}] - Failed to connect client with ID {1}, client already connected.", "FacepunchTransport", connection.Id));
			}
		}

		void ISocketManager.OnDisconnected(Connection connection, ConnectionInfo info)
		{
			if (!connectedClients.ContainsKey(connection.Id))
			{
				if (LogLevel <= LogLevel.Developer)
				{
					Debug.Log("[FacepunchTransport] - Key was already removed from connectedClients; returning");
				}
				return;
			}
			connectedClients.Remove(connection.Id);
			InvokeOnTransportEvent(NetworkEvent.Disconnect, connection.Id, default(ArraySegment<byte>), Time.realtimeSinceStartup);
			if (LogLevel <= LogLevel.Developer)
			{
				Debug.Log(string.Format("[{0}] - Disconnected Steam user {1}", "FacepunchTransport", info.Identity.SteamId));
			}
		}

		unsafe void ISocketManager.OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
		{
			EnsurePayloadCapacity(size);
			fixed (byte* destination = payloadCache)
			{
				UnsafeUtility.MemCpy(destination, (void*)data, size);
			}
			InvokeOnTransportEvent(NetworkEvent.Data, connection.Id, new ArraySegment<byte>(payloadCache, 0, size), Time.realtimeSinceStartup);
		}

		private IEnumerator InitSteamworks()
		{
			yield return new WaitUntil(() => SteamClient.IsValid);
			SteamNetworkingUtils.InitRelayNetworkAccess();
			if (LogLevel <= LogLevel.Developer)
			{
				Debug.Log("[FacepunchTransport] - Initialized access to Steam Relay Network.");
			}
			userSteamId = SteamClient.SteamId;
			if (LogLevel <= LogLevel.Developer)
			{
				Debug.Log("[FacepunchTransport] - Fetched user Steam ID.");
			}
		}
	}
}
