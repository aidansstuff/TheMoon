using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Unity.Services.Core.Internal
{
	internal class LockedComponentRegistry : IComponentRegistry
	{
		private const string k_ErrorMessage = "Component registration has been locked. Make sure to register service components before all packages have finished initializing.";

		[NotNull]
		internal IComponentRegistry Registry { get; }

		public LockedComponentRegistry([NotNull] IComponentRegistry registryToLock)
		{
			Registry = registryToLock;
		}

		public void RegisterServiceComponent<TComponent>(TComponent component) where TComponent : IServiceComponent
		{
			throw new InvalidOperationException("Component registration has been locked. Make sure to register service components before all packages have finished initializing.");
		}

		public TComponent GetServiceComponent<TComponent>() where TComponent : IServiceComponent
		{
			return Registry.GetServiceComponent<TComponent>();
		}

		public void ResetProvidedComponents(IDictionary<int, IServiceComponent> componentTypeHashToInstance)
		{
			throw new InvalidOperationException("Component registration has been locked. Make sure to register service components before all packages have finished initializing.");
		}
	}
}
