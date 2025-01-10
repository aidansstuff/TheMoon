using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class CascadePartitionSplitParameter : VolumeParameter<float>
	{
		[NonSerialized]
		private NoInterpMinFloatParameter maxDistance;

		internal bool normalized;

		[NonSerialized]
		private CascadePartitionSplitParameter previous;

		[NonSerialized]
		private CascadePartitionSplitParameter next;

		[NonSerialized]
		private NoInterpClampedIntParameter cascadeCounts;

		private int minCascadeToAppears;

		internal float min => previous?.value ?? 0f;

		internal float max
		{
			get
			{
				if (cascadeCounts.value <= minCascadeToAppears || next == null)
				{
					return 1f;
				}
				return next.value;
			}
		}

		internal float representationDistance => maxDistance.value;

		public override float value
		{
			get
			{
				return m_Value;
			}
			set
			{
				m_Value = Mathf.Clamp(value, min, max);
			}
		}

		public CascadePartitionSplitParameter(float value, bool normalized = false, bool overrideState = false)
			: base(value, overrideState)
		{
			this.normalized = normalized;
		}

		internal void Init(NoInterpClampedIntParameter cascadeCounts, int minCascadeToAppears, NoInterpMinFloatParameter maxDistance, CascadePartitionSplitParameter previous, CascadePartitionSplitParameter next)
		{
			this.maxDistance = maxDistance;
			this.previous = previous;
			this.next = next;
			this.cascadeCounts = cascadeCounts;
			this.minCascadeToAppears = minCascadeToAppears;
		}
	}
}
