using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDRayTracingLights
	{
		public List<HDLightRenderEntity> hdPointLightArray = new List<HDLightRenderEntity>();

		public List<HDLightRenderEntity> hdLineLightArray = new List<HDLightRenderEntity>();

		public List<HDLightRenderEntity> hdRectLightArray = new List<HDLightRenderEntity>();

		public List<HDLightRenderEntity> hdLightEntityArray = new List<HDLightRenderEntity>();

		public List<HDAdditionalLightData> hdDirectionalLightArray = new List<HDAdditionalLightData>();

		public List<HDProbe> reflectionProbeArray = new List<HDProbe>();

		public int lightCount;

		internal void Reset()
		{
			hdDirectionalLightArray.Clear();
			hdPointLightArray.Clear();
			hdLineLightArray.Clear();
			hdRectLightArray.Clear();
			hdLightEntityArray.Clear();
			reflectionProbeArray.Clear();
			lightCount = 0;
		}
	}
}
