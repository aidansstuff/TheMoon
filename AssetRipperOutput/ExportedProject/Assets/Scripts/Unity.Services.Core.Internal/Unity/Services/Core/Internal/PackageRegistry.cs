using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Unity.Services.Core.Internal
{
	internal class PackageRegistry : IPackageRegistry
	{
		public DependencyTree Tree { get; set; }

		public PackageRegistry([CanBeNull] DependencyTree tree)
		{
			Tree = tree;
		}

		public CoreRegistration RegisterPackage<TPackage>(TPackage package) where TPackage : IInitializablePackage
		{
			int hashCode = typeof(TPackage).GetHashCode();
			Tree.PackageTypeHashToInstance[hashCode] = package;
			Tree.PackageTypeHashToComponentTypeHashDependencies[hashCode] = new List<int>();
			return new CoreRegistration(this, hashCode);
		}

		public void RegisterDependency<TComponent>(int packageTypeHash) where TComponent : IServiceComponent
		{
			Type typeFromHandle = typeof(TComponent);
			int hashCode = typeFromHandle.GetHashCode();
			Tree.ComponentTypeHashToInstance[hashCode] = new MissingComponent(typeFromHandle);
			AddComponentDependencyToPackage(hashCode, packageTypeHash);
		}

		public void RegisterOptionalDependency<TComponent>(int packageTypeHash) where TComponent : IServiceComponent
		{
			int hashCode = typeof(TComponent).GetHashCode();
			if (!Tree.ComponentTypeHashToInstance.ContainsKey(hashCode))
			{
				Tree.ComponentTypeHashToInstance[hashCode] = null;
			}
			AddComponentDependencyToPackage(hashCode, packageTypeHash);
		}

		public void RegisterProvision<TComponent>(int packageTypeHash) where TComponent : IServiceComponent
		{
			int hashCode = typeof(TComponent).GetHashCode();
			Tree.ComponentTypeHashToPackageTypeHash[hashCode] = packageTypeHash;
		}

		private void AddComponentDependencyToPackage(int componentTypeHash, int packageTypeHash)
		{
			List<int> list = Tree.PackageTypeHashToComponentTypeHashDependencies[packageTypeHash];
			if (!list.Contains(componentTypeHash))
			{
				list.Add(componentTypeHash);
			}
		}
	}
}
