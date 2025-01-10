using System;

namespace Steamworks
{
	internal static class Epoch
	{
		private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static int Current => (int)DateTime.UtcNow.Subtract(epoch).TotalSeconds;

		public static DateTime ToDateTime(decimal unixTime)
		{
			DateTime dateTime = epoch;
			return dateTime.AddSeconds((long)unixTime);
		}

		public static uint FromDateTime(DateTime dt)
		{
			return (uint)dt.Subtract(epoch).TotalSeconds;
		}
	}
}
