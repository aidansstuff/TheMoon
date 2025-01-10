using System;
using System.Collections;

namespace Unity.Services.Core.Internal
{
	internal interface IAsyncOperation : IEnumerator
	{
		bool IsDone { get; }

		AsyncOperationStatus Status { get; }

		Exception Exception { get; }

		event Action<IAsyncOperation> Completed;
	}
	internal interface IAsyncOperation<out T> : IEnumerator
	{
		bool IsDone { get; }

		AsyncOperationStatus Status { get; }

		Exception Exception { get; }

		T Result { get; }

		event Action<IAsyncOperation<T>> Completed;
	}
}
