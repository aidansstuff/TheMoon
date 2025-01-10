using System.Collections.Generic;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class DisabledMetricsFactory : IMetricsFactory, IServiceComponent
	{
		IReadOnlyDictionary<string, string> IMetricsFactory.CommonTags { get; } = new Dictionary<string, string>();


		IMetrics IMetricsFactory.Create(string packageName)
		{
			return new DisabledMetrics();
		}
	}
}
