using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamInventoryStartPurchaseResult_t : ICallbackData
	{
		internal Result Result;

		internal ulong OrderID;

		internal ulong TransID;

		public static int _datasize = Marshal.SizeOf(typeof(SteamInventoryStartPurchaseResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamInventoryStartPurchaseResult;
	}
}
