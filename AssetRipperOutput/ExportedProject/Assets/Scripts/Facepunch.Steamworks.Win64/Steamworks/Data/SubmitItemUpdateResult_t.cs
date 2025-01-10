using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SubmitItemUpdateResult_t : ICallbackData
	{
		internal Result Result;

		[MarshalAs(UnmanagedType.I1)]
		internal bool UserNeedsToAcceptWorkshopLegalAgreement;

		internal PublishedFileId PublishedFileId;

		public static int _datasize = Marshal.SizeOf(typeof(SubmitItemUpdateResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SubmitItemUpdateResult;
	}
}
