using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct AppProofOfPurchaseKeyResponse_t : ICallbackData
	{
		internal Result Result;

		internal uint AppID;

		internal uint CchKeyLength;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 240)]
		internal byte[] Key;

		public static int _datasize = Marshal.SizeOf(typeof(AppProofOfPurchaseKeyResponse_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.AppProofOfPurchaseKeyResponse;

		internal string KeyUTF8()
		{
			return Encoding.UTF8.GetString(Key, 0, Array.IndexOf(Key, (byte)0));
		}
	}
}
