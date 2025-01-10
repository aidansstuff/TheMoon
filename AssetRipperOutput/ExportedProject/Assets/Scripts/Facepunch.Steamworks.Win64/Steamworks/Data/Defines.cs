namespace Steamworks.Data
{
	internal static class Defines
	{
		internal static readonly int k_cubSaltSize = 8;

		internal static readonly GID_t k_GIDNil = ulong.MaxValue;

		internal static readonly GID_t k_TxnIDNil = k_GIDNil;

		internal static readonly GID_t k_TxnIDUnknown = 0uL;

		internal static readonly JobID_t k_JobIDNil = ulong.MaxValue;

		internal static readonly PackageId_t k_uPackageIdInvalid = uint.MaxValue;

		internal static readonly BundleId_t k_uBundleIdInvalid = 0u;

		internal static readonly AppId k_uAppIdInvalid = 0;

		internal static readonly AssetClassId_t k_ulAssetClassIdInvalid = 0uL;

		internal static readonly PhysicalItemId_t k_uPhysicalItemIdInvalid = 0u;

		internal static readonly DepotId_t k_uDepotIdInvalid = 0u;

		internal static readonly CellID_t k_uCellIDInvalid = uint.MaxValue;

		internal static readonly SteamAPICall_t k_uAPICallInvalid = 0uL;

		internal static readonly PartnerId_t k_uPartnerIdInvalid = 0u;

		internal static readonly ManifestId_t k_uManifestIdInvalid = 0uL;

		internal static readonly SiteId_t k_ulSiteIdInvalid = 0uL;

		internal static readonly PartyBeaconID_t k_ulPartyBeaconIdInvalid = 0uL;

		internal static readonly HAuthTicket k_HAuthTicketInvalid = 0u;

		internal static readonly uint k_unSteamAccountIDMask = uint.MaxValue;

		internal static readonly uint k_unSteamAccountInstanceMask = 1048575u;

		internal static readonly uint k_unSteamUserDefaultInstance = 1u;

		internal static readonly int k_cchGameExtraInfoMax = 64;

		internal static readonly int k_cchMaxFriendsGroupName = 64;

		internal static readonly int k_cFriendsGroupLimit = 100;

		internal static readonly FriendsGroupID_t k_FriendsGroupID_Invalid = (short)(-1);

		internal static readonly int k_cEnumerateFollowersMax = 50;

		internal static readonly uint k_cubChatMetadataMax = 8192u;

		internal static readonly int k_cbMaxGameServerGameDir = 32;

		internal static readonly int k_cbMaxGameServerMapName = 32;

		internal static readonly int k_cbMaxGameServerGameDescription = 64;

		internal static readonly int k_cbMaxGameServerName = 64;

		internal static readonly int k_cbMaxGameServerTags = 128;

		internal static readonly int k_cbMaxGameServerGameData = 2048;

		internal static readonly int HSERVERQUERY_INVALID = -1;

		internal static readonly uint k_unFavoriteFlagNone = 0u;

		internal static readonly uint k_unFavoriteFlagFavorite = 1u;

		internal static readonly uint k_unFavoriteFlagHistory = 2u;

		internal static readonly uint k_unMaxCloudFileChunkSize = 104857600u;

		internal static readonly PublishedFileId k_PublishedFileIdInvalid = 0uL;

		internal static readonly UGCHandle_t k_UGCHandleInvalid = ulong.MaxValue;

		internal static readonly PublishedFileUpdateHandle_t k_PublishedFileUpdateHandleInvalid = ulong.MaxValue;

		internal static readonly UGCFileWriteStreamHandle_t k_UGCFileStreamHandleInvalid = ulong.MaxValue;

		internal static readonly uint k_cchPublishedDocumentTitleMax = 129u;

		internal static readonly uint k_cchPublishedDocumentDescriptionMax = 8000u;

		internal static readonly uint k_cchPublishedDocumentChangeDescriptionMax = 8000u;

		internal static readonly uint k_unEnumeratePublishedFilesMaxResults = 50u;

		internal static readonly uint k_cchTagListMax = 1025u;

		internal static readonly uint k_cchFilenameMax = 260u;

		internal static readonly uint k_cchPublishedFileURLMax = 256u;

		internal static readonly int k_cubAppProofOfPurchaseKeyMax = 240;

		internal static readonly uint k_nScreenshotMaxTaggedUsers = 32u;

		internal static readonly uint k_nScreenshotMaxTaggedPublishedFiles = 32u;

		internal static readonly int k_cubUFSTagTypeMax = 255;

		internal static readonly int k_cubUFSTagValueMax = 255;

		internal static readonly int k_ScreenshotThumbWidth = 200;

		internal static readonly UGCQueryHandle_t k_UGCQueryHandleInvalid = ulong.MaxValue;

		internal static readonly UGCUpdateHandle_t k_UGCUpdateHandleInvalid = ulong.MaxValue;

		internal static readonly uint kNumUGCResultsPerPage = 50u;

		internal static readonly uint k_cchDeveloperMetadataMax = 5000u;

		internal static readonly uint INVALID_HTMLBROWSER = 0u;

		internal static readonly InventoryItemId k_SteamItemInstanceIDInvalid = ulong.MaxValue;

		internal static readonly SteamInventoryResult_t k_SteamInventoryResultInvalid = -1;

		internal static readonly SteamInventoryUpdateHandle_t k_SteamInventoryUpdateHandleInvalid = ulong.MaxValue;

		internal static readonly Connection k_HSteamNetConnection_Invalid = 0u;

		internal static readonly Socket k_HSteamListenSocket_Invalid = 0u;

		internal static readonly HSteamNetPollGroup k_HSteamNetPollGroup_Invalid = 0u;

		internal static readonly int k_cchMaxSteamNetworkingErrMsg = 1024;

		internal static readonly int k_cchSteamNetworkingMaxConnectionCloseReason = 128;

		internal static readonly int k_cchSteamNetworkingMaxConnectionDescription = 128;

		internal static readonly int k_cbMaxSteamNetworkingSocketsMessageSizeSend = 524288;

		internal static readonly int k_nSteamNetworkingSend_Unreliable = 0;

		internal static readonly int k_nSteamNetworkingSend_NoNagle = 1;

		internal static readonly int k_nSteamNetworkingSend_UnreliableNoNagle = k_nSteamNetworkingSend_Unreliable | k_nSteamNetworkingSend_NoNagle;

		internal static readonly int k_nSteamNetworkingSend_NoDelay = 4;

		internal static readonly int k_nSteamNetworkingSend_UnreliableNoDelay = k_nSteamNetworkingSend_Unreliable | k_nSteamNetworkingSend_NoDelay | k_nSteamNetworkingSend_NoNagle;

		internal static readonly int k_nSteamNetworkingSend_Reliable = 8;

		internal static readonly int k_nSteamNetworkingSend_ReliableNoNagle = k_nSteamNetworkingSend_Reliable | k_nSteamNetworkingSend_NoNagle;

		internal static readonly int k_nSteamNetworkingSend_UseCurrentThread = 16;

		internal static readonly int k_cchMaxSteamNetworkingPingLocationString = 1024;

		internal static readonly int k_nSteamNetworkingPing_Failed = -1;

		internal static readonly int k_nSteamNetworkingPing_Unknown = -2;

		internal static readonly SteamNetworkingPOPID k_SteamDatagramPOPID_dev = 6579574u;

		internal static readonly uint k_unServerFlagNone = 0u;

		internal static readonly uint k_unServerFlagActive = 1u;

		internal static readonly uint k_unServerFlagSecure = 2u;

		internal static readonly uint k_unServerFlagDedicated = 4u;

		internal static readonly uint k_unServerFlagLinux = 8u;

		internal static readonly uint k_unServerFlagPassworded = 16u;

		internal static readonly uint k_unServerFlagPrivate = 32u;

		internal static readonly uint k_cbSteamDatagramMaxSerializedTicket = 512u;

		internal static readonly uint k_cbMaxSteamDatagramGameCoordinatorServerLoginAppData = 2048u;

		internal static readonly uint k_cbMaxSteamDatagramGameCoordinatorServerLoginSerialized = 4096u;
	}
}
