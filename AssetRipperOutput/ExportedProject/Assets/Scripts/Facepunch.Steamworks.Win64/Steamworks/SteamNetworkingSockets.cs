using System;
using System.Collections.Generic;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamNetworkingSockets : SteamSharedClass<SteamNetworkingSockets>
	{
		private static readonly Dictionary<uint, SocketManager> SocketInterfaces = new Dictionary<uint, SocketManager>();

		private static readonly Dictionary<uint, ConnectionManager> ConnectionInterfaces = new Dictionary<uint, ConnectionManager>();

		internal static ISteamNetworkingSockets Internal => SteamSharedClass<SteamNetworkingSockets>.Interface as ISteamNetworkingSockets;

		public static event Action<Connection, ConnectionInfo> OnConnectionStatusChanged;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamNetworkingSockets(server));
			InstallEvents(server);
		}

		internal static SocketManager GetSocketManager(uint id)
		{
			if (SocketInterfaces == null)
			{
				return null;
			}
			if (id == 0)
			{
				throw new ArgumentException("Invalid Socket");
			}
			if (SocketInterfaces.TryGetValue(id, out var value))
			{
				return value;
			}
			return null;
		}

		internal static void SetSocketManager(uint id, SocketManager manager)
		{
			if (id == 0)
			{
				throw new ArgumentException("Invalid Socket");
			}
			SocketInterfaces[id] = manager;
		}

		internal static ConnectionManager GetConnectionManager(uint id)
		{
			if (ConnectionInterfaces == null)
			{
				return null;
			}
			if (id == 0)
			{
				return null;
			}
			if (ConnectionInterfaces.TryGetValue(id, out var value))
			{
				return value;
			}
			return null;
		}

		internal static void SetConnectionManager(uint id, ConnectionManager manager)
		{
			if (id == 0)
			{
				throw new ArgumentException("Invalid Connection");
			}
			ConnectionInterfaces[id] = manager;
		}

		internal void InstallEvents(bool server)
		{
			Dispatch.Install<SteamNetConnectionStatusChangedCallback_t>(ConnectionStatusChanged, server);
		}

		private static void ConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t data)
		{
			if (data.Nfo.listenSocket.Id != 0)
			{
				GetSocketManager(data.Nfo.listenSocket.Id)?.OnConnectionChanged(data.Conn, data.Nfo);
			}
			else
			{
				GetConnectionManager(data.Conn.Id)?.OnConnectionChanged(data.Nfo);
			}
			SteamNetworkingSockets.OnConnectionStatusChanged?.Invoke(data.Conn, data.Nfo);
		}

		public static T CreateNormalSocket<T>(NetAddress address) where T : SocketManager, new()
		{
			T val = new T();
			NetKeyValue[] array = Array.Empty<NetKeyValue>();
			val.Socket = Internal.CreateListenSocketIP(ref address, array.Length, array);
			val.Initialize();
			SetSocketManager(val.Socket.Id, val);
			return val;
		}

		public static SocketManager CreateNormalSocket(NetAddress address, ISocketManager intrface)
		{
			NetKeyValue[] array = Array.Empty<NetKeyValue>();
			Socket socket = Internal.CreateListenSocketIP(ref address, array.Length, array);
			SocketManager socketManager = new SocketManager
			{
				Socket = socket,
				Interface = intrface
			};
			socketManager.Initialize();
			SetSocketManager(socketManager.Socket.Id, socketManager);
			return socketManager;
		}

		public static T ConnectNormal<T>(NetAddress address) where T : ConnectionManager, new()
		{
			T val = new T();
			NetKeyValue[] array = Array.Empty<NetKeyValue>();
			val.Connection = Internal.ConnectByIPAddress(ref address, array.Length, array);
			SetConnectionManager(val.Connection.Id, val);
			return val;
		}

		public static ConnectionManager ConnectNormal(NetAddress address, IConnectionManager iface)
		{
			NetKeyValue[] array = Array.Empty<NetKeyValue>();
			Connection connection = Internal.ConnectByIPAddress(ref address, array.Length, array);
			ConnectionManager connectionManager = new ConnectionManager
			{
				Connection = connection,
				Interface = iface
			};
			SetConnectionManager(connectionManager.Connection.Id, connectionManager);
			return connectionManager;
		}

		public static T CreateRelaySocket<T>(int virtualport = 0) where T : SocketManager, new()
		{
			T val = new T();
			NetKeyValue[] array = Array.Empty<NetKeyValue>();
			val.Socket = Internal.CreateListenSocketP2P(virtualport, array.Length, array);
			val.Initialize();
			SetSocketManager(val.Socket.Id, val);
			return val;
		}

		public static T ConnectRelay<T>(SteamId serverId, int virtualport = 0) where T : ConnectionManager, new()
		{
			T val = new T();
			NetIdentity identityRemote = serverId;
			NetKeyValue[] array = Array.Empty<NetKeyValue>();
			val.Connection = Internal.ConnectP2P(ref identityRemote, virtualport, array.Length, array);
			SetConnectionManager(val.Connection.Id, val);
			return val;
		}
	}
}
