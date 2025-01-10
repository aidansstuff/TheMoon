using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Networking.Transport
{
	public static class NetworkPipelineStageCollection
	{
		internal static List<INetworkPipelineStage> m_stages;

		static NetworkPipelineStageCollection()
		{
			m_stages = new List<INetworkPipelineStage>();
			RegisterPipelineStage(default(NullPipelineStage));
			RegisterPipelineStage(default(FragmentationPipelineStage));
			RegisterPipelineStage(default(ReliableSequencedPipelineStage));
			RegisterPipelineStage(default(UnreliableSequencedPipelineStage));
			RegisterPipelineStage(default(SimulatorPipelineStage));
			RegisterPipelineStage(default(SimulatorPipelineStageInSend));
		}

		public static void RegisterPipelineStage(INetworkPipelineStage stage)
		{
			for (int i = 0; i < m_stages.Count; i++)
			{
				if (m_stages[i].GetType() == stage.GetType())
				{
					m_stages[i] = stage;
					return;
				}
			}
			m_stages.Add(stage);
		}

		public static NetworkPipelineStageId GetStageId(Type stageType)
		{
			for (int i = 0; i < m_stages.Count; i++)
			{
				if (stageType == m_stages[i].GetType())
				{
					NetworkPipelineStageId result = default(NetworkPipelineStageId);
					result.Index = i;
					result.IsValid = 1;
					return result;
				}
			}
			Debug.LogError($"Pipeline stage {stageType} is not registered");
			return default(NetworkPipelineStageId);
		}
	}
}
