using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
	[Serializable]
	public sealed class LineGraphConfiguration
	{
		[SerializeField]
		[Range(1f, 5f)]
		private float m_LineThickness = 1f;

		public float LineThickness
		{
			get
			{
				return m_LineThickness;
			}
			set
			{
				m_LineThickness = Mathf.Clamp(value, 1f, 5f);
			}
		}

		internal int ComputeHashCode()
		{
			return LineThickness.GetHashCode();
		}
	}
}
