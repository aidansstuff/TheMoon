using System.Collections.Generic;
using JetBrains.Annotations;

namespace Unity.Services.Core.Internal
{
	public sealed class CoreRegistry
	{
		public static CoreRegistry Instance { get; internal set; }

		[NotNull]
		internal IPackageRegistry PackageRegistry { get; private set; }

		[NotNull]
		internal IComponentRegistry ComponentRegistry { get; private set; }

		internal CoreRegistry()
		{
			DependencyTree dependencyTree = new DependencyTree();
			PackageRegistry = new PackageRegistry(dependencyTree);
			Dictionary<int, IServiceComponent> componentTypeHashToInstance = new Dictionary<int, IServiceComponent>(dependencyTree.ComponentTypeHashToInstance.Count);
			ComponentRegistry = new ComponentRegistry(componentTypeHashToInstance);
		}

		internal CoreRegistry([NotNull] IPackageRegistry packageRegistry, [NotNull] IComponentRegistry componentRegistry)
		{
			PackageRegistry = packageRegistry;
			ComponentRegistry = componentRegistry;
		}

		public CoreRegistration RegisterPackage<TPackage>([NotNull] TPackage package) where TPackage : IInitializablePackage
		{
			return PackageRegistry.RegisterPackage(package);
		}

		public void RegisterServiceComponent<TComponent>([NotNull] TComponent component) where TComponent : IServiceComponent
		{
			ComponentRegistry.RegisterServiceComponent(component);
		}

		public TComponent GetServiceComponent<TComponent>() where TComponent : IServiceComponent
		{
			return ComponentRegistry.GetServiceComponent<TComponent>();
		}

		internal void LockPackageRegistration()
		{
			if (!(PackageRegistry is LockedPackageRegistry))
			{
				PackageRegistry = new LockedPackageRegistry(PackageRegistry);
			}
		}

		internal void LockComponentRegistration()
		{
			if (!(ComponentRegistry is LockedComponentRegistry))
			{
				ComponentRegistry = new LockedComponentRegistry(ComponentRegistry);
			}
		}
	}
}
