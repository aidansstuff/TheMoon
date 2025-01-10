using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Internal;

namespace Unity.Services.Authentication
{
	internal class EnvironmentIdComponent : IEnvironmentId, IServiceComponent
	{
		public string EnvironmentId { get; internal set; }

		internal EnvironmentIdComponent()
		{
		}
	}
}
