using System.Runtime.CompilerServices;

namespace Unity.Services.Core.Internal
{
	internal interface IAsyncOperationAwaiter : ICriticalNotifyCompletion, INotifyCompletion
	{
		bool IsCompleted { get; }

		void GetResult();
	}
	internal interface IAsyncOperationAwaiter<out T> : ICriticalNotifyCompletion, INotifyCompletion
	{
		bool IsCompleted { get; }

		T GetResult();
	}
}
