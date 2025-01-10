namespace Unity.Multiplayer.Tools.MetricTypes
{
	internal static class NetworkDirectionExtensions
	{
		public static string DisplayString(this NetworkDirection direction)
		{
			if (direction != 0)
			{
				return direction.ToString();
			}
			return "";
		}
	}
}
