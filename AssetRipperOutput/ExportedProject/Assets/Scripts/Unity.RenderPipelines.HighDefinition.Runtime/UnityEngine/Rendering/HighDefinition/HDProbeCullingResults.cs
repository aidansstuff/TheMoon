using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDProbeCullingResults
	{
		private static readonly IReadOnlyList<HDProbe> s_EmptyList = new List<HDProbe>();

		private List<HDProbe> m_VisibleProbes = new List<HDProbe>();

		public IReadOnlyList<HDProbe> visibleProbes => m_VisibleProbes;

		internal void Reset()
		{
			m_VisibleProbes.Clear();
		}

		internal void AddProbe(HDProbe visibleProbes)
		{
			m_VisibleProbes.Add(visibleProbes);
		}
	}
}
