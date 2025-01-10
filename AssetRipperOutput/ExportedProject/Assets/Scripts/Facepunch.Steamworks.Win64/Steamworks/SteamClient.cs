using System;
using System.Collections.Generic;

namespace Steamworks
{
	public static class SteamClient
	{
		private static bool initialized;

		private static readonly List<SteamClass> openInterfaces = new List<SteamClass>();

		public static bool IsValid => initialized;

		public static bool IsLoggedOn => SteamUser.Internal.BLoggedOn();

		public static SteamId SteamId => SteamUser.Internal.GetSteamID();

		public static string Name => SteamFriends.Internal.GetPersonaName();

		public static FriendState State => SteamFriends.Internal.GetPersonaState();

		public static AppId AppId { get; internal set; }

		public static void Init(uint appid, bool asyncCallbacks = true)
		{
			if (initialized)
			{
				throw new Exception("Calling SteamClient.Init but is already initialized");
			}
			Environment.SetEnvironmentVariable("SteamAppId", appid.ToString());
			Environment.SetEnvironmentVariable("SteamGameId", appid.ToString());
			if (!SteamAPI.Init())
			{
				throw new Exception("SteamApi_Init returned false. Steam isn't running, couldn't find Steam, AppId is ureleased, Don't own AppId.");
			}
			AppId = appid;
			initialized = true;
			Dispatch.Init();
			Dispatch.ClientPipe = SteamAPI.GetHSteamPipe();
			AddInterface<SteamApps>();
			AddInterface<SteamFriends>();
			AddInterface<SteamInput>();
			AddInterface<SteamInventory>();
			AddInterface<SteamMatchmaking>();
			AddInterface<SteamMatchmakingServers>();
			AddInterface<SteamMusic>();
			AddInterface<SteamNetworking>();
			AddInterface<SteamNetworkingSockets>();
			AddInterface<SteamNetworkingUtils>();
			AddInterface<SteamParental>();
			AddInterface<SteamParties>();
			AddInterface<SteamRemoteStorage>();
			AddInterface<SteamScreenshots>();
			AddInterface<SteamUGC>();
			AddInterface<SteamUser>();
			AddInterface<SteamUserStats>();
			AddInterface<SteamUtils>();
			AddInterface<SteamVideo>();
			AddInterface<SteamRemotePlay>();
			if (asyncCallbacks)
			{
				Dispatch.LoopClientAsync();
			}
		}

		internal static void AddInterface<T>() where T : SteamClass, new()
		{
			T val = new T();
			val.InitializeInterface(server: false);
			openInterfaces.Add(val);
		}

		internal static void ShutdownInterfaces()
		{
			foreach (SteamClass openInterface in openInterfaces)
			{
				openInterface.DestroyInterface(server: false);
			}
			openInterfaces.Clear();
		}

		public static void Shutdown()
		{
			if (IsValid)
			{
				Cleanup();
				SteamAPI.Shutdown();
			}
		}

		internal static void Cleanup()
		{
			Dispatch.ShutdownClient();
			initialized = false;
			ShutdownInterfaces();
		}

		public static void RunCallbacks()
		{
			if (Dispatch.ClientPipe != 0)
			{
				Dispatch.Frame(Dispatch.ClientPipe);
			}
		}

		public static bool RestartAppIfNecessary(uint appid)
		{
			return SteamAPI.RestartAppIfNecessary(appid);
		}

		internal static void ValidCheck()
		{
			if (!IsValid)
			{
				throw new Exception("SteamClient isn't initialized");
			}
		}
	}
}
