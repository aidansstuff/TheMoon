using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Threading.Internal
{
	internal class UnityThreadUtilsInternal : IUnityThreadUtils, IServiceComponent
	{
		bool IUnityThreadUtils.IsRunningOnUnityThread => UnityThreadUtils.IsRunningOnUnityThread;

		public static Task PostAsync(Action action)
		{
			return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, UnityThreadUtils.UnityThreadScheduler);
		}

		public static Task PostAsync(Action<object> action, object state)
		{
			return Task.Factory.StartNew(action, state, CancellationToken.None, TaskCreationOptions.None, UnityThreadUtils.UnityThreadScheduler);
		}

		public static Task<T> PostAsync<T>(Func<T> action)
		{
			return Task<T>.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, UnityThreadUtils.UnityThreadScheduler);
		}

		public static Task<T> PostAsync<T>(Func<object, T> action, object state)
		{
			return Task<T>.Factory.StartNew(action, state, CancellationToken.None, TaskCreationOptions.None, UnityThreadUtils.UnityThreadScheduler);
		}

		public static void Send(Action action)
		{
			if (UnityThreadUtils.IsRunningOnUnityThread)
			{
				action();
			}
			else
			{
				PostAsync(action).Wait();
			}
		}

		public static void Send(Action<object> action, object state)
		{
			if (UnityThreadUtils.IsRunningOnUnityThread)
			{
				action(state);
			}
			else
			{
				PostAsync(action, state).Wait();
			}
		}

		public static T Send<T>(Func<T> action)
		{
			if (UnityThreadUtils.IsRunningOnUnityThread)
			{
				return action();
			}
			Task<T> task = PostAsync(action);
			task.Wait();
			return task.Result;
		}

		public static T Send<T>(Func<object, T> action, object state)
		{
			if (UnityThreadUtils.IsRunningOnUnityThread)
			{
				return action(state);
			}
			Task<T> task = PostAsync(action, state);
			task.Wait();
			return task.Result;
		}

		Task IUnityThreadUtils.PostAsync(Action action)
		{
			return PostAsync(action);
		}

		Task IUnityThreadUtils.PostAsync(Action<object> action, object state)
		{
			return PostAsync(action, state);
		}

		Task<T> IUnityThreadUtils.PostAsync<T>(Func<T> action)
		{
			return PostAsync(action);
		}

		Task<T> IUnityThreadUtils.PostAsync<T>(Func<object, T> action, object state)
		{
			return PostAsync(action, state);
		}

		void IUnityThreadUtils.Send(Action action)
		{
			Send(action);
		}

		void IUnityThreadUtils.Send(Action<object> action, object state)
		{
			Send(action, state);
		}

		T IUnityThreadUtils.Send<T>(Func<T> action)
		{
			return Send(action);
		}

		T IUnityThreadUtils.Send<T>(Func<object, T> action, object state)
		{
			return Send(action, state);
		}
	}
}
