using System;
using System.Threading.Tasks;

namespace Unity.Services.Core.Internal
{
	internal static class AsyncOperationExtensions
	{
		public static AsyncOperationAwaiter GetAwaiter(this IAsyncOperation self)
		{
			return new AsyncOperationAwaiter(self);
		}

		public static Task AsTask(this IAsyncOperation self)
		{
			if (self.Status == AsyncOperationStatus.Succeeded)
			{
				return Task.CompletedTask;
			}
			TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
			if (self.IsDone)
			{
				CompleteTask(self);
			}
			else
			{
				self.Completed += CompleteTask;
			}
			return taskCompletionSource.Task;
			void CompleteTask(IAsyncOperation operation)
			{
				switch (operation.Status)
				{
				case AsyncOperationStatus.Failed:
					taskCompletionSource.TrySetException(operation.Exception);
					break;
				case AsyncOperationStatus.Cancelled:
					taskCompletionSource.TrySetCanceled();
					break;
				case AsyncOperationStatus.Succeeded:
					taskCompletionSource.TrySetResult(null);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		public static AsyncOperationAwaiter<T> GetAwaiter<T>(this IAsyncOperation<T> self)
		{
			return new AsyncOperationAwaiter<T>(self);
		}

		public static Task<T> AsTask<T>(this IAsyncOperation<T> self)
		{
			TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
			if (self.IsDone)
			{
				CompleteTask(self);
			}
			else
			{
				self.Completed += CompleteTask;
			}
			return taskCompletionSource.Task;
			void CompleteTask(IAsyncOperation<T> operation)
			{
				switch (operation.Status)
				{
				case AsyncOperationStatus.Succeeded:
					taskCompletionSource.TrySetResult(operation.Result);
					break;
				case AsyncOperationStatus.Failed:
					taskCompletionSource.TrySetException(operation.Exception);
					break;
				case AsyncOperationStatus.Cancelled:
					taskCompletionSource.TrySetCanceled();
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}
