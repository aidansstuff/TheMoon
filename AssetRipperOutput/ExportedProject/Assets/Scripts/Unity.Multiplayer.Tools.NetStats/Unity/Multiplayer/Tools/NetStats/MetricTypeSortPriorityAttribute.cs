using System;

namespace Unity.Multiplayer.Tools.NetStats
{
	[AttributeUsage(AttributeTargets.Enum)]
	internal class MetricTypeSortPriorityAttribute : Attribute
	{
		public SortPriority SortPriority { get; set; }
	}
}
