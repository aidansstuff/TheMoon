using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GSClientDeny_t : ICallbackData
	{
		internal ulong SteamID;

		internal DenyReason DenyReason;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		internal byte[] OptionalText;

		public static int _datasize = Marshal.SizeOf(typeof(GSClientDeny_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GSClientDeny;

		internal string OptionalTextUTF8()
		{
			return Encoding.UTF8.GetString(OptionalText, 0, Array.IndexOf(OptionalText, (byte)0));
		}
	}
}
