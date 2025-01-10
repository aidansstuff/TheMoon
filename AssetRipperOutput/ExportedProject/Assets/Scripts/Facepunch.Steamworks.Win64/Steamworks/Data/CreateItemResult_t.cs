using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CreateItemResult_t : ICallbackData
	{
		internal Result Result;

		internal PublishedFileId PublishedFileId;

		[MarshalAs(UnmanagedType.I1)]
		internal bool UserNeedsToAcceptWorkshopLegalAgreement;

		public static int _datasize = Marshal.SizeOf(typeof(CreateItemResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.CreateItemResult;
	}
}
