using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamUGC : SteamInterface
	{
		internal ISteamUGC(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamUGC_v014();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamUGC_v014();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamGameServerUGC_v014();

		public override IntPtr GetServerInterfacePointer()
		{
			return SteamAPI_SteamGameServerUGC_v014();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_CreateQueryUserUGCRequest")]
		private static extern UGCQueryHandle_t _CreateQueryUserUGCRequest(IntPtr self, AccountID_t unAccountID, UserUGCList eListType, UgcType eMatchingUGCType, UserUGCListSortOrder eSortOrder, AppId nCreatorAppID, AppId nConsumerAppID, uint unPage);

		internal UGCQueryHandle_t CreateQueryUserUGCRequest(AccountID_t unAccountID, UserUGCList eListType, UgcType eMatchingUGCType, UserUGCListSortOrder eSortOrder, AppId nCreatorAppID, AppId nConsumerAppID, uint unPage)
		{
			return _CreateQueryUserUGCRequest(Self, unAccountID, eListType, eMatchingUGCType, eSortOrder, nCreatorAppID, nConsumerAppID, unPage);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_CreateQueryAllUGCRequestPage")]
		private static extern UGCQueryHandle_t _CreateQueryAllUGCRequest(IntPtr self, UGCQuery eQueryType, UgcType eMatchingeMatchingUGCTypeFileType, AppId nCreatorAppID, AppId nConsumerAppID, uint unPage);

		internal UGCQueryHandle_t CreateQueryAllUGCRequest(UGCQuery eQueryType, UgcType eMatchingeMatchingUGCTypeFileType, AppId nCreatorAppID, AppId nConsumerAppID, uint unPage)
		{
			return _CreateQueryAllUGCRequest(Self, eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, unPage);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_CreateQueryAllUGCRequestCursor")]
		private static extern UGCQueryHandle_t _CreateQueryAllUGCRequest(IntPtr self, UGCQuery eQueryType, UgcType eMatchingeMatchingUGCTypeFileType, AppId nCreatorAppID, AppId nConsumerAppID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchCursor);

		internal UGCQueryHandle_t CreateQueryAllUGCRequest(UGCQuery eQueryType, UgcType eMatchingeMatchingUGCTypeFileType, AppId nCreatorAppID, AppId nConsumerAppID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchCursor)
		{
			return _CreateQueryAllUGCRequest(Self, eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, pchCursor);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_CreateQueryUGCDetailsRequest")]
		private static extern UGCQueryHandle_t _CreateQueryUGCDetailsRequest(IntPtr self, [In][Out] PublishedFileId[] pvecPublishedFileID, uint unNumPublishedFileIDs);

		internal UGCQueryHandle_t CreateQueryUGCDetailsRequest([In][Out] PublishedFileId[] pvecPublishedFileID, uint unNumPublishedFileIDs)
		{
			return _CreateQueryUGCDetailsRequest(Self, pvecPublishedFileID, unNumPublishedFileIDs);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SendQueryUGCRequest")]
		private static extern SteamAPICall_t _SendQueryUGCRequest(IntPtr self, UGCQueryHandle_t handle);

		internal CallResult<SteamUGCQueryCompleted_t> SendQueryUGCRequest(UGCQueryHandle_t handle)
		{
			SteamAPICall_t call = _SendQueryUGCRequest(Self, handle);
			return new CallResult<SteamUGCQueryCompleted_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetQueryUGCResult")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetQueryUGCResult(IntPtr self, UGCQueryHandle_t handle, uint index, ref SteamUGCDetails_t pDetails);

		internal bool GetQueryUGCResult(UGCQueryHandle_t handle, uint index, ref SteamUGCDetails_t pDetails)
		{
			return _GetQueryUGCResult(Self, handle, index, ref pDetails);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetQueryUGCPreviewURL")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetQueryUGCPreviewURL(IntPtr self, UGCQueryHandle_t handle, uint index, IntPtr pchURL, uint cchURLSize);

		internal bool GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, out string pchURL)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetQueryUGCPreviewURL(Self, handle, index, intPtr, 32768u);
			pchURL = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetQueryUGCMetadata")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetQueryUGCMetadata(IntPtr self, UGCQueryHandle_t handle, uint index, IntPtr pchMetadata, uint cchMetadatasize);

		internal bool GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, out string pchMetadata)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetQueryUGCMetadata(Self, handle, index, intPtr, 32768u);
			pchMetadata = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetQueryUGCChildren")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetQueryUGCChildren(IntPtr self, UGCQueryHandle_t handle, uint index, [In][Out] PublishedFileId[] pvecPublishedFileID, uint cMaxEntries);

		internal bool GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, [In][Out] PublishedFileId[] pvecPublishedFileID, uint cMaxEntries)
		{
			return _GetQueryUGCChildren(Self, handle, index, pvecPublishedFileID, cMaxEntries);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetQueryUGCStatistic")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetQueryUGCStatistic(IntPtr self, UGCQueryHandle_t handle, uint index, ItemStatistic eStatType, ref ulong pStatValue);

		internal bool GetQueryUGCStatistic(UGCQueryHandle_t handle, uint index, ItemStatistic eStatType, ref ulong pStatValue)
		{
			return _GetQueryUGCStatistic(Self, handle, index, eStatType, ref pStatValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetQueryUGCNumAdditionalPreviews")]
		private static extern uint _GetQueryUGCNumAdditionalPreviews(IntPtr self, UGCQueryHandle_t handle, uint index);

		internal uint GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index)
		{
			return _GetQueryUGCNumAdditionalPreviews(Self, handle, index);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetQueryUGCAdditionalPreview")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetQueryUGCAdditionalPreview(IntPtr self, UGCQueryHandle_t handle, uint index, uint previewIndex, IntPtr pchURLOrVideoID, uint cchURLSize, IntPtr pchOriginalFileName, uint cchOriginalFileNameSize, ref ItemPreviewType pPreviewType);

		internal bool GetQueryUGCAdditionalPreview(UGCQueryHandle_t handle, uint index, uint previewIndex, out string pchURLOrVideoID, out string pchOriginalFileName, ref ItemPreviewType pPreviewType)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			IntPtr intPtr2 = Helpers.TakeMemory();
			bool result = _GetQueryUGCAdditionalPreview(Self, handle, index, previewIndex, intPtr, 32768u, intPtr2, 32768u, ref pPreviewType);
			pchURLOrVideoID = Helpers.MemoryToString(intPtr);
			pchOriginalFileName = Helpers.MemoryToString(intPtr2);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetQueryUGCNumKeyValueTags")]
		private static extern uint _GetQueryUGCNumKeyValueTags(IntPtr self, UGCQueryHandle_t handle, uint index);

		internal uint GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index)
		{
			return _GetQueryUGCNumKeyValueTags(Self, handle, index);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetQueryUGCKeyValueTag")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetQueryUGCKeyValueTag(IntPtr self, UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, IntPtr pchKey, uint cchKeySize, IntPtr pchValue, uint cchValueSize);

		internal bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, out string pchKey, out string pchValue)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			IntPtr intPtr2 = Helpers.TakeMemory();
			bool result = _GetQueryUGCKeyValueTag(Self, handle, index, keyValueTagIndex, intPtr, 32768u, intPtr2, 32768u);
			pchKey = Helpers.MemoryToString(intPtr);
			pchValue = Helpers.MemoryToString(intPtr2);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetQueryFirstUGCKeyValueTag")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetQueryUGCKeyValueTag(IntPtr self, UGCQueryHandle_t handle, uint index, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, IntPtr pchValue, uint cchValueSize);

		internal bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, out string pchValue)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetQueryUGCKeyValueTag(Self, handle, index, pchKey, intPtr, 32768u);
			pchValue = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_ReleaseQueryUGCRequest")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ReleaseQueryUGCRequest(IntPtr self, UGCQueryHandle_t handle);

		internal bool ReleaseQueryUGCRequest(UGCQueryHandle_t handle)
		{
			return _ReleaseQueryUGCRequest(Self, handle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_AddRequiredTag")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _AddRequiredTag(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pTagName);

		internal bool AddRequiredTag(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pTagName)
		{
			return _AddRequiredTag(Self, handle, pTagName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_AddRequiredTagGroup")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _AddRequiredTagGroup(IntPtr self, UGCQueryHandle_t handle, ref SteamParamStringArray_t pTagGroups);

		internal bool AddRequiredTagGroup(UGCQueryHandle_t handle, ref SteamParamStringArray_t pTagGroups)
		{
			return _AddRequiredTagGroup(Self, handle, ref pTagGroups);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_AddExcludedTag")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _AddExcludedTag(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pTagName);

		internal bool AddExcludedTag(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pTagName)
		{
			return _AddExcludedTag(Self, handle, pTagName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetReturnOnlyIDs")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetReturnOnlyIDs(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnOnlyIDs);

		internal bool SetReturnOnlyIDs(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnOnlyIDs)
		{
			return _SetReturnOnlyIDs(Self, handle, bReturnOnlyIDs);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetReturnKeyValueTags")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetReturnKeyValueTags(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnKeyValueTags);

		internal bool SetReturnKeyValueTags(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnKeyValueTags)
		{
			return _SetReturnKeyValueTags(Self, handle, bReturnKeyValueTags);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetReturnLongDescription")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetReturnLongDescription(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnLongDescription);

		internal bool SetReturnLongDescription(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnLongDescription)
		{
			return _SetReturnLongDescription(Self, handle, bReturnLongDescription);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetReturnMetadata")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetReturnMetadata(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnMetadata);

		internal bool SetReturnMetadata(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnMetadata)
		{
			return _SetReturnMetadata(Self, handle, bReturnMetadata);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetReturnChildren")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetReturnChildren(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnChildren);

		internal bool SetReturnChildren(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnChildren)
		{
			return _SetReturnChildren(Self, handle, bReturnChildren);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetReturnAdditionalPreviews")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetReturnAdditionalPreviews(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnAdditionalPreviews);

		internal bool SetReturnAdditionalPreviews(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnAdditionalPreviews)
		{
			return _SetReturnAdditionalPreviews(Self, handle, bReturnAdditionalPreviews);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetReturnTotalOnly")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetReturnTotalOnly(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnTotalOnly);

		internal bool SetReturnTotalOnly(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bReturnTotalOnly)
		{
			return _SetReturnTotalOnly(Self, handle, bReturnTotalOnly);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetReturnPlaytimeStats")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetReturnPlaytimeStats(IntPtr self, UGCQueryHandle_t handle, uint unDays);

		internal bool SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint unDays)
		{
			return _SetReturnPlaytimeStats(Self, handle, unDays);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetLanguage")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetLanguage(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchLanguage);

		internal bool SetLanguage(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchLanguage)
		{
			return _SetLanguage(Self, handle, pchLanguage);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetAllowCachedResponse")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetAllowCachedResponse(IntPtr self, UGCQueryHandle_t handle, uint unMaxAgeSeconds);

		internal bool SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds)
		{
			return _SetAllowCachedResponse(Self, handle, unMaxAgeSeconds);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetCloudFileNameFilter")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetCloudFileNameFilter(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pMatchCloudFileName);

		internal bool SetCloudFileNameFilter(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pMatchCloudFileName)
		{
			return _SetCloudFileNameFilter(Self, handle, pMatchCloudFileName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetMatchAnyTag")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetMatchAnyTag(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bMatchAnyTag);

		internal bool SetMatchAnyTag(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bMatchAnyTag)
		{
			return _SetMatchAnyTag(Self, handle, bMatchAnyTag);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetSearchText")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetSearchText(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pSearchText);

		internal bool SetSearchText(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pSearchText)
		{
			return _SetSearchText(Self, handle, pSearchText);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetRankedByTrendDays")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetRankedByTrendDays(IntPtr self, UGCQueryHandle_t handle, uint unDays);

		internal bool SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays)
		{
			return _SetRankedByTrendDays(Self, handle, unDays);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_AddRequiredKeyValueTag")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _AddRequiredKeyValueTag(IntPtr self, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pValue);

		internal bool AddRequiredKeyValueTag(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pValue)
		{
			return _AddRequiredKeyValueTag(Self, handle, pKey, pValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_RequestUGCDetails")]
		private static extern SteamAPICall_t _RequestUGCDetails(IntPtr self, PublishedFileId nPublishedFileID, uint unMaxAgeSeconds);

		internal CallResult<SteamUGCRequestUGCDetailsResult_t> RequestUGCDetails(PublishedFileId nPublishedFileID, uint unMaxAgeSeconds)
		{
			SteamAPICall_t call = _RequestUGCDetails(Self, nPublishedFileID, unMaxAgeSeconds);
			return new CallResult<SteamUGCRequestUGCDetailsResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_CreateItem")]
		private static extern SteamAPICall_t _CreateItem(IntPtr self, AppId nConsumerAppId, WorkshopFileType eFileType);

		internal CallResult<CreateItemResult_t> CreateItem(AppId nConsumerAppId, WorkshopFileType eFileType)
		{
			SteamAPICall_t call = _CreateItem(Self, nConsumerAppId, eFileType);
			return new CallResult<CreateItemResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_StartItemUpdate")]
		private static extern UGCUpdateHandle_t _StartItemUpdate(IntPtr self, AppId nConsumerAppId, PublishedFileId nPublishedFileID);

		internal UGCUpdateHandle_t StartItemUpdate(AppId nConsumerAppId, PublishedFileId nPublishedFileID)
		{
			return _StartItemUpdate(Self, nConsumerAppId, nPublishedFileID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetItemTitle")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetItemTitle(IntPtr self, UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchTitle);

		internal bool SetItemTitle(UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchTitle)
		{
			return _SetItemTitle(Self, handle, pchTitle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetItemDescription")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetItemDescription(IntPtr self, UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchDescription);

		internal bool SetItemDescription(UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchDescription)
		{
			return _SetItemDescription(Self, handle, pchDescription);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetItemUpdateLanguage")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetItemUpdateLanguage(IntPtr self, UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchLanguage);

		internal bool SetItemUpdateLanguage(UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchLanguage)
		{
			return _SetItemUpdateLanguage(Self, handle, pchLanguage);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetItemMetadata")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetItemMetadata(IntPtr self, UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchMetaData);

		internal bool SetItemMetadata(UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchMetaData)
		{
			return _SetItemMetadata(Self, handle, pchMetaData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetItemVisibility")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetItemVisibility(IntPtr self, UGCUpdateHandle_t handle, RemoteStoragePublishedFileVisibility eVisibility);

		internal bool SetItemVisibility(UGCUpdateHandle_t handle, RemoteStoragePublishedFileVisibility eVisibility)
		{
			return _SetItemVisibility(Self, handle, eVisibility);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetItemTags")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetItemTags(IntPtr self, UGCUpdateHandle_t updateHandle, ref SteamParamStringArray_t pTags);

		internal bool SetItemTags(UGCUpdateHandle_t updateHandle, ref SteamParamStringArray_t pTags)
		{
			return _SetItemTags(Self, updateHandle, ref pTags);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetItemContent")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetItemContent(IntPtr self, UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszContentFolder);

		internal bool SetItemContent(UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszContentFolder)
		{
			return _SetItemContent(Self, handle, pszContentFolder);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetItemPreview")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetItemPreview(IntPtr self, UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszPreviewFile);

		internal bool SetItemPreview(UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszPreviewFile)
		{
			return _SetItemPreview(Self, handle, pszPreviewFile);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetAllowLegacyUpload")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetAllowLegacyUpload(IntPtr self, UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bAllowLegacyUpload);

		internal bool SetAllowLegacyUpload(UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.U1)] bool bAllowLegacyUpload)
		{
			return _SetAllowLegacyUpload(Self, handle, bAllowLegacyUpload);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_RemoveAllItemKeyValueTags")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _RemoveAllItemKeyValueTags(IntPtr self, UGCUpdateHandle_t handle);

		internal bool RemoveAllItemKeyValueTags(UGCUpdateHandle_t handle)
		{
			return _RemoveAllItemKeyValueTags(Self, handle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_RemoveItemKeyValueTags")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _RemoveItemKeyValueTags(IntPtr self, UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey);

		internal bool RemoveItemKeyValueTags(UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey)
		{
			return _RemoveItemKeyValueTags(Self, handle, pchKey);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_AddItemKeyValueTag")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _AddItemKeyValueTag(IntPtr self, UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue);

		internal bool AddItemKeyValueTag(UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue)
		{
			return _AddItemKeyValueTag(Self, handle, pchKey, pchValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_AddItemPreviewFile")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _AddItemPreviewFile(IntPtr self, UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszPreviewFile, ItemPreviewType type);

		internal bool AddItemPreviewFile(UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszPreviewFile, ItemPreviewType type)
		{
			return _AddItemPreviewFile(Self, handle, pszPreviewFile, type);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_AddItemPreviewVideo")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _AddItemPreviewVideo(IntPtr self, UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszVideoID);

		internal bool AddItemPreviewVideo(UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszVideoID)
		{
			return _AddItemPreviewVideo(Self, handle, pszVideoID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_UpdateItemPreviewFile")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _UpdateItemPreviewFile(IntPtr self, UGCUpdateHandle_t handle, uint index, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszPreviewFile);

		internal bool UpdateItemPreviewFile(UGCUpdateHandle_t handle, uint index, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszPreviewFile)
		{
			return _UpdateItemPreviewFile(Self, handle, index, pszPreviewFile);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_UpdateItemPreviewVideo")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _UpdateItemPreviewVideo(IntPtr self, UGCUpdateHandle_t handle, uint index, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszVideoID);

		internal bool UpdateItemPreviewVideo(UGCUpdateHandle_t handle, uint index, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszVideoID)
		{
			return _UpdateItemPreviewVideo(Self, handle, index, pszVideoID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_RemoveItemPreview")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _RemoveItemPreview(IntPtr self, UGCUpdateHandle_t handle, uint index);

		internal bool RemoveItemPreview(UGCUpdateHandle_t handle, uint index)
		{
			return _RemoveItemPreview(Self, handle, index);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SubmitItemUpdate")]
		private static extern SteamAPICall_t _SubmitItemUpdate(IntPtr self, UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchChangeNote);

		internal CallResult<SubmitItemUpdateResult_t> SubmitItemUpdate(UGCUpdateHandle_t handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchChangeNote)
		{
			SteamAPICall_t call = _SubmitItemUpdate(Self, handle, pchChangeNote);
			return new CallResult<SubmitItemUpdateResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetItemUpdateProgress")]
		private static extern ItemUpdateStatus _GetItemUpdateProgress(IntPtr self, UGCUpdateHandle_t handle, ref ulong punBytesProcessed, ref ulong punBytesTotal);

		internal ItemUpdateStatus GetItemUpdateProgress(UGCUpdateHandle_t handle, ref ulong punBytesProcessed, ref ulong punBytesTotal)
		{
			return _GetItemUpdateProgress(Self, handle, ref punBytesProcessed, ref punBytesTotal);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SetUserItemVote")]
		private static extern SteamAPICall_t _SetUserItemVote(IntPtr self, PublishedFileId nPublishedFileID, [MarshalAs(UnmanagedType.U1)] bool bVoteUp);

		internal CallResult<SetUserItemVoteResult_t> SetUserItemVote(PublishedFileId nPublishedFileID, [MarshalAs(UnmanagedType.U1)] bool bVoteUp)
		{
			SteamAPICall_t call = _SetUserItemVote(Self, nPublishedFileID, bVoteUp);
			return new CallResult<SetUserItemVoteResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetUserItemVote")]
		private static extern SteamAPICall_t _GetUserItemVote(IntPtr self, PublishedFileId nPublishedFileID);

		internal CallResult<GetUserItemVoteResult_t> GetUserItemVote(PublishedFileId nPublishedFileID)
		{
			SteamAPICall_t call = _GetUserItemVote(Self, nPublishedFileID);
			return new CallResult<GetUserItemVoteResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_AddItemToFavorites")]
		private static extern SteamAPICall_t _AddItemToFavorites(IntPtr self, AppId nAppId, PublishedFileId nPublishedFileID);

		internal CallResult<UserFavoriteItemsListChanged_t> AddItemToFavorites(AppId nAppId, PublishedFileId nPublishedFileID)
		{
			SteamAPICall_t call = _AddItemToFavorites(Self, nAppId, nPublishedFileID);
			return new CallResult<UserFavoriteItemsListChanged_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_RemoveItemFromFavorites")]
		private static extern SteamAPICall_t _RemoveItemFromFavorites(IntPtr self, AppId nAppId, PublishedFileId nPublishedFileID);

		internal CallResult<UserFavoriteItemsListChanged_t> RemoveItemFromFavorites(AppId nAppId, PublishedFileId nPublishedFileID)
		{
			SteamAPICall_t call = _RemoveItemFromFavorites(Self, nAppId, nPublishedFileID);
			return new CallResult<UserFavoriteItemsListChanged_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SubscribeItem")]
		private static extern SteamAPICall_t _SubscribeItem(IntPtr self, PublishedFileId nPublishedFileID);

		internal CallResult<RemoteStorageSubscribePublishedFileResult_t> SubscribeItem(PublishedFileId nPublishedFileID)
		{
			SteamAPICall_t call = _SubscribeItem(Self, nPublishedFileID);
			return new CallResult<RemoteStorageSubscribePublishedFileResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_UnsubscribeItem")]
		private static extern SteamAPICall_t _UnsubscribeItem(IntPtr self, PublishedFileId nPublishedFileID);

		internal CallResult<RemoteStorageUnsubscribePublishedFileResult_t> UnsubscribeItem(PublishedFileId nPublishedFileID)
		{
			SteamAPICall_t call = _UnsubscribeItem(Self, nPublishedFileID);
			return new CallResult<RemoteStorageUnsubscribePublishedFileResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetNumSubscribedItems")]
		private static extern uint _GetNumSubscribedItems(IntPtr self);

		internal uint GetNumSubscribedItems()
		{
			return _GetNumSubscribedItems(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetSubscribedItems")]
		private static extern uint _GetSubscribedItems(IntPtr self, [In][Out] PublishedFileId[] pvecPublishedFileID, uint cMaxEntries);

		internal uint GetSubscribedItems([In][Out] PublishedFileId[] pvecPublishedFileID, uint cMaxEntries)
		{
			return _GetSubscribedItems(Self, pvecPublishedFileID, cMaxEntries);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetItemState")]
		private static extern uint _GetItemState(IntPtr self, PublishedFileId nPublishedFileID);

		internal uint GetItemState(PublishedFileId nPublishedFileID)
		{
			return _GetItemState(Self, nPublishedFileID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetItemInstallInfo")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetItemInstallInfo(IntPtr self, PublishedFileId nPublishedFileID, ref ulong punSizeOnDisk, IntPtr pchFolder, uint cchFolderSize, ref uint punTimeStamp);

		internal bool GetItemInstallInfo(PublishedFileId nPublishedFileID, ref ulong punSizeOnDisk, out string pchFolder, ref uint punTimeStamp)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetItemInstallInfo(Self, nPublishedFileID, ref punSizeOnDisk, intPtr, 32768u, ref punTimeStamp);
			pchFolder = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetItemDownloadInfo")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetItemDownloadInfo(IntPtr self, PublishedFileId nPublishedFileID, ref ulong punBytesDownloaded, ref ulong punBytesTotal);

		internal bool GetItemDownloadInfo(PublishedFileId nPublishedFileID, ref ulong punBytesDownloaded, ref ulong punBytesTotal)
		{
			return _GetItemDownloadInfo(Self, nPublishedFileID, ref punBytesDownloaded, ref punBytesTotal);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_DownloadItem")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _DownloadItem(IntPtr self, PublishedFileId nPublishedFileID, [MarshalAs(UnmanagedType.U1)] bool bHighPriority);

		internal bool DownloadItem(PublishedFileId nPublishedFileID, [MarshalAs(UnmanagedType.U1)] bool bHighPriority)
		{
			return _DownloadItem(Self, nPublishedFileID, bHighPriority);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_BInitWorkshopForGameServer")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BInitWorkshopForGameServer(IntPtr self, DepotId_t unWorkshopDepotID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszFolder);

		internal bool BInitWorkshopForGameServer(DepotId_t unWorkshopDepotID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszFolder)
		{
			return _BInitWorkshopForGameServer(Self, unWorkshopDepotID, pszFolder);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_SuspendDownloads")]
		private static extern void _SuspendDownloads(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bSuspend);

		internal void SuspendDownloads([MarshalAs(UnmanagedType.U1)] bool bSuspend)
		{
			_SuspendDownloads(Self, bSuspend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_StartPlaytimeTracking")]
		private static extern SteamAPICall_t _StartPlaytimeTracking(IntPtr self, [In][Out] PublishedFileId[] pvecPublishedFileID, uint unNumPublishedFileIDs);

		internal CallResult<StartPlaytimeTrackingResult_t> StartPlaytimeTracking([In][Out] PublishedFileId[] pvecPublishedFileID, uint unNumPublishedFileIDs)
		{
			SteamAPICall_t call = _StartPlaytimeTracking(Self, pvecPublishedFileID, unNumPublishedFileIDs);
			return new CallResult<StartPlaytimeTrackingResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_StopPlaytimeTracking")]
		private static extern SteamAPICall_t _StopPlaytimeTracking(IntPtr self, [In][Out] PublishedFileId[] pvecPublishedFileID, uint unNumPublishedFileIDs);

		internal CallResult<StopPlaytimeTrackingResult_t> StopPlaytimeTracking([In][Out] PublishedFileId[] pvecPublishedFileID, uint unNumPublishedFileIDs)
		{
			SteamAPICall_t call = _StopPlaytimeTracking(Self, pvecPublishedFileID, unNumPublishedFileIDs);
			return new CallResult<StopPlaytimeTrackingResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_StopPlaytimeTrackingForAllItems")]
		private static extern SteamAPICall_t _StopPlaytimeTrackingForAllItems(IntPtr self);

		internal CallResult<StopPlaytimeTrackingResult_t> StopPlaytimeTrackingForAllItems()
		{
			SteamAPICall_t call = _StopPlaytimeTrackingForAllItems(Self);
			return new CallResult<StopPlaytimeTrackingResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_AddDependency")]
		private static extern SteamAPICall_t _AddDependency(IntPtr self, PublishedFileId nParentPublishedFileID, PublishedFileId nChildPublishedFileID);

		internal CallResult<AddUGCDependencyResult_t> AddDependency(PublishedFileId nParentPublishedFileID, PublishedFileId nChildPublishedFileID)
		{
			SteamAPICall_t call = _AddDependency(Self, nParentPublishedFileID, nChildPublishedFileID);
			return new CallResult<AddUGCDependencyResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_RemoveDependency")]
		private static extern SteamAPICall_t _RemoveDependency(IntPtr self, PublishedFileId nParentPublishedFileID, PublishedFileId nChildPublishedFileID);

		internal CallResult<RemoveUGCDependencyResult_t> RemoveDependency(PublishedFileId nParentPublishedFileID, PublishedFileId nChildPublishedFileID)
		{
			SteamAPICall_t call = _RemoveDependency(Self, nParentPublishedFileID, nChildPublishedFileID);
			return new CallResult<RemoveUGCDependencyResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_AddAppDependency")]
		private static extern SteamAPICall_t _AddAppDependency(IntPtr self, PublishedFileId nPublishedFileID, AppId nAppID);

		internal CallResult<AddAppDependencyResult_t> AddAppDependency(PublishedFileId nPublishedFileID, AppId nAppID)
		{
			SteamAPICall_t call = _AddAppDependency(Self, nPublishedFileID, nAppID);
			return new CallResult<AddAppDependencyResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_RemoveAppDependency")]
		private static extern SteamAPICall_t _RemoveAppDependency(IntPtr self, PublishedFileId nPublishedFileID, AppId nAppID);

		internal CallResult<RemoveAppDependencyResult_t> RemoveAppDependency(PublishedFileId nPublishedFileID, AppId nAppID)
		{
			SteamAPICall_t call = _RemoveAppDependency(Self, nPublishedFileID, nAppID);
			return new CallResult<RemoveAppDependencyResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_GetAppDependencies")]
		private static extern SteamAPICall_t _GetAppDependencies(IntPtr self, PublishedFileId nPublishedFileID);

		internal CallResult<GetAppDependenciesResult_t> GetAppDependencies(PublishedFileId nPublishedFileID)
		{
			SteamAPICall_t call = _GetAppDependencies(Self, nPublishedFileID);
			return new CallResult<GetAppDependenciesResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUGC_DeleteItem")]
		private static extern SteamAPICall_t _DeleteItem(IntPtr self, PublishedFileId nPublishedFileID);

		internal CallResult<DeleteItemResult_t> DeleteItem(PublishedFileId nPublishedFileID)
		{
			SteamAPICall_t call = _DeleteItem(Self, nPublishedFileID);
			return new CallResult<DeleteItemResult_t>(call, base.IsServer);
		}
	}
}
