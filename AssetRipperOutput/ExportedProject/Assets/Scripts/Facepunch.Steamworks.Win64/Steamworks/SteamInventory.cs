using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamInventory : SteamSharedClass<SteamInventory>
	{
		private static Dictionary<int, InventoryDef> _defMap;

		internal static ISteamInventory Internal => SteamSharedClass<SteamInventory>.Interface as ISteamInventory;

		public static string Currency { get; internal set; }

		public static InventoryItem[] Items { get; internal set; }

		public static InventoryDef[] Definitions { get; internal set; }

		public static event Action<InventoryResult> OnInventoryUpdated;

		public static event Action OnDefinitionsUpdated;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamInventory(server));
			InstallEvents(server);
		}

		internal static void InstallEvents(bool server)
		{
			if (!server)
			{
				Dispatch.Install(delegate(SteamInventoryFullUpdate_t x)
				{
					InventoryUpdated(x);
				});
			}
			Dispatch.Install<SteamInventoryDefinitionUpdate_t>(delegate
			{
				LoadDefinitions();
			}, server);
		}

		private static void InventoryUpdated(SteamInventoryFullUpdate_t x)
		{
			InventoryResult obj = new InventoryResult(x.Handle, expired: false);
			Items = obj.GetItems();
			SteamInventory.OnInventoryUpdated?.Invoke(obj);
		}

		private static void LoadDefinitions()
		{
			Definitions = GetDefinitions();
			if (Definitions != null)
			{
				_defMap = new Dictionary<int, InventoryDef>();
				InventoryDef[] definitions = Definitions;
				foreach (InventoryDef inventoryDef in definitions)
				{
					_defMap[inventoryDef.Id] = inventoryDef;
				}
				SteamInventory.OnDefinitionsUpdated?.Invoke();
			}
		}

		public static void LoadItemDefinitions()
		{
			if (Definitions == null)
			{
				LoadDefinitions();
			}
			Internal.LoadItemDefinitions();
		}

		public static async Task<bool> WaitForDefinitions(float timeoutSeconds = 30f)
		{
			if (Definitions != null)
			{
				return true;
			}
			LoadDefinitions();
			LoadItemDefinitions();
			if (Definitions != null)
			{
				return true;
			}
			Stopwatch sw = Stopwatch.StartNew();
			while (Definitions == null)
			{
				if (sw.Elapsed.TotalSeconds > (double)timeoutSeconds)
				{
					return false;
				}
				await Task.Delay(10);
			}
			return true;
		}

		public static InventoryDef FindDefinition(InventoryDefId defId)
		{
			if (_defMap == null)
			{
				return null;
			}
			if (_defMap.TryGetValue(defId, out var value))
			{
				return value;
			}
			return null;
		}

		public static async Task<InventoryDef[]> GetDefinitionsWithPricesAsync()
		{
			SteamInventoryRequestPricesResult_t? priceRequest = await Internal.RequestPrices();
			if (!priceRequest.HasValue || priceRequest.Value.Result != Result.OK)
			{
				return null;
			}
			Currency = priceRequest?.CurrencyUTF8();
			uint num = Internal.GetNumItemsWithPrices();
			if (num == 0)
			{
				return null;
			}
			InventoryDefId[] defs = new InventoryDefId[num];
			ulong[] currentPrices = new ulong[num];
			ulong[] baseprices = new ulong[num];
			if (!Internal.GetItemsWithPrices(defs, currentPrices, baseprices, num))
			{
				return null;
			}
			return defs.Select((InventoryDefId x) => new InventoryDef(x)).ToArray();
		}

		internal static InventoryDef[] GetDefinitions()
		{
			uint punItemDefIDsArraySize = 0u;
			if (!Internal.GetItemDefinitionIDs(null, ref punItemDefIDsArraySize))
			{
				return null;
			}
			InventoryDefId[] array = new InventoryDefId[punItemDefIDsArraySize];
			if (!Internal.GetItemDefinitionIDs(array, ref punItemDefIDsArraySize))
			{
				return null;
			}
			return array.Select((InventoryDefId x) => new InventoryDef(x)).ToArray();
		}

		public static bool GetAllItems()
		{
			SteamInventoryResult_t pResultHandle = Defines.k_SteamInventoryResultInvalid;
			return Internal.GetAllItems(ref pResultHandle);
		}

		public static async Task<InventoryResult?> GetAllItemsAsync()
		{
			SteamInventoryResult_t sresult = Defines.k_SteamInventoryResultInvalid;
			if (!Internal.GetAllItems(ref sresult))
			{
				return null;
			}
			return await InventoryResult.GetAsync(sresult);
		}

		public static async Task<InventoryResult?> GenerateItemAsync(InventoryDef target, int amount)
		{
			SteamInventoryResult_t sresult = Defines.k_SteamInventoryResultInvalid;
			InventoryDefId[] defs = new InventoryDefId[1] { target.Id };
			uint[] cnts = new uint[1] { (uint)amount };
			if (!Internal.GenerateItems(ref sresult, defs, cnts, 1u))
			{
				return null;
			}
			return await InventoryResult.GetAsync(sresult);
		}

		public static async Task<InventoryResult?> CraftItemAsync(InventoryItem[] list, InventoryDef target)
		{
			SteamInventoryResult_t sresult = Defines.k_SteamInventoryResultInvalid;
			InventoryDefId[] give = new InventoryDefId[1] { target.Id };
			uint[] givec = new uint[1] { 1u };
			InventoryItemId[] sell = list.Select((InventoryItem x) => x.Id).ToArray();
			uint[] sellc = list.Select((InventoryItem x) => 1u).ToArray();
			if (!Internal.ExchangeItems(ref sresult, give, givec, 1u, sell, sellc, (uint)sell.Length))
			{
				return null;
			}
			return await InventoryResult.GetAsync(sresult);
		}

		public static async Task<InventoryResult?> CraftItemAsync(InventoryItem.Amount[] list, InventoryDef target)
		{
			SteamInventoryResult_t sresult = Defines.k_SteamInventoryResultInvalid;
			InventoryDefId[] give = new InventoryDefId[1] { target.Id };
			uint[] givec = new uint[1] { 1u };
			InventoryItemId[] sell = list.Select((InventoryItem.Amount x) => x.Item.Id).ToArray();
			uint[] sellc = list.Select((InventoryItem.Amount x) => (uint)x.Quantity).ToArray();
			if (!Internal.ExchangeItems(ref sresult, give, givec, 1u, sell, sellc, (uint)sell.Length))
			{
				return null;
			}
			return await InventoryResult.GetAsync(sresult);
		}

		public static async Task<InventoryResult?> DeserializeAsync(byte[] data, int dataLength = -1)
		{
			if (data == null)
			{
				throw new ArgumentException("data should not be null");
			}
			if (dataLength == -1)
			{
				dataLength = data.Length;
			}
			IntPtr ptr = Marshal.AllocHGlobal(dataLength);
			try
			{
				Marshal.Copy(data, 0, ptr, dataLength);
				SteamInventoryResult_t sresult = Defines.k_SteamInventoryResultInvalid;
				if (!Internal.DeserializeResult(ref sresult, ptr, (uint)dataLength, bRESERVED_MUST_BE_FALSE: false))
				{
					return null;
				}
				return await InventoryResult.GetAsync(sresult.Value);
			}
			finally
			{
				Marshal.FreeHGlobal(ptr);
			}
		}

		public static async Task<InventoryResult?> GrantPromoItemsAsync()
		{
			SteamInventoryResult_t sresult = Defines.k_SteamInventoryResultInvalid;
			if (!Internal.GrantPromoItems(ref sresult))
			{
				return null;
			}
			return await InventoryResult.GetAsync(sresult);
		}

		public static async Task<InventoryResult?> TriggerItemDropAsync(InventoryDefId id)
		{
			SteamInventoryResult_t sresult = Defines.k_SteamInventoryResultInvalid;
			if (!Internal.TriggerItemDrop(ref sresult, id))
			{
				return null;
			}
			return await InventoryResult.GetAsync(sresult);
		}

		public static async Task<InventoryResult?> AddPromoItemAsync(InventoryDefId id)
		{
			SteamInventoryResult_t sresult = Defines.k_SteamInventoryResultInvalid;
			if (!Internal.AddPromoItem(ref sresult, id))
			{
				return null;
			}
			return await InventoryResult.GetAsync(sresult);
		}

		public static async Task<InventoryPurchaseResult?> StartPurchaseAsync(InventoryDef[] items)
		{
			InventoryDefId[] item_i = items.Select((InventoryDef x) => x._id).ToArray();
			uint[] item_q = items.Select((InventoryDef x) => 1u).ToArray();
			SteamInventoryStartPurchaseResult_t? r = await Internal.StartPurchase(item_i, item_q, (uint)item_i.Length);
			if (!r.HasValue)
			{
				return null;
			}
			InventoryPurchaseResult value = default(InventoryPurchaseResult);
			value.Result = r.Value.Result;
			value.OrderID = r.Value.OrderID;
			value.TransID = r.Value.TransID;
			return value;
		}
	}
}
