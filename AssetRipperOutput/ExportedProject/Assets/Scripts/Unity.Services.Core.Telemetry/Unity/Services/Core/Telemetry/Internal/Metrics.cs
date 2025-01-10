using System.Collections.Generic;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class Metrics : IMetrics
	{
		internal MetricsHandler Handler { get; }

		internal IDictionary<string, string> PackageTags { get; }

		public Metrics(MetricsHandler handler, IDictionary<string, string> packageTags)
		{
			Handler = handler;
			PackageTags = packageTags;
		}

		internal Metric CreateMetric(string name, double value, MetricType type, IDictionary<string, string> tags)
		{
			Metric result = default(Metric);
			result.Name = name;
			result.Value = value;
			result.Type = type;
			result.Tags = ((tags == null) ? PackageTags : tags.MergeAllowOverride(PackageTags));
			return result;
		}

		void IMetrics.SendGaugeMetric(string name, double value, IDictionary<string, string> tags)
		{
			Metric telemetryEvent = CreateMetric(name, value, MetricType.Gauge, tags);
			Handler.Register(telemetryEvent);
		}

		void IMetrics.SendHistogramMetric(string name, double time, IDictionary<string, string> tags)
		{
			Metric telemetryEvent = CreateMetric(name, time, MetricType.Histogram, tags);
			Handler.Register(telemetryEvent);
		}

		void IMetrics.SendSumMetric(string name, double value, IDictionary<string, string> tags)
		{
			Metric telemetryEvent = CreateMetric(name, value, MetricType.Sum, tags);
			Handler.Register(telemetryEvent);
		}
	}
}
