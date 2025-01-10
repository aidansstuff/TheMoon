using Unity.Services.Core.Internal;
using UnityEngine.Scripting;

namespace Unity.Services.Core.Analytics.Internal
{
	[RequireImplementors]
	public interface IAnalyticsUserId : IServiceComponent
	{
		string GetAnalyticsUserId();
	}
}
