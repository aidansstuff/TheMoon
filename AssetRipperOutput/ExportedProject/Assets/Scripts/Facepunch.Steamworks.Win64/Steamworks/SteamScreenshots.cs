using System;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamScreenshots : SteamClientClass<SteamScreenshots>
	{
		internal static ISteamScreenshots Internal => SteamClientClass<SteamScreenshots>.Interface as ISteamScreenshots;

		public static bool Hooked
		{
			get
			{
				return Internal.IsScreenshotsHooked();
			}
			set
			{
				Internal.HookScreenshots(value);
			}
		}

		public static event Action OnScreenshotRequested;

		public static event Action<Screenshot> OnScreenshotReady;

		public static event Action<Result> OnScreenshotFailed;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamScreenshots(server));
			InstallEvents();
		}

		internal static void InstallEvents()
		{
			Dispatch.Install<ScreenshotRequested_t>(delegate
			{
				SteamScreenshots.OnScreenshotRequested?.Invoke();
			});
			Dispatch.Install(delegate(ScreenshotReady_t x)
			{
				if (x.Result != Result.OK)
				{
					SteamScreenshots.OnScreenshotFailed?.Invoke(x.Result);
				}
				else
				{
					SteamScreenshots.OnScreenshotReady?.Invoke(new Screenshot
					{
						Value = x.Local
					});
				}
			});
		}

		public unsafe static Screenshot? WriteScreenshot(byte[] data, int width, int height)
		{
			fixed (byte* ptr = data)
			{
				ScreenshotHandle value = Internal.WriteScreenshot((IntPtr)ptr, (uint)data.Length, width, height);
				if (value.Value == 0)
				{
					return null;
				}
				Screenshot value2 = default(Screenshot);
				value2.Value = value;
				return value2;
			}
		}

		public static Screenshot? AddScreenshot(string filename, string thumbnail, int width, int height)
		{
			ScreenshotHandle value = Internal.AddScreenshotToLibrary(filename, thumbnail, width, height);
			if (value.Value == 0)
			{
				return null;
			}
			Screenshot value2 = default(Screenshot);
			value2.Value = value;
			return value2;
		}

		public static void TriggerScreenshot()
		{
			Internal.TriggerScreenshot();
		}
	}
}
