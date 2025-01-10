using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[RequireComponent(typeof(Camera))]
	public class HDAdditionalCameraData : MonoBehaviour, IFrameSettingsHistoryContainer, IDebugData, IAdditionalData, IVersionable<HDAdditionalCameraData.Version>
	{
		public enum FlipYMode
		{
			Automatic = 0,
			ForceFlipY = 1
		}

		[Flags]
		public enum BufferAccessType
		{
			Depth = 1,
			Normal = 2,
			Color = 4
		}

		public struct BufferAccess
		{
			internal BufferAccessType bufferAccess;

			internal void Reset()
			{
				bufferAccess = (BufferAccessType)0;
			}

			public void RequestAccess(BufferAccessType flags)
			{
				bufferAccess |= flags;
			}
		}

		public delegate Matrix4x4 NonObliqueProjectionGetter(Camera camera);

		public enum ClearColorMode
		{
			Sky = 0,
			Color = 1,
			None = 2
		}

		public enum AntialiasingMode
		{
			[InspectorName("No Anti-aliasing")]
			None = 0,
			[InspectorName("Fast Approximate Anti-aliasing (FXAA)")]
			FastApproximateAntialiasing = 1,
			[InspectorName("Temporal Anti-aliasing (TAA)")]
			TemporalAntialiasing = 2,
			[InspectorName("Subpixel Morphological Anti-aliasing (SMAA)")]
			SubpixelMorphologicalAntiAliasing = 3
		}

		public enum SMAAQualityLevel
		{
			Low = 0,
			Medium = 1,
			High = 2
		}

		public enum TAAQualityLevel
		{
			Low = 0,
			Medium = 1,
			High = 2
		}

		public delegate void RequestAccessDelegate(ref BufferAccess bufferAccess);

		protected enum Version
		{
			None = 0,
			First = 1,
			SeparatePassThrough = 2,
			UpgradingFrameSettingsToStruct = 3,
			AddAfterPostProcessFrameSetting = 4,
			AddFrameSettingSpecularLighting = 5,
			AddReflectionSettings = 6,
			AddCustomPostprocessAndCustomPass = 7,
			UpdateMSAA = 8,
			UpdatePhysicalCameraPropertiesToCore = 9
		}

		[ExcludeCopy]
		private Camera m_Camera;

		public ClearColorMode clearColorMode;

		[ColorUsage(true, true)]
		public Color backgroundColorHDR = new Color(0.025f, 0.07f, 0.19f, 0f);

		public bool clearDepth = true;

		[Tooltip("LayerMask HDRP uses for Volume interpolation for this Camera.")]
		public LayerMask volumeLayerMask = 1;

		public Transform volumeAnchorOverride;

		public AntialiasingMode antialiasing;

		public SMAAQualityLevel SMAAQuality = SMAAQualityLevel.High;

		public bool dithering;

		public bool stopNaNs;

		[Range(0f, 2f)]
		public float taaSharpenStrength = 0.5f;

		public TAAQualityLevel TAAQuality = TAAQualityLevel.Medium;

		[Range(0f, 1f)]
		public float taaHistorySharpening = 0.35f;

		[Range(0f, 1f)]
		public float taaAntiFlicker = 0.5f;

		[Range(0f, 1f)]
		public float taaMotionVectorRejection;

		public bool taaAntiHistoryRinging;

		[Range(0.6f, 0.95f)]
		public float taaBaseBlendFactor = 0.875f;

		[Range(0.1f, 1f)]
		public float taaJitterScale = 1f;

		[ValueCopy]
		[Obsolete("Physical camera properties have been migrated to Camera.", false)]
		public HDPhysicalCamera physicalParameters = HDPhysicalCamera.GetDefaults();

		public FlipYMode flipYMode;

		public bool xrRendering = true;

		[Tooltip("Skips rendering settings to directly render in fullscreen (Useful for video).")]
		public bool fullscreenPassthrough;

		[Tooltip("Allows dynamic resolution on buffers linked to this camera.")]
		public bool allowDynamicResolution;

		[Tooltip("Allows you to override the default settings for this camera.")]
		public bool customRenderingSettings;

		public bool invertFaceCulling;

		public LayerMask probeLayerMask = -1;

		public bool hasPersistentHistory;

		public Vector4 screenSizeOverride;

		public Vector4 screenCoordScaleBias;

		[Tooltip("Allow NVIDIA Deep Learning Super Sampling (DLSS) on this camera")]
		public bool allowDeepLearningSuperSampling = true;

		[Tooltip("If set to true, NVIDIA Deep Learning Super Sampling (DLSS) will utilize the Quality setting set on this camera instead of the one specified in the quality asset.")]
		public bool deepLearningSuperSamplingUseCustomQualitySettings;

		[Tooltip("Selects a performance quality setting for NVIDIA Deep Learning Super Sampling (DLSS) for this camera of this project.")]
		public uint deepLearningSuperSamplingQuality;

		[Tooltip("If set to true, NVIDIA Deep Learning Super Sampling (DLSS) will utilize the attributes (Optimal Settings and Sharpness) specified on this camera, instead of the ones specified in the quality asset of this project.")]
		public bool deepLearningSuperSamplingUseCustomAttributes;

		[Tooltip("Sets the sharpness and scale automatically for NVIDIA Deep Learning Super Sampling (DLSS) for this camera, depending on the values of quality settings.")]
		public bool deepLearningSuperSamplingUseOptimalSettings = true;

		[Tooltip("Sets the Sharpening value for NVIDIA Deep Learning Super Sampling (DLSS) for this camera.")]
		[Range(0f, 1f)]
		public float deepLearningSuperSamplingSharpening;

		[ExcludeCopy]
		internal bool cameraCanRenderDLSS;

		[Tooltip("If set to true, AMD FidelityFX Super Resolution (FSR) will utilize the sharpness setting set on this camera instead of the one specified in the quality asset.")]
		public bool fsrOverrideSharpness;

		[Tooltip("Sets this camera's sharpness value for AMD FidelityFX Super Resolution 1.0 (FSR).")]
		[Range(0f, 1f)]
		public float fsrSharpness = 0.92f;

		public GameObject exposureTarget;

		public float materialMipBias;

		internal float probeCustomFixedExposure = 1f;

		[ExcludeCopy]
		internal float deExposureMultiplier = 1f;

		[SerializeField]
		[FormerlySerializedAs("renderingPathCustomFrameSettings")]
		private FrameSettings m_RenderingPathCustomFrameSettings = FrameSettings.NewDefaultCamera();

		public FrameSettingsOverrideMask renderingPathCustomFrameSettingsOverrideMask;

		public FrameSettingsRenderType defaultFrameSettings;

		[ExcludeCopy]
		private FrameSettingsHistory m_RenderingPathHistory = new FrameSettingsHistory
		{
			defaultType = FrameSettingsRenderType.Camera
		};

		[ExcludeCopy]
		internal ProfilingSampler profilingSampler;

		[ExcludeCopy]
		private AOVRequestDataCollection m_AOVRequestDataCollection = new AOVRequestDataCollection(null);

		[ExcludeCopy]
		private bool m_IsDebugRegistered;

		[ExcludeCopy]
		private string m_CameraRegisterName;

		[ExcludeCopy]
		public NonObliqueProjectionGetter nonObliqueProjectionGetter = GeometryUtils.CalculateProjectionMatrix;

		[SerializeField]
		[FormerlySerializedAs("version")]
		[ExcludeCopy]
		private Version m_Version = MigrationDescription.LastVersion<Version>();

		private static readonly MigrationDescription<Version, HDAdditionalCameraData> k_Migration = MigrationDescription.New<Version, HDAdditionalCameraData>(MigrationStep.New(Version.SeparatePassThrough, delegate(HDAdditionalCameraData data)
		{
			switch (data.m_ObsoleteRenderingPath)
			{
			case 0:
				data.fullscreenPassthrough = false;
				data.customRenderingSettings = false;
				break;
			case 1:
				data.fullscreenPassthrough = false;
				data.customRenderingSettings = true;
				break;
			case 2:
				data.fullscreenPassthrough = true;
				data.customRenderingSettings = false;
				break;
			}
		}), MigrationStep.New(Version.UpgradingFrameSettingsToStruct, delegate(HDAdditionalCameraData data)
		{
			if (data.m_ObsoleteFrameSettings != null)
			{
				FrameSettings.MigrateFromClassVersion(ref data.m_ObsoleteFrameSettings, ref data.renderingPathCustomFrameSettings, ref data.renderingPathCustomFrameSettingsOverrideMask);
			}
		}), MigrationStep.New(Version.AddAfterPostProcessFrameSetting, delegate(HDAdditionalCameraData data)
		{
			FrameSettings.MigrateToAfterPostprocess(ref data.renderingPathCustomFrameSettings);
		}), MigrationStep.New(Version.AddReflectionSettings, delegate(HDAdditionalCameraData data)
		{
			FrameSettings.MigrateToDefaultReflectionSettings(ref data.renderingPathCustomFrameSettings);
		}), MigrationStep.New(Version.AddCustomPostprocessAndCustomPass, delegate(HDAdditionalCameraData data)
		{
			FrameSettings.MigrateToCustomPostprocessAndCustomPass(ref data.renderingPathCustomFrameSettings);
		}), MigrationStep.New(Version.UpdateMSAA, delegate(HDAdditionalCameraData data)
		{
			FrameSettings.MigrateMSAA(ref data.renderingPathCustomFrameSettings, ref data.renderingPathCustomFrameSettingsOverrideMask);
		}), MigrationStep.New(Version.UpdatePhysicalCameraPropertiesToCore, delegate(HDAdditionalCameraData data)
		{
			Camera component = data.GetComponent<Camera>();
			HDPhysicalCamera hDPhysicalCamera = data.physicalParameters;
			if (component != null)
			{
				component.iso = hDPhysicalCamera.iso;
				component.shutterSpeed = hDPhysicalCamera.shutterSpeed;
				component.aperture = hDPhysicalCamera.aperture;
				component.focusDistance = hDPhysicalCamera.focusDistance;
				component.bladeCount = hDPhysicalCamera.bladeCount;
				component.curvature = hDPhysicalCamera.curvature;
				component.barrelClipping = hDPhysicalCamera.barrelClipping;
				component.anamorphism = hDPhysicalCamera.anamorphism;
			}
		}));

		[SerializeField]
		[FormerlySerializedAs("renderingPath")]
		[Obsolete("For Data Migration")]
		[ExcludeCopy]
		private int m_ObsoleteRenderingPath;

		[SerializeField]
		[FormerlySerializedAs("serializedFrameSettings")]
		[FormerlySerializedAs("m_FrameSettings")]
		[ExcludeCopy]
		private ObsoleteFrameSettings m_ObsoleteFrameSettings;

		public bool hasCustomRender => this.customRender != null;

		public ref FrameSettings renderingPathCustomFrameSettings => ref m_RenderingPathCustomFrameSettings;

		bool IFrameSettingsHistoryContainer.hasCustomFrameSettings => customRenderingSettings;

		FrameSettingsOverrideMask IFrameSettingsHistoryContainer.frameSettingsMask => renderingPathCustomFrameSettingsOverrideMask;

		FrameSettings IFrameSettingsHistoryContainer.frameSettings => m_RenderingPathCustomFrameSettings;

		FrameSettingsHistory IFrameSettingsHistoryContainer.frameSettingsHistory
		{
			get
			{
				return m_RenderingPathHistory;
			}
			set
			{
				m_RenderingPathHistory = value;
			}
		}

		string IFrameSettingsHistoryContainer.panelName => m_CameraRegisterName;

		public IEnumerable<AOVRequestData> aovRequests => m_AOVRequestDataCollection ?? (m_AOVRequestDataCollection = new AOVRequestDataCollection(null));

		[field: ExcludeCopy]
		public bool isEditorCameraPreview { get; internal set; }

		Version IVersionable<Version>.version
		{
			get
			{
				return m_Version;
			}
			set
			{
				m_Version = value;
			}
		}

		public event Action<ScriptableRenderContext, HDCamera> customRender;

		public event RequestAccessDelegate requestGraphicsBuffer;

		Action IDebugData.GetReset()
		{
			return delegate
			{
				m_RenderingPathHistory.TriggerReset();
			};
		}

		public void SetAOVRequests(AOVRequestDataCollection aovRequests)
		{
			m_AOVRequestDataCollection = aovRequests;
		}

		public void CopyTo(HDAdditionalCameraData data)
		{
			data.clearColorMode = clearColorMode;
			data.backgroundColorHDR = backgroundColorHDR;
			data.clearDepth = clearDepth;
			data.customRenderingSettings = customRenderingSettings;
			data.volumeLayerMask = volumeLayerMask;
			data.volumeAnchorOverride = volumeAnchorOverride;
			data.antialiasing = antialiasing;
			data.dithering = dithering;
			data.xrRendering = xrRendering;
			data.SMAAQuality = SMAAQuality;
			data.stopNaNs = stopNaNs;
			data.taaSharpenStrength = taaSharpenStrength;
			data.TAAQuality = TAAQuality;
			data.taaHistorySharpening = taaHistorySharpening;
			data.taaAntiFlicker = taaAntiFlicker;
			data.taaMotionVectorRejection = taaMotionVectorRejection;
			data.taaAntiHistoryRinging = taaAntiHistoryRinging;
			data.taaBaseBlendFactor = taaBaseBlendFactor;
			data.taaJitterScale = taaJitterScale;
			data.flipYMode = flipYMode;
			data.fullscreenPassthrough = fullscreenPassthrough;
			data.allowDynamicResolution = allowDynamicResolution;
			data.invertFaceCulling = invertFaceCulling;
			data.probeLayerMask = probeLayerMask;
			data.hasPersistentHistory = hasPersistentHistory;
			data.exposureTarget = exposureTarget;
			data.physicalParameters = physicalParameters;
			data.renderingPathCustomFrameSettings = renderingPathCustomFrameSettings;
			data.renderingPathCustomFrameSettingsOverrideMask = renderingPathCustomFrameSettingsOverrideMask;
			data.defaultFrameSettings = defaultFrameSettings;
			data.probeCustomFixedExposure = probeCustomFixedExposure;
			data.allowDeepLearningSuperSampling = allowDeepLearningSuperSampling;
			data.deepLearningSuperSamplingUseCustomQualitySettings = deepLearningSuperSamplingUseCustomQualitySettings;
			data.deepLearningSuperSamplingQuality = deepLearningSuperSamplingQuality;
			data.deepLearningSuperSamplingUseCustomAttributes = deepLearningSuperSamplingUseCustomAttributes;
			data.deepLearningSuperSamplingUseOptimalSettings = deepLearningSuperSamplingUseOptimalSettings;
			data.deepLearningSuperSamplingSharpening = deepLearningSuperSamplingSharpening;
			data.fsrOverrideSharpness = fsrOverrideSharpness;
			data.fsrSharpness = fsrSharpness;
			data.materialMipBias = materialMipBias;
			data.screenSizeOverride = screenSizeOverride;
			data.screenCoordScaleBias = screenCoordScaleBias;
		}

		public Matrix4x4 GetNonObliqueProjection(Camera camera)
		{
			return nonObliqueProjectionGetter(camera);
		}

		private void RegisterDebug()
		{
			if (!m_IsDebugRegistered)
			{
				m_CameraRegisterName = base.name;
				if (m_Camera.cameraType != CameraType.Preview && m_Camera.cameraType != CameraType.Reflection)
				{
					DebugDisplaySettings.RegisterCamera(this);
				}
				m_IsDebugRegistered = true;
			}
		}

		private void UnRegisterDebug()
		{
			if (!m_IsDebugRegistered)
			{
				return;
			}
			if (m_Camera.cameraType != CameraType.Preview)
			{
				Camera camera = m_Camera;
				if ((object)camera == null || camera.cameraType != CameraType.Reflection)
				{
					DebugDisplaySettings.UnRegisterCamera(this);
				}
			}
			m_IsDebugRegistered = false;
		}

		private void OnEnable()
		{
			m_Camera = GetComponent<Camera>();
			if (!(m_Camera == null))
			{
				m_Camera.allowMSAA = false;
				m_Camera.allowHDR = false;
				FrameSettings aggregatedFrameSettings = default(FrameSettings);
				FrameSettingsHistory.AggregateFrameSettings(ref aggregatedFrameSettings, m_Camera, this, HDRenderPipeline.currentAsset, null);
				RegisterDebug();
			}
		}

		private void UpdateDebugCameraName()
		{
			profilingSampler = new ProfilingSampler(HDUtils.ComputeCameraName(base.name));
			if (base.name != m_CameraRegisterName)
			{
				UnRegisterDebug();
				RegisterDebug();
			}
		}

		private void OnDisable()
		{
			UnRegisterDebug();
		}

		internal static void InitDefaultHDAdditionalCameraData(HDAdditionalCameraData cameraData)
		{
			Camera component = cameraData.gameObject.GetComponent<Camera>();
			cameraData.clearDepth = component.clearFlags != CameraClearFlags.Nothing;
			if (component.clearFlags == CameraClearFlags.Skybox)
			{
				cameraData.clearColorMode = ClearColorMode.Sky;
			}
			else if (component.clearFlags == CameraClearFlags.Color)
			{
				cameraData.clearColorMode = ClearColorMode.Color;
			}
			else
			{
				cameraData.clearColorMode = ClearColorMode.None;
			}
		}

		internal void ExecuteCustomRender(ScriptableRenderContext renderContext, HDCamera hdCamera)
		{
			if (this.customRender != null)
			{
				this.customRender(renderContext, hdCamera);
			}
		}

		internal BufferAccessType GetBufferAccess()
		{
			BufferAccess bufferAccess = default(BufferAccess);
			this.requestGraphicsBuffer?.Invoke(ref bufferAccess);
			return bufferAccess.bufferAccess;
		}

		public RTHandle GetGraphicsBuffer(BufferAccessType type)
		{
			HDCamera orCreate = HDCamera.GetOrCreate(m_Camera);
			if ((type & BufferAccessType.Color) != 0)
			{
				return orCreate.GetCurrentFrameRT(0);
			}
			if ((type & BufferAccessType.Depth) != 0)
			{
				return orCreate.GetCurrentFrameRT(6);
			}
			if ((type & BufferAccessType.Normal) != 0)
			{
				return orCreate.GetCurrentFrameRT(5);
			}
			return null;
		}

		private void Awake()
		{
			k_Migration.Migrate(this);
		}
	}
}
