using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Environments.Internal
{
	internal class Environments : IEnvironments, IServiceComponent
	{
		public string Current { get; internal set; }
	}
}
