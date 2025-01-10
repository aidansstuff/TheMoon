using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.Core.Internal
{
	internal class TaskAsyncOperation : AsyncOperationBase, INotifyCompletion
	{
		internal static TaskScheduler Scheduler;

		private Task m_Task;

		public override bool IsCompleted => m_Task.IsCompleted;

		public override AsyncOperationStatus Status
		{
			get
			{
				if (m_Task == null)
				{
					return AsyncOperationStatus.None;
				}
				if (!m_Task.IsCompleted)
				{
					return AsyncOperationStatus.InProgress;
				}
				if (m_Task.IsCanceled)
				{
					return AsyncOperationStatus.Cancelled;
				}
				if (m_Task.IsFaulted)
				{
					return AsyncOperationStatus.Failed;
				}
				return AsyncOperationStatus.Succeeded;
			}
		}

		public override Exception Exception => m_Task?.Exception;

		public override void GetResult()
		{
		}

		public override AsyncOperationBase GetAwaiter()
		{
			return this;
		}

		public TaskAsyncOperation(Task task)
		{
			if (Scheduler == null)
			{
				SetScheduler();
			}
			m_Task = task;
			task.ContinueWith(delegate(Task t, object state)
			{
				((TaskAsyncOperation)state).DidComplete();
			}, this, CancellationToken.None, TaskContinuationOptions.None, Scheduler);
		}

		public static TaskAsyncOperation Run(Action action)
		{
			Task task = new Task(action);
			TaskAsyncOperation result = new TaskAsyncOperation(task);
			task.Start();
			return result;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		internal static void SetScheduler()
		{
			Scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		}
	}
	internal class TaskAsyncOperation<T> : AsyncOperationBase<T>
	{
		private Task<T> m_Task;

		public override bool IsCompleted => m_Task.IsCompleted;

		public override T Result => m_Task.Result;

		public override AsyncOperationStatus Status
		{
			get
			{
				if (m_Task == null)
				{
					return AsyncOperationStatus.None;
				}
				if (!m_Task.IsCompleted)
				{
					return AsyncOperationStatus.InProgress;
				}
				if (m_Task.IsCanceled)
				{
					return AsyncOperationStatus.Cancelled;
				}
				if (m_Task.IsFaulted)
				{
					return AsyncOperationStatus.Failed;
				}
				return AsyncOperationStatus.Succeeded;
			}
		}

		public override Exception Exception => m_Task?.Exception;

		public override T GetResult()
		{
			return m_Task.GetAwaiter().GetResult();
		}

		public override AsyncOperationBase<T> GetAwaiter()
		{
			return this;
		}

		public TaskAsyncOperation(Task<T> task)
		{
			if (TaskAsyncOperation.Scheduler == null)
			{
				TaskAsyncOperation.SetScheduler();
			}
			m_Task = task;
			task.ContinueWith(delegate(Task<T> t, object state)
			{
				((TaskAsyncOperation<T>)state).DidComplete();
			}, this, CancellationToken.None, TaskContinuationOptions.None, TaskAsyncOperation.Scheduler);
		}

		public static TaskAsyncOperation<T> Run(Func<T> func)
		{
			Task<T> task = new Task<T>(func);
			TaskAsyncOperation<T> result = new TaskAsyncOperation<T>(task);
			task.Start();
			return result;
		}
	}
}
