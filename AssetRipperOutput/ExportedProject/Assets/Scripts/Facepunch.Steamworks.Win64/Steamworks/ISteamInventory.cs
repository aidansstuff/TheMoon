using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamInventory : SteamInterface
	{
		internal ISteamInventory(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamInventory_v003();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamInventory_v003();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamGameServerInventory_v003();

		public override IntPtr GetServerInterfacePointer()
		{
			return SteamAPI_SteamGameServerInventory_v003();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GetResultStatus")]
		private static extern Result _GetResultStatus(IntPtr self, SteamInventoryResult_t resultHandle);

		internal Result GetResultStatus(SteamInventoryResult_t resultHandle)
		{
			return _GetResultStatus(Self, resultHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GetResultItems")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetResultItems(IntPtr self, SteamInventoryResult_t resultHandle, [In][Out] SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize);

		internal bool GetResultItems(SteamInventoryResult_t resultHandle, [In][Out] SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize)
		{
			return _GetResultItems(Self, resultHandle, pOutItemsArray, ref punOutItemsArraySize);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GetResultItemProperty")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetResultItemProperty(IntPtr self, SteamInventoryResult_t resultHandle, uint unItemIndex, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut);

		internal bool GetResultItemProperty(SteamInventoryResult_t resultHandle, uint unItemIndex, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName, out string pchValueBuffer, ref uint punValueBufferSizeOut)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetResultItemProperty(Self, resultHandle, unItemIndex, pchPropertyName, intPtr, ref punValueBufferSizeOut);
			pchValueBuffer = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GetResultTimestamp")]
		private static extern uint _GetResultTimestamp(IntPtr self, SteamInventoryResult_t resultHandle);

		internal uint GetResultTimestamp(SteamInventoryResult_t resultHandle)
		{
			return _GetResultTimestamp(Self, resultHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_CheckResultSteamID")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _CheckResultSteamID(IntPtr self, SteamInventoryResult_t resultHandle, SteamId steamIDExpected);

		internal bool CheckResultSteamID(SteamInventoryResult_t resultHandle, SteamId steamIDExpected)
		{
			return _CheckResultSteamID(Self, resultHandle, steamIDExpected);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_DestroyResult")]
		private static extern void _DestroyResult(IntPtr self, SteamInventoryResult_t resultHandle);

		internal void DestroyResult(SteamInventoryResult_t resultHandle)
		{
			_DestroyResult(Self, resultHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GetAllItems")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetAllItems(IntPtr self, ref SteamInventoryResult_t pResultHandle);

		internal bool GetAllItems(ref SteamInventoryResult_t pResultHandle)
		{
			return _GetAllItems(Self, ref pResultHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GetItemsByID")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetItemsByID(IntPtr self, ref SteamInventoryResult_t pResultHandle, ref InventoryItemId pInstanceIDs, uint unCountInstanceIDs);

		internal bool GetItemsByID(ref SteamInventoryResult_t pResultHandle, ref InventoryItemId pInstanceIDs, uint unCountInstanceIDs)
		{
			return _GetItemsByID(Self, ref pResultHandle, ref pInstanceIDs, unCountInstanceIDs);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_SerializeResult")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SerializeResult(IntPtr self, SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, ref uint punOutBufferSize);

		internal bool SerializeResult(SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, ref uint punOutBufferSize)
		{
			return _SerializeResult(Self, resultHandle, pOutBuffer, ref punOutBufferSize);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_DeserializeResult")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _DeserializeResult(IntPtr self, ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, [MarshalAs(UnmanagedType.U1)] bool bRESERVED_MUST_BE_FALSE);

		internal bool DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, [MarshalAs(UnmanagedType.U1)] bool bRESERVED_MUST_BE_FALSE)
		{
			return _DeserializeResult(Self, ref pOutResultHandle, pBuffer, unBufferSize, bRESERVED_MUST_BE_FALSE);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GenerateItems")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GenerateItems(IntPtr self, ref SteamInventoryResult_t pResultHandle, [In][Out] InventoryDefId[] pArrayItemDefs, [In][Out] uint[] punArrayQuantity, uint unArrayLength);

		internal bool GenerateItems(ref SteamInventoryResult_t pResultHandle, [In][Out] InventoryDefId[] pArrayItemDefs, [In][Out] uint[] punArrayQuantity, uint unArrayLength)
		{
			return _GenerateItems(Self, ref pResultHandle, pArrayItemDefs, punArrayQuantity, unArrayLength);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GrantPromoItems")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GrantPromoItems(IntPtr self, ref SteamInventoryResult_t pResultHandle);

		internal bool GrantPromoItems(ref SteamInventoryResult_t pResultHandle)
		{
			return _GrantPromoItems(Self, ref pResultHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_AddPromoItem")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _AddPromoItem(IntPtr self, ref SteamInventoryResult_t pResultHandle, InventoryDefId itemDef);

		internal bool AddPromoItem(ref SteamInventoryResult_t pResultHandle, InventoryDefId itemDef)
		{
			return _AddPromoItem(Self, ref pResultHandle, itemDef);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_AddPromoItems")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _AddPromoItems(IntPtr self, ref SteamInventoryResult_t pResultHandle, [In][Out] InventoryDefId[] pArrayItemDefs, uint unArrayLength);

		internal bool AddPromoItems(ref SteamInventoryResult_t pResultHandle, [In][Out] InventoryDefId[] pArrayItemDefs, uint unArrayLength)
		{
			return _AddPromoItems(Self, ref pResultHandle, pArrayItemDefs, unArrayLength);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_ConsumeItem")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ConsumeItem(IntPtr self, ref SteamInventoryResult_t pResultHandle, InventoryItemId itemConsume, uint unQuantity);

		internal bool ConsumeItem(ref SteamInventoryResult_t pResultHandle, InventoryItemId itemConsume, uint unQuantity)
		{
			return _ConsumeItem(Self, ref pResultHandle, itemConsume, unQuantity);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_ExchangeItems")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ExchangeItems(IntPtr self, ref SteamInventoryResult_t pResultHandle, [In][Out] InventoryDefId[] pArrayGenerate, [In][Out] uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, [In][Out] InventoryItemId[] pArrayDestroy, [In][Out] uint[] punArrayDestroyQuantity, uint unArrayDestroyLength);

		internal bool ExchangeItems(ref SteamInventoryResult_t pResultHandle, [In][Out] InventoryDefId[] pArrayGenerate, [In][Out] uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, [In][Out] InventoryItemId[] pArrayDestroy, [In][Out] uint[] punArrayDestroyQuantity, uint unArrayDestroyLength)
		{
			return _ExchangeItems(Self, ref pResultHandle, pArrayGenerate, punArrayGenerateQuantity, unArrayGenerateLength, pArrayDestroy, punArrayDestroyQuantity, unArrayDestroyLength);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_TransferItemQuantity")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _TransferItemQuantity(IntPtr self, ref SteamInventoryResult_t pResultHandle, InventoryItemId itemIdSource, uint unQuantity, InventoryItemId itemIdDest);

		internal bool TransferItemQuantity(ref SteamInventoryResult_t pResultHandle, InventoryItemId itemIdSource, uint unQuantity, InventoryItemId itemIdDest)
		{
			return _TransferItemQuantity(Self, ref pResultHandle, itemIdSource, unQuantity, itemIdDest);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_SendItemDropHeartbeat")]
		private static extern void _SendItemDropHeartbeat(IntPtr self);

		internal void SendItemDropHeartbeat()
		{
			_SendItemDropHeartbeat(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_TriggerItemDrop")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _TriggerItemDrop(IntPtr self, ref SteamInventoryResult_t pResultHandle, InventoryDefId dropListDefinition);

		internal bool TriggerItemDrop(ref SteamInventoryResult_t pResultHandle, InventoryDefId dropListDefinition)
		{
			return _TriggerItemDrop(Self, ref pResultHandle, dropListDefinition);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_TradeItems")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _TradeItems(IntPtr self, ref SteamInventoryResult_t pResultHandle, SteamId steamIDTradePartner, [In][Out] InventoryItemId[] pArrayGive, [In][Out] uint[] pArrayGiveQuantity, uint nArrayGiveLength, [In][Out] InventoryItemId[] pArrayGet, [In][Out] uint[] pArrayGetQuantity, uint nArrayGetLength);

		internal bool TradeItems(ref SteamInventoryResult_t pResultHandle, SteamId steamIDTradePartner, [In][Out] InventoryItemId[] pArrayGive, [In][Out] uint[] pArrayGiveQuantity, uint nArrayGiveLength, [In][Out] InventoryItemId[] pArrayGet, [In][Out] uint[] pArrayGetQuantity, uint nArrayGetLength)
		{
			return _TradeItems(Self, ref pResultHandle, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_LoadItemDefinitions")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _LoadItemDefinitions(IntPtr self);

		internal bool LoadItemDefinitions()
		{
			return _LoadItemDefinitions(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GetItemDefinitionIDs")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetItemDefinitionIDs(IntPtr self, [In][Out] InventoryDefId[] pItemDefIDs, ref uint punItemDefIDsArraySize);

		internal bool GetItemDefinitionIDs([In][Out] InventoryDefId[] pItemDefIDs, ref uint punItemDefIDsArraySize)
		{
			return _GetItemDefinitionIDs(Self, pItemDefIDs, ref punItemDefIDsArraySize);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GetItemDefinitionProperty")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetItemDefinitionProperty(IntPtr self, InventoryDefId iDefinition, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut);

		internal bool GetItemDefinitionProperty(InventoryDefId iDefinition, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName, out string pchValueBuffer, ref uint punValueBufferSizeOut)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetItemDefinitionProperty(Self, iDefinition, pchPropertyName, intPtr, ref punValueBufferSizeOut);
			pchValueBuffer = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_RequestEligiblePromoItemDefinitionsIDs")]
		private static extern SteamAPICall_t _RequestEligiblePromoItemDefinitionsIDs(IntPtr self, SteamId steamID);

		internal CallResult<SteamInventoryEligiblePromoItemDefIDs_t> RequestEligiblePromoItemDefinitionsIDs(SteamId steamID)
		{
			SteamAPICall_t call = _RequestEligiblePromoItemDefinitionsIDs(Self, steamID);
			return new CallResult<SteamInventoryEligiblePromoItemDefIDs_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GetEligiblePromoItemDefinitionIDs")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetEligiblePromoItemDefinitionIDs(IntPtr self, SteamId steamID, [In][Out] InventoryDefId[] pItemDefIDs, ref uint punItemDefIDsArraySize);

		internal bool GetEligiblePromoItemDefinitionIDs(SteamId steamID, [In][Out] InventoryDefId[] pItemDefIDs, ref uint punItemDefIDsArraySize)
		{
			return _GetEligiblePromoItemDefinitionIDs(Self, steamID, pItemDefIDs, ref punItemDefIDsArraySize);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_StartPurchase")]
		private static extern SteamAPICall_t _StartPurchase(IntPtr self, [In][Out] InventoryDefId[] pArrayItemDefs, [In][Out] uint[] punArrayQuantity, uint unArrayLength);

		internal CallResult<SteamInventoryStartPurchaseResult_t> StartPurchase([In][Out] InventoryDefId[] pArrayItemDefs, [In][Out] uint[] punArrayQuantity, uint unArrayLength)
		{
			SteamAPICall_t call = _StartPurchase(Self, pArrayItemDefs, punArrayQuantity, unArrayLength);
			return new CallResult<SteamInventoryStartPurchaseResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_RequestPrices")]
		private static extern SteamAPICall_t _RequestPrices(IntPtr self);

		internal CallResult<SteamInventoryRequestPricesResult_t> RequestPrices()
		{
			SteamAPICall_t call = _RequestPrices(Self);
			return new CallResult<SteamInventoryRequestPricesResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GetNumItemsWithPrices")]
		private static extern uint _GetNumItemsWithPrices(IntPtr self);

		internal uint GetNumItemsWithPrices()
		{
			return _GetNumItemsWithPrices(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GetItemsWithPrices")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetItemsWithPrices(IntPtr self, [In][Out] InventoryDefId[] pArrayItemDefs, [In][Out] ulong[] pCurrentPrices, [In][Out] ulong[] pBasePrices, uint unArrayLength);

		internal bool GetItemsWithPrices([In][Out] InventoryDefId[] pArrayItemDefs, [In][Out] ulong[] pCurrentPrices, [In][Out] ulong[] pBasePrices, uint unArrayLength)
		{
			return _GetItemsWithPrices(Self, pArrayItemDefs, pCurrentPrices, pBasePrices, unArrayLength);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_GetItemPrice")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetItemPrice(IntPtr self, InventoryDefId iDefinition, ref ulong pCurrentPrice, ref ulong pBasePrice);

		internal bool GetItemPrice(InventoryDefId iDefinition, ref ulong pCurrentPrice, ref ulong pBasePrice)
		{
			return _GetItemPrice(Self, iDefinition, ref pCurrentPrice, ref pBasePrice);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_StartUpdateProperties")]
		private static extern SteamInventoryUpdateHandle_t _StartUpdateProperties(IntPtr self);

		internal SteamInventoryUpdateHandle_t StartUpdateProperties()
		{
			return _StartUpdateProperties(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_RemoveProperty")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _RemoveProperty(IntPtr self, SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName);

		internal bool RemoveProperty(SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName)
		{
			return _RemoveProperty(Self, handle, nItemID, pchPropertyName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_SetPropertyString")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetProperty(IntPtr self, SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyValue);

		internal bool SetProperty(SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyValue)
		{
			return _SetProperty(Self, handle, nItemID, pchPropertyName, pchPropertyValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_SetPropertyBool")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetProperty(IntPtr self, SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName, [MarshalAs(UnmanagedType.U1)] bool bValue);

		internal bool SetProperty(SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName, [MarshalAs(UnmanagedType.U1)] bool bValue)
		{
			return _SetProperty(Self, handle, nItemID, pchPropertyName, bValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_SetPropertyInt64")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetProperty(IntPtr self, SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName, long nValue);

		internal bool SetProperty(SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName, long nValue)
		{
			return _SetProperty(Self, handle, nItemID, pchPropertyName, nValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_SetPropertyFloat")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetProperty(IntPtr self, SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName, float flValue);

		internal bool SetProperty(SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPropertyName, float flValue)
		{
			return _SetProperty(Self, handle, nItemID, pchPropertyName, flValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInventory_SubmitUpdateProperties")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SubmitUpdateProperties(IntPtr self, SteamInventoryUpdateHandle_t handle, ref SteamInventoryResult_t pResultHandle);

		internal bool SubmitUpdateProperties(SteamInventoryUpdateHandle_t handle, ref SteamInventoryResult_t pResultHandle)
		{
			return _SubmitUpdateProperties(Self, handle, ref pResultHandle);
		}
	}
}
