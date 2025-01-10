using System;

namespace Unity.Collections
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
	public class BurstCompatibleAttribute : Attribute
	{
		public enum BurstCompatibleCompileTarget
		{
			Player = 0,
			Editor = 1,
			PlayerAndEditor = 2
		}

		public string RequiredUnityDefine;

		public BurstCompatibleCompileTarget CompileTarget;

		public Type[] GenericTypeArguments { get; set; }
	}
}
