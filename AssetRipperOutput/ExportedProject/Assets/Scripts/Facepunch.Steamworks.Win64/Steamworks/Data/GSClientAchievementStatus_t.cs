using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GSClientAchievementStatus_t : ICallbackData
	{
		internal ulong SteamID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		internal byte[] PchAchievement;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Unlocked;

		public static int _datasize = Marshal.SizeOf(typeof(GSClientAchievementStatus_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GSClientAchievementStatus;

		internal string PchAchievementUTF8()
		{
			return Encoding.UTF8.GetString(PchAchievement, 0, Array.IndexOf(PchAchievement, (byte)0));
		}
	}
}
