using System.Collections.Generic;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class DisabledMetrics : IMetrics
	{
		void IMetrics.SendGaugeMetric(string name, double value, IDictionary<string, string> tags)
		{
		}

		void IMetrics.SendHistogramMetric(string name, double time, IDictionary<string, string> tags)
		{
		}

		void IMetrics.SendSumMetric(string name, double value, IDictionary<string, string> tags)
		{
		}
	}
}
