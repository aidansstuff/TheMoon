using System;
using System.Collections.Generic;
using System.Threading;

namespace Unity.Services.Relay.Scheduler
{
	public sealed class TaskSchedulerThreaded : TaskScheduler
	{
		private Queue<Action> m_mainThreadTaskQueue = new Queue<Action>();

		private object m_lock = new object();

		private Thread m_mainThread;

		private void Start()
		{
			m_mainThread = Thread.CurrentThread;
		}

		public override bool IsMainThread()
		{
			return m_mainThread == Thread.CurrentThread;
		}

		public override void ScheduleBackgroundTask(Action task)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				task();
			});
		}

		public override void ScheduleMainThreadTask(Action task)
		{
			lock (m_lock)
			{
				m_mainThreadTaskQueue.Enqueue(task);
			}
		}

		private void Update()
		{
			Queue<Action> queue = null;
			lock (m_lock)
			{
				queue = new Queue<Action>(m_mainThreadTaskQueue);
				m_mainThreadTaskQueue.Clear();
			}
			foreach (Action item in queue)
			{
				item();
			}
		}
	}
}
