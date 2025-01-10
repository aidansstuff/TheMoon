using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public struct PartyBeacon
	{
		internal PartyBeaconID_t Id;

		private static ISteamParties Internal => SteamParties.Internal;

		public SteamId Owner
		{
			get
			{
				SteamId pSteamIDBeaconOwner = default(SteamId);
				SteamPartyBeaconLocation_t pLocation = default(SteamPartyBeaconLocation_t);
				Internal.GetBeaconDetails(Id, ref pSteamIDBeaconOwner, ref pLocation, out var _);
				return pSteamIDBeaconOwner;
			}
		}

		public string MetaData
		{
			get
			{
				SteamId pSteamIDBeaconOwner = default(SteamId);
				SteamPartyBeaconLocation_t pLocation = default(SteamPartyBeaconLocation_t);
				Internal.GetBeaconDetails(Id, ref pSteamIDBeaconOwner, ref pLocation, out var pchMetadata);
				return pchMetadata;
			}
		}

		public async Task<string> JoinAsync()
		{
			JoinPartyCallback_t? result = await Internal.JoinParty(Id);
			if (!result.HasValue || result.Value.Result != Result.OK)
			{
				return null;
			}
			return result.Value.ConnectStringUTF8();
		}

		public void OnReservationCompleted(SteamId steamid)
		{
			Internal.OnReservationCompleted(Id, steamid);
		}

		public void CancelReservation(SteamId steamid)
		{
			Internal.CancelReservation(Id, steamid);
		}

		public bool Destroy()
		{
			return Internal.DestroyBeacon(Id);
		}
	}
}
