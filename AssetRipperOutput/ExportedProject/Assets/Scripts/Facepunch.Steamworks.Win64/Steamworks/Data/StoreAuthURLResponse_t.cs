using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct StoreAuthURLResponse_t : ICallbackData
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
		internal byte[] URL;

		public static int _datasize = Marshal.SizeOf(typeof(StoreAuthURLResponse_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.StoreAuthURLResponse;

		internal string URLUTF8()
		{
			return Encoding.UTF8.GetString(URL, 0, Array.IndexOf(URL, (byte)0));
		}
	}
}
