using System;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal static class UnitPrefixExtensions
	{
		public static string GetSymbol(this MetricPrefix prefix)
		{
			return prefix switch
			{
				MetricPrefix.Atto => "a", 
				MetricPrefix.Femto => "f", 
				MetricPrefix.Pico => "p", 
				MetricPrefix.Nano => "n", 
				MetricPrefix.Micro => "μ", 
				MetricPrefix.Milli => "m", 
				MetricPrefix.None => "", 
				MetricPrefix.Kilo => "k", 
				MetricPrefix.Mega => "M", 
				MetricPrefix.Giga => "G", 
				MetricPrefix.Tera => "T", 
				MetricPrefix.Peta => "P", 
				MetricPrefix.Exa => "E", 
				_ => throw new ArgumentException(string.Format("Unhandled {0} {1}", "MetricPrefix", prefix)), 
			};
		}

		public static float GetValueFloat(this MetricPrefix prefix)
		{
			return prefix switch
			{
				MetricPrefix.Atto => 1E-18f, 
				MetricPrefix.Femto => 1E-15f, 
				MetricPrefix.Pico => 1E-12f, 
				MetricPrefix.Nano => 1E-09f, 
				MetricPrefix.Micro => 1E-06f, 
				MetricPrefix.Milli => 0.001f, 
				MetricPrefix.None => 1f, 
				MetricPrefix.Kilo => 1000f, 
				MetricPrefix.Mega => 1000000f, 
				MetricPrefix.Giga => 1E+09f, 
				MetricPrefix.Tera => 1E+12f, 
				MetricPrefix.Peta => 1E+15f, 
				MetricPrefix.Exa => 1E+18f, 
				_ => throw new ArgumentException(string.Format("Unhandled {0} {1}", "MetricPrefix", prefix)), 
			};
		}
	}
}
