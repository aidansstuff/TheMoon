using System;
using System.Linq;

namespace Steamworks
{
	public struct InventoryRecipe : IEquatable<InventoryRecipe>
	{
		public struct Ingredient
		{
			public int DefinitionId;

			public InventoryDef Definition;

			public int Count;

			internal static Ingredient FromString(string part)
			{
				Ingredient result = default(Ingredient);
				result.Count = 1;
				try
				{
					if (part.Contains("x"))
					{
						int num = part.IndexOf('x');
						int result2 = 0;
						if (int.TryParse(part.Substring(num + 1), out result2))
						{
							result.Count = result2;
						}
						part = part.Substring(0, num);
					}
					result.DefinitionId = int.Parse(part);
					result.Definition = SteamInventory.FindDefinition(result.DefinitionId);
				}
				catch (Exception)
				{
					return result;
				}
				return result;
			}
		}

		public InventoryDef Result;

		public Ingredient[] Ingredients;

		public string Source;

		internal static InventoryRecipe FromString(string part, InventoryDef Result)
		{
			InventoryRecipe inventoryRecipe = default(InventoryRecipe);
			inventoryRecipe.Result = Result;
			inventoryRecipe.Source = part;
			InventoryRecipe result = inventoryRecipe;
			string[] source = part.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			result.Ingredients = (from x in source
				select Ingredient.FromString(x) into x
				where x.DefinitionId != 0
				select x).ToArray();
			return result;
		}

		internal bool ContainsIngredient(InventoryDef inventoryDef)
		{
			return Ingredients.Any((Ingredient x) => x.DefinitionId == inventoryDef.Id);
		}

		public static bool operator ==(InventoryRecipe a, InventoryRecipe b)
		{
			return a.GetHashCode() == b.GetHashCode();
		}

		public static bool operator !=(InventoryRecipe a, InventoryRecipe b)
		{
			return a.GetHashCode() != b.GetHashCode();
		}

		public override bool Equals(object p)
		{
			return Equals((InventoryRecipe)p);
		}

		public override int GetHashCode()
		{
			return Source.GetHashCode();
		}

		public bool Equals(InventoryRecipe p)
		{
			return p.GetHashCode() == GetHashCode();
		}
	}
}
