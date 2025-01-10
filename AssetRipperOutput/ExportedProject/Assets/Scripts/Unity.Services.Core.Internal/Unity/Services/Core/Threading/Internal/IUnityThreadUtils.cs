using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Threading.Internal
{
	public interface IUnityThreadUtils : IServiceComponent
	{
		bool IsRunningOnUnityThread { get; }

		Task PostAsync([NotNull] Action action);

		Task PostAsync([NotNull] Action<object> action, object state);

		Task<T> PostAsync<T>([NotNull] Func<T> action);

		Task<T> PostAsync<T>([NotNull] Func<object, T> action, object state);

		void Send([NotNull] Action action);

		void Send([NotNull] Action<object> action, object state);

		T Send<T>([NotNull] Func<T> action);

		T Send<T>([NotNull] Func<object, T> action, object state);
	}
}
