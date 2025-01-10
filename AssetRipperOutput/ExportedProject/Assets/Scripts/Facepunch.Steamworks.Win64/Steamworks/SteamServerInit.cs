using System;
using System.Net;

namespace Steamworks
{
	public struct SteamServerInit
	{
		public IPAddress IpAddress;

		public ushort SteamPort;

		public ushort GamePort;

		public ushort QueryPort;

		public bool Secure;

		public string VersionString;

		public string ModDir;

		public string GameDescription;

		public bool DedicatedServer;

		public SteamServerInit(string modDir, string gameDesc)
		{
			DedicatedServer = true;
			ModDir = modDir;
			GameDescription = gameDesc;
			GamePort = 27015;
			QueryPort = 27016;
			Secure = true;
			VersionString = "1.0.0.0";
			IpAddress = null;
			SteamPort = 0;
		}

		public SteamServerInit WithRandomSteamPort()
		{
			SteamPort = (ushort)new Random().Next(10000, 60000);
			return this;
		}

		public SteamServerInit WithQueryShareGamePort()
		{
			QueryPort = ushort.MaxValue;
			return this;
		}
	}
}
