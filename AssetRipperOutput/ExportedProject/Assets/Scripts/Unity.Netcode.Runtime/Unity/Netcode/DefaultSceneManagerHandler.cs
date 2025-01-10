using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.Netcode
{
	internal class DefaultSceneManagerHandler : ISceneManagerHandler
	{
		internal struct SceneEntry
		{
			public bool IsAssigned;

			public Scene Scene;
		}

		private Scene m_InvalidScene;

		internal Dictionary<string, Dictionary<int, SceneEntry>> SceneNameToSceneHandles = new Dictionary<string, Dictionary<int, SceneEntry>>();

		private List<Scene> m_ScenesToUnload = new List<Scene>();

		public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, SceneEventProgress sceneEventProgress)
		{
			AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
			sceneEventProgress.SetAsyncOperation(asyncOperation);
			return asyncOperation;
		}

		public AsyncOperation UnloadSceneAsync(Scene scene, SceneEventProgress sceneEventProgress)
		{
			AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scene);
			sceneEventProgress.SetAsyncOperation(asyncOperation);
			return asyncOperation;
		}

		public void ClearSceneTracking(NetworkManager networkManager)
		{
			SceneNameToSceneHandles.Clear();
		}

		public void StopTrackingScene(int handle, string name, NetworkManager networkManager)
		{
			if (SceneNameToSceneHandles.ContainsKey(name) && SceneNameToSceneHandles[name].ContainsKey(handle))
			{
				SceneNameToSceneHandles[name].Remove(handle);
				if (SceneNameToSceneHandles[name].Count == 0)
				{
					SceneNameToSceneHandles.Remove(name);
				}
			}
		}

		public void StartTrackingScene(Scene scene, bool assigned, NetworkManager networkManager)
		{
			if (!SceneNameToSceneHandles.ContainsKey(scene.name))
			{
				SceneNameToSceneHandles.Add(scene.name, new Dictionary<int, SceneEntry>());
			}
			if (!SceneNameToSceneHandles[scene.name].ContainsKey(scene.handle))
			{
				SceneEntry sceneEntry = default(SceneEntry);
				sceneEntry.IsAssigned = true;
				sceneEntry.Scene = scene;
				SceneEntry value = sceneEntry;
				SceneNameToSceneHandles[scene.name].Add(scene.handle, value);
				return;
			}
			throw new Exception($"[Duplicate Handle] Scene {scene.name} already has scene handle {scene.handle} registered!");
		}

		public bool DoesSceneHaveUnassignedEntry(string sceneName, NetworkManager networkManager)
		{
			List<Scene> list = new List<Scene>();
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.name == sceneName)
				{
					list.Add(sceneAt);
				}
			}
			if (list.Count == 0)
			{
				return false;
			}
			if (list.Count > 0 && !SceneNameToSceneHandles.ContainsKey(sceneName))
			{
				return true;
			}
			foreach (Scene item in list)
			{
				if (!SceneNameToSceneHandles[item.name].ContainsKey(item.handle))
				{
					return true;
				}
				if (!SceneNameToSceneHandles[item.name][item.handle].IsAssigned)
				{
					return true;
				}
			}
			return false;
		}

		public Scene GetSceneFromLoadedScenes(string sceneName, NetworkManager networkManager)
		{
			if (SceneNameToSceneHandles.ContainsKey(sceneName))
			{
				foreach (KeyValuePair<int, SceneEntry> item in SceneNameToSceneHandles[sceneName])
				{
					if (!item.Value.IsAssigned)
					{
						SceneEntry value = item.Value;
						value.IsAssigned = true;
						SceneNameToSceneHandles[sceneName][item.Key] = value;
						return value.Scene;
					}
				}
			}
			return m_InvalidScene;
		}

		public void PopulateLoadedScenes(ref Dictionary<int, Scene> scenesLoaded, NetworkManager networkManager)
		{
			SceneNameToSceneHandles.Clear();
			int sceneCount = SceneManager.sceneCount;
			for (int i = 0; i < sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (!SceneNameToSceneHandles.ContainsKey(sceneAt.name))
				{
					SceneNameToSceneHandles.Add(sceneAt.name, new Dictionary<int, SceneEntry>());
				}
				if (!SceneNameToSceneHandles[sceneAt.name].ContainsKey(sceneAt.handle))
				{
					SceneEntry sceneEntry = default(SceneEntry);
					sceneEntry.IsAssigned = false;
					sceneEntry.Scene = sceneAt;
					SceneEntry value = sceneEntry;
					SceneNameToSceneHandles[sceneAt.name].Add(sceneAt.handle, value);
					if (!scenesLoaded.ContainsKey(sceneAt.handle))
					{
						scenesLoaded.Add(sceneAt.handle, sceneAt);
					}
					continue;
				}
				throw new Exception($"[Duplicate Handle] Scene {sceneAt.name} already has scene handle {sceneAt.handle} registered!");
			}
		}

		public void UnloadUnassignedScenes(NetworkManager networkManager = null)
		{
			NetworkSceneManager sceneManager = networkManager.SceneManager;
			SceneManager.sceneUnloaded += SceneManager_SceneUnloaded;
			foreach (KeyValuePair<string, Dictionary<int, SceneEntry>> sceneNameToSceneHandle in SceneNameToSceneHandles)
			{
				foreach (KeyValuePair<int, SceneEntry> item in SceneNameToSceneHandles[sceneNameToSceneHandle.Key])
				{
					if (!item.Value.IsAssigned && (sceneManager.VerifySceneBeforeUnloading == null || sceneManager.VerifySceneBeforeUnloading(item.Value.Scene)))
					{
						m_ScenesToUnload.Add(item.Value.Scene);
					}
				}
			}
			foreach (Scene item2 in m_ScenesToUnload)
			{
				SceneManager.UnloadSceneAsync(item2);
			}
		}

		private void SceneManager_SceneUnloaded(Scene scene)
		{
			if (SceneNameToSceneHandles.ContainsKey(scene.name))
			{
				if (SceneNameToSceneHandles[scene.name].ContainsKey(scene.handle))
				{
					SceneNameToSceneHandles[scene.name].Remove(scene.handle);
				}
				if (SceneNameToSceneHandles[scene.name].Count == 0)
				{
					SceneNameToSceneHandles.Remove(scene.name);
				}
				m_ScenesToUnload.Remove(scene);
				if (m_ScenesToUnload.Count == 0)
				{
					SceneManager.sceneUnloaded -= SceneManager_SceneUnloaded;
				}
			}
		}

		public bool ClientShouldPassThrough(string sceneName, bool isPrimaryScene, LoadSceneMode clientSynchronizationMode, NetworkManager networkManager)
		{
			bool flag = clientSynchronizationMode != 0 && DoesSceneHaveUnassignedEntry(sceneName, networkManager);
			Scene activeScene = SceneManager.GetActiveScene();
			if (!flag && sceneName == activeScene.name && (clientSynchronizationMode == LoadSceneMode.Additive || (isPrimaryScene && clientSynchronizationMode == LoadSceneMode.Single)))
			{
				flag = true;
			}
			return flag;
		}

		public void MoveObjectsFromSceneToDontDestroyOnLoad(ref NetworkManager networkManager, Scene scene)
		{
			_ = scene == SceneManager.GetActiveScene();
			foreach (NetworkObject item in new HashSet<NetworkObject>(networkManager.SpawnManager.SpawnedObjectsList))
			{
				if (item == null || (item != null && item.gameObject.scene.handle != scene.handle))
				{
					continue;
				}
				if (!item.DestroyWithScene && item.gameObject.scene != networkManager.SceneManager.DontDestroyOnLoadScene)
				{
					if (item.gameObject.transform.parent == null && item.IsSceneObject.HasValue && !item.IsSceneObject.Value)
					{
						UnityEngine.Object.DontDestroyOnLoad(item.gameObject);
					}
				}
				else if (networkManager.IsServer)
				{
					item.Despawn();
				}
				else
				{
					UnityEngine.Object.DontDestroyOnLoad(item.gameObject);
				}
			}
		}

		public void SetClientSynchronizationMode(ref NetworkManager networkManager, LoadSceneMode mode)
		{
			NetworkSceneManager sceneManager = networkManager.SceneManager;
			if (!networkManager.IsServer)
			{
				if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
				{
					NetworkLog.LogWarning("Clients should not set this value as it is automatically synchronized with the server's setting!");
				}
				return;
			}
			if (networkManager.ConnectedClientsIds.Count > (networkManager.IsHost ? 1 : 0) && sceneManager.ClientSynchronizationMode != mode && NetworkLog.CurrentLogLevel <= LogLevel.Normal)
			{
				NetworkLog.LogWarning("Server is changing client synchronization mode after clients have been synchronized! It is recommended to do this before clients are connected!");
			}
			if (mode == LoadSceneMode.Additive)
			{
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if ((sceneManager.VerifySceneBeforeLoading == null || sceneManager.VerifySceneBeforeLoading(sceneAt.buildIndex, sceneAt.name, LoadSceneMode.Additive)) && !sceneManager.ScenesLoaded.ContainsKey(sceneAt.handle))
					{
						sceneManager.ScenesLoaded.Add(sceneAt.handle, sceneAt);
					}
				}
			}
			sceneManager.ClientSynchronizationMode = mode;
		}
	}
}
