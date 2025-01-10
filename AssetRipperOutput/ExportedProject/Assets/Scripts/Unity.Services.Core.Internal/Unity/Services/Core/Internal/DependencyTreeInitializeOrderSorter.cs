using System.Collections.Generic;

namespace Unity.Services.Core.Internal
{
	internal struct DependencyTreeInitializeOrderSorter
	{
		private enum ExplorationMark
		{
			None = 0,
			Viewed = 1,
			Sorted = 2
		}

		public readonly DependencyTree Tree;

		public readonly ICollection<int> Target;

		private Dictionary<int, ExplorationMark> m_PackageTypeHashExplorationHistory;

		public DependencyTreeInitializeOrderSorter(DependencyTree tree, ICollection<int> target)
		{
			Tree = tree;
			Target = target;
			m_PackageTypeHashExplorationHistory = null;
		}

		public void SortRegisteredPackagesIntoTarget()
		{
			Target.Clear();
			RemoveUnprovidedOptionalDependenciesFromTree();
			IReadOnlyCollection<int> packageTypeHashes = GetPackageTypeHashes();
			m_PackageTypeHashExplorationHistory = new Dictionary<int, ExplorationMark>(packageTypeHashes.Count);
			try
			{
				foreach (int item in packageTypeHashes)
				{
					SortTreeThrough(item);
				}
			}
			catch (HashException inner)
			{
				throw new DependencyTreeSortFailedException(Tree, Target, inner);
			}
			m_PackageTypeHashExplorationHistory = null;
		}

		private void RemoveUnprovidedOptionalDependenciesFromTree()
		{
			foreach (List<int> value in Tree.PackageTypeHashToComponentTypeHashDependencies.Values)
			{
				RemoveUnprovidedOptionalDependencies(value);
			}
		}

		private void RemoveUnprovidedOptionalDependencies(IList<int> dependencyTypeHashes)
		{
			for (int num = dependencyTypeHashes.Count - 1; num >= 0; num--)
			{
				int componentTypeHash = dependencyTypeHashes[num];
				if (Tree.IsOptional(componentTypeHash) && !Tree.IsProvided(componentTypeHash))
				{
					dependencyTypeHashes.RemoveAt(num);
				}
			}
		}

		private void SortTreeThrough(int packageTypeHash)
		{
			m_PackageTypeHashExplorationHistory.TryGetValue(packageTypeHash, out var value);
			switch (value)
			{
			case ExplorationMark.Viewed:
				throw new CircularDependencyException();
			case ExplorationMark.Sorted:
				return;
			}
			MarkPackage(packageTypeHash, ExplorationMark.Viewed);
			IEnumerable<int> dependencyTypeHashesFor = GetDependencyTypeHashesFor(packageTypeHash);
			try
			{
				SortTreeThrough(dependencyTypeHashesFor);
			}
			catch (DependencyTreeComponentHashException ex)
			{
				throw new DependencyTreePackageHashException(packageTypeHash, $"Component with hash[{ex.Hash}] threw exception when sorting package[{packageTypeHash}][{Tree.PackageTypeHashToInstance[packageTypeHash].GetType().FullName}]", ex);
			}
			Target.Add(packageTypeHash);
			MarkPackage(packageTypeHash, ExplorationMark.Sorted);
		}

		private void SortTreeThrough(IEnumerable<int> dependencyTypeHashes)
		{
			foreach (int dependencyTypeHash in dependencyTypeHashes)
			{
				int packageTypeHashFor = GetPackageTypeHashFor(dependencyTypeHash);
				SortTreeThrough(packageTypeHashFor);
			}
		}

		private void MarkPackage(int packageTypeHash, ExplorationMark mark)
		{
			m_PackageTypeHashExplorationHistory[packageTypeHash] = mark;
		}

		private IReadOnlyCollection<int> GetPackageTypeHashes()
		{
			return Tree.PackageTypeHashToInstance.Keys;
		}

		private int GetPackageTypeHashFor(int componentTypeHash)
		{
			if (!Tree.ComponentTypeHashToPackageTypeHash.TryGetValue(componentTypeHash, out var value))
			{
				throw new DependencyTreeComponentHashException(componentTypeHash, $"Component with hash[{componentTypeHash}] does not exist!");
			}
			return value;
		}

		private IEnumerable<int> GetDependencyTypeHashesFor(int packageTypeHash)
		{
			if (!Tree.PackageTypeHashToComponentTypeHashDependencies.TryGetValue(packageTypeHash, out var value))
			{
				throw new DependencyTreePackageHashException(packageTypeHash, $"Package with hash[{packageTypeHash}] does not exist!");
			}
			return value;
		}
	}
}
