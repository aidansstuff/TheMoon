using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks.Data;

namespace Steamworks
{
	public class InventoryDef : IEquatable<InventoryDef>
	{
		internal InventoryDefId _id;

		internal Dictionary<string, string> _properties;

		private InventoryRecipe[] _recContaining;

		public int Id => _id.Value;

		public string Name => GetProperty("name");

		public string Description => GetProperty("description");

		public string IconUrl => GetProperty("icon_url");

		public string IconUrlLarge => GetProperty("icon_url_large");

		public string PriceCategory => GetProperty("price_category");

		public string Type => GetProperty("type");

		public bool IsGenerator => Type == "generator";

		public string ExchangeSchema => GetProperty("exchange");

		public bool Marketable => GetBoolProperty("marketable");

		public bool Tradable => GetBoolProperty("tradable");

		public DateTime Created => GetProperty<DateTime>("timestamp");

		public DateTime Modified => GetProperty<DateTime>("modified");

		public IEnumerable<KeyValuePair<string, string>> Properties
		{
			get
			{
				string list = GetProperty(null);
				string[] keys = list.Split(new char[1] { ',' });
				string[] array = keys;
				foreach (string key in array)
				{
					yield return new KeyValuePair<string, string>(key, GetProperty(key));
				}
			}
		}

		public int LocalPrice
		{
			get
			{
				ulong pCurrentPrice = 0uL;
				ulong pBasePrice = 0uL;
				if (!SteamInventory.Internal.GetItemPrice(Id, ref pCurrentPrice, ref pBasePrice))
				{
					return 0;
				}
				return (int)pCurrentPrice;
			}
		}

		public string LocalPriceFormatted => Utility.FormatPrice(SteamInventory.Currency, (double)LocalPrice / 100.0);

		public int LocalBasePrice
		{
			get
			{
				ulong pCurrentPrice = 0uL;
				ulong pBasePrice = 0uL;
				if (!SteamInventory.Internal.GetItemPrice(Id, ref pCurrentPrice, ref pBasePrice))
				{
					return 0;
				}
				return (int)pBasePrice;
			}
		}

		public string LocalBasePriceFormatted => Utility.FormatPrice(SteamInventory.Currency, (double)LocalPrice / 100.0);

		public InventoryDef(InventoryDefId defId)
		{
			_id = defId;
		}

		public InventoryRecipe[] GetRecipes()
		{
			if (string.IsNullOrEmpty(ExchangeSchema))
			{
				return null;
			}
			string[] source = ExchangeSchema.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			return source.Select((string x) => InventoryRecipe.FromString(x, this)).ToArray();
		}

		public string GetProperty(string name)
		{
			if (_properties != null && _properties.TryGetValue(name, out var value))
			{
				return value;
			}
			uint punValueBufferSizeOut = 32768u;
			if (!SteamInventory.Internal.GetItemDefinitionProperty(Id, name, out var pchValueBuffer, ref punValueBufferSizeOut))
			{
				return null;
			}
			if (name == null)
			{
				return pchValueBuffer;
			}
			if (_properties == null)
			{
				_properties = new Dictionary<string, string>();
			}
			_properties[name] = pchValueBuffer;
			return pchValueBuffer;
		}

		public bool GetBoolProperty(string name)
		{
			string property = GetProperty(name);
			if (property.Length == 0)
			{
				return false;
			}
			if (property[0] == '0' || property[0] == 'F' || property[0] == 'f')
			{
				return false;
			}
			return true;
		}

		public T GetProperty<T>(string name)
		{
			string property = GetProperty(name);
			if (string.IsNullOrEmpty(property))
			{
				return default(T);
			}
			try
			{
				return (T)Convert.ChangeType(property, typeof(T));
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public InventoryRecipe[] GetRecipesContainingThis()
		{
			if (_recContaining != null)
			{
				return _recContaining;
			}
			IEnumerable<InventoryRecipe> source = (from x in SteamInventory.Definitions
				select x.GetRecipes() into x
				where x != null
				select x).SelectMany((InventoryRecipe[] x) => x);
			_recContaining = source.Where((InventoryRecipe x) => x.ContainsIngredient(this)).ToArray();
			return _recContaining;
		}

		public static bool operator ==(InventoryDef a, InventoryDef b)
		{
			return a?.Equals(b) ?? ((object)b == null);
		}

		public static bool operator !=(InventoryDef a, InventoryDef b)
		{
			return !(a == b);
		}

		public override bool Equals(object p)
		{
			return Equals((InventoryDef)p);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public bool Equals(InventoryDef p)
		{
			if (p == null)
			{
				return false;
			}
			return p.Id == Id;
		}
	}
}
