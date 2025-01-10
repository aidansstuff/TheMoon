using System;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamUtils : SteamSharedClass<SteamUtils>
	{
		private static NotificationPosition overlayNotificationPosition = NotificationPosition.BottomRight;

		internal static ISteamUtils Internal => SteamSharedClass<SteamUtils>.Interface as ISteamUtils;

		public static uint SecondsSinceAppActive => Internal.GetSecondsSinceAppActive();

		public static uint SecondsSinceComputerActive => Internal.GetSecondsSinceComputerActive();

		public static Universe ConnectedUniverse => Internal.GetConnectedUniverse();

		public static DateTime SteamServerTime => Epoch.ToDateTime(Internal.GetServerRealTime());

		public static string IpCountry => Internal.GetIPCountry();

		public static bool UsingBatteryPower => Internal.GetCurrentBatteryPower() != byte.MaxValue;

		public static float CurrentBatteryPower => Math.Min(Internal.GetCurrentBatteryPower() / 100, 1f);

		public static NotificationPosition OverlayNotificationPosition
		{
			get
			{
				return overlayNotificationPosition;
			}
			set
			{
				overlayNotificationPosition = value;
				Internal.SetOverlayNotificationPosition(value);
			}
		}

		public static bool IsOverlayEnabled => Internal.IsOverlayEnabled();

		public static bool DoesOverlayNeedPresent => Internal.BOverlayNeedsPresent();

		public static string SteamUILanguage => Internal.GetSteamUILanguage();

		public static bool IsSteamRunningInVR => Internal.IsSteamRunningInVR();

		public static bool IsSteamInBigPictureMode => Internal.IsSteamInBigPictureMode();

		public static bool VrHeadsetStreaming
		{
			get
			{
				return Internal.IsVRHeadsetStreamingEnabled();
			}
			set
			{
				Internal.SetVRHeadsetStreamingEnabled(value);
			}
		}

		public static bool IsSteamChinaLauncher => Internal.IsSteamChinaLauncher();

		public static event Action OnIpCountryChanged;

		public static event Action<int> OnLowBatteryPower;

		public static event Action OnSteamShutdown;

		public static event Action<bool> OnGamepadTextInputDismissed;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamUtils(server));
			InstallEvents(server);
		}

		internal static void InstallEvents(bool server)
		{
			Dispatch.Install<IPCountry_t>(delegate
			{
				SteamUtils.OnIpCountryChanged?.Invoke();
			}, server);
			Dispatch.Install(delegate(LowBatteryPower_t x)
			{
				SteamUtils.OnLowBatteryPower?.Invoke(x.MinutesBatteryLeft);
			}, server);
			Dispatch.Install<SteamShutdown_t>(delegate
			{
				SteamClosed();
			}, server);
			Dispatch.Install(delegate(GamepadTextInputDismissed_t x)
			{
				SteamUtils.OnGamepadTextInputDismissed?.Invoke(x.Submitted);
			}, server);
		}

		private static void SteamClosed()
		{
			SteamClient.Cleanup();
			SteamUtils.OnSteamShutdown?.Invoke();
		}

		public static bool GetImageSize(int image, out uint width, out uint height)
		{
			width = 0u;
			height = 0u;
			return Internal.GetImageSize(image, ref width, ref height);
		}

		public static Image? GetImage(int image)
		{
			switch (image)
			{
			case -1:
				return null;
			case 0:
				return null;
			default:
			{
				Image value = default(Image);
				if (!GetImageSize(image, out value.Width, out value.Height))
				{
					return null;
				}
				uint num = value.Width * value.Height * 4;
				byte[] array = Helpers.TakeBuffer((int)num);
				if (!Internal.GetImageRGBA(image, array, (int)num))
				{
					return null;
				}
				value.Data = new byte[num];
				Array.Copy(array, 0L, value.Data, 0L, num);
				return value;
			}
			}
		}

		public static async Task<CheckFileSignature> CheckFileSignatureAsync(string filename)
		{
			CheckFileSignature_t? r = await Internal.CheckFileSignature(filename);
			if (!r.HasValue)
			{
				throw new Exception("Something went wrong");
			}
			return r.Value.CheckFileSignature;
		}

		public static bool ShowGamepadTextInput(GamepadTextInputMode inputMode, GamepadTextInputLineMode lineInputMode, string description, int maxChars, string existingText = "")
		{
			return Internal.ShowGamepadTextInput(inputMode, lineInputMode, description, (uint)maxChars, existingText);
		}

		public static string GetEnteredGamepadText()
		{
			if (Internal.GetEnteredGamepadTextLength() == 0)
			{
				return string.Empty;
			}
			if (!Internal.GetEnteredGamepadTextInput(out var pchText))
			{
				return string.Empty;
			}
			return pchText;
		}

		public static void SetOverlayNotificationInset(int x, int y)
		{
			Internal.SetOverlayNotificationInset(x, y);
		}

		public static void StartVRDashboard()
		{
			Internal.StartVRDashboard();
		}

		internal static bool IsCallComplete(SteamAPICall_t call, out bool failed)
		{
			failed = false;
			return Internal.IsAPICallCompleted(call, ref failed);
		}
	}
}
