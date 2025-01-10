using System;
using System.Diagnostics;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Lighting/Indirect Lighting Controller", new Type[] { typeof(HDRenderPipeline) })]
	public class IndirectLightingController : VolumeComponent
	{
		[Serializable]
		[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
		public sealed class LightLayerEnumParameter : VolumeParameter<LightLayerEnum>
		{
			public LightLayerEnumParameter(LightLayerEnum value, bool overrideState = false)
				: base(value, overrideState)
			{
			}
		}

		[FormerlySerializedAs("indirectDiffuseIntensity")]
		public MinFloatParameter indirectDiffuseLightingMultiplier = new MinFloatParameter(1f, 0f);

		public LightLayerEnumParameter indirectDiffuseLightingLayers = new LightLayerEnumParameter(LightLayerEnum.Everything);

		public MinFloatParameter reflectionLightingMultiplier = new MinFloatParameter(1f, 0f);

		public LightLayerEnumParameter reflectionLightingLayers = new LightLayerEnumParameter(LightLayerEnum.Everything);

		[FormerlySerializedAs("indirectSpecularIntensity")]
		public MinFloatParameter reflectionProbeIntensityMultiplier = new MinFloatParameter(1f, 0f);

		public uint GetReflectionLightingLayers()
		{
			int value = (int)reflectionLightingLayers.GetValue<LightLayerEnum>();
			if (value >= 0)
			{
				return (uint)value;
			}
			return 255u;
		}

		public uint GetIndirectDiffuseLightingLayers()
		{
			int value = (int)indirectDiffuseLightingLayers.GetValue<LightLayerEnum>();
			if (value >= 0)
			{
				return (uint)value;
			}
			return 255u;
		}
	}
}
