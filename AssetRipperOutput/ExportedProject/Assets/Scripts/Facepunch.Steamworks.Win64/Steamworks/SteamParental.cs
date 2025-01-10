using System;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamParental : SteamSharedClass<SteamParental>
	{
		internal static ISteamParentalSettings Internal => SteamSharedClass<SteamParental>.Interface as ISteamParentalSettings;

		public static bool IsParentalLockEnabled => Internal.BIsParentalLockEnabled();

		public static bool IsParentalLockLocked => Internal.BIsParentalLockLocked();

		public static event Action OnSettingsChanged;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamParentalSettings(server));
			InstallEvents(server);
		}

		internal static void InstallEvents(bool server)
		{
			Dispatch.Install<SteamParentalSettingsChanged_t>(delegate
			{
				SteamParental.OnSettingsChanged?.Invoke();
			}, server);
		}

		public static bool IsAppBlocked(AppId app)
		{
			return Internal.BIsAppBlocked(app.Value);
		}

		public static bool BIsAppInBlockList(AppId app)
		{
			return Internal.BIsAppInBlockList(app.Value);
		}

		public static bool IsFeatureBlocked(ParentalFeature feature)
		{
			return Internal.BIsFeatureBlocked(feature);
		}

		public static bool BIsFeatureInBlockList(ParentalFeature feature)
		{
			return Internal.BIsFeatureInBlockList(feature);
		}
	}
}
