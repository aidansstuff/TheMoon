using System;
using System.Threading.Tasks;

namespace Unity.Services.Relay.ErrorMitigation
{
	internal class RetryPolicyProvider : IRetryPolicyProvider
	{
		public IRetryPolicy<T> ForOperation<T>(Func<int, Task<T>> operation)
		{
			return RetryPolicy<T>.ForOperation(operation);
		}

		public IRetryPolicy<T> ForOperation<T>(Func<Task<T>> operation)
		{
			return RetryPolicy<T>.ForOperation(operation);
		}
	}
}
