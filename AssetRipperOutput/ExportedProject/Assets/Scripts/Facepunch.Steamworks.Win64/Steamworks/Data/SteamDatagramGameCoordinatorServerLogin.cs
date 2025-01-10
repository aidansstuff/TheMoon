using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamDatagramGameCoordinatorServerLogin
	{
		internal NetIdentity Dentity;

		internal SteamDatagramHostedAddress Outing;

		internal AppId AppID;

		internal uint Time;

		internal int CbAppData;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
		internal byte[] AppData;

		internal string AppDataUTF8()
		{
			return Encoding.UTF8.GetString(AppData, 0, Array.IndexOf(AppData, (byte)0));
		}
	}
}
