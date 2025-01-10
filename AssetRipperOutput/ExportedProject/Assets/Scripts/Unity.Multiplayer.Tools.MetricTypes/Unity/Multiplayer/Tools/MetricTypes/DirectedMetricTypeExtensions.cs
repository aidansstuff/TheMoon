using System.Collections.Generic;
using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	internal static class DirectedMetricTypeExtensions
	{
		private static readonly Dictionary<DirectedMetricType, string> s_Identifiers;

		private static readonly Dictionary<DirectedMetricType, string> s_DisplayNames;

		static DirectedMetricTypeExtensions()
		{
			s_Identifiers = new Dictionary<DirectedMetricType, string>();
			s_DisplayNames = new Dictionary<DirectedMetricType, string>();
			MetricType[] values = EnumUtil.GetValues<MetricType>();
			NetworkDirection[] values2 = EnumUtil.GetValues<NetworkDirection>();
			MetricType[] array = values;
			for (int i = 0; i < array.Length; i++)
			{
				MetricType metricType = array[i];
				NetworkDirection[] array2 = values2;
				for (int j = 0; j < array2.Length; j++)
				{
					NetworkDirection direction = array2[j];
					DirectedMetricType directedMetric = metricType.GetDirectedMetric(direction);
					string text = metricType.ToString() + direction;
					s_Identifiers[directedMetric] = text;
					s_DisplayNames[directedMetric] = StringUtil.AddSpacesToCamelCase(text);
				}
			}
		}

		internal static DirectedMetricType GetDirectedMetric(this MetricType metricType, NetworkDirection direction)
		{
			return (DirectedMetricType)(((int)metricType << 2) | (int)(direction & NetworkDirection.SentAndReceived));
		}

		internal static MetricType GetMetric(this DirectedMetricType directedMetric)
		{
			return (MetricType)((int)directedMetric >> 2);
		}

		internal static NetworkDirection GetDirection(this DirectedMetricType directedMetric)
		{
			return (NetworkDirection)(directedMetric & (DirectedMetricType)3);
		}

		internal static MetricId GetId(this DirectedMetricType directedMetric)
		{
			return MetricId.Create(directedMetric);
		}

		internal static string GetDisplayName(this DirectedMetricType directedMetric)
		{
			if (s_DisplayNames.TryGetValue(directedMetric, out var value))
			{
				return value;
			}
			return directedMetric.ToString();
		}
	}
}
