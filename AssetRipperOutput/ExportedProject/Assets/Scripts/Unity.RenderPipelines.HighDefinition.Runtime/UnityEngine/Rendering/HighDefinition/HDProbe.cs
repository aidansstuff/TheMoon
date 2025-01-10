using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[ExecuteAlways]
	public abstract class HDProbe : MonoBehaviour, IVersionable<HDProbe.Version>
	{
		[Serializable]
		public struct RenderData
		{
			[SerializeField]
			[FormerlySerializedAs("worldToCameraRHS")]
			private Matrix4x4 m_WorldToCameraRHS;

			[SerializeField]
			[FormerlySerializedAs("projectionMatrix")]
			private Matrix4x4 m_ProjectionMatrix;

			[SerializeField]
			[FormerlySerializedAs("capturePosition")]
			private Vector3 m_CapturePosition;

			[SerializeField]
			private Quaternion m_CaptureRotation;

			[SerializeField]
			private float m_FieldOfView;

			[SerializeField]
			private float m_Aspect;

			public Matrix4x4 worldToCameraRHS => m_WorldToCameraRHS;

			public Matrix4x4 projectionMatrix => m_ProjectionMatrix;

			public Vector3 capturePosition => m_CapturePosition;

			public Quaternion captureRotation => m_CaptureRotation;

			public float fieldOfView => m_FieldOfView;

			public float aspect => m_Aspect;

			public RenderData(CameraSettings camera, CameraPositionSettings position)
				: this(position.GetUsedWorldToCameraMatrix(), camera.frustum.GetUsedProjectionMatrix(), position.position, position.rotation, camera.frustum.fieldOfView, camera.frustum.aspect)
			{
			}

			public RenderData(Matrix4x4 worldToCameraRHS, Matrix4x4 projectionMatrix, Vector3 capturePosition, Quaternion captureRotation, float fov, float aspect)
			{
				m_WorldToCameraRHS = worldToCameraRHS;
				m_ProjectionMatrix = projectionMatrix;
				m_CapturePosition = capturePosition;
				m_CaptureRotation = captureRotation;
				m_FieldOfView = fov;
				m_Aspect = aspect;
			}
		}

		protected enum Version
		{
			Initial = 0,
			ProbeSettings = 1,
			SeparatePassThrough = 2,
			UpgradeFrameSettingsToStruct = 3,
			AddFrameSettingSpecularLighting = 4,
			AddReflectionFrameSetting = 5,
			AddFrameSettingDirectSpecularLighting = 6,
			PlanarResolutionScalability = 7,
			UpdateMSAA = 8
		}

		[SerializeField]
		protected ProbeSettings m_ProbeSettings = ProbeSettings.NewDefault();

		[SerializeField]
		private ProbeSettingsOverride m_ProbeSettingsOverride;

		[SerializeField]
		private ReflectionProxyVolumeComponent m_ProxyVolume;

		[SerializeField]
		private Texture m_BakedTexture;

		[SerializeField]
		private Texture m_CustomTexture;

		[SerializeField]
		private RenderData m_BakedRenderData;

		[SerializeField]
		private RenderData m_CustomRenderData;

		private RTHandle m_RealtimeTexture;

		private RTHandle m_RealtimeDepthBuffer;

		private RenderData m_RealtimeRenderData;

		private bool m_WasRenderedSinceLastOnDemandRequest = true;

		private ProbeRenderSteps m_RemainingRenderSteps;

		private bool m_HasPendingRenderRequest;

		private uint m_RealtimeRenderCount;

		private int m_LastStepFrameCount = -1;

		[SerializeField]
		private bool m_HasValidSHForNormalization;

		[SerializeField]
		private SphericalHarmonicsL2 m_SHForNormalization;

		[SerializeField]
		private Vector3 m_SHValidForCapturePosition;

		[SerializeField]
		private Vector3 m_SHValidForSourcePosition;

		internal string[] probeName = new string[6];

		private float m_ProbeExposureValue = 1f;

		protected static readonly MigrationDescription<Version, HDProbe> k_Migration = MigrationDescription.New<Version, HDProbe>(MigrationStep.New(Version.ProbeSettings, delegate(HDProbe p)
		{
			p.m_ProbeSettings.influence = new InfluenceVolume();
			if (p.m_ObsoleteInfluenceVolume != null)
			{
				p.m_ObsoleteInfluenceVolume.CopyTo(p.m_ProbeSettings.influence);
			}
			p.m_ProbeSettings.cameraSettings.m_ObsoleteFrameSettings = p.m_ObsoleteFrameSettings;
			p.m_ProbeSettings.lighting.multiplier = p.m_ObsoleteMultiplier;
			p.m_ProbeSettings.lighting.weight = p.m_ObsoleteWeight;
			p.m_ProbeSettings.lighting.lightLayer = p.m_ObsoleteLightLayers;
			p.m_ProbeSettings.mode = p.m_ObsoleteMode;
			p.m_ProbeSettings.cameraSettings.bufferClearing.clearColorMode = p.m_ObsoleteCaptureSettings.clearColorMode;
			p.m_ProbeSettings.cameraSettings.bufferClearing.backgroundColorHDR = p.m_ObsoleteCaptureSettings.backgroundColorHDR;
			p.m_ProbeSettings.cameraSettings.bufferClearing.clearDepth = p.m_ObsoleteCaptureSettings.clearDepth;
			p.m_ProbeSettings.cameraSettings.culling.cullingMask = p.m_ObsoleteCaptureSettings.cullingMask;
			p.m_ProbeSettings.cameraSettings.culling.useOcclusionCulling = p.m_ObsoleteCaptureSettings.useOcclusionCulling;
			p.m_ProbeSettings.cameraSettings.frustum.nearClipPlaneRaw = p.m_ObsoleteCaptureSettings.nearClipPlane;
			p.m_ProbeSettings.cameraSettings.frustum.farClipPlaneRaw = p.m_ObsoleteCaptureSettings.farClipPlane;
			p.m_ProbeSettings.cameraSettings.volumes.layerMask = p.m_ObsoleteCaptureSettings.volumeLayerMask;
			p.m_ProbeSettings.cameraSettings.volumes.anchorOverride = p.m_ObsoleteCaptureSettings.volumeAnchorOverride;
			p.m_ProbeSettings.cameraSettings.frustum.fieldOfView = p.m_ObsoleteCaptureSettings.fieldOfView;
			p.m_ProbeSettings.cameraSettings.m_ObsoleteRenderingPath = p.m_ObsoleteCaptureSettings.renderingPath;
		}), MigrationStep.New(Version.SeparatePassThrough, delegate(HDProbe p)
		{
			p.m_ProbeSettings.cameraSettings.customRenderingSettings = p.m_ProbeSettings.cameraSettings.m_ObsoleteRenderingPath == 1;
		}), MigrationStep.New(Version.UpgradeFrameSettingsToStruct, delegate(HDProbe data)
		{
			if (data.m_ObsoleteFrameSettings != null)
			{
				FrameSettings.MigrateFromClassVersion(ref data.m_ProbeSettings.cameraSettings.m_ObsoleteFrameSettings, ref data.m_ProbeSettings.cameraSettings.renderingPathCustomFrameSettings, ref data.m_ProbeSettings.cameraSettings.renderingPathCustomFrameSettingsOverrideMask);
			}
		}), MigrationStep.New(Version.AddReflectionFrameSetting, delegate(HDProbe data)
		{
			FrameSettings.MigrateToNoReflectionSettings(ref data.m_ProbeSettings.cameraSettings.renderingPathCustomFrameSettings);
		}), MigrationStep.New(Version.AddFrameSettingDirectSpecularLighting, delegate(HDProbe data)
		{
			FrameSettings.MigrateToNoDirectSpecularLighting(ref data.m_ProbeSettings.cameraSettings.renderingPathCustomFrameSettings);
		}), MigrationStep.New(Version.UpdateMSAA, delegate(HDProbe data)
		{
			FrameSettings.MigrateMSAA(ref data.m_ProbeSettings.cameraSettings.renderingPathCustomFrameSettings, ref data.m_ProbeSettings.cameraSettings.renderingPathCustomFrameSettingsOverrideMask);
		}));

		[SerializeField]
		private Version m_HDProbeVersion;

		[SerializeField]
		[FormerlySerializedAs("m_InfiniteProjection")]
		[Obsolete("For Data Migration")]
		protected bool m_ObsoleteInfiniteProjection = true;

		[SerializeField]
		[FormerlySerializedAs("m_InfluenceVolume")]
		[Obsolete("For Data Migration")]
		protected InfluenceVolume m_ObsoleteInfluenceVolume;

		[SerializeField]
		[FormerlySerializedAs("m_FrameSettings")]
		[Obsolete("For Data Migration")]
		private ObsoleteFrameSettings m_ObsoleteFrameSettings;

		[SerializeField]
		[FormerlySerializedAs("m_Multiplier")]
		[FormerlySerializedAs("dimmer")]
		[FormerlySerializedAs("m_Dimmer")]
		[FormerlySerializedAs("multiplier")]
		[Obsolete("For Data Migration")]
		protected float m_ObsoleteMultiplier = 1f;

		[SerializeField]
		[FormerlySerializedAs("m_Weight")]
		[FormerlySerializedAs("weight")]
		[Obsolete("For Data Migration")]
		[Range(0f, 1f)]
		protected float m_ObsoleteWeight = 1f;

		[SerializeField]
		[FormerlySerializedAs("m_Mode")]
		[Obsolete("For Data Migration")]
		protected ProbeSettings.Mode m_ObsoleteMode;

		[SerializeField]
		[FormerlySerializedAs("lightLayers")]
		[Obsolete("For Data Migration")]
		private LightLayerEnum m_ObsoleteLightLayers = LightLayerEnum.LightLayerDefault;

		[SerializeField]
		[FormerlySerializedAs("m_CaptureSettings")]
		[Obsolete("For Data Migration")]
		internal ObsoleteCaptureSettings m_ObsoleteCaptureSettings;

		public bool ExposureControlEnabled { get; set; }

		internal bool requiresRealtimeUpdate
		{
			get
			{
				if (mode != ProbeSettings.Mode.Realtime)
				{
					return false;
				}
				switch (realtimeMode)
				{
				case ProbeSettings.RealtimeMode.EveryFrame:
					return true;
				case ProbeSettings.RealtimeMode.OnEnable:
					if (wasRenderedAfterOnEnable)
					{
						return HasRemainingRenderSteps();
					}
					return true;
				case ProbeSettings.RealtimeMode.OnDemand:
					if (m_WasRenderedSinceLastOnDemandRequest)
					{
						return HasRemainingRenderSteps();
					}
					return true;
				default:
					throw new ArgumentOutOfRangeException("realtimeMode");
				}
			}
		}

		public Texture bakedTexture
		{
			get
			{
				return m_BakedTexture;
			}
			set
			{
				m_BakedTexture = value;
			}
		}

		public Texture customTexture
		{
			get
			{
				return m_CustomTexture;
			}
			set
			{
				m_CustomTexture = value;
			}
		}

		public RenderTexture realtimeTexture
		{
			get
			{
				return (m_RealtimeTexture != null) ? m_RealtimeTexture : null;
			}
			set
			{
				if (m_RealtimeTexture != null)
				{
					m_RealtimeTexture.Release();
				}
				m_RealtimeTexture = RTHandles.Alloc(value);
				m_RealtimeTexture.rt.name = "ProbeRealTimeTexture_" + base.name;
			}
		}

		public RenderTexture realtimeDepthTexture
		{
			get
			{
				return (m_RealtimeDepthBuffer != null) ? m_RealtimeDepthBuffer : null;
			}
			set
			{
				if (m_RealtimeDepthBuffer != null)
				{
					m_RealtimeDepthBuffer.Release();
				}
				m_RealtimeDepthBuffer = RTHandles.Alloc(value);
				m_RealtimeDepthBuffer.rt.name = "ProbeRealTimeDepthTexture_" + base.name;
			}
		}

		public RTHandle realtimeTextureRTH => m_RealtimeTexture;

		public RTHandle realtimeDepthTextureRTH => m_RealtimeDepthBuffer;

		public Texture texture => GetTexture(mode);

		public RenderData bakedRenderData
		{
			get
			{
				return m_BakedRenderData;
			}
			set
			{
				m_BakedRenderData = value;
			}
		}

		public RenderData customRenderData
		{
			get
			{
				return m_CustomRenderData;
			}
			set
			{
				m_CustomRenderData = value;
			}
		}

		public RenderData realtimeRenderData
		{
			get
			{
				return m_RealtimeRenderData;
			}
			set
			{
				m_RealtimeRenderData = value;
			}
		}

		public RenderData renderData => GetRenderData(mode);

		public ProbeSettings.ProbeType type
		{
			get
			{
				return m_ProbeSettings.type;
			}
			protected set
			{
				m_ProbeSettings.type = value;
			}
		}

		public ProbeSettings.Mode mode
		{
			get
			{
				return m_ProbeSettings.mode;
			}
			set
			{
				m_ProbeSettings.mode = value;
			}
		}

		public ProbeSettings.RealtimeMode realtimeMode
		{
			get
			{
				return m_ProbeSettings.realtimeMode;
			}
			set
			{
				m_ProbeSettings.realtimeMode = value;
			}
		}

		public bool timeSlicing
		{
			get
			{
				return m_ProbeSettings.timeSlicing;
			}
			set
			{
				m_ProbeSettings.timeSlicing = value;
			}
		}

		public PlanarReflectionAtlasResolution resolution
		{
			get
			{
				HDRenderPipeline hDRenderPipeline = (HDRenderPipeline)RenderPipelineManager.currentPipeline;
				if (hDRenderPipeline == null)
				{
					return m_ProbeSettings.resolution;
				}
				return m_ProbeSettings.resolutionScalable.Value(hDRenderPipeline.asset.currentPlatformRenderPipelineSettings.planarReflectionResolution);
			}
		}

		public CubeReflectionResolution cubeResolution
		{
			get
			{
				HDRenderPipeline hDRenderPipeline = (HDRenderPipeline)RenderPipelineManager.currentPipeline;
				if (hDRenderPipeline == null)
				{
					return CubeReflectionResolution.CubeReflectionResolution128;
				}
				return m_ProbeSettings.cubeResolution.Value(hDRenderPipeline.asset.currentPlatformRenderPipelineSettings.cubeReflectionResolution);
			}
		}

		public LightLayerEnum lightLayers
		{
			get
			{
				return m_ProbeSettings.lighting.lightLayer;
			}
			set
			{
				m_ProbeSettings.lighting.lightLayer = value;
			}
		}

		public uint lightLayersAsUInt
		{
			get
			{
				if (lightLayers >= LightLayerEnum.Nothing)
				{
					return (uint)lightLayers;
				}
				return 255u;
			}
		}

		public float multiplier
		{
			get
			{
				return m_ProbeSettings.lighting.multiplier;
			}
			set
			{
				m_ProbeSettings.lighting.multiplier = value;
			}
		}

		public float weight
		{
			get
			{
				return m_ProbeSettings.lighting.weight;
			}
			set
			{
				m_ProbeSettings.lighting.weight = value;
			}
		}

		public float fadeDistance
		{
			get
			{
				return m_ProbeSettings.lighting.fadeDistance;
			}
			set
			{
				m_ProbeSettings.lighting.fadeDistance = value;
			}
		}

		public float rangeCompressionFactor
		{
			get
			{
				return m_ProbeSettings.lighting.rangeCompressionFactor;
			}
			set
			{
				m_ProbeSettings.lighting.rangeCompressionFactor = value;
			}
		}

		public ReflectionProxyVolumeComponent proxyVolume
		{
			get
			{
				return m_ProxyVolume;
			}
			set
			{
				m_ProxyVolume = value;
			}
		}

		public bool useInfluenceVolumeAsProxyVolume
		{
			get
			{
				return m_ProbeSettings.proxySettings.useInfluenceVolumeAsProxyVolume;
			}
			internal set
			{
				m_ProbeSettings.proxySettings.useInfluenceVolumeAsProxyVolume = value;
			}
		}

		public bool isProjectionInfinite
		{
			get
			{
				if (!(m_ProxyVolume != null) || m_ProxyVolume.proxyVolume.shape != ProxyShape.Infinite)
				{
					if (m_ProxyVolume == null)
					{
						return !m_ProbeSettings.proxySettings.useInfluenceVolumeAsProxyVolume;
					}
					return false;
				}
				return true;
			}
		}

		public InfluenceVolume influenceVolume
		{
			get
			{
				return m_ProbeSettings.influence ?? (m_ProbeSettings.influence = new InfluenceVolume());
			}
			private set
			{
				m_ProbeSettings.influence = value;
			}
		}

		public ref FrameSettings frameSettings => ref m_ProbeSettings.cameraSettings.renderingPathCustomFrameSettings;

		public ref FrameSettingsOverrideMask frameSettingsOverrideMask => ref m_ProbeSettings.cameraSettings.renderingPathCustomFrameSettingsOverrideMask;

		public Vector3 proxyExtents
		{
			get
			{
				if (!(proxyVolume != null))
				{
					return influenceExtents;
				}
				return proxyVolume.proxyVolume.extents;
			}
		}

		public BoundingSphere boundingSphere => influenceVolume.GetBoundingSphereAt(base.transform.position);

		public Bounds bounds => influenceVolume.GetBoundsAt(base.transform.position);

		public ref ProbeSettings settingsRaw => ref m_ProbeSettings;

		public ProbeSettings settings
		{
			get
			{
				ProbeSettings probeSettings = m_ProbeSettings;
				probeSettings.proxy = m_ProxyVolume?.proxyVolume;
				probeSettings.influence = probeSettings.influence ?? new InfluenceVolume();
				return probeSettings;
			}
		}

		internal Matrix4x4 influenceToWorld => Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);

		internal Vector3 influenceExtents => influenceVolume.extents;

		internal Matrix4x4 proxyToWorld
		{
			get
			{
				if (!(proxyVolume != null))
				{
					return influenceToWorld;
				}
				return Matrix4x4.TRS(proxyVolume.transform.position, proxyVolume.transform.rotation, Vector3.one);
			}
		}

		internal bool wasRenderedAfterOnEnable { get; private set; }

		internal bool hasEverRendered => m_RealtimeRenderCount != 0;

		Version IVersionable<Version>.version
		{
			get
			{
				return m_HDProbeVersion;
			}
			set
			{
				m_HDProbeVersion = value;
			}
		}

		internal void SetProbeExposureValue(float exposure)
		{
			m_ProbeExposureValue = exposure;
		}

		internal float ProbeExposureValue()
		{
			return m_ProbeExposureValue;
		}

		private bool HasRemainingRenderSteps()
		{
			if (m_RemainingRenderSteps.IsNone())
			{
				return m_HasPendingRenderRequest;
			}
			return true;
		}

		private void EnqueueAllRenderSteps()
		{
			ProbeRenderSteps probeRenderSteps = ProbeRenderStepsExt.FromProbeType(type);
			if (m_RemainingRenderSteps != probeRenderSteps)
			{
				m_HasPendingRenderRequest = true;
			}
		}

		internal ProbeRenderSteps NextRenderSteps()
		{
			if (m_RemainingRenderSteps.IsNone() && m_HasPendingRenderRequest)
			{
				m_RemainingRenderSteps = ProbeRenderStepsExt.FromProbeType(type);
				m_HasPendingRenderRequest = false;
			}
			if (type == ProbeSettings.ProbeType.ReflectionProbe)
			{
				ProbeRenderSteps probeRenderSteps = (timeSlicing ? m_RemainingRenderSteps.LowestSetBit() : m_RemainingRenderSteps);
				bool flag = realtimeMode == ProbeSettings.RealtimeMode.EveryFrame || timeSlicing;
				if (!probeRenderSteps.IsNone() && flag)
				{
					int frameCount = Time.frameCount;
					if (m_LastStepFrameCount == frameCount)
					{
						probeRenderSteps = ProbeRenderSteps.None;
					}
					else
					{
						m_LastStepFrameCount = frameCount;
					}
				}
				m_RemainingRenderSteps &= ~probeRenderSteps;
				return probeRenderSteps;
			}
			m_RemainingRenderSteps = ProbeRenderSteps.None;
			return ProbeRenderSteps.PlanarProbeMask;
		}

		internal void IncrementRealtimeRenderCount()
		{
			m_RealtimeRenderCount++;
			texture.IncrementUpdateCount();
		}

		internal void RepeatRenderSteps(ProbeRenderSteps renderSteps)
		{
			m_RemainingRenderSteps |= renderSteps;
		}

		internal uint GetTextureHash()
		{
			if (mode != ProbeSettings.Mode.Realtime)
			{
				return texture.updateCount;
			}
			return m_RealtimeRenderCount;
		}

		internal bool HasValidRenderedData()
		{
			bool flag = texture != null;
			if (mode != ProbeSettings.Mode.Realtime)
			{
				return flag;
			}
			return hasEverRendered && flag;
		}

		public Texture GetTexture(ProbeSettings.Mode targetMode)
		{
			return targetMode switch
			{
				ProbeSettings.Mode.Baked => m_BakedTexture, 
				ProbeSettings.Mode.Custom => m_CustomTexture, 
				ProbeSettings.Mode.Realtime => m_RealtimeTexture, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}

		public Texture SetTexture(ProbeSettings.Mode targetMode, Texture texture)
		{
			if (targetMode == ProbeSettings.Mode.Realtime && !(texture is RenderTexture))
			{
				throw new ArgumentException("'texture' must be a RenderTexture for the Realtime mode.");
			}
			return targetMode switch
			{
				ProbeSettings.Mode.Baked => m_BakedTexture = texture, 
				ProbeSettings.Mode.Custom => m_CustomTexture = texture, 
				ProbeSettings.Mode.Realtime => realtimeTexture = (RenderTexture)texture, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}

		public Texture SetDepthTexture(ProbeSettings.Mode targetMode, Texture texture)
		{
			if (targetMode == ProbeSettings.Mode.Realtime && !(texture is RenderTexture))
			{
				throw new ArgumentException("'texture' must be a RenderTexture for the Realtime mode.");
			}
			return targetMode switch
			{
				ProbeSettings.Mode.Baked => m_BakedTexture = texture, 
				ProbeSettings.Mode.Custom => m_CustomTexture = texture, 
				ProbeSettings.Mode.Realtime => realtimeDepthTexture = (RenderTexture)texture, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}

		public RenderData GetRenderData(ProbeSettings.Mode targetMode)
		{
			return targetMode switch
			{
				ProbeSettings.Mode.Baked => bakedRenderData, 
				ProbeSettings.Mode.Custom => customRenderData, 
				ProbeSettings.Mode.Realtime => realtimeRenderData, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}

		public void SetRenderData(ProbeSettings.Mode targetMode, RenderData renderData)
		{
			switch (targetMode)
			{
			case ProbeSettings.Mode.Baked:
				bakedRenderData = renderData;
				break;
			case ProbeSettings.Mode.Custom:
				customRenderData = renderData;
				break;
			case ProbeSettings.Mode.Realtime:
				realtimeRenderData = renderData;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		internal void SetIsRendered()
		{
			switch (realtimeMode)
			{
			case ProbeSettings.RealtimeMode.EveryFrame:
				EnqueueAllRenderSteps();
				break;
			case ProbeSettings.RealtimeMode.OnEnable:
				if (!wasRenderedAfterOnEnable)
				{
					EnqueueAllRenderSteps();
					wasRenderedAfterOnEnable = true;
				}
				break;
			case ProbeSettings.RealtimeMode.OnDemand:
				if (!m_WasRenderedSinceLastOnDemandRequest)
				{
					EnqueueAllRenderSteps();
					m_WasRenderedSinceLastOnDemandRequest = true;
				}
				break;
			}
		}

		public virtual void PrepareCulling()
		{
		}

		public void RequestRenderNextUpdate()
		{
			m_WasRenderedSinceLastOnDemandRequest = false;
		}

		internal void TryUpdateLuminanceSHL2ForNormalization()
		{
		}

		internal bool GetSHForNormalization(out Vector4 outL0L1, out Vector4 outL2_1, out float outL2_2)
		{
			HDRenderPipeline hDRenderPipeline = (HDRenderPipeline)RenderPipelineManager.currentPipeline;
			if (!m_HasValidSHForNormalization || !hDRenderPipeline.asset.currentPlatformRenderPipelineSettings.supportProbeVolume)
			{
				outL0L1 = (outL2_1 = Vector4.zero);
				outL2_2 = 0f;
				return false;
			}
			if (m_SHForNormalization[0, 0] == float.MaxValue)
			{
				outL0L1 = new Vector4(float.MaxValue, 0f, 0f, 0f);
				outL2_1 = Vector4.zero;
				outL2_2 = 0f;
				return true;
			}
			Vector3 coefficient = SphericalHarmonicsL2Utils.GetCoefficient(m_SHForNormalization, 0);
			Vector3 coefficient2 = SphericalHarmonicsL2Utils.GetCoefficient(m_SHForNormalization, 1);
			Vector3 coefficient3 = SphericalHarmonicsL2Utils.GetCoefficient(m_SHForNormalization, 2);
			Vector3 coefficient4 = SphericalHarmonicsL2Utils.GetCoefficient(m_SHForNormalization, 3);
			Vector3 coefficient5 = SphericalHarmonicsL2Utils.GetCoefficient(m_SHForNormalization, 4);
			Vector3 coefficient6 = SphericalHarmonicsL2Utils.GetCoefficient(m_SHForNormalization, 5);
			Vector3 coefficient7 = SphericalHarmonicsL2Utils.GetCoefficient(m_SHForNormalization, 6);
			Vector3 coefficient8 = SphericalHarmonicsL2Utils.GetCoefficient(m_SHForNormalization, 7);
			Vector3 coefficient9 = SphericalHarmonicsL2Utils.GetCoefficient(m_SHForNormalization, 8);
			if (hDRenderPipeline.asset.currentPlatformRenderPipelineSettings.probeVolumeSHBands == ProbeVolumeSHBands.SphericalHarmonicsL2)
			{
				coefficient -= coefficient7;
				coefficient7 *= 3f;
			}
			Color color = new Color(coefficient.x, coefficient.y, coefficient.z);
			outL0L1.x = ColorUtils.Luminance(in color);
			color = new Color(coefficient2.x, coefficient2.y, coefficient2.z);
			outL0L1.y = ColorUtils.Luminance(in color);
			color = new Color(coefficient3.x, coefficient3.y, coefficient3.z);
			outL0L1.z = ColorUtils.Luminance(in color);
			color = new Color(coefficient4.x, coefficient4.y, coefficient4.z);
			outL0L1.w = ColorUtils.Luminance(in color);
			color = new Color(coefficient5.x, coefficient5.y, coefficient5.z);
			outL2_1.x = ColorUtils.Luminance(in color);
			color = new Color(coefficient6.x, coefficient6.y, coefficient6.z);
			outL2_1.y = ColorUtils.Luminance(in color);
			color = new Color(coefficient7.x, coefficient7.y, coefficient7.z);
			outL2_1.z = ColorUtils.Luminance(in color);
			color = new Color(coefficient8.x, coefficient8.y, coefficient8.z);
			outL2_1.w = ColorUtils.Luminance(in color);
			color = new Color(coefficient9.x, coefficient9.y, coefficient9.z);
			outL2_2 = ColorUtils.Luminance(in color);
			return true;
		}

		private void UpdateProbeName()
		{
			if (settings.type == ProbeSettings.ProbeType.ReflectionProbe)
			{
				for (int i = 0; i < 6; i++)
				{
					probeName[i] = $"Reflection Probe RenderCamera ({base.name}: {(CubemapFace)i})";
				}
			}
			else
			{
				probeName[0] = "Planar Probe RenderCamera (" + base.name + ")";
			}
		}

		private void DequeueSHRequest()
		{
		}

		private void SetOrReleaseCustomTextureReference()
		{
		}

		private void OnEnable()
		{
			wasRenderedAfterOnEnable = false;
			PrepareCulling();
			HDProbeSystem.RegisterProbe(this);
			UpdateProbeName();
		}

		private void OnDisable()
		{
			HDProbeSystem.UnregisterProbe(this);
		}
	}
}
