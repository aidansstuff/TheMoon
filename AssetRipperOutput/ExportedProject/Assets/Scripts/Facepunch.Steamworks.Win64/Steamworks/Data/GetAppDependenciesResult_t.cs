using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GetAppDependenciesResult_t : ICallbackData
	{
		internal Result Result;

		internal PublishedFileId PublishedFileId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.U4)]
		internal AppId[] GAppIDs;

		internal uint NumAppDependencies;

		internal uint TotalNumAppDependencies;

		public static int _datasize = Marshal.SizeOf(typeof(GetAppDependenciesResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GetAppDependenciesResult;
	}
}
