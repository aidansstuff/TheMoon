using System;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDProcessedVisibleLightsBuilder
	{
		[Flags]
		internal enum ShadowMapFlags
		{
			None = 0,
			WillRenderShadowMap = 1,
			WillRenderScreenSpaceShadow = 2,
			WillRenderRayTracedShadow = 4
		}

		private enum ProcessLightsCountSlots
		{
			ProcessedLights = 0,
			DirectionalLights = 1,
			PunctualLights = 2,
			AreaLightCounts = 3,
			ShadowLights = 4,
			BakedShadows = 5
		}

		[BurstCompile]
		private struct ProcessVisibleLightJob : IJobParallelFor
		{
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<HDLightRenderData> lightData;

			[ReadOnly]
			public NativeArray<VisibleLight> visibleLights;

			[ReadOnly]
			public NativeArray<int> visibleLightEntityDataIndices;

			[ReadOnly]
			public NativeArray<LightBakingOutput> visibleLightBakingOutput;

			[ReadOnly]
			public NativeArray<LightShadows> visibleLightShadows;

			[ReadOnly]
			public int totalLightCounts;

			[ReadOnly]
			public float3 cameraPosition;

			[ReadOnly]
			public int pixelCount;

			[ReadOnly]
			public bool enableAreaLights;

			[ReadOnly]
			public bool enableRayTracing;

			[ReadOnly]
			public bool showDirectionalLight;

			[ReadOnly]
			public bool showPunctualLight;

			[ReadOnly]
			public bool showAreaLight;

			[ReadOnly]
			public bool enableShadowMaps;

			[ReadOnly]
			public bool enableScreenSpaceShadows;

			[ReadOnly]
			public int maxDirectionalLightsOnScreen;

			[ReadOnly]
			public int maxPunctualLightsOnScreen;

			[ReadOnly]
			public int maxAreaLightsOnScreen;

			[ReadOnly]
			public DebugLightFilterMode debugFilterMode;

			[WriteOnly]
			public NativeArray<int> processedVisibleLightCountsPtr;

			[WriteOnly]
			public NativeArray<LightVolumeType> processedLightVolumeType;

			[WriteOnly]
			public NativeArray<HDProcessedVisibleLight> processedEntities;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<uint> sortKeys;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<int> shadowLightsDataIndices;

			private bool TrivialRejectLight(in VisibleLight light, int dataIndex)
			{
				if (dataIndex < 0)
				{
					return true;
				}
				if (light.screenRect.height * light.screenRect.width * (float)pixelCount < 1f)
				{
					return true;
				}
				return false;
			}

			private unsafe int IncrementCounter(ProcessLightsCountSlots counterSlot)
			{
				return Interlocked.Increment(ref UnsafeUtility.AsRef<int>((byte*)processedVisibleLightCountsPtr.GetUnsafePtr() + (nint)counterSlot * (nint)4));
			}

			private unsafe int DecrementCounter(ProcessLightsCountSlots counterSlot)
			{
				return Interlocked.Decrement(ref UnsafeUtility.AsRef<int>((byte*)processedVisibleLightCountsPtr.GetUnsafePtr() + (nint)counterSlot * (nint)4));
			}

			private int NextOutputIndex()
			{
				return IncrementCounter(ProcessLightsCountSlots.ProcessedLights) - 1;
			}

			private bool IncrementLightCounterAndTestLimit(LightCategory lightCategory, GPULightType gpuLightType)
			{
				switch (lightCategory)
				{
				case LightCategory.Punctual:
					if (gpuLightType == GPULightType.Directional)
					{
						int num2 = IncrementCounter(ProcessLightsCountSlots.DirectionalLights) - 1;
						if (!showDirectionalLight || num2 >= maxDirectionalLightsOnScreen)
						{
							DecrementCounter(ProcessLightsCountSlots.DirectionalLights);
							return false;
						}
					}
					else
					{
						int num3 = IncrementCounter(ProcessLightsCountSlots.PunctualLights) - 1;
						if (!showPunctualLight || num3 >= maxPunctualLightsOnScreen)
						{
							DecrementCounter(ProcessLightsCountSlots.PunctualLights);
							return false;
						}
					}
					break;
				case LightCategory.Area:
				{
					int num = IncrementCounter(ProcessLightsCountSlots.AreaLightCounts) - 1;
					if (!showAreaLight || num >= maxAreaLightsOnScreen)
					{
						DecrementCounter(ProcessLightsCountSlots.AreaLightCounts);
						return false;
					}
					break;
				}
				}
				return true;
			}

			private ShadowMapFlags EvaluateShadowState(LightShadows shadows, HDLightType lightType, GPULightType gpuLightType, AreaLightShape areaLightShape, bool useScreenSpaceShadowsVal, bool useRayTracingShadowsVal, float shadowDimmerVal, float shadowFadeDistanceVal, float distanceToCamera, LightVolumeType lightVolumeType)
			{
				ShadowMapFlags shadowMapFlags = ShadowMapFlags.None;
				if (shadows == LightShadows.None || !enableShadowMaps)
				{
					return shadowMapFlags;
				}
				if (shadowDimmerVal <= 0f)
				{
					return shadowMapFlags;
				}
				if (lightType != HDLightType.Directional && !(distanceToCamera < shadowFadeDistanceVal))
				{
					return shadowMapFlags;
				}
				if (lightType == HDLightType.Area && areaLightShape != 0)
				{
					return shadowMapFlags;
				}
				shadowMapFlags |= ShadowMapFlags.WillRenderShadowMap;
				if (!enableScreenSpaceShadows)
				{
					return shadowMapFlags;
				}
				if (enableRayTracing && useRayTracingShadowsVal)
				{
					bool flag = false;
					if (gpuLightType == GPULightType.Point || gpuLightType == GPULightType.Rectangle || (gpuLightType == GPULightType.Spot && lightVolumeType == LightVolumeType.Cone))
					{
						flag = true;
					}
					if (flag)
					{
						shadowMapFlags |= ShadowMapFlags.WillRenderScreenSpaceShadow | ShadowMapFlags.WillRenderRayTracedShadow;
					}
				}
				if (useScreenSpaceShadowsVal && gpuLightType == GPULightType.Directional)
				{
					shadowMapFlags |= ShadowMapFlags.WillRenderScreenSpaceShadow;
					if (enableRayTracing && useRayTracingShadowsVal)
					{
						shadowMapFlags |= ShadowMapFlags.WillRenderRayTracedShadow;
					}
				}
				return shadowMapFlags;
			}

			private unsafe ref HDLightRenderData GetLightData(int dataIndex)
			{
				return ref UnsafeUtility.AsRef<HDLightRenderData>((byte*)lightData.GetUnsafePtr() + (nint)dataIndex * (nint)sizeof(HDLightRenderData));
			}

			public void Execute(int index)
			{
				VisibleLight light = visibleLights[index];
				int dataIndex = visibleLightEntityDataIndices[index];
				LightBakingOutput lightBakingOutput = visibleLightBakingOutput[index];
				LightShadows shadows = visibleLightShadows[index];
				if (TrivialRejectLight(in light, dataIndex))
				{
					return;
				}
				ref HDLightRenderData reference = ref GetLightData(dataIndex);
				if (enableRayTracing && !reference.includeForRayTracing)
				{
					return;
				}
				float3 y = light.GetPosition();
				float distanceToCamera = math.distance(cameraPosition, y);
				HDLightType hDLightType = HDAdditionalLightData.TranslateLightType(light.lightType, reference.pointLightType);
				LightCategory lightCategory = LightCategory.Count;
				GPULightType gpuLightType = GPULightType.Point;
				AreaLightShape areaLightShape = reference.areaLightShape;
				if (!enableAreaLights && hDLightType == HDLightType.Area && (areaLightShape == AreaLightShape.Rectangle || areaLightShape == AreaLightShape.Tube))
				{
					return;
				}
				SpotLightShape spotLightShape = reference.spotLightShape;
				LightVolumeType lightVolumeType = LightVolumeType.Count;
				bool flag = lightBakingOutput.lightmapBakeType == LightmapBakeType.Mixed && lightBakingOutput.mixedLightingMode == MixedLightingMode.Shadowmask && lightBakingOutput.occlusionMaskChannel != -1;
				HDRenderPipeline.EvaluateGPULightType(hDLightType, spotLightShape, areaLightShape, ref lightCategory, ref gpuLightType, ref lightVolumeType);
				if (debugFilterMode != 0 && debugFilterMode.IsEnabledFor(gpuLightType, spotLightShape))
				{
					return;
				}
				float num = ((gpuLightType == GPULightType.Directional) ? 1f : HDUtils.ComputeLinearDistanceFade(distanceToCamera, reference.fadeDistance));
				float lightVolumetricDistanceFade = ((gpuLightType == GPULightType.Directional) ? 1f : HDUtils.ComputeLinearDistanceFade(distanceToCamera, reference.volumetricFadeDistance));
				bool num2 = ((reference.lightDimmer > 0f && (reference.affectDiffuse || reference.affectSpecular)) || (reference.affectVolumetric ? reference.volumetricDimmer : 0f) > 0f) && num > 0f;
				ShadowMapFlags shadowMapFlags = EvaluateShadowState(shadows, hDLightType, gpuLightType, areaLightShape, reference.useScreenSpaceShadows, reference.useRayTracedShadows, reference.shadowDimmer, reference.shadowFadeDistance, distanceToCamera, lightVolumeType);
				if (num2 && IncrementLightCounterAndTestLimit(lightCategory, gpuLightType))
				{
					int index2 = NextOutputIndex();
					sortKeys[index2] = HDGpuLightsBuilder.PackLightSortKey(lightCategory, gpuLightType, lightVolumeType, index);
					processedLightVolumeType[index] = lightVolumeType;
					processedEntities[index] = new HDProcessedVisibleLight
					{
						dataIndex = dataIndex,
						gpuLightType = gpuLightType,
						lightType = hDLightType,
						lightDistanceFade = num,
						lightVolumetricDistanceFade = lightVolumetricDistanceFade,
						distanceToCamera = distanceToCamera,
						shadowMapFlags = shadowMapFlags,
						isBakedShadowMask = flag
					};
					if (flag)
					{
						IncrementCounter(ProcessLightsCountSlots.BakedShadows);
					}
					if ((shadowMapFlags & ShadowMapFlags.WillRenderShadowMap) != 0)
					{
						int index3 = IncrementCounter(ProcessLightsCountSlots.ShadowLights) - 1;
						shadowLightsDataIndices[index3] = index;
					}
				}
			}
		}

		private const int ArrayCapacity = 32;

		private NativeArray<int> m_ProcessVisibleLightCounts;

		private NativeArray<int> m_VisibleLightEntityDataIndices;

		private NativeArray<LightBakingOutput> m_VisibleLightBakingOutput;

		private NativeArray<LightShadowCasterMode> m_VisibleLightShadowCasterMode;

		private NativeArray<LightShadows> m_VisibleLightShadows;

		private NativeArray<LightVolumeType> m_ProcessedLightVolumeType;

		private NativeArray<HDProcessedVisibleLight> m_ProcessedEntities;

		private int m_Capacity;

		private int m_Size;

		private NativeArray<uint> m_SortKeys;

		private NativeArray<uint> m_SortSupportArray;

		private NativeArray<int> m_ShadowLightsDataIndices;

		private JobHandle m_ProcessVisibleLightJobHandle;

		public int sortedLightCounts => m_ProcessVisibleLightCounts[0];

		public int sortedDirectionalLightCounts => m_ProcessVisibleLightCounts[1];

		public int sortedNonDirectionalLightCounts => sortedLightCounts - sortedDirectionalLightCounts;

		public int bakedShadowsCount => m_ProcessVisibleLightCounts[5];

		public NativeArray<LightBakingOutput> visibleLightBakingOutput => m_VisibleLightBakingOutput;

		public NativeArray<LightShadowCasterMode> visibleLightShadowCasterMode => m_VisibleLightShadowCasterMode;

		public NativeArray<int> visibleLightEntityDataIndices => m_VisibleLightEntityDataIndices;

		public NativeArray<LightVolumeType> processedLightVolumeType => m_ProcessedLightVolumeType;

		public NativeArray<HDProcessedVisibleLight> processedEntities => m_ProcessedEntities;

		public NativeArray<uint> sortKeys => m_SortKeys;

		public NativeArray<uint> sortSupportArray => m_SortSupportArray;

		public NativeArray<int> shadowLightsDataIndices => m_ShadowLightsDataIndices;

		public void Reset()
		{
			m_Size = 0;
		}

		public void Build(HDCamera hdCamera, in CullingResults cullingResult, bool rayTracingState, HDShadowManager shadowManager, in HDShadowInitParameters inShadowInitParameters, in AOVRequestData aovRequestData, in GlobalLightLoopSettings lightLoopSettings, DebugDisplaySettings debugDisplaySettings)
		{
			BuildVisibleLightEntities(in cullingResult);
			if (m_Size != 0)
			{
				FilterVisibleLightsByAOV(aovRequestData);
				StartProcessVisibleLightJob(hdCamera, rayTracingState, cullingResult.visibleLights, in lightLoopSettings, debugDisplaySettings);
				CompleteProcessVisibleLightJob();
				SortLightKeys();
				ProcessShadows(hdCamera, shadowManager, in inShadowInitParameters, in cullingResult);
			}
		}

		private void ResizeArrays(int newCapacity)
		{
			m_Capacity = Math.Max(Math.Max(newCapacity, 32), m_Capacity * 2);
			ArrayExtensions.ResizeArray(ref m_VisibleLightEntityDataIndices, m_Capacity);
			ArrayExtensions.ResizeArray(ref m_VisibleLightBakingOutput, m_Capacity);
			ArrayExtensions.ResizeArray(ref m_VisibleLightShadowCasterMode, m_Capacity);
			ArrayExtensions.ResizeArray(ref m_VisibleLightShadows, m_Capacity);
			ArrayExtensions.ResizeArray(ref m_ProcessedLightVolumeType, m_Capacity);
			ArrayExtensions.ResizeArray(ref m_ProcessedEntities, m_Capacity);
			ArrayExtensions.ResizeArray(ref m_SortKeys, m_Capacity);
			ArrayExtensions.ResizeArray(ref m_ShadowLightsDataIndices, m_Capacity);
		}

		public void Cleanup()
		{
			if (m_SortSupportArray.IsCreated)
			{
				m_SortSupportArray.Dispose();
			}
			if (m_Capacity != 0)
			{
				m_ProcessVisibleLightCounts.Dispose();
				m_VisibleLightEntityDataIndices.Dispose();
				m_VisibleLightBakingOutput.Dispose();
				m_VisibleLightShadowCasterMode.Dispose();
				m_VisibleLightShadows.Dispose();
				m_ProcessedLightVolumeType.Dispose();
				m_ProcessedEntities.Dispose();
				m_SortKeys.Dispose();
				m_ShadowLightsDataIndices.Dispose();
				m_Capacity = 0;
				m_Size = 0;
			}
		}

		public void StartProcessVisibleLightJob(HDCamera hdCamera, bool rayTracingState, NativeArray<VisibleLight> visibleLights, in GlobalLightLoopSettings lightLoopSettings, DebugDisplaySettings debugDisplaySettings)
		{
			if (m_Size != 0)
			{
				HDLightRenderDatabase instance = HDLightRenderDatabase.instance;
				ProcessVisibleLightJob processVisibleLightJob = default(ProcessVisibleLightJob);
				processVisibleLightJob.totalLightCounts = instance.lightCount;
				processVisibleLightJob.cameraPosition = hdCamera.camera.transform.position;
				processVisibleLightJob.pixelCount = hdCamera.actualWidth * hdCamera.actualHeight;
				processVisibleLightJob.enableAreaLights = ShaderConfig.s_AreaLights != 0;
				processVisibleLightJob.enableRayTracing = hdCamera.frameSettings.IsEnabled(FrameSettingsField.RayTracing) && rayTracingState;
				processVisibleLightJob.showDirectionalLight = debugDisplaySettings.data.lightingDebugSettings.showDirectionalLight;
				processVisibleLightJob.showPunctualLight = debugDisplaySettings.data.lightingDebugSettings.showPunctualLight;
				processVisibleLightJob.showAreaLight = debugDisplaySettings.data.lightingDebugSettings.showAreaLight;
				processVisibleLightJob.enableShadowMaps = hdCamera.frameSettings.IsEnabled(FrameSettingsField.ShadowMaps);
				processVisibleLightJob.enableScreenSpaceShadows = hdCamera.frameSettings.IsEnabled(FrameSettingsField.ScreenSpaceShadows);
				processVisibleLightJob.maxDirectionalLightsOnScreen = lightLoopSettings.maxDirectionalLightsOnScreen;
				processVisibleLightJob.maxPunctualLightsOnScreen = lightLoopSettings.maxPunctualLightsOnScreen;
				processVisibleLightJob.maxAreaLightsOnScreen = lightLoopSettings.maxAreaLightsOnScreen;
				processVisibleLightJob.debugFilterMode = debugDisplaySettings.GetDebugLightFilterMode();
				processVisibleLightJob.lightData = instance.lightData;
				processVisibleLightJob.visibleLights = visibleLights;
				processVisibleLightJob.visibleLightEntityDataIndices = m_VisibleLightEntityDataIndices;
				processVisibleLightJob.visibleLightBakingOutput = m_VisibleLightBakingOutput;
				processVisibleLightJob.visibleLightShadows = m_VisibleLightShadows;
				processVisibleLightJob.processedVisibleLightCountsPtr = m_ProcessVisibleLightCounts;
				processVisibleLightJob.processedLightVolumeType = m_ProcessedLightVolumeType;
				processVisibleLightJob.processedEntities = m_ProcessedEntities;
				processVisibleLightJob.sortKeys = m_SortKeys;
				processVisibleLightJob.shadowLightsDataIndices = m_ShadowLightsDataIndices;
				ProcessVisibleLightJob jobData = processVisibleLightJob;
				m_ProcessVisibleLightJobHandle = IJobParallelForExtensions.Schedule(jobData, m_Size, 32);
			}
		}

		public void CompleteProcessVisibleLightJob()
		{
			if (m_Size != 0)
			{
				m_ProcessVisibleLightJobHandle.Complete();
			}
		}

		private void SortLightKeys()
		{
			using (new ProfilingScope(null, ProfilingSampler.Get(HDProfileId.SortVisibleLights)))
			{
				int num = sortedLightCounts;
				if (num <= 32)
				{
					CoreUnsafeUtils.InsertionSort(m_SortKeys, num);
				}
				else if (m_Size <= 200)
				{
					CoreUnsafeUtils.MergeSort(m_SortKeys, num, ref m_SortSupportArray);
				}
				else
				{
					CoreUnsafeUtils.RadixSort(m_SortKeys, num, ref m_SortSupportArray);
				}
			}
		}

		private void BuildVisibleLightEntities(in CullingResults cullResults)
		{
			m_Size = 0;
			if (!m_ProcessVisibleLightCounts.IsCreated)
			{
				int length = Enum.GetValues(typeof(ProcessLightsCountSlots)).Length;
				ArrayExtensions.ResizeArray(ref m_ProcessVisibleLightCounts, length);
			}
			for (int i = 0; i < m_ProcessVisibleLightCounts.Length; i++)
			{
				m_ProcessVisibleLightCounts[i] = 0;
			}
			using (new ProfilingScope(null, ProfilingSampler.Get(HDProfileId.BuildVisibleLightEntities)))
			{
				if (cullResults.visibleLights.Length == 0 || HDLightRenderDatabase.instance == null)
				{
					return;
				}
				if (cullResults.visibleLights.Length > m_Capacity)
				{
					ResizeArrays(cullResults.visibleLights.Length);
				}
				m_Size = cullResults.visibleLights.Length;
				HDLightRenderEntity defaultLightEntity = HDLightRenderDatabase.instance.GetDefaultLightEntity();
				for (int j = 0; j < cullResults.visibleLights.Length; j++)
				{
					Light light = cullResults.visibleLights[j].light;
					int num = HDLightRenderDatabase.instance.FindEntityDataIndex(in light);
					if (num == HDLightRenderDatabase.InvalidDataIndex)
					{
						if (light.TryGetComponent<HDAdditionalLightData>(out var component))
						{
							if (!component.lightEntity.valid)
							{
								component.CreateHDLightRenderEntity(autoDestroy: true);
							}
						}
						else if (light != null && light.type == LightType.Directional)
						{
							HDAdditionalLightData hDAdditionalLightData = light.gameObject.AddComponent<HDAdditionalLightData>();
							if ((bool)hDAdditionalLightData)
							{
								HDAdditionalLightData.InitDefaultHDAdditionalLightData(hDAdditionalLightData);
							}
							if (!hDAdditionalLightData.lightEntity.valid)
							{
								hDAdditionalLightData.CreateHDLightRenderEntity(autoDestroy: true);
							}
						}
						else
						{
							num = HDLightRenderDatabase.instance.GetEntityDataIndex(defaultLightEntity);
						}
					}
					m_VisibleLightEntityDataIndices[j] = num;
					m_VisibleLightBakingOutput[j] = light.bakingOutput;
					m_VisibleLightShadowCasterMode[j] = light.lightShadowCasterMode;
					m_VisibleLightShadows[j] = light.shadows;
				}
			}
		}

		private unsafe void ProcessShadows(HDCamera hdCamera, HDShadowManager shadowManager, in HDShadowInitParameters inShadowInitParameters, in CullingResults cullResults)
		{
			int num = m_ProcessVisibleLightCounts[4];
			if (num == 0)
			{
				return;
			}
			using (new ProfilingScope(null, ProfilingSampler.Get(HDProfileId.ProcessShadows)))
			{
				NativeArray<VisibleLight> visibleLights = cullResults.visibleLights;
				HDShadowSettings component = hdCamera.volumeStack.GetComponent<HDShadowSettings>();
				HDLightRenderEntity defaultLightEntity = HDLightRenderDatabase.instance.GetDefaultLightEntity();
				int entityDataIndex = HDLightRenderDatabase.instance.GetEntityDataIndex(defaultLightEntity);
				HDProcessedVisibleLight* unsafePtr = (HDProcessedVisibleLight*)m_ProcessedEntities.GetUnsafePtr();
				for (int i = 0; i < num; i++)
				{
					int num2 = m_ShadowLightsDataIndices[i];
					HDProcessedVisibleLight* ptr = unsafePtr + num2;
					if (!cullResults.GetShadowCasterBounds(num2, out var _) || entityDataIndex == ptr->dataIndex)
					{
						ptr->shadowMapFlags = ShadowMapFlags.None;
						continue;
					}
					HDAdditionalLightData hDAdditionalLightData = HDLightRenderDatabase.instance.hdAdditionalLightData[ptr->dataIndex];
					if (!(hDAdditionalLightData == null))
					{
						VisibleLight visibleLight = visibleLights[num2];
						hDAdditionalLightData.ReserveShadowMap(hdCamera.camera, shadowManager, component, in inShadowInitParameters, in visibleLight, ptr->lightType);
					}
				}
			}
		}

		private void FilterVisibleLightsByAOV(AOVRequestData aovRequest)
		{
			if (!aovRequest.hasLightFilter)
			{
				return;
			}
			for (int i = 0; i < m_Size; i++)
			{
				int num = m_VisibleLightEntityDataIndices[i];
				if (num != HDLightRenderDatabase.InvalidDataIndex)
				{
					GameObject gameObject = HDLightRenderDatabase.instance.aovGameObjects[num];
					if (!(gameObject == null) && !aovRequest.IsLightEnabled(gameObject))
					{
						m_VisibleLightEntityDataIndices[i] = HDLightRenderDatabase.InvalidDataIndex;
					}
				}
			}
		}
	}
}
