using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.Relay.ErrorMitigation
{
	internal class RetryPolicy<T> : IRetryPolicy<T>
	{
		private RetryPolicyConfig _retryPolicyConfig = new RetryPolicyConfig();

		private Func<int, Task<T>> CreateOperation { get; set; }

		private Func<T, Task<bool>> RetryCondition { get; set; }

		private RetryPolicy(Func<int, Task<T>> createAsyncOp)
		{
			CreateOperation = createAsyncOp;
		}

		private RetryPolicy(Func<Task<T>> createAsyncOp)
		{
			CreateOperation = (int _) => createAsyncOp();
		}

		private static float AddJitter(float number, float magnitude)
		{
			return number + UnityEngine.Random.value * magnitude;
		}

		private static float Pow2(float exponent, float scale)
		{
			return (float)(Math.Pow(2.0, exponent) * (double)scale);
		}

		private static float CalculateDelay(int attemptNumber, float maxDelayTime, float delayScale, float jitterMagnitude)
		{
			return Math.Min(AddJitter(Pow2(attemptNumber, delayScale), jitterMagnitude), maxDelayTime);
		}

		public IRetryPolicy<T> WithJitterMagnitude(float magnitude)
		{
			_retryPolicyConfig.JitterMagnitude = magnitude;
			return this;
		}

		public IRetryPolicy<T> WithDelayScale(float scale)
		{
			_retryPolicyConfig.DelayScale = scale;
			return this;
		}

		public IRetryPolicy<T> WithMaxDelayTime(float time)
		{
			_retryPolicyConfig.MaxDelayTime = time;
			return this;
		}

		public static RetryPolicy<T> ForOperation(Func<int, Task<T>> operation)
		{
			return new RetryPolicy<T>(operation);
		}

		public static RetryPolicy<T> ForOperation(Func<Task<T>> operation)
		{
			return new RetryPolicy<T>(operation);
		}

		public IRetryPolicy<T> WithRetryCondition(Func<T, Task<bool>> shouldRetry)
		{
			RetryCondition = shouldRetry;
			return this;
		}

		public IRetryPolicy<T> UptoMaximumRetries(uint amount)
		{
			_retryPolicyConfig.MaxRetries = amount;
			return this;
		}

		public IRetryPolicy<T> HandleException<TException>() where TException : Exception
		{
			_retryPolicyConfig.HandleException<TException>();
			return this;
		}

		public IRetryPolicy<T> HandleException<TException>(Func<TException, bool> condition) where TException : Exception
		{
			_retryPolicyConfig.HandleException(condition);
			return this;
		}

		public async Task<T> RunAsync(RetryPolicyConfig retryPolicyConfig = null)
		{
			T opResult = default(T);
			if (retryPolicyConfig == null)
			{
				retryPolicyConfig = _retryPolicyConfig;
			}
			int attempt = 0;
			while (attempt <= retryPolicyConfig.MaxRetries)
			{
				try
				{
					opResult = await CreateOperation(attempt + 1);
				}
				catch (Exception e)
				{
					if (!retryPolicyConfig.IsHandledException(e))
					{
						throw;
					}
				}
				if (RetryCondition != null && opResult != null)
				{
					if (!(await RetryCondition(opResult)))
					{
						break;
					}
				}
				else if (opResult != null)
				{
					break;
				}
				await Task.Delay((int)(CalculateDelay(attempt, retryPolicyConfig.MaxDelayTime, retryPolicyConfig.DelayScale, retryPolicyConfig.JitterMagnitude) * 1000f));
				int num = attempt + 1;
				attempt = num;
			}
			return opResult;
		}
	}
}
