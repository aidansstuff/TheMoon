using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct UserAchievementIconFetched_t : ICallbackData
	{
		internal GameId GameID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		internal byte[] AchievementName;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Achieved;

		internal int IconHandle;

		public static int _datasize = Marshal.SizeOf(typeof(UserAchievementIconFetched_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.UserAchievementIconFetched;

		internal string AchievementNameUTF8()
		{
			return Encoding.UTF8.GetString(AchievementName, 0, Array.IndexOf(AchievementName, (byte)0));
		}
	}
}
