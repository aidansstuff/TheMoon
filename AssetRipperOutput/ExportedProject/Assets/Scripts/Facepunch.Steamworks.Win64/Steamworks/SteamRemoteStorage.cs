using System;
using System.Collections.Generic;

namespace Steamworks
{
	public class SteamRemoteStorage : SteamClientClass<SteamRemoteStorage>
	{
		internal static ISteamRemoteStorage Internal => SteamClientClass<SteamRemoteStorage>.Interface as ISteamRemoteStorage;

		public static ulong QuotaBytes
		{
			get
			{
				ulong pnTotalBytes = 0uL;
				ulong puAvailableBytes = 0uL;
				Internal.GetQuota(ref pnTotalBytes, ref puAvailableBytes);
				return pnTotalBytes;
			}
		}

		public static ulong QuotaUsedBytes
		{
			get
			{
				ulong pnTotalBytes = 0uL;
				ulong puAvailableBytes = 0uL;
				Internal.GetQuota(ref pnTotalBytes, ref puAvailableBytes);
				return pnTotalBytes - puAvailableBytes;
			}
		}

		public static ulong QuotaRemainingBytes
		{
			get
			{
				ulong pnTotalBytes = 0uL;
				ulong puAvailableBytes = 0uL;
				Internal.GetQuota(ref pnTotalBytes, ref puAvailableBytes);
				return puAvailableBytes;
			}
		}

		public static bool IsCloudEnabled => IsCloudEnabledForAccount && IsCloudEnabledForApp;

		public static bool IsCloudEnabledForAccount => Internal.IsCloudEnabledForAccount();

		public static bool IsCloudEnabledForApp
		{
			get
			{
				return Internal.IsCloudEnabledForApp();
			}
			set
			{
				Internal.SetCloudEnabledForApp(value);
			}
		}

		public static int FileCount => Internal.GetFileCount();

		public static IEnumerable<string> Files
		{
			get
			{
				int _ = 0;
				for (int i = 0; i < FileCount; i++)
				{
					yield return Internal.GetFileNameAndSize(i, ref _);
				}
			}
		}

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamRemoteStorage(server));
		}

		public unsafe static bool FileWrite(string filename, byte[] data)
		{
			fixed (byte* ptr = data)
			{
				return Internal.FileWrite(filename, (IntPtr)ptr, data.Length);
			}
		}

		public unsafe static byte[] FileRead(string filename)
		{
			int num = FileSize(filename);
			if (num <= 0)
			{
				return null;
			}
			byte[] array = new byte[num];
			fixed (byte* ptr = array)
			{
				int num2 = Internal.FileRead(filename, (IntPtr)ptr, num);
				return array;
			}
		}

		public static bool FileExists(string filename)
		{
			return Internal.FileExists(filename);
		}

		public static bool FilePersisted(string filename)
		{
			return Internal.FilePersisted(filename);
		}

		public static DateTime FileTime(string filename)
		{
			return Epoch.ToDateTime(Internal.GetFileTimestamp(filename));
		}

		public static int FileSize(string filename)
		{
			return Internal.GetFileSize(filename);
		}

		public static bool FileForget(string filename)
		{
			return Internal.FileForget(filename);
		}

		public static bool FileDelete(string filename)
		{
			return Internal.FileDelete(filename);
		}
	}
}
