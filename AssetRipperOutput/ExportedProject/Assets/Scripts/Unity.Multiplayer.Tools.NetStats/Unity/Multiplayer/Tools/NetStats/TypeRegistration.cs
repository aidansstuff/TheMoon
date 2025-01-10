using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal static class TypeRegistration
	{
		public const string k_ClassName = "<NetStats_TypeRegistration>";

		public const string k_MethodName = "Run";

		private static bool s_TypeRegistrationComplete;

		private static readonly object s_LockObject = new object();

		public static void RunIfNeeded()
		{
			lock (s_LockObject)
			{
				if (s_TypeRegistrationComplete)
				{
					return;
				}
				s_TypeRegistrationComplete = true;
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					if (assembly.GetCustomAttributes<AssemblyRequiresTypeRegistrationAttribute>().Any())
					{
						MethodInfo methodInfo = assembly.GetType("<NetStats_TypeRegistration>")?.GetMethod("Run", BindingFlags.Static | BindingFlags.NonPublic);
						if (methodInfo == null)
						{
							Debug.LogError("Failed to load type initialization for assembly " + assembly.GetName().Name);
						}
						else
						{
							methodInfo.Invoke(null, null);
						}
					}
				}
				MetricIdTypeLibrary.TypeRegistrationPostProcess();
			}
		}
	}
}
