using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Services.Core.Internal
{
	internal abstract class AsyncOperationBase : CustomYieldInstruction, IAsyncOperation, IEnumerator, INotifyCompletion
	{
		private Action<IAsyncOperation> m_CompletedCallback;

		public override bool keepWaiting => !IsCompleted;

		public abstract bool IsCompleted { get; }

		public bool IsDone => IsCompleted;

		public abstract AsyncOperationStatus Status { get; }

		public abstract Exception Exception { get; }

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

		public abstract void GetResult();

		public abstract AsyncOperationBase GetAwaiter();

		protected void DidComplete()
		{
			m_CompletedCallback?.Invoke(this);
		}

		public virtual void OnCompleted(Action continuation)
		{
			Completed += delegate
			{
				continuation?.Invoke();
			};
		}
	}
	internal abstract class AsyncOperationBase<T> : CustomYieldInstruction, IAsyncOperation<T>, IEnumerator, INotifyCompletion
	{
		private Action<IAsyncOperation<T>> m_CompletedCallback;

		public override bool keepWaiting => !IsCompleted;

		public abstract bool IsCompleted { get; }

		public bool IsDone => IsCompleted;

		public abstract AsyncOperationStatus Status { get; }

		public abstract Exception Exception { get; }

		public abstract T Result { get; }

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

		public abstract T GetResult();

		public abstract AsyncOperationBase<T> GetAwaiter();

		protected void DidComplete()
		{
			m_CompletedCallback?.Invoke(this);
		}

		public virtual void OnCompleted(Action continuation)
		{
			Completed += delegate
			{
				continuation?.Invoke();
			};
		}
	}
}
