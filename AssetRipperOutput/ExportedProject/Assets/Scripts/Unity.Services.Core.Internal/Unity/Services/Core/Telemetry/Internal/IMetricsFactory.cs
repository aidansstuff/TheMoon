using System.Collections.Generic;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Telemetry.Internal
{
	public interface IMetricsFactory : IServiceComponent
	{
		IReadOnlyDictionary<string, string> CommonTags { get; }

		IMetrics Create(string packageName);
	}
}
