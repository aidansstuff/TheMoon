using System.Collections.Generic;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Telemetry.Internal
{
	public interface IDiagnosticsFactory : IServiceComponent
	{
		IReadOnlyDictionary<string, string> CommonTags { get; }

		IDiagnostics Create(string packageName);
	}
}
