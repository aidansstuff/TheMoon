using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
	[Serializable]
	public sealed class ExponentialMovingAverageParams
	{
		[SerializeField]
		[Min(0f)]
		private double m_HalfLife = 1.0;

		public double HalfLife
		{
			get
			{
				return m_HalfLife;
			}
			set
			{
				m_HalfLife = Math.Max(value, 0.0);
			}
		}

		internal int ComputeHashCode()
		{
			return HalfLife.GetHashCode();
		}
	}
}
