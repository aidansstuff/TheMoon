using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GSReputation_t : ICallbackData
	{
		internal Result Result;

		internal uint ReputationScore;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Banned;

		internal uint BannedIP;

		internal ushort BannedPort;

		internal ulong BannedGameID;

		internal uint BanExpires;

		public static int _datasize = Marshal.SizeOf(typeof(GSReputation_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GSReputation;
	}
}
