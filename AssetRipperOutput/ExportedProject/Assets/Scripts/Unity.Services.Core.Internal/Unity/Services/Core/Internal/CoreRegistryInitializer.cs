using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Unity.Services.Core.Internal
{
	internal class CoreRegistryInitializer
	{
		[NotNull]
		private readonly CoreRegistry m_Registry;

		[NotNull]
		private readonly List<int> m_SortedPackageTypeHashes;

		public CoreRegistryInitializer([NotNull] CoreRegistry registry, [NotNull] List<int> sortedPackageTypeHashes)
		{
			m_Registry = registry;
			m_SortedPackageTypeHashes = sortedPackageTypeHashes;
		}

		public async Task<List<PackageInitializationInfo>> InitializeRegistryAsync()
		{
			List<PackageInitializationInfo> packagesInitInfos = new List<PackageInitializationInfo>(m_SortedPackageTypeHashes.Count);
			if (m_SortedPackageTypeHashes.Count <= 0)
			{
				return packagesInitInfos;
			}
			DependencyTree dependencyTree = m_Registry.PackageRegistry.Tree;
			if (dependencyTree == null)
			{
				NullReferenceException innerException = new NullReferenceException("Registry requires a valid dependency tree to be initialized.");
				throw new ServicesInitializationException("Registry is in an invalid state (dependency tree is null) and can't be initialized.", innerException);
			}
			m_Registry.ComponentRegistry.ResetProvidedComponents(dependencyTree.ComponentTypeHashToInstance);
			List<Exception> failureReasons = new List<Exception>(m_SortedPackageTypeHashes.Count);
			Stopwatch stopwatch = new Stopwatch();
			for (int i = 0; i < m_SortedPackageTypeHashes.Count; i++)
			{
				IInitializablePackage package2 = GetPackageAt(i);
				await TryInitializePackageAsync(package2);
			}
			if (failureReasons.Count > 0)
			{
				Fail();
			}
			return packagesInitInfos;
			void Fail()
			{
				AggregateException innerException2 = new AggregateException(failureReasons);
				throw new ServicesInitializationException("Some services couldn't be initialized. Look at inner exceptions to get more information.", innerException2);
			}
			IInitializablePackage GetPackageAt(int index)
			{
				int key = m_SortedPackageTypeHashes[index];
				return dependencyTree.PackageTypeHashToInstance[key];
			}
			async Task TryInitializePackageAsync(IInitializablePackage package)
			{
				try
				{
					stopwatch.Restart();
					await package.Initialize(m_Registry);
					stopwatch.Stop();
					PackageInitializationInfo item = new PackageInitializationInfo
					{
						PackageType = package.GetType(),
						InitializationTimeInSeconds = stopwatch.Elapsed.TotalSeconds
					};
					packagesInitInfos.Add(item);
				}
				catch (Exception item2)
				{
					stopwatch.Stop();
					failureReasons.Add(item2);
				}
			}
		}
	}
}
