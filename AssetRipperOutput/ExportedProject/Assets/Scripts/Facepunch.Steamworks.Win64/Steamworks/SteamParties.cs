using System;
using System.Collections.Generic;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamParties : SteamClientClass<SteamParties>
	{
		internal static ISteamParties Internal => SteamClientClass<SteamParties>.Interface as ISteamParties;

		public static int ActiveBeaconCount => (int)Internal.GetNumActiveBeacons();

		public static IEnumerable<PartyBeacon> ActiveBeacons
		{
			get
			{
				for (uint i = 0u; i < ActiveBeaconCount; i++)
				{
					yield return new PartyBeacon
					{
						Id = Internal.GetBeaconByIndex(i)
					};
				}
			}
		}

		public static event Action OnBeaconLocationsUpdated;

		public static event Action OnActiveBeaconsUpdated;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamParties(server));
			InstallEvents(server);
		}

		internal void InstallEvents(bool server)
		{
			Dispatch.Install<AvailableBeaconLocationsUpdated_t>(delegate
			{
				SteamParties.OnBeaconLocationsUpdated?.Invoke();
			}, server);
			Dispatch.Install<ActiveBeaconsUpdated_t>(delegate
			{
				SteamParties.OnActiveBeaconsUpdated?.Invoke();
			}, server);
		}
	}
}
