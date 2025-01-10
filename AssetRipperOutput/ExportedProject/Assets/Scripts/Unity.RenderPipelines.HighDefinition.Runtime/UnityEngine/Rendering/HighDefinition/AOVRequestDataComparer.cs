using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class AOVRequestDataComparer : IEqualityComparer<AOVRequestData>
	{
		public bool Equals(AOVRequestData x, AOVRequestData y)
		{
			return x.HasSameSettings(y);
		}

		public int GetHashCode(AOVRequestData obj)
		{
			return obj.GetHash();
		}
	}
}
