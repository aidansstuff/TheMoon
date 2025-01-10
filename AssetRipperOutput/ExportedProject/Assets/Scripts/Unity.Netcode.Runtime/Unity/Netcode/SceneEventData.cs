using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.Netcode
{
	internal class SceneEventData : IDisposable
	{
		internal SceneEventType SceneEventType;

		internal LoadSceneMode LoadSceneMode;

		internal ForceNetworkSerializeByMemcpy<Guid> SceneEventProgressId;

		internal uint SceneEventId;

		internal uint ActiveSceneHash;

		internal uint SceneHash;

		internal int SceneHandle;

		internal uint ClientSceneHash;

		internal int NetworkSceneHandle;

		internal ulong TargetClientId;

		private Dictionary<uint, List<NetworkObject>> m_SceneNetworkObjects;

		private Dictionary<uint, long> m_SceneNetworkObjectDataOffsets;

		private List<NetworkObject> m_NetworkObjectsSync = new List<NetworkObject>();

		private List<NetworkObject> m_DespawnedInSceneObjectsSync = new List<NetworkObject>();

		private Dictionary<int, List<uint>> m_DespawnedInSceneObjects = new Dictionary<int, List<uint>>();

		private List<ulong> m_NetworkObjectsToBeRemoved = new List<ulong>();

		private bool m_HasInternalBuffer;

		internal FastBufferReader InternalBuffer;

		private NetworkManager m_NetworkManager;

		internal List<ulong> ClientsCompleted;

		internal List<ulong> ClientsTimedOut;

		internal Queue<uint> ScenesToSynchronize;

		internal Queue<uint> SceneHandlesToSynchronize;

		internal LoadSceneMode ClientSynchronizationMode;

		internal void AddSceneToSynchronize(uint sceneHash, int sceneHandle)
		{
			ScenesToSynchronize.Enqueue(sceneHash);
			SceneHandlesToSynchronize.Enqueue((uint)sceneHandle);
		}

		internal uint GetNextSceneSynchronizationHash()
		{
			return ScenesToSynchronize.Dequeue();
		}

		internal int GetNextSceneSynchronizationHandle()
		{
			return (int)SceneHandlesToSynchronize.Dequeue();
		}

		internal bool IsDoneWithSynchronization()
		{
			if (ScenesToSynchronize.Count == 0 && SceneHandlesToSynchronize.Count == 0)
			{
				return true;
			}
			if (ScenesToSynchronize.Count != SceneHandlesToSynchronize.Count)
			{
				throw new Exception("[SceneEventData-Internal Mismatch Error] ScenesToSynchronize count != SceneHandlesToSynchronize count!");
			}
			return false;
		}

		internal void InitializeForSynch()
		{
			if (m_SceneNetworkObjects == null)
			{
				m_SceneNetworkObjects = new Dictionary<uint, List<NetworkObject>>();
			}
			else
			{
				m_SceneNetworkObjects.Clear();
			}
			if (ScenesToSynchronize == null)
			{
				ScenesToSynchronize = new Queue<uint>();
			}
			else
			{
				ScenesToSynchronize.Clear();
			}
			if (SceneHandlesToSynchronize == null)
			{
				SceneHandlesToSynchronize = new Queue<uint>();
			}
			else
			{
				SceneHandlesToSynchronize.Clear();
			}
		}

		internal void AddSpawnedNetworkObjects()
		{
			m_NetworkObjectsSync.Clear();
			foreach (NetworkObject spawnedObjects in m_NetworkManager.SpawnManager.SpawnedObjectsList)
			{
				if (spawnedObjects.Observers.Contains(TargetClientId))
				{
					m_NetworkObjectsSync.Add(spawnedObjects);
				}
			}
			m_NetworkObjectsSync.Sort(SortParentedNetworkObjects);
			m_NetworkObjectsSync.Sort(SortNetworkObjects);
			if (m_NetworkManager.LogLevel != 0)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(65535);
			stringBuilder.Append("[Server-Side Client-Synchronization] NetworkObject serialization order:");
			foreach (NetworkObject item in m_NetworkObjectsSync)
			{
				stringBuilder.Append(item.name ?? "");
			}
			NetworkLog.LogInfo(stringBuilder.ToString());
		}

		internal void AddDespawnedInSceneNetworkObjects()
		{
			m_DespawnedInSceneObjectsSync.Clear();
			foreach (NetworkObject item in from c in UnityEngine.Object.FindObjectsOfType<NetworkObject>(includeInactive: true)
				where c.NetworkManager == m_NetworkManager
				select c)
			{
				if (item.IsSceneObject.HasValue && item.IsSceneObject.Value && !item.IsSpawned)
				{
					m_DespawnedInSceneObjectsSync.Add(item);
				}
			}
		}

		internal void AddNetworkObjectForSynch(uint sceneIndex, NetworkObject networkObject)
		{
			if (!m_SceneNetworkObjects.ContainsKey(sceneIndex))
			{
				m_SceneNetworkObjects.Add(sceneIndex, new List<NetworkObject>());
			}
			m_SceneNetworkObjects[sceneIndex].Add(networkObject);
		}

		internal bool IsSceneEventClientSide()
		{
			SceneEventType sceneEventType = SceneEventType;
			if (sceneEventType <= SceneEventType.UnloadEventCompleted || sceneEventType - 9 <= SceneEventType.Unload)
			{
				return true;
			}
			return false;
		}

		private int SortNetworkObjects(NetworkObject first, NetworkObject second)
		{
			bool flag = m_NetworkManager.PrefabHandler.ContainsHandler(first);
			bool flag2 = m_NetworkManager.PrefabHandler.ContainsHandler(second);
			if (flag != flag2)
			{
				if (flag)
				{
					return 1;
				}
				return -1;
			}
			return 0;
		}

		private int SortParentedNetworkObjects(NetworkObject first, NetworkObject second)
		{
			if (first.transform.parent != null)
			{
				return 1;
			}
			if (second.transform.parent != null)
			{
				return -1;
			}
			return 0;
		}

		internal void Serialize(FastBufferWriter writer)
		{
			writer.WriteValueSafe(in SceneEventType, default(FastBufferWriter.ForEnums));
			if (SceneEventType == SceneEventType.ActiveSceneChanged)
			{
				writer.WriteValueSafe(in ActiveSceneHash, default(FastBufferWriter.ForPrimitives));
				return;
			}
			if (SceneEventType == SceneEventType.ObjectSceneChanged)
			{
				SerializeObjectsMovedIntoNewScene(writer);
				return;
			}
			byte value = (byte)LoadSceneMode;
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			if (SceneEventType != SceneEventType.Synchronize)
			{
				writer.WriteValueSafe(in SceneEventProgressId, default(FastBufferWriter.ForStructs));
			}
			else
			{
				writer.WriteValueSafe(in ClientSynchronizationMode, default(FastBufferWriter.ForEnums));
			}
			writer.WriteValueSafe(in SceneHash, default(FastBufferWriter.ForPrimitives));
			writer.WriteValueSafe(in SceneHandle, default(FastBufferWriter.ForPrimitives));
			switch (SceneEventType)
			{
			case SceneEventType.Synchronize:
				writer.WriteValueSafe(in ActiveSceneHash, default(FastBufferWriter.ForPrimitives));
				WriteSceneSynchronizationData(writer);
				break;
			case SceneEventType.Load:
				SerializeScenePlacedObjects(writer);
				break;
			case SceneEventType.SynchronizeComplete:
				WriteClientSynchronizationResults(writer);
				break;
			case SceneEventType.ReSynchronize:
				WriteClientReSynchronizationData(writer);
				break;
			case SceneEventType.LoadEventCompleted:
			case SceneEventType.UnloadEventCompleted:
				WriteSceneEventProgressDone(writer);
				break;
			case SceneEventType.Unload:
			case SceneEventType.LoadComplete:
			case SceneEventType.UnloadComplete:
				break;
			}
		}

		internal void WriteSceneSynchronizationData(FastBufferWriter writer)
		{
			writer.WriteValueSafe(ScenesToSynchronize.ToArray(), default(FastBufferWriter.ForPrimitives));
			writer.WriteValueSafe(SceneHandlesToSynchronize.ToArray(), default(FastBufferWriter.ForPrimitives));
			int position = writer.Position;
			int value = 0;
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			int num = 0;
			value = m_NetworkObjectsSync.Count;
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			for (int i = 0; i < m_NetworkObjectsSync.Count; i++)
			{
				int position2 = writer.Position;
				m_NetworkObjectsSync[i].GetMessageSceneObject(TargetClientId).Serialize(writer);
				int position3 = writer.Position;
				num += position3 - position2;
			}
			value = m_DespawnedInSceneObjectsSync.Count;
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			for (int j = 0; j < m_DespawnedInSceneObjectsSync.Count; j++)
			{
				int position4 = writer.Position;
				value = m_DespawnedInSceneObjectsSync[j].GetSceneOriginHandle();
				writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
				writer.WriteValueSafe(in m_DespawnedInSceneObjectsSync[j].GlobalObjectIdHash, default(FastBufferWriter.ForPrimitives));
				int position5 = writer.Position;
				num += position5 - position4;
			}
			int position6 = writer.Position;
			uint value2 = (uint)(position6 - (position + 4));
			writer.Seek(position);
			writer.WriteValueSafe(in value2, default(FastBufferWriter.ForPrimitives));
			writer.Seek(position6);
		}

		internal void SerializeScenePlacedObjects(FastBufferWriter writer)
		{
			ushort value = 0;
			int position = writer.Position;
			ushort value2 = 0;
			writer.WriteValueSafe(in value2, default(FastBufferWriter.ForPrimitives));
			foreach (KeyValuePair<uint, Dictionary<int, NetworkObject>> scenePlacedObject in m_NetworkManager.SceneManager.ScenePlacedObjects)
			{
				foreach (KeyValuePair<int, NetworkObject> item in scenePlacedObject.Value)
				{
					if (item.Value.Observers.Contains(TargetClientId))
					{
						item.Value.GetMessageSceneObject(TargetClientId).Serialize(writer);
						value++;
					}
				}
			}
			int value3 = m_DespawnedInSceneObjectsSync.Count;
			writer.WriteValueSafe(in value3, default(FastBufferWriter.ForPrimitives));
			for (int i = 0; i < m_DespawnedInSceneObjectsSync.Count; i++)
			{
				value3 = m_DespawnedInSceneObjectsSync[i].GetSceneOriginHandle();
				writer.WriteValueSafe(in value3, default(FastBufferWriter.ForPrimitives));
				writer.WriteValueSafe(in m_DespawnedInSceneObjectsSync[i].GlobalObjectIdHash, default(FastBufferWriter.ForPrimitives));
			}
			int position2 = writer.Position;
			writer.Seek(position);
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			writer.Seek(position2);
		}

		internal unsafe void Deserialize(FastBufferReader reader)
		{
			reader.ReadValueSafe(out SceneEventType, default(FastBufferWriter.ForEnums));
			if (SceneEventType == SceneEventType.ActiveSceneChanged)
			{
				reader.ReadValueSafe(out ActiveSceneHash, default(FastBufferWriter.ForPrimitives));
				return;
			}
			if (SceneEventType == SceneEventType.ObjectSceneChanged)
			{
				if (!m_NetworkManager.IsConnectedClient)
				{
					DeferObjectsMovedIntoNewScene(reader);
				}
				else
				{
					DeserializeObjectsMovedIntoNewScene(reader);
				}
				return;
			}
			reader.ReadValueSafe(out byte value, default(FastBufferWriter.ForPrimitives));
			LoadSceneMode = (LoadSceneMode)value;
			if (SceneEventType != SceneEventType.Synchronize)
			{
				reader.ReadValueSafe(out SceneEventProgressId, default(FastBufferWriter.ForStructs));
			}
			else
			{
				reader.ReadValueSafe(out ClientSynchronizationMode, default(FastBufferWriter.ForEnums));
			}
			reader.ReadValueSafe(out SceneHash, default(FastBufferWriter.ForPrimitives));
			reader.ReadValueSafe(out SceneHandle, default(FastBufferWriter.ForPrimitives));
			switch (SceneEventType)
			{
			case SceneEventType.Synchronize:
				reader.ReadValueSafe(out ActiveSceneHash, default(FastBufferWriter.ForPrimitives));
				CopySceneSynchronizationData(reader);
				break;
			case SceneEventType.SynchronizeComplete:
				CheckClientSynchronizationResults(reader);
				break;
			case SceneEventType.Load:
				m_HasInternalBuffer = true;
				InternalBuffer = new FastBufferReader(reader.GetUnsafePtrAtCurrentPosition(), Allocator.Persistent, reader.Length - reader.Position);
				break;
			case SceneEventType.ReSynchronize:
				ReadClientReSynchronizationData(reader);
				break;
			case SceneEventType.LoadEventCompleted:
			case SceneEventType.UnloadEventCompleted:
				ReadSceneEventProgressDone(reader);
				break;
			case SceneEventType.Unload:
			case SceneEventType.LoadComplete:
			case SceneEventType.UnloadComplete:
				break;
			}
		}

		internal unsafe void CopySceneSynchronizationData(FastBufferReader reader)
		{
			m_NetworkObjectsSync.Clear();
			reader.ReadValueSafe(out uint[] value, default(FastBufferWriter.ForPrimitives));
			reader.ReadValueSafe(out uint[] value2, default(FastBufferWriter.ForPrimitives));
			ScenesToSynchronize = new Queue<uint>(value);
			SceneHandlesToSynchronize = new Queue<uint>(value2);
			reader.ReadValueSafe(out int value3, default(FastBufferWriter.ForPrimitives));
			if (!reader.TryBeginRead(value3))
			{
				throw new OverflowException("Not enough space in the buffer to read recorded synchronization data size.");
			}
			m_HasInternalBuffer = true;
			InternalBuffer = new FastBufferReader(reader.GetUnsafePtrAtCurrentPosition(), Allocator.Persistent, value3);
		}

		internal void DeserializeScenePlacedObjects()
		{
			try
			{
				InternalBuffer.ReadValueSafe(out ushort value, default(FastBufferWriter.ForPrimitives));
				for (ushort num = 0; num < value; num++)
				{
					NetworkObject.SceneObject sceneObject = default(NetworkObject.SceneObject);
					sceneObject.Deserialize(InternalBuffer);
					if (sceneObject.IsSceneObject)
					{
						m_NetworkManager.SceneManager.SetTheSceneBeingSynchronized(sceneObject.NetworkSceneHandle);
					}
					NetworkObject.AddSceneObject(in sceneObject, InternalBuffer, m_NetworkManager);
				}
				DeserializeDespawnedInScenePlacedNetworkObjects();
			}
			finally
			{
				InternalBuffer.Dispose();
				m_HasInternalBuffer = false;
			}
		}

		internal void ReadClientReSynchronizationData(FastBufferReader reader)
		{
			reader.ReadValueSafe(out uint[] value, default(FastBufferWriter.ForPrimitives));
			if (value.Length == 0)
			{
				return;
			}
			NetworkObject[] array = UnityEngine.Object.FindObjectsOfType<NetworkObject>();
			Dictionary<ulong, NetworkObject> dictionary = new Dictionary<ulong, NetworkObject>();
			NetworkObject[] array2 = array;
			foreach (NetworkObject networkObject in array2)
			{
				if (!dictionary.ContainsKey(networkObject.NetworkObjectId))
				{
					dictionary.Add(networkObject.NetworkObjectId, networkObject);
				}
			}
			uint[] array3 = value;
			foreach (uint num in array3)
			{
				if (!dictionary.ContainsKey(num))
				{
					continue;
				}
				NetworkObject networkObject2 = dictionary[num];
				dictionary.Remove(num);
				networkObject2.IsSpawned = false;
				if (m_NetworkManager.PrefabHandler.ContainsHandler(networkObject2))
				{
					if (m_NetworkManager.SpawnManager.SpawnedObjects.ContainsKey(num))
					{
						m_NetworkManager.SpawnManager.SpawnedObjects.Remove(num);
					}
					if (m_NetworkManager.SpawnManager.SpawnedObjectsList.Contains(networkObject2))
					{
						m_NetworkManager.SpawnManager.SpawnedObjectsList.Remove(networkObject2);
					}
					NetworkManager.Singleton.PrefabHandler.HandleNetworkPrefabDestroy(networkObject2);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(networkObject2.gameObject);
				}
			}
		}

		internal void WriteClientReSynchronizationData(FastBufferWriter writer)
		{
			writer.WriteValueSafe(m_NetworkObjectsToBeRemoved.ToArray(), default(FastBufferWriter.ForPrimitives));
		}

		internal bool ClientNeedsReSynchronization()
		{
			return m_NetworkObjectsToBeRemoved.Count > 0;
		}

		internal void CheckClientSynchronizationResults(FastBufferReader reader)
		{
			m_NetworkObjectsToBeRemoved.Clear();
			reader.ReadValueSafe(out uint value, default(FastBufferWriter.ForPrimitives));
			for (int i = 0; i < value; i++)
			{
				reader.ReadValueSafe(out uint value2, default(FastBufferWriter.ForPrimitives));
				if (!m_NetworkManager.SpawnManager.SpawnedObjects.ContainsKey(value2))
				{
					m_NetworkObjectsToBeRemoved.Add(value2);
				}
			}
		}

		internal void WriteClientSynchronizationResults(FastBufferWriter writer)
		{
			uint value = (uint)m_NetworkObjectsSync.Count;
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			foreach (NetworkObject item in m_NetworkObjectsSync)
			{
				value = (uint)item.NetworkObjectId;
				writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			}
		}

		private void DeserializeDespawnedInScenePlacedNetworkObjects()
		{
			m_DespawnedInSceneObjects.Clear();
			InternalBuffer.ReadValueSafe(out int value, default(FastBufferWriter.ForPrimitives));
			Dictionary<int, Dictionary<uint, NetworkObject>> dictionary = new Dictionary<int, Dictionary<uint, NetworkObject>>();
			for (int i = 0; i < value; i++)
			{
				InternalBuffer.ReadValueSafe(out int value2, default(FastBufferWriter.ForPrimitives));
				InternalBuffer.ReadValueSafe(out uint value3, default(FastBufferWriter.ForPrimitives));
				Dictionary<uint, NetworkObject> dictionary2 = new Dictionary<uint, NetworkObject>();
				if (!dictionary.ContainsKey(value2))
				{
					if (m_NetworkManager.SceneManager.ServerSceneHandleToClientSceneHandle.ContainsKey(value2))
					{
						int localSceneHandle = m_NetworkManager.SceneManager.ServerSceneHandleToClientSceneHandle[value2];
						if (m_NetworkManager.SceneManager.ScenesLoaded.ContainsKey(localSceneHandle))
						{
							_ = m_NetworkManager.SceneManager.ScenesLoaded[localSceneHandle];
							foreach (NetworkObject item in (from c in UnityEngine.Object.FindObjectsOfType<NetworkObject>(includeInactive: true)
								where c.GetSceneOriginHandle() == localSceneHandle && c.IsSceneObject != false
								select c).ToList())
							{
								if (!dictionary2.ContainsKey(item.GlobalObjectIdHash))
								{
									dictionary2.Add(item.GlobalObjectIdHash, item);
								}
							}
							dictionary.Add(value2, dictionary2);
						}
						else
						{
							Debug.LogError($"In-Scene NetworkObject GlobalObjectIdHash ({value3}) cannot find its relative local scene handle {localSceneHandle}!");
						}
					}
					else
					{
						Debug.LogError($"In-Scene NetworkObject GlobalObjectIdHash ({value3}) cannot find its relative NetworkSceneHandle {value2}!");
					}
				}
				else
				{
					dictionary2 = dictionary[value2];
				}
				if (dictionary2.ContainsKey(value3))
				{
					dictionary2[value3].InvokeBehaviourNetworkDespawn();
					if (!m_NetworkManager.SceneManager.ScenePlacedObjects.ContainsKey(value3))
					{
						m_NetworkManager.SceneManager.ScenePlacedObjects.Add(value3, new Dictionary<int, NetworkObject>());
					}
					if (!m_NetworkManager.SceneManager.ScenePlacedObjects[value3].ContainsKey(dictionary2[value3].GetSceneOriginHandle()))
					{
						m_NetworkManager.SceneManager.ScenePlacedObjects[value3].Add(dictionary2[value3].GetSceneOriginHandle(), dictionary2[value3]);
					}
				}
				else
				{
					Debug.LogError($"In-Scene NetworkObject GlobalObjectIdHash ({value3}) could not be found!");
				}
			}
		}

		internal void SynchronizeSceneNetworkObjects(NetworkManager networkManager)
		{
			try
			{
				InternalBuffer.ReadValueSafe(out int value, default(FastBufferWriter.ForPrimitives));
				for (int i = 0; i < value; i++)
				{
					NetworkObject.SceneObject sceneObject = default(NetworkObject.SceneObject);
					sceneObject.Deserialize(InternalBuffer);
					if (sceneObject.IsSceneObject)
					{
						m_NetworkManager.SceneManager.SetTheSceneBeingSynchronized(sceneObject.NetworkSceneHandle);
					}
					NetworkObject networkObject = NetworkObject.AddSceneObject(in sceneObject, InternalBuffer, networkManager);
					if (networkObject != null && !m_NetworkObjectsSync.Contains(networkObject))
					{
						m_NetworkObjectsSync.Add(networkObject);
					}
				}
				DeserializeDespawnedInScenePlacedNetworkObjects();
			}
			finally
			{
				InternalBuffer.Dispose();
				m_HasInternalBuffer = false;
			}
		}

		internal void WriteSceneEventProgressDone(FastBufferWriter writer)
		{
			ushort value = (ushort)ClientsCompleted.Count;
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			foreach (ulong item in ClientsCompleted)
			{
				ulong value2 = item;
				writer.WriteValueSafe(in value2, default(FastBufferWriter.ForPrimitives));
			}
			value = (ushort)ClientsTimedOut.Count;
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			foreach (ulong item2 in ClientsTimedOut)
			{
				ulong value3 = item2;
				writer.WriteValueSafe(in value3, default(FastBufferWriter.ForPrimitives));
			}
		}

		internal void ReadSceneEventProgressDone(FastBufferReader reader)
		{
			reader.ReadValueSafe(out ushort value, default(FastBufferWriter.ForPrimitives));
			ClientsCompleted = new List<ulong>();
			for (int i = 0; i < value; i++)
			{
				reader.ReadValueSafe(out ulong value2, default(FastBufferWriter.ForPrimitives));
				ClientsCompleted.Add(value2);
			}
			reader.ReadValueSafe(out ushort value3, default(FastBufferWriter.ForPrimitives));
			ClientsTimedOut = new List<ulong>();
			for (int j = 0; j < value3; j++)
			{
				reader.ReadValueSafe(out ulong value4, default(FastBufferWriter.ForPrimitives));
				ClientsTimedOut.Add(value4);
			}
		}

		private void SerializeObjectsMovedIntoNewScene(FastBufferWriter writer)
		{
			NetworkSceneManager sceneManager = m_NetworkManager.SceneManager;
			int value = sceneManager.ObjectsMigratedIntoNewScene.Count;
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			foreach (KeyValuePair<int, List<NetworkObject>> item in sceneManager.ObjectsMigratedIntoNewScene)
			{
				value = item.Key;
				writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
				value = item.Value.Count;
				writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
				foreach (NetworkObject item2 in item.Value)
				{
					ulong value2 = item2.NetworkObjectId;
					writer.WriteValueSafe(in value2, default(FastBufferWriter.ForPrimitives));
				}
			}
			sceneManager.ObjectsMigratedIntoNewScene.Clear();
		}

		private void DeserializeObjectsMovedIntoNewScene(FastBufferReader reader)
		{
			NetworkSceneManager sceneManager = m_NetworkManager.SceneManager;
			NetworkSpawnManager spawnManager = m_NetworkManager.SpawnManager;
			sceneManager.ObjectsMigratedIntoNewScene.Clear();
			int value = 0;
			int value2 = 0;
			int value3 = 0;
			ulong value4 = 0uL;
			reader.ReadValueSafe(out value, default(FastBufferWriter.ForPrimitives));
			for (int i = 0; i < value; i++)
			{
				reader.ReadValueSafe(out value2, default(FastBufferWriter.ForPrimitives));
				sceneManager.ObjectsMigratedIntoNewScene.Add(value2, new List<NetworkObject>());
				reader.ReadValueSafe(out value3, default(FastBufferWriter.ForPrimitives));
				for (int j = 0; j < value3; j++)
				{
					reader.ReadValueSafe(out value4, default(FastBufferWriter.ForPrimitives));
					if (!spawnManager.SpawnedObjects.ContainsKey(value4))
					{
						NetworkLog.LogError($"[Object Scene Migration] Trying to synchronize NetworkObjectId ({value4}) but it was not spawned or no longer exists!!");
					}
					else
					{
						sceneManager.ObjectsMigratedIntoNewScene[value2].Add(spawnManager.SpawnedObjects[value4]);
					}
				}
			}
		}

		private void DeferObjectsMovedIntoNewScene(FastBufferReader reader)
		{
			NetworkSceneManager sceneManager = m_NetworkManager.SceneManager;
			_ = m_NetworkManager.SpawnManager;
			int value = 0;
			int value2 = 0;
			int value3 = 0;
			ulong value4 = 0uL;
			NetworkSceneManager.DeferredObjectsMovedEvent deferredObjectsMovedEvent = default(NetworkSceneManager.DeferredObjectsMovedEvent);
			deferredObjectsMovedEvent.ObjectsMigratedTable = new Dictionary<int, List<ulong>>();
			NetworkSceneManager.DeferredObjectsMovedEvent item = deferredObjectsMovedEvent;
			reader.ReadValueSafe(out value, default(FastBufferWriter.ForPrimitives));
			for (int i = 0; i < value; i++)
			{
				reader.ReadValueSafe(out value2, default(FastBufferWriter.ForPrimitives));
				item.ObjectsMigratedTable.Add(value2, new List<ulong>());
				reader.ReadValueSafe(out value3, default(FastBufferWriter.ForPrimitives));
				for (int j = 0; j < value3; j++)
				{
					reader.ReadValueSafe(out value4, default(FastBufferWriter.ForPrimitives));
					item.ObjectsMigratedTable[value2].Add(value4);
				}
			}
			sceneManager.DeferredObjectsMovedEvents.Add(item);
		}

		internal void ProcessDeferredObjectSceneChangedEvents()
		{
			NetworkSceneManager sceneManager = m_NetworkManager.SceneManager;
			NetworkSpawnManager spawnManager = m_NetworkManager.SpawnManager;
			if (sceneManager.DeferredObjectsMovedEvents.Count == 0)
			{
				return;
			}
			foreach (NetworkSceneManager.DeferredObjectsMovedEvent deferredObjectsMovedEvent in sceneManager.DeferredObjectsMovedEvents)
			{
				foreach (KeyValuePair<int, List<ulong>> item2 in deferredObjectsMovedEvent.ObjectsMigratedTable)
				{
					if (!sceneManager.ObjectsMigratedIntoNewScene.ContainsKey(item2.Key))
					{
						sceneManager.ObjectsMigratedIntoNewScene.Add(item2.Key, new List<NetworkObject>());
					}
					foreach (ulong item3 in item2.Value)
					{
						if (!spawnManager.SpawnedObjects.ContainsKey(item3))
						{
							NetworkLog.LogWarning($"[Deferred][Object Scene Migration] Trying to synchronize NetworkObjectId ({item3}) but it was not spawned or no longer exists!");
							continue;
						}
						NetworkObject item = spawnManager.SpawnedObjects[item3];
						if (!sceneManager.ObjectsMigratedIntoNewScene[item2.Key].Contains(item))
						{
							sceneManager.ObjectsMigratedIntoNewScene[item2.Key].Add(item);
						}
					}
				}
				deferredObjectsMovedEvent.ObjectsMigratedTable.Clear();
			}
			sceneManager.DeferredObjectsMovedEvents.Clear();
			if (sceneManager.ObjectsMigratedIntoNewScene.Count > 0)
			{
				sceneManager.MigrateNetworkObjectsIntoScenes();
			}
		}

		public void Dispose()
		{
			if (m_HasInternalBuffer)
			{
				InternalBuffer.Dispose();
				m_HasInternalBuffer = false;
			}
		}

		internal SceneEventData(NetworkManager networkManager)
		{
			m_NetworkManager = networkManager;
			SceneEventId = Guid.NewGuid().ToString().Hash32();
		}
	}
}
