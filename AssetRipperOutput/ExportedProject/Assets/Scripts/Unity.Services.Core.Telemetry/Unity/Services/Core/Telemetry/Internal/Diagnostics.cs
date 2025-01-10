using System;
using System.Collections.Generic;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class Diagnostics : IDiagnostics
	{
		internal const int MaxDiagnosticMessageLength = 10000;

		internal const string DiagnosticMessageTruncateSuffix = "[truncated]";

		internal DiagnosticsHandler Handler { get; }

		internal IDictionary<string, string> PackageTags { get; }

		public Diagnostics(DiagnosticsHandler handler, IDictionary<string, string> packageTags)
		{
			Handler = handler;
			PackageTags = packageTags;
		}

		public void SendDiagnostic(string name, string message, IDictionary<string, string> tags = null)
		{
			Diagnostic diagnostic = default(Diagnostic);
			diagnostic.Content = ((tags == null) ? new Dictionary<string, string>(PackageTags) : new Dictionary<string, string>(tags).MergeAllowOverride(PackageTags));
			Diagnostic telemetryEvent = diagnostic;
			telemetryEvent.Content.Add("name", name);
			if (message != null && message.Length > 10000)
			{
				message = message.Substring(0, 10000) + Environment.NewLine + "[truncated]";
			}
			telemetryEvent.Content.Add("message", message);
			Handler.Register(telemetryEvent);
		}
	}
}
