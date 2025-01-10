using System;
using UnityEngine;

namespace Unity.Services.Relay.Scheduler
{
	public abstract class TaskScheduler : MonoBehaviour
	{
		public abstract void ScheduleBackgroundTask(Action task);

		public abstract bool IsMainThread();

		public abstract void ScheduleMainThreadTask(Action task);

		public void ScheduleOrExecuteOnMain(Action action)
		{
			if (IsMainThread())
			{
				action?.Invoke();
			}
			else
			{
				ScheduleMainThreadTask(action);
			}
		}
	}
}
