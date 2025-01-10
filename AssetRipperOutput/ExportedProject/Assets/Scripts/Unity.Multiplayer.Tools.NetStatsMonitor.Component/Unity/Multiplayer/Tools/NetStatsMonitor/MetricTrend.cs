using System;
using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
	[Serializable]
	internal class MetricTrend
	{
		[field: SerializeField]
		public MetricId Metric { get; set; }

		[field: SerializeField]
		public LogNormalRandomWalk Trend { get; set; }
	}
}
