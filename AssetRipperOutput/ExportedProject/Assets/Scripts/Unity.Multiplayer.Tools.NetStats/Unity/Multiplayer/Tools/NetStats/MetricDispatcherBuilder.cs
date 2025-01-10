using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal sealed class MetricDispatcherBuilder
	{
		private readonly IDictionary<MetricId, IMetric<long>> m_Counters = new Dictionary<MetricId, IMetric<long>>();

		private readonly IDictionary<MetricId, IMetric<double>> m_Gauges = new Dictionary<MetricId, IMetric<double>>();

		private readonly IDictionary<MetricId, IMetric<TimeSpan>> m_Timers = new Dictionary<MetricId, IMetric<TimeSpan>>();

		private readonly IDictionary<MetricId, IEventMetric> m_PayloadEvents = new Dictionary<MetricId, IEventMetric>();

		private readonly List<IResettable> m_Resettables = new List<IResettable>();

		public MetricDispatcherBuilder WithCounters(params Counter[] counters)
		{
			foreach (Counter counter in counters)
			{
				m_Counters[counter.Id] = counter;
				m_Resettables.Add(counter);
			}
			return this;
		}

		public MetricDispatcherBuilder WithGauges(params Gauge[] gauges)
		{
			foreach (Gauge gauge in gauges)
			{
				m_Gauges[gauge.Id] = gauge;
				m_Resettables.Add(gauge);
			}
			return this;
		}

		public MetricDispatcherBuilder WithTimers(params Timer[] timers)
		{
			foreach (Timer timer in timers)
			{
				m_Timers[timer.Id] = timer;
				m_Resettables.Add(timer);
			}
			return this;
		}

		public MetricDispatcherBuilder WithMetricEvents<TEvent>(params EventMetric<TEvent>[] metricEvents) where TEvent : unmanaged
		{
			foreach (EventMetric<TEvent> eventMetric in metricEvents)
			{
				m_PayloadEvents[eventMetric.Id] = eventMetric;
				m_Resettables.Add(eventMetric);
			}
			return this;
		}

		public IMetricDispatcher Build()
		{
			return new MetricDispatcher(new MetricCollection(new ReadOnlyDictionary<MetricId, IMetric<long>>(m_Counters), new ReadOnlyDictionary<MetricId, IMetric<double>>(m_Gauges), new ReadOnlyDictionary<MetricId, IMetric<TimeSpan>>(m_Timers), new ReadOnlyDictionary<MetricId, IEventMetric>(m_PayloadEvents)), m_Resettables, m_PayloadEvents.Values.ToList());
		}
	}
}
