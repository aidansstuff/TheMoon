using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.Qos.Scheduler
{
	internal static class ThreadHelper
	{
		private static SynchronizationContext _unitySynchronizationContext;

		private static TaskScheduler _taskScheduler;

		private static int _mainThreadId;

		public static SynchronizationContext SynchronizationContext => _unitySynchronizationContext;

		public static TaskScheduler TaskScheduler => _taskScheduler;

		public static int MainThreadId => _mainThreadId;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Init()
		{
			_unitySynchronizationContext = SynchronizationContext.Current;
			_taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
			_mainThreadId = Thread.CurrentThread.ManagedThreadId;
		}
	}
}
