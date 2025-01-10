using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Unity.Services.Core.Internal
{
	internal class UnityServicesInternal : IUnityServices
	{
		internal bool CanInitialize;

		private TaskCompletionSource<object> m_Initialization;

		public ServicesInitializationState State { get; private set; }

		public InitializationOptions Options { get; internal set; }

		[NotNull]
		private CoreRegistry Registry { get; }

		[NotNull]
		private CoreMetrics Metrics { get; }

		[NotNull]
		private CoreDiagnostics Diagnostics { get; }

		public UnityServicesInternal([NotNull] CoreRegistry registry, [NotNull] CoreMetrics metrics, [NotNull] CoreDiagnostics diagnostics)
		{
			Registry = registry;
			Metrics = metrics;
			Diagnostics = diagnostics;
		}

		public async Task InitializeAsync(InitializationOptions options)
		{
			if (!HasRequestedInitialization() || HasInitializationFailed())
			{
				Options = options;
				m_Initialization = new TaskCompletionSource<object>();
			}
			if (!CanInitialize || State != 0)
			{
				await m_Initialization.Task;
			}
			else
			{
				await InitializeServicesAsync();
			}
			bool HasInitializationFailed()
			{
				if (m_Initialization.Task.IsCompleted)
				{
					return m_Initialization.Task.Status != TaskStatus.RanToCompletion;
				}
				return false;
			}
		}

		private bool HasRequestedInitialization()
		{
			return m_Initialization != null;
		}

		private async Task InitializeServicesAsync()
		{
			State = ServicesInitializationState.Initializing;
			Stopwatch initStopwatch = new Stopwatch();
			initStopwatch.Start();
			DependencyTree dependencyTree = Registry.PackageRegistry.Tree;
			if (dependencyTree == null)
			{
				NullReferenceException ex = new NullReferenceException("Services require a valid dependency tree to be initialized.");
				FailServicesInitialization(ex);
				throw ex;
			}
			List<int> sortedPackageTypeHashes = new List<int>(dependencyTree.PackageTypeHashToInstance.Count);
			List<PackageInitializationInfo> packageInitInfos;
			try
			{
				SortPackages();
				packageInitInfos = await InitializePackagesAsync();
			}
			catch (Exception reason2)
			{
				FailServicesInitialization(reason2);
				throw;
			}
			SendInitializationMetrics(packageInitInfos);
			SucceedServicesInitialization();
			void FailServicesInitialization(Exception reason)
			{
				State = ServicesInitializationState.Uninitialized;
				initStopwatch.Stop();
				m_Initialization.TrySetException(reason);
				if (reason is CircularDependencyException)
				{
					Diagnostics.SendCircularDependencyDiagnostics(reason);
				}
				else
				{
					Diagnostics.SendOperateServicesInitDiagnostics(reason);
				}
			}
			async Task<List<PackageInitializationInfo>> InitializePackagesAsync()
			{
				return await new CoreRegistryInitializer(Registry, sortedPackageTypeHashes).InitializeRegistryAsync();
			}
			void SortPackages()
			{
				new DependencyTreeInitializeOrderSorter(dependencyTree, sortedPackageTypeHashes).SortRegisteredPackagesIntoTarget();
			}
			void SucceedServicesInitialization()
			{
				State = ServicesInitializationState.Initialized;
				Registry.PackageRegistry.Tree = null;
				Registry.LockComponentRegistration();
				initStopwatch.Stop();
				m_Initialization.TrySetResult(null);
				Metrics.SendAllPackagesInitSuccessMetric();
				Metrics.SendAllPackagesInitTimeMetric(initStopwatch.Elapsed.TotalSeconds);
			}
		}

		internal void SendInitializationMetrics(List<PackageInitializationInfo> packageInitInfos)
		{
			foreach (PackageInitializationInfo packageInitInfo in packageInitInfos)
			{
				Metrics.SendInitTimeMetricForPackage(packageInitInfo.PackageType, packageInitInfo.InitializationTimeInSeconds);
			}
		}

		internal async Task EnableInitializationAsync()
		{
			CanInitialize = true;
			Registry.LockPackageRegistration();
			if (HasRequestedInitialization())
			{
				await InitializeServicesAsync();
			}
		}
	}
}
