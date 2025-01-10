using System;
using System.Collections.Generic;

namespace Unity.Services.Relay.Scheduler
{
	public sealed class TaskSchedulerWebGL : TaskScheduler
	{
		private Queue<Action> m_mainThreadTaskQueue = new Queue<Action>();

		public override void ScheduleBackgroundTask(Action task)
		{
			ScheduleMainThreadTask(task);
		}

		public override bool IsMainThread()
		{
			return false;
		}

		public override void ScheduleMainThreadTask(Action task)
		{
			m_mainThreadTaskQueue.Enqueue(task);
		}

		private void Update()
		{
			((m_mainThreadTaskQueue.Count > 0) ? m_mainThreadTaskQueue.Dequeue() : null)?.Invoke();
		}
	}
}
