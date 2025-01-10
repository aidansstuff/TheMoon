using System;
using System.Collections.Generic;
using System.Net;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamServer : SteamServerClass<SteamServer>
	{
		private static readonly List<SteamClass> openInterfaces = new List<SteamClass>();

		private static bool _dedicatedServer;

		private static int _maxplayers = 0;

		private static int _botcount = 0;

		private static string _mapname;

		private static string _modDir = "";

		private static string _product = "";

		private static string _gameDescription = "";

		private static string _serverName = "";

		private static bool _passworded;

		private static string _gametags = "";

		private static Dictionary<string, string> KeyValue = new Dictionary<string, string>();

		internal static ISteamGameServer Internal => SteamServerClass<SteamServer>.Interface as ISteamGameServer;

		public static bool IsValid => Internal != null && Internal.IsValid;

		public static bool DedicatedServer
		{
			get
			{
				return _dedicatedServer;
			}
			set
			{
				if (_dedicatedServer != value)
				{
					Internal.SetDedicatedServer(value);
					_dedicatedServer = value;
				}
			}
		}

		public static int MaxPlayers
		{
			get
			{
				return _maxplayers;
			}
			set
			{
				if (_maxplayers != value)
				{
					Internal.SetMaxPlayerCount(value);
					_maxplayers = value;
				}
			}
		}

		public static int BotCount
		{
			get
			{
				return _botcount;
			}
			set
			{
				if (_botcount != value)
				{
					Internal.SetBotPlayerCount(value);
					_botcount = value;
				}
			}
		}

		public static string MapName
		{
			get
			{
				return _mapname;
			}
			set
			{
				if (!(_mapname == value))
				{
					Internal.SetMapName(value);
					_mapname = value;
				}
			}
		}

		public static string ModDir
		{
			get
			{
				return _modDir;
			}
			internal set
			{
				if (!(_modDir == value))
				{
					Internal.SetModDir(value);
					_modDir = value;
				}
			}
		}

		public static string Product
		{
			get
			{
				return _product;
			}
			internal set
			{
				if (!(_product == value))
				{
					Internal.SetProduct(value);
					_product = value;
				}
			}
		}

		public static string GameDescription
		{
			get
			{
				return _gameDescription;
			}
			internal set
			{
				if (!(_gameDescription == value))
				{
					Internal.SetGameDescription(value);
					_gameDescription = value;
				}
			}
		}

		public static string ServerName
		{
			get
			{
				return _serverName;
			}
			set
			{
				if (!(_serverName == value))
				{
					Internal.SetServerName(value);
					_serverName = value;
				}
			}
		}

		public static bool Passworded
		{
			get
			{
				return _passworded;
			}
			set
			{
				if (_passworded != value)
				{
					Internal.SetPasswordProtected(value);
					_passworded = value;
				}
			}
		}

		public static string GameTags
		{
			get
			{
				return _gametags;
			}
			set
			{
				if (!(_gametags == value))
				{
					Internal.SetGameTags(value);
					_gametags = value;
				}
			}
		}

		public static bool LoggedOn => Internal.BLoggedOn();

		public static IPAddress PublicIp => Internal.GetPublicIP();

		public static bool AutomaticHeartbeats
		{
			set
			{
				Internal.EnableHeartbeats(value);
			}
		}

		public static int AutomaticHeartbeatRate
		{
			set
			{
				Internal.SetHeartbeatInterval(value);
			}
		}

		public static event Action<SteamId, SteamId, AuthResponse> OnValidateAuthTicketResponse;

		public static event Action OnSteamServersConnected;

		public static event Action<Result, bool> OnSteamServerConnectFailure;

		public static event Action<Result> OnSteamServersDisconnected;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamGameServer(server));
			InstallEvents();
		}

		internal static void InstallEvents()
		{
			Dispatch.Install(delegate(ValidateAuthTicketResponse_t x)
			{
				SteamServer.OnValidateAuthTicketResponse?.Invoke(x.SteamID, x.OwnerSteamID, x.AuthSessionResponse);
			}, server: true);
			Dispatch.Install<SteamServersConnected_t>(delegate
			{
				SteamServer.OnSteamServersConnected?.Invoke();
			}, server: true);
			Dispatch.Install(delegate(SteamServerConnectFailure_t x)
			{
				SteamServer.OnSteamServerConnectFailure?.Invoke(x.Result, x.StillRetrying);
			}, server: true);
			Dispatch.Install(delegate(SteamServersDisconnected_t x)
			{
				SteamServer.OnSteamServersDisconnected?.Invoke(x.Result);
			}, server: true);
		}

		public static void Init(AppId appid, SteamServerInit init, bool asyncCallbacks = true)
		{
			if (IsValid)
			{
				throw new Exception("Calling SteamServer.Init but is already initialized");
			}
			uint num = 0u;
			if (init.SteamPort == 0)
			{
				init = init.WithRandomSteamPort();
			}
			if (init.IpAddress != null)
			{
				num = init.IpAddress.IpToInt32();
			}
			Environment.SetEnvironmentVariable("SteamAppId", appid.ToString());
			Environment.SetEnvironmentVariable("SteamGameId", appid.ToString());
			int num2 = (init.Secure ? 3 : 2);
			if (!SteamInternal.GameServer_Init(num, init.SteamPort, init.GamePort, init.QueryPort, num2, init.VersionString))
			{
				throw new Exception($"InitGameServer returned false ({num},{init.SteamPort},{init.GamePort},{init.QueryPort},{num2},\"{init.VersionString}\")");
			}
			Dispatch.Init();
			Dispatch.ServerPipe = SteamGameServer.GetHSteamPipe();
			AddInterface<SteamServer>();
			AddInterface<SteamUtils>();
			AddInterface<SteamNetworking>();
			AddInterface<SteamServerStats>();
			AddInterface<SteamInventory>();
			AddInterface<SteamUGC>();
			AddInterface<SteamApps>();
			AddInterface<SteamNetworkingUtils>();
			AddInterface<SteamNetworkingSockets>();
			AutomaticHeartbeats = true;
			MaxPlayers = 32;
			BotCount = 0;
			Product = $"{appid.Value}";
			ModDir = init.ModDir;
			GameDescription = init.GameDescription;
			Passworded = false;
			DedicatedServer = init.DedicatedServer;
			if (asyncCallbacks)
			{
				Dispatch.LoopServerAsync();
			}
		}

		internal static void AddInterface<T>() where T : SteamClass, new()
		{
			T val = new T();
			val.InitializeInterface(server: true);
			openInterfaces.Add(val);
		}

		internal static void ShutdownInterfaces()
		{
			foreach (SteamClass openInterface in openInterfaces)
			{
				openInterface.DestroyInterface(server: true);
			}
			openInterfaces.Clear();
		}

		public static void Shutdown()
		{
			Dispatch.ShutdownServer();
			ShutdownInterfaces();
			SteamGameServer.Shutdown();
		}

		public static void RunCallbacks()
		{
			if (Dispatch.ServerPipe != 0)
			{
				Dispatch.Frame(Dispatch.ServerPipe);
			}
		}

		public static void LogOnAnonymous()
		{
			Internal.LogOnAnonymous();
			ForceHeartbeat();
		}

		public static void LogOff()
		{
			Internal.LogOff();
		}

		public static void ForceHeartbeat()
		{
			Internal.ForceHeartbeat();
		}

		public static void UpdatePlayer(SteamId steamid, string name, int score)
		{
			Internal.BUpdateUserData(steamid, name, (uint)score);
		}

		public static void SetKey(string Key, string Value)
		{
			if (KeyValue.ContainsKey(Key))
			{
				if (KeyValue[Key] == Value)
				{
					return;
				}
				KeyValue[Key] = Value;
			}
			else
			{
				KeyValue.Add(Key, Value);
			}
			Internal.SetKeyValue(Key, Value);
		}

		public static void ClearKeys()
		{
			KeyValue.Clear();
			Internal.ClearAllKeyValues();
		}

		public unsafe static bool BeginAuthSession(byte[] data, SteamId steamid)
		{
			fixed (byte* ptr = data)
			{
				if (Internal.BeginAuthSession((IntPtr)ptr, data.Length, steamid) == BeginAuthResult.OK)
				{
					return true;
				}
				return false;
			}
		}

		public static void EndSession(SteamId steamid)
		{
			Internal.EndAuthSession(steamid);
		}

		public unsafe static bool GetOutgoingPacket(out OutgoingPacket packet)
		{
			byte[] array = Helpers.TakeBuffer(32768);
			packet = default(OutgoingPacket);
			fixed (byte* ptr = array)
			{
				uint pNetAdr = 0u;
				ushort pPort = 0;
				int nextOutgoingPacket = Internal.GetNextOutgoingPacket((IntPtr)ptr, array.Length, ref pNetAdr, ref pPort);
				if (nextOutgoingPacket == 0)
				{
					return false;
				}
				packet.Size = nextOutgoingPacket;
				packet.Data = array;
				packet.Address = pNetAdr;
				packet.Port = pPort;
				return true;
			}
		}

		public unsafe static void HandleIncomingPacket(byte[] data, int size, uint address, ushort port)
		{
			fixed (byte* ptr = data)
			{
				HandleIncomingPacket((IntPtr)ptr, size, address, port);
			}
		}

		public static void HandleIncomingPacket(IntPtr ptr, int size, uint address, ushort port)
		{
			Internal.HandleIncomingPacket(ptr, size, address, port);
		}

		public static UserHasLicenseForAppResult UserHasLicenseForApp(SteamId steamid, AppId appid)
		{
			return Internal.UserHasLicenseForApp(steamid, appid);
		}
	}
}
