using System;
using System.Collections;

namespace Unity.Services.Core.Internal
{
	internal class AsyncOperation : IAsyncOperation, IEnumerator
	{
		protected Action<IAsyncOperation> m_CompletedCallback;

		public bool IsDone { get; protected set; }

		public AsyncOperationStatus Status { get; protected set; }

		public Exception Exception { get; protected set; }

		object IEnumerator.Current => null;

		public event Action<IAsyncOperation> Completed
		{
			add
			{
				if (IsDone)
				{
					value(this);
				}
				else
				{
					m_CompletedCallback = (Action<IAsyncOperation>)Delegate.Combine(m_CompletedCallback, value);
				}
			}
			remove
			{
				m_CompletedCallback = (Action<IAsyncOperation>)Delegate.Remove(m_CompletedCallback, value);
			}
		}

		public void SetInProgress()
		{
			Status = AsyncOperationStatus.InProgress;
		}

		public void Succeed()
		{
			if (!IsDone)
			{
				IsDone = true;
				Status = AsyncOperationStatus.Succeeded;
				m_CompletedCallback?.Invoke(this);
				m_CompletedCallback = null;
			}
		}

		public void Fail(Exception reason)
		{
			if (!IsDone)
			{
				Exception = reason;
				IsDone = true;
				Status = AsyncOperationStatus.Failed;
				m_CompletedCallback?.Invoke(this);
				m_CompletedCallback = null;
			}
		}

		public void Cancel()
		{
			if (!IsDone)
			{
				Exception = new OperationCanceledException();
				IsDone = true;
				Status = AsyncOperationStatus.Cancelled;
				m_CompletedCallback?.Invoke(this);
				m_CompletedCallback = null;
			}
		}

		bool IEnumerator.MoveNext()
		{
			return !IsDone;
		}

		void IEnumerator.Reset()
		{
		}
	}
	internal class AsyncOperation<T> : IAsyncOperation<T>, IEnumerator
	{
		protected Action<IAsyncOperation<T>> m_CompletedCallback;

		public bool IsDone { get; protected set; }

		public AsyncOperationStatus Status { get; protected set; }

		public Exception Exception { get; protected set; }

		public T Result { get; protected set; }

		object IEnumerator.Current => null;

		public event Action<IAsyncOperation<T>> Completed
		{
			add
			{
				if (IsDone)
				{
					value(this);
				}
				else
				{
					m_CompletedCallback = (Action<IAsyncOperation<T>>)Delegate.Combine(m_CompletedCallback, value);
				}
			}
			remove
			{
				m_CompletedCallback = (Action<IAsyncOperation<T>>)Delegate.Remove(m_CompletedCallback, value);
			}
		}

		public void SetInProgress()
		{
			Status = AsyncOperationStatus.InProgress;
		}

		public void Succeed(T result)
		{
			if (!IsDone)
			{
				Result = result;
				IsDone = true;
				Status = AsyncOperationStatus.Succeeded;
				m_CompletedCallback?.Invoke(this);
				m_CompletedCallback = null;
			}
		}

		public void Fail(Exception reason)
		{
			if (!IsDone)
			{
				Exception = reason;
				IsDone = true;
				Status = AsyncOperationStatus.Failed;
				m_CompletedCallback?.Invoke(this);
				m_CompletedCallback = null;
			}
		}

		public void Cancel()
		{
			if (!IsDone)
			{
				Exception = new OperationCanceledException();
				IsDone = true;
				Status = AsyncOperationStatus.Cancelled;
				m_CompletedCallback?.Invoke(this);
				m_CompletedCallback = null;
			}
		}

		bool IEnumerator.MoveNext()
		{
			return !IsDone;
		}

		void IEnumerator.Reset()
		{
		}
	}
}
