using System;
using System.Threading.Tasks;

namespace Unity.Services.Relay.ErrorMitigation
{
	internal interface IRetryPolicyProvider
	{
		IRetryPolicy<T> ForOperation<T>(Func<int, Task<T>> operation);

		IRetryPolicy<T> ForOperation<T>(Func<Task<T>> operation);
	}
}
