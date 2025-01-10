using System.Collections.Generic;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class DisabledDiagnostics : IDiagnostics
	{
		void IDiagnostics.SendDiagnostic(string name, string message, IDictionary<string, string> tags)
		{
		}
	}
}
