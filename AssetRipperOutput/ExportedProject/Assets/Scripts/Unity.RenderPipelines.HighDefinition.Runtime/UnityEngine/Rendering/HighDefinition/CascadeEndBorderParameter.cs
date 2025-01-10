using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class CascadeEndBorderParameter : VolumeParameter<float>
	{
		internal bool normalized;

		[NonSerialized]
		private CascadePartitionSplitParameter min;

		[NonSerialized]
		private CascadePartitionSplitParameter max;

		[NonSerialized]
		private NoInterpMinFloatParameter maxDistance;

		[NonSerialized]
		private NoInterpClampedIntParameter cascadeCounts;

		private int minCascadeToAppears;

		internal float representationDistance => (((cascadeCounts.value > minCascadeToAppears && max != null) ? max.value : 1f) - (min?.value ?? 0f)) * maxDistance.value;

		public override float value
		{
			get
			{
				return m_Value;
			}
			set
			{
				m_Value = Mathf.Clamp01(value);
			}
		}

		public CascadeEndBorderParameter(float value, bool normalized = false, bool overrideState = false)
			: base(value, overrideState)
		{
			this.normalized = normalized;
		}

		internal void Init(NoInterpClampedIntParameter cascadeCounts, int minCascadeToAppears, NoInterpMinFloatParameter maxDistance, CascadePartitionSplitParameter min, CascadePartitionSplitParameter max)
		{
			this.maxDistance = maxDistance;
			this.min = min;
			this.max = max;
			this.cascadeCounts = cascadeCounts;
			this.minCascadeToAppears = minCascadeToAppears;
		}
	}
}
