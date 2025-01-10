using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.Netcode
{
	public class NetworkSceneManager : IDisposable
	{
		public delegate void SceneEventDelegate(SceneEvent sceneEvent);

		public delegate void OnLoadDelegateHandler(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation);

		public delegate void OnUnloadDelegateHandler(ulong clientId, string sceneName, AsyncOperation asyncOperation);

		public delegate void OnSynchronizeDelegateHandler(ulong clientId);

		public delegate void OnEventCompletedDelegateHandler(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut);

		public delegate void OnLoadCompleteDelegateHandler(ulong clientId, string sceneName, LoadSceneMode loadSceneMode);

		public delegate void OnUnloadCompleteDelegateHandler(ulong clientId, string sceneName);

		public delegate void OnSynchronizeCompleteDelegateHandler(ulong clientId);

		public delegate bool VerifySceneBeforeLoadingDelegateHandler(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode);

		public delegate bool VerifySceneBeforeUnloadingDelegateHandler(Scene scene);

		internal class SceneUnloadEventHandler
		{
			private static Dictionary<NetworkManager, List<SceneUnloadEventHandler>> s_Instances = new Dictionary<NetworkManager, List<SceneUnloadEventHandler>>();

			private NetworkSceneManager m_NetworkSceneManager;

			private AsyncOperation m_AsyncOperation;

			private LoadSceneMode m_LoadSceneMode;

			private ulong m_ClientId;

			private Scene m_Scene;

			private bool m_ShuttingDown;

			internal static void RegisterScene(NetworkSceneManager networkSceneManager, Scene scene, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation = null)
			{
				NetworkManager networkManager = networkSceneManager.NetworkManager;
				if (!s_Instances.ContainsKey(networkManager))
				{
					s_Instances.Add(networkManager, new List<SceneUnloadEventHandler>());
				}
				ulong clientId = (networkManager.IsServer ? 0 : networkManager.LocalClientId);
				s_Instances[networkManager].Add(new SceneUnloadEventHandler(networkSceneManager, scene, clientId, loadSceneMode, asyncOperation));
			}

			private static void SceneUnloadComplete(SceneUnloadEventHandler sceneUnloadEventHandler)
			{
				if (sceneUnloadEventHandler == null || sceneUnloadEventHandler.m_NetworkSceneManager == null || sceneUnloadEventHandler.m_NetworkSceneManager.NetworkManager == null)
				{
					return;
				}
				NetworkManager networkManager = sceneUnloadEventHandler.m_NetworkSceneManager.NetworkManager;
				if (s_Instances.ContainsKey(networkManager))
				{
					s_Instances[networkManager].Remove(sceneUnloadEventHandler);
					if (s_Instances[networkManager].Count == 0)
					{
						s_Instances.Remove(networkManager);
					}
				}
			}

			internal static void Shutdown()
			{
				foreach (KeyValuePair<NetworkManager, List<SceneUnloadEventHandler>> s_Instance in s_Instances)
				{
					foreach (SceneUnloadEventHandler item in s_Instance.Value)
					{
						item.OnShutdown();
					}
					s_Instance.Value.Clear();
				}
				s_Instances.Clear();
			}

			private void OnShutdown()
			{
				m_ShuttingDown = true;
				SceneManager.sceneUnloaded -= SceneUnloaded;
			}

			private void SceneUnloaded(Scene scene)
			{
				if (m_Scene.handle == scene.handle && !m_ShuttingDown)
				{
					if (m_NetworkSceneManager != null && m_NetworkSceneManager.NetworkManager != null)
					{
						m_NetworkSceneManager.OnSceneEvent?.Invoke(new SceneEvent
						{
							AsyncOperation = m_AsyncOperation,
							SceneEventType = SceneEventType.UnloadComplete,
							SceneName = m_Scene.name,
							LoadSceneMode = m_LoadSceneMode,
							ClientId = m_ClientId
						});
						m_NetworkSceneManager.OnUnloadComplete?.Invoke(m_ClientId, m_Scene.name);
					}
					SceneManager.sceneUnloaded -= SceneUnloaded;
					SceneUnloadComplete(this);
				}
			}

			private SceneUnloadEventHandler(NetworkSceneManager networkSceneManager, Scene scene, ulong clientId, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation = null)
			{
				m_LoadSceneMode = loadSceneMode;
				m_AsyncOperation = asyncOperation;
				m_NetworkSceneManager = networkSceneManager;
				m_ClientId = clientId;
				m_Scene = scene;
				SceneManager.sceneUnloaded += SceneUnloaded;
				m_NetworkSceneManager.OnSceneEvent?.Invoke(new SceneEvent
				{
					AsyncOperation = m_AsyncOperation,
					SceneEventType = SceneEventType.Unload,
					SceneName = m_Scene.name,
					LoadSceneMode = m_LoadSceneMode,
					ClientId = clientId
				});
				m_NetworkSceneManager.OnUnload?.Invoke(networkSceneManager.NetworkManager.LocalClientId, m_Scene.name, null);
			}
		}

		internal struct DeferredObjectsMovedEvent
		{
			internal Dictionary<int, List<ulong>> ObjectsMigratedTable;
		}

		private const NetworkDelivery k_DeliveryType = NetworkDelivery.ReliableFragmentedSequenced;

		internal const int InvalidSceneNameOrPath = -1;

		internal static bool DisableReSynchronization;

		private bool m_IsSceneEventActive;

		public VerifySceneBeforeLoadingDelegateHandler VerifySceneBeforeLoading;

		public VerifySceneBeforeUnloadingDelegateHandler VerifySceneBeforeUnloading;

		public bool PostSynchronizationSceneUnloading;

		private bool m_ActiveSceneSynchronizationEnabled;

		internal ISceneManagerHandler SceneManagerHandler = new DefaultSceneManagerHandler();

		internal readonly Dictionary<Guid, SceneEventProgress> SceneEventProgressTracking = new Dictionary<Guid, SceneEventProgress>();

		internal readonly Dictionary<uint, Dictionary<int, NetworkObject>> ScenePlacedObjects = new Dictionary<uint, Dictionary<int, NetworkObject>>();

		internal Scene SceneBeingSynchronized;

		internal Dictionary<int, Scene> ScenesLoaded = new Dictionary<int, Scene>();

		internal Dictionary<int, int> ServerSceneHandleToClientSceneHandle = new Dictionary<int, int>();

		internal Dictionary<int, int> ClientSceneHandleToServerSceneHandle = new Dictionary<int, int>();

		internal Dictionary<uint, int> HashToBuildIndex = new Dictionary<uint, int>();

		internal Dictionary<int, uint> BuildIndexToHash = new Dictionary<int, uint>();

		internal static bool IsSpawnedObjectsPendingInDontDestroyOnLoad;

		internal Dictionary<uint, SceneEventData> SceneEventDataStore;

		internal readonly NetworkManager NetworkManager;

		internal Scene DontDestroyOnLoadScene;

		private bool m_DisableValidationWarningMessages;

		internal Func<string, Scene> OverrideGetAndAddNewlyLoadedSceneByName;

		internal Func<Scene, bool> ExcludeSceneFromSychronization;

		internal Dictionary<int, List<NetworkObject>> ObjectsMigratedIntoNewScene = new Dictionary<int, List<NetworkObject>>();

		private List<int> m_ScenesToRemoveFromObjectMigration = new List<int>();

		internal List<DeferredObjectsMovedEvent> DeferredObjectsMovedEvents = new List<DeferredObjectsMovedEvent>();

		public bool ActiveSceneSynchronizationEnabled
		{
			get
			{
				return m_ActiveSceneSynchronizationEnabled;
			}
			set
			{
				if (m_ActiveSceneSynchronizationEnabled != value)
				{
					m_ActiveSceneSynchronizationEnabled = value;
					if (m_ActiveSceneSynchronizationEnabled)
					{
						SceneManager.activeSceneChanged += SceneManager_ActiveSceneChanged;
					}
					else
					{
						SceneManager.activeSceneChanged -= SceneManager_ActiveSceneChanged;
					}
				}
			}
		}

		public LoadSceneMode ClientSynchronizationMode { get; internal set; }

		public event SceneEventDelegate OnSceneEvent;

		public event OnLoadDelegateHandler OnLoad;

		public event OnUnloadDelegateHandler OnUnload;

		public event OnSynchronizeDelegateHandler OnSynchronize;

		public event OnEventCompletedDelegateHandler OnLoadEventCompleted;

		public event OnEventCompletedDelegateHandler OnUnloadEventCompleted;

		public event OnLoadCompleteDelegateHandler OnLoadComplete;

		public event OnUnloadCompleteDelegateHandler OnUnloadComplete;

		public event OnSynchronizeCompleteDelegateHandler OnSynchronizeComplete;

		internal bool UpdateServerClientSceneHandle(int serverHandle, int clientHandle, Scene localScene)
		{
			if (!ServerSceneHandleToClientSceneHandle.ContainsKey(serverHandle))
			{
				ServerSceneHandleToClientSceneHandle.Add(serverHandle, clientHandle);
				if (!ClientSceneHandleToServerSceneHandle.ContainsKey(clientHandle))
				{
					ClientSceneHandleToServerSceneHandle.Add(clientHandle, serverHandle);
					if (!ScenesLoaded.ContainsKey(clientHandle))
					{
						ScenesLoaded.Add(clientHandle, localScene);
					}
					return true;
				}
				return false;
			}
			return false;
		}

		internal bool RemoveServerClientSceneHandle(int serverHandle, int clientHandle)
		{
			if (ServerSceneHandleToClientSceneHandle.ContainsKey(serverHandle))
			{
				ServerSceneHandleToClientSceneHandle.Remove(serverHandle);
				if (ClientSceneHandleToServerSceneHandle.ContainsKey(clientHandle))
				{
					ClientSceneHandleToServerSceneHandle.Remove(clientHandle);
					if (ScenesLoaded.ContainsKey(clientHandle))
					{
						ScenesLoaded.Remove(clientHandle);
						return true;
					}
					return false;
				}
				return false;
			}
			return false;
		}

		public void Dispose()
		{
			SceneManager.activeSceneChanged -= SceneManager_ActiveSceneChanged;
			SceneUnloadEventHandler.Shutdown();
			foreach (KeyValuePair<uint, SceneEventData> item in SceneEventDataStore)
			{
				if (NetworkLog.CurrentLogLevel == LogLevel.Developer)
				{
					NetworkLog.LogInfo(string.Format("{0} is disposing {1} '{2}'.", "SceneEventDataStore", "SceneEventId", item.Key));
				}
				item.Value.Dispose();
			}
			SceneEventDataStore.Clear();
			SceneEventDataStore = null;
		}

		internal SceneEventData BeginSceneEvent()
		{
			SceneEventData sceneEventData = new SceneEventData(NetworkManager);
			SceneEventDataStore.Add(sceneEventData.SceneEventId, sceneEventData);
			return sceneEventData;
		}

		internal void EndSceneEvent(uint sceneEventId)
		{
			if (SceneEventDataStore.ContainsKey(sceneEventId))
			{
				SceneEventDataStore[sceneEventId].Dispose();
				SceneEventDataStore.Remove(sceneEventId);
			}
			else
			{
				Debug.LogWarning($"Trying to dispose and remove SceneEventData Id '{sceneEventId}' that no longer exists!");
			}
		}

		internal string GetSceneNameFromPath(string scenePath)
		{
			int num = scenePath.LastIndexOf("/", StringComparison.Ordinal) + 1;
			int num2 = scenePath.LastIndexOf(".", StringComparison.Ordinal);
			return scenePath.Substring(num, num2 - num);
		}

		internal void GenerateScenesInBuild()
		{
			HashToBuildIndex.Clear();
			BuildIndexToHash.Clear();
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				string scenePathByBuildIndex = SceneUtility.GetScenePathByBuildIndex(i);
				uint num = scenePathByBuildIndex.Hash32();
				int buildIndexByScenePath = SceneUtility.GetBuildIndexByScenePath(scenePathByBuildIndex);
				if (!HashToBuildIndex.ContainsKey(num))
				{
					HashToBuildIndex.Add(num, buildIndexByScenePath);
					BuildIndexToHash.Add(buildIndexByScenePath, num);
				}
				else
				{
					Debug.LogError("NetworkSceneManager is skipping duplicate scene path entry " + scenePathByBuildIndex + ". Make sure your scenes in build list does not contain duplicates!");
				}
			}
		}

		internal string SceneNameFromHash(uint sceneHash)
		{
			if (sceneHash == 0)
			{
				return "No Scene";
			}
			return GetSceneNameFromPath(ScenePathFromHash(sceneHash));
		}

		internal string ScenePathFromHash(uint sceneHash)
		{
			if (HashToBuildIndex.ContainsKey(sceneHash))
			{
				return SceneUtility.GetScenePathByBuildIndex(HashToBuildIndex[sceneHash]);
			}
			throw new Exception(string.Format("Scene Hash {0} does not exist in the {1} table!  Verify that all scenes requiring", sceneHash, "HashToBuildIndex") + " server to client synchronization are in the scenes in build list.");
		}

		internal uint SceneHashFromNameOrPath(string sceneNameOrPath)
		{
			int buildIndexByScenePath = SceneUtility.GetBuildIndexByScenePath(sceneNameOrPath);
			if (buildIndexByScenePath >= 0)
			{
				if (BuildIndexToHash.ContainsKey(buildIndexByScenePath))
				{
					return BuildIndexToHash[buildIndexByScenePath];
				}
				throw new Exception(string.Format("Scene '{0}' has a build index of {1} that does not exist in the {2} table!", sceneNameOrPath, buildIndexByScenePath, "BuildIndexToHash"));
			}
			throw new Exception("Scene '" + sceneNameOrPath + "' couldn't be loaded because it has not been added to the build settings scenes in build list.");
		}

		public void DisableValidationWarnings(bool disabled)
		{
			m_DisableValidationWarningMessages = disabled;
		}

		public void SetClientSynchronizationMode(LoadSceneMode mode)
		{
			NetworkManager networkManager = NetworkManager;
			SceneManagerHandler.SetClientSynchronizationMode(ref networkManager, mode);
		}

		internal NetworkSceneManager(NetworkManager networkManager)
		{
			NetworkManager = networkManager;
			SceneEventDataStore = new Dictionary<uint, SceneEventData>();
			GenerateScenesInBuild();
			DontDestroyOnLoadScene = networkManager.gameObject.scene;
			UpdateServerClientSceneHandle(DontDestroyOnLoadScene.handle, DontDestroyOnLoadScene.handle, DontDestroyOnLoadScene);
		}

		private void SceneManager_ActiveSceneChanged(Scene current, Scene next)
		{
			if (NetworkManager.ConnectedClientsIds.Count <= (NetworkManager.IsHost ? 1 : 0))
			{
				return;
			}
			foreach (KeyValuePair<Guid, SceneEventProgress> item in SceneEventProgressTracking)
			{
				if (!item.Value.HasTimedOut() && item.Value.Status == SceneEventProgressStatus.Started)
				{
					return;
				}
			}
			if (BuildIndexToHash.ContainsKey(next.buildIndex))
			{
				SceneEventData sceneEventData = BeginSceneEvent();
				sceneEventData.SceneEventType = SceneEventType.ActiveSceneChanged;
				sceneEventData.ActiveSceneHash = BuildIndexToHash[next.buildIndex];
				SendSceneEventData(sceneEventData.SceneEventId, NetworkManager.ConnectedClientsIds.Where((ulong c) => c != 0).ToArray());
				EndSceneEvent(sceneEventData.SceneEventId);
			}
		}

		internal bool ValidateSceneBeforeLoading(uint sceneHash, LoadSceneMode loadSceneMode)
		{
			string text = SceneNameFromHash(sceneHash);
			int buildIndexByScenePath = SceneUtility.GetBuildIndexByScenePath(text);
			return ValidateSceneBeforeLoading(buildIndexByScenePath, text, loadSceneMode);
		}

		internal bool ValidateSceneBeforeLoading(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode)
		{
			bool flag = true;
			if (VerifySceneBeforeLoading != null)
			{
				flag = VerifySceneBeforeLoading(sceneIndex, sceneName, loadSceneMode);
			}
			if (!flag && !m_DisableValidationWarningMessages)
			{
				string text = "Client";
				if (NetworkManager.IsServer)
				{
					text = (NetworkManager.IsHost ? "Host" : "Server");
				}
				Debug.LogWarning($"Scene {sceneName} of Scenes in Build Index {sceneIndex} being loaded in {loadSceneMode} mode failed validation on the {text}!");
			}
			return flag;
		}

		internal Scene GetAndAddNewlyLoadedSceneByName(string sceneName)
		{
			if (OverrideGetAndAddNewlyLoadedSceneByName != null)
			{
				return OverrideGetAndAddNewlyLoadedSceneByName(sceneName);
			}
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.name == sceneName && !ScenesLoaded.ContainsKey(sceneAt.handle))
				{
					ScenesLoaded.Add(sceneAt.handle, sceneAt);
					SceneManagerHandler.StartTrackingScene(sceneAt, assigned: true, NetworkManager);
					return sceneAt;
				}
			}
			throw new Exception("Failed to find any loaded scene named " + sceneName + "!");
		}

		internal void SetTheSceneBeingSynchronized(int serverSceneHandle)
		{
			int num = serverSceneHandle;
			if (ServerSceneHandleToClientSceneHandle.ContainsKey(serverSceneHandle))
			{
				num = ServerSceneHandleToClientSceneHandle[serverSceneHandle];
				if (!SceneBeingSynchronized.IsValid() || !SceneBeingSynchronized.isLoaded || SceneBeingSynchronized.handle != num)
				{
					SceneBeingSynchronized = (ScenesLoaded.ContainsKey(num) ? ScenesLoaded[num] : default(Scene));
					if (!SceneBeingSynchronized.IsValid() || !SceneBeingSynchronized.isLoaded)
					{
						SceneBeingSynchronized = SceneManager.GetActiveScene();
						Debug.LogWarning("[NetworkSceneManager- ScenesLoaded] Could not find the appropriate scene to set as being synchronized! Using the currently active scene.");
					}
				}
			}
			else if (serverSceneHandle == DontDestroyOnLoadScene.handle)
			{
				SceneBeingSynchronized = NetworkManager.gameObject.scene;
			}
			else
			{
				SceneBeingSynchronized = SceneManager.GetActiveScene();
				Debug.LogWarning("[SceneEventData- Scene Handle Mismatch] serverSceneHandle could not be found in ServerSceneHandleToClientSceneHandle. Using the currently active scene.");
			}
		}

		internal NetworkObject GetSceneRelativeInSceneNetworkObject(uint globalObjectIdHash, int? networkSceneHandle)
		{
			if (ScenePlacedObjects.ContainsKey(globalObjectIdHash))
			{
				int key = SceneBeingSynchronized.handle;
				if (networkSceneHandle.HasValue && networkSceneHandle.Value != 0)
				{
					key = ServerSceneHandleToClientSceneHandle[networkSceneHandle.Value];
				}
				if (ScenePlacedObjects[globalObjectIdHash].ContainsKey(key))
				{
					return ScenePlacedObjects[globalObjectIdHash][key];
				}
			}
			return null;
		}

		private void SendSceneEventData(uint sceneEventId, ulong[] targetClientIds)
		{
			if (targetClientIds.Length != 0)
			{
				SceneEventMessage sceneEventMessage = default(SceneEventMessage);
				sceneEventMessage.EventData = SceneEventDataStore[sceneEventId];
				SceneEventMessage message = sceneEventMessage;
				int num = NetworkManager.ConnectionManager.SendMessage(ref message, NetworkDelivery.ReliableFragmentedSequenced, in targetClientIds);
				NetworkManager.NetworkMetrics.TrackSceneEventSent(targetClientIds, (uint)SceneEventDataStore[sceneEventId].SceneEventType, SceneNameFromHash(SceneEventDataStore[sceneEventId].SceneHash), num);
			}
		}

		private SceneEventProgress ValidateSceneEventUnloading(Scene scene)
		{
			if (!NetworkManager.IsServer)
			{
				throw new NotServerException("Only server can start a scene event!");
			}
			if (!NetworkManager.NetworkConfig.EnableSceneManagement)
			{
				throw new Exception("EnableSceneManagement flag is not enabled in the NetworkManager's NetworkConfig. Please set EnableSceneManagement flag to true before calling LoadScene or UnloadScene.");
			}
			if (!scene.isLoaded)
			{
				Debug.LogWarning("UnloadScene was called, but the scene " + scene.name + " is not currently loaded!");
				return new SceneEventProgress(null, SceneEventProgressStatus.SceneNotLoaded);
			}
			return ValidateSceneEvent(scene.name, isUnloading: true);
		}

		private SceneEventProgress ValidateSceneEventLoading(string sceneName)
		{
			if (!NetworkManager.IsServer)
			{
				throw new NotServerException("Only server can start a scene event!");
			}
			if (!NetworkManager.NetworkConfig.EnableSceneManagement)
			{
				throw new Exception("EnableSceneManagement flag is not enabled in the NetworkManager's NetworkConfig. Please set EnableSceneManagement flag to true before calling LoadScene or UnloadScene.");
			}
			return ValidateSceneEvent(sceneName);
		}

		private SceneEventProgress ValidateSceneEvent(string sceneName, bool isUnloading = false)
		{
			if (m_IsSceneEventActive)
			{
				return new SceneEventProgress(null, SceneEventProgressStatus.SceneEventInProgress);
			}
			if (SceneUtility.GetBuildIndexByScenePath(sceneName) == -1)
			{
				Debug.LogError("Scene '" + sceneName + "' couldn't be loaded because it has not been added to the build settings scenes in build list.");
				return new SceneEventProgress(null, SceneEventProgressStatus.InvalidSceneName);
			}
			SceneEventProgress sceneEventProgress = new SceneEventProgress(NetworkManager)
			{
				SceneHash = SceneHashFromNameOrPath(sceneName)
			};
			SceneEventProgressTracking.Add(sceneEventProgress.Guid, sceneEventProgress);
			m_IsSceneEventActive = true;
			sceneEventProgress.OnComplete = OnSceneEventProgressCompleted;
			return sceneEventProgress;
		}

		private bool OnSceneEventProgressCompleted(SceneEventProgress sceneEventProgress)
		{
			SceneEventData sceneEventData = BeginSceneEvent();
			List<ulong> clientsWithStatus = sceneEventProgress.GetClientsWithStatus(completedSceneEvent: true);
			List<ulong> clientsWithStatus2 = sceneEventProgress.GetClientsWithStatus(completedSceneEvent: false);
			sceneEventData.SceneEventProgressId = sceneEventProgress.Guid;
			sceneEventData.SceneHash = sceneEventProgress.SceneHash;
			sceneEventData.SceneEventType = sceneEventProgress.SceneEventType;
			sceneEventData.ClientsCompleted = clientsWithStatus;
			sceneEventData.LoadSceneMode = sceneEventProgress.LoadSceneMode;
			sceneEventData.ClientsTimedOut = clientsWithStatus2;
			SceneEventMessage sceneEventMessage = default(SceneEventMessage);
			sceneEventMessage.EventData = sceneEventData;
			SceneEventMessage message = sceneEventMessage;
			NetworkConnectionManager connectionManager = NetworkManager.ConnectionManager;
			IReadOnlyList<ulong> clientIds = NetworkManager.ConnectedClientsIds;
			int num = connectionManager.SendMessage(ref message, NetworkDelivery.ReliableFragmentedSequenced, in clientIds);
			NetworkManager.NetworkMetrics.TrackSceneEventSent(NetworkManager.ConnectedClientsIds, (uint)sceneEventProgress.SceneEventType, SceneNameFromHash(sceneEventProgress.SceneHash), num);
			this.OnSceneEvent?.Invoke(new SceneEvent
			{
				SceneEventType = sceneEventProgress.SceneEventType,
				SceneName = SceneNameFromHash(sceneEventProgress.SceneHash),
				ClientId = 0uL,
				LoadSceneMode = sceneEventProgress.LoadSceneMode,
				ClientsThatCompleted = clientsWithStatus,
				ClientsThatTimedOut = clientsWithStatus2
			});
			if (sceneEventData.SceneEventType == SceneEventType.LoadEventCompleted)
			{
				this.OnLoadEventCompleted?.Invoke(SceneNameFromHash(sceneEventProgress.SceneHash), sceneEventProgress.LoadSceneMode, sceneEventData.ClientsCompleted, sceneEventData.ClientsTimedOut);
			}
			else
			{
				this.OnUnloadEventCompleted?.Invoke(SceneNameFromHash(sceneEventProgress.SceneHash), sceneEventProgress.LoadSceneMode, sceneEventData.ClientsCompleted, sceneEventData.ClientsTimedOut);
			}
			EndSceneEvent(sceneEventData.SceneEventId);
			return true;
		}

		public SceneEventProgressStatus UnloadScene(Scene scene)
		{
			string name = scene.name;
			int handle = scene.handle;
			if (!scene.isLoaded)
			{
				Debug.LogWarning("UnloadScene was called, but the scene " + scene.name + " is not currently loaded!");
				return SceneEventProgressStatus.SceneNotLoaded;
			}
			SceneEventProgress sceneEventProgress = ValidateSceneEventUnloading(scene);
			if (sceneEventProgress.Status != SceneEventProgressStatus.Started)
			{
				return sceneEventProgress.Status;
			}
			if (!ScenesLoaded.ContainsKey(handle))
			{
				Debug.LogError(string.Format("{0} internal error! {1} with handle {2} is not within the internal scenes loaded dictionary!", "UnloadScene", name, scene.handle));
				return SceneEventProgressStatus.InternalNetcodeError;
			}
			NetworkManager networkManager = NetworkManager;
			SceneManagerHandler.MoveObjectsFromSceneToDontDestroyOnLoad(ref networkManager, scene);
			SceneEventData sceneEventData = BeginSceneEvent();
			sceneEventData.SceneEventProgressId = sceneEventProgress.Guid;
			sceneEventData.SceneEventType = SceneEventType.Unload;
			sceneEventData.SceneHash = SceneHashFromNameOrPath(name);
			sceneEventData.LoadSceneMode = LoadSceneMode.Additive;
			sceneEventData.SceneHandle = handle;
			sceneEventProgress.SceneEventType = SceneEventType.UnloadEventCompleted;
			ScenesLoaded.Remove(scene.handle);
			sceneEventProgress.SceneEventId = sceneEventData.SceneEventId;
			sceneEventProgress.OnSceneEventCompleted = OnSceneUnloaded;
			AsyncOperation asyncOperation = SceneManagerHandler.UnloadSceneAsync(scene, sceneEventProgress);
			this.OnSceneEvent?.Invoke(new SceneEvent
			{
				AsyncOperation = asyncOperation,
				SceneEventType = sceneEventData.SceneEventType,
				LoadSceneMode = sceneEventData.LoadSceneMode,
				SceneName = name,
				ClientId = 0uL
			});
			this.OnUnload?.Invoke(0uL, name, asyncOperation);
			return sceneEventProgress.Status;
		}

		private void OnClientUnloadScene(uint sceneEventId)
		{
			SceneEventData sceneEventData = SceneEventDataStore[sceneEventId];
			string text = SceneNameFromHash(sceneEventData.SceneHash);
			if (!ServerSceneHandleToClientSceneHandle.ContainsKey(sceneEventData.SceneHandle))
			{
				Debug.Log("Client failed to unload scene " + text + " " + $"because we are missing the client scene handle due to the server scene handle {sceneEventData.SceneHandle} not being found.");
				EndSceneEvent(sceneEventId);
				return;
			}
			int num = ServerSceneHandleToClientSceneHandle[sceneEventData.SceneHandle];
			if (!ScenesLoaded.ContainsKey(num))
			{
				throw new Exception("Client failed to unload scene " + text + " " + $"because the client scene handle {num} was not found in ScenesLoaded!");
			}
			Scene scene = ScenesLoaded[num];
			NetworkManager networkManager = NetworkManager;
			SceneManagerHandler.MoveObjectsFromSceneToDontDestroyOnLoad(ref networkManager, scene);
			m_IsSceneEventActive = true;
			SceneEventProgress sceneEventProgress = new SceneEventProgress(NetworkManager)
			{
				SceneEventId = sceneEventData.SceneEventId,
				OnSceneEventCompleted = OnSceneUnloaded
			};
			AsyncOperation asyncOperation = SceneManagerHandler.UnloadSceneAsync(scene, sceneEventProgress);
			SceneManagerHandler.StopTrackingScene(num, text, NetworkManager);
			if (!RemoveServerClientSceneHandle(sceneEventData.SceneHandle, num))
			{
				throw new Exception($"Failed to remove server scene handle ({sceneEventData.SceneHandle}) or client scene handle({num})! Happened during scene unload for {text}.");
			}
			this.OnSceneEvent?.Invoke(new SceneEvent
			{
				AsyncOperation = asyncOperation,
				SceneEventType = sceneEventData.SceneEventType,
				LoadSceneMode = LoadSceneMode.Additive,
				SceneName = text,
				ClientId = NetworkManager.LocalClientId
			});
			this.OnUnload?.Invoke(NetworkManager.LocalClientId, text, asyncOperation);
		}

		private void OnSceneUnloaded(uint sceneEventId)
		{
			if (!NetworkManager.IsListening || NetworkManager.ShutdownInProgress)
			{
				return;
			}
			MoveObjectsFromDontDestroyOnLoadToScene(SceneManager.GetActiveScene());
			SceneEventData sceneEventData = SceneEventDataStore[sceneEventId];
			if (NetworkManager.IsServer)
			{
				SendSceneEventData(sceneEventId, NetworkManager.ConnectedClientsIds.Where((ulong c) => c != 0).ToArray());
				if (SceneEventProgressTracking.ContainsKey(sceneEventData.SceneEventProgressId) && NetworkManager.IsHost)
				{
					SceneEventProgressTracking[sceneEventData.SceneEventProgressId].ClientFinishedSceneEvent(0uL);
				}
			}
			sceneEventData.SceneEventType = SceneEventType.UnloadComplete;
			this.OnSceneEvent?.Invoke(new SceneEvent
			{
				SceneEventType = sceneEventData.SceneEventType,
				LoadSceneMode = sceneEventData.LoadSceneMode,
				SceneName = SceneNameFromHash(sceneEventData.SceneHash),
				ClientId = (NetworkManager.IsServer ? 0 : NetworkManager.LocalClientId)
			});
			this.OnUnloadComplete?.Invoke(NetworkManager.LocalClientId, SceneNameFromHash(sceneEventData.SceneHash));
			if (!NetworkManager.IsServer)
			{
				SendSceneEventData(sceneEventId, new ulong[1]);
			}
			EndSceneEvent(sceneEventId);
			m_IsSceneEventActive = false;
		}

		private void EmptySceneUnloadedOperation(uint sceneEventId)
		{
		}

		internal void UnloadAdditivelyLoadedScenes(uint sceneEventId)
		{
			_ = SceneEventDataStore[sceneEventId];
			Scene activeScene = SceneManager.GetActiveScene();
			foreach (KeyValuePair<int, Scene> item in ScenesLoaded)
			{
				if (activeScene.name != item.Value.name && item.Value.buildIndex >= 0)
				{
					SceneEventProgress sceneEventProgress = new SceneEventProgress(NetworkManager)
					{
						SceneEventId = sceneEventId,
						OnSceneEventCompleted = EmptySceneUnloadedOperation
					};
					AsyncOperation asyncOperation = SceneManagerHandler.UnloadSceneAsync(item.Value, sceneEventProgress);
					SceneUnloadEventHandler.RegisterScene(this, item.Value, LoadSceneMode.Additive, asyncOperation);
				}
			}
			ScenesLoaded.Clear();
			SceneManagerHandler.ClearSceneTracking(NetworkManager);
		}

		public SceneEventProgressStatus LoadScene(string sceneName, LoadSceneMode loadSceneMode)
		{
			SceneEventProgress sceneEventProgress = ValidateSceneEventLoading(sceneName);
			if (sceneEventProgress.Status != SceneEventProgressStatus.Started)
			{
				return sceneEventProgress.Status;
			}
			sceneEventProgress.SceneEventType = SceneEventType.LoadEventCompleted;
			sceneEventProgress.LoadSceneMode = loadSceneMode;
			SceneEventData sceneEventData = BeginSceneEvent();
			sceneEventData.SceneEventProgressId = sceneEventProgress.Guid;
			sceneEventData.SceneEventType = SceneEventType.Load;
			sceneEventData.SceneHash = SceneHashFromNameOrPath(sceneName);
			sceneEventData.LoadSceneMode = loadSceneMode;
			uint sceneEventId = sceneEventData.SceneEventId;
			m_IsSceneEventActive = ValidateSceneBeforeLoading(sceneEventData.SceneHash, loadSceneMode);
			if (!m_IsSceneEventActive)
			{
				EndSceneEvent(sceneEventId);
				return SceneEventProgressStatus.SceneFailedVerification;
			}
			if (sceneEventData.LoadSceneMode == LoadSceneMode.Single)
			{
				IsSpawnedObjectsPendingInDontDestroyOnLoad = true;
				NetworkManager.SpawnManager.ServerDestroySpawnedSceneObjects();
				MoveObjectsToDontDestroyOnLoad();
				UnloadAdditivelyLoadedScenes(sceneEventId);
				SceneUnloadEventHandler.RegisterScene(this, SceneManager.GetActiveScene(), LoadSceneMode.Single);
			}
			sceneEventProgress.SceneEventId = sceneEventId;
			sceneEventProgress.OnSceneEventCompleted = OnSceneLoaded;
			AsyncOperation asyncOperation = SceneManagerHandler.LoadSceneAsync(sceneName, loadSceneMode, sceneEventProgress);
			this.OnSceneEvent?.Invoke(new SceneEvent
			{
				AsyncOperation = asyncOperation,
				SceneEventType = sceneEventData.SceneEventType,
				LoadSceneMode = sceneEventData.LoadSceneMode,
				SceneName = sceneName,
				ClientId = 0uL
			});
			this.OnLoad?.Invoke(0uL, sceneName, sceneEventData.LoadSceneMode, asyncOperation);
			return sceneEventProgress.Status;
		}

		private void OnClientSceneLoadingEvent(uint sceneEventId)
		{
			SceneEventData sceneEventData = SceneEventDataStore[sceneEventId];
			string sceneName = SceneNameFromHash(sceneEventData.SceneHash);
			if (!ValidateSceneBeforeLoading(sceneEventData.SceneHash, sceneEventData.LoadSceneMode))
			{
				EndSceneEvent(sceneEventId);
				return;
			}
			if (sceneEventData.LoadSceneMode == LoadSceneMode.Single)
			{
				MoveObjectsToDontDestroyOnLoad();
				UnloadAdditivelyLoadedScenes(sceneEventData.SceneEventId);
			}
			if (sceneEventData.LoadSceneMode == LoadSceneMode.Single)
			{
				IsSpawnedObjectsPendingInDontDestroyOnLoad = true;
				SceneUnloadEventHandler.RegisterScene(this, SceneManager.GetActiveScene(), LoadSceneMode.Single);
			}
			SceneEventProgress sceneEventProgress = new SceneEventProgress(NetworkManager)
			{
				SceneEventId = sceneEventId,
				OnSceneEventCompleted = OnSceneLoaded
			};
			AsyncOperation asyncOperation = SceneManagerHandler.LoadSceneAsync(sceneName, sceneEventData.LoadSceneMode, sceneEventProgress);
			this.OnSceneEvent?.Invoke(new SceneEvent
			{
				AsyncOperation = asyncOperation,
				SceneEventType = sceneEventData.SceneEventType,
				LoadSceneMode = sceneEventData.LoadSceneMode,
				SceneName = sceneName,
				ClientId = NetworkManager.LocalClientId
			});
			this.OnLoad?.Invoke(NetworkManager.LocalClientId, sceneName, sceneEventData.LoadSceneMode, asyncOperation);
		}

		private void OnSceneLoaded(uint sceneEventId)
		{
			if (!NetworkManager.IsListening || NetworkManager.ShutdownInProgress)
			{
				return;
			}
			SceneEventData sceneEventData = SceneEventDataStore[sceneEventId];
			Scene andAddNewlyLoadedSceneByName = GetAndAddNewlyLoadedSceneByName(SceneNameFromHash(sceneEventData.SceneHash));
			if (!andAddNewlyLoadedSceneByName.isLoaded || !andAddNewlyLoadedSceneByName.IsValid())
			{
				throw new Exception("Failed to find valid scene internal Unity.Netcode for GameObjects error!");
			}
			if (sceneEventData.LoadSceneMode == LoadSceneMode.Single)
			{
				SceneManager.SetActiveScene(andAddNewlyLoadedSceneByName);
			}
			PopulateScenePlacedObjects(andAddNewlyLoadedSceneByName);
			if (sceneEventData.LoadSceneMode == LoadSceneMode.Single)
			{
				MoveObjectsFromDontDestroyOnLoadToScene(andAddNewlyLoadedSceneByName);
			}
			IsSpawnedObjectsPendingInDontDestroyOnLoad = false;
			if (NetworkManager.IsServer)
			{
				OnServerLoadedScene(sceneEventId, andAddNewlyLoadedSceneByName);
				return;
			}
			if (!UpdateServerClientSceneHandle(sceneEventData.SceneHandle, andAddNewlyLoadedSceneByName.handle, andAddNewlyLoadedSceneByName))
			{
				throw new Exception($"Server Scene Handle ({sceneEventData.SceneHandle}) already exist!  Happened during scene load of {andAddNewlyLoadedSceneByName.name} with Client Handle ({andAddNewlyLoadedSceneByName.handle})");
			}
			OnClientLoadedScene(sceneEventId, andAddNewlyLoadedSceneByName);
		}

		private void OnServerLoadedScene(uint sceneEventId, Scene scene)
		{
			SceneEventData sceneEventData = SceneEventDataStore[sceneEventId];
			foreach (KeyValuePair<uint, Dictionary<int, NetworkObject>> scenePlacedObject in ScenePlacedObjects)
			{
				foreach (KeyValuePair<int, NetworkObject> item in scenePlacedObject.Value)
				{
					if (!item.Value.IsPlayerObject)
					{
						NetworkManager.SpawnManager.SpawnNetworkObjectLocally(item.Value, NetworkManager.SpawnManager.GetNetworkObjectId(), sceneObject: true, playerObject: false, 0uL, destroyWithScene: true);
					}
				}
			}
			sceneEventData.AddDespawnedInSceneNetworkObjects();
			sceneEventData.SceneHandle = scene.handle;
			for (int i = 0; i < NetworkManager.ConnectedClientsList.Count; i++)
			{
				ulong clientId = NetworkManager.ConnectedClientsList[i].ClientId;
				if (clientId != 0L)
				{
					sceneEventData.TargetClientId = clientId;
					SceneEventMessage sceneEventMessage = default(SceneEventMessage);
					sceneEventMessage.EventData = sceneEventData;
					SceneEventMessage message = sceneEventMessage;
					int num = NetworkManager.ConnectionManager.SendMessage(ref message, NetworkDelivery.ReliableFragmentedSequenced, clientId);
					NetworkManager.NetworkMetrics.TrackSceneEventSent(clientId, (uint)sceneEventData.SceneEventType, scene.name, num);
				}
			}
			m_IsSceneEventActive = false;
			this.OnSceneEvent?.Invoke(new SceneEvent
			{
				SceneEventType = SceneEventType.LoadComplete,
				LoadSceneMode = sceneEventData.LoadSceneMode,
				SceneName = SceneNameFromHash(sceneEventData.SceneHash),
				ClientId = 0uL,
				Scene = scene
			});
			this.OnLoadComplete?.Invoke(0uL, SceneNameFromHash(sceneEventData.SceneHash), sceneEventData.LoadSceneMode);
			if (SceneEventProgressTracking.ContainsKey(sceneEventData.SceneEventProgressId) && NetworkManager.IsHost)
			{
				SceneEventProgressTracking[sceneEventData.SceneEventProgressId].ClientFinishedSceneEvent(0uL);
			}
			EndSceneEvent(sceneEventId);
		}

		private void OnClientLoadedScene(uint sceneEventId, Scene scene)
		{
			SceneEventData sceneEventData = SceneEventDataStore[sceneEventId];
			sceneEventData.DeserializeScenePlacedObjects();
			sceneEventData.SceneEventType = SceneEventType.LoadComplete;
			SendSceneEventData(sceneEventId, new ulong[1]);
			m_IsSceneEventActive = false;
			this.OnSceneEvent?.Invoke(new SceneEvent
			{
				SceneEventType = SceneEventType.LoadComplete,
				LoadSceneMode = sceneEventData.LoadSceneMode,
				SceneName = SceneNameFromHash(sceneEventData.SceneHash),
				ClientId = NetworkManager.LocalClientId,
				Scene = scene
			});
			this.OnLoadComplete?.Invoke(NetworkManager.LocalClientId, SceneNameFromHash(sceneEventData.SceneHash), sceneEventData.LoadSceneMode);
			EndSceneEvent(sceneEventId);
		}

		internal void SynchronizeNetworkObjects(ulong clientId)
		{
			NetworkManager.SpawnManager.UpdateObservedNetworkObjects(clientId);
			SceneEventData sceneEventData = BeginSceneEvent();
			sceneEventData.ClientSynchronizationMode = ClientSynchronizationMode;
			sceneEventData.InitializeForSynch();
			sceneEventData.TargetClientId = clientId;
			sceneEventData.LoadSceneMode = ClientSynchronizationMode;
			Scene activeScene = SceneManager.GetActiveScene();
			sceneEventData.SceneEventType = SceneEventType.Synchronize;
			if (BuildIndexToHash.ContainsKey(activeScene.buildIndex))
			{
				sceneEventData.ActiveSceneHash = BuildIndexToHash[activeScene.buildIndex];
			}
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if ((ExcludeSceneFromSychronization != null && !ExcludeSceneFromSychronization(sceneAt)) || sceneAt == DontDestroyOnLoadScene)
				{
					continue;
				}
				if (activeScene == sceneAt)
				{
					if (!ValidateSceneBeforeLoading(sceneAt.buildIndex, sceneAt.name, sceneEventData.LoadSceneMode))
					{
						continue;
					}
					sceneEventData.SceneHash = SceneHashFromNameOrPath(sceneAt.path);
					sceneEventData.SceneHandle = sceneAt.handle;
				}
				else if (!ValidateSceneBeforeLoading(sceneAt.buildIndex, sceneAt.name, LoadSceneMode.Additive))
				{
					continue;
				}
				sceneEventData.AddSceneToSynchronize(SceneHashFromNameOrPath(sceneAt.path), sceneAt.handle);
			}
			sceneEventData.AddSpawnedNetworkObjects();
			sceneEventData.AddDespawnedInSceneNetworkObjects();
			SceneEventMessage sceneEventMessage = default(SceneEventMessage);
			sceneEventMessage.EventData = sceneEventData;
			SceneEventMessage message = sceneEventMessage;
			int num = NetworkManager.ConnectionManager.SendMessage(ref message, NetworkDelivery.ReliableFragmentedSequenced, clientId);
			NetworkManager.NetworkMetrics.TrackSceneEventSent(clientId, (uint)sceneEventData.SceneEventType, "", num);
			this.OnSceneEvent?.Invoke(new SceneEvent
			{
				SceneEventType = sceneEventData.SceneEventType,
				ClientId = clientId
			});
			this.OnSynchronize?.Invoke(clientId);
			EndSceneEvent(sceneEventData.SceneEventId);
		}

		private void OnClientBeginSync(uint sceneEventId)
		{
			SceneEventData sceneEventData = SceneEventDataStore[sceneEventId];
			uint nextSceneSynchronizationHash = sceneEventData.GetNextSceneSynchronizationHash();
			int nextSceneSynchronizationHandle = sceneEventData.GetNextSceneSynchronizationHandle();
			string text = SceneNameFromHash(nextSceneSynchronizationHash);
			SceneManager.GetActiveScene();
			LoadSceneMode loadSceneMode = ((nextSceneSynchronizationHash != sceneEventData.SceneHash) ? LoadSceneMode.Additive : sceneEventData.LoadSceneMode);
			sceneEventData.NetworkSceneHandle = nextSceneSynchronizationHandle;
			sceneEventData.ClientSceneHash = nextSceneSynchronizationHash;
			if (nextSceneSynchronizationHash == sceneEventData.SceneHash)
			{
				this.OnSceneEvent?.Invoke(new SceneEvent
				{
					SceneEventType = SceneEventType.Synchronize,
					ClientId = NetworkManager.LocalClientId
				});
				this.OnSynchronize?.Invoke(NetworkManager.LocalClientId);
			}
			if (!ValidateSceneBeforeLoading(nextSceneSynchronizationHash, loadSceneMode))
			{
				HandleClientSceneEvent(sceneEventId);
				if (NetworkManager.LogLevel == LogLevel.Developer)
				{
					NetworkLog.LogInfo("Client declined to load the scene " + text + ", continuing with synchronization.");
				}
				return;
			}
			AsyncOperation asyncOperation = null;
			if (!SceneManagerHandler.ClientShouldPassThrough(text, nextSceneSynchronizationHash == sceneEventData.SceneHash, ClientSynchronizationMode, NetworkManager))
			{
				SceneEventProgress sceneEventProgress = new SceneEventProgress(NetworkManager)
				{
					SceneEventId = sceneEventId,
					OnSceneEventCompleted = ClientLoadedSynchronization
				};
				asyncOperation = SceneManagerHandler.LoadSceneAsync(text, loadSceneMode, sceneEventProgress);
				this.OnSceneEvent?.Invoke(new SceneEvent
				{
					AsyncOperation = asyncOperation,
					SceneEventType = SceneEventType.Load,
					LoadSceneMode = loadSceneMode,
					SceneName = text,
					ClientId = NetworkManager.LocalClientId
				});
				this.OnLoad?.Invoke(NetworkManager.LocalClientId, text, loadSceneMode, asyncOperation);
			}
			else
			{
				ClientLoadedSynchronization(sceneEventId);
			}
		}

		private void ClientLoadedSynchronization(uint sceneEventId)
		{
			SceneEventData sceneEventData = SceneEventDataStore[sceneEventId];
			string sceneName = SceneNameFromHash(sceneEventData.ClientSceneHash);
			Scene scene = SceneManagerHandler.GetSceneFromLoadedScenes(sceneName, NetworkManager);
			if (!scene.IsValid())
			{
				scene = GetAndAddNewlyLoadedSceneByName(sceneName);
			}
			if (!scene.isLoaded || !scene.IsValid())
			{
				throw new Exception("Failed to find valid scene internal Unity.Netcode for GameObjects error!");
			}
			LoadSceneMode loadSceneMode = ((sceneEventData.ClientSceneHash != sceneEventData.SceneHash) ? LoadSceneMode.Additive : sceneEventData.LoadSceneMode);
			if (loadSceneMode == LoadSceneMode.Single)
			{
				SceneManager.SetActiveScene(scene);
			}
			if (!UpdateServerClientSceneHandle(sceneEventData.NetworkSceneHandle, scene.handle, scene))
			{
				throw new Exception($"Server Scene Handle ({sceneEventData.SceneHandle}) already exist!  Happened during scene load of {scene.name} with Client Handle ({scene.handle})");
			}
			PopulateScenePlacedObjects(scene, clearScenePlacedObjects: false);
			SceneEventData sceneEventData2 = BeginSceneEvent();
			sceneEventData2.LoadSceneMode = loadSceneMode;
			sceneEventData2.SceneEventType = SceneEventType.LoadComplete;
			sceneEventData2.SceneHash = sceneEventData.ClientSceneHash;
			SceneEventMessage sceneEventMessage = default(SceneEventMessage);
			sceneEventMessage.EventData = sceneEventData2;
			SceneEventMessage message = sceneEventMessage;
			int num = NetworkManager.ConnectionManager.SendMessage(ref message, NetworkDelivery.ReliableFragmentedSequenced, 0uL);
			NetworkManager.NetworkMetrics.TrackSceneEventSent(0uL, (uint)sceneEventData2.SceneEventType, sceneName, num);
			EndSceneEvent(sceneEventData2.SceneEventId);
			this.OnSceneEvent?.Invoke(new SceneEvent
			{
				SceneEventType = SceneEventType.LoadComplete,
				LoadSceneMode = loadSceneMode,
				SceneName = sceneName,
				Scene = scene,
				ClientId = NetworkManager.LocalClientId
			});
			this.OnLoadComplete?.Invoke(NetworkManager.LocalClientId, sceneName, loadSceneMode);
			HandleClientSceneEvent(sceneEventId);
		}

		private void SynchronizeNetworkObjectScene()
		{
			foreach (NetworkObject spawnedObjects in NetworkManager.SpawnManager.SpawnedObjectsList)
			{
				if (spawnedObjects.IsSceneObject.Value || !ServerSceneHandleToClientSceneHandle.ContainsKey(spawnedObjects.NetworkSceneHandle))
				{
					continue;
				}
				spawnedObjects.SceneOriginHandle = ServerSceneHandleToClientSceneHandle[spawnedObjects.NetworkSceneHandle];
				if (spawnedObjects.gameObject.scene.handle == spawnedObjects.SceneOriginHandle || !(spawnedObjects.transform.parent == null))
				{
					continue;
				}
				if (ScenesLoaded.ContainsKey(spawnedObjects.SceneOriginHandle))
				{
					Scene scene = ScenesLoaded[spawnedObjects.SceneOriginHandle];
					if (scene == DontDestroyOnLoadScene)
					{
						Debug.Log(spawnedObjects.gameObject.name + " migrating into DDOL!");
					}
					SceneManager.MoveGameObjectToScene(spawnedObjects.gameObject, scene);
				}
				else if (NetworkManager.LogLevel <= LogLevel.Normal)
				{
					NetworkLog.LogWarningServer($"[Client-{NetworkManager.LocalClientId}][{spawnedObjects.gameObject.name}] Server - " + $"client scene mismatch detected! Client-side has no scene loaded with handle ({spawnedObjects.SceneOriginHandle})!");
				}
			}
		}

		private void HandleClientSceneEvent(uint sceneEventId)
		{
			SceneEventData sceneEventData = SceneEventDataStore[sceneEventId];
			switch (sceneEventData.SceneEventType)
			{
			case SceneEventType.ActiveSceneChanged:
				if (HashToBuildIndex.ContainsKey(sceneEventData.ActiveSceneHash))
				{
					Scene sceneByBuildIndex2 = SceneManager.GetSceneByBuildIndex(HashToBuildIndex[sceneEventData.ActiveSceneHash]);
					if (sceneByBuildIndex2.isLoaded)
					{
						SceneManager.SetActiveScene(sceneByBuildIndex2);
					}
				}
				break;
			case SceneEventType.ObjectSceneChanged:
				MigrateNetworkObjectsIntoScenes();
				break;
			case SceneEventType.Load:
				OnClientSceneLoadingEvent(sceneEventId);
				break;
			case SceneEventType.Unload:
				OnClientUnloadScene(sceneEventId);
				break;
			case SceneEventType.Synchronize:
				if (!sceneEventData.IsDoneWithSynchronization())
				{
					OnClientBeginSync(sceneEventId);
					break;
				}
				PopulateScenePlacedObjects(DontDestroyOnLoadScene, clearScenePlacedObjects: false);
				if (HashToBuildIndex.ContainsKey(sceneEventData.ActiveSceneHash))
				{
					Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(HashToBuildIndex[sceneEventData.ActiveSceneHash]);
					if (sceneByBuildIndex.isLoaded && sceneByBuildIndex.handle != SceneManager.GetActiveScene().handle)
					{
						SceneManager.SetActiveScene(sceneByBuildIndex);
					}
				}
				sceneEventData.SynchronizeSceneNetworkObjects(NetworkManager);
				SynchronizeNetworkObjectScene();
				sceneEventData.SceneEventType = SceneEventType.SynchronizeComplete;
				SendSceneEventData(sceneEventId, new ulong[1]);
				NetworkManager.IsConnectedClient = true;
				NetworkManager.ConnectionManager.InvokeOnClientConnectedCallback(NetworkManager.LocalClientId);
				this.OnSceneEvent?.Invoke(new SceneEvent
				{
					SceneEventType = sceneEventData.SceneEventType,
					ClientId = NetworkManager.LocalClientId
				});
				sceneEventData.ProcessDeferredObjectSceneChangedEvents();
				if (PostSynchronizationSceneUnloading && ClientSynchronizationMode == LoadSceneMode.Additive)
				{
					SceneManagerHandler.UnloadUnassignedScenes(NetworkManager);
				}
				this.OnSynchronizeComplete?.Invoke(NetworkManager.LocalClientId);
				EndSceneEvent(sceneEventId);
				break;
			case SceneEventType.ReSynchronize:
				this.OnSceneEvent?.Invoke(new SceneEvent
				{
					SceneEventType = sceneEventData.SceneEventType,
					ClientId = 0uL
				});
				EndSceneEvent(sceneEventId);
				break;
			case SceneEventType.LoadEventCompleted:
			case SceneEventType.UnloadEventCompleted:
				this.OnSceneEvent?.Invoke(new SceneEvent
				{
					SceneEventType = sceneEventData.SceneEventType,
					LoadSceneMode = sceneEventData.LoadSceneMode,
					SceneName = SceneNameFromHash(sceneEventData.SceneHash),
					ClientId = 0uL,
					ClientsThatCompleted = sceneEventData.ClientsCompleted,
					ClientsThatTimedOut = sceneEventData.ClientsTimedOut
				});
				if (sceneEventData.SceneEventType == SceneEventType.LoadEventCompleted)
				{
					this.OnLoadEventCompleted?.Invoke(SceneNameFromHash(sceneEventData.SceneHash), sceneEventData.LoadSceneMode, sceneEventData.ClientsCompleted, sceneEventData.ClientsTimedOut);
				}
				else
				{
					this.OnUnloadEventCompleted?.Invoke(SceneNameFromHash(sceneEventData.SceneHash), sceneEventData.LoadSceneMode, sceneEventData.ClientsCompleted, sceneEventData.ClientsTimedOut);
				}
				EndSceneEvent(sceneEventId);
				break;
			default:
				Debug.LogWarning($"{sceneEventData.SceneEventType} is not currently supported!");
				break;
			}
		}

		private void HandleServerSceneEvent(uint sceneEventId, ulong clientId)
		{
			SceneEventData sceneEventData = SceneEventDataStore[sceneEventId];
			switch (sceneEventData.SceneEventType)
			{
			case SceneEventType.LoadComplete:
				this.OnSceneEvent?.Invoke(new SceneEvent
				{
					SceneEventType = sceneEventData.SceneEventType,
					LoadSceneMode = sceneEventData.LoadSceneMode,
					SceneName = SceneNameFromHash(sceneEventData.SceneHash),
					ClientId = clientId
				});
				this.OnLoadComplete?.Invoke(clientId, SceneNameFromHash(sceneEventData.SceneHash), sceneEventData.LoadSceneMode);
				if (SceneEventProgressTracking.ContainsKey(sceneEventData.SceneEventProgressId))
				{
					SceneEventProgressTracking[sceneEventData.SceneEventProgressId].ClientFinishedSceneEvent(clientId);
				}
				EndSceneEvent(sceneEventId);
				break;
			case SceneEventType.UnloadComplete:
				if (SceneEventProgressTracking.ContainsKey(sceneEventData.SceneEventProgressId))
				{
					SceneEventProgressTracking[sceneEventData.SceneEventProgressId].ClientFinishedSceneEvent(clientId);
				}
				this.OnSceneEvent?.Invoke(new SceneEvent
				{
					SceneEventType = sceneEventData.SceneEventType,
					LoadSceneMode = sceneEventData.LoadSceneMode,
					SceneName = SceneNameFromHash(sceneEventData.SceneHash),
					ClientId = clientId
				});
				this.OnUnloadComplete?.Invoke(clientId, SceneNameFromHash(sceneEventData.SceneHash));
				EndSceneEvent(sceneEventId);
				break;
			case SceneEventType.SynchronizeComplete:
				this.OnSceneEvent?.Invoke(new SceneEvent
				{
					SceneEventType = sceneEventData.SceneEventType,
					SceneName = string.Empty,
					ClientId = clientId
				});
				NetworkManager.ConnectedClients[clientId].IsConnected = true;
				this.OnSynchronizeComplete?.Invoke(clientId);
				NetworkManager.ConnectionManager.InvokeOnClientConnectedCallback(clientId);
				if (sceneEventData.ClientNeedsReSynchronization() && !DisableReSynchronization && NetworkManager.ConnectedClients.ContainsKey(clientId))
				{
					sceneEventData.SceneEventType = SceneEventType.ReSynchronize;
					SendSceneEventData(sceneEventId, new ulong[1] { clientId });
					this.OnSceneEvent?.Invoke(new SceneEvent
					{
						SceneEventType = sceneEventData.SceneEventType,
						SceneName = string.Empty,
						ClientId = clientId
					});
				}
				EndSceneEvent(sceneEventId);
				break;
			default:
				Debug.LogWarning($"{sceneEventData.SceneEventType} is not currently supported!");
				break;
			}
		}

		internal void HandleSceneEvent(ulong clientId, FastBufferReader reader)
		{
			if (NetworkManager != null)
			{
				SceneEventData sceneEventData = BeginSceneEvent();
				sceneEventData.Deserialize(reader);
				NetworkManager.NetworkMetrics.TrackSceneEventReceived(clientId, (uint)sceneEventData.SceneEventType, SceneNameFromHash(sceneEventData.SceneHash), reader.Length);
				if (sceneEventData.IsSceneEventClientSide())
				{
					if (sceneEventData.SceneEventType == SceneEventType.Synchronize)
					{
						ScenePlacedObjects.Clear();
						ClientSynchronizationMode = sceneEventData.ClientSynchronizationMode;
						if (ClientSynchronizationMode == LoadSceneMode.Additive)
						{
							SceneManagerHandler.PopulateLoadedScenes(ref ScenesLoaded, NetworkManager);
						}
					}
					HandleClientSceneEvent(sceneEventData.SceneEventId);
				}
				else
				{
					HandleServerSceneEvent(sceneEventData.SceneEventId, clientId);
				}
			}
			else
			{
				Debug.LogError("HandleSceneEvent was invoked but NetworkManager reference was null!");
			}
		}

		internal void MoveObjectsToDontDestroyOnLoad()
		{
			foreach (NetworkObject item in new HashSet<NetworkObject>(NetworkManager.SpawnManager.SpawnedObjectsList))
			{
				if (item == null || (item != null && item.gameObject.scene == DontDestroyOnLoadScene))
				{
					continue;
				}
				if (!item.DestroyWithScene)
				{
					if (item.gameObject.transform.parent == null && item.IsSceneObject.HasValue && !item.IsSceneObject.Value)
					{
						UnityEngine.Object.DontDestroyOnLoad(item.gameObject);
					}
				}
				else if (NetworkManager.IsServer)
				{
					item.Despawn();
				}
			}
		}

		internal void PopulateScenePlacedObjects(Scene sceneToFilterBy, bool clearScenePlacedObjects = true)
		{
			if (clearScenePlacedObjects)
			{
				ScenePlacedObjects.Clear();
			}
			NetworkObject[] array = UnityEngine.Object.FindObjectsOfType<NetworkObject>();
			foreach (NetworkObject networkObject in array)
			{
				uint globalObjectIdHash = networkObject.GlobalObjectIdHash;
				int handle = networkObject.gameObject.scene.handle;
				if (networkObject.IsSceneObject != false && (networkObject.NetworkManager == NetworkManager || networkObject.NetworkManagerOwner == null) && handle == sceneToFilterBy.handle)
				{
					if (!ScenePlacedObjects.ContainsKey(globalObjectIdHash))
					{
						ScenePlacedObjects.Add(globalObjectIdHash, new Dictionary<int, NetworkObject>());
					}
					if (ScenePlacedObjects[globalObjectIdHash].ContainsKey(handle))
					{
						string arg = ((ScenePlacedObjects[globalObjectIdHash][handle] != null) ? ScenePlacedObjects[globalObjectIdHash][handle].name : "Null Entry");
						throw new Exception(networkObject.name + " tried to registered with ScenePlacedObjects which already contains " + string.Format("the same {0} value {1} for {2}!", "GlobalObjectIdHash", globalObjectIdHash, arg));
					}
					ScenePlacedObjects[globalObjectIdHash].Add(handle, networkObject);
				}
			}
		}

		internal void MoveObjectsFromDontDestroyOnLoadToScene(Scene scene)
		{
			foreach (NetworkObject spawnedObjects in NetworkManager.SpawnManager.SpawnedObjectsList)
			{
				if (!(spawnedObjects == null) && spawnedObjects.gameObject.scene == DontDestroyOnLoadScene && !spawnedObjects.DestroyWithScene && spawnedObjects.gameObject.transform.parent == null && spawnedObjects.IsSceneObject.HasValue && !spawnedObjects.IsSceneObject.Value)
				{
					SceneManager.MoveGameObjectToScene(spawnedObjects.gameObject, scene);
				}
			}
		}

		internal void NotifyNetworkObjectSceneChanged(NetworkObject networkObject)
		{
			if (!NetworkManager.IsServer)
			{
				if (NetworkManager.LogLevel == LogLevel.Developer)
				{
					NetworkLog.LogErrorServer("[Please Report This Error][NotifyNetworkObjectSceneChanged] A client is trying to notify of an object's scene change!");
				}
			}
			else if (networkObject.IsSceneObject != false)
			{
				if (NetworkManager.LogLevel == LogLevel.Developer)
				{
					NetworkLog.LogErrorServer("[Please Report This Error][NotifyNetworkObjectSceneChanged] Trying to notify in-scene placed object scene change!");
				}
			}
			else
			{
				if (networkObject.gameObject.scene == SceneManager.GetActiveScene() && networkObject.ActiveSceneSynchronization)
				{
					return;
				}
				foreach (KeyValuePair<Guid, SceneEventProgress> item in SceneEventProgressTracking)
				{
					if (!item.Value.HasTimedOut() && item.Value.Status == SceneEventProgressStatus.Started)
					{
						return;
					}
				}
				if (!ObjectsMigratedIntoNewScene.ContainsKey(networkObject.gameObject.scene.handle))
				{
					ObjectsMigratedIntoNewScene.Add(networkObject.gameObject.scene.handle, new List<NetworkObject>());
				}
				ObjectsMigratedIntoNewScene[networkObject.gameObject.scene.handle].Add(networkObject);
			}
		}

		internal void MigrateNetworkObjectsIntoScenes()
		{
			try
			{
				foreach (KeyValuePair<int, List<NetworkObject>> item in ObjectsMigratedIntoNewScene)
				{
					if (!ServerSceneHandleToClientSceneHandle.ContainsKey(item.Key))
					{
						continue;
					}
					int key = ServerSceneHandleToClientSceneHandle[item.Key];
					if (!ScenesLoaded.ContainsKey(ServerSceneHandleToClientSceneHandle[item.Key]))
					{
						continue;
					}
					Scene scene = ScenesLoaded[key];
					foreach (NetworkObject item2 in item.Value)
					{
						SceneManager.MoveGameObjectToScene(item2.gameObject, scene);
					}
				}
			}
			catch (Exception ex)
			{
				NetworkLog.LogErrorServer(ex.Message + "\n Stack Trace:\n " + ex.StackTrace);
			}
			ObjectsMigratedIntoNewScene.Clear();
		}

		internal void CheckForAndSendNetworkObjectSceneChanged()
		{
			if (!NetworkManager.IsServer || ObjectsMigratedIntoNewScene.Count == 0)
			{
				return;
			}
			m_ScenesToRemoveFromObjectMigration.Clear();
			foreach (KeyValuePair<int, List<NetworkObject>> item in ObjectsMigratedIntoNewScene)
			{
				for (int num = item.Value.Count - 1; num >= 0; num--)
				{
					if (!item.Value[num].IsSpawned)
					{
						item.Value.RemoveAt(num);
					}
				}
				if (item.Value.Count == 0)
				{
					m_ScenesToRemoveFromObjectMigration.Add(item.Key);
				}
			}
			foreach (int item2 in m_ScenesToRemoveFromObjectMigration)
			{
				ObjectsMigratedIntoNewScene.Remove(item2);
			}
			if (ObjectsMigratedIntoNewScene.Count != 0)
			{
				SceneEventData sceneEventData = BeginSceneEvent();
				sceneEventData.SceneEventType = SceneEventType.ObjectSceneChanged;
				SendSceneEventData(sceneEventData.SceneEventId, NetworkManager.ConnectedClientsIds.Where((ulong c) => c != 0).ToArray());
				EndSceneEvent(sceneEventData.SceneEventId);
			}
		}
	}
}
