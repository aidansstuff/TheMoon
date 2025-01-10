namespace UnityEngine.Rendering.HighDefinition
{
	internal struct HDProbeCullState
	{
		private CullingGroup m_CullingGroup;

		private HDProbe[] m_HDProbes;

		private Hash128 m_StateHash;

		internal CullingGroup cullingGroup => m_CullingGroup;

		internal HDProbe[] hdProbes => m_HDProbes;

		internal Hash128 stateHash => m_StateHash;

		internal HDProbeCullState(CullingGroup cullingGroup, HDProbe[] hdProbes, Hash128 stateHash)
		{
			m_CullingGroup = cullingGroup;
			m_HDProbes = hdProbes;
			m_StateHash = stateHash;
		}
	}
}
