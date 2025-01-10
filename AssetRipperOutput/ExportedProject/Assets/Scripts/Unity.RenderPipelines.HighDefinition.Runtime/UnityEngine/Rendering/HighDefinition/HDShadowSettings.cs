using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Shadowing/Shadows", new Type[] { typeof(HDRenderPipeline) })]
	public class HDShadowSettings : VolumeComponent
	{
		private float[] m_CascadeShadowSplits = new float[3];

		private float[] m_CascadeShadowBorders = new float[4];

		[Tooltip("Sets the maximum distance HDRP renders shadows for all Light types.")]
		public NoInterpMinFloatParameter maxShadowDistance = new NoInterpMinFloatParameter(500f, 0f);

		[Tooltip("Multiplier for thick transmission.")]
		public ClampedFloatParameter directionalTransmissionMultiplier = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Controls the number of cascades HDRP uses for cascaded shadow maps.")]
		public NoInterpClampedIntParameter cascadeShadowSplitCount = new NoInterpClampedIntParameter(4, 1, 4);

		[Tooltip("Sets the position of the first cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.")]
		public CascadePartitionSplitParameter cascadeShadowSplit0 = new CascadePartitionSplitParameter(0.05f);

		[Tooltip("Sets the position of the second cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.")]
		public CascadePartitionSplitParameter cascadeShadowSplit1 = new CascadePartitionSplitParameter(0.15f);

		[Tooltip("Sets the position of the third cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.")]
		public CascadePartitionSplitParameter cascadeShadowSplit2 = new CascadePartitionSplitParameter(0.3f);

		[Tooltip("Sets the border size between the first and second cascade split.")]
		public CascadeEndBorderParameter cascadeShadowBorder0 = new CascadeEndBorderParameter(0f);

		[Tooltip("Sets the border size between the second and third cascade split.")]
		public CascadeEndBorderParameter cascadeShadowBorder1 = new CascadeEndBorderParameter(0f);

		[Tooltip("Sets the border size between the third and last cascade split.")]
		public CascadeEndBorderParameter cascadeShadowBorder2 = new CascadeEndBorderParameter(0f);

		[Tooltip("Sets the border size at the end of the last cascade split.")]
		public CascadeEndBorderParameter cascadeShadowBorder3 = new CascadeEndBorderParameter(0f);

		public float[] cascadeShadowSplits
		{
			get
			{
				m_CascadeShadowSplits[0] = cascadeShadowSplit0.value;
				m_CascadeShadowSplits[1] = cascadeShadowSplit1.value;
				m_CascadeShadowSplits[2] = cascadeShadowSplit2.value;
				return m_CascadeShadowSplits;
			}
		}

		public float[] cascadeShadowBorders
		{
			get
			{
				m_CascadeShadowBorders[0] = cascadeShadowBorder0.value;
				m_CascadeShadowBorders[1] = cascadeShadowBorder1.value;
				m_CascadeShadowBorders[2] = cascadeShadowBorder2.value;
				m_CascadeShadowBorders[3] = cascadeShadowBorder3.value;
				if (!HDRenderPipeline.s_UseCascadeBorders)
				{
					m_CascadeShadowBorders[cascadeShadowSplitCount.value - 1] = 0.2f;
				}
				return m_CascadeShadowBorders;
			}
		}

		private HDShadowSettings()
		{
			base.displayName = "Shadows";
			cascadeShadowSplit0.Init(cascadeShadowSplitCount, 2, maxShadowDistance, null, cascadeShadowSplit1);
			cascadeShadowSplit1.Init(cascadeShadowSplitCount, 3, maxShadowDistance, cascadeShadowSplit0, cascadeShadowSplit2);
			cascadeShadowSplit2.Init(cascadeShadowSplitCount, 4, maxShadowDistance, cascadeShadowSplit1, null);
			cascadeShadowBorder0.Init(cascadeShadowSplitCount, 1, maxShadowDistance, null, cascadeShadowSplit0);
			cascadeShadowBorder1.Init(cascadeShadowSplitCount, 2, maxShadowDistance, cascadeShadowSplit0, cascadeShadowSplit1);
			cascadeShadowBorder2.Init(cascadeShadowSplitCount, 3, maxShadowDistance, cascadeShadowSplit1, cascadeShadowSplit2);
			cascadeShadowBorder3.Init(cascadeShadowSplitCount, 4, maxShadowDistance, cascadeShadowSplit2, null);
		}

		internal void InitNormalized(bool normalized)
		{
			cascadeShadowSplit0.normalized = normalized;
			cascadeShadowSplit1.normalized = normalized;
			cascadeShadowSplit2.normalized = normalized;
			cascadeShadowBorder0.normalized = normalized;
			cascadeShadowBorder1.normalized = normalized;
			cascadeShadowBorder2.normalized = normalized;
			cascadeShadowBorder3.normalized = normalized;
		}
	}
}
