using System;
using System.Collections;
using UnityEngine;

namespace Unity.Services.Qos.Helpers
{
	internal static class AsyncOpRetry
	{
		public static AsyncOpRetry<T> FromCreateAsync<T>(Func<int, T> op)
		{
			return AsyncOpRetry<T>.FromCreateAsync(op);
		}
	}
	internal class AsyncOpRetry<T>
	{
		private uint MaxRetries { get; set; } = 4u;


		private float JitterMagnitude { get; set; } = 1f;


		private float DelayScale { get; set; } = 1f;


		private float MaxDelayTime { get; set; } = 8f;


		private Func<int, T> CreateOperation { get; set; }

		private Func<T, bool> RetryCondition { get; set; }

		private Action<T> OnComplete { get; set; }

		private AsyncOpRetry(Func<int, T> createAsyncOp)
		{
			CreateOperation = createAsyncOp;
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

		public static AsyncOpRetry<T> FromCreateAsync(Func<int, T> op)
		{
			return new AsyncOpRetry<T>(op);
		}

		public AsyncOpRetry<T> WithRetryCondition(Func<T, bool> shouldRetry)
		{
			RetryCondition = shouldRetry;
			return this;
		}

		public AsyncOpRetry<T> WhenComplete(Action<T> onComplete)
		{
			OnComplete = onComplete;
			return this;
		}

		public IEnumerator Run()
		{
			T asyncOp = default(T);
			int attempt = 0;
			while (attempt <= MaxRetries)
			{
				asyncOp = CreateOperation(attempt + 1);
				yield return asyncOp;
				Func<T, bool> retryCondition = RetryCondition;
				if (retryCondition != null && !retryCondition(asyncOp))
				{
					break;
				}
				float time = CalculateDelay(attempt, MaxDelayTime, DelayScale, JitterMagnitude);
				yield return new WaitForSecondsRealtime(time);
				int num = attempt + 1;
				attempt = num;
			}
			OnComplete?.Invoke(asyncOp);
		}
	}
}
