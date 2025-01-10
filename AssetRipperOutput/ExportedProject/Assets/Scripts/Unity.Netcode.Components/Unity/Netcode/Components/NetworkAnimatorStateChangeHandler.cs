using System.Collections.Generic;

namespace Unity.Netcode.Components
{
	internal class NetworkAnimatorStateChangeHandler : INetworkUpdateSystem
	{
		private struct AnimationUpdate
		{
			public ClientRpcParams ClientRpcParams;

			public NetworkAnimator.AnimationMessage AnimationMessage;
		}

		private struct ParameterUpdate
		{
			public ClientRpcParams ClientRpcParams;

			public NetworkAnimator.ParametersUpdateMessage ParametersUpdateMessage;
		}

		private struct TriggerUpdate
		{
			public bool SendToServer;

			public ClientRpcParams ClientRpcParams;

			public NetworkAnimator.AnimationTriggerMessage AnimationTriggerMessage;
		}

		private NetworkAnimator m_NetworkAnimator;

		private bool m_IsServer;

		private List<AnimationUpdate> m_SendAnimationUpdates = new List<AnimationUpdate>();

		private List<ParameterUpdate> m_SendParameterUpdates = new List<ParameterUpdate>();

		private List<NetworkAnimator.ParametersUpdateMessage> m_ProcessParameterUpdates = new List<NetworkAnimator.ParametersUpdateMessage>();

		private List<TriggerUpdate> m_SendTriggerUpdates = new List<TriggerUpdate>();

		private void FlushMessages()
		{
			foreach (AnimationUpdate sendAnimationUpdate in m_SendAnimationUpdates)
			{
				m_NetworkAnimator.SendAnimStateClientRpc(sendAnimationUpdate.AnimationMessage, sendAnimationUpdate.ClientRpcParams);
			}
			m_SendAnimationUpdates.Clear();
			foreach (ParameterUpdate sendParameterUpdate in m_SendParameterUpdates)
			{
				m_NetworkAnimator.SendParametersUpdateClientRpc(sendParameterUpdate.ParametersUpdateMessage, sendParameterUpdate.ClientRpcParams);
			}
			m_SendParameterUpdates.Clear();
			foreach (TriggerUpdate sendTriggerUpdate in m_SendTriggerUpdates)
			{
				if (!sendTriggerUpdate.SendToServer)
				{
					m_NetworkAnimator.SendAnimTriggerClientRpc(sendTriggerUpdate.AnimationTriggerMessage, sendTriggerUpdate.ClientRpcParams);
				}
				else
				{
					m_NetworkAnimator.SendAnimTriggerServerRpc(sendTriggerUpdate.AnimationTriggerMessage);
				}
			}
			m_SendTriggerUpdates.Clear();
		}

		public void NetworkUpdate(NetworkUpdateStage updateStage)
		{
			if (updateStage == NetworkUpdateStage.PreUpdate)
			{
				if (m_NetworkAnimator.IsOwner || m_IsServer)
				{
					FlushMessages();
				}
				for (int i = 0; i < m_ProcessParameterUpdates.Count; i++)
				{
					NetworkAnimator.ParametersUpdateMessage parametersUpdate = m_ProcessParameterUpdates[i];
					m_NetworkAnimator.UpdateParameters(ref parametersUpdate);
				}
				m_ProcessParameterUpdates.Clear();
				bool flag = m_NetworkAnimator.IsServerAuthoritative();
				if ((!flag && m_NetworkAnimator.IsOwner) || (flag && m_NetworkAnimator.IsServer))
				{
					m_NetworkAnimator.CheckForAnimatorChanges();
				}
			}
		}

		internal void SendAnimationUpdate(NetworkAnimator.AnimationMessage animationMessage, ClientRpcParams clientRpcParams = default(ClientRpcParams))
		{
			m_SendAnimationUpdates.Add(new AnimationUpdate
			{
				ClientRpcParams = clientRpcParams,
				AnimationMessage = animationMessage
			});
		}

		internal void SendParameterUpdate(NetworkAnimator.ParametersUpdateMessage parametersUpdateMessage, ClientRpcParams clientRpcParams = default(ClientRpcParams))
		{
			m_SendParameterUpdates.Add(new ParameterUpdate
			{
				ClientRpcParams = clientRpcParams,
				ParametersUpdateMessage = parametersUpdateMessage
			});
		}

		internal void ProcessParameterUpdate(NetworkAnimator.ParametersUpdateMessage parametersUpdateMessage)
		{
			m_ProcessParameterUpdates.Add(parametersUpdateMessage);
		}

		internal void QueueTriggerUpdateToClient(NetworkAnimator.AnimationTriggerMessage animationTriggerMessage, ClientRpcParams clientRpcParams = default(ClientRpcParams))
		{
			m_SendTriggerUpdates.Add(new TriggerUpdate
			{
				ClientRpcParams = clientRpcParams,
				AnimationTriggerMessage = animationTriggerMessage
			});
		}

		internal void QueueTriggerUpdateToServer(NetworkAnimator.AnimationTriggerMessage animationTriggerMessage)
		{
			m_SendTriggerUpdates.Add(new TriggerUpdate
			{
				AnimationTriggerMessage = animationTriggerMessage,
				SendToServer = true
			});
		}

		internal void DeregisterUpdate()
		{
			this.UnregisterNetworkUpdate(NetworkUpdateStage.PreUpdate);
		}

		internal NetworkAnimatorStateChangeHandler(NetworkAnimator networkAnimator)
		{
			m_NetworkAnimator = networkAnimator;
			m_IsServer = networkAnimator.NetworkManager.IsServer;
			this.RegisterNetworkUpdate(NetworkUpdateStage.PreUpdate);
		}
	}
}
