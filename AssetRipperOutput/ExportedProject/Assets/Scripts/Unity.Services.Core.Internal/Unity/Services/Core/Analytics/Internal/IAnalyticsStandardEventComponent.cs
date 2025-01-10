using System.Collections.Generic;
using Unity.Services.Core.Internal;
using UnityEngine.Scripting;

namespace Unity.Services.Core.Analytics.Internal
{
	[RequireImplementors]
	public interface IAnalyticsStandardEventComponent : IServiceComponent
	{
		void Record(string eventName, IDictionary<string, object> eventParameters, int eventVersion, string packageName);
	}
}
