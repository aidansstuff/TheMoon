using System;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal static class BaseUnitExtensions
	{
		public static string GetSymbol(this BaseUnit unit)
		{
			return unit switch
			{
				BaseUnit.Byte => "B", 
				BaseUnit.Second => "s", 
				_ => throw new ArgumentException($"Unhandled BaseUnit {unit}"), 
			};
		}
	}
}
