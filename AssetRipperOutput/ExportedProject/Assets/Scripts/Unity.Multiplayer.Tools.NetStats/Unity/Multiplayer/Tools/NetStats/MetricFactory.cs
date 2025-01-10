using System.Collections.Generic;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal class MetricFactory
	{
		private readonly Dictionary<MetricContainerType, IMetricFactory> k_Factories = new Dictionary<MetricContainerType, IMetricFactory>
		{
			{
				MetricContainerType.Counter,
				new CounterFactory()
			},
			{
				MetricContainerType.Event,
				new EventMetricFactory()
			},
			{
				MetricContainerType.Gauge,
				new GaugeFactory()
			},
			{
				MetricContainerType.Timer,
				new TimerFactory()
			}
		};

		public bool TryConstruct(MetricHeader header, out IMetric metric)
		{
			if (!k_Factories.TryGetValue(header.MetricContainerType, out var value))
			{
				Debug.LogError("Failed to find factory for type " + header.MetricContainerType);
				metric = null;
				return false;
			}
			return value.TryConstruct(header, out metric);
		}
	}
}
