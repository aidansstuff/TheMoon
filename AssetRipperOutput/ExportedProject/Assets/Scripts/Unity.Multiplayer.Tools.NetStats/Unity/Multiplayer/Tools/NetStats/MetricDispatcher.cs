using System.Collections.Generic;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal class MetricDispatcher : IMetricDispatcher
	{
		internal const string k_ThrottlingWarning = "Some metrics in the collection went over the configured values limit. Some values were ignored.";

		private readonly MetricCollection m_Collection;

		private readonly IReadOnlyList<IResettable> m_Resettables;

		private readonly IReadOnlyList<IEventMetric> m_EventMetrics;

		private readonly IList<IMetricObserver> m_Observers = new List<IMetricObserver>();

		internal MetricDispatcher(MetricCollection collection, IReadOnlyList<IResettable> resettables, IReadOnlyList<IEventMetric> eventMetrics)
		{
			m_Collection = collection;
			m_Resettables = resettables;
			m_EventMetrics = eventMetrics;
		}

		public void RegisterObserver(IMetricObserver observer)
		{
			m_Observers.Add(observer);
		}

		public void SetConnectionId(ulong connectionId)
		{
			m_Collection.ConnectionId = connectionId;
		}

		public void Dispatch()
		{
			bool flag = false;
			for (int i = 0; i < m_EventMetrics.Count; i++)
			{
				if (m_EventMetrics[i].WentOverLimit)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Debug.LogWarning("Some metrics in the collection went over the configured values limit. Some values were ignored.");
			}
			for (int j = 0; j < m_Observers.Count; j++)
			{
				m_Observers[j].Observe(m_Collection);
			}
			for (int k = 0; k < m_Resettables.Count; k++)
			{
				IResettable resettable = m_Resettables[k];
				if (resettable.ShouldResetOnDispatch)
				{
					resettable.Reset();
				}
			}
		}
	}
}
