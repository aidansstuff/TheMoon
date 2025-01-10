using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDProbeSystemInternal : IDisposable
	{
		private HashSet<HDProbe> m_BakedProbes = new HashSet<HDProbe>();

		private HashSet<HDProbe> m_RealtimeViewDependentProbes = new HashSet<HDProbe>();

		private HashSet<HDProbe> m_RealtimeViewIndependentProbes = new HashSet<HDProbe>();

		private int m_PlanarProbeCount;

		private bool m_RebuildPlanarProbeArray;

		private HashSet<HDProbe> m_PlanarProbes = new HashSet<HDProbe>();

		private PlanarReflectionProbe[] m_PlanarProbesArray = new PlanarReflectionProbe[32];

		private BoundingSphere[] m_PlanarProbeBounds = new BoundingSphere[32];

		private CullingGroup m_PlanarProbeCullingGroup = new CullingGroup();

		public ReflectionSystemParameters Parameters;

		private int[] m_QueryCullResults_Indices;

		public IEnumerable<HDProbe> bakedProbes
		{
			get
			{
				RemoveDestroyedProbes(m_BakedProbes);
				return m_BakedProbes;
			}
		}

		public IEnumerable<HDProbe> realtimeViewDependentProbes
		{
			get
			{
				RemoveDestroyedProbes(m_RealtimeViewDependentProbes);
				return m_RealtimeViewDependentProbes;
			}
		}

		public IEnumerable<HDProbe> realtimeViewIndependentProbes
		{
			get
			{
				RemoveDestroyedProbes(m_RealtimeViewIndependentProbes);
				return m_RealtimeViewIndependentProbes;
			}
		}

		public int bakedProbeCount => m_BakedProbes.Count;

		public void Dispose()
		{
			m_PlanarProbeCullingGroup.Dispose();
			m_PlanarProbeCullingGroup = null;
		}

		internal void RegisterProbe(HDProbe probe)
		{
			ProbeSettings settings = probe.settings;
			switch (settings.mode)
			{
			case ProbeSettings.Mode.Baked:
				m_BakedProbes.Add(probe);
				break;
			case ProbeSettings.Mode.Realtime:
				switch (settings.type)
				{
				case ProbeSettings.ProbeType.PlanarProbe:
					if (!m_RealtimeViewDependentProbes.Contains(probe))
					{
						m_RealtimeViewDependentProbes.Add(probe);
					}
					break;
				case ProbeSettings.ProbeType.ReflectionProbe:
					if (!m_RealtimeViewIndependentProbes.Contains(probe))
					{
						m_RealtimeViewIndependentProbes.Add(probe);
					}
					break;
				}
				break;
			}
			if (settings.type == ProbeSettings.ProbeType.PlanarProbe && m_PlanarProbes.Add((PlanarReflectionProbe)probe))
			{
				if (m_PlanarProbeCount >= m_PlanarProbesArray.Length)
				{
					Array.Resize(ref m_PlanarProbesArray, m_PlanarProbeCount * 2);
					Array.Resize(ref m_PlanarProbeBounds, m_PlanarProbeCount * 2);
				}
				m_PlanarProbesArray[m_PlanarProbeCount] = (PlanarReflectionProbe)probe;
				m_PlanarProbeBounds[m_PlanarProbeCount] = ((PlanarReflectionProbe)probe).boundingSphere;
				m_PlanarProbeCount++;
			}
		}

		internal void UnregisterProbe(HDProbe probe)
		{
			m_BakedProbes.Remove(probe);
			m_RealtimeViewDependentProbes.Remove(probe);
			m_RealtimeViewIndependentProbes.Remove(probe);
			if (m_PlanarProbes.Remove(probe))
			{
				m_RebuildPlanarProbeArray = true;
			}
		}

		internal HDProbeCullState PrepareCull(Camera camera)
		{
			if (m_PlanarProbeCullingGroup == null)
			{
				return default(HDProbeCullState);
			}
			RebuildPlanarProbeArrayIfRequired();
			UpdateBoundsAndRemoveDestroyedProbes(m_PlanarProbesArray, m_PlanarProbeBounds, ref m_PlanarProbeCount);
			m_PlanarProbeCullingGroup.targetCamera = camera;
			m_PlanarProbeCullingGroup.SetBoundingSpheres(m_PlanarProbeBounds);
			m_PlanarProbeCullingGroup.SetBoundingSphereCount(m_PlanarProbeCount);
			BoundingSphere[] planarProbeBounds = m_PlanarProbeBounds;
			HDProbe[] planarProbesArray = m_PlanarProbesArray;
			Hash128 stateHash = ComputeStateHashDebug(planarProbeBounds, planarProbesArray, m_PlanarProbeCount);
			CullingGroup planarProbeCullingGroup = m_PlanarProbeCullingGroup;
			planarProbesArray = m_PlanarProbesArray;
			return new HDProbeCullState(planarProbeCullingGroup, planarProbesArray, stateHash);
		}

		private void RebuildPlanarProbeArrayIfRequired()
		{
			if (!m_RebuildPlanarProbeArray)
			{
				return;
			}
			RemoveDestroyedProbes(m_PlanarProbes);
			m_RebuildPlanarProbeArray = false;
			int num = 0;
			foreach (HDProbe planarProbe in m_PlanarProbes)
			{
				m_PlanarProbesArray[num] = (PlanarReflectionProbe)planarProbe;
				num++;
			}
			m_PlanarProbeCount = m_PlanarProbes.Count;
		}

		internal void QueryCullResults(HDProbeCullState state, ref HDProbeCullingResults results)
		{
			BoundingSphere[] planarProbeBounds = m_PlanarProbeBounds;
			HDProbe[] planarProbesArray = m_PlanarProbesArray;
			ComputeStateHashDebug(planarProbeBounds, planarProbesArray, m_PlanarProbeCount);
			results.Reset();
			Array.Resize(ref m_QueryCullResults_Indices, Parameters.maxActivePlanarReflectionProbe + Parameters.maxActiveEnvReflectionProbe);
			int num = state.cullingGroup.QueryIndices(visible: true, m_QueryCullResults_Indices, 0);
			for (int i = 0; i < num; i++)
			{
				results.AddProbe(state.hdProbes[m_QueryCullResults_Indices[i]]);
			}
		}

		private static void RemoveDestroyedProbes(HashSet<HDProbe> probes)
		{
			probes.RemoveWhere((HDProbe p) => p == null || p.Equals(null));
		}

		private static void UpdateBoundsAndRemoveDestroyedProbes(PlanarReflectionProbe[] probes, BoundingSphere[] bounds, ref int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (probes[i] == null || probes[i].Equals(null))
				{
					probes[i] = probes[count - 1];
					bounds[i] = bounds[count - 1];
					probes[count - 1] = null;
					count--;
				}
				if ((bool)probes[i])
				{
					bounds[i] = probes[i].boundingSphere;
				}
			}
		}

		private static Hash128 ComputeStateHashDebug(BoundingSphere[] probeBounds, HDProbe[] probes, int probeCount)
		{
			return default(Hash128);
		}
	}
}
