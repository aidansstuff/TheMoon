using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct MarketEligibilityResponse_t : ICallbackData
	{
		[MarshalAs(UnmanagedType.I1)]
		internal bool Allowed;

		internal MarketNotAllowedReasonFlags NotAllowedReason;

		internal uint TAllowedAtTime;

		internal int CdaySteamGuardRequiredDays;

		internal int CdayNewDeviceCooldown;

		public static int _datasize = Marshal.SizeOf(typeof(MarketEligibilityResponse_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.MarketEligibilityResponse;
	}
}
