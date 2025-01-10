using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{bitDatas.humanizedData}")]
	[DebuggerTypeProxy(typeof(FrameSettingsDebugView))]
	public struct FrameSettings
	{
		[DebuggerDisplay("{m_Value}", Name = "{m_Label,nq}")]
		internal class DebuggerEntry
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private string m_Label;

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private object m_Value;

			public DebuggerEntry(string label, object value)
			{
				m_Label = label;
				m_Value = value;
			}
		}

		[DebuggerDisplay("", Name = "{m_GroupName,nq}")]
		internal class DebuggerGroup
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private string m_GroupName;

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public DebuggerEntry[] m_Entries;

			public DebuggerGroup(string groupName, DebuggerEntry[] entries)
			{
				m_GroupName = groupName;
				m_Entries = entries;
			}
		}

		internal class FrameSettingsDebugView
		{
			private const int numberOfNonBitValues = 2;

			private FrameSettings m_FrameSettings;

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public DebuggerGroup[] Keys
			{
				get
				{
					_ = Enum.GetValues(typeof(FrameSettingsField)).Length;
					Dictionary<FrameSettingsField, FrameSettingsFieldAttribute> dictionary = new Dictionary<FrameSettingsField, FrameSettingsFieldAttribute>();
					List<DebuggerGroup> list = new List<DebuggerGroup>();
					Dictionary<FrameSettingsField, string> enumNameMap = FrameSettingsFieldAttribute.GetEnumNameMap();
					Type typeFromHandle = typeof(FrameSettingsField);
					List<FrameSettingsField> list2 = new List<FrameSettingsField>();
					foreach (FrameSettingsField key in enumNameMap.Keys)
					{
						dictionary[key] = typeFromHandle.GetField(enumNameMap[key]).GetCustomAttribute<FrameSettingsFieldAttribute>();
						if (dictionary[key] == null)
						{
							list2.Add(key);
						}
					}
					foreach (int groupIndex in (from a in dictionary.Values
						where a != null
						select a.@group).Distinct())
					{
						list.Add(new DebuggerGroup(FrameSettingsHistory.foldoutNames[groupIndex], (from kvp in dictionary?.Where(delegate(KeyValuePair<FrameSettingsField, FrameSettingsFieldAttribute> pair)
							{
								FrameSettingsFieldAttribute value = pair.Value;
								return value != null && value.@group == groupIndex;
							})?.OrderBy((KeyValuePair<FrameSettingsField, FrameSettingsFieldAttribute> pair) => pair.Value.orderInGroup)
							select new DebuggerEntry(Enum.GetName(typeof(FrameSettingsField), kvp.Key), m_FrameSettings.bitDatas[(uint)kvp.Key])).ToArray()));
					}
					list.Add(new DebuggerGroup("Bits without attribute", list2.Where((FrameSettingsField fs) => fs != FrameSettingsField.None)?.Select((FrameSettingsField fs) => new DebuggerEntry(Enum.GetName(typeof(FrameSettingsField), fs), m_FrameSettings.bitDatas[(uint)fs])).ToArray()));
					list.Add(new DebuggerGroup("Non Bit data", new DebuggerEntry[11]
					{
						new DebuggerEntry("sssQualityMode", m_FrameSettings.sssQualityMode),
						new DebuggerEntry("sssQualityLevel", m_FrameSettings.sssQualityLevel),
						new DebuggerEntry("sssCustomSampleBudget", m_FrameSettings.sssCustomSampleBudget),
						new DebuggerEntry("lodBias", m_FrameSettings.lodBias),
						new DebuggerEntry("lodBiasMode", m_FrameSettings.lodBiasMode),
						new DebuggerEntry("lodBiasQualityLevel", m_FrameSettings.lodBiasQualityLevel),
						new DebuggerEntry("maximumLODLevel", m_FrameSettings.maximumLODLevel),
						new DebuggerEntry("maximumLODLevelMode", m_FrameSettings.maximumLODLevelMode),
						new DebuggerEntry("maximumLODLevelQualityLevel", m_FrameSettings.maximumLODLevelQualityLevel),
						new DebuggerEntry("materialQuality", m_FrameSettings.materialQuality),
						new DebuggerEntry("msaaMode", m_FrameSettings.msaaMode)
					}));
					return list.ToArray();
				}
			}

			public FrameSettingsDebugView(FrameSettings frameSettings)
			{
				m_FrameSettings = frameSettings;
			}
		}

		[SerializeField]
		private BitArray128 bitDatas;

		[SerializeField]
		public float lodBias;

		[SerializeField]
		public LODBiasMode lodBiasMode;

		[SerializeField]
		public int lodBiasQualityLevel;

		[SerializeField]
		public int maximumLODLevel;

		[SerializeField]
		public MaximumLODLevelMode maximumLODLevelMode;

		[SerializeField]
		public int maximumLODLevelQualityLevel;

		[SerializeField]
		public SssQualityMode sssQualityMode;

		[SerializeField]
		public int sssQualityLevel;

		[SerializeField]
		public int sssCustomSampleBudget;

		[SerializeField]
		public MSAAMode msaaMode;

		internal int sssResolvedSampleBudget;

		public MaterialQuality materialQuality;

		public LitShaderMode litShaderMode
		{
			get
			{
				if (!bitDatas[0u])
				{
					return LitShaderMode.Forward;
				}
				return LitShaderMode.Deferred;
			}
			set
			{
				bitDatas[0u] = value == LitShaderMode.Deferred;
			}
		}

		internal bool fptl
		{
			get
			{
				if (litShaderMode != LitShaderMode.Deferred)
				{
					return bitDatas[120u];
				}
				return true;
			}
		}

		internal float specularGlobalDimmer
		{
			get
			{
				if (!bitDatas[38u])
				{
					return 0f;
				}
				return 1f;
			}
		}

		private bool asyncEnabled
		{
			get
			{
				if (SystemInfo.supportsAsyncCompute || RenderGraph.requireDebugData)
				{
					return bitDatas[40u];
				}
				return false;
			}
		}

		internal static FrameSettings NewDefaultCamera()
		{
			FrameSettings result = default(FrameSettings);
			result.bitDatas = new BitArray128(new uint[70]
			{
				20u, 21u, 22u, 34u, 23u, 94u, 24u, 95u, 46u, 26u,
				27u, 28u, 29u, 30u, 32u, 0u, 8u, 9u, 6u, 68u,
				10u, 11u, 12u, 96u, 13u, 14u, 67u, 15u, 39u, 80u,
				81u, 82u, 83u, 84u, 97u, 85u, 86u, 87u, 88u, 93u,
				89u, 90u, 91u, 17u, 18u, 19u, 2u, 3u, 40u, 41u,
				42u, 42u, 43u, 44u, 45u, 122u, 123u, 124u, 125u, 120u,
				121u, 16u, 33u, 35u, 37u, 38u, 92u, 127u, 79u, 99u
			});
			result.lodBias = 1f;
			result.sssQualityMode = SssQualityMode.FromQualitySettings;
			result.sssQualityLevel = 0;
			result.sssCustomSampleBudget = 20;
			result.msaaMode = MSAAMode.None;
			return result;
		}

		internal static FrameSettings NewDefaultRealtimeReflectionProbe()
		{
			FrameSettings result = default(FrameSettings);
			result.bitDatas = new BitArray128(new uint[34]
			{
				20u, 46u, 26u, 28u, 29u, 30u, 0u, 8u, 9u, 6u,
				68u, 10u, 11u, 12u, 96u, 2u, 3u, 40u, 41u, 42u,
				42u, 43u, 44u, 45u, 122u, 123u, 124u, 125u, 120u, 121u,
				33u, 92u, 127u, 38u
			});
			result.lodBias = 1f;
			result.sssQualityMode = SssQualityMode.FromQualitySettings;
			result.sssQualityLevel = 0;
			result.sssCustomSampleBudget = 20;
			result.msaaMode = MSAAMode.None;
			return result;
		}

		internal static FrameSettings NewDefaultCustomOrBakeReflectionProbe()
		{
			FrameSettings result = default(FrameSettings);
			result.bitDatas = new BitArray128(new uint[37]
			{
				20u, 21u, 22u, 24u, 46u, 26u, 27u, 28u, 29u, 30u,
				0u, 8u, 9u, 6u, 68u, 12u, 96u, 13u, 14u, 67u,
				2u, 3u, 40u, 41u, 43u, 44u, 45u, 122u, 123u, 124u,
				125u, 120u, 121u, 36u, 79u, 99u, 127u
			});
			result.lodBias = 1f;
			result.sssQualityMode = SssQualityMode.FromQualitySettings;
			result.sssQualityLevel = 0;
			result.sssCustomSampleBudget = 20;
			result.msaaMode = MSAAMode.None;
			return result;
		}

		public bool IsEnabled(FrameSettingsField field)
		{
			return bitDatas[(uint)field];
		}

		public void SetEnabled(FrameSettingsField field, bool value)
		{
			bitDatas[(uint)field] = value;
		}

		public float GetResolvedLODBias(HDRenderPipelineAsset hdrp)
		{
			FloatScalableSetting floatScalableSetting = hdrp.currentPlatformRenderPipelineSettings.lodBias;
			return lodBiasMode switch
			{
				LODBiasMode.FromQualitySettings => floatScalableSetting[lodBiasQualityLevel], 
				LODBiasMode.OverrideQualitySettings => lodBias, 
				LODBiasMode.ScaleQualitySettings => lodBias * floatScalableSetting[lodBiasQualityLevel], 
				_ => throw new ArgumentOutOfRangeException("lodBiasMode"), 
			};
		}

		public int GetResolvedMaximumLODLevel(HDRenderPipelineAsset hdrp)
		{
			IntScalableSetting intScalableSetting = hdrp.currentPlatformRenderPipelineSettings.maximumLODLevel;
			return maximumLODLevelMode switch
			{
				MaximumLODLevelMode.FromQualitySettings => intScalableSetting[maximumLODLevelQualityLevel], 
				MaximumLODLevelMode.OffsetQualitySettings => intScalableSetting[maximumLODLevelQualityLevel] + maximumLODLevel, 
				MaximumLODLevelMode.OverrideQualitySettings => maximumLODLevel, 
				_ => throw new ArgumentOutOfRangeException("maximumLODLevelMode"), 
			};
		}

		public int GetResolvedSssSampleBudget(HDRenderPipelineAsset hdrp)
		{
			IntScalableSetting sssSampleBudget = hdrp.currentPlatformRenderPipelineSettings.sssSampleBudget;
			return sssQualityMode switch
			{
				SssQualityMode.FromQualitySettings => sssSampleBudget[sssQualityLevel], 
				SssQualityMode.OverrideQualitySettings => sssCustomSampleBudget, 
				_ => throw new ArgumentOutOfRangeException("sssCustomSampleBudget"), 
			};
		}

		public MSAASamples GetResolvedMSAAMode(HDRenderPipelineAsset hdrp)
		{
			if (msaaMode == MSAAMode.FromHDRPAsset)
			{
				return hdrp.currentPlatformRenderPipelineSettings.msaaSampleCount;
			}
			return (MSAASamples)msaaMode;
		}

		internal bool BuildLightListRunsAsync()
		{
			if (asyncEnabled)
			{
				return bitDatas[41u];
			}
			return false;
		}

		internal bool SSRRunsAsync()
		{
			if (asyncEnabled)
			{
				return bitDatas[42u];
			}
			return false;
		}

		internal bool SSAORunsAsync()
		{
			if (asyncEnabled)
			{
				return bitDatas[43u];
			}
			return false;
		}

		internal bool ContactShadowsRunsAsync()
		{
			if (asyncEnabled)
			{
				return bitDatas[44u];
			}
			return false;
		}

		internal bool VolumeVoxelizationRunsAsync()
		{
			if (asyncEnabled)
			{
				return bitDatas[45u];
			}
			return false;
		}

		internal static void Override(ref FrameSettings overriddenFrameSettings, FrameSettings overridingFrameSettings, FrameSettingsOverrideMask frameSettingsOverideMask)
		{
			overriddenFrameSettings.bitDatas = (overridingFrameSettings.bitDatas & frameSettingsOverideMask.mask) | (~frameSettingsOverideMask.mask & overriddenFrameSettings.bitDatas);
			if (frameSettingsOverideMask.mask[47u])
			{
				overriddenFrameSettings.sssQualityMode = overridingFrameSettings.sssQualityMode;
			}
			if (frameSettingsOverideMask.mask[48u])
			{
				overriddenFrameSettings.sssQualityLevel = overridingFrameSettings.sssQualityLevel;
			}
			if (frameSettingsOverideMask.mask[49u])
			{
				overriddenFrameSettings.sssCustomSampleBudget = overridingFrameSettings.sssCustomSampleBudget;
			}
			if (frameSettingsOverideMask.mask[61u])
			{
				overriddenFrameSettings.lodBias = overridingFrameSettings.lodBias;
			}
			if (frameSettingsOverideMask.mask[60u])
			{
				overriddenFrameSettings.lodBiasMode = overridingFrameSettings.lodBiasMode;
			}
			if (frameSettingsOverideMask.mask[64u])
			{
				overriddenFrameSettings.lodBiasQualityLevel = overridingFrameSettings.lodBiasQualityLevel;
			}
			if (frameSettingsOverideMask.mask[63u])
			{
				overriddenFrameSettings.maximumLODLevel = overridingFrameSettings.maximumLODLevel;
			}
			if (frameSettingsOverideMask.mask[62u])
			{
				overriddenFrameSettings.maximumLODLevelMode = overridingFrameSettings.maximumLODLevelMode;
			}
			if (frameSettingsOverideMask.mask[65u])
			{
				overriddenFrameSettings.maximumLODLevelQualityLevel = overridingFrameSettings.maximumLODLevelQualityLevel;
			}
			if (frameSettingsOverideMask.mask[66u])
			{
				overriddenFrameSettings.materialQuality = overridingFrameSettings.materialQuality;
			}
			if (frameSettingsOverideMask.mask[4u])
			{
				overriddenFrameSettings.msaaMode = overridingFrameSettings.msaaMode;
			}
		}

		internal static void Sanitize(ref FrameSettings sanitizedFrameSettings, Camera camera, RenderPipelineSettings renderPipelineSettings)
		{
			bool flag = camera.cameraType == CameraType.Reflection;
			bool flag2 = GeometryUtils.IsProjectionMatrixOblique(camera.projectionMatrix);
			bool flag3 = HDUtils.IsRegularPreviewCamera(camera);
			bool flag4 = CoreUtils.IsSceneViewFogEnabled(camera);
			bool flag5 = !flag || (flag && flag2);
			switch (renderPipelineSettings.supportedLitShaderMode)
			{
			case RenderPipelineSettings.SupportedLitShaderMode.ForwardOnly:
				sanitizedFrameSettings.litShaderMode = LitShaderMode.Forward;
				break;
			case RenderPipelineSettings.SupportedLitShaderMode.DeferredOnly:
				sanitizedFrameSettings.litShaderMode = LitShaderMode.Deferred;
				break;
			}
			sanitizedFrameSettings.bitDatas[20u] &= !flag3;
			sanitizedFrameSettings.bitDatas[22u] &= renderPipelineSettings.supportShadowMask && !flag3;
			sanitizedFrameSettings.bitDatas[21u] &= !flag3;
			bool flag6 = HDRenderPipeline.PipelineSupportsRayTracing(renderPipelineSettings);
			bool flag7 = (sanitizedFrameSettings.bitDatas[92u] &= flag6 && !flag3 && flag5);
			if (sanitizedFrameSettings.litShaderMode != LitShaderMode.Forward || flag6 || renderPipelineSettings.supportWater)
			{
				sanitizedFrameSettings.msaaMode = MSAAMode.None;
			}
			bool flag8 = ((sanitizedFrameSettings.msaaMode == MSAAMode.FromHDRPAsset) ? (renderPipelineSettings.msaaSampleCount != MSAASamples.None) : (sanitizedFrameSettings.msaaMode != MSAAMode.None));
			sanitizedFrameSettings.bitDatas[34u] &= renderPipelineSettings.hdShadowInitParams.supportScreenSpaceShadows && sanitizedFrameSettings.bitDatas[2u] && !flag8;
			bool flag9 = (sanitizedFrameSettings.bitDatas[23u] &= renderPipelineSettings.supportSSR && !flag8 && !flag3 && flag5);
			sanitizedFrameSettings.bitDatas[94u] &= flag9 && renderPipelineSettings.supportSSRTransparent && sanitizedFrameSettings.bitDatas[3u] && renderPipelineSettings.supportTransparentDepthPrepass;
			sanitizedFrameSettings.bitDatas[13u] &= !flag3;
			sanitizedFrameSettings.bitDatas[24u] &= renderPipelineSettings.supportSSAO && !flag3 && sanitizedFrameSettings.bitDatas[2u] && flag5;
			sanitizedFrameSettings.bitDatas[95u] &= renderPipelineSettings.supportSSGI && !flag3 && sanitizedFrameSettings.bitDatas[2u] && flag5;
			sanitizedFrameSettings.bitDatas[46u] &= renderPipelineSettings.supportSubsurfaceScattering;
			sanitizedFrameSettings.bitDatas[79u] &= renderPipelineSettings.supportVolumetricClouds && !flag3;
			sanitizedFrameSettings.bitDatas[98u] &= sanitizedFrameSettings.bitDatas[79u];
			sanitizedFrameSettings.bitDatas[99u] &= renderPipelineSettings.supportWater && !flag3;
			sanitizedFrameSettings.bitDatas[97u] &= sanitizedFrameSettings.bitDatas[97u] && renderPipelineSettings.supportDataDrivenLensFlare;
			bool flag10 = (sanitizedFrameSettings.bitDatas[27u] &= flag4 && !flag3);
			sanitizedFrameSettings.bitDatas[28u] &= renderPipelineSettings.supportVolumetrics && flag10;
			sanitizedFrameSettings.bitDatas[29u] &= !flag3 && flag5;
			sanitizedFrameSettings.bitDatas[30u] &= renderPipelineSettings.supportLightLayers && !flag3;
			sanitizedFrameSettings.bitDatas[32u] &= (!flag || (flag2 && flag)) && !flag3;
			sanitizedFrameSettings.bitDatas[15u] &= !flag && !flag3;
			sanitizedFrameSettings.bitDatas[8u] &= renderPipelineSettings.supportTransparentDepthPrepass && !flag3 && sanitizedFrameSettings.bitDatas[3u];
			bool flag11 = (sanitizedFrameSettings.bitDatas[10u] &= renderPipelineSettings.supportMotionVectors && !flag3);
			sanitizedFrameSettings.bitDatas[11u] &= flag11 && !flag3;
			sanitizedFrameSettings.bitDatas[16u] &= flag11 && !flag3;
			sanitizedFrameSettings.bitDatas[12u] &= renderPipelineSettings.supportDecals && !flag3;
			sanitizedFrameSettings.bitDatas[96u] &= renderPipelineSettings.supportDecalLayers && sanitizedFrameSettings.bitDatas[12u];
			sanitizedFrameSettings.bitDatas[9u] &= renderPipelineSettings.supportTransparentDepthPostpass && !flag3 && sanitizedFrameSettings.bitDatas[3u];
			bool flag12 = (sanitizedFrameSettings.bitDatas[14u] &= renderPipelineSettings.supportDistortion && !flag3);
			sanitizedFrameSettings.bitDatas[67u] &= flag12 && !flag3;
			sanitizedFrameSettings.bitDatas[18u] &= renderPipelineSettings.lowresTransparentSettings.enabled && sanitizedFrameSettings.bitDatas[3u];
			sanitizedFrameSettings.bitDatas[41u] &= sanitizedFrameSettings.asyncEnabled;
			sanitizedFrameSettings.bitDatas[42u] &= sanitizedFrameSettings.asyncEnabled;
			sanitizedFrameSettings.bitDatas[43u] &= sanitizedFrameSettings.asyncEnabled;
			sanitizedFrameSettings.bitDatas[44u] &= sanitizedFrameSettings.asyncEnabled && !flag7;
			sanitizedFrameSettings.bitDatas[45u] &= sanitizedFrameSettings.asyncEnabled;
			sanitizedFrameSettings.bitDatas[6u] &= renderPipelineSettings.supportCustomPass;
			sanitizedFrameSettings.bitDatas[6u] &= camera.cameraType != CameraType.Preview;
			sanitizedFrameSettings.bitDatas[39u] &= camera.cameraType != CameraType.Preview;
			sanitizedFrameSettings.bitDatas[120u] &= !flag8;
			sanitizedFrameSettings.bitDatas[127u] &= renderPipelineSettings.supportProbeVolume && !flag3;
			sanitizedFrameSettings.bitDatas[126u] &= renderPipelineSettings.supportProbeVolume;
			sanitizedFrameSettings.bitDatas[33u] &= !flag3;
			sanitizedFrameSettings.bitDatas[35u] &= !flag3;
			sanitizedFrameSettings.bitDatas[46u] &= sanitizedFrameSettings.bitDatas[2u];
			sanitizedFrameSettings.bitDatas[68u] = false;
		}

		internal static void AggregateFrameSettings(ref FrameSettings aggregatedFrameSettings, Camera camera, HDAdditionalCameraData additionalData, HDRenderPipelineAsset hdrpAsset)
		{
			AggregateFrameSettings(ref aggregatedFrameSettings, camera, additionalData, ref HDRenderPipelineGlobalSettings.instance.GetDefaultFrameSettings(additionalData?.defaultFrameSettings ?? FrameSettingsRenderType.Camera), hdrpAsset.currentPlatformRenderPipelineSettings);
		}

		internal static void AggregateFrameSettings(ref FrameSettings aggregatedFrameSettings, Camera camera, HDAdditionalCameraData additionalData, ref FrameSettings defaultFrameSettings, RenderPipelineSettings supportedFeatures)
		{
			aggregatedFrameSettings = defaultFrameSettings;
			if ((bool)additionalData && additionalData.customRenderingSettings)
			{
				Override(ref aggregatedFrameSettings, additionalData.renderingPathCustomFrameSettings, additionalData.renderingPathCustomFrameSettingsOverrideMask);
			}
			Sanitize(ref aggregatedFrameSettings, camera, supportedFeatures);
		}

		public static bool operator ==(FrameSettings a, FrameSettings b)
		{
			if (a.bitDatas == b.bitDatas && a.sssQualityMode == b.sssQualityMode && a.sssQualityLevel == b.sssQualityLevel && a.sssCustomSampleBudget == b.sssCustomSampleBudget && a.lodBias == b.lodBias && a.lodBiasMode == b.lodBiasMode && a.lodBiasQualityLevel == b.lodBiasQualityLevel && a.maximumLODLevel == b.maximumLODLevel && a.maximumLODLevelMode == b.maximumLODLevelMode && a.maximumLODLevelQualityLevel == b.maximumLODLevelQualityLevel && a.materialQuality == b.materialQuality)
			{
				return a.msaaMode == b.msaaMode;
			}
			return false;
		}

		public static bool operator !=(FrameSettings a, FrameSettings b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is FrameSettings && bitDatas.Equals(((FrameSettings)obj).bitDatas) && sssQualityMode.Equals(((FrameSettings)obj).sssQualityMode) && sssQualityLevel.Equals(((FrameSettings)obj).sssQualityLevel) && sssCustomSampleBudget.Equals(((FrameSettings)obj).sssCustomSampleBudget) && lodBias.Equals(((FrameSettings)obj).lodBias) && lodBiasMode.Equals(((FrameSettings)obj).lodBiasMode) && lodBiasQualityLevel.Equals(((FrameSettings)obj).lodBiasQualityLevel) && maximumLODLevel.Equals(((FrameSettings)obj).maximumLODLevel) && maximumLODLevelMode.Equals(((FrameSettings)obj).maximumLODLevelMode) && maximumLODLevelQualityLevel.Equals(((FrameSettings)obj).maximumLODLevelQualityLevel) && materialQuality.Equals(((FrameSettings)obj).materialQuality))
			{
				return msaaMode.Equals(((FrameSettings)obj).msaaMode);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((((((((((1474027755 * -1521134295 + bitDatas.GetHashCode()) * -1521134295 + sssQualityMode.GetHashCode()) * -1521134295 + sssQualityLevel.GetHashCode()) * -1521134295 + sssCustomSampleBudget.GetHashCode()) * -1521134295 + lodBias.GetHashCode()) * -1521134295 + lodBiasMode.GetHashCode()) * -1521134295 + lodBiasQualityLevel.GetHashCode()) * -1521134295 + maximumLODLevel.GetHashCode()) * -1521134295 + maximumLODLevelMode.GetHashCode()) * -1521134295 + maximumLODLevelQualityLevel.GetHashCode()) * -1521134295 + materialQuality.GetHashCode()) * -1521134295 + msaaMode.GetHashCode();
		}

		internal static void MigrateFromClassVersion(ref ObsoleteFrameSettings oldFrameSettingsFormat, ref FrameSettings newFrameSettingsFormat, ref FrameSettingsOverrideMask newFrameSettingsOverrideMask)
		{
			if (oldFrameSettingsFormat == null)
			{
				return;
			}
			switch (oldFrameSettingsFormat.shaderLitMode)
			{
			case ObsoleteLitShaderMode.Forward:
				newFrameSettingsFormat.litShaderMode = LitShaderMode.Forward;
				break;
			case ObsoleteLitShaderMode.Deferred:
				newFrameSettingsFormat.litShaderMode = LitShaderMode.Deferred;
				break;
			default:
				throw new ArgumentException("Unknown ObsoleteLitShaderMode");
			}
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.ShadowMaps, oldFrameSettingsFormat.enableShadow);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.ContactShadows, oldFrameSettingsFormat.enableContactShadows);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.Shadowmask, oldFrameSettingsFormat.enableShadowMask);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.SSR, oldFrameSettingsFormat.enableSSR);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.SSAO, oldFrameSettingsFormat.enableSSAO);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.SubsurfaceScattering, oldFrameSettingsFormat.enableSubsurfaceScattering);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.Transmission, oldFrameSettingsFormat.enableTransmission);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.AtmosphericScattering, oldFrameSettingsFormat.enableAtmosphericScattering);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.Volumetrics, oldFrameSettingsFormat.enableVolumetrics);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.ReprojectionForVolumetrics, oldFrameSettingsFormat.enableReprojectionForVolumetrics);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.LightLayers, oldFrameSettingsFormat.enableLightLayers);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.DepthPrepassWithDeferredRendering, oldFrameSettingsFormat.enableDepthPrepassWithDeferredRendering);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.TransparentPrepass, oldFrameSettingsFormat.enableTransparentPrepass);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.MotionVectors, oldFrameSettingsFormat.enableMotionVectors);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.ObjectMotionVectors, oldFrameSettingsFormat.enableObjectMotionVectors);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.Decals, oldFrameSettingsFormat.enableDecals);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.Refraction, oldFrameSettingsFormat.enableRoughRefraction);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.TransparentPostpass, oldFrameSettingsFormat.enableTransparentPostpass);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.Distortion, oldFrameSettingsFormat.enableDistortion);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.Postprocess, oldFrameSettingsFormat.enablePostprocess);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.OpaqueObjects, oldFrameSettingsFormat.enableOpaqueObjects);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.TransparentObjects, oldFrameSettingsFormat.enableTransparentObjects);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.MSAA, oldFrameSettingsFormat.enableMSAA);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.ExposureControl, oldFrameSettingsFormat.enableExposureControl);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.AsyncCompute, oldFrameSettingsFormat.enableAsyncCompute);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.LightListAsync, oldFrameSettingsFormat.runLightListAsync);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.SSRAsync, oldFrameSettingsFormat.runSSRAsync);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.SSAOAsync, oldFrameSettingsFormat.runSSAOAsync);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.ContactShadowsAsync, oldFrameSettingsFormat.runContactShadowsAsync);
			newFrameSettingsFormat.SetEnabled(FrameSettingsField.VolumeVoxelizationsAsync, oldFrameSettingsFormat.runVolumeVoxelizationAsync);
			if (oldFrameSettingsFormat.lightLoopSettings != null)
			{
				newFrameSettingsFormat.SetEnabled(FrameSettingsField.DeferredTile, oldFrameSettingsFormat.lightLoopSettings.enableDeferredTileAndCluster);
				newFrameSettingsFormat.SetEnabled(FrameSettingsField.ComputeLightEvaluation, oldFrameSettingsFormat.lightLoopSettings.enableComputeLightEvaluation);
				newFrameSettingsFormat.SetEnabled(FrameSettingsField.ComputeLightVariants, oldFrameSettingsFormat.lightLoopSettings.enableComputeLightVariants);
				newFrameSettingsFormat.SetEnabled(FrameSettingsField.ComputeMaterialVariants, oldFrameSettingsFormat.lightLoopSettings.enableComputeMaterialVariants);
				newFrameSettingsFormat.SetEnabled(FrameSettingsField.FPTLForForwardOpaque, oldFrameSettingsFormat.lightLoopSettings.enableFptlForForwardOpaque);
				newFrameSettingsFormat.SetEnabled(FrameSettingsField.BigTilePrepass, oldFrameSettingsFormat.lightLoopSettings.enableBigTilePrepass);
			}
			newFrameSettingsOverrideMask.mask = default(BitArray128);
			foreach (ObsoleteFrameSettingsOverrides value in Enum.GetValues(typeof(ObsoleteFrameSettingsOverrides)))
			{
				if ((value & oldFrameSettingsFormat.overrides) > (ObsoleteFrameSettingsOverrides)0)
				{
					switch (value)
					{
					case ObsoleteFrameSettingsOverrides.Shadow:
						newFrameSettingsOverrideMask.mask[20u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.ContactShadow:
						newFrameSettingsOverrideMask.mask[21u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.ShadowMask:
						newFrameSettingsOverrideMask.mask[22u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.SSR:
						newFrameSettingsOverrideMask.mask[23u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.SSAO:
						newFrameSettingsOverrideMask.mask[24u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.SubsurfaceScattering:
						newFrameSettingsOverrideMask.mask[46u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.Transmission:
						newFrameSettingsOverrideMask.mask[26u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.AtmosphericScaterring:
						newFrameSettingsOverrideMask.mask[27u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.Volumetrics:
						newFrameSettingsOverrideMask.mask[28u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.ReprojectionForVolumetrics:
						newFrameSettingsOverrideMask.mask[29u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.LightLayers:
						newFrameSettingsOverrideMask.mask[30u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.ShaderLitMode:
						newFrameSettingsOverrideMask.mask[0u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.DepthPrepassWithDeferredRendering:
						newFrameSettingsOverrideMask.mask[1u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.TransparentPrepass:
						newFrameSettingsOverrideMask.mask[8u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.MotionVectors:
						newFrameSettingsOverrideMask.mask[10u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.ObjectMotionVectors:
						newFrameSettingsOverrideMask.mask[11u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.Decals:
						newFrameSettingsOverrideMask.mask[12u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.RoughRefraction:
						newFrameSettingsOverrideMask.mask[13u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.TransparentPostpass:
						newFrameSettingsOverrideMask.mask[9u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.Distortion:
						newFrameSettingsOverrideMask.mask[14u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.Postprocess:
						newFrameSettingsOverrideMask.mask[15u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.OpaqueObjects:
						newFrameSettingsOverrideMask.mask[2u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.TransparentObjects:
						newFrameSettingsOverrideMask.mask[3u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.MSAA:
						newFrameSettingsOverrideMask.mask[31u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.ExposureControl:
						newFrameSettingsOverrideMask.mask[32u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.AsyncCompute:
						newFrameSettingsOverrideMask.mask[40u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.LightListAsync:
						newFrameSettingsOverrideMask.mask[41u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.SSRAsync:
						newFrameSettingsOverrideMask.mask[42u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.SSAOAsync:
						newFrameSettingsOverrideMask.mask[43u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.ContactShadowsAsync:
						newFrameSettingsOverrideMask.mask[44u] = true;
						break;
					case ObsoleteFrameSettingsOverrides.VolumeVoxelizationsAsync:
						newFrameSettingsOverrideMask.mask[45u] = true;
						break;
					default:
						throw new ArgumentException("Unknown ObsoleteFrameSettingsOverride, was " + value);
					}
				}
			}
			if (oldFrameSettingsFormat.lightLoopSettings != null)
			{
				foreach (ObsoleteLightLoopSettingsOverrides value2 in Enum.GetValues(typeof(ObsoleteLightLoopSettingsOverrides)))
				{
					if ((value2 & oldFrameSettingsFormat.lightLoopSettings.overrides) > (ObsoleteLightLoopSettingsOverrides)0)
					{
						switch (value2)
						{
						case ObsoleteLightLoopSettingsOverrides.TileAndCluster:
							newFrameSettingsOverrideMask.mask[122u] = true;
							break;
						case ObsoleteLightLoopSettingsOverrides.BigTilePrepass:
							newFrameSettingsOverrideMask.mask[121u] = true;
							break;
						case ObsoleteLightLoopSettingsOverrides.ComputeLightEvaluation:
							newFrameSettingsOverrideMask.mask[123u] = true;
							break;
						case ObsoleteLightLoopSettingsOverrides.ComputeLightVariants:
							newFrameSettingsOverrideMask.mask[124u] = true;
							break;
						case ObsoleteLightLoopSettingsOverrides.ComputeMaterialVariants:
							newFrameSettingsOverrideMask.mask[125u] = true;
							break;
						case ObsoleteLightLoopSettingsOverrides.FptlForForwardOpaque:
							newFrameSettingsOverrideMask.mask[120u] = true;
							break;
						default:
							throw new ArgumentException("Unknown ObsoleteLightLoopSettingsOverrides");
						}
					}
				}
			}
			oldFrameSettingsFormat = null;
		}

		internal static void MigrateMSAA(ref FrameSettings cameraFrameSettings, ref FrameSettingsOverrideMask newFrameSettingsOverrideMask)
		{
			if (cameraFrameSettings.IsEnabled(FrameSettingsField.MSAA))
			{
				cameraFrameSettings.msaaMode = MSAAMode.FromHDRPAsset;
			}
			else
			{
				cameraFrameSettings.msaaMode = MSAAMode.None;
			}
			newFrameSettingsOverrideMask.mask[4u] = newFrameSettingsOverrideMask.mask[31u];
			newFrameSettingsOverrideMask.mask[31u] = false;
		}

		internal static void MigrateToCustomPostprocessAndCustomPass(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.CustomPass, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.CustomPostProcess, value: true);
		}

		internal static void MigrateToAfterPostprocess(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.AfterPostprocess, value: true);
		}

		internal static void MigrateToDefaultReflectionSettings(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.ReflectionProbe, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.PlanarProbe, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.ReplaceDiffuseForIndirect, value: false);
			cameraFrameSettings.SetEnabled(FrameSettingsField.SkyReflection, value: true);
		}

		internal static void MigrateToNoReflectionRealtimeSettings(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.ReflectionProbe, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.PlanarProbe, value: false);
			cameraFrameSettings.SetEnabled(FrameSettingsField.ReplaceDiffuseForIndirect, value: false);
			cameraFrameSettings.SetEnabled(FrameSettingsField.SkyReflection, value: true);
		}

		internal static void MigrateToNoReflectionSettings(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.ReflectionProbe, value: false);
			cameraFrameSettings.SetEnabled(FrameSettingsField.PlanarProbe, value: false);
			cameraFrameSettings.SetEnabled(FrameSettingsField.ReplaceDiffuseForIndirect, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.SkyReflection, value: false);
		}

		internal static void MigrateToPostProcess(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.StopNaN, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.DepthOfField, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.MotionBlur, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.PaniniProjection, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.Bloom, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.LensFlareDataDriven, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.LensDistortion, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.ChromaticAberration, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.Vignette, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.ColorGrading, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.FilmGrain, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.Dithering, value: true);
			cameraFrameSettings.SetEnabled(FrameSettingsField.Antialiasing, value: true);
		}

		internal static void MigrateToLensFlare(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.LensFlareDataDriven, value: true);
		}

		internal static void MigrateToDirectSpecularLighting(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.DirectSpecularLighting, value: true);
		}

		internal static void MigrateToNoDirectSpecularLighting(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.DirectSpecularLighting, value: false);
		}

		internal static void MigrateToRayTracing(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.RayTracing, value: true);
		}

		internal static void MigrateToSeparateColorGradingAndTonemapping(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.Tonemapping, value: true);
		}

		internal static void MigrateSubsurfaceParams(ref FrameSettings fs, bool previouslyHighQuality)
		{
			fs.SetEnabled(FrameSettingsField.SubsurfaceScattering, fs.bitDatas[25u]);
			fs.sssQualityMode = (previouslyHighQuality ? SssQualityMode.OverrideQualitySettings : SssQualityMode.FromQualitySettings);
			fs.sssQualityLevel = 0;
			fs.sssCustomSampleBudget = (previouslyHighQuality ? 55 : 20);
		}

		internal static void MigrateRoughDistortion(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.RoughDistortion, value: true);
		}

		internal static void MigrateVirtualTexturing(ref FrameSettings cameraFrameSettings)
		{
			cameraFrameSettings.SetEnabled(FrameSettingsField.VirtualTexturing, value: true);
		}
	}
}
