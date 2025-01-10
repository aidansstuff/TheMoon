using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct SteamInventoryEligiblePromoItemDefIDs_t : ICallbackData
	{
		internal Result Result;

		internal ulong SteamID;

		internal int UmEligiblePromoItemDefs;

		[MarshalAs(UnmanagedType.I1)]
		internal bool CachedData;

		public static int _datasize = Marshal.SizeOf(typeof(SteamInventoryEligiblePromoItemDefIDs_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamInventoryEligiblePromoItemDefIDs;
	}
}
