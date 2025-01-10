using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
	[Serializable]
	public sealed class SimpleMovingAverageParams
	{
		[SerializeField]
		[Min(1f)]
		[Tooltip("The number of samples that are maintained for the purpose of smoothing.The value is clamped to the range [8, 4096].")]
		[Range(8f, 4096f)]
		private int m_SampleCount = 64;

		public int SampleCount
		{
			get
			{
				return m_SampleCount;
			}
			set
			{
				m_SampleCount = Mathf.Clamp(value, 8, 4096);
			}
		}

		internal int ComputeHashCode()
		{
			return SampleCount;
		}
	}
}
