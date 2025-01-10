using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.Common;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
	[AddComponentMenu("")]
	internal class CustomTestDataGenerator : MonoBehaviour
	{
		private RuntimeNetStatsMonitor m_Rnsm;

		private System.Random m_Random = new System.Random();

		[field: Tooltip("Pairs of metrics and trends to generate test data for")]
		[field: SerializeField]
		internal List<MetricTrend> MetricTrends { get; set; } = new List<MetricTrend>
		{
			new MetricTrend
			{
				Trend = new LogNormalRandomWalk()
			}
		};


		private void Start()
		{
			m_Rnsm = UnityEngine.Object.FindObjectOfType<RuntimeNetStatsMonitor>();
		}

		private void Update()
		{
			if (!m_Rnsm)
			{
				return;
			}
			foreach (MetricTrend metricTrend in MetricTrends)
			{
				float value = metricTrend.Trend.NextFloat(m_Random);
				m_Rnsm.AddCustomValue(metricTrend.Metric, value);
			}
		}
	}
}
