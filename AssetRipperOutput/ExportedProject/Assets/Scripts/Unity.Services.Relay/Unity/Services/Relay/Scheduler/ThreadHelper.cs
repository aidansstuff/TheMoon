using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.Relay.Scheduler
{
	internal static class ThreadHelper
	{
		private static SynchronizationContext _unitySynchronizationContext;

		private static System.Threading.Tasks.TaskScheduler _taskScheduler;

		private static int _mainThreadId;

		public static SynchronizationContext SynchronizationContext => _unitySynchronizationContext;

		public static System.Threading.Tasks.TaskScheduler TaskScheduler => _taskScheduler;

		public static int MainThreadId => _mainThreadId;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Init()
		{
			_unitySynchronizationContext = SynchronizationContext.Current;
			_taskScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
			_mainThreadId = Thread.CurrentThread.ManagedThreadId;
		}
	}
}
