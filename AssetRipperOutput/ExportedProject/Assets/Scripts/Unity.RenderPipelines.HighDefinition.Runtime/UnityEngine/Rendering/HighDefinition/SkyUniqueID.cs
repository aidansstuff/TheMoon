using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class SkyUniqueID : Attribute
	{
		internal readonly int uniqueID;

		public SkyUniqueID(int uniqueID)
		{
			this.uniqueID = uniqueID;
		}
	}
}
