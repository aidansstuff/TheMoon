using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct JoinPartyCallback_t : ICallbackData
	{
		internal Result Result;

		internal ulong BeaconID;

		internal ulong SteamIDBeaconOwner;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		internal byte[] ConnectString;

		public static int _datasize = Marshal.SizeOf(typeof(JoinPartyCallback_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.JoinPartyCallback;

		internal string ConnectStringUTF8()
		{
			return Encoding.UTF8.GetString(ConnectString, 0, Array.IndexOf(ConnectString, (byte)0));
		}
	}
}
