using System;
using System.Collections.Generic;

namespace Unity.Netcode
{
	internal static class ComponentFactory
	{
		internal delegate object CreateObjectDelegate(NetworkManager networkManager);

		private static Dictionary<Type, CreateObjectDelegate> s_Delegates = new Dictionary<Type, CreateObjectDelegate>();

		public static T Create<T>(NetworkManager networkManager)
		{
			return (T)s_Delegates[typeof(T)](networkManager);
		}

		public static void Register<T>(CreateObjectDelegate creator)
		{
			s_Delegates[typeof(T)] = creator;
		}

		public static void Deregister<T>()
		{
			s_Delegates.Remove(typeof(T));
			SetDefaults();
		}

		public static void SetDefaults()
		{
			SetDefault<IDeferredNetworkMessageManager>((NetworkManager networkManager) => new DeferredMessageManager(networkManager));
			SetDefault<IRealTimeProvider>((NetworkManager networkManager) => new RealTimeProvider());
		}

		private static void SetDefault<T>(CreateObjectDelegate creator)
		{
			if (!s_Delegates.ContainsKey(typeof(T)))
			{
				s_Delegates[typeof(T)] = creator;
			}
		}
	}
}
