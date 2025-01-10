using System;

namespace Unity.Multiplayer.Tools.NetStats
{
	[AttributeUsage(AttributeTargets.Enum)]
	public class MetricTypeEnumAttribute : Attribute
	{
		public string DisplayName { get; set; }
	}
}
