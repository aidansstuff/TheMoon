using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct PSNGameBootInviteResult_t : ICallbackData
	{
		[MarshalAs(UnmanagedType.I1)]
		internal bool GameBootInviteExists;

		internal ulong SteamIDLobby;

		public static int _datasize = Marshal.SizeOf(typeof(PSNGameBootInviteResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.PSNGameBootInviteResult;
	}
}
