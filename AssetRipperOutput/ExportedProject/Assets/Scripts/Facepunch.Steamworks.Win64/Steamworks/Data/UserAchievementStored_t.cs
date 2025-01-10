using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct UserAchievementStored_t : ICallbackData
	{
		internal ulong GameID;

		[MarshalAs(UnmanagedType.I1)]
		internal bool GroupAchievement;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		internal byte[] AchievementName;

		internal uint CurProgress;

		internal uint MaxProgress;

		public static int _datasize = Marshal.SizeOf(typeof(UserAchievementStored_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.UserAchievementStored;

		internal string AchievementNameUTF8()
		{
			return Encoding.UTF8.GetString(AchievementName, 0, Array.IndexOf(AchievementName, (byte)0));
		}
	}
}
