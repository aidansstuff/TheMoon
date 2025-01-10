using System;
using JetBrains.Annotations;

namespace Unity.Services.Core.Internal
{
	internal class LockedPackageRegistry : IPackageRegistry
	{
		private const string k_ErrorMessage = "Package registration has been locked. Make sure to register service packages in[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)].";

		[NotNull]
		internal IPackageRegistry Registry { get; }

		public DependencyTree Tree
		{
			get
			{
				return Registry.Tree;
			}
			set
			{
				Registry.Tree = value;
			}
		}

		public LockedPackageRegistry([NotNull] IPackageRegistry registryToLock)
		{
			Registry = registryToLock;
		}

		public CoreRegistration RegisterPackage<TPackage>(TPackage package) where TPackage : IInitializablePackage
		{
			throw new InvalidOperationException("Package registration has been locked. Make sure to register service packages in[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)].");
		}

		public void RegisterDependency<TComponent>(int packageTypeHash) where TComponent : IServiceComponent
		{
			throw new InvalidOperationException("Package registration has been locked. Make sure to register service packages in[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)].");
		}

		public void RegisterOptionalDependency<TComponent>(int packageTypeHash) where TComponent : IServiceComponent
		{
			throw new InvalidOperationException("Package registration has been locked. Make sure to register service packages in[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)].");
		}

		public void RegisterProvision<TComponent>(int packageTypeHash) where TComponent : IServiceComponent
		{
			throw new InvalidOperationException("Package registration has been locked. Make sure to register service packages in[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)].");
		}
	}
}
