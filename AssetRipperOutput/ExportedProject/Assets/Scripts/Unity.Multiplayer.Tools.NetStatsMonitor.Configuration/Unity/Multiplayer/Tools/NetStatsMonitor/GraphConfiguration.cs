using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
	[Serializable]
	public sealed class GraphConfiguration
	{
		[SerializeField]
		[Tooltip("The number of samples that are maintained for the purpose of graphing. The value is clamped to the range [8, 4096].")]
		[Range(8f, 4096f)]
		private int m_SampleCount = 256;

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

		[field: SerializeField]
		public List<Color> VariableColors { get; set; } = new List<Color>();


		[field: SerializeField]
		public GraphXAxisType XAxisType { get; set; }

		[field: SerializeField]
		public LineGraphConfiguration LineGraphConfiguration { get; set; } = new LineGraphConfiguration();


		internal int ComputeHashCode()
		{
			int num = HashCode.Combine(SampleCount, (int)XAxisType, LineGraphConfiguration.ComputeHashCode());
			if (VariableColors != null)
			{
				foreach (Color variableColor in VariableColors)
				{
					num = HashCode.Combine(num, variableColor);
				}
			}
			return num;
		}
	}
}
