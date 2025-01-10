using System.Collections.Generic;

namespace Unity.Services.Core.Internal
{
	internal class DependencyTree
	{
		public readonly Dictionary<int, IInitializablePackage> PackageTypeHashToInstance;

		public readonly Dictionary<int, int> ComponentTypeHashToPackageTypeHash;

		public readonly Dictionary<int, List<int>> PackageTypeHashToComponentTypeHashDependencies;

		public readonly Dictionary<int, IServiceComponent> ComponentTypeHashToInstance;

		internal DependencyTree()
			: this(new Dictionary<int, IInitializablePackage>(), new Dictionary<int, int>(), new Dictionary<int, List<int>>(), new Dictionary<int, IServiceComponent>())
		{
		}

		internal DependencyTree(Dictionary<int, IInitializablePackage> packageToInstance, Dictionary<int, int> componentToPackage, Dictionary<int, List<int>> packageToComponentDependencies, Dictionary<int, IServiceComponent> componentToInstance)
		{
			PackageTypeHashToInstance = packageToInstance;
			ComponentTypeHashToPackageTypeHash = componentToPackage;
			PackageTypeHashToComponentTypeHashDependencies = packageToComponentDependencies;
			ComponentTypeHashToInstance = componentToInstance;
		}
	}
}
