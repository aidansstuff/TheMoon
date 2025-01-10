using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class CloudUniqueID : Attribute
	{
		internal readonly int uniqueID;

		public CloudUniqueID(int uniqueID)
		{
			this.uniqueID = uniqueID;
		}
	}
}
