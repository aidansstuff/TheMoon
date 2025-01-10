using System;
using System.Runtime.CompilerServices;

namespace Unity.Services.Core.Internal
{
	internal struct AsyncOperationAwaiter : IAsyncOperationAwaiter, ICriticalNotifyCompletion, INotifyCompletion
	{
		private IAsyncOperation m_Operation;

		public bool IsCompleted => m_Operation.IsDone;

		public AsyncOperationAwaiter(IAsyncOperation asyncOperation)
		{
			m_Operation = asyncOperation;
		}

		public void OnCompleted(Action continuation)
		{
			m_Operation.Completed += delegate
			{
				continuation();
			};
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			m_Operation.Completed += delegate
			{
				continuation();
			};
		}

		public void GetResult()
		{
			if (m_Operation.Status == AsyncOperationStatus.Failed || m_Operation.Status == AsyncOperationStatus.Cancelled)
			{
				throw m_Operation.Exception;
			}
		}
	}
	internal struct AsyncOperationAwaiter<T> : IAsyncOperationAwaiter<T>, ICriticalNotifyCompletion, INotifyCompletion
	{
		private IAsyncOperation<T> m_Operation;

		public bool IsCompleted => m_Operation.IsDone;

		public AsyncOperationAwaiter(IAsyncOperation<T> asyncOperation)
		{
			m_Operation = asyncOperation;
		}

		public void OnCompleted(Action continuation)
		{
			m_Operation.Completed += delegate
			{
				continuation();
			};
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			m_Operation.Completed += delegate
			{
				continuation();
			};
		}

		public T GetResult()
		{
			if (m_Operation.Status == AsyncOperationStatus.Failed || m_Operation.Status == AsyncOperationStatus.Cancelled)
			{
				throw m_Operation.Exception;
			}
			return m_Operation.Result;
		}
	}
}
