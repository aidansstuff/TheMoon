using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDGpuLightsBuilder
	{
		public struct LightsPerView
		{
			public Matrix4x4 worldToView;

			public int boundsOffset;

			public int boundsCount;
		}

		internal enum GPULightTypeCountSlots
		{
			Directional = 0,
			Punctual = 1,
			Area = 2
		}

		internal struct CreateGpuLightDataJobGlobalConfig
		{
			public bool lightLayersEnabled;

			public float specularGlobalDimmer;

			public int invalidScreenSpaceShadowIndex;

			public float maxShadowFadeDistance;

			public static CreateGpuLightDataJobGlobalConfig Create(HDCamera hdCamera, HDShadowSettings hdShadowSettings)
			{
				CreateGpuLightDataJobGlobalConfig result = default(CreateGpuLightDataJobGlobalConfig);
				result.lightLayersEnabled = hdCamera.frameSettings.IsEnabled(FrameSettingsField.LightLayers);
				result.specularGlobalDimmer = hdCamera.frameSettings.specularGlobalDimmer;
				result.maxShadowFadeDistance = hdShadowSettings.maxShadowDistance.value;
				result.invalidScreenSpaceShadowIndex = (int)LightDefinitions.s_InvalidScreenSpaceShadow;
				return result;
			}
		}

		[BurstCompile]
		internal struct CreateGpuLightDataJob : IJobParallelFor
		{
			[ReadOnly]
			public int totalLightCounts;

			[ReadOnly]
			public int outputLightCounts;

			[ReadOnly]
			public int outputDirectionalLightCounts;

			[ReadOnly]
			public int outputLightBoundsCount;

			[ReadOnly]
			public CreateGpuLightDataJobGlobalConfig globalConfig;

			[ReadOnly]
			public Vector3 cameraPos;

			[ReadOnly]
			public int directionalSortedLightCounts;

			[ReadOnly]
			public bool isPbrSkyActive;

			[ReadOnly]
			public int precomputedAtmosphericAttenuation;

			[ReadOnly]
			public int defaultDataIndex;

			[ReadOnly]
			public int viewCounts;

			[ReadOnly]
			public bool useCameraRelativePosition;

			[ReadOnly]
			public Vector3 planetCenterPosition;

			[ReadOnly]
			public float planetaryRadius;

			[ReadOnly]
			public float airScaleHeight;

			[ReadOnly]
			public float aerosolScaleHeight;

			[ReadOnly]
			public Vector3 airExtinctionCoefficient;

			[ReadOnly]
			public float aerosolExtinctionCoefficient;

			[ReadOnly]
			public float maxShadowDistance;

			[ReadOnly]
			public float shadowOutBorderDistance;

			[NativeDisableContainerSafetyRestriction]
			public NativeArray<HDLightRenderData> lightRenderDataArray;

			[ReadOnly]
			public NativeArray<uint> sortKeys;

			[ReadOnly]
			public NativeArray<HDProcessedVisibleLight> processedEntities;

			[ReadOnly]
			public NativeArray<VisibleLight> visibleLights;

			[ReadOnly]
			public NativeArray<LightBakingOutput> visibleLightBakingOutput;

			[ReadOnly]
			public NativeArray<LightShadowCasterMode> visibleLightShadowCasterMode;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<LightData> lights;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<DirectionalLightData> directionalLights;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<LightsPerView> lightsPerView;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<SFiniteLightBound> lightBounds;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<LightVolumeData> lightVolumes;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<int> gpuLightCounters;

			private unsafe ref HDLightRenderData GetLightData(int dataIndex)
			{
				return ref UnsafeUtility.AsRef<HDLightRenderData>((byte*)lightRenderDataArray.GetUnsafePtr() + (nint)dataIndex * (nint)sizeof(HDLightRenderData));
			}

			private static uint GetLightLayer(bool lightLayersEnabled, in HDLightRenderData lightRenderData)
			{
				int lightLayer = (int)lightRenderData.lightLayer;
				uint result = ((lightLayer < 0) ? 255u : ((uint)lightLayer));
				if (!lightLayersEnabled)
				{
					return uint.MaxValue;
				}
				return result;
			}

			private static Vector3 GetLightColor(in VisibleLight light)
			{
				return new Vector3(light.finalColor.r, light.finalColor.g, light.finalColor.b);
			}

			private unsafe void IncrementCounter(GPULightTypeCountSlots counterSlot)
			{
				Interlocked.Increment(ref UnsafeUtility.AsRef<int>((byte*)gpuLightCounters.GetUnsafePtr() + (nint)counterSlot * (nint)4));
			}

			public static void ConvertLightToGPUFormat(LightCategory lightCategory, GPULightType gpuLightType, in CreateGpuLightDataJobGlobalConfig globalConfig, LightShadowCasterMode visibleLightShadowCasterMode, in LightBakingOutput visibleLightBakingOutput, in VisibleLight light, in HDProcessedVisibleLight processedEntity, in HDLightRenderData lightRenderData, out Vector3 lightDimensions, ref LightData lightData)
			{
				lightData.lightLayers = GetLightLayer(globalConfig.lightLayersEnabled, in lightRenderData);
				lightData.lightType = gpuLightType;
				VisibleLightExtensionMethods.VisibleLightAxisAndPosition axisAndPosition = light.GetAxisAndPosition();
				lightData.positionRWS = axisAndPosition.Position;
				lightData.range = light.range;
				if (lightRenderData.applyRangeAttenuation)
				{
					lightData.rangeAttenuationScale = 1f / (light.range * light.range);
					lightData.rangeAttenuationBias = 1f;
					if (lightData.lightType == GPULightType.Rectangle)
					{
						lightData.rangeAttenuationScale = 1f;
					}
				}
				else
				{
					lightData.rangeAttenuationScale = 4096f / (light.range * light.range);
					lightData.rangeAttenuationBias = 16777216f;
					if (lightData.lightType == GPULightType.Rectangle)
					{
						lightData.rangeAttenuationScale = 4096f;
					}
				}
				float shapeWidth = lightRenderData.shapeWidth;
				float shapeHeight = lightRenderData.shapeHeight;
				lightData.color = GetLightColor(in light);
				lightData.forward = axisAndPosition.Forward;
				lightData.up = axisAndPosition.Up;
				lightData.right = axisAndPosition.Right;
				lightDimensions.x = shapeWidth;
				lightDimensions.y = shapeHeight;
				lightDimensions.z = light.range;
				lightData.boxLightSafeExtent = 1f;
				if (lightData.lightType == GPULightType.ProjectorBox)
				{
					lightData.right *= 2f / Mathf.Max(shapeWidth, 0.001f);
					lightData.up *= 2f / Mathf.Max(shapeHeight, 0.001f);
				}
				else if (lightData.lightType == GPULightType.ProjectorPyramid)
				{
					float spotAngle = light.spotAngle;
					float aspectRatio = lightRenderData.aspectRatio;
					float num;
					float num2;
					if (aspectRatio >= 1f)
					{
						num = 2f * Mathf.Tan(spotAngle * 0.5f * (MathF.PI / 180f));
						num2 = num * aspectRatio;
					}
					else
					{
						num2 = 2f * Mathf.Tan(spotAngle * 0.5f * (MathF.PI / 180f));
						num = num2 / aspectRatio;
					}
					lightDimensions.x = num2;
					lightDimensions.y = num;
					lightData.right *= 2f / num2;
					lightData.up *= 2f / num;
				}
				if (lightData.lightType == GPULightType.Spot)
				{
					float spotAngle2 = light.spotAngle;
					float num3 = lightRenderData.innerSpotPercent / 100f;
					float num4 = Mathf.Clamp(Mathf.Cos(spotAngle2 * 0.5f * (MathF.PI / 180f)), 0f, 1f);
					float num5 = Mathf.Sqrt(1f - num4 * num4);
					float num6 = Mathf.Clamp(Mathf.Cos(spotAngle2 * 0.5f * num3 * (MathF.PI / 180f)), 0f, 1f);
					float num7 = Mathf.Max(0.0001f, num6 - num4);
					lightData.angleScale = 1f / num7;
					lightData.angleOffset = (0f - num4) * lightData.angleScale;
					lightData.iesCut = lightRenderData.spotIESCutoffPercent / 100f;
					float num8 = num4 / num5;
					lightData.up *= num8;
					lightData.right *= num8;
				}
				else
				{
					lightData.angleScale = 0f;
					lightData.angleOffset = 1f;
					lightData.iesCut = 1f;
				}
				float shapeRadius = lightRenderData.shapeRadius;
				if (lightData.lightType != 0 && lightData.lightType != GPULightType.ProjectorBox)
				{
					lightData.size = new Vector4(shapeRadius * shapeRadius, 0f, 0f, 0f);
				}
				if (lightData.lightType == GPULightType.Rectangle || lightData.lightType == GPULightType.Tube)
				{
					lightData.size = new Vector4(shapeWidth, shapeHeight, Mathf.Cos(lightRenderData.barnDoorAngle * MathF.PI / 180f), lightRenderData.barnDoorLength);
				}
				float lightDimmer = lightRenderData.lightDimmer;
				lightData.lightDimmer = processedEntity.lightDistanceFade * lightDimmer;
				lightData.diffuseDimmer = processedEntity.lightDistanceFade * (lightRenderData.affectDiffuse ? lightDimmer : 0f);
				lightData.specularDimmer = processedEntity.lightDistanceFade * (lightRenderData.affectSpecular ? (lightDimmer * globalConfig.specularGlobalDimmer) : 0f);
				lightData.volumetricLightDimmer = Mathf.Min(processedEntity.lightVolumetricDistanceFade, processedEntity.lightDistanceFade) * (lightRenderData.affectVolumetric ? lightRenderData.volumetricDimmer : 0f);
				lightData.cookieMode = CookieMode.None;
				lightData.shadowIndex = -1;
				lightData.screenSpaceShadowIndex = globalConfig.invalidScreenSpaceShadowIndex;
				lightData.isRayTracedContactShadow = 0f;
				float distanceToCamera = processedEntity.distanceToCamera;
				float shadowFadeDistance = lightRenderData.shadowFadeDistance;
				float shadowDimmer = lightRenderData.shadowDimmer;
				float num9 = (lightRenderData.affectVolumetric ? lightRenderData.volumetricShadowDimmer : 0f);
				float num10 = HDUtils.ComputeLinearDistanceFade(distanceToCamera, Mathf.Min(globalConfig.maxShadowFadeDistance, shadowFadeDistance));
				lightData.shadowDimmer = num10 * shadowDimmer;
				lightData.volumetricShadowDimmer = num10 * num9;
				Color shadowTint = lightRenderData.shadowTint;
				bool flag = lightRenderData.penumbraTint && (shadowTint.r != shadowTint.g || shadowTint.g != shadowTint.b);
				lightData.penumbraTint = (flag ? 1f : 0f);
				if (flag)
				{
					lightData.shadowTint = new Vector3(Mathf.Pow(shadowTint.r, 2.2f), Mathf.Pow(shadowTint.g, 2.2f), Mathf.Pow(shadowTint.b, 2.2f));
				}
				else
				{
					lightData.shadowTint = new Vector3(shadowTint.r, shadowTint.g, shadowTint.b);
				}
				float num11 = Mathf.Clamp01(1.1725f / (1.01f + Mathf.Pow(1f * (shapeRadius + 0.1f), 2f)) - 0.15f);
				lightData.minRoughness = (1f - num11) * (1f - num11);
				lightData.shadowMaskSelector = Vector4.zero;
				if (processedEntity.isBakedShadowMask)
				{
					lightData.shadowMaskSelector[visibleLightBakingOutput.occlusionMaskChannel] = 1f;
					lightData.nonLightMappedOnly = ((visibleLightShadowCasterMode == LightShadowCasterMode.NonLightmappedOnly) ? 1 : 0);
				}
				else
				{
					lightData.shadowMaskSelector.x = -1f;
					lightData.nonLightMappedOnly = 0;
				}
			}

			private void StoreAndConvertLightToGPUFormat(int outputIndex, int lightIndex, LightCategory lightCategory, GPULightType gpuLightType, LightVolumeType lightVolumeType)
			{
				VisibleLight light = visibleLights[lightIndex];
				HDProcessedVisibleLight processedEntity = processedEntities[lightIndex];
				LightData lightData = default(LightData);
				ref HDLightRenderData lightData2 = ref GetLightData(processedEntity.dataIndex);
				ref CreateGpuLightDataJobGlobalConfig reference = ref globalConfig;
				LightShadowCasterMode num = visibleLightShadowCasterMode[lightIndex];
				LightBakingOutput lightBakingOutput = visibleLightBakingOutput[lightIndex];
				ConvertLightToGPUFormat(lightCategory, gpuLightType, in reference, num, in lightBakingOutput, in light, in processedEntity, in lightData2, out var lightDimensions, ref lightData);
				for (int i = 0; i < viewCounts; i++)
				{
					LightsPerView lightsPerView = this.lightsPerView[i];
					ComputeLightVolumeDataAndBound(lightCategory, gpuLightType, lightVolumeType, in light, in lightData, in lightDimensions, in lightsPerView.worldToView, outputIndex + lightsPerView.boundsOffset);
				}
				if (useCameraRelativePosition)
				{
					lightData.positionRWS -= cameraPos;
				}
				switch (lightCategory)
				{
				case LightCategory.Punctual:
					IncrementCounter(GPULightTypeCountSlots.Punctual);
					break;
				case LightCategory.Area:
					IncrementCounter(GPULightTypeCountSlots.Area);
					break;
				}
				lights[outputIndex] = lightData;
			}

			private void ComputeLightVolumeDataAndBound(LightCategory lightCategory, GPULightType gpuLightType, LightVolumeType lightVolumeType, in VisibleLight light, in LightData lightData, in Vector3 lightDimensions, in Matrix4x4 worldToView, int outputIndex)
			{
				float z = lightDimensions.z;
				Matrix4x4 localToWorldMatrix = light.localToWorldMatrix;
				Vector3 positionRWS = lightData.positionRWS;
				Vector3 vector = worldToView.MultiplyPoint(positionRWS);
				Vector3 vector2 = worldToView.MultiplyVector(localToWorldMatrix.GetColumn(0));
				Vector3 vector3 = worldToView.MultiplyVector(localToWorldMatrix.GetColumn(1));
				Vector3 vector4 = worldToView.MultiplyVector(localToWorldMatrix.GetColumn(2));
				SFiniteLightBound value = default(SFiniteLightBound);
				LightVolumeData value2 = default(LightVolumeData);
				value2.lightCategory = (uint)lightCategory;
				value2.lightVolume = (uint)lightVolumeType;
				switch (gpuLightType)
				{
				case GPULightType.Spot:
				case GPULightType.ProjectorPyramid:
				{
					Vector3 vector15 = localToWorldMatrix.GetColumn(2);
					Vector3 vector16 = vector2;
					Vector3 vector17 = vector3;
					Vector3 vector18 = vector4;
					float spotAngle = light.spotAngle;
					float num2 = Mathf.Cos(0.5f * spotAngle * (MathF.PI / 180f));
					float num3 = Mathf.Sin(0.5f * spotAngle * (MathF.PI / 180f));
					if (gpuLightType == GPULightType.ProjectorPyramid)
					{
						Vector3 value3 = 0.5f * lightDimensions.x * vector16 + 0.5f * lightDimensions.y * vector17 + 1f * vector18;
						num2 = Vector3.Dot(vector18, Vector3.Normalize(value3));
						num3 = Mathf.Sqrt(1f - num2 * num2);
					}
					float num4 = ((num2 > 0f) ? (num3 / num2) : float.MaxValue);
					float cotan = ((num3 > 0f) ? (num2 / num3) : float.MaxValue);
					bool flag = true;
					float num5 = (flag ? num4 : num3);
					value.center = worldToView.MultiplyPoint(positionRWS + 0.5f * z * vector15);
					value.boxAxisX = num5 * z * vector16;
					value.boxAxisY = num5 * z * vector17;
					value.boxAxisZ = 0.5f * z * vector18;
					float num6 = num3;
					float num7 = num2 - 0.5f;
					num6 *= z;
					float num8 = num7 * z;
					float num9 = Mathf.Sqrt(num8 * num8 + 1f * num6 * num6);
					value.radius = ((num9 > 0.5f * z) ? num9 : (0.5f * z));
					value.scaleXY = (flag ? 0.01f : 1f);
					value2.lightAxisX = vector16;
					value2.lightAxisY = vector17;
					value2.lightAxisZ = vector18;
					value2.lightPos = vector;
					value2.radiusSq = z * z;
					value2.cotan = cotan;
					value2.featureFlags = 4096u;
					break;
				}
				case GPULightType.Point:
				{
					Vector3 vector12 = new Vector3(1f, 0f, 0f);
					Vector3 vector13 = new Vector3(0f, 1f, 0f);
					Vector3 vector14 = new Vector3(0f, 0f, 1f);
					value.center = vector;
					value.boxAxisX = vector12 * z;
					value.boxAxisY = vector13 * z;
					value.boxAxisZ = vector14 * z;
					value.scaleXY = 1f;
					value.radius = z;
					value2.lightAxisX = vector12;
					value2.lightAxisY = vector13;
					value2.lightAxisZ = vector14;
					value2.lightPos = value.center;
					value2.radiusSq = z * z;
					value2.featureFlags = 4096u;
					break;
				}
				case GPULightType.Tube:
				{
					Vector3 vector10 = new Vector3(lightDimensions.x + 2f * z, 2f * z, 2f * z);
					Vector3 vector11 = 0.5f * vector10;
					Vector3 lightPos2 = (value.center = vector);
					value.boxAxisX = vector11.x * vector2;
					value.boxAxisY = vector11.y * vector3;
					value.boxAxisZ = vector11.z * vector4;
					value.radius = vector11.x;
					value.scaleXY = 1f;
					value2.lightPos = lightPos2;
					value2.lightAxisX = vector2;
					value2.lightAxisY = vector3;
					value2.lightAxisZ = vector4;
					value2.boxInvRange.Set(1f / vector11.x, 1f / vector11.y, 1f / vector11.z);
					value2.featureFlags = 8192u;
					break;
				}
				case GPULightType.Rectangle:
				{
					Vector3 vector7 = new Vector3(lightDimensions.x + 2f * z, lightDimensions.y + 2f * z, z);
					Vector3 vector8 = 0.5f * vector7;
					Vector3 vector9 = vector + vector8.z * vector4;
					float num = z + 0.5f * Mathf.Sqrt(lightDimensions.x * lightDimensions.x + lightDimensions.y * lightDimensions.y);
					value.center = vector9;
					value.boxAxisX = vector8.x * vector2;
					value.boxAxisY = vector8.y * vector3;
					value.boxAxisZ = vector8.z * vector4;
					value.radius = Mathf.Sqrt(num * num + 0.5f * z * (0.5f * z));
					value.scaleXY = 1f;
					value2.lightPos = vector9;
					value2.lightAxisX = vector2;
					value2.lightAxisY = vector3;
					value2.lightAxisZ = vector4;
					value2.boxInvRange.Set(1f / vector8.x, 1f / vector8.y, 1f / vector8.z);
					value2.featureFlags = 8192u;
					break;
				}
				case GPULightType.ProjectorBox:
				{
					Vector3 vector5 = new Vector3(lightDimensions.x, lightDimensions.y, z);
					Vector3 vector6 = 0.5f * vector5;
					Vector3 lightPos = (value.center = vector + vector6.z * vector4);
					value.boxAxisX = vector6.x * vector2;
					value.boxAxisY = vector6.y * vector3;
					value.boxAxisZ = vector6.z * vector4;
					value.radius = vector6.magnitude;
					value.scaleXY = 1f;
					value2.lightPos = lightPos;
					value2.lightAxisX = vector2;
					value2.lightAxisY = vector3;
					value2.lightAxisZ = vector4;
					value2.boxInvRange.Set(1f / vector6.x, 1f / vector6.y, 1f / vector6.z);
					value2.featureFlags = 4096u;
					break;
				}
				default:
					_ = 7;
					break;
				}
				lightBounds[outputIndex] = value;
				lightVolumes[outputIndex] = value2;
			}

			private void ConvertDirectionalLightToGPUFormat(int outputIndex, int lightIndex, LightCategory lightCategory, GPULightType gpuLightType, LightVolumeType lightVolumeType)
			{
				VisibleLight light = visibleLights[lightIndex];
				HDProcessedVisibleLight hDProcessedVisibleLight = processedEntities[lightIndex];
				int dataIndex = hDProcessedVisibleLight.dataIndex;
				DirectionalLightData value = default(DirectionalLightData);
				ref HDLightRenderData lightData = ref GetLightData(dataIndex);
				value.lightLayers = GetLightLayer(globalConfig.lightLayersEnabled, in lightData);
				value.forward = light.GetForward();
				value.color = GetLightColor(in light);
				value.color *= ((defaultDataIndex == dataIndex) ? MathF.PI : 1f);
				value.lightDimmer = lightData.lightDimmer;
				value.diffuseDimmer = (lightData.affectDiffuse ? value.lightDimmer : 0f);
				value.specularDimmer = (lightData.affectSpecular ? (value.lightDimmer * globalConfig.specularGlobalDimmer) : 0f);
				value.volumetricLightDimmer = (lightData.affectVolumetric ? lightData.volumetricDimmer : 0f);
				value.shadowIndex = -1;
				value.screenSpaceShadowIndex = globalConfig.invalidScreenSpaceShadowIndex;
				value.isRayTracedContactShadow = 0f;
				value.right = light.GetRight() * 2f / Mathf.Max(lightData.shapeWidth, 0.001f);
				value.up = light.GetUp() * 2f / Mathf.Max(lightData.shapeHeight, 0.001f);
				value.positionRWS = light.GetPosition();
				value.shadowDimmer = lightData.shadowDimmer;
				float volumetricShadowDimmer = (lightData.affectVolumetric ? lightData.volumetricShadowDimmer : 0f);
				value.volumetricShadowDimmer = volumetricShadowDimmer;
				Color shadowTint = lightData.shadowTint;
				bool flag = lightData.penumbraTint && (shadowTint.r != shadowTint.g || shadowTint.g != shadowTint.b);
				value.penumbraTint = (flag ? 1f : 0f);
				if (flag)
				{
					value.shadowTint = new Vector3(shadowTint.r * shadowTint.r, shadowTint.g * shadowTint.g, shadowTint.b * shadowTint.b);
				}
				else
				{
					value.shadowTint = new Vector3(shadowTint.r, shadowTint.g, shadowTint.b);
				}
				float num = Mathf.Clamp01(1.35f / (1f + Mathf.Pow(1.15f * (0.0315f * lightData.angularDiameter + 0.4f), 2f)) - 0.11f);
				value.minRoughness = (1f - num) * (1f - num);
				value.shadowMaskSelector = Vector4.zero;
				if (hDProcessedVisibleLight.isBakedShadowMask)
				{
					LightBakingOutput lightBakingOutput = visibleLightBakingOutput[lightIndex];
					value.shadowMaskSelector[lightBakingOutput.occlusionMaskChannel] = 1f;
					value.nonLightMappedOnly = ((visibleLightShadowCasterMode[lightIndex] == LightShadowCasterMode.NonLightmappedOnly) ? 1 : 0);
					float num2 = maxShadowDistance * maxShadowDistance;
					float num3 = shadowOutBorderDistance;
					if (num3 < 0.0001f)
					{
						value.cascadesBorderFadeScaleBias = new Vector2(1000000f, (0f - num2) * 1000000f);
					}
					else
					{
						num3 = 1f - num3;
						num3 *= num3;
						float num4 = num3 * num2;
						value.cascadesBorderFadeScaleBias.x = 1f / (num2 - num4);
						value.cascadesBorderFadeScaleBias.y = (0f - num4) / (num2 - num4);
					}
				}
				else
				{
					value.shadowMaskSelector.x = -1f;
					value.nonLightMappedOnly = 0;
				}
				bool num5 = isPbrSkyActive && lightData.interactsWithSky;
				value.distanceFromCamera = -1f;
				if (num5)
				{
					value.distanceFromCamera = lightData.distance;
					if (precomputedAtmosphericAttenuation != 0)
					{
						float num6 = airScaleHeight;
						float num7 = aerosolScaleHeight;
						ref Vector3 reference = ref airExtinctionCoefficient;
						float num8 = aerosolExtinctionCoefficient;
						ref Vector3 c = ref planetCenterPosition;
						float r = planetaryRadius;
						Vector3 L = -value.forward;
						Vector3 vector = PhysicallyBasedSky.EvaluateAtmosphericAttenuation(num6, num7, in reference, num8, in c, r, in L, in cameraPos);
						value.color.x *= vector.x;
						value.color.y *= vector.y;
						value.color.z *= vector.z;
					}
				}
				value.angularDiameter = lightData.angularDiameter * (MathF.PI / 180f);
				value.flareSize = Mathf.Max(lightData.flareSize * (MathF.PI / 180f), 5.9604645E-08f);
				value.flareFalloff = lightData.flareFalloff;
				float num9 = 0.5f * value.angularDiameter;
				value.flareCosInner = Mathf.Cos(num9);
				value.flareCosOuter = Mathf.Cos(num9 + value.flareSize);
				value.flareTint = (Vector4)lightData.flareTint;
				value.surfaceTint = (Vector4)lightData.surfaceTint;
				if (useCameraRelativePosition)
				{
					value.positionRWS -= cameraPos;
				}
				IncrementCounter(GPULightTypeCountSlots.Directional);
				directionalLights[outputIndex] = value;
			}

			public void Execute(int index)
			{
				UnpackLightSortKey(sortKeys[index], out var lightCategory, out var gpuLightType, out var lightVolumeType, out var lightIndex);
				if (gpuLightType == GPULightType.Directional)
				{
					ConvertDirectionalLightToGPUFormat(index, lightIndex, lightCategory, gpuLightType, lightVolumeType);
					return;
				}
				int outputIndex = index - directionalSortedLightCounts;
				StoreAndConvertLightToGPUFormat(outputIndex, lightIndex, lightCategory, gpuLightType, lightVolumeType);
			}
		}

		public const int ArrayCapacity = 100;

		private NativeArray<LightsPerView> m_LightsPerView;

		private int m_LighsPerViewCapacity;

		private int m_LightsPerViewCount;

		private NativeArray<SFiniteLightBound> m_LightBounds;

		private NativeArray<LightVolumeData> m_LightVolumes;

		private int m_LightBoundsCapacity;

		private int m_LightBoundsCount;

		private NativeArray<LightData> m_Lights;

		private int m_LightCapacity;

		private int m_LightCount;

		private NativeArray<DirectionalLightData> m_DirectionalLights;

		private int m_DirectionalLightCapacity;

		private int m_DirectionalLightCount;

		private NativeArray<int> m_LightTypeCounters;

		private HDRenderPipelineAsset m_Asset;

		private HDShadowManager m_ShadowManager;

		private HDRenderPipeline.LightLoopTextureCaches m_TextureCaches;

		private HashSet<HDAdditionalLightData> m_ScreenSpaceShadowsUnion = new HashSet<HDAdditionalLightData>();

		private int m_CurrentShadowSortedSunLightIndex = -1;

		private HDProcessedVisibleLightsBuilder.ShadowMapFlags m_CurrentSunShadowMapFlags;

		private DirectionalLightData m_CurrentSunLightDirectionalLightData;

		private int m_ContactShadowIndex;

		private int m_ScreenSpaceShadowIndex;

		private int m_ScreenSpaceShadowChannelSlot;

		private int m_DebugSelectedLightShadowIndex;

		private int m_DebugSelectedLightShadowCount;

		private HDRenderPipeline.ScreenSpaceShadowData[] m_CurrentScreenSpaceShadowData;

		private int m_BoundsEyeDataOffset;

		private JobHandle m_CreateGpuLightDataJobHandle;

		public NativeArray<LightData> lights => m_Lights;

		public int lightsCount => m_LightCount;

		public NativeArray<DirectionalLightData> directionalLights => m_DirectionalLights;

		public int directionalLightCount
		{
			get
			{
				if (!m_LightTypeCounters.IsCreated)
				{
					return 0;
				}
				return m_LightTypeCounters[0];
			}
		}

		public int punctualLightCount
		{
			get
			{
				if (!m_LightTypeCounters.IsCreated)
				{
					return 0;
				}
				return m_LightTypeCounters[1];
			}
		}

		public int areaLightCount
		{
			get
			{
				if (!m_LightTypeCounters.IsCreated)
				{
					return 0;
				}
				return m_LightTypeCounters[2];
			}
		}

		public NativeArray<LightsPerView> lightsPerView => m_LightsPerView;

		public NativeArray<SFiniteLightBound> lightBounds => m_LightBounds;

		public NativeArray<LightVolumeData> lightVolumes => m_LightVolumes;

		public int lightsPerViewCount => m_LightsPerViewCount;

		public int lightBoundsCount => m_LightBoundsCount;

		public int boundsEyeDataOffset => m_BoundsEyeDataOffset;

		public int allLightBoundsCount => m_BoundsEyeDataOffset * lightsPerViewCount;

		public int currentShadowSortedSunLightIndex => m_CurrentShadowSortedSunLightIndex;

		public HDProcessedVisibleLightsBuilder.ShadowMapFlags currentSunShadowMapFlags => m_CurrentSunShadowMapFlags;

		public DirectionalLightData currentSunLightDirectionalLightData => m_CurrentSunLightDirectionalLightData;

		public int contactShadowIndex => m_ContactShadowIndex;

		public int screenSpaceShadowIndex => m_ScreenSpaceShadowIndex;

		public int screenSpaceShadowChannelSlot => m_ScreenSpaceShadowChannelSlot;

		public int debugSelectedLightShadowIndex => m_DebugSelectedLightShadowIndex;

		public int debugSelectedLightShadowCount => m_DebugSelectedLightShadowCount;

		public HDRenderPipeline.ScreenSpaceShadowData[] currentScreenSpaceShadowData => m_CurrentScreenSpaceShadowData;

		public static uint PackLightSortKey(LightCategory lightCategory, GPULightType gpuLightType, LightVolumeType lightVolumeType, int lightIndex)
		{
			return (uint)((((gpuLightType != 0) ? 1 : 0) << 31) | ((int)lightCategory << 27) | ((int)gpuLightType << 22) | ((int)lightVolumeType << 17) | lightIndex);
		}

		public static void UnpackLightSortKey(uint sortKey, out LightCategory lightCategory, out GPULightType gpuLightType, out LightVolumeType lightVolumeType, out int lightIndex)
		{
			lightCategory = (LightCategory)((int)(sortKey >> 27) & 0xF);
			gpuLightType = (GPULightType)((int)(sortKey >> 22) & 0x1F);
			lightVolumeType = (LightVolumeType)((int)(sortKey >> 17) & 0x1F);
			lightIndex = (int)(sortKey & 0xFFFF);
		}

		public void Initialize(HDRenderPipelineAsset asset, HDShadowManager shadowManager, HDRenderPipeline.LightLoopTextureCaches textureCaches)
		{
			m_Asset = asset;
			m_TextureCaches = textureCaches;
			m_ShadowManager = shadowManager;
			int num = Math.Max(m_Asset.currentPlatformRenderPipelineSettings.hdShadowInitParams.maxScreenSpaceShadowSlots, 1);
			m_CurrentScreenSpaceShadowData = new HDRenderPipeline.ScreenSpaceShadowData[num];
			AllocateLightData(0, 0);
		}

		public void AddLightBounds(int viewId, in SFiniteLightBound lightBound, in LightVolumeData volumeData)
		{
			LightsPerView value = m_LightsPerView[viewId];
			m_LightBounds[value.boundsOffset + value.boundsCount] = lightBound;
			m_LightVolumes[value.boundsOffset + value.boundsCount] = volumeData;
			value.boundsCount++;
			m_LightsPerView[viewId] = value;
		}

		public void Cleanup()
		{
			if (m_Lights.IsCreated)
			{
				m_Lights.Dispose();
			}
			if (m_DirectionalLights.IsCreated)
			{
				m_DirectionalLights.Dispose();
			}
			if (m_LightsPerView.IsCreated)
			{
				m_LightsPerView.Dispose();
			}
			if (m_LightBounds.IsCreated)
			{
				m_LightBounds.Dispose();
			}
			if (m_LightVolumes.IsCreated)
			{
				m_LightVolumes.Dispose();
			}
			if (m_LightTypeCounters.IsCreated)
			{
				m_LightTypeCounters.Dispose();
			}
		}

		private void AllocateLightData(int lightCount, int directionalLightCount)
		{
			int num = Math.Max(1, lightCount);
			if (num > m_LightCapacity)
			{
				m_LightCapacity = Math.Max(Math.Max(m_LightCapacity * 2, num), 100);
				ArrayExtensions.ResizeArray(ref m_Lights, m_LightCapacity);
			}
			m_LightCount = lightCount;
			int num2 = Math.Max(1, directionalLightCount);
			if (num2 > m_DirectionalLightCapacity)
			{
				m_DirectionalLightCapacity = Math.Max(Math.Max(m_DirectionalLightCapacity * 2, num2), 100);
				ArrayExtensions.ResizeArray(ref m_DirectionalLights, m_DirectionalLightCapacity);
			}
			m_DirectionalLightCount = directionalLightCount;
		}

		public void StartCreateGpuLightDataJob(HDCamera hdCamera, in CullingResults cullingResult, HDShadowSettings hdShadowSettings, HDProcessedVisibleLightsBuilder visibleLights, HDLightRenderDatabase lightEntities)
		{
			VisualEnvironment component = hdCamera.volumeStack.GetComponent<VisualEnvironment>();
			PhysicallyBasedSky component2 = hdCamera.volumeStack.GetComponent<PhysicallyBasedSky>();
			HDShadowSettings component3 = hdCamera.volumeStack.GetComponent<HDShadowSettings>();
			bool isPbrSkyActive = component.skyType.value == 4;
			CreateGpuLightDataJob createGpuLightDataJob = default(CreateGpuLightDataJob);
			createGpuLightDataJob.totalLightCounts = lightEntities.lightCount;
			createGpuLightDataJob.outputLightCounts = m_LightCount;
			createGpuLightDataJob.outputDirectionalLightCounts = m_DirectionalLightCount;
			createGpuLightDataJob.outputLightBoundsCount = m_LightBoundsCount;
			createGpuLightDataJob.globalConfig = CreateGpuLightDataJobGlobalConfig.Create(hdCamera, hdShadowSettings);
			createGpuLightDataJob.cameraPos = hdCamera.mainViewConstants.worldSpaceCameraPos;
			createGpuLightDataJob.directionalSortedLightCounts = visibleLights.sortedDirectionalLightCounts;
			createGpuLightDataJob.isPbrSkyActive = isPbrSkyActive;
			createGpuLightDataJob.precomputedAtmosphericAttenuation = ShaderConfig.s_PrecomputedAtmosphericAttenuation;
			createGpuLightDataJob.defaultDataIndex = lightEntities.GetEntityDataIndex(lightEntities.GetDefaultLightEntity());
			createGpuLightDataJob.viewCounts = hdCamera.viewCount;
			createGpuLightDataJob.useCameraRelativePosition = ShaderConfig.s_CameraRelativeRendering != 0;
			createGpuLightDataJob.planetCenterPosition = component2.GetPlanetCenterPosition(hdCamera.camera.transform.position);
			createGpuLightDataJob.planetaryRadius = component2.GetPlanetaryRadius();
			createGpuLightDataJob.airScaleHeight = component2.GetAirScaleHeight();
			createGpuLightDataJob.aerosolScaleHeight = component2.GetAerosolScaleHeight();
			createGpuLightDataJob.airExtinctionCoefficient = component2.GetAirExtinctionCoefficient();
			createGpuLightDataJob.aerosolExtinctionCoefficient = component2.GetAerosolExtinctionCoefficient();
			createGpuLightDataJob.maxShadowDistance = component3.maxShadowDistance.value;
			createGpuLightDataJob.shadowOutBorderDistance = component3.cascadeShadowBorders[component3.cascadeShadowSplitCount.value - 1];
			createGpuLightDataJob.lightRenderDataArray = lightEntities.lightData;
			createGpuLightDataJob.sortKeys = visibleLights.sortKeys;
			createGpuLightDataJob.processedEntities = visibleLights.processedEntities;
			createGpuLightDataJob.visibleLights = cullingResult.visibleLights;
			createGpuLightDataJob.visibleLightBakingOutput = visibleLights.visibleLightBakingOutput;
			createGpuLightDataJob.visibleLightShadowCasterMode = visibleLights.visibleLightShadowCasterMode;
			createGpuLightDataJob.gpuLightCounters = m_LightTypeCounters;
			createGpuLightDataJob.lights = m_Lights;
			createGpuLightDataJob.directionalLights = m_DirectionalLights;
			createGpuLightDataJob.lightsPerView = m_LightsPerView;
			createGpuLightDataJob.lightBounds = m_LightBounds;
			createGpuLightDataJob.lightVolumes = m_LightVolumes;
			CreateGpuLightDataJob jobData = createGpuLightDataJob;
			m_CreateGpuLightDataJobHandle = IJobParallelForExtensions.Schedule(jobData, visibleLights.sortedLightCounts, 32);
		}

		public void CompleteGpuLightDataJob()
		{
			m_CreateGpuLightDataJobHandle.Complete();
		}

		public void NewFrame(HDCamera hdCamera, int maxLightCount)
		{
			int viewCount = hdCamera.viewCount;
			if (viewCount > m_LighsPerViewCapacity)
			{
				m_LighsPerViewCapacity = viewCount;
				ArrayExtensions.ResizeArray(ref m_LightsPerView, m_LighsPerViewCapacity);
			}
			m_LightsPerViewCount = viewCount;
			int val = maxLightCount * viewCount;
			int num = Math.Max(val, 1);
			if (num > m_LightBoundsCapacity)
			{
				m_LightBoundsCapacity = Math.Max(Math.Max(m_LightBoundsCapacity * 2, num), 100);
				ArrayExtensions.ResizeArray(ref m_LightBounds, m_LightBoundsCapacity);
				ArrayExtensions.ResizeArray(ref m_LightVolumes, m_LightBoundsCapacity);
			}
			m_LightBoundsCount = val;
			m_BoundsEyeDataOffset = maxLightCount;
			for (int i = 0; i < viewCount; i++)
			{
				m_LightsPerView[i] = new LightsPerView
				{
					worldToView = HDRenderPipeline.GetWorldToViewMatrix(hdCamera, i),
					boundsOffset = i * m_BoundsEyeDataOffset,
					boundsCount = 0
				};
			}
			if (!m_LightTypeCounters.IsCreated)
			{
				ArrayExtensions.ResizeArray(ref m_LightTypeCounters, Enum.GetValues(typeof(GPULightTypeCountSlots)).Length);
			}
			m_LightCount = 0;
			m_ContactShadowIndex = 0;
			m_ScreenSpaceShadowIndex = 0;
			m_ScreenSpaceShadowChannelSlot = 0;
			m_ScreenSpaceShadowsUnion.Clear();
			m_CurrentShadowSortedSunLightIndex = -1;
			m_CurrentSunShadowMapFlags = HDProcessedVisibleLightsBuilder.ShadowMapFlags.None;
			m_DebugSelectedLightShadowIndex = -1;
			m_DebugSelectedLightShadowCount = 0;
			for (int j = 0; j < m_Asset.currentPlatformRenderPipelineSettings.hdShadowInitParams.maxScreenSpaceShadowSlots; j++)
			{
				m_CurrentScreenSpaceShadowData[j].additionalLightData = null;
				m_CurrentScreenSpaceShadowData[j].lightDataIndex = -1;
				m_CurrentScreenSpaceShadowData[j].valid = false;
			}
			for (int k = 0; k < m_LightTypeCounters.Length; k++)
			{
				m_LightTypeCounters[k] = 0;
			}
		}

		public void Build(CommandBuffer cmd, HDCamera hdCamera, in CullingResults cullingResult, HDProcessedVisibleLightsBuilder visibleLights, HDLightRenderDatabase lightEntities, in HDShadowInitParameters shadowInitParams, DebugDisplaySettings debugDisplaySettings)
		{
			m_ShadowManager.LayoutShadowMaps(debugDisplaySettings.data.lightingDebugSettings);
			m_TextureCaches.lightCookieManager.LayoutIfNeeded();
			int sortedLightCounts = visibleLights.sortedLightCounts;
			int sortedNonDirectionalLightCounts = visibleLights.sortedNonDirectionalLightCounts;
			int sortedDirectionalLightCounts = visibleLights.sortedDirectionalLightCounts;
			AllocateLightData(sortedNonDirectionalLightCounts, sortedDirectionalLightCounts);
			if (sortedLightCounts > 0)
			{
				for (int i = 0; i < hdCamera.viewCount; i++)
				{
					LightsPerView value = m_LightsPerView[i];
					value.boundsCount += sortedNonDirectionalLightCounts;
					m_LightsPerView[i] = value;
				}
				HDShadowSettings component = hdCamera.volumeStack.GetComponent<HDShadowSettings>();
				StartCreateGpuLightDataJob(hdCamera, in cullingResult, component, visibleLights, lightEntities);
				CompleteGpuLightDataJob();
				CalculateAllLightDataTextureInfo(cmd, hdCamera, in cullingResult, visibleLights, lightEntities, component, in shadowInitParams, debugDisplaySettings);
			}
		}

		public void ProcessLightDataShadowIndex(CommandBuffer cmd, in HDShadowInitParameters shadowInitParams, HDLightType lightType, Light lightComponent, HDAdditionalLightData additionalLightData, int shadowIndex, ref LightData lightData)
		{
			if (lightData.lightType == GPULightType.ProjectorBox && shadowIndex >= 0)
			{
				float value = additionalLightData.shadowResolution.Value(shadowInitParams.shadowResolutionPunctual);
				value = Mathf.Clamp(value, 128f, 2048f);
				float num = Mathf.Lerp(0.05f, 0.01f, Mathf.Max(value / 2048f, 0f));
				lightData.boxLightSafeExtent = 1f - num;
			}
			if (lightComponent != null && ((lightType == HDLightType.Spot && (lightComponent.cookie != null || additionalLightData.IESPoint != null)) || (lightType == HDLightType.Area && lightData.lightType == GPULightType.Rectangle && (lightComponent.cookie != null || additionalLightData.IESSpot != null)) || (lightType == HDLightType.Point && (lightComponent.cookie != null || additionalLightData.IESPoint != null))))
			{
				switch (lightType)
				{
				case HDLightType.Spot:
				{
					Texture cookie = lightComponent.cookie;
					lightData.cookieMode = (((object)cookie == null || cookie.wrapMode != 0) ? CookieMode.Clamp : CookieMode.Repeat);
					if (additionalLightData.IESSpot != null && lightComponent.cookie != null && additionalLightData.IESSpot != lightComponent.cookie)
					{
						lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.Fetch2DCookie(cmd, lightComponent.cookie, additionalLightData.IESSpot);
					}
					else if (lightComponent.cookie != null)
					{
						lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.Fetch2DCookie(cmd, lightComponent.cookie);
					}
					else if (additionalLightData.IESSpot != null)
					{
						lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.Fetch2DCookie(cmd, additionalLightData.IESSpot);
					}
					else
					{
						lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.Fetch2DCookie(cmd, Texture2D.whiteTexture);
					}
					break;
				}
				case HDLightType.Point:
					lightData.cookieMode = CookieMode.Repeat;
					if (additionalLightData.IESPoint != null && lightComponent.cookie != null && additionalLightData.IESPoint != lightComponent.cookie)
					{
						lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.FetchCubeCookie(cmd, lightComponent.cookie, additionalLightData.IESPoint);
					}
					else if (lightComponent.cookie != null)
					{
						lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.FetchCubeCookie(cmd, lightComponent.cookie);
					}
					else if (additionalLightData.IESPoint != null)
					{
						lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.FetchCubeCookie(cmd, additionalLightData.IESPoint);
					}
					break;
				case HDLightType.Area:
					lightData.cookieMode = CookieMode.Clamp;
					if (additionalLightData.areaLightCookie != null && additionalLightData.IESSpot != null && additionalLightData.areaLightCookie != additionalLightData.IESSpot)
					{
						lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.FetchAreaCookie(cmd, additionalLightData.areaLightCookie, additionalLightData.IESSpot);
					}
					else if (additionalLightData.IESSpot != null)
					{
						lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.FetchAreaCookie(cmd, additionalLightData.IESSpot);
					}
					else if (additionalLightData.areaLightCookie != null)
					{
						lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.FetchAreaCookie(cmd, additionalLightData.areaLightCookie);
					}
					break;
				}
			}
			else if (lightType == HDLightType.Spot && additionalLightData.spotLightShape != 0)
			{
				lightData.cookieMode = CookieMode.Clamp;
				lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.Fetch2DCookie(cmd, Texture2D.whiteTexture);
			}
			else if (lightData.lightType == GPULightType.Rectangle && (additionalLightData.areaLightCookie != null || additionalLightData.IESPoint != null))
			{
				lightData.cookieMode = CookieMode.Clamp;
				if (additionalLightData.areaLightCookie != null && additionalLightData.IESSpot != null && additionalLightData.areaLightCookie != additionalLightData.IESSpot)
				{
					lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.FetchAreaCookie(cmd, additionalLightData.areaLightCookie, additionalLightData.IESSpot);
				}
				else if (additionalLightData.IESSpot != null)
				{
					lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.FetchAreaCookie(cmd, additionalLightData.IESSpot);
				}
				else if (additionalLightData.areaLightCookie != null)
				{
					lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.FetchAreaCookie(cmd, additionalLightData.areaLightCookie);
				}
			}
			lightData.shadowIndex = shadowIndex;
			additionalLightData.shadowIndex = shadowIndex;
		}

		private void GetContactShadowMask(HDAdditionalLightData hdAdditionalLightData, BoolScalableSetting contactShadowEnabled, HDCamera hdCamera, ref int contactShadowMask, ref float rayTracingShadowFlag)
		{
			contactShadowMask = 0;
			rayTracingShadowFlag = 0f;
			if (hdAdditionalLightData.useContactShadow.Value(contactShadowEnabled) && m_ContactShadowIndex < LightDefinitions.s_ContactShadowMaskMask)
			{
				contactShadowMask = 1 << m_ContactShadowIndex;
				m_ContactShadowIndex++;
				if (hdCamera.frameSettings.IsEnabled(FrameSettingsField.RayTracing) && hdAdditionalLightData.rayTraceContactShadow)
				{
					rayTracingShadowFlag = 1f;
				}
			}
		}

		private bool EnoughScreenSpaceShadowSlots(GPULightType gpuLightType, int screenSpaceChannelSlot)
		{
			if (gpuLightType == GPULightType.Rectangle)
			{
				return screenSpaceChannelSlot + 1 < m_Asset.currentPlatformRenderPipelineSettings.hdShadowInitParams.maxScreenSpaceShadowSlots;
			}
			return screenSpaceChannelSlot < m_Asset.currentPlatformRenderPipelineSettings.hdShadowInitParams.maxScreenSpaceShadowSlots;
		}

		private void CalculateDirectionalLightDataTextureInfo(ref DirectionalLightData lightData, CommandBuffer cmd, in VisibleLight light, in Light lightComponent, in HDAdditionalLightData additionalLightData, HDCamera hdCamera, HDProcessedVisibleLightsBuilder.ShadowMapFlags shadowFlags, int lightDataIndex, int shadowIndex)
		{
			if (shadowIndex != -1)
			{
				if ((shadowFlags & HDProcessedVisibleLightsBuilder.ShadowMapFlags.WillRenderScreenSpaceShadow) != 0)
				{
					lightData.screenSpaceShadowIndex = m_ScreenSpaceShadowChannelSlot;
					bool flag = (shadowFlags & HDProcessedVisibleLightsBuilder.ShadowMapFlags.WillRenderRayTracedShadow) != 0;
					if (additionalLightData.colorShadow && flag)
					{
						m_ScreenSpaceShadowChannelSlot += 3;
						lightData.screenSpaceShadowIndex |= (int)LightDefinitions.s_ScreenSpaceColorShadowFlag;
					}
					else
					{
						m_ScreenSpaceShadowChannelSlot++;
					}
					if (flag)
					{
						lightData.screenSpaceShadowIndex |= (int)LightDefinitions.s_RayTracedScreenSpaceShadowFlag;
					}
					m_ScreenSpaceShadowIndex++;
					m_ScreenSpaceShadowsUnion.Add(additionalLightData);
				}
				m_CurrentSunLightDirectionalLightData = lightData;
				m_CurrentShadowSortedSunLightIndex = lightDataIndex;
				m_CurrentSunShadowMapFlags = shadowFlags;
			}
			CookieParameters cookieParameters = default(CookieParameters);
			cookieParameters.texture = lightComponent?.cookie;
			cookieParameters.size = new Vector2(additionalLightData.shapeWidth, additionalLightData.shapeHeight);
			cookieParameters.position = light.GetPosition();
			CookieParameters cookieParams = cookieParameters;
			if (lightComponent == HDRenderPipeline.currentPipeline.GetMainLight())
			{
				CloudSettings cloudSettings;
				CloudRenderer cloudRenderer;
				if (HDRenderPipeline.currentPipeline.HasVolumetricCloudsShadows_IgnoreSun(hdCamera))
				{
					cookieParams = HDRenderPipeline.currentPipeline.RenderVolumetricCloudsShadows(cmd, hdCamera);
					lightData.positionRWS = cookieParams.position;
					if (ShaderConfig.s_CameraRelativeRendering != 0)
					{
						lightData.positionRWS -= hdCamera.mainViewConstants.worldSpaceCameraPos;
					}
				}
				else if (HDRenderPipeline.currentPipeline.skyManager.TryGetCloudSettings(hdCamera, out cloudSettings, out cloudRenderer) && cloudRenderer.GetSunLightCookieParameters(cloudSettings, ref cookieParams))
				{
					BuiltinSunCookieParameters builtinSunCookieParameters = default(BuiltinSunCookieParameters);
					builtinSunCookieParameters.cloudSettings = cloudSettings;
					builtinSunCookieParameters.sunLight = lightComponent;
					builtinSunCookieParameters.hdCamera = hdCamera;
					builtinSunCookieParameters.commandBuffer = cmd;
					BuiltinSunCookieParameters builtinParams = builtinSunCookieParameters;
					cloudRenderer.RenderSunLightCookie(builtinParams);
				}
			}
			if ((bool)cookieParams.texture)
			{
				lightData.cookieMode = ((cookieParams.texture.wrapMode != 0) ? CookieMode.Clamp : CookieMode.Repeat);
				lightData.cookieScaleOffset = m_TextureCaches.lightCookieManager.Fetch2DCookie(cmd, cookieParams.texture);
			}
			else
			{
				lightData.cookieMode = CookieMode.None;
			}
			lightData.right = light.GetRight() * 2f / Mathf.Max(cookieParams.size.x, 0.001f);
			lightData.up = light.GetUp() * 2f / Mathf.Max(cookieParams.size.y, 0.001f);
			if (additionalLightData.surfaceTexture == null)
			{
				lightData.surfaceTextureScaleOffset = Vector4.zero;
			}
			else
			{
				lightData.surfaceTextureScaleOffset = m_TextureCaches.lightCookieManager.Fetch2DCookie(cmd, additionalLightData.surfaceTexture);
			}
			GetContactShadowMask(additionalLightData, HDAdditionalLightData.ScalableSettings.UseContactShadow(m_Asset), hdCamera, ref lightData.contactShadowMask, ref lightData.isRayTracedContactShadow);
			lightData.shadowIndex = shadowIndex;
		}

		private void CalculateLightDataTextureInfo(ref LightData lightData, CommandBuffer cmd, in Light lightComponent, HDAdditionalLightData additionalLightData, in HDShadowInitParameters shadowInitParams, in HDCamera hdCamera, BoolScalableSetting contactShadowScalableSetting, HDLightType lightType, HDProcessedVisibleLightsBuilder.ShadowMapFlags shadowFlags, bool rayTracingEnabled, int lightDataIndex, int shadowIndex)
		{
			ProcessLightDataShadowIndex(cmd, in shadowInitParams, lightType, lightComponent, additionalLightData, shadowIndex, ref lightData);
			GetContactShadowMask(additionalLightData, contactShadowScalableSetting, hdCamera, ref lightData.contactShadowMask, ref lightData.isRayTracedContactShadow);
			if (rayTracingEnabled && EnoughScreenSpaceShadowSlots(lightData.lightType, m_ScreenSpaceShadowChannelSlot) && (shadowFlags & HDProcessedVisibleLightsBuilder.ShadowMapFlags.WillRenderScreenSpaceShadow) != 0)
			{
				if (lightData.lightType == GPULightType.Rectangle && m_ScreenSpaceShadowChannelSlot % 4 == 3)
				{
					m_ScreenSpaceShadowChannelSlot++;
				}
				lightData.screenSpaceShadowIndex = m_ScreenSpaceShadowChannelSlot;
				m_CurrentScreenSpaceShadowData[m_ScreenSpaceShadowIndex].additionalLightData = additionalLightData;
				m_CurrentScreenSpaceShadowData[m_ScreenSpaceShadowIndex].lightDataIndex = lightDataIndex;
				m_CurrentScreenSpaceShadowData[m_ScreenSpaceShadowIndex].valid = true;
				m_ScreenSpaceShadowsUnion.Add(additionalLightData);
				m_ScreenSpaceShadowIndex++;
				if (lightData.lightType == GPULightType.Rectangle)
				{
					m_ScreenSpaceShadowChannelSlot += 2;
				}
				else
				{
					m_ScreenSpaceShadowChannelSlot++;
				}
			}
		}

		private unsafe void CalculateAllLightDataTextureInfo(CommandBuffer cmd, HDCamera hdCamera, in CullingResults cullResults, HDProcessedVisibleLightsBuilder visibleLights, HDLightRenderDatabase lightEntities, HDShadowSettings hdShadowSettings, in HDShadowInitParameters shadowInitParams, DebugDisplaySettings debugDisplaySettings)
		{
			BoolScalableSetting contactShadowScalableSetting = HDAdditionalLightData.ScalableSettings.UseContactShadow(m_Asset);
			bool rayTracingEnabled = hdCamera.frameSettings.IsEnabled(FrameSettingsField.RayTracing);
			HDProcessedVisibleLight* unsafePtr = (HDProcessedVisibleLight*)visibleLights.processedEntities.GetUnsafePtr();
			LightData* unsafePtr2 = (LightData*)m_Lights.GetUnsafePtr();
			DirectionalLightData* unsafePtr3 = (DirectionalLightData*)m_DirectionalLights.GetUnsafePtr();
			VisibleLight* unsafePtr4 = (VisibleLight*)cullResults.visibleLights.GetUnsafePtr();
			HDShadowFilteringQuality shadowFilteringQuality = m_Asset.currentPlatformRenderPipelineSettings.hdShadowInitParams.shadowFilteringQuality;
			HDAreaShadowFilteringQuality areaShadowFilteringQuality = m_Asset.currentPlatformRenderPipelineSettings.hdShadowInitParams.areaShadowFilteringQuality;
			int sortedDirectionalLightCounts = visibleLights.sortedDirectionalLightCounts;
			int sortedLightCounts = visibleLights.sortedLightCounts;
			for (int i = 0; i < sortedLightCounts; i++)
			{
				uint num = visibleLights.sortKeys[i];
				_ = (num >> 27) & 0x1F;
				GPULightType gPULightType = (GPULightType)((int)(num >> 22) & 0x1F);
				_ = (num >> 17) & 0x1F;
				int num2 = (int)(num & 0xFFFF);
				int num3 = visibleLights.visibleLightEntityDataIndices[num2];
				if (num3 == HDLightRenderDatabase.InvalidDataIndex)
				{
					continue;
				}
				HDAdditionalLightData additionalLightData = lightEntities.hdAdditionalLightData[num3];
				if (!(additionalLightData == null))
				{
					ref HDProcessedVisibleLight reference = ref UnsafeUtility.AsRef<HDProcessedVisibleLight>(unsafePtr + num2);
					HDLightType lightType = reference.lightType;
					Light lightComponent = additionalLightData.legacyLight;
					int shadowIndex = -1;
					if (lightComponent != null && (reference.shadowMapFlags & HDProcessedVisibleLightsBuilder.ShadowMapFlags.WillRenderShadowMap) != 0)
					{
						ref VisibleLight reference2 = ref UnsafeUtility.AsRef<VisibleLight>(unsafePtr4 + num2);
						shadowIndex = additionalLightData.UpdateShadowRequest(hdCamera, m_ShadowManager, hdShadowSettings, reference2, cullResults, num2, debugDisplaySettings.data.lightingDebugSettings, shadowFilteringQuality, areaShadowFilteringQuality, out var _);
					}
					if (gPULightType == GPULightType.Directional)
					{
						ref VisibleLight light = ref UnsafeUtility.AsRef<VisibleLight>(unsafePtr4 + num2);
						int num4 = i;
						CalculateDirectionalLightDataTextureInfo(ref UnsafeUtility.AsRef<DirectionalLightData>(unsafePtr3 + num4), cmd, in light, in lightComponent, in additionalLightData, hdCamera, reference.shadowMapFlags, num4, shadowIndex);
					}
					else
					{
						int num5 = i - sortedDirectionalLightCounts;
						CalculateLightDataTextureInfo(ref UnsafeUtility.AsRef<LightData>(unsafePtr2 + num5), cmd, in lightComponent, additionalLightData, in shadowInitParams, in hdCamera, contactShadowScalableSetting, lightType, reference.shadowMapFlags, rayTracingEnabled, num5, shadowIndex);
					}
				}
			}
		}
	}
}
