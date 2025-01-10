using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.Netcode
{
	internal class SceneEventProgress
	{
		internal delegate bool OnCompletedDelegate(SceneEventProgress sceneEventProgress);

		internal List<ulong> ClientsThatDisconnected = new List<ulong>();

		internal float WhenSceneEventHasTimedOut;

		internal OnCompletedDelegate OnComplete;

		internal Action<uint> OnSceneEventCompleted;

		internal uint SceneEventId;

		private Coroutine m_TimeOutCoroutine;

		private AsyncOperation m_AsyncOperation;

		internal LoadSceneMode LoadSceneMode;

		internal Dictionary<ulong, bool> ClientsProcessingSceneEvent { get; } = new Dictionary<ulong, bool>();


		internal uint SceneHash { get; set; }

		internal Guid Guid { get; } = Guid.NewGuid();


		private NetworkManager m_NetworkManager { get; }

		internal SceneEventProgressStatus Status { get; set; }

		internal SceneEventType SceneEventType { get; set; }

		internal bool HasTimedOut()
		{
			return WhenSceneEventHasTimedOut <= m_NetworkManager.RealTimeProvider.RealTimeSinceStartup;
		}

		internal List<ulong> GetClientsWithStatus(bool completedSceneEvent)
		{
			List<ulong> list = new List<ulong>();
			if (completedSceneEvent)
			{
				if (m_NetworkManager.IsHost && m_AsyncOperation.isDone)
				{
					list.Add(m_NetworkManager.LocalClientId);
				}
				foreach (KeyValuePair<ulong, bool> item in ClientsProcessingSceneEvent)
				{
					if (item.Value == completedSceneEvent)
					{
						list.Add(item.Key);
					}
				}
			}
			else
			{
				if (m_NetworkManager.IsHost && !m_AsyncOperation.isDone)
				{
					list.Add(m_NetworkManager.LocalClientId);
				}
				list.AddRange(ClientsThatDisconnected);
			}
			return list;
		}

		internal SceneEventProgress(NetworkManager networkManager, SceneEventProgressStatus status = SceneEventProgressStatus.Started)
		{
			if (status == SceneEventProgressStatus.Started)
			{
				m_NetworkManager = networkManager;
				if (networkManager.IsServer)
				{
					m_NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
					foreach (ulong connectedClientsId in networkManager.ConnectedClientsIds)
					{
						if (connectedClientsId != 0L)
						{
							ClientsProcessingSceneEvent.Add(connectedClientsId, value: false);
						}
					}
					WhenSceneEventHasTimedOut = networkManager.RealTimeProvider.RealTimeSinceStartup + (float)networkManager.NetworkConfig.LoadSceneTimeOut;
					m_TimeOutCoroutine = m_NetworkManager.StartCoroutine(TimeOutSceneEventProgress());
				}
			}
			Status = status;
		}

		private void OnClientDisconnectCallback(ulong clientId)
		{
			if (ClientsProcessingSceneEvent.ContainsKey(clientId))
			{
				ClientsThatDisconnected.Add(clientId);
				ClientsProcessingSceneEvent.Remove(clientId);
			}
		}

		internal IEnumerator TimeOutSceneEventProgress()
		{
			WaitForSeconds waitForNetworkTick = new WaitForSeconds(1f / (float)m_NetworkManager.NetworkConfig.TickRate);
			while (!HasTimedOut())
			{
				yield return waitForNetworkTick;
				TryFinishingSceneEventProgress();
			}
		}

		internal void ClientFinishedSceneEvent(ulong clientId)
		{
			if (ClientsProcessingSceneEvent.ContainsKey(clientId))
			{
				ClientsProcessingSceneEvent[clientId] = true;
				TryFinishingSceneEventProgress();
			}
		}

		private bool HasFinished()
		{
			if (!IsNetworkSessionActive())
			{
				return true;
			}
			foreach (KeyValuePair<ulong, bool> item in ClientsProcessingSceneEvent)
			{
				if (!item.Value)
				{
					return false;
				}
			}
			if (m_AsyncOperation != null)
			{
				return m_AsyncOperation.isDone;
			}
			return false;
		}

		internal void SetAsyncOperation(AsyncOperation asyncOperation)
		{
			m_AsyncOperation = asyncOperation;
			m_AsyncOperation.completed += delegate
			{
				if (IsNetworkSessionActive())
				{
					OnSceneEventCompleted?.Invoke(SceneEventId);
				}
				TryFinishingSceneEventProgress();
			};
		}

		internal bool IsNetworkSessionActive()
		{
			if (m_NetworkManager != null && m_NetworkManager.IsListening)
			{
				return !m_NetworkManager.ShutdownInProgress;
			}
			return false;
		}

		internal void TryFinishingSceneEventProgress()
		{
			if (HasFinished() || HasTimedOut())
			{
				if (IsNetworkSessionActive())
				{
					OnComplete?.Invoke(this);
					m_NetworkManager.SceneManager.SceneEventProgressTracking.Remove(Guid);
					m_NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
				}
				if (m_TimeOutCoroutine != null)
				{
					m_NetworkManager.StopCoroutine(m_TimeOutCoroutine);
				}
			}
		}
	}
}
