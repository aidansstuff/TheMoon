using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public struct InventoryItem : IEquatable<InventoryItem>
	{
		public struct Amount
		{
			public InventoryItem Item;

			public int Quantity;
		}

		internal InventoryItemId _id;

		internal InventoryDefId _def;

		internal SteamItemFlags _flags;

		internal ushort _quantity;

		internal Dictionary<string, string> _properties;

		public InventoryItemId Id => _id;

		public InventoryDefId DefId => _def;

		public int Quantity => _quantity;

		public InventoryDef Def => SteamInventory.FindDefinition(DefId);

		public Dictionary<string, string> Properties => _properties;

		public bool IsNoTrade => _flags.HasFlag(SteamItemFlags.NoTrade);

		public bool IsRemoved => _flags.HasFlag(SteamItemFlags.Removed);

		public bool IsConsumed => _flags.HasFlag(SteamItemFlags.Consumed);

		public DateTime Acquired
		{
			get
			{
				if (Properties == null)
				{
					return DateTime.UtcNow;
				}
				if (Properties.TryGetValue("acquired", out var value))
				{
					int year = int.Parse(value.Substring(0, 4));
					int month = int.Parse(value.Substring(4, 2));
					int day = int.Parse(value.Substring(6, 2));
					int hour = int.Parse(value.Substring(9, 2));
					int minute = int.Parse(value.Substring(11, 2));
					int second = int.Parse(value.Substring(13, 2));
					return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
				}
				return DateTime.UtcNow;
			}
		}

		public string Origin
		{
			get
			{
				if (Properties == null)
				{
					return null;
				}
				if (Properties.TryGetValue("origin", out var value))
				{
					return value;
				}
				return null;
			}
		}

		public async Task<InventoryResult?> ConsumeAsync(int amount = 1)
		{
			SteamInventoryResult_t sresult = Defines.k_SteamInventoryResultInvalid;
			if (!SteamInventory.Internal.ConsumeItem(ref sresult, Id, (uint)amount))
			{
				return null;
			}
			return await InventoryResult.GetAsync(sresult);
		}

		public async Task<InventoryResult?> SplitStackAsync(int quantity = 1)
		{
			SteamInventoryResult_t sresult = Defines.k_SteamInventoryResultInvalid;
			if (!SteamInventory.Internal.TransferItemQuantity(ref sresult, Id, (uint)quantity, ulong.MaxValue))
			{
				return null;
			}
			return await InventoryResult.GetAsync(sresult);
		}

		public async Task<InventoryResult?> AddAsync(InventoryItem add, int quantity = 1)
		{
			SteamInventoryResult_t sresult = Defines.k_SteamInventoryResultInvalid;
			if (!SteamInventory.Internal.TransferItemQuantity(ref sresult, add.Id, (uint)quantity, Id))
			{
				return null;
			}
			return await InventoryResult.GetAsync(sresult);
		}

		internal static InventoryItem From(SteamItemDetails_t details)
		{
			InventoryItem result = default(InventoryItem);
			result._id = details.ItemId;
			result._def = details.Definition;
			result._flags = (SteamItemFlags)details.Flags;
			result._quantity = details.Quantity;
			return result;
		}

		internal static Dictionary<string, string> GetProperties(SteamInventoryResult_t result, int index)
		{
			uint punValueBufferSizeOut = 32768u;
			if (!SteamInventory.Internal.GetResultItemProperty(result, (uint)index, null, out var pchValueBuffer, ref punValueBufferSizeOut))
			{
				return null;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = pchValueBuffer.Split(new char[1] { ',' });
			foreach (string text in array)
			{
				punValueBufferSizeOut = 32768u;
				if (SteamInventory.Internal.GetResultItemProperty(result, (uint)index, text, out var pchValueBuffer2, ref punValueBufferSizeOut))
				{
					dictionary.Add(text, pchValueBuffer2);
				}
			}
			return dictionary;
		}

		public static bool operator ==(InventoryItem a, InventoryItem b)
		{
			return a._id == b._id;
		}

		public static bool operator !=(InventoryItem a, InventoryItem b)
		{
			return a._id != b._id;
		}

		public override bool Equals(object p)
		{
			return Equals((InventoryItem)p);
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}

		public bool Equals(InventoryItem p)
		{
			return p._id == _id;
		}
	}
}
