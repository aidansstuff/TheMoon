using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamInventoryRequestPricesResult_t : ICallbackData
	{
		internal Result Result;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		internal byte[] Currency;

		public static int _datasize = Marshal.SizeOf(typeof(SteamInventoryRequestPricesResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamInventoryRequestPricesResult;

		internal string CurrencyUTF8()
		{
			return Encoding.UTF8.GetString(Currency, 0, Array.IndexOf(Currency, (byte)0));
		}
	}
}
