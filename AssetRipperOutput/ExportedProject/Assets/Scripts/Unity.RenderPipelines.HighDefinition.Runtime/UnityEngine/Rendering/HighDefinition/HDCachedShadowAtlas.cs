using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDCachedShadowAtlas : HDShadowAtlas
	{
		private struct CachedShadowRecord
		{
			internal int shadowIndex;

			internal int viewportSize;

			internal Vector4 offsetInAtlas;

			internal bool rendersOnPlacement;
		}

		private struct CachedTransform
		{
			internal Vector3 position;

			internal Vector3 angles;
		}

		private enum SlotValue : byte
		{
			Free = 0,
			Occupied = 1,
			TempOccupied = 2
		}

		private static int s_InitialCapacity = 256;

		private const int m_MaxShadowsPerLight = 6;

		private int m_NextLightID;

		private bool m_CanTryPlacement;

		private int m_AtlasResolutionInSlots;

		private bool m_NeedOptimalPacking = true;

		private List<SlotValue> m_AtlasSlots;

		private Dictionary<int, CachedShadowRecord> m_PlacedShadows;

		private Dictionary<int, CachedShadowRecord> m_ShadowsPendingRendering;

		private Dictionary<int, int> m_ShadowsWithValidData;

		private Dictionary<int, HDAdditionalLightData> m_RegisteredLightDataPendingPlacement;

		private Dictionary<int, CachedShadowRecord> m_RecordsPendingPlacement;

		private Dictionary<int, CachedTransform> m_TransformCaches;

		private List<CachedShadowRecord> m_TempListForPlacement;

		private ShadowMapType m_ShadowType;

		public HDCachedShadowAtlas(ShadowMapType type)
		{
			m_PlacedShadows = new Dictionary<int, CachedShadowRecord>(s_InitialCapacity);
			m_ShadowsPendingRendering = new Dictionary<int, CachedShadowRecord>(s_InitialCapacity);
			m_ShadowsWithValidData = new Dictionary<int, int>(s_InitialCapacity);
			m_TempListForPlacement = new List<CachedShadowRecord>(s_InitialCapacity);
			m_RegisteredLightDataPendingPlacement = new Dictionary<int, HDAdditionalLightData>(s_InitialCapacity);
			m_RecordsPendingPlacement = new Dictionary<int, CachedShadowRecord>(s_InitialCapacity);
			m_TransformCaches = new Dictionary<int, CachedTransform>(s_InitialCapacity / 2);
			m_ShadowType = type;
		}

		public override void InitAtlas(HDShadowAtlasInitParameters atlasInitParams)
		{
			base.InitAtlas(atlasInitParams);
			m_IsACacheForShadows = true;
			m_AtlasResolutionInSlots = HDUtils.DivRoundUp(base.width, 64);
			m_AtlasSlots = new List<SlotValue>(m_AtlasResolutionInSlots * m_AtlasResolutionInSlots);
			for (int i = 0; i < m_AtlasResolutionInSlots * m_AtlasResolutionInSlots; i++)
			{
				m_AtlasSlots.Add(SlotValue.Free);
			}
			DefragmentAtlasAndReRender(atlasInitParams.initParams);
			m_CanTryPlacement = true;
			m_NeedOptimalPacking = true;
		}

		private bool IsEntryEmpty(int x, int y)
		{
			return m_AtlasSlots[y * m_AtlasResolutionInSlots + x] == SlotValue.Free;
		}

		private bool IsEntryFull(int x, int y)
		{
			return m_AtlasSlots[y * m_AtlasResolutionInSlots + x] != SlotValue.Free;
		}

		private bool IsEntryTempOccupied(int x, int y)
		{
			return m_AtlasSlots[y * m_AtlasResolutionInSlots + x] == SlotValue.TempOccupied;
		}

		private void FillEntries(int x, int y, int numEntries)
		{
			MarkEntries(x, y, numEntries, SlotValue.Occupied);
		}

		private void MarkEntries(int x, int y, int numEntries, SlotValue value)
		{
			for (int i = y; i < y + numEntries; i++)
			{
				for (int j = x; j < x + numEntries; j++)
				{
					m_AtlasSlots[i * m_AtlasResolutionInSlots + j] = value;
				}
			}
		}

		private bool CheckSlotAvailability(int x, int y, int numEntries)
		{
			for (int i = y; i < y + numEntries; i++)
			{
				for (int j = x; j < x + numEntries; j++)
				{
					if (j >= m_AtlasResolutionInSlots || i >= m_AtlasResolutionInSlots || IsEntryFull(j, i))
					{
						return false;
					}
				}
			}
			return true;
		}

		internal bool FindSlotInAtlas(int resolution, bool tempFill, out int x, out int y)
		{
			int numEntries = HDUtils.DivRoundUp(resolution, 64);
			for (int i = 0; i < m_AtlasResolutionInSlots; i++)
			{
				for (int j = 0; j < m_AtlasResolutionInSlots; j++)
				{
					if (CheckSlotAvailability(j, i, numEntries))
					{
						x = j;
						y = i;
						if (tempFill)
						{
							MarkEntries(x, y, numEntries, SlotValue.TempOccupied);
						}
						return true;
					}
				}
			}
			x = 0;
			y = 0;
			return false;
		}

		internal void FreeTempFilled(int x, int y, int resolution)
		{
			int num = HDUtils.DivRoundUp(resolution, 64);
			for (int i = y; i < y + num; i++)
			{
				for (int j = x; j < x + num; j++)
				{
					if (m_AtlasSlots[i * m_AtlasResolutionInSlots + j] == SlotValue.TempOccupied)
					{
						m_AtlasSlots[i * m_AtlasResolutionInSlots + j] = SlotValue.Free;
					}
				}
			}
		}

		internal bool FindSlotInAtlas(int resolution, out int x, out int y)
		{
			return FindSlotInAtlas(resolution, tempFill: false, out x, out y);
		}

		internal bool GetSlotInAtlas(int resolution, out int x, out int y)
		{
			if (FindSlotInAtlas(resolution, out x, out y))
			{
				int numEntries = HDUtils.DivRoundUp(resolution, 64);
				FillEntries(x, y, numEntries);
				return true;
			}
			return false;
		}

		internal int GetNextLightIdentifier()
		{
			int nextLightID = m_NextLightID;
			m_NextLightID += 6;
			return nextLightID;
		}

		internal void RegisterLight(HDAdditionalLightData lightData)
		{
			if ((lightData.lightIdxForCachedShadows < 0 || !m_PlacedShadows.ContainsKey(lightData.lightIdxForCachedShadows)) && !m_RegisteredLightDataPendingPlacement.ContainsKey(lightData.lightIdxForCachedShadows) && lightData.isActiveAndEnabled)
			{
				lightData.legacyLight.useViewFrustumForShadowCasterCull = false;
				lightData.lightIdxForCachedShadows = GetNextLightIdentifier();
				RegisterTransformCacheSlot(lightData);
				m_RegisteredLightDataPendingPlacement.Add(lightData.lightIdxForCachedShadows, lightData);
				m_CanTryPlacement = true;
			}
		}

		internal void EvictLight(HDAdditionalLightData lightData)
		{
			m_RegisteredLightDataPendingPlacement.Remove(lightData.lightIdxForCachedShadows);
			RemoveTransformFromCache(lightData);
			int num = ((lightData.type != HDLightType.Point) ? 1 : 6);
			int lightIdxForCachedShadows = lightData.lightIdxForCachedShadows;
			lightData.lightIdxForCachedShadows = -1;
			for (int i = 0; i < num; i++)
			{
				int key = lightIdxForCachedShadows + i;
				m_RecordsPendingPlacement.Remove(key);
				if (m_PlacedShadows.TryGetValue(key, out var value))
				{
					lightData.legacyLight.useViewFrustumForShadowCasterCull = true;
					m_PlacedShadows.Remove(key);
					m_ShadowsPendingRendering.Remove(key);
					m_ShadowsWithValidData.Remove(key);
					MarkEntries((int)value.offsetInAtlas.z, (int)value.offsetInAtlas.w, HDUtils.DivRoundUp(value.viewportSize, 64), SlotValue.Free);
					m_CanTryPlacement = true;
				}
			}
		}

		internal void RegisterTransformCacheSlot(HDAdditionalLightData lightData)
		{
			if (lightData.lightIdxForCachedShadows >= 0 && lightData.updateUponLightMovement && !m_TransformCaches.ContainsKey(lightData.lightIdxForCachedShadows))
			{
				CachedTransform value = default(CachedTransform);
				value.position = lightData.transform.position;
				value.angles = lightData.transform.eulerAngles;
				m_TransformCaches.Add(lightData.lightIdxForCachedShadows, value);
			}
		}

		internal void RemoveTransformFromCache(HDAdditionalLightData lightData)
		{
			m_TransformCaches.Remove(lightData.lightIdxForCachedShadows);
		}

		private void InsertionSort(ref List<CachedShadowRecord> list, int startIndex, int lastIndex)
		{
			for (int i = startIndex; i < lastIndex; i++)
			{
				CachedShadowRecord value = list[i];
				int num = i - 1;
				while (num >= 0 && value.viewportSize > list[num].viewportSize)
				{
					list[num + 1] = list[num];
					num--;
				}
				list[num + 1] = value;
			}
		}

		private void AddLightListToRecordList(Dictionary<int, HDAdditionalLightData> lightList, HDShadowInitParameters initParams, ref List<CachedShadowRecord> recordList)
		{
			CachedShadowRecord item = default(CachedShadowRecord);
			foreach (HDAdditionalLightData value in lightList.Values)
			{
				int num = 0;
				num = value.GetResolutionFromSettings(m_ShadowType, initParams);
				int num2 = ((value.type != HDLightType.Point) ? 1 : 6);
				for (int i = 0; i < num2; i++)
				{
					item.shadowIndex = value.lightIdxForCachedShadows + i;
					item.viewportSize = num;
					item.offsetInAtlas = new Vector4(-1f, -1f, -1f, -1f);
					item.rendersOnPlacement = value.shadowUpdateMode != ShadowUpdateMode.OnDemand || value.forceRenderOnPlacement || value.onDemandShadowRenderOnPlacement;
					value.forceRenderOnPlacement = false;
					recordList.Add(item);
				}
			}
		}

		private bool PlaceMultipleShadows(int startIdx, int numberOfShadows)
		{
			_ = m_TempListForPlacement[startIdx];
			Vector2Int[] array = new Vector2Int[6];
			int num = 0;
			int x;
			int y;
			for (int i = 0; i < numberOfShadows && GetSlotInAtlas(m_TempListForPlacement[startIdx + i].viewportSize, out x, out y); i++)
			{
				num++;
				array[i] = new Vector2Int(x, y);
			}
			if (num == numberOfShadows)
			{
				for (int j = 0; j < numberOfShadows; j++)
				{
					CachedShadowRecord value = m_TempListForPlacement[startIdx + j];
					value.offsetInAtlas = new Vector4(array[j].x * 64, array[j].y * 64, array[j].x, array[j].y);
					if (value.rendersOnPlacement)
					{
						m_ShadowsPendingRendering.Add(value.shadowIndex, value);
					}
					m_PlacedShadows.Add(value.shadowIndex, value);
				}
				return true;
			}
			if (num > 0)
			{
				int numEntries = HDUtils.DivRoundUp(m_TempListForPlacement[startIdx].viewportSize, 64);
				for (int k = 0; k < num; k++)
				{
					MarkEntries(array[k].x, array[k].y, numEntries, SlotValue.Free);
				}
			}
			return false;
		}

		private void PerformPlacement()
		{
			int num = 0;
			while (num < m_TempListForPlacement.Count)
			{
				CachedShadowRecord value = m_TempListForPlacement[num];
				if (value.shadowIndex % 6 == 0 && num + 1 < m_TempListForPlacement.Count && m_TempListForPlacement[num + 1].shadowIndex % 6 != 0)
				{
					if (PlaceMultipleShadows(num, 6))
					{
						m_RegisteredLightDataPendingPlacement.Remove(value.shadowIndex);
						for (int i = 0; i < 6; i++)
						{
							m_RecordsPendingPlacement.Remove(value.shadowIndex + i);
						}
					}
					num += 6;
					continue;
				}
				if (GetSlotInAtlas(value.viewportSize, out var x, out var y))
				{
					value.offsetInAtlas = new Vector4(x * 64, y * 64, x, y);
					if (value.rendersOnPlacement)
					{
						m_ShadowsPendingRendering.Add(value.shadowIndex, value);
					}
					m_PlacedShadows.Add(value.shadowIndex, value);
					m_RegisteredLightDataPendingPlacement.Remove(value.shadowIndex);
					m_RecordsPendingPlacement.Remove(value.shadowIndex);
				}
				num++;
			}
		}

		internal void AssignOffsetsInAtlas(HDShadowInitParameters initParameters)
		{
			if (m_RegisteredLightDataPendingPlacement.Count > 0 && m_CanTryPlacement)
			{
				m_TempListForPlacement.Clear();
				m_TempListForPlacement.AddRange(m_RecordsPendingPlacement.Values);
				AddLightListToRecordList(m_RegisteredLightDataPendingPlacement, initParameters, ref m_TempListForPlacement);
				if (m_NeedOptimalPacking)
				{
					InsertionSort(ref m_TempListForPlacement, 0, m_TempListForPlacement.Count);
					m_NeedOptimalPacking = false;
				}
				PerformPlacement();
				m_CanTryPlacement = false;
			}
		}

		internal void DefragmentAtlasAndReRender(HDShadowInitParameters initParams)
		{
			m_TempListForPlacement.Clear();
			m_TempListForPlacement.AddRange(m_PlacedShadows.Values);
			m_TempListForPlacement.AddRange(m_RecordsPendingPlacement.Values);
			AddLightListToRecordList(m_RegisteredLightDataPendingPlacement, initParams, ref m_TempListForPlacement);
			for (int i = 0; i < m_AtlasResolutionInSlots * m_AtlasResolutionInSlots; i++)
			{
				m_AtlasSlots[i] = SlotValue.Free;
			}
			m_PlacedShadows.Clear();
			m_ShadowsPendingRendering.Clear();
			m_ShadowsWithValidData.Clear();
			m_RecordsPendingPlacement.Clear();
			InsertionSort(ref m_TempListForPlacement, 0, m_TempListForPlacement.Count);
			PerformPlacement();
			foreach (CachedShadowRecord item in m_TempListForPlacement)
			{
				if (!m_PlacedShadows.ContainsKey(item.shadowIndex))
				{
					int key = item.shadowIndex - item.shadowIndex % 6;
					if (!m_RegisteredLightDataPendingPlacement.ContainsKey(key) && !m_RecordsPendingPlacement.ContainsKey(item.shadowIndex))
					{
						m_RecordsPendingPlacement.Add(item.shadowIndex, item);
					}
				}
			}
			m_CanTryPlacement = false;
		}

		internal bool LightIsPendingPlacement(HDAdditionalLightData lightData)
		{
			if (!m_RegisteredLightDataPendingPlacement.ContainsKey(lightData.lightIdxForCachedShadows))
			{
				return m_RecordsPendingPlacement.ContainsKey(lightData.lightIdxForCachedShadows);
			}
			return true;
		}

		internal bool ShadowIsPendingRendering(int shadowIdx)
		{
			return m_ShadowsPendingRendering.ContainsKey(shadowIdx);
		}

		internal bool ShadowHasRenderedAtLeastOnce(int shadowIdx)
		{
			return m_ShadowsWithValidData.ContainsKey(shadowIdx);
		}

		internal bool FullLightShadowHasRenderedAtLeastOnce(HDAdditionalLightData lightData)
		{
			int lightIdxForCachedShadows = lightData.lightIdxForCachedShadows;
			if (lightData.type == HDLightType.Point)
			{
				bool flag = true;
				for (int i = 0; i < 6; i++)
				{
					flag = flag && m_ShadowsWithValidData.ContainsKey(lightIdxForCachedShadows + i);
				}
				return flag;
			}
			return m_ShadowsWithValidData.ContainsKey(lightIdxForCachedShadows);
		}

		internal bool LightIsPlaced(HDAdditionalLightData lightData)
		{
			int lightIdxForCachedShadows = lightData.lightIdxForCachedShadows;
			if (lightIdxForCachedShadows >= 0)
			{
				return m_PlacedShadows.ContainsKey(lightIdxForCachedShadows);
			}
			return false;
		}

		internal void ScheduleShadowUpdate(HDAdditionalLightData lightData)
		{
			if (!lightData.isActiveAndEnabled)
			{
				return;
			}
			int lightIdxForCachedShadows = lightData.lightIdxForCachedShadows;
			if (!m_PlacedShadows.ContainsKey(lightIdxForCachedShadows))
			{
				if (!m_RegisteredLightDataPendingPlacement.ContainsKey(lightIdxForCachedShadows))
				{
					lightData.forceRenderOnPlacement = true;
					RegisterLight(lightData);
				}
				return;
			}
			int num = ((lightData.type != HDLightType.Point) ? 1 : 6);
			for (int i = 0; i < num; i++)
			{
				int shadowIdx = lightIdxForCachedShadows + i;
				ScheduleShadowUpdate(shadowIdx);
			}
		}

		internal void ScheduleShadowUpdate(int shadowIdx)
		{
			if (m_PlacedShadows.TryGetValue(shadowIdx, out var value) && !m_ShadowsPendingRendering.ContainsKey(shadowIdx))
			{
				m_ShadowsPendingRendering.Add(shadowIdx, value);
			}
		}

		internal void MarkAsRendered(int shadowIdx)
		{
			if (m_ShadowsPendingRendering.ContainsKey(shadowIdx))
			{
				m_ShadowsPendingRendering.Remove(shadowIdx);
				if (!m_ShadowsWithValidData.ContainsKey(shadowIdx))
				{
					m_ShadowsWithValidData.Add(shadowIdx, shadowIdx);
				}
			}
		}

		internal void UpdateResolutionRequest(ref HDShadowResolutionRequest request, int shadowIdx)
		{
			if (!m_PlacedShadows.TryGetValue(shadowIdx, out var value))
			{
				Debug.LogWarning("Trying to render a cached shadow map that doesn't have a slot in the atlas yet.");
			}
			request.cachedAtlasViewport = new Rect(value.offsetInAtlas.x, value.offsetInAtlas.y, value.viewportSize, value.viewportSize);
			request.resolution = new Vector2(value.viewportSize, value.viewportSize);
		}

		internal bool NeedRenderingDueToTransformChange(HDAdditionalLightData lightData, HDLightType lightType)
		{
			bool flag = false;
			if (m_TransformCaches.TryGetValue(lightData.lightIdxForCachedShadows, out var value))
			{
				float cachedShadowTranslationUpdateThreshold = lightData.cachedShadowTranslationUpdateThreshold;
				Vector3 vector = value.position - lightData.transform.position;
				if (Vector3.Dot(vector, vector) > cachedShadowTranslationUpdateThreshold * cachedShadowTranslationUpdateThreshold)
				{
					flag = true;
				}
				if (lightType != HDLightType.Point)
				{
					float cachedShadowAngleUpdateThreshold = lightData.cachedShadowAngleUpdateThreshold;
					Vector3 vector2 = value.angles - lightData.transform.eulerAngles;
					if (Mathf.Abs(vector2.x) > cachedShadowAngleUpdateThreshold || Mathf.Abs(vector2.y) > cachedShadowAngleUpdateThreshold || Mathf.Abs(vector2.z) > cachedShadowAngleUpdateThreshold)
					{
						flag = true;
					}
				}
				if (flag)
				{
					m_TransformCaches.Remove(lightData.lightIdxForCachedShadows);
					value.position = lightData.transform.position;
					value.angles = lightData.transform.eulerAngles;
					m_TransformCaches.Add(lightData.lightIdxForCachedShadows, value);
				}
			}
			return flag;
		}
	}
}
