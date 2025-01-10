using System;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public struct InventoryResult : IDisposable
	{
		internal SteamInventoryResult_t _id;

		public bool Expired { get; internal set; }

		public int ItemCount
		{
			get
			{
				uint punOutItemsArraySize = 0u;
				if (!SteamInventory.Internal.GetResultItems(_id, null, ref punOutItemsArraySize))
				{
					return 0;
				}
				return (int)punOutItemsArraySize;
			}
		}

		internal InventoryResult(SteamInventoryResult_t id, bool expired)
		{
			_id = id;
			Expired = expired;
		}

		public bool BelongsTo(SteamId steamId)
		{
			return SteamInventory.Internal.CheckResultSteamID(_id, steamId);
		}

		public InventoryItem[] GetItems(bool includeProperties = false)
		{
			uint punOutItemsArraySize = (uint)ItemCount;
			if (punOutItemsArraySize == 0)
			{
				return null;
			}
			SteamItemDetails_t[] array = new SteamItemDetails_t[punOutItemsArraySize];
			if (!SteamInventory.Internal.GetResultItems(_id, array, ref punOutItemsArraySize))
			{
				return null;
			}
			InventoryItem[] array2 = new InventoryItem[punOutItemsArraySize];
			for (int i = 0; i < punOutItemsArraySize; i++)
			{
				InventoryItem inventoryItem = InventoryItem.From(array[i]);
				if (includeProperties)
				{
					inventoryItem._properties = InventoryItem.GetProperties(_id, i);
				}
				array2[i] = inventoryItem;
			}
			return array2;
		}

		public void Dispose()
		{
			if (_id.Value != -1)
			{
				SteamInventory.Internal.DestroyResult(_id);
			}
		}

		internal static async Task<InventoryResult?> GetAsync(SteamInventoryResult_t sresult)
		{
			Result _result = Result.Pending;
			int num;
			while (true)
			{
				switch (_result)
				{
				case Result.Pending:
					_result = SteamInventory.Internal.GetResultStatus(sresult);
					await Task.Delay(10);
					continue;
				default:
					num = ((_result != Result.Expired) ? 1 : 0);
					break;
				case Result.OK:
					num = 0;
					break;
				}
				break;
			}
			if (num != 0)
			{
				return null;
			}
			return new InventoryResult(sresult, _result == Result.Expired);
		}

		public unsafe byte[] Serialize()
		{
			uint punOutBufferSize = 0u;
			if (!SteamInventory.Internal.SerializeResult(_id, IntPtr.Zero, ref punOutBufferSize))
			{
				return null;
			}
			byte[] array = new byte[punOutBufferSize];
			fixed (byte* ptr = array)
			{
				if (!SteamInventory.Internal.SerializeResult(_id, (IntPtr)ptr, ref punOutBufferSize))
				{
					return null;
				}
			}
			return array;
		}
	}
}
