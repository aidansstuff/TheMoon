using System;
using System.Threading.Tasks;

namespace Unity.Services.Relay.ErrorMitigation
{
	internal interface IRetryPolicy<T>
	{
		IRetryPolicy<T> WithJitterMagnitude(float magnitude);

		IRetryPolicy<T> WithDelayScale(float scale);

		IRetryPolicy<T> WithMaxDelayTime(float time);

		IRetryPolicy<T> WithRetryCondition(Func<T, Task<bool>> shouldRetry);

		IRetryPolicy<T> UptoMaximumRetries(uint amount);

		IRetryPolicy<T> HandleException<TException>() where TException : Exception;

		IRetryPolicy<T> HandleException<TException>(Func<TException, bool> condition) where TException : Exception;

		Task<T> RunAsync(RetryPolicyConfig retryPolicyConfig = null);
	}
}
