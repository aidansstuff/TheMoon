using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GameServerChangeRequested_t : ICallbackData
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		internal byte[] Server;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		internal byte[] Password;

		public static int _datasize = Marshal.SizeOf(typeof(GameServerChangeRequested_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GameServerChangeRequested;

		internal string ServerUTF8()
		{
			return Encoding.UTF8.GetString(Server, 0, Array.IndexOf(Server, (byte)0));
		}

		internal string PasswordUTF8()
		{
			return Encoding.UTF8.GetString(Password, 0, Array.IndexOf(Password, (byte)0));
		}
	}
}
