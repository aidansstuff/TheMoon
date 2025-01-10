using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	internal class CopyFilterAttribute : Attribute
	{
		public enum Filter
		{
			Exclude = 1,
			CheckContent = 2
		}

		protected CopyFilterAttribute(Filter test)
		{
		}
	}
}
