using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Unity.Services.Core.Telemetry.Internal
{
	[Serializable]
	internal struct Diagnostic : ITelemetryEvent
	{
		[JsonProperty("content")]
		public IDictionary<string, string> Content;
	}
}
