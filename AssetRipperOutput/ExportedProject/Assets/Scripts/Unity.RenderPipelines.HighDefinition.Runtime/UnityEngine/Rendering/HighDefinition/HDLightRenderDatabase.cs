using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDLightRenderDatabase
	{
		private struct LightEntityInfo
		{
			public int dataIndex;

			public int lightInstanceID;

			public static readonly LightEntityInfo Invalid = new LightEntityInfo
			{
				dataIndex = InvalidDataIndex,
				lightInstanceID = -1
			};

			public bool valid
			{
				get
				{
					if (dataIndex != -1)
					{
						return lightInstanceID != -1;
					}
					return false;
				}
			}
		}

		public static int InvalidDataIndex = -1;

		private const int ArrayCapacity = 100;

		private static HDLightRenderDatabase s_Instance = null;

		private int m_Capacity;

		private int m_LightCount;

		private int m_AttachedGameObjects;

		private HDLightRenderEntity m_DefaultLightEntity = HDLightRenderEntity.Invalid;

		private List<LightEntityInfo> m_LightEntities = new List<LightEntityInfo>();

		private Queue<int> m_FreeIndices = new Queue<int>();

		private Dictionary<int, LightEntityInfo> m_LightsToEntityItem = new Dictionary<int, LightEntityInfo>();

		private NativeArray<HDLightRenderData> m_LightData;

		private NativeArray<HDLightRenderEntity> m_OwnerEntity;

		private NativeArray<bool> m_AutoDestroy;

		private DynamicArray<GameObject> m_AOVGameObjects = new DynamicArray<GameObject>();

		private DynamicArray<HDAdditionalLightData> m_HDAdditionalLightData = new DynamicArray<HDAdditionalLightData>();

		public int lightCount => m_LightCount;

		public NativeArray<HDLightRenderData> lightData => m_LightData;

		public NativeArray<HDLightRenderEntity> lightEntities => m_OwnerEntity;

		public DynamicArray<HDAdditionalLightData> hdAdditionalLightData => m_HDAdditionalLightData;

		public DynamicArray<GameObject> aovGameObjects => m_AOVGameObjects;

		public static HDLightRenderDatabase instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = new HDLightRenderDatabase();
				}
				return s_Instance;
			}
		}

		public ref HDLightRenderData GetLightDataAsRef(in HDLightRenderEntity entity)
		{
			return ref EditLightDataAsRef(in entity);
		}

		public ref HDLightRenderData EditLightDataAsRef(in HDLightRenderEntity entity)
		{
			return ref EditLightDataAsRef(m_LightEntities[entity.entityIndex].dataIndex);
		}

		public ref HDLightRenderData GetLightDataAsRef(int dataIndex)
		{
			return ref EditLightDataAsRef(dataIndex);
		}

		public unsafe ref HDLightRenderData EditLightDataAsRef(int dataIndex)
		{
			if (dataIndex >= m_LightCount)
			{
				throw new Exception("Entity passed in is out of bounds. Index requested " + dataIndex + " and maximum length is " + m_LightCount);
			}
			return ref UnsafeUtility.AsRef<HDLightRenderData>((byte*)m_LightData.GetUnsafePtr() + (nint)dataIndex * (nint)sizeof(HDLightRenderData));
		}

		public HDLightRenderEntity CreateEntity(bool autoDestroy)
		{
			LightEntityInfo lightEntityInfo = AllocateEntityData();
			HDLightRenderEntity invalid = HDLightRenderEntity.Invalid;
			if (m_FreeIndices.Count == 0)
			{
				invalid.entityIndex = m_LightEntities.Count;
				m_LightEntities.Add(lightEntityInfo);
			}
			else
			{
				invalid.entityIndex = m_FreeIndices.Dequeue();
				m_LightEntities[invalid.entityIndex] = lightEntityInfo;
			}
			m_OwnerEntity[lightEntityInfo.dataIndex] = invalid;
			m_AutoDestroy[lightEntityInfo.dataIndex] = autoDestroy;
			return invalid;
		}

		public void AttachGameObjectData(HDLightRenderEntity entity, int instanceID, HDAdditionalLightData additionalLightData, GameObject aovGameObject)
		{
			if (IsValid(entity))
			{
				LightEntityInfo value = m_LightEntities[entity.entityIndex];
				int dataIndex = value.dataIndex;
				if (dataIndex != InvalidDataIndex)
				{
					value.lightInstanceID = instanceID;
					m_LightEntities[entity.entityIndex] = value;
					m_LightsToEntityItem.Add(value.lightInstanceID, value);
					m_HDAdditionalLightData[dataIndex] = additionalLightData;
					m_AOVGameObjects[dataIndex] = aovGameObject;
					m_AttachedGameObjects++;
				}
			}
		}

		public void DestroyEntity(HDLightRenderEntity lightEntity)
		{
			m_FreeIndices.Enqueue(lightEntity.entityIndex);
			LightEntityInfo lightEntityInfo = m_LightEntities[lightEntity.entityIndex];
			m_LightsToEntityItem.Remove(lightEntityInfo.lightInstanceID);
			if (m_HDAdditionalLightData[lightEntityInfo.dataIndex] != null)
			{
				m_AttachedGameObjects--;
			}
			RemoveAtSwapBackArrays(lightEntityInfo.dataIndex);
			if (m_LightCount == 0)
			{
				DeleteArrays();
				return;
			}
			HDLightRenderEntity hDLightRenderEntity = m_OwnerEntity[lightEntityInfo.dataIndex];
			LightEntityInfo value = m_LightEntities[hDLightRenderEntity.entityIndex];
			value.dataIndex = lightEntityInfo.dataIndex;
			m_LightEntities[hDLightRenderEntity.entityIndex] = value;
			if (value.lightInstanceID != lightEntityInfo.lightInstanceID)
			{
				m_LightsToEntityItem[value.lightInstanceID] = value;
			}
		}

		public void Cleanup()
		{
			m_DefaultLightEntity = HDLightRenderEntity.Invalid;
			HDUtils.s_DefaultHDAdditionalLightData.DestroyHDLightRenderEntity();
			List<HDAdditionalLightData> list = new List<HDAdditionalLightData>();
			for (int i = 0; i < m_LightCount; i++)
			{
				if (m_AutoDestroy[i] && m_HDAdditionalLightData[i] != null)
				{
					list.Add(m_HDAdditionalLightData[i]);
				}
			}
			foreach (HDAdditionalLightData item in list)
			{
				item.DestroyHDLightRenderEntity();
			}
		}

		public HDLightRenderEntity GetDefaultLightEntity()
		{
			if (!IsValid(m_DefaultLightEntity))
			{
				HDUtils.s_DefaultHDAdditionalLightData.CreateHDLightRenderEntity(autoDestroy: true);
				m_DefaultLightEntity = HDUtils.s_DefaultHDAdditionalLightData.lightEntity;
			}
			return m_DefaultLightEntity;
		}

		public bool IsValid(HDLightRenderEntity entity)
		{
			if (entity.valid)
			{
				return entity.entityIndex < m_LightEntities.Count;
			}
			return false;
		}

		public int GetEntityDataIndex(HDLightRenderEntity entity)
		{
			return GetEntityData(entity).dataIndex;
		}

		public int FindEntityDataIndex(in VisibleLight visibleLight)
		{
			Light light = visibleLight.light;
			return FindEntityDataIndex(in light);
		}

		public int FindEntityDataIndex(in Light light)
		{
			if (light != null && m_LightsToEntityItem.TryGetValue(light.GetInstanceID(), out var value))
			{
				return value.dataIndex;
			}
			return -1;
		}

		private void ResizeArrays()
		{
			m_HDAdditionalLightData.Resize(m_Capacity, keepContent: true);
			m_AOVGameObjects.Resize(m_Capacity, keepContent: true);
			ArrayExtensions.ResizeArray(ref m_LightData, m_Capacity);
			ArrayExtensions.ResizeArray(ref m_OwnerEntity, m_Capacity);
			ArrayExtensions.ResizeArray(ref m_AutoDestroy, m_Capacity);
		}

		private void RemoveAtSwapBackArrays(int removeIndexAt)
		{
			int index = m_LightCount - 1;
			m_HDAdditionalLightData[removeIndexAt] = m_HDAdditionalLightData[index];
			m_HDAdditionalLightData[index] = null;
			m_AOVGameObjects[removeIndexAt] = m_AOVGameObjects[index];
			m_AOVGameObjects[index] = null;
			m_LightData[removeIndexAt] = m_LightData[index];
			m_OwnerEntity[removeIndexAt] = m_OwnerEntity[index];
			m_AutoDestroy[removeIndexAt] = m_AutoDestroy[index];
			m_LightCount--;
		}

		private void DeleteArrays()
		{
			if (m_Capacity != 0)
			{
				m_HDAdditionalLightData.Clear();
				m_AOVGameObjects.Clear();
				m_LightData.Dispose();
				m_OwnerEntity.Dispose();
				m_AutoDestroy.Dispose();
				m_FreeIndices.Clear();
				m_LightEntities.Clear();
				m_Capacity = 0;
			}
		}

		private LightEntityInfo GetEntityData(HDLightRenderEntity entity)
		{
			return m_LightEntities[entity.entityIndex];
		}

		private LightEntityInfo AllocateEntityData()
		{
			if (m_Capacity == 0 || m_LightCount == m_Capacity)
			{
				m_Capacity = Math.Max(Math.Max(m_Capacity * 2, m_LightCount), 100);
				ResizeArrays();
			}
			int dataIndex = m_LightCount++;
			LightEntityInfo result = default(LightEntityInfo);
			result.dataIndex = dataIndex;
			result.lightInstanceID = -1;
			return result;
		}

		~HDLightRenderDatabase()
		{
			DeleteArrays();
		}
	}
}
