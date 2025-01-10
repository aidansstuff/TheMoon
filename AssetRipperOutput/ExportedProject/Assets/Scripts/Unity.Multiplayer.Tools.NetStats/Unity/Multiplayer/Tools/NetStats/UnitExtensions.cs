using System;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal static class UnitExtensions
	{
		internal static BaseUnits GetBaseUnits(this Units units)
		{
			return units switch
			{
				Units.None => default(BaseUnits), 
				Units.Bytes => new BaseUnits(1, 0), 
				Units.BytesPerSecond => new BaseUnits(1, -1), 
				Units.Seconds => new BaseUnits(0, 1), 
				Units.Hertz => new BaseUnits(0, -1), 
				_ => throw new ArgumentOutOfRangeException("units", units, null), 
			};
		}
	}
}
