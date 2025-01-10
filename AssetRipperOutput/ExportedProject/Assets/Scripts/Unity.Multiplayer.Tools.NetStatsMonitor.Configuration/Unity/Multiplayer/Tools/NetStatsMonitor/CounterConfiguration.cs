using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
	[Serializable]
	public sealed class CounterConfiguration
	{
		[SerializeField]
		[Range(1f, 7f)]
		[Tooltip("The number of significant digits to display for this counter.")]
		private int m_SignificantDigits = 3;

		[field: SerializeField]
		public SmoothingMethod SmoothingMethod { get; set; }

		[field: SerializeField]
		public AggregationMethod AggregationMethod { get; set; }

		public int SignificantDigits
		{
			get
			{
				return m_SignificantDigits;
			}
			set
			{
				m_SignificantDigits = Mathf.Clamp(value, 1, 7);
			}
		}

		[field: SerializeField]
		[field: Tooltip("Values below this threshold will be highlighted by the default styling, and can be highlighted by custom styling using the following USS classes: \"rnsm-counter-out-of-bounds\", or \"rnsm-counter-below-threshold\"")]
		public float HighlightLowerBound { get; set; } = float.NegativeInfinity;


		[field: SerializeField]
		[field: Tooltip("Values above this threshold will be highlighted by the default styling, and can be highlighted by custom styling using the following USS classes: \"rnsm-counter-out-of-bounds\", or \"rnsm-counter-above-threshold\"")]
		public float HighlightUpperBound { get; set; } = float.PositiveInfinity;


		[field: SerializeField]
		public ExponentialMovingAverageParams ExponentialMovingAverageParams { get; set; } = new ExponentialMovingAverageParams();


		[field: SerializeField]
		public SimpleMovingAverageParams SimpleMovingAverageParams { get; set; } = new SimpleMovingAverageParams();


		public int SampleCount
		{
			get
			{
				if (SmoothingMethod != SmoothingMethod.SimpleMovingAverage)
				{
					return 0;
				}
				return SimpleMovingAverageParams.SampleCount;
			}
		}

		internal int ComputeHashCode()
		{
			return HashCode.Combine((int)SmoothingMethod, (int)AggregationMethod, SignificantDigits, HighlightLowerBound, HighlightUpperBound, ExponentialMovingAverageParams.ComputeHashCode(), SimpleMovingAverageParams.ComputeHashCode());
		}
	}
}
