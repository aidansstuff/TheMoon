using System;

namespace Unity.Multiplayer.Tools.NetStats
{
	[AttributeUsage(AttributeTargets.Field)]
	public class MetricMetadataAttribute : Attribute
	{
		public string DisplayName { get; set; }

		public MetricKind MetricKind { get; set; }

		public Units Units { get; set; }

		public bool DisplayAsPercentage { get; set; }
	}
}
