using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Collections;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	[DebuggerDisplay("({camera.name})")]
	public class HDCamera
	{
		public struct ViewConstants
		{
			public Matrix4x4 viewMatrix;

			public Matrix4x4 invViewMatrix;

			public Matrix4x4 projMatrix;

			public Matrix4x4 invProjMatrix;

			public Matrix4x4 viewProjMatrix;

			public Matrix4x4 invViewProjMatrix;

			public Matrix4x4 nonJitteredViewProjMatrix;

			public Matrix4x4 prevViewMatrix;

			public Matrix4x4 prevViewProjMatrix;

			public Matrix4x4 prevInvViewProjMatrix;

			public Matrix4x4 prevViewProjMatrixNoCameraTrans;

			public Matrix4x4 pixelCoordToViewDirWS;

			internal Matrix4x4 viewProjectionNoCameraTrans;

			public Vector3 worldSpaceCameraPos;

			internal float pad0;

			public Vector3 worldSpaceCameraPosViewOffset;

			internal float pad1;

			public Vector3 prevWorldSpaceCameraPos;

			internal float pad2;
		}

		internal struct ShadowHistoryUsage
		{
			public int lightInstanceID;

			public uint frameCount;

			public GPULightType lightType;

			public Matrix4x4 transform;
		}

		internal enum HistoryEffectSlot
		{
			GlobalIllumination0 = 0,
			GlobalIllumination1 = 1,
			RayTracedReflections = 2,
			VolumetricClouds = 3,
			RayTracedAmbientOcclusion = 4,
			Count = 5
		}

		internal enum HistoryEffectFlags
		{
			FullResolution = 1,
			RayTraced = 2,
			ExposureControl = 4,
			CustomBit0 = 8,
			CustomBit1 = 0x10,
			CustomBit2 = 0x20,
			CustomBit3 = 0x40,
			CustomBit4 = 0x80
		}

		internal struct HistoryEffectValidity
		{
			public int frameCount;

			public int flagMask;
		}

		internal struct VolumetricCloudsAnimationData
		{
			public float lastTime;

			public Vector2 cloudOffset;

			public float verticalShapeOffset;

			public float verticalErosionOffset;
		}

		private struct ExposureGpuReadbackRequest
		{
			public bool isDeExposure;

			public AsyncGPUReadbackRequest request;
		}

		internal struct ExposureTextures
		{
			public bool useCurrentCamera;

			public RTHandle parent;

			public RTHandle current;

			public RTHandle previous;

			public bool useFetchedExposure;

			public float fetchedGpuExposure;

			public void clear()
			{
				parent = null;
				current = null;
				previous = null;
				useFetchedExposure = false;
				fetchedGpuExposure = 1f;
			}
		}

		internal struct DynamicResolutionRequest
		{
			public bool enabled;

			public bool cameraRequested;

			public bool hardwareEnabled;

			public DynamicResUpscaleFilter filter;
		}

		private class ExecuteCaptureActionsPassData
		{
			public TextureHandle input;

			public TextureHandle tempTexture;

			public IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> recorderCaptureActions;

			public Vector2 viewportScale;

			public Material blitMaterial;
		}

		internal struct CustomHistoryAllocator
		{
			private Vector2 scaleFactor;

			private GraphicsFormat format;

			private string name;

			public CustomHistoryAllocator(Vector2 scaleFactor, GraphicsFormat format, string name)
			{
				this.scaleFactor = scaleFactor;
				this.format = format;
				this.name = name;
			}

			public RTHandle Allocator(string id, int frameIndex, RTHandleSystem rtHandleSystem)
			{
				return rtHandleSystem.Alloc(Vector2.one * scaleFactor, TextureXR.slices, DepthBits.None, format, FilterMode.Point, TextureWrapMode.Repeat, TextureXR.dimension, enableRandomWrite: true, useMipMap: false, autoGenerateMips: true, isShadowMap: false, 1, 0f, MSAASamples.None, bindTextureMS: false, useDynamicScale: true, RenderTextureMemoryless.None, VRTextureUsage.None, $"{id}_{name}_{frameIndex}");
			}
		}

		public Vector4 screenSize;

		public Frustum frustum;

		public Camera camera;

		public Vector4 taaJitter;

		public ViewConstants mainViewConstants;

		public bool colorPyramidHistoryIsValid;

		public bool volumetricHistoryIsValid;

		internal int volumetricValidFrames;

		internal int colorPyramidHistoryValidFrames;

		public float time;

		internal bool dofHistoryIsValid;

		internal bool previousFrameWasTAAUpsampled;

		public RayTracingAccelerationStructure rayTracingAccelerationStructure;

		public bool transformsDirty;

		public bool materialsDirty;

		internal Vector4[] frustumPlaneEquations;

		internal int taaFrameIndex;

		internal float taaSharpenStrength;

		internal float taaHistorySharpening;

		internal float taaAntiFlicker;

		internal float taaMotionVectorRejection;

		internal float taaBaseBlendFactor;

		internal float taaJitterScale;

		internal bool taaAntiRinging;

		internal Vector4 zBufferParams;

		internal Vector4 unity_OrthoParams;

		internal Vector4 projectionParams;

		internal Vector4 screenParams;

		internal int volumeLayerMask;

		internal Transform volumeAnchor;

		internal Rect finalViewport = new Rect(Vector2.zero, -1f * Vector2.one);

		internal Rect prevFinalViewport;

		internal int colorPyramidHistoryMipCount;

		internal VBufferParameters[] vBufferParams;

		internal RTHandle[] volumetricHistoryBuffers;

		internal uint cameraFrameCount;

		internal bool animateMaterials;

		internal float lastTime;

		private Camera m_parentCamera;

		internal float lowResScale = 0.5f;

		private Vector4 m_PostProcessScreenSize = new Vector4(0f, 0f, 0f, 0f);

		private Vector4 m_PostProcessRTScales = new Vector4(1f, 1f, 1f, 1f);

		private Vector4 m_PostProcessRTScalesHistory = new Vector4(1f, 1f, 1f, 1f);

		private Vector2Int m_PostProcessRTHistoryMaxReference = new Vector2Int(1, 1);

		internal ShadowHistoryUsage[] shadowHistoryUsage;

		internal HistoryEffectValidity[] historyEffectUsage;

		internal bool realtimeReflectionProbe;

		internal SkyUpdateContext m_LightingOverrideSky = new SkyUpdateContext();

		internal bool isPersistent;

		internal HDUtils.PackedMipChainInfo m_DepthBufferMipChainInfo;

		private HDAdditionalCameraData.ClearColorMode m_PreviousClearColorMode = HDAdditionalCameraData.ClearColorMode.None;

		private float m_GpuExposureValue = 1f;

		private float m_GpuDeExposureValue = 1f;

		private Queue<ExposureGpuReadbackRequest> m_ExposureAsyncRequest = new Queue<ExposureGpuReadbackRequest>();

		private bool m_ExposureControlFS;

		private ExposureTextures m_ExposureTextures = new ExposureTextures
		{
			useCurrentCamera = true,
			current = null,
			previous = null
		};

		internal bool resetPostProcessingHistory = true;

		internal bool didResetPostProcessingHistoryInLastFrame;

		private static Dictionary<(Camera, int), HDCamera> s_Cameras = new Dictionary<(Camera, int), HDCamera>();

		private static List<(Camera, int)> s_Cleanup = new List<(Camera, int)>();

		private HDAdditionalCameraData m_AdditionalCameraData;

		private BufferedRTHandleSystem m_HistoryRTSystem = new BufferedRTHandleSystem();

		private int m_NumVolumetricBuffersAllocated;

		private float m_AmbientOcclusionResolutionScale;

		private float m_ScreenSpaceAccumulationResolutionScale;

		private Dictionary<AOVRequestData, BufferedRTHandleSystem> m_AOVHistoryRTSystem = new Dictionary<AOVRequestData, BufferedRTHandleSystem>(new AOVRequestDataComparer());

		public ScreenSpaceReflectionAlgorithm currentSSRAlgorithm;

		internal ViewConstants[] m_XRViewConstants;

		private IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> m_RecorderCaptureActions;

		private int m_RecorderTempRT = Shader.PropertyToID("TempRecorder");

		private MaterialPropertyBlock m_RecorderPropertyBlock = new MaterialPropertyBlock();

		private Rect? m_OverridePixelRect;

		private DynamicResolutionHandler.UpsamplerScheduleType m_PrevUpsamplerSchedule = DynamicResolutionHandler.UpsamplerScheduleType.AfterPost;

		public string name { get; private set; }

		public Vector4 postProcessScreenSize => m_PostProcessScreenSize;

		public int actualWidth { get; private set; }

		public int actualHeight { get; private set; }

		public MSAASamples msaaSamples { get; private set; }

		public bool msaaEnabled => msaaSamples != MSAASamples.None;

		public FrameSettings frameSettings { get; private set; }

		public RTHandleProperties historyRTHandleProperties => m_HistoryRTSystem.rtHandleProperties;

		public VolumeStack volumeStack { get; private set; }

		internal Camera parentCamera => m_parentCamera;

		internal bool isLowResScaleHalf => lowResScale == 0.5f;

		internal Vector2 postProcessRTScales => new Vector2(m_PostProcessRTScales.x, m_PostProcessRTScales.y);

		internal Vector4 postProcessRTScalesHistory => m_PostProcessRTScalesHistory;

		internal Vector2Int postProcessRTHistoryMaxReference => m_PostProcessRTHistoryMaxReference;

		internal ref HDUtils.PackedMipChainInfo depthBufferMipChainInfo => ref m_DepthBufferMipChainInfo;

		internal Vector2Int depthMipChainSize => m_DepthBufferMipChainInfo.textureSize;

		internal SkyUpdateContext visualSky { get; private set; } = new SkyUpdateContext();


		internal SkyUpdateContext lightingSky { get; private set; }

		internal SkyAmbientMode skyAmbientMode { get; private set; }

		internal XRPass xr { get; private set; }

		internal float globalMipBias { get; set; }

		internal float deltaTime => time - lastTime;

		internal float animateMaterialsTime { get; set; } = -1f;


		internal float animateMaterialsTimeLast { get; set; } = -1f;


		internal Matrix4x4 nonObliqueProjMatrix
		{
			get
			{
				if (!(m_AdditionalCameraData != null))
				{
					return GeometryUtils.CalculateProjectionMatrix(camera);
				}
				return m_AdditionalCameraData.GetNonObliqueProjection(camera);
			}
		}

		internal bool isFirstFrame { get; private set; }

		internal bool isMainGameView
		{
			get
			{
				if (camera.cameraType == CameraType.Game)
				{
					return camera.targetTexture == null;
				}
				return false;
			}
		}

		internal bool canDoDynamicResolution => camera.cameraType == CameraType.Game;

		internal int viewCount => Math.Max(1, xr.viewCount);

		internal bool clearDepth
		{
			get
			{
				if (!(m_AdditionalCameraData != null))
				{
					return camera.clearFlags != CameraClearFlags.Nothing;
				}
				return m_AdditionalCameraData.clearDepth;
			}
		}

		internal HDAdditionalCameraData.ClearColorMode clearColorMode
		{
			get
			{
				if (CameraIsSceneFiltering())
				{
					return HDAdditionalCameraData.ClearColorMode.Color;
				}
				if (m_AdditionalCameraData != null)
				{
					return m_AdditionalCameraData.clearColorMode;
				}
				if (camera.clearFlags == CameraClearFlags.Skybox)
				{
					return HDAdditionalCameraData.ClearColorMode.Sky;
				}
				if (camera.clearFlags == CameraClearFlags.Color)
				{
					return HDAdditionalCameraData.ClearColorMode.Color;
				}
				return HDAdditionalCameraData.ClearColorMode.None;
			}
		}

		internal Color backgroundColorHDR
		{
			get
			{
				if (m_AdditionalCameraData != null)
				{
					return m_AdditionalCameraData.backgroundColorHDR;
				}
				return camera.backgroundColor.linear;
			}
		}

		internal HDAdditionalCameraData.FlipYMode flipYMode
		{
			get
			{
				if (m_AdditionalCameraData != null)
				{
					return m_AdditionalCameraData.flipYMode;
				}
				return HDAdditionalCameraData.FlipYMode.Automatic;
			}
		}

		internal GameObject exposureTarget
		{
			get
			{
				if (m_AdditionalCameraData != null)
				{
					return m_AdditionalCameraData.exposureTarget;
				}
				return null;
			}
		}

		internal bool exposureControlFS => m_ExposureControlFS;

		internal ExposureTextures currentExposureTextures => m_ExposureTextures;

		internal HDAdditionalCameraData.AntialiasingMode antialiasing { get; private set; }

		internal HDAdditionalCameraData.SMAAQualityLevel SMAAQuality { get; private set; } = HDAdditionalCameraData.SMAAQualityLevel.Medium;


		internal HDAdditionalCameraData.TAAQualityLevel TAAQuality { get; private set; } = HDAdditionalCameraData.TAAQualityLevel.Medium;


		internal bool dithering
		{
			get
			{
				if (m_AdditionalCameraData != null)
				{
					return m_AdditionalCameraData.dithering;
				}
				return false;
			}
		}

		internal bool stopNaNs
		{
			get
			{
				if (m_AdditionalCameraData != null)
				{
					return m_AdditionalCameraData.stopNaNs;
				}
				return false;
			}
		}

		internal bool allowDynamicResolution
		{
			get
			{
				if (m_AdditionalCameraData != null)
				{
					return m_AdditionalCameraData.allowDynamicResolution;
				}
				return false;
			}
		}

		internal IEnumerable<AOVRequestData> aovRequests
		{
			get
			{
				if (!(m_AdditionalCameraData != null) || m_AdditionalCameraData.Equals(null))
				{
					return Enumerable.Empty<AOVRequestData>();
				}
				return m_AdditionalCameraData.aovRequests;
			}
		}

		internal LayerMask probeLayerMask
		{
			get
			{
				if (!(m_AdditionalCameraData != null))
				{
					return -1;
				}
				return m_AdditionalCameraData.probeLayerMask;
			}
		}

		internal float probeRangeCompressionFactor
		{
			get
			{
				if (!(m_AdditionalCameraData != null))
				{
					return 1f;
				}
				return m_AdditionalCameraData.probeCustomFixedExposure;
			}
		}

		internal DynamicResolutionRequest DynResRequest { get; set; }

		internal ProfilingSampler profilingSampler => m_AdditionalCameraData?.profilingSampler ?? ProfilingSampler.Get(HDProfileId.HDRenderPipelineRenderCamera);

		internal bool allowDeepLearningSuperSampling
		{
			get
			{
				if (!(m_AdditionalCameraData == null))
				{
					return m_AdditionalCameraData.allowDeepLearningSuperSampling;
				}
				return false;
			}
		}

		internal bool deepLearningSuperSamplingUseCustomQualitySettings
		{
			get
			{
				if (!(m_AdditionalCameraData == null))
				{
					return m_AdditionalCameraData.deepLearningSuperSamplingUseCustomQualitySettings;
				}
				return false;
			}
		}

		internal uint deepLearningSuperSamplingQuality
		{
			get
			{
				if (!(m_AdditionalCameraData == null))
				{
					return m_AdditionalCameraData.deepLearningSuperSamplingQuality;
				}
				return 0u;
			}
		}

		internal bool deepLearningSuperSamplingUseCustomAttributes
		{
			get
			{
				if (!(m_AdditionalCameraData == null))
				{
					return m_AdditionalCameraData.deepLearningSuperSamplingUseCustomAttributes;
				}
				return false;
			}
		}

		internal bool deepLearningSuperSamplingUseOptimalSettings
		{
			get
			{
				if (!(m_AdditionalCameraData == null))
				{
					return m_AdditionalCameraData.deepLearningSuperSamplingUseOptimalSettings;
				}
				return false;
			}
		}

		internal float deepLearningSuperSamplingSharpening
		{
			get
			{
				if (!(m_AdditionalCameraData == null))
				{
					return m_AdditionalCameraData.deepLearningSuperSamplingSharpening;
				}
				return 0f;
			}
		}

		internal bool fsrOverrideSharpness
		{
			get
			{
				if (!(m_AdditionalCameraData == null))
				{
					return m_AdditionalCameraData.fsrOverrideSharpness;
				}
				return false;
			}
		}

		internal float fsrSharpness
		{
			get
			{
				if (!(m_AdditionalCameraData == null))
				{
					return m_AdditionalCameraData.fsrSharpness;
				}
				return 0.92f;
			}
		}

		internal bool hasCaptureActions => m_RecorderCaptureActions != null;

		public static HDCamera GetOrCreate(Camera camera, int xrMultipassId = 0)
		{
			if (!s_Cameras.TryGetValue((camera, xrMultipassId), out var value))
			{
				value = new HDCamera(camera);
				s_Cameras.Add((camera, xrMultipassId), value);
			}
			return value;
		}

		public void Reset()
		{
			isFirstFrame = true;
			cameraFrameCount = 0u;
			resetPostProcessingHistory = true;
			volumetricHistoryIsValid = false;
			volumetricValidFrames = 0;
			colorPyramidHistoryIsValid = false;
			colorPyramidHistoryValidFrames = 0;
			dofHistoryIsValid = false;
			if (visualSky != null)
			{
				visualSky.Reset();
			}
			if (lightingSky != null && visualSky != lightingSky)
			{
				lightingSky.Reset();
			}
		}

		public RTHandle AllocHistoryFrameRT(int id, Func<string, int, RTHandleSystem, RTHandle> allocator, int bufferCount)
		{
			m_HistoryRTSystem.AllocBuffer(id, (RTHandleSystem rts, int i) => allocator(camera.name, i, rts), bufferCount);
			return m_HistoryRTSystem.GetFrameRT(id, 0);
		}

		public RTHandle GetPreviousFrameRT(int id)
		{
			return m_HistoryRTSystem.GetFrameRT(id, 1);
		}

		public RTHandle GetCurrentFrameRT(int id)
		{
			return m_HistoryRTSystem.GetFrameRT(id, 0);
		}

		internal void SetParentCamera(HDCamera parentHdCam, bool useGpuFetchedExposure, float fetchedGpuExposure)
		{
			if (parentHdCam == null)
			{
				m_ExposureTextures.clear();
				m_ExposureTextures.useCurrentCamera = true;
				m_parentCamera = null;
				return;
			}
			m_parentCamera = parentHdCam.camera;
			if (!m_ExposureControlFS)
			{
				m_ExposureTextures.clear();
				m_ExposureTextures.useCurrentCamera = true;
				return;
			}
			m_ExposureTextures.clear();
			m_ExposureTextures.useCurrentCamera = false;
			m_ExposureTextures.parent = parentHdCam.currentExposureTextures.current;
			if (useGpuFetchedExposure)
			{
				m_ExposureTextures.useFetchedExposure = true;
				m_ExposureTextures.fetchedGpuExposure = fetchedGpuExposure;
			}
		}

		internal bool CameraIsSceneFiltering()
		{
			if (CoreUtils.IsSceneFilteringEnabled())
			{
				return camera.cameraType == CameraType.SceneView;
			}
			return false;
		}

		internal void RequestGpuExposureValue(RTHandle exposureTexture)
		{
			RequestGpuTexelValue(exposureTexture, isDeExposure: false);
		}

		internal void RequestGpuDeExposureValue(RTHandle exposureTexture)
		{
			RequestGpuTexelValue(exposureTexture, isDeExposure: true);
		}

		private void RequestGpuTexelValue(RTHandle exposureTexture, bool isDeExposure)
		{
			ExposureGpuReadbackRequest item = default(ExposureGpuReadbackRequest);
			item.request = AsyncGPUReadback.Request(exposureTexture.rt, 0, 0, 1, 0, 1, 0, 1);
			item.isDeExposure = isDeExposure;
			m_ExposureAsyncRequest.Enqueue(item);
		}

		private void PumpReadbackQueue()
		{
			while (m_ExposureAsyncRequest.Count != 0)
			{
				ExposureGpuReadbackRequest exposureGpuReadbackRequest = m_ExposureAsyncRequest.Peek();
				ref AsyncGPUReadbackRequest request = ref exposureGpuReadbackRequest.request;
				if (!request.done && !request.hasError)
				{
					break;
				}
				if (!request.hasError)
				{
					NativeArray<float> data = request.GetData<float>();
					if (exposureGpuReadbackRequest.isDeExposure)
					{
						m_GpuDeExposureValue = data[0];
					}
					else
					{
						m_GpuExposureValue = data[0];
					}
				}
				m_ExposureAsyncRequest.Dequeue();
			}
		}

		internal float GpuExposureValue()
		{
			PumpReadbackQueue();
			return m_GpuExposureValue;
		}

		internal float GpuDeExposureValue()
		{
			PumpReadbackQueue();
			return m_GpuDeExposureValue;
		}

		internal void SetupExposureTextures()
		{
			if (!m_ExposureControlFS)
			{
				m_ExposureTextures.current = null;
				m_ExposureTextures.previous = null;
				return;
			}
			RTHandle rTHandle = GetCurrentFrameRT(1);
			if (rTHandle == null)
			{
				rTHandle = AllocHistoryFrameRT(1, Allocator, 2);
			}
			m_ExposureTextures.current = GetPreviousFrameRT(1);
			m_ExposureTextures.previous = rTHandle;
			static RTHandle Allocator(string id, int frameIndex, RTHandleSystem rtHandleSystem)
			{
				RTHandle rTHandle2 = rtHandleSystem.Alloc(1, 1, 1, DepthBits.None, GraphicsFormat.R32G32_SFloat, FilterMode.Point, TextureWrapMode.Repeat, TextureDimension.Tex2D, enableRandomWrite: true, useMipMap: false, autoGenerateMips: true, isShadowMap: false, 1, 0f, MSAASamples.None, bindTextureMS: false, useDynamicScale: false, RenderTextureMemoryless.None, VRTextureUsage.None, $"{id} Exposure Texture {frameIndex}");
				HDRenderPipeline.SetExposureTextureToEmpty(rTHandle2);
				return rTHandle2;
			}
		}

		internal bool ValidShadowHistory(HDAdditionalLightData lightData, int screenSpaceShadowIndex, GPULightType lightType)
		{
			if (shadowHistoryUsage[screenSpaceShadowIndex].lightInstanceID == lightData.GetInstanceID() && shadowHistoryUsage[screenSpaceShadowIndex].frameCount == cameraFrameCount - 1)
			{
				return shadowHistoryUsage[screenSpaceShadowIndex].lightType == lightType;
			}
			return false;
		}

		internal void PropagateShadowHistory(HDAdditionalLightData lightData, int screenSpaceShadowIndex, GPULightType lightType)
		{
			shadowHistoryUsage[screenSpaceShadowIndex].lightInstanceID = lightData.GetInstanceID();
			shadowHistoryUsage[screenSpaceShadowIndex].frameCount = cameraFrameCount;
			shadowHistoryUsage[screenSpaceShadowIndex].lightType = lightType;
			shadowHistoryUsage[screenSpaceShadowIndex].transform = lightData.transform.localToWorldMatrix;
		}

		internal bool EffectHistoryValidity(HistoryEffectSlot slot, int flagMask)
		{
			flagMask |= (exposureControlFS ? 4 : 0);
			if (historyEffectUsage[(int)slot].frameCount == cameraFrameCount - 1)
			{
				return historyEffectUsage[(int)slot].flagMask == flagMask;
			}
			return false;
		}

		internal void PropagateEffectHistoryValidity(HistoryEffectSlot slot, int flagMask)
		{
			flagMask |= (exposureControlFS ? 4 : 0);
			historyEffectUsage[(int)slot].frameCount = (int)cameraFrameCount;
			historyEffectUsage[(int)slot].flagMask = flagMask;
		}

		internal uint GetCameraFrameCount()
		{
			return cameraFrameCount;
		}

		internal void RequestDynamicResolution(bool cameraRequestedDynamicRes, DynamicResolutionHandler dynResHandler)
		{
			DynResRequest = new DynamicResolutionRequest
			{
				enabled = dynResHandler.DynamicResolutionEnabled(),
				cameraRequested = cameraRequestedDynamicRes,
				hardwareEnabled = dynResHandler.HardwareDynamicResIsEnabled(),
				filter = dynResHandler.filter
			};
		}

		internal HDCamera(Camera cam)
		{
			camera = cam;
			name = cam.name;
			frustum = default(Frustum);
			frustum.planes = new Plane[6];
			frustum.corners = new Vector3[8];
			frustumPlaneEquations = new Vector4[6];
			volumeStack = VolumeManager.instance.CreateStack();
			m_DepthBufferMipChainInfo.Allocate();
			Reset();
		}

		internal bool IsDLSSEnabled()
		{
			if (!(m_AdditionalCameraData == null))
			{
				return m_AdditionalCameraData.cameraCanRenderDLSS;
			}
			return false;
		}

		internal bool IsTAAUEnabled()
		{
			if (DynamicResolutionHandler.instance.DynamicResolutionEnabled() && DynamicResolutionHandler.instance.filter == DynamicResUpscaleFilter.TAAU)
			{
				return !IsDLSSEnabled();
			}
			return false;
		}

		internal bool IsPathTracingEnabled()
		{
			PathTracing component = volumeStack.GetComponent<PathTracing>();
			if (!component)
			{
				return false;
			}
			return component.enable.value;
		}

		internal DynamicResolutionHandler.UpsamplerScheduleType UpsampleSyncPoint()
		{
			if (IsDLSSEnabled())
			{
				return HDRenderPipeline.currentAsset.currentPlatformRenderPipelineSettings.dynamicResolutionSettings.DLSSInjectionPoint;
			}
			if (IsTAAUEnabled())
			{
				return DynamicResolutionHandler.UpsamplerScheduleType.BeforePost;
			}
			return DynamicResolutionHandler.UpsamplerScheduleType.AfterPost;
		}

		internal bool RequiresCameraJitter()
		{
			if (antialiasing == HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing || IsDLSSEnabled() || IsTAAUEnabled())
			{
				return !IsPathTracingEnabled();
			}
			return false;
		}

		internal bool IsSSREnabled(bool transparent = false)
		{
			ScreenSpaceReflection component = volumeStack.GetComponent<ScreenSpaceReflection>();
			if (!transparent)
			{
				if (frameSettings.IsEnabled(FrameSettingsField.SSR) && component.enabled.value)
				{
					return frameSettings.IsEnabled(FrameSettingsField.OpaqueObjects);
				}
				return false;
			}
			if (frameSettings.IsEnabled(FrameSettingsField.TransparentSSR))
			{
				return component.enabledTransparent.value;
			}
			return false;
		}

		internal bool IsSSGIEnabled()
		{
			GlobalIllumination component = volumeStack.GetComponent<GlobalIllumination>();
			if (frameSettings.IsEnabled(FrameSettingsField.SSGI))
			{
				return component.enable.value;
			}
			return false;
		}

		internal bool IsVolumetricReprojectionEnabled()
		{
			bool num = Fog.IsVolumetricFogEnabled(this);
			bool flag = camera.cameraType == CameraType.Game || (camera.cameraType == CameraType.SceneView && CoreUtils.AreAnimatedMaterialsEnabled(camera));
			bool flag2 = frameSettings.IsEnabled(FrameSettingsField.ReprojectionForVolumetrics);
			return num && flag && flag2;
		}

		internal void Update(FrameSettings currentFrameSettings, HDRenderPipeline hdrp, XRPass xrPass, bool allocateHistoryBuffers = true)
		{
			Camera camera = ((parentCamera != null) ? parentCamera : this.camera);
			animateMaterials = CoreUtils.AreAnimatedMaterialsEnabled(camera);
			if (animateMaterials)
			{
				float num = Time.time;
				float num2 = Time.deltaTime;
				time = num;
				lastTime = num - num2;
			}
			else
			{
				time = 0f;
				lastTime = 0f;
			}
			if (shadowHistoryUsage == null || shadowHistoryUsage.Length != hdrp.currentPlatformRenderPipelineSettings.hdShadowInitParams.maxScreenSpaceShadowSlots)
			{
				shadowHistoryUsage = new ShadowHistoryUsage[hdrp.currentPlatformRenderPipelineSettings.hdShadowInitParams.maxScreenSpaceShadowSlots];
			}
			if (historyEffectUsage == null || historyEffectUsage.Length != 5)
			{
				historyEffectUsage = new HistoryEffectValidity[5];
				for (int i = 0; i < 5; i++)
				{
					historyEffectUsage[i].frameCount = -1;
				}
			}
			this.camera.TryGetComponent<HDAdditionalCameraData>(out m_AdditionalCameraData);
			globalMipBias = ((m_AdditionalCameraData == null) ? 0f : m_AdditionalCameraData.materialMipBias);
			UpdateVolumeAndPhysicalParameters();
			xr = xrPass;
			frameSettings = currentFrameSettings;
			m_ExposureControlFS = frameSettings.IsEnabled(FrameSettingsField.ExposureControl);
			UpdateAntialiasing();
			DynamicResolutionHandler.instance.upsamplerSchedule = UpsampleSyncPoint();
			if (allocateHistoryBuffers)
			{
				HDRenderPipeline.ReinitializeVolumetricBufferParams(this);
				bool flag = frameSettings.IsEnabled(FrameSettingsField.Refraction) || frameSettings.IsEnabled(FrameSettingsField.Distortion) || frameSettings.IsEnabled(FrameSettingsField.Water);
				bool num3 = IsSSREnabled() || IsSSREnabled(transparent: true) || IsSSGIEnabled();
				bool flag2 = IsVolumetricReprojectionEnabled();
				HDRenderPipeline hDRenderPipeline = (HDRenderPipeline)RenderPipelineManager.currentPipeline;
				bool flag3 = false;
				int num4 = 0;
				int numFramesAllocated = m_HistoryRTSystem.GetNumFramesAllocated(num4);
				if (numFramesAllocated > 0)
				{
					RTHandle currentFrameRT = GetCurrentFrameRT(num4);
					if (currentFrameRT != null && currentFrameRT.rt.graphicsFormat != hDRenderPipeline.GetColorBufferFormat())
					{
						flag3 = true;
					}
				}
				int num5 = 0;
				if (flag)
				{
					num5 = 1;
				}
				if (num3)
				{
					num5 = 2;
				}
				foreach (AOVRequestData aovRequest in aovRequests)
				{
					if (GetHistoryRTHandleSystem(aovRequest).GetNumFramesAllocated(num4) != num5)
					{
						flag3 = true;
						break;
					}
				}
				if (m_PrevUpsamplerSchedule != DynamicResolutionHandler.instance.upsamplerSchedule || previousFrameWasTAAUpsampled != IsTAAUEnabled())
				{
					flag3 = true;
					m_PrevUpsamplerSchedule = DynamicResolutionHandler.instance.upsamplerSchedule;
				}
				if (numFramesAllocated != num5 || flag3)
				{
					colorPyramidHistoryIsValid = false;
					resetPostProcessingHistory = true;
					if (flag3)
					{
						m_HistoryRTSystem.Dispose();
						m_HistoryRTSystem = new BufferedRTHandleSystem();
					}
					else
					{
						m_HistoryRTSystem.ReleaseBuffer(0);
					}
					m_ExposureTextures.clear();
					if (num5 != 0 || flag3)
					{
						bool flag4 = num5 > 0;
						if (flag4)
						{
							AllocHistoryFrameRT(0, HistoryBufferAllocatorFunction, num5);
						}
						BufferedRTHandleSystem historyRTHandleSystem = GetHistoryRTHandleSystem();
						foreach (AOVRequestData aovRequest2 in aovRequests)
						{
							BufferedRTHandleSystem historyRTHandleSystem2 = GetHistoryRTHandleSystem(aovRequest2);
							BindHistoryRTHandleSystem(historyRTHandleSystem2);
							if (flag4)
							{
								AllocHistoryFrameRT(0, HistoryBufferAllocatorFunction, num5);
							}
						}
						BindHistoryRTHandleSystem(historyRTHandleSystem);
					}
				}
				int num6 = (flag2 ? 2 : 0);
				if (m_NumVolumetricBuffersAllocated != num6)
				{
					HDRenderPipeline.DestroyVolumetricHistoryBuffers(this);
					if (num6 != 0)
					{
						HDRenderPipeline.CreateVolumetricHistoryBuffers(this, num6);
					}
					m_NumVolumetricBuffersAllocated = num6;
				}
			}
			prevFinalViewport = finalViewport;
			if (xr.enabled)
			{
				finalViewport = xr.GetViewport();
			}
			else
			{
				finalViewport = GetPixelRect();
			}
			actualWidth = Math.Max((int)finalViewport.size.x, 1);
			actualHeight = Math.Max((int)finalViewport.size.y, 1);
			DynamicResolutionHandler.instance.finalViewport = new Vector2Int((int)finalViewport.width, (int)finalViewport.height);
			Vector2Int vector2Int = new Vector2Int(actualWidth, actualHeight);
			m_DepthBufferMipChainInfo.ComputePackedMipChainInfo(vector2Int);
			lowResScale = 0.5f;
			if (canDoDynamicResolution)
			{
				Vector2Int scaledSize = DynamicResolutionHandler.instance.GetScaledSize(new Vector2Int(actualWidth, actualHeight));
				actualWidth = scaledSize.x;
				actualHeight = scaledSize.y;
				globalMipBias += DynamicResolutionHandler.instance.CalculateMipBias(scaledSize, vector2Int, UpsampleSyncPoint() <= DynamicResolutionHandler.UpsamplerScheduleType.AfterDepthOfField);
				lowResScale = DynamicResolutionHandler.instance.GetLowResMultiplier(lowResScale);
			}
			int num7 = actualWidth;
			int num8 = actualHeight;
			msaaSamples = frameSettings.GetResolvedMSAAMode(hdrp.asset);
			screenSize = new Vector4(num7, num8, 1f / (float)num7, 1f / (float)num8);
			SetPostProcessScreenSize(num7, num8);
			screenParams = new Vector4(screenSize.x, screenSize.y, 1f + screenSize.z, 1f + screenSize.w);
			if (++taaFrameIndex >= 8)
			{
				taaFrameIndex = 0;
			}
			UpdateAllViewConstants();
			isFirstFrame = false;
			cameraFrameCount++;
			HDRenderPipeline.UpdateVolumetricBufferParams(this);
			HDRenderPipeline.ResizeVolumetricHistoryBuffers(this);
		}

		internal void SetReferenceSize()
		{
			RTHandles.SetReferenceSize(actualWidth, actualHeight);
			m_HistoryRTSystem.SwapAndSetReferenceSize(actualWidth, actualHeight);
			SetPostProcessScreenSize(actualWidth, actualHeight);
			foreach (KeyValuePair<AOVRequestData, BufferedRTHandleSystem> item in m_AOVHistoryRTSystem)
			{
				item.Value.SwapAndSetReferenceSize(actualWidth, actualHeight);
			}
		}

		internal void SetPostProcessScreenSize(int width, int height)
		{
			m_PostProcessScreenSize = new Vector4(width, height, 1f / (float)width, 1f / (float)height);
			Vector2 vector = RTHandles.CalculateRatioAgainstMaxSize(width, height);
			m_PostProcessRTScales = new Vector4(vector.x, vector.y, m_PostProcessRTScales.x, m_PostProcessRTScales.y);
		}

		internal void SetPostProcessHistorySizeAndReference(int width, int height, int referenceWidth, int referenceHeight)
		{
			m_PostProcessRTHistoryMaxReference = new Vector2Int(Math.Max(referenceWidth, m_PostProcessRTHistoryMaxReference.x), Math.Max(referenceHeight, m_PostProcessRTHistoryMaxReference.y));
			m_PostProcessRTScalesHistory = new Vector4((float)width / (float)m_PostProcessRTHistoryMaxReference.x, (float)height / (float)m_PostProcessRTHistoryMaxReference.y, m_PostProcessRTScalesHistory.x, m_PostProcessRTScalesHistory.y);
		}

		internal void BeginRender(CommandBuffer cmd)
		{
			SetReferenceSize();
			m_RecorderCaptureActions = CameraCaptureBridge.GetCaptureActions(camera);
			SetupCurrentMaterialQuality(cmd);
			SetupExposureTextures();
		}

		internal void UpdateAllViewConstants(bool jitterProjectionMatrix)
		{
			UpdateAllViewConstants(jitterProjectionMatrix, updatePreviousFrameConstants: false);
		}

		internal void GetPixelCoordToViewDirWS(Vector4 resolution, float aspect, ref Matrix4x4[] transforms)
		{
			if (xr.singlePassEnabled)
			{
				for (int i = 0; i < viewCount; i++)
				{
					transforms[i] = ComputePixelCoordToWorldSpaceViewDirectionMatrix(m_XRViewConstants[i], resolution, aspect);
				}
			}
			else
			{
				transforms[0] = ComputePixelCoordToWorldSpaceViewDirectionMatrix(mainViewConstants, resolution, aspect);
			}
		}

		internal static void ClearAll()
		{
			foreach (KeyValuePair<(Camera, int), HDCamera> s_Camera in s_Cameras)
			{
				s_Camera.Value.ReleaseHistoryBuffer();
				s_Camera.Value.Dispose();
			}
			s_Cameras.Clear();
			s_Cleanup.Clear();
		}

		internal static void CleanUnused()
		{
			foreach (var key in s_Cameras.Keys)
			{
				HDCamera hDCamera = s_Cameras[key];
				if (!(hDCamera.camera != null) || hDCamera.camera.cameraType != CameraType.SceneView)
				{
					bool flag = hDCamera.m_AdditionalCameraData != null && hDCamera.m_AdditionalCameraData.hasPersistentHistory;
					if (hDCamera.camera == null || (!hDCamera.camera.isActiveAndEnabled && hDCamera.camera.cameraType != CameraType.Preview && !flag && !hDCamera.isPersistent))
					{
						s_Cleanup.Add(key);
					}
				}
			}
			foreach (var item in s_Cleanup)
			{
				s_Cameras[item].Dispose();
				s_Cameras.Remove(item);
			}
			s_Cleanup.Clear();
		}

		internal static void ResetAllHistoryRTHandleSystems(int width, int height)
		{
			foreach (KeyValuePair<(Camera, int), HDCamera> s_Camera in s_Cameras)
			{
				HDCamera value = s_Camera.Value;
				Vector2Int currentRenderTargetSize = value.m_HistoryRTSystem.rtHandleProperties.currentRenderTargetSize;
				if (width >= currentRenderTargetSize.x && height >= currentRenderTargetSize.y)
				{
					continue;
				}
				value.m_HistoryRTSystem.ResetReferenceSize(width, height);
				foreach (KeyValuePair<AOVRequestData, BufferedRTHandleSystem> item in value.m_AOVHistoryRTSystem)
				{
					item.Value.ResetReferenceSize(width, height);
				}
			}
		}

		internal void UpdateScalesAndScreenSizesCB(ref ShaderVariablesGlobal cb)
		{
			cb._ScreenSize = screenSize;
			cb._PostProcessScreenSize = postProcessScreenSize;
			cb._RTHandleScale = RTHandles.rtHandleProperties.rtHandleScale;
			cb._RTHandleScaleHistory = m_HistoryRTSystem.rtHandleProperties.rtHandleScale;
			cb._RTHandlePostProcessScale = m_PostProcessRTScales;
			cb._RTHandlePostProcessScaleHistory = m_PostProcessRTScalesHistory;
			cb._DynamicResolutionFullscreenScale = new Vector4((float)actualWidth / finalViewport.width, (float)actualHeight / finalViewport.height, 0f, 0f);
		}

		internal void UpdateShaderVariablesGlobalCB(ref ShaderVariablesGlobal cb)
		{
			UpdateShaderVariablesGlobalCB(ref cb, (int)cameraFrameCount);
		}

		internal unsafe void UpdateShaderVariablesGlobalCB(ref ShaderVariablesGlobal cb, int frameCount)
		{
			bool flag = frameSettings.IsEnabled(FrameSettingsField.Postprocess) && antialiasing == HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing && camera.cameraType == CameraType.Game;
			bool flag2 = m_AdditionalCameraData == null;
			cb._ViewMatrix = mainViewConstants.viewMatrix;
			cb._CameraViewMatrix = mainViewConstants.viewMatrix;
			cb._InvViewMatrix = mainViewConstants.invViewMatrix;
			cb._ProjMatrix = mainViewConstants.projMatrix;
			cb._InvProjMatrix = mainViewConstants.invProjMatrix;
			cb._ViewProjMatrix = mainViewConstants.viewProjMatrix;
			cb._CameraViewProjMatrix = mainViewConstants.viewProjMatrix;
			cb._InvViewProjMatrix = mainViewConstants.invViewProjMatrix;
			cb._NonJitteredViewProjMatrix = mainViewConstants.nonJitteredViewProjMatrix;
			cb._PrevViewProjMatrix = mainViewConstants.prevViewProjMatrix;
			cb._PrevInvViewProjMatrix = mainViewConstants.prevInvViewProjMatrix;
			cb._WorldSpaceCameraPos_Internal = mainViewConstants.worldSpaceCameraPos;
			cb._PrevCamPosRWS_Internal = mainViewConstants.prevWorldSpaceCameraPos;
			UpdateScalesAndScreenSizesCB(ref cb);
			cb._ZBufferParams = zBufferParams;
			cb._ProjectionParams = projectionParams;
			cb.unity_OrthoParams = unity_OrthoParams;
			cb._ScreenParams = screenParams;
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					cb._FrustumPlanes[i * 4 + j] = frustumPlaneEquations[i][j];
				}
			}
			cb._TaaFrameInfo = new Vector4(taaSharpenStrength, 0f, taaFrameIndex, flag ? 1 : 0);
			cb._TaaJitterStrength = taaJitter;
			cb._ColorPyramidLodCount = colorPyramidHistoryMipCount;
			cb._GlobalMipBias = globalMipBias;
			cb._GlobalMipBiasPow2 = (float)Math.Pow(2.0, globalMipBias);
			float num = time;
			float num2 = lastTime;
			float num3 = Time.deltaTime;
			float smoothDeltaTime = Time.smoothDeltaTime;
			cb._Time = new Vector4(num * 0.05f, num, num * 2f, num * 3f);
			cb._SinTime = new Vector4(Mathf.Sin(num * 0.125f), Mathf.Sin(num * 0.25f), Mathf.Sin(num * 0.5f), Mathf.Sin(num));
			cb._CosTime = new Vector4(Mathf.Cos(num * 0.125f), Mathf.Cos(num * 0.25f), Mathf.Cos(num * 0.5f), Mathf.Cos(num));
			cb.unity_DeltaTime = new Vector4(num3, 1f / num3, smoothDeltaTime, 1f / smoothDeltaTime);
			cb._TimeParameters = new Vector4(num, Mathf.Sin(num), Mathf.Cos(num), 0f);
			cb._LastTimeParameters = new Vector4(num2, Mathf.Sin(num2), Mathf.Cos(num2), 0f);
			cb._FrameCount = frameCount;
			cb._XRViewCount = (uint)viewCount;
			float probeExposureScale = 1f / Mathf.Max(probeRangeCompressionFactor, 1E-06f);
			cb._ProbeExposureScale = probeExposureScale;
			cb._DeExposureMultiplier = (flag2 ? 1f : m_AdditionalCameraData.deExposureMultiplier);
			cb._TransparentCameraOnlyMotionVectors = ((frameSettings.IsEnabled(FrameSettingsField.MotionVectors) && !frameSettings.IsEnabled(FrameSettingsField.TransparentsWriteMotionVector)) ? 1 : 0);
			cb._ScreenSizeOverride = (flag2 ? cb._ScreenSize : m_AdditionalCameraData.screenSizeOverride);
			cb._ScreenCoordScaleBias = (flag2 ? new Vector4(1f, 1f, 0f, 0f) : m_AdditionalCameraData.screenCoordScaleBias);
		}

		internal void PushBuiltinShaderConstantsXR(CommandBuffer cmd)
		{
			if (!xr.enabled)
			{
				return;
			}
			cmd.SetViewProjectionMatrices(xr.GetViewMatrix(), xr.GetProjMatrix());
			if (xr.singlePassEnabled)
			{
				for (int i = 0; i < viewCount; i++)
				{
					XRBuiltinShaderConstants.UpdateBuiltinShaderConstants(xr.GetViewMatrix(i), xr.GetProjMatrix(i), renderIntoTexture: true, i);
				}
				XRBuiltinShaderConstants.SetBuiltinShaderConstants(cmd);
			}
		}

		internal unsafe void UpdateShaderVariablesXRCB(ref ShaderVariablesXR cb)
		{
			for (int i = 0; i < viewCount; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					cb._XRViewMatrix[i * 16 + j] = m_XRViewConstants[i].viewMatrix[j];
					cb._XRInvViewMatrix[i * 16 + j] = m_XRViewConstants[i].invViewMatrix[j];
					cb._XRProjMatrix[i * 16 + j] = m_XRViewConstants[i].projMatrix[j];
					cb._XRInvProjMatrix[i * 16 + j] = m_XRViewConstants[i].invProjMatrix[j];
					cb._XRViewProjMatrix[i * 16 + j] = m_XRViewConstants[i].viewProjMatrix[j];
					cb._XRInvViewProjMatrix[i * 16 + j] = m_XRViewConstants[i].invViewProjMatrix[j];
					cb._XRNonJitteredViewProjMatrix[i * 16 + j] = m_XRViewConstants[i].nonJitteredViewProjMatrix[j];
					cb._XRPrevViewProjMatrix[i * 16 + j] = m_XRViewConstants[i].prevViewProjMatrix[j];
					cb._XRPrevInvViewProjMatrix[i * 16 + j] = m_XRViewConstants[i].prevInvViewProjMatrix[j];
					cb._XRViewProjMatrixNoCameraTrans[i * 16 + j] = m_XRViewConstants[i].viewProjectionNoCameraTrans[j];
					cb._XRPrevViewProjMatrixNoCameraTrans[i * 16 + j] = m_XRViewConstants[i].prevViewProjMatrixNoCameraTrans[j];
					cb._XRPixelCoordToViewDirWS[i * 16 + j] = m_XRViewConstants[i].pixelCoordToViewDirWS[j];
				}
				for (int k = 0; k < 3; k++)
				{
					cb._XRWorldSpaceCameraPos[i * 4 + k] = m_XRViewConstants[i].worldSpaceCameraPos[k];
					cb._XRWorldSpaceCameraPosViewOffset[i * 4 + k] = m_XRViewConstants[i].worldSpaceCameraPosViewOffset[k];
					cb._XRPrevWorldSpaceCameraPos[i * 4 + k] = m_XRViewConstants[i].prevWorldSpaceCameraPos[k];
				}
			}
		}

		internal bool AllocateAmbientOcclusionHistoryBuffer(float scaleFactor)
		{
			if (scaleFactor != m_AmbientOcclusionResolutionScale || GetCurrentFrameRT(8) == null)
			{
				ReleaseHistoryFrameRT(8);
				CustomHistoryAllocator customHistoryAllocator = new CustomHistoryAllocator(new Vector2(scaleFactor, scaleFactor), GraphicsFormat.R8G8B8A8_UNorm, "AO Packed history");
				AllocHistoryFrameRT(8, ((CustomHistoryAllocator)customHistoryAllocator).Allocator, 2);
				m_AmbientOcclusionResolutionScale = scaleFactor;
				return true;
			}
			return false;
		}

		internal void AllocateScreenSpaceAccumulationHistoryBuffer(float scaleFactor)
		{
			if (scaleFactor != m_ScreenSpaceAccumulationResolutionScale || GetCurrentFrameRT(21) == null)
			{
				ReleaseHistoryFrameRT(21);
				CustomHistoryAllocator customHistoryAllocator = new CustomHistoryAllocator(new Vector2(scaleFactor, scaleFactor), GraphicsFormat.R16G16B16A16_SFloat, "SSR_Accum Packed history");
				AllocHistoryFrameRT(21, ((CustomHistoryAllocator)customHistoryAllocator).Allocator, 2);
				m_ScreenSpaceAccumulationResolutionScale = scaleFactor;
			}
		}

		internal void ReleaseHistoryFrameRT(int id)
		{
			m_HistoryRTSystem.ReleaseBuffer(id);
		}

		internal void ExecuteCaptureActions(RenderGraph renderGraph, TextureHandle input)
		{
			if (m_RecorderCaptureActions == null || !m_RecorderCaptureActions.MoveNext())
			{
				return;
			}
			ExecuteCaptureActionsPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<ExecuteCaptureActionsPassData>("Execute Capture Actions", out passData);
			try
			{
				TextureDesc textureDesc = renderGraph.GetTextureDesc(input);
				Vector4 rtHandleScale = RTHandles.rtHandleProperties.rtHandleScale;
				passData.viewportScale = new Vector2(rtHandleScale.x, rtHandleScale.y);
				passData.blitMaterial = HDUtils.GetBlitMaterial(textureDesc.dimension);
				passData.recorderCaptureActions = m_RecorderCaptureActions;
				passData.input = renderGraphBuilder.ReadTexture(in input);
				ExecuteCaptureActionsPassData executeCaptureActionsPassData = passData;
				TextureDesc desc = new TextureDesc(actualWidth, actualHeight)
				{
					colorFormat = textureDesc.colorFormat,
					name = "TempCaptureActions"
				};
				executeCaptureActionsPassData.tempTexture = renderGraphBuilder.CreateTransientTexture(in desc);
				renderGraphBuilder.SetRenderFunc(delegate(ExecuteCaptureActionsPassData data, RenderGraphContext ctx)
				{
					MaterialPropertyBlock tempMaterialPropertyBlock = ctx.renderGraphPool.GetTempMaterialPropertyBlock();
					tempMaterialPropertyBlock.SetTexture(HDShaderIDs._BlitTexture, data.input);
					tempMaterialPropertyBlock.SetVector(HDShaderIDs._BlitScaleBias, data.viewportScale);
					tempMaterialPropertyBlock.SetFloat(HDShaderIDs._BlitMipLevel, 0f);
					ctx.cmd.SetRenderTarget(data.tempTexture);
					ctx.cmd.DrawProcedural(Matrix4x4.identity, data.blitMaterial, 0, MeshTopology.Triangles, 3, 1, tempMaterialPropertyBlock);
					data.recorderCaptureActions.Reset();
					while (data.recorderCaptureActions.MoveNext())
					{
						data.recorderCaptureActions.Current(data.tempTexture, ctx.cmd);
					}
				});
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}

		internal void UpdateCurrentSky(SkyManager skyManager)
		{
			skyAmbientMode = volumeStack.GetComponent<VisualEnvironment>().skyAmbientMode.value;
			visualSky.skySettings = SkyManager.GetSkySetting(volumeStack);
			visualSky.cloudSettings = SkyManager.GetCloudSetting(volumeStack);
			visualSky.volumetricClouds = SkyManager.GetVolumetricClouds(volumeStack);
			lightingSky = visualSky;
			if ((int)skyManager.lightingOverrideLayerMask == 0)
			{
				return;
			}
			VolumeManager.instance.Update(skyManager.lightingOverrideVolumeStack, volumeAnchor, skyManager.lightingOverrideLayerMask);
			if (VolumeManager.instance.IsComponentActiveInMask<VisualEnvironment>(skyManager.lightingOverrideLayerMask))
			{
				SkySettings skySetting = SkyManager.GetSkySetting(skyManager.lightingOverrideVolumeStack);
				CloudSettings cloudSetting = SkyManager.GetCloudSetting(skyManager.lightingOverrideVolumeStack);
				VolumetricClouds volumetricClouds = SkyManager.GetVolumetricClouds(skyManager.lightingOverrideVolumeStack);
				if ((m_LightingOverrideSky.skySettings != null && skySetting == null) || (m_LightingOverrideSky.cloudSettings != null && cloudSetting == null) || (m_LightingOverrideSky.volumetricClouds != null && volumetricClouds == null))
				{
					visualSky.skyParametersHash = -1;
				}
				m_LightingOverrideSky.skySettings = skySetting;
				m_LightingOverrideSky.cloudSettings = cloudSetting;
				m_LightingOverrideSky.volumetricClouds = volumetricClouds;
				lightingSky = m_LightingOverrideSky;
			}
		}

		internal void OverridePixelRect(Rect newPixelRect)
		{
			m_OverridePixelRect = newPixelRect;
		}

		internal void ResetPixelRect()
		{
			m_OverridePixelRect = null;
		}

		private void SetupCurrentMaterialQuality(CommandBuffer cmd)
		{
			HDRenderPipelineAsset currentAsset = HDRenderPipeline.currentAsset;
			MaterialQuality availableMaterialQualityLevels = currentAsset.availableMaterialQualityLevels;
			MaterialQuality requestedLevel = ((frameSettings.materialQuality == (MaterialQuality)0) ? currentAsset.defaultMaterialQualityLevel : frameSettings.materialQuality);
			availableMaterialQualityLevels.GetClosestQuality(requestedLevel).SetGlobalShaderKeywords(cmd);
		}

		private void UpdateAntialiasing()
		{
			HDAdditionalCameraData.AntialiasingMode num = antialiasing;
			if (!frameSettings.IsEnabled(FrameSettingsField.Postprocess) || !CoreUtils.ArePostProcessesEnabled(camera))
			{
				antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
			}
			else if (m_AdditionalCameraData != null)
			{
				antialiasing = m_AdditionalCameraData.antialiasing;
				SMAAQuality = m_AdditionalCameraData.SMAAQuality;
				TAAQuality = m_AdditionalCameraData.TAAQuality;
				taaSharpenStrength = m_AdditionalCameraData.taaSharpenStrength;
				taaHistorySharpening = m_AdditionalCameraData.taaHistorySharpening;
				taaAntiFlicker = m_AdditionalCameraData.taaAntiFlicker;
				taaAntiRinging = m_AdditionalCameraData.taaAntiHistoryRinging;
				taaJitterScale = m_AdditionalCameraData.taaJitterScale;
				taaMotionVectorRejection = m_AdditionalCameraData.taaMotionVectorRejection;
				taaBaseBlendFactor = m_AdditionalCameraData.taaBaseBlendFactor;
			}
			else
			{
				antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
			}
			if (!RequiresCameraJitter())
			{
				taaFrameIndex = 0;
				taaJitter = Vector4.zero;
			}
			if (IsTAAUEnabled())
			{
				antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;
			}
			if ((num != antialiasing && antialiasing == HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing) || m_PreviousClearColorMode != clearColorMode)
			{
				resetPostProcessingHistory = true;
				m_PreviousClearColorMode = clearColorMode;
			}
		}

		private void GetXrViewParameters(int xrViewIndex, out Matrix4x4 proj, out Matrix4x4 view, out Vector3 cameraPosition)
		{
			proj = xr.GetProjMatrix(xrViewIndex);
			view = xr.GetViewMatrix(xrViewIndex);
			cameraPosition = view.inverse.GetColumn(3);
		}

		private void UpdateAllViewConstants()
		{
			if (m_XRViewConstants == null || m_XRViewConstants.Length != viewCount)
			{
				m_XRViewConstants = new ViewConstants[viewCount];
				resetPostProcessingHistory = true;
				isFirstFrame = true;
			}
			UpdateAllViewConstants(RequiresCameraJitter(), updatePreviousFrameConstants: true);
		}

		private void UpdateAllViewConstants(bool jitterProjectionMatrix, bool updatePreviousFrameConstants)
		{
			Matrix4x4 proj = camera.projectionMatrix;
			Matrix4x4 view = camera.worldToCameraMatrix;
			Vector3 cameraPosition = camera.transform.position;
			if (xr.enabled && viewCount == 1)
			{
				GetXrViewParameters(0, out proj, out view, out cameraPosition);
			}
			UpdateViewConstants(ref mainViewConstants, proj, view, cameraPosition, jitterProjectionMatrix, updatePreviousFrameConstants);
			if (xr.singlePassEnabled)
			{
				for (int i = 0; i < viewCount; i++)
				{
					GetXrViewParameters(i, out proj, out view, out cameraPosition);
					UpdateViewConstants(ref m_XRViewConstants[i], proj, view, cameraPosition, jitterProjectionMatrix, updatePreviousFrameConstants);
					m_XRViewConstants[i].worldSpaceCameraPosViewOffset = m_XRViewConstants[i].worldSpaceCameraPos - mainViewConstants.worldSpaceCameraPos;
				}
			}
			else
			{
				m_XRViewConstants[0] = mainViewConstants;
			}
			UpdateFrustum(in mainViewConstants);
			m_RecorderCaptureActions = CameraCaptureBridge.GetCaptureActions(camera);
		}

		private void UpdateViewConstants(ref ViewConstants viewConstants, Matrix4x4 projMatrix, Matrix4x4 viewMatrix, Vector3 cameraPosition, bool jitterProjectionMatrix, bool updatePreviousFrameConstants)
		{
			Matrix4x4 matrix = GL.GetGPUProjectionMatrix(jitterProjectionMatrix ? GetJitteredProjectionMatrix(projMatrix) : projMatrix, renderIntoTexture: true);
			Matrix4x4 matrix4x = viewMatrix;
			Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(projMatrix, renderIntoTexture: true);
			if (ShaderConfig.s_CameraRelativeRendering != 0)
			{
				matrix4x.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
			}
			Matrix4x4 prevViewProjMatrix = gPUProjectionMatrix * matrix4x;
			Matrix4x4 matrix4x2 = matrix4x;
			if (ShaderConfig.s_CameraRelativeRendering == 0)
			{
				matrix4x2.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
			}
			Matrix4x4 matrix4x3 = gPUProjectionMatrix * matrix4x2;
			if (updatePreviousFrameConstants)
			{
				if (isFirstFrame)
				{
					viewConstants.prevWorldSpaceCameraPos = cameraPosition;
					viewConstants.prevViewMatrix = matrix4x;
					viewConstants.prevViewProjMatrix = prevViewProjMatrix;
					viewConstants.prevInvViewProjMatrix = viewConstants.prevViewProjMatrix.inverse;
					viewConstants.prevViewProjMatrixNoCameraTrans = matrix4x3;
				}
				else
				{
					viewConstants.prevWorldSpaceCameraPos = viewConstants.worldSpaceCameraPos;
					viewConstants.prevViewMatrix = viewConstants.viewMatrix;
					viewConstants.prevViewProjMatrix = viewConstants.nonJitteredViewProjMatrix;
					viewConstants.prevViewProjMatrixNoCameraTrans = viewConstants.viewProjectionNoCameraTrans;
				}
			}
			viewConstants.viewMatrix = matrix4x;
			viewConstants.invViewMatrix = matrix4x.inverse;
			viewConstants.projMatrix = matrix;
			viewConstants.invProjMatrix = matrix.inverse;
			viewConstants.viewProjMatrix = matrix * matrix4x;
			viewConstants.invViewProjMatrix = viewConstants.viewProjMatrix.inverse;
			viewConstants.nonJitteredViewProjMatrix = gPUProjectionMatrix * matrix4x;
			viewConstants.worldSpaceCameraPos = cameraPosition;
			viewConstants.worldSpaceCameraPosViewOffset = Vector3.zero;
			viewConstants.viewProjectionNoCameraTrans = matrix4x3;
			float aspect = HDUtils.ProjectionMatrixAspect(in matrix);
			viewConstants.pixelCoordToViewDirWS = ComputePixelCoordToWorldSpaceViewDirectionMatrix(viewConstants, screenSize, aspect);
			if (updatePreviousFrameConstants)
			{
				Vector3 vector = viewConstants.worldSpaceCameraPos - viewConstants.prevWorldSpaceCameraPos;
				viewConstants.prevWorldSpaceCameraPos -= viewConstants.worldSpaceCameraPos;
				viewConstants.prevViewProjMatrix *= Matrix4x4.Translate(vector);
				viewConstants.prevInvViewProjMatrix = viewConstants.prevViewProjMatrix.inverse;
			}
		}

		private void UpdateFrustum(in ViewConstants viewConstants)
		{
			Matrix4x4 matrix4x = mainViewConstants.projMatrix;
			Matrix4x4 matrix4x2 = mainViewConstants.invProjMatrix;
			Matrix4x4 viewProjMatrix = mainViewConstants.viewProjMatrix;
			if (xr.enabled)
			{
				Matrix4x4 stereoProjectionMatrix = xr.cullingParams.stereoProjectionMatrix;
				Matrix4x4 stereoViewMatrix = xr.cullingParams.stereoViewMatrix;
				if (ShaderConfig.s_CameraRelativeRendering != 0)
				{
					Vector4 column = stereoViewMatrix.inverse.GetColumn(3) - (Vector4)camera.transform.position;
					stereoViewMatrix.SetColumn(3, column);
				}
				matrix4x = GL.GetGPUProjectionMatrix(stereoProjectionMatrix, renderIntoTexture: true);
				matrix4x2 = matrix4x.inverse;
				viewProjMatrix = matrix4x * stereoViewMatrix;
			}
			float nearClipPlane = camera.nearClipPlane;
			float farClipPlane = camera.farClipPlane;
			float num = matrix4x[2, 3] / (farClipPlane * nearClipPlane) * (farClipPlane - nearClipPlane);
			Mathf.Abs(num);
			bool num2 = num > 0f;
			bool flag = matrix4x2.MultiplyPoint(new Vector3(0f, 1f, 0f)).y < 0f;
			if (num2)
			{
				zBufferParams = new Vector4(-1f + farClipPlane / nearClipPlane, 1f, -1f / farClipPlane + 1f / nearClipPlane, 1f / farClipPlane);
			}
			else
			{
				zBufferParams = new Vector4(1f - farClipPlane / nearClipPlane, farClipPlane / nearClipPlane, 1f / farClipPlane - 1f / nearClipPlane, 1f / nearClipPlane);
			}
			projectionParams = new Vector4((!flag) ? 1 : (-1), nearClipPlane, farClipPlane, 1f / farClipPlane);
			float num3 = (camera.orthographic ? (2f * camera.orthographicSize) : 0f);
			float x = num3 * camera.aspect;
			unity_OrthoParams = new Vector4(x, num3, 0f, camera.orthographic ? 1 : 0);
			Vector3 viewDir = -viewConstants.invViewMatrix.GetColumn(2);
			viewDir.Normalize();
			Frustum.Create(ref frustum, viewProjMatrix, viewConstants.invViewMatrix.GetColumn(3), viewDir, nearClipPlane, farClipPlane);
			for (int i = 0; i < 6; i++)
			{
				frustumPlaneEquations[i] = new Vector4(frustum.planes[i].normal.x, frustum.planes[i].normal.y, frustum.planes[i].normal.z, frustum.planes[i].distance);
			}
		}

		internal static int GetSceneViewLayerMaskFallback()
		{
			HDRenderPipeline hDRenderPipeline = RenderPipelineManager.currentPipeline as HDRenderPipeline;
			if ((int)hDRenderPipeline.asset.currentPlatformRenderPipelineSettings.lightLoopSettings.skyLightingOverrideLayerMask == -1)
			{
				return -1;
			}
			return -1 & ~((int)hDRenderPipeline.asset.currentPlatformRenderPipelineSettings.lightLoopSettings.skyLightingOverrideLayerMask | int.MinValue);
		}

		private void UpdateVolumeAndPhysicalParameters()
		{
			volumeAnchor = null;
			volumeLayerMask = -1;
			if (m_AdditionalCameraData != null)
			{
				volumeLayerMask = m_AdditionalCameraData.volumeLayerMask;
				volumeAnchor = m_AdditionalCameraData.volumeAnchorOverride;
			}
			else if (camera.cameraType == CameraType.SceneView)
			{
				Camera main = Camera.main;
				bool flag = true;
				if (main != null && main.TryGetComponent<HDAdditionalCameraData>(out var component))
				{
					volumeLayerMask = component.volumeLayerMask;
					volumeAnchor = component.volumeAnchorOverride;
					flag = false;
				}
				if (flag)
				{
					volumeLayerMask = GetSceneViewLayerMaskFallback();
				}
			}
			if (volumeAnchor == null)
			{
				volumeAnchor = camera.transform;
			}
			using (new ProfilingScope(null, ProfilingSampler.Get(HDProfileId.VolumeUpdate)))
			{
				VolumeManager.instance.Update(volumeStack, volumeAnchor, volumeLayerMask);
			}
			switch (volumeStack.GetComponent<Exposure>().targetMidGray.value)
			{
			case TargetMidGray.Grey125:
				ColorUtils.s_LightMeterCalibrationConstant = 12.5f;
				break;
			case TargetMidGray.Grey14:
				ColorUtils.s_LightMeterCalibrationConstant = 14f;
				break;
			case TargetMidGray.Grey18:
				ColorUtils.s_LightMeterCalibrationConstant = 18f;
				break;
			default:
				ColorUtils.s_LightMeterCalibrationConstant = 12.5f;
				break;
			}
		}

		internal Matrix4x4 GetJitteredProjectionMatrix(Matrix4x4 origProj)
		{
			if (xr.enabled && !HDRenderPipeline.currentAsset.currentPlatformRenderPipelineSettings.xrSettings.cameraJitter)
			{
				taaJitter = Vector4.zero;
				return origProj;
			}
			if (FrameDebugger.enabled)
			{
				taaJitter = Vector4.zero;
				return origProj;
			}
			float num = HaltonSequence.Get((taaFrameIndex & 0x3FF) + 1, 2) - 0.5f;
			float num2 = HaltonSequence.Get((taaFrameIndex & 0x3FF) + 1, 3) - 0.5f;
			if (!IsDLSSEnabled() && !IsTAAUEnabled() && camera.cameraType != CameraType.SceneView)
			{
				num *= taaJitterScale;
				num2 *= taaJitterScale;
			}
			taaJitter = new Vector4(num, num2, num / (float)actualWidth, num2 / (float)actualHeight);
			if (camera.orthographic)
			{
				float orthographicSize = camera.orthographicSize;
				float num3 = orthographicSize * camera.aspect;
				Vector4 vector = taaJitter;
				vector.x *= num3 / (0.5f * (float)actualWidth);
				vector.y *= orthographicSize / (0.5f * (float)actualHeight);
				float left = vector.x - num3;
				float right = vector.x + num3;
				float top = vector.y + orthographicSize;
				float bottom = vector.y - orthographicSize;
				return Matrix4x4.Ortho(left, right, bottom, top, camera.nearClipPlane, camera.farClipPlane);
			}
			FrustumPlanes decomposeProjection = origProj.decomposeProjection;
			float num4 = Math.Abs(decomposeProjection.top) + Math.Abs(decomposeProjection.bottom);
			float num5 = Math.Abs(decomposeProjection.left) + Math.Abs(decomposeProjection.right);
			Vector2 vector2 = new Vector2(num * num5 / (float)actualWidth, num2 * num4 / (float)actualHeight);
			decomposeProjection.left += vector2.x;
			decomposeProjection.right += vector2.x;
			decomposeProjection.top += vector2.y;
			decomposeProjection.bottom += vector2.y;
			if (float.IsInfinity(decomposeProjection.zFar))
			{
				decomposeProjection.zFar = frustum.planes[5].distance;
			}
			return Matrix4x4.Frustum(decomposeProjection);
		}

		private Matrix4x4 ComputePixelCoordToWorldSpaceViewDirectionMatrix(ViewConstants viewConstants, Vector4 resolution, float aspect = -1f)
		{
			if ((xr.enabled || frameSettings.IsEnabled(FrameSettingsField.AsymmetricProjection)) | (HDUtils.IsProjectionMatrixAsymmetric(in viewConstants.projMatrix) && !camera.usePhysicalProperties))
			{
				Matrix4x4 matrix4x = new Matrix4x4(new Vector4(2f * resolution.z, 0f, 0f, -1f), new Vector4(0f, -2f * resolution.w, 0f, 1f), new Vector4(0f, 0f, 1f, 0f), new Vector4(0f, 0f, 0f, 1f));
				Matrix4x4 matrix4x2 = viewConstants.invViewProjMatrix.transpose * Matrix4x4.Scale(new Vector3(-1f, -1f, -1f));
				return matrix4x * matrix4x2;
			}
			float verticalFoV = camera.GetGateFittedFieldOfView() * (MathF.PI / 180f);
			if (!camera.usePhysicalProperties)
			{
				verticalFoV = Mathf.Atan(-1f / viewConstants.projMatrix[1, 1]) * 2f;
			}
			Vector2 gateFittedLensShift = camera.GetGateFittedLensShift();
			return HDUtils.ComputePixelCoordToWorldSpaceViewDirectionMatrix(verticalFoV, gateFittedLensShift, resolution, viewConstants.viewMatrix, renderToCubemap: false, aspect, camera.orthographic);
		}

		private void Dispose()
		{
			HDRenderPipeline.DestroyVolumetricHistoryBuffers(this);
			VolumeManager.instance.DestroyStack(volumeStack);
			if (m_HistoryRTSystem != null)
			{
				m_HistoryRTSystem.Dispose();
				m_HistoryRTSystem = null;
			}
			foreach (KeyValuePair<AOVRequestData, BufferedRTHandleSystem> item in m_AOVHistoryRTSystem)
			{
				item.Value.Dispose();
			}
			m_AOVHistoryRTSystem.Clear();
			if (lightingSky != null && lightingSky != visualSky)
			{
				lightingSky.Cleanup();
			}
			if (visualSky != null)
			{
				visualSky.Cleanup();
			}
		}

		private static RTHandle HistoryBufferAllocatorFunction(string viewName, int frameIndex, RTHandleSystem rtHandleSystem)
		{
			frameIndex &= 1;
			HDRenderPipeline hDRenderPipeline = (HDRenderPipeline)RenderPipelineManager.currentPipeline;
			return rtHandleSystem.Alloc(Vector2.one, TextureXR.slices, DepthBits.None, hDRenderPipeline.GetColorBufferFormat(), FilterMode.Point, TextureWrapMode.Repeat, TextureXR.dimension, enableRandomWrite: true, useMipMap: true, autoGenerateMips: false, isShadowMap: false, 1, 0f, MSAASamples.None, bindTextureMS: false, useDynamicScale: true, RenderTextureMemoryless.None, VRTextureUsage.None, $"{viewName}_CameraColorBufferMipChain{frameIndex}");
		}

		private void ReleaseHistoryBuffer()
		{
			m_HistoryRTSystem.ReleaseAll();
			foreach (KeyValuePair<AOVRequestData, BufferedRTHandleSystem> item in m_AOVHistoryRTSystem)
			{
				item.Value.ReleaseAll();
			}
		}

		private Rect GetPixelRect()
		{
			if (m_OverridePixelRect.HasValue)
			{
				return m_OverridePixelRect.Value;
			}
			return new Rect(camera.pixelRect.x, camera.pixelRect.y, camera.pixelWidth, camera.pixelHeight);
		}

		internal BufferedRTHandleSystem GetHistoryRTHandleSystem()
		{
			return m_HistoryRTSystem;
		}

		internal void BindHistoryRTHandleSystem(BufferedRTHandleSystem historyRTSystem)
		{
			m_HistoryRTSystem = historyRTSystem;
		}

		internal BufferedRTHandleSystem GetHistoryRTHandleSystem(AOVRequestData aovRequest)
		{
			if (m_AOVHistoryRTSystem.TryGetValue(aovRequest, out var value))
			{
				return value;
			}
			BufferedRTHandleSystem bufferedRTHandleSystem = new BufferedRTHandleSystem();
			m_AOVHistoryRTSystem.Add(aovRequest, bufferedRTHandleSystem);
			return bufferedRTHandleSystem;
		}
	}
}
