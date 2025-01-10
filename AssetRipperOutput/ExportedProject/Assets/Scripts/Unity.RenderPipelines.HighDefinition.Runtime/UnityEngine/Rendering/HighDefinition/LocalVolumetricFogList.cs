using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	internal struct LocalVolumetricFogList
	{
		public List<OrientedBBox> bounds;

		public List<LocalVolumetricFogEngineData> density;

		public int volumeCount;
	}
}
