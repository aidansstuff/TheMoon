using System.Collections.Generic;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class DisabledDiagnosticsFactory : IDiagnosticsFactory, IServiceComponent
	{
		IReadOnlyDictionary<string, string> IDiagnosticsFactory.CommonTags { get; } = new Dictionary<string, string>();


		IDiagnostics IDiagnosticsFactory.Create(string packageName)
		{
			return new DisabledDiagnostics();
		}
	}
}
