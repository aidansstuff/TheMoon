using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal interface IEventMetric : IMetric
	{
		bool WentOverLimit { get; }

		int Count { get; }
	}
	internal interface IEventMetric<TValue> : IEventMetric, IMetric
	{
		IReadOnlyList<TValue> Values { get; }
	}
}
