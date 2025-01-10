using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal static class MetricsCollectionExtensions
	{
		public static IReadOnlyList<TMetric> GetEventValues<TMetric>(this MetricCollection collection, MetricId metricId)
		{
			if (!collection.TryGetEvent(metricId, out IEventMetric<TMetric> metricEvent))
			{
				return Array.Empty<TMetric>();
			}
			return metricEvent.Values;
		}
	}
}
