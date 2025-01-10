namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
	internal static class ConfigurationLimits
	{
		internal const int k_GraphSampleMin = 8;

		internal const int k_GraphSampleMax = 4096;

		internal static readonly string k_GraphMaxSampleWarningMessage = "The samples count is set to a very high value. This may have negative impacts on rendering or memory usage." + $" The sampling count will be clamped to {4096}.";

		internal const int k_CounterSampleMin = 8;

		internal const int k_CounterSampleMax = 4096;

		internal const int k_CounterSignificantDigitsMin = 1;

		internal const int k_CounterSignificantDigitsMax = 7;

		internal const double k_ExponentialMovingAverageHalfLifeMin = 0.0;

		internal const double k_RefreshRateMin = 1.0;

		internal const float k_PositionMin = 0f;

		internal const float k_PositionMax = 1f;

		internal const float k_GraphLineThicknessMin = 1f;

		internal const float k_GraphLineThicknessMax = 5f;
	}
}
