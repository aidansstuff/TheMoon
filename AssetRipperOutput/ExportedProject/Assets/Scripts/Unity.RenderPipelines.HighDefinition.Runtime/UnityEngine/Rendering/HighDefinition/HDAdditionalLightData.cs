using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Light))]
	[ExecuteAlways]
	public class HDAdditionalLightData : MonoBehaviour, ISerializationCallbackReceiver, IAdditionalData, IVersionable<HDAdditionalLightData.Version>
	{
		internal static class ScalableSettings
		{
			public static IntScalableSetting ShadowResolutionArea(HDRenderPipelineAsset hdrp)
			{
				return hdrp.currentPlatformRenderPipelineSettings.hdShadowInitParams.shadowResolutionArea;
			}

			public static IntScalableSetting ShadowResolutionPunctual(HDRenderPipelineAsset hdrp)
			{
				return hdrp.currentPlatformRenderPipelineSettings.hdShadowInitParams.shadowResolutionPunctual;
			}

			public static IntScalableSetting ShadowResolutionDirectional(HDRenderPipelineAsset hdrp)
			{
				return hdrp.currentPlatformRenderPipelineSettings.hdShadowInitParams.shadowResolutionDirectional;
			}

			public static BoolScalableSetting UseContactShadow(HDRenderPipelineAsset hdrp)
			{
				return hdrp.currentPlatformRenderPipelineSettings.lightSettings.useContactShadow;
			}
		}

		public delegate Matrix4x4 CustomViewCallback(Matrix4x4 lightLocalToWorldMatrix);

		private enum Version
		{
			_Unused00 = 0,
			_Unused01 = 1,
			ShadowNearPlane = 2,
			LightLayer = 3,
			ShadowLayer = 4,
			_Unused02 = 5,
			ShadowResolution = 6,
			RemoveAdditionalShadowData = 7,
			AreaLightShapeTypeLogicIsolation = 8,
			PCSSUIUpdate = 9,
			MoveEmissionMesh = 10,
			EnableApplyRangeAttenuationOnBoxLight = 11
		}

		[Obsolete]
		private enum ShadowResolutionTier
		{
			Low = 0,
			Medium = 1,
			High = 2,
			VeryHigh = 3
		}

		[Obsolete]
		private enum LightTypeExtent
		{
			Punctual = 0,
			Rectangle = 1,
			Tube = 2
		}

		internal enum PointLightHDType
		{
			Punctual = 0,
			Area = 1
		}

		internal const float k_MinLightSize = 0.01f;

		public const float k_DefaultDirectionalLightIntensity = MathF.PI;

		public const float k_DefaultPunctualLightIntensity = 600f;

		public const float k_DefaultAreaLightIntensity = 200f;

		public const float k_MinSpotAngle = 1f;

		public const float k_MaxSpotAngle = 179f;

		public const float k_MinAspectRatio = 0.05f;

		public const float k_MaxAspectRatio = 20f;

		public const float k_MinViewBiasScale = 0f;

		public const float k_MaxViewBiasScale = 15f;

		public const float k_MinAreaWidth = 0.01f;

		public const int k_DefaultShadowResolution = 512;

		internal const float k_MinEvsmExponent = 5f;

		internal const float k_MaxEvsmExponent = 42f;

		internal const float k_MinEvsmLightLeakBias = 0f;

		internal const float k_MaxEvsmLightLeakBias = 1f;

		internal const float k_MinEvsmVarianceBias = 0f;

		internal const float k_MaxEvsmVarianceBias = 0.001f;

		internal const int k_MinEvsmBlurPasses = 0;

		internal const int k_MaxEvsmBlurPasses = 8;

		internal const float k_MinSpotInnerPercent = 0f;

		internal const float k_MaxSpotInnerPercent = 100f;

		internal const float k_MinAreaLightShadowCone = 10f;

		internal const float k_MaxAreaLightShadowCone = 179f;

		internal static HashSet<HDAdditionalLightData> s_overlappingHDLights = new HashSet<HDAdditionalLightData>();

		[ExcludeCopy]
		internal HDLightRenderEntity lightEntity = HDLightRenderEntity.Invalid;

		[SerializeField]
		[FormerlySerializedAs("displayLightIntensity")]
		private float m_Intensity;

		[SerializeField]
		[FormerlySerializedAs("enableSpotReflector")]
		private bool m_EnableSpotReflector = true;

		[SerializeField]
		[FormerlySerializedAs("luxAtDistance")]
		private float m_LuxAtDistance = 1f;

		[Range(0f, 100f)]
		[SerializeField]
		private float m_InnerSpotPercent;

		[Range(0f, 100f)]
		[SerializeField]
		private float m_SpotIESCutoffPercent = 100f;

		[Range(0f, 16f)]
		[SerializeField]
		[FormerlySerializedAs("lightDimmer")]
		private float m_LightDimmer = 1f;

		[Range(0f, 16f)]
		[SerializeField]
		[FormerlySerializedAs("volumetricDimmer")]
		private float m_VolumetricDimmer = 1f;

		[SerializeField]
		[FormerlySerializedAs("lightUnit")]
		private LightUnit m_LightUnit;

		[SerializeField]
		[FormerlySerializedAs("fadeDistance")]
		private float m_FadeDistance = 10000f;

		[SerializeField]
		private float m_VolumetricFadeDistance = 10000f;

		[SerializeField]
		[FormerlySerializedAs("affectDiffuse")]
		private bool m_AffectDiffuse = true;

		[SerializeField]
		[FormerlySerializedAs("affectSpecular")]
		private bool m_AffectSpecular = true;

		[SerializeField]
		[FormerlySerializedAs("nonLightmappedOnly")]
		private bool m_NonLightmappedOnly;

		[SerializeField]
		[FormerlySerializedAs("shapeWidth")]
		private float m_ShapeWidth = 0.5f;

		[SerializeField]
		[FormerlySerializedAs("shapeHeight")]
		private float m_ShapeHeight = 0.5f;

		[SerializeField]
		[FormerlySerializedAs("aspectRatio")]
		private float m_AspectRatio = 1f;

		[SerializeField]
		[FormerlySerializedAs("shapeRadius")]
		private float m_ShapeRadius = 0.025f;

		[SerializeField]
		private float m_SoftnessScale = 1f;

		[SerializeField]
		[FormerlySerializedAs("useCustomSpotLightShadowCone")]
		private bool m_UseCustomSpotLightShadowCone;

		[SerializeField]
		[FormerlySerializedAs("customSpotLightShadowCone")]
		private float m_CustomSpotLightShadowCone = 30f;

		[Range(0f, 1f)]
		[SerializeField]
		[FormerlySerializedAs("maxSmoothness")]
		private float m_MaxSmoothness = 0.99f;

		[SerializeField]
		[FormerlySerializedAs("applyRangeAttenuation")]
		private bool m_ApplyRangeAttenuation = true;

		[SerializeField]
		[FormerlySerializedAs("displayAreaLightEmissiveMesh")]
		private bool m_DisplayAreaLightEmissiveMesh;

		[SerializeField]
		[FormerlySerializedAs("areaLightCookie")]
		private Texture m_AreaLightCookie;

		[SerializeField]
		internal Texture m_IESPoint;

		[SerializeField]
		internal Texture m_IESSpot;

		[SerializeField]
		private bool m_IncludeForRayTracing = true;

		[Range(10f, 179f)]
		[SerializeField]
		[FormerlySerializedAs("areaLightShadowCone")]
		private float m_AreaLightShadowCone = 120f;

		[SerializeField]
		[FormerlySerializedAs("useScreenSpaceShadows")]
		private bool m_UseScreenSpaceShadows;

		[SerializeField]
		[FormerlySerializedAs("interactsWithSky")]
		private bool m_InteractsWithSky = true;

		[SerializeField]
		[FormerlySerializedAs("angularDiameter")]
		private float m_AngularDiameter = 0.5f;

		[SerializeField]
		[FormerlySerializedAs("flareSize")]
		private float m_FlareSize = 2f;

		[SerializeField]
		[FormerlySerializedAs("flareTint")]
		private Color m_FlareTint = Color.white;

		[SerializeField]
		[FormerlySerializedAs("flareFalloff")]
		private float m_FlareFalloff = 4f;

		[SerializeField]
		[FormerlySerializedAs("surfaceTexture")]
		private Texture2D m_SurfaceTexture;

		[SerializeField]
		[FormerlySerializedAs("surfaceTint")]
		private Color m_SurfaceTint = Color.white;

		[SerializeField]
		[FormerlySerializedAs("distance")]
		private float m_Distance = 1.5E+11f;

		[SerializeField]
		[FormerlySerializedAs("useRayTracedShadows")]
		private bool m_UseRayTracedShadows;

		[Range(1f, 32f)]
		[SerializeField]
		[FormerlySerializedAs("numRayTracingSamples")]
		private int m_NumRayTracingSamples = 4;

		[SerializeField]
		[FormerlySerializedAs("filterTracedShadow")]
		private bool m_FilterTracedShadow = true;

		[Range(1f, 32f)]
		[SerializeField]
		[FormerlySerializedAs("filterSizeTraced")]
		private int m_FilterSizeTraced = 16;

		[Range(0f, 2f)]
		[SerializeField]
		[FormerlySerializedAs("sunLightConeAngle")]
		private float m_SunLightConeAngle = 0.5f;

		[SerializeField]
		[FormerlySerializedAs("lightShadowRadius")]
		private float m_LightShadowRadius = 0.5f;

		[SerializeField]
		private bool m_SemiTransparentShadow;

		[SerializeField]
		private bool m_ColorShadow = true;

		[SerializeField]
		private bool m_DistanceBasedFiltering;

		[Range(5f, 42f)]
		[SerializeField]
		[FormerlySerializedAs("evsmExponent")]
		private float m_EvsmExponent = 15f;

		[Range(0f, 1f)]
		[SerializeField]
		[FormerlySerializedAs("evsmLightLeakBias")]
		private float m_EvsmLightLeakBias;

		[Range(0f, 0.001f)]
		[SerializeField]
		[FormerlySerializedAs("evsmVarianceBias")]
		private float m_EvsmVarianceBias = 1E-05f;

		[Range(0f, 8f)]
		[SerializeField]
		[FormerlySerializedAs("evsmBlurPasses")]
		private int m_EvsmBlurPasses;

		[SerializeField]
		[FormerlySerializedAs("lightlayersMask")]
		private LightLayerEnum m_LightlayersMask = LightLayerEnum.LightLayerDefault;

		[SerializeField]
		[FormerlySerializedAs("linkShadowLayers")]
		private bool m_LinkShadowLayers = true;

		[SerializeField]
		[FormerlySerializedAs("shadowNearPlane")]
		private float m_ShadowNearPlane = 0.1f;

		[Range(1f, 64f)]
		[SerializeField]
		[FormerlySerializedAs("blockerSampleCount")]
		private int m_BlockerSampleCount = 24;

		[Range(1f, 64f)]
		[SerializeField]
		[FormerlySerializedAs("filterSampleCount")]
		private int m_FilterSampleCount = 16;

		[Range(0f, 1f)]
		[SerializeField]
		[FormerlySerializedAs("minFilterSize")]
		private float m_MinFilterSize = 0.1f;

		[Range(1f, 32f)]
		[SerializeField]
		[FormerlySerializedAs("kernelSize")]
		private int m_KernelSize = 5;

		[Range(0f, 9f)]
		[SerializeField]
		[FormerlySerializedAs("lightAngle")]
		private float m_LightAngle = 1f;

		[Range(0.0001f, 0.01f)]
		[SerializeField]
		[FormerlySerializedAs("maxDepthBias")]
		private float m_MaxDepthBias = 0.001f;

		[ValueCopy]
		[SerializeField]
		private IntScalableSettingValue m_ShadowResolution = new IntScalableSettingValue
		{
			@override = 512,
			useOverride = true
		};

		[Range(0f, 1f)]
		[SerializeField]
		private float m_ShadowDimmer = 1f;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_VolumetricShadowDimmer = 1f;

		[SerializeField]
		private float m_ShadowFadeDistance = 10000f;

		[SerializeField]
		[ValueCopy]
		private BoolScalableSettingValue m_UseContactShadow = new BoolScalableSettingValue
		{
			useOverride = true
		};

		[SerializeField]
		private bool m_RayTracedContactShadow;

		[SerializeField]
		private Color m_ShadowTint = Color.black;

		[SerializeField]
		private bool m_PenumbraTint;

		[SerializeField]
		private float m_NormalBias = 0.75f;

		[SerializeField]
		private float m_SlopeBias = 0.5f;

		[SerializeField]
		private ShadowUpdateMode m_ShadowUpdateMode;

		[SerializeField]
		private bool m_AlwaysDrawDynamicShadows;

		[SerializeField]
		private bool m_UpdateShadowOnLightMovement;

		[SerializeField]
		private float m_CachedShadowTranslationThreshold = 0.01f;

		[SerializeField]
		private float m_CachedShadowAngularThreshold = 0.5f;

		[Range(0f, 90f)]
		[SerializeField]
		private float m_BarnDoorAngle = 90f;

		[SerializeField]
		private float m_BarnDoorLength = 0.05f;

		[SerializeField]
		private bool m_preserveCachedShadow;

		[SerializeField]
		private bool m_OnDemandShadowRenderOnPlacement = true;

		internal bool forceRenderOnPlacement;

		[SerializeField]
		[ValueCopy]
		private float[] m_ShadowCascadeRatios = new float[3] { 0.05f, 0.2f, 0.3f };

		[SerializeField]
		[ValueCopy]
		private float[] m_ShadowCascadeBorders = new float[4] { 0.2f, 0.2f, 0.2f, 0.2f };

		[SerializeField]
		private int m_ShadowAlgorithm;

		[SerializeField]
		private int m_ShadowVariant;

		[SerializeField]
		private int m_ShadowPrecision;

		[SerializeField]
		[FormerlySerializedAs("useOldInspector")]
		private bool useOldInspector;

		[SerializeField]
		[FormerlySerializedAs("useVolumetric")]
		private bool useVolumetric = true;

		[SerializeField]
		[FormerlySerializedAs("featuresFoldout")]
		private bool featuresFoldout = true;

		[ExcludeCopy]
		private HDShadowRequest[] shadowRequests;

		[ExcludeCopy]
		private int[] m_ShadowRequestIndices;

		[NonSerialized]
		[ExcludeCopy]
		internal int lightIdxForCachedShadows = -1;

		[ExcludeCopy]
		private Vector3[] m_CachedViewPositions;

		[NonSerialized]
		[ExcludeCopy]
		private Plane[] m_ShadowFrustumPlanes = new Plane[6];

		[NonSerialized]
		[ExcludeCopy]
		internal Matrix4x4 previousTransform = Matrix4x4.identity;

		[NonSerialized]
		[ExcludeCopy]
		internal int shadowIndex = -1;

		[ExcludeCopy]
		private Light m_Light;

		private const string k_EmissiveMeshViewerName = "EmissiveMeshViewer";

		[ExcludeCopy]
		private GameObject m_ChildEmissiveMeshViewer;

		[ExcludeCopy]
		private MeshFilter m_EmissiveMeshFilter;

		[ExcludeCopy]
		private bool needRefreshEmissiveMeshesFromTimeLineUpdate;

		[SerializeField]
		private ShadowCastingMode m_AreaLightEmissiveMeshShadowCastingMode;

		[SerializeField]
		private MotionVectorGenerationMode m_AreaLightEmissiveMeshMotionVectorGenerationMode;

		[SerializeField]
		private int m_AreaLightEmissiveMeshLayer = -1;

		public CustomViewCallback CustomViewCallbackEvent;

		[NonSerialized]
		private TimelineWorkaround timelineWorkaround;

		[NonSerialized]
		[ExcludeCopy]
		private bool m_Animated;

		[SerializeField]
		[ExcludeCopy]
		private Version m_Version = MigrationDescription.LastVersion<Version>();

		private static readonly MigrationDescription<Version, HDAdditionalLightData> k_HDLightMigrationSteps = MigrationDescription.New<Version, HDAdditionalLightData>(MigrationStep.New(Version.ShadowNearPlane, delegate(HDAdditionalLightData data)
		{
			data.shadowNearPlane = data.legacyLight.shadowNearPlane;
		}), MigrationStep.New(Version.LightLayer, delegate(HDAdditionalLightData data)
		{
			data.legacyLight.renderingLayerMask = LightLayerToRenderingLayerMask((int)data.m_LightLayers, data.legacyLight.renderingLayerMask);
		}), MigrationStep.New(Version.ShadowLayer, delegate(HDAdditionalLightData data)
		{
			data.lightlayersMask = (LightLayerEnum)RenderingLayerMaskToLightLayer(data.legacyLight.renderingLayerMask);
		}), MigrationStep.New(Version.ShadowResolution, delegate(HDAdditionalLightData data)
		{
			AdditionalShadowData component3 = data.GetComponent<AdditionalShadowData>();
			if (component3 != null)
			{
				data.m_ObsoleteCustomShadowResolution = component3.customResolution;
				data.m_ObsoleteContactShadows = component3.contactShadows;
				data.shadowDimmer = component3.shadowDimmer;
				data.volumetricShadowDimmer = component3.volumetricShadowDimmer;
				data.shadowFadeDistance = component3.shadowFadeDistance;
				data.shadowTint = component3.shadowTint;
				data.normalBias = component3.normalBias;
				data.shadowUpdateMode = component3.shadowUpdateMode;
				data.shadowCascadeRatios = component3.shadowCascadeRatios;
				data.shadowCascadeBorders = component3.shadowCascadeBorders;
				data.shadowAlgorithm = component3.shadowAlgorithm;
				data.shadowVariant = component3.shadowVariant;
				data.shadowPrecision = component3.shadowPrecision;
				CoreUtils.Destroy(component3);
			}
			data.shadowResolution.@override = data.m_ObsoleteCustomShadowResolution;
			switch (data.m_ObsoleteShadowResolutionTier)
			{
			case ShadowResolutionTier.Low:
				data.shadowResolution.level = 0;
				break;
			case ShadowResolutionTier.Medium:
				data.shadowResolution.level = 1;
				break;
			case ShadowResolutionTier.High:
				data.shadowResolution.level = 2;
				break;
			case ShadowResolutionTier.VeryHigh:
				data.shadowResolution.level = 3;
				break;
			}
			data.shadowResolution.useOverride = !data.m_ObsoleteUseShadowQualitySettings;
			data.useContactShadow.@override = data.m_ObsoleteContactShadows;
		}), MigrationStep.New(Version.RemoveAdditionalShadowData, delegate(HDAdditionalLightData data)
		{
			AdditionalShadowData component2 = data.GetComponent<AdditionalShadowData>();
			if (component2 != null)
			{
				CoreUtils.Destroy(component2);
			}
		}), MigrationStep.New(Version.AreaLightShapeTypeLogicIsolation, delegate(HDAdditionalLightData data)
		{
			switch ((LightTypeExtent)data.m_PointlightHDType)
			{
			case LightTypeExtent.Punctual:
				data.m_PointlightHDType = PointLightHDType.Punctual;
				break;
			case LightTypeExtent.Rectangle:
				data.m_PointlightHDType = PointLightHDType.Area;
				data.m_AreaLightShape = AreaLightShape.Rectangle;
				break;
			case LightTypeExtent.Tube:
				data.m_PointlightHDType = PointLightHDType.Area;
				data.m_AreaLightShape = AreaLightShape.Tube;
				break;
			}
		}), MigrationStep.New(Version.PCSSUIUpdate, delegate(HDAdditionalLightData data)
		{
			data.minFilterSize *= 1000f;
		}), MigrationStep.New(Version.MoveEmissionMesh, delegate(HDAdditionalLightData data)
		{
			MeshRenderer component = data.GetComponent<MeshRenderer>();
			bool num = component != null;
			ShadowCastingMode shadowCastingMode = ShadowCastingMode.Off;
			MotionVectorGenerationMode motionVectorGenerationMode = MotionVectorGenerationMode.Camera;
			if (num)
			{
				shadowCastingMode = component.shadowCastingMode;
				motionVectorGenerationMode = component.motionVectorGenerationMode;
			}
			CoreUtils.Destroy(data.GetComponent<MeshFilter>());
			CoreUtils.Destroy(component);
			if (num)
			{
				data.m_AreaLightEmissiveMeshShadowCastingMode = shadowCastingMode;
				data.m_AreaLightEmissiveMeshMotionVectorGenerationMode = motionVectorGenerationMode;
			}
		}), MigrationStep.New(Version.EnableApplyRangeAttenuationOnBoxLight, delegate(HDAdditionalLightData data)
		{
			if (data.type == HDLightType.Spot && data.spotLightShape == SpotLightShape.Box)
			{
				data.applyRangeAttenuation = false;
			}
		}));

		[Obsolete("Use Light.renderingLayerMask instead")]
		[FormerlySerializedAs("lightLayers")]
		[ExcludeCopy]
		private LightLayerEnum m_LightLayers = LightLayerEnum.LightLayerDefault;

		[Obsolete]
		[SerializeField]
		[FormerlySerializedAs("m_ShadowResolutionTier")]
		[ExcludeCopy]
		private ShadowResolutionTier m_ObsoleteShadowResolutionTier = ShadowResolutionTier.Medium;

		[Obsolete]
		[SerializeField]
		[FormerlySerializedAs("m_UseShadowQualitySettings")]
		[ExcludeCopy]
		private bool m_ObsoleteUseShadowQualitySettings;

		[FormerlySerializedAs("m_CustomShadowResolution")]
		[Obsolete]
		[SerializeField]
		[ExcludeCopy]
		private int m_ObsoleteCustomShadowResolution = 512;

		[FormerlySerializedAs("m_ContactShadows")]
		[Obsolete]
		[SerializeField]
		[ExcludeCopy]
		private bool m_ObsoleteContactShadows;

		[NonSerialized]
		private static Dictionary<int, LightUnit[]> supportedLightTypeCache = new Dictionary<int, LightUnit[]>();

		[SerializeField]
		[FormerlySerializedAs("lightTypeExtent")]
		[FormerlySerializedAs("m_LightTypeExtent")]
		private PointLightHDType m_PointlightHDType;

		[SerializeField]
		[FormerlySerializedAs("spotLightShape")]
		private SpotLightShape m_SpotLightShape;

		[SerializeField]
		private AreaLightShape m_AreaLightShape;

		public float intensity
		{
			get
			{
				return m_Intensity;
			}
			set
			{
				if (m_Intensity != value)
				{
					m_Intensity = Mathf.Clamp(value, 0f, float.MaxValue);
					UpdateLightIntensity();
				}
			}
		}

		public bool enableSpotReflector
		{
			get
			{
				return m_EnableSpotReflector;
			}
			set
			{
				if (m_EnableSpotReflector != value)
				{
					m_EnableSpotReflector = value;
					UpdateLightIntensity();
				}
			}
		}

		public float luxAtDistance
		{
			get
			{
				return m_LuxAtDistance;
			}
			set
			{
				if (m_LuxAtDistance != value)
				{
					m_LuxAtDistance = Mathf.Clamp(value, 0f, float.MaxValue);
					UpdateLightIntensity();
				}
			}
		}

		public float innerSpotPercent
		{
			get
			{
				return m_InnerSpotPercent;
			}
			set
			{
				if (m_InnerSpotPercent != value)
				{
					m_InnerSpotPercent = Mathf.Clamp(value, 0f, 100f);
					UpdateLightIntensity();
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).innerSpotPercent = m_InnerSpotPercent;
					}
				}
			}
		}

		public float innerSpotPercent01 => innerSpotPercent / 100f;

		public float spotIESCutoffPercent
		{
			get
			{
				return m_SpotIESCutoffPercent;
			}
			set
			{
				if (m_SpotIESCutoffPercent != value)
				{
					m_SpotIESCutoffPercent = Mathf.Clamp(value, 0f, 100f);
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).spotIESCutoffPercent = m_SpotIESCutoffPercent;
					}
				}
			}
		}

		public float spotIESCutoffPercent01 => spotIESCutoffPercent / 100f;

		public float lightDimmer
		{
			get
			{
				return m_LightDimmer;
			}
			set
			{
				if (m_LightDimmer != value)
				{
					m_LightDimmer = Mathf.Clamp(value, 0f, 16f);
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).lightDimmer = m_LightDimmer;
					}
				}
			}
		}

		public float volumetricDimmer
		{
			get
			{
				if (!useVolumetric)
				{
					return 0f;
				}
				return m_VolumetricDimmer;
			}
			set
			{
				if (m_VolumetricDimmer != value)
				{
					m_VolumetricDimmer = Mathf.Clamp(value, 0f, 16f);
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).volumetricDimmer = m_VolumetricDimmer;
					}
				}
			}
		}

		public LightUnit lightUnit
		{
			get
			{
				return m_LightUnit;
			}
			set
			{
				if (m_LightUnit != value)
				{
					if (!IsValidLightUnitForType(type, m_SpotLightShape, value))
					{
						string arg = string.Join(", ", GetSupportedLightUnits(type, m_SpotLightShape));
						Debug.LogError($"Set Light Unit '{value}' to a {GetLightTypeName()} is not allowed, only {arg} are supported.");
					}
					else
					{
						LightUtils.ConvertLightIntensity(m_LightUnit, value, this, legacyLight);
						m_LightUnit = value;
						UpdateLightIntensity();
					}
				}
			}
		}

		public float fadeDistance
		{
			get
			{
				return m_FadeDistance;
			}
			set
			{
				if (m_FadeDistance != value)
				{
					m_FadeDistance = Mathf.Clamp(value, 0f, float.MaxValue);
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).fadeDistance = m_FadeDistance;
					}
				}
			}
		}

		public float volumetricFadeDistance
		{
			get
			{
				return m_VolumetricFadeDistance;
			}
			set
			{
				if (m_VolumetricFadeDistance != value)
				{
					m_VolumetricFadeDistance = Mathf.Clamp(value, 0f, float.MaxValue);
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).volumetricFadeDistance = m_VolumetricFadeDistance;
					}
				}
			}
		}

		public bool affectDiffuse
		{
			get
			{
				return m_AffectDiffuse;
			}
			set
			{
				if (m_AffectDiffuse != value)
				{
					m_AffectDiffuse = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).affectDiffuse = m_AffectDiffuse;
					}
				}
			}
		}

		public bool affectSpecular
		{
			get
			{
				return m_AffectSpecular;
			}
			set
			{
				if (m_AffectSpecular != value)
				{
					m_AffectSpecular = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).affectSpecular = m_AffectSpecular;
					}
				}
			}
		}

		public bool nonLightmappedOnly
		{
			get
			{
				return m_NonLightmappedOnly;
			}
			set
			{
				if (m_NonLightmappedOnly != value)
				{
					m_NonLightmappedOnly = value;
					legacyLight.lightShadowCasterMode = (value ? LightShadowCasterMode.NonLightmappedOnly : LightShadowCasterMode.Everything);
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).useRayTracedShadows = m_UseRayTracedShadows && !m_NonLightmappedOnly;
					}
				}
			}
		}

		public float shapeWidth
		{
			get
			{
				return m_ShapeWidth;
			}
			set
			{
				if (m_ShapeWidth != value)
				{
					if (type == HDLightType.Area)
					{
						m_ShapeWidth = Mathf.Clamp(value, 0.01f, float.MaxValue);
					}
					else
					{
						m_ShapeWidth = Mathf.Clamp(value, 0f, float.MaxValue);
					}
					UpdateAllLightValues();
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).shapeWidth = m_ShapeWidth;
					}
				}
			}
		}

		public float shapeHeight
		{
			get
			{
				return m_ShapeHeight;
			}
			set
			{
				if (m_ShapeHeight != value)
				{
					if (type == HDLightType.Area)
					{
						m_ShapeHeight = Mathf.Clamp(value, 0.01f, float.MaxValue);
					}
					else
					{
						m_ShapeHeight = Mathf.Clamp(value, 0f, float.MaxValue);
					}
					UpdateAllLightValues();
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).shapeHeight = m_ShapeHeight;
					}
				}
			}
		}

		public float aspectRatio
		{
			get
			{
				return m_AspectRatio;
			}
			set
			{
				if (m_AspectRatio != value)
				{
					m_AspectRatio = Mathf.Clamp(value, 0.05f, 20f);
					UpdateAllLightValues();
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).aspectRatio = m_AspectRatio;
					}
				}
			}
		}

		public float shapeRadius
		{
			get
			{
				return m_ShapeRadius;
			}
			set
			{
				if (m_ShapeRadius != value)
				{
					m_ShapeRadius = Mathf.Clamp(value, 0f, float.MaxValue);
					UpdateAllLightValues();
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).shapeRadius = m_ShapeRadius;
					}
				}
			}
		}

		public float softnessScale
		{
			get
			{
				return m_SoftnessScale;
			}
			set
			{
				if (m_SoftnessScale != value)
				{
					m_SoftnessScale = Mathf.Clamp(value, 0f, float.MaxValue);
					UpdateAllLightValues();
				}
			}
		}

		public bool useCustomSpotLightShadowCone
		{
			get
			{
				return m_UseCustomSpotLightShadowCone;
			}
			set
			{
				if (m_UseCustomSpotLightShadowCone != value)
				{
					m_UseCustomSpotLightShadowCone = value;
				}
			}
		}

		public float customSpotLightShadowCone
		{
			get
			{
				return m_CustomSpotLightShadowCone;
			}
			set
			{
				if (m_CustomSpotLightShadowCone != value)
				{
					m_CustomSpotLightShadowCone = value;
				}
			}
		}

		public float maxSmoothness
		{
			get
			{
				return m_MaxSmoothness;
			}
			set
			{
				if (m_MaxSmoothness != value)
				{
					m_MaxSmoothness = Mathf.Clamp01(value);
				}
			}
		}

		public bool applyRangeAttenuation
		{
			get
			{
				return m_ApplyRangeAttenuation;
			}
			set
			{
				if (m_ApplyRangeAttenuation != value)
				{
					m_ApplyRangeAttenuation = value;
					UpdateAllLightValues();
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).applyRangeAttenuation = m_ApplyRangeAttenuation;
					}
				}
			}
		}

		public bool displayAreaLightEmissiveMesh
		{
			get
			{
				return m_DisplayAreaLightEmissiveMesh;
			}
			set
			{
				if (m_DisplayAreaLightEmissiveMesh != value)
				{
					m_DisplayAreaLightEmissiveMesh = value;
					UpdateAllLightValues();
				}
			}
		}

		public Texture areaLightCookie
		{
			get
			{
				return m_AreaLightCookie;
			}
			set
			{
				if (!(m_AreaLightCookie == value))
				{
					m_AreaLightCookie = value;
					UpdateAllLightValues();
				}
			}
		}

		internal Texture IESPoint
		{
			get
			{
				return m_IESPoint;
			}
			set
			{
				if (value.dimension == TextureDimension.Cube)
				{
					m_IESPoint = value;
					UpdateAllLightValues();
				}
				else
				{
					Debug.LogError("Texture dimension " + value.dimension.ToString() + " is not supported for point lights.");
					m_IESPoint = null;
				}
			}
		}

		internal Texture IESSpot
		{
			get
			{
				return m_IESSpot;
			}
			set
			{
				if (value.dimension == TextureDimension.Tex2D && value.width == value.height)
				{
					m_IESSpot = value;
					UpdateAllLightValues();
				}
				else
				{
					Debug.LogError("Texture dimension " + value.dimension.ToString() + " is not supported for spot lights or rectangular light (only square images).");
					m_IESSpot = null;
				}
			}
		}

		public Texture IESTexture
		{
			get
			{
				if (type == HDLightType.Point)
				{
					return IESPoint;
				}
				if (type == HDLightType.Spot || (type == HDLightType.Area && areaLightShape == AreaLightShape.Rectangle))
				{
					return IESSpot;
				}
				return null;
			}
			set
			{
				if (type == HDLightType.Point)
				{
					IESPoint = value;
				}
				else if (type == HDLightType.Spot || (type == HDLightType.Area && areaLightShape == AreaLightShape.Rectangle))
				{
					IESSpot = value;
				}
			}
		}

		public bool includeForRayTracing
		{
			get
			{
				return m_IncludeForRayTracing;
			}
			set
			{
				if (m_IncludeForRayTracing != value)
				{
					m_IncludeForRayTracing = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).includeForRayTracing = m_IncludeForRayTracing;
					}
					UpdateAllLightValues();
				}
			}
		}

		public float areaLightShadowCone
		{
			get
			{
				return m_AreaLightShadowCone;
			}
			set
			{
				if (m_AreaLightShadowCone != value)
				{
					m_AreaLightShadowCone = Mathf.Clamp(value, 10f, 179f);
					UpdateAllLightValues();
				}
			}
		}

		public bool useScreenSpaceShadows
		{
			get
			{
				return m_UseScreenSpaceShadows;
			}
			set
			{
				if (m_UseScreenSpaceShadows != value)
				{
					m_UseScreenSpaceShadows = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).useScreenSpaceShadows = m_UseScreenSpaceShadows;
					}
				}
			}
		}

		public bool interactsWithSky
		{
			get
			{
				return m_InteractsWithSky;
			}
			set
			{
				if (m_InteractsWithSky != value)
				{
					m_InteractsWithSky = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).interactsWithSky = m_InteractsWithSky;
					}
				}
			}
		}

		public float angularDiameter
		{
			get
			{
				return m_AngularDiameter;
			}
			set
			{
				if (m_AngularDiameter != value)
				{
					m_AngularDiameter = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).angularDiameter = m_AngularDiameter;
					}
				}
			}
		}

		public float flareSize
		{
			get
			{
				return m_FlareSize;
			}
			set
			{
				if (m_FlareSize != value)
				{
					m_FlareSize = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).flareSize = m_FlareSize;
					}
				}
			}
		}

		public Color flareTint
		{
			get
			{
				return m_FlareTint;
			}
			set
			{
				if (!(m_FlareTint == value))
				{
					m_FlareTint = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).flareTint = m_FlareTint;
					}
				}
			}
		}

		public float flareFalloff
		{
			get
			{
				return m_FlareFalloff;
			}
			set
			{
				if (m_FlareFalloff != value)
				{
					m_FlareFalloff = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).flareFalloff = m_FlareFalloff;
					}
				}
			}
		}

		public Texture2D surfaceTexture
		{
			get
			{
				return m_SurfaceTexture;
			}
			set
			{
				if (!(m_SurfaceTexture == value))
				{
					m_SurfaceTexture = value;
				}
			}
		}

		public Color surfaceTint
		{
			get
			{
				return m_SurfaceTint;
			}
			set
			{
				if (!(m_SurfaceTint == value))
				{
					m_SurfaceTint = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).surfaceTint = m_SurfaceTint;
					}
				}
			}
		}

		public float distance
		{
			get
			{
				return m_Distance;
			}
			set
			{
				if (m_Distance != value)
				{
					m_Distance = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).distance = m_Distance;
					}
				}
			}
		}

		public bool useRayTracedShadows
		{
			get
			{
				return m_UseRayTracedShadows;
			}
			set
			{
				if (m_UseRayTracedShadows != value)
				{
					m_UseRayTracedShadows = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).useRayTracedShadows = m_UseRayTracedShadows;
					}
				}
			}
		}

		public int numRayTracingSamples
		{
			get
			{
				return m_NumRayTracingSamples;
			}
			set
			{
				if (m_NumRayTracingSamples != value)
				{
					m_NumRayTracingSamples = Mathf.Clamp(value, 1, 32);
				}
			}
		}

		public bool filterTracedShadow
		{
			get
			{
				return m_FilterTracedShadow;
			}
			set
			{
				if (m_FilterTracedShadow != value)
				{
					m_FilterTracedShadow = value;
				}
			}
		}

		public int filterSizeTraced
		{
			get
			{
				return m_FilterSizeTraced;
			}
			set
			{
				if (m_FilterSizeTraced != value)
				{
					m_FilterSizeTraced = Mathf.Clamp(value, 1, 32);
				}
			}
		}

		public float sunLightConeAngle
		{
			get
			{
				return m_SunLightConeAngle;
			}
			set
			{
				if (m_SunLightConeAngle != value)
				{
					m_SunLightConeAngle = Mathf.Clamp(value, 0f, 2f);
				}
			}
		}

		public float lightShadowRadius
		{
			get
			{
				return m_LightShadowRadius;
			}
			set
			{
				if (m_LightShadowRadius != value)
				{
					m_LightShadowRadius = Mathf.Max(value, 0.001f);
				}
			}
		}

		public bool semiTransparentShadow
		{
			get
			{
				return m_SemiTransparentShadow;
			}
			set
			{
				if (m_SemiTransparentShadow != value)
				{
					m_SemiTransparentShadow = value;
				}
			}
		}

		public bool colorShadow
		{
			get
			{
				return m_ColorShadow;
			}
			set
			{
				if (m_ColorShadow != value)
				{
					m_ColorShadow = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).colorShadow = m_ColorShadow;
					}
				}
			}
		}

		internal bool distanceBasedFiltering
		{
			get
			{
				return m_DistanceBasedFiltering;
			}
			set
			{
				if (m_DistanceBasedFiltering != value)
				{
					m_DistanceBasedFiltering = value;
				}
			}
		}

		public float evsmExponent
		{
			get
			{
				return m_EvsmExponent;
			}
			set
			{
				if (m_EvsmExponent != value)
				{
					m_EvsmExponent = Mathf.Clamp(value, 5f, 42f);
				}
			}
		}

		public float evsmLightLeakBias
		{
			get
			{
				return m_EvsmLightLeakBias;
			}
			set
			{
				if (m_EvsmLightLeakBias != value)
				{
					m_EvsmLightLeakBias = Mathf.Clamp(value, 0f, 1f);
				}
			}
		}

		public float evsmVarianceBias
		{
			get
			{
				return m_EvsmVarianceBias;
			}
			set
			{
				if (m_EvsmVarianceBias != value)
				{
					m_EvsmVarianceBias = Mathf.Clamp(value, 0f, 0.001f);
				}
			}
		}

		public int evsmBlurPasses
		{
			get
			{
				return m_EvsmBlurPasses;
			}
			set
			{
				if (m_EvsmBlurPasses != value)
				{
					m_EvsmBlurPasses = Mathf.Clamp(value, 0, 8);
				}
			}
		}

		public LightLayerEnum lightlayersMask
		{
			get
			{
				if (!linkShadowLayers)
				{
					return m_LightlayersMask;
				}
				return (LightLayerEnum)RenderingLayerMaskToLightLayer(legacyLight.renderingLayerMask);
			}
			set
			{
				m_LightlayersMask = value;
				if (lightEntity.valid)
				{
					HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).lightLayer = m_LightlayersMask;
				}
				if (linkShadowLayers)
				{
					legacyLight.renderingLayerMask = LightLayerToRenderingLayerMask((int)m_LightlayersMask, legacyLight.renderingLayerMask);
				}
			}
		}

		public bool linkShadowLayers
		{
			get
			{
				return m_LinkShadowLayers;
			}
			set
			{
				m_LinkShadowLayers = value;
			}
		}

		public float shadowNearPlane
		{
			get
			{
				return m_ShadowNearPlane;
			}
			set
			{
				if (m_ShadowNearPlane != value)
				{
					m_ShadowNearPlane = Mathf.Clamp(value, 0f, HDShadowUtils.k_MaxShadowNearPlane);
				}
			}
		}

		public int blockerSampleCount
		{
			get
			{
				return m_BlockerSampleCount;
			}
			set
			{
				if (m_BlockerSampleCount != value)
				{
					m_BlockerSampleCount = Mathf.Clamp(value, 1, 64);
				}
			}
		}

		public int filterSampleCount
		{
			get
			{
				return m_FilterSampleCount;
			}
			set
			{
				if (m_FilterSampleCount != value)
				{
					m_FilterSampleCount = Mathf.Clamp(value, 1, 64);
				}
			}
		}

		public float minFilterSize
		{
			get
			{
				return m_MinFilterSize;
			}
			set
			{
				if (m_MinFilterSize != value)
				{
					m_MinFilterSize = Mathf.Clamp(value, 0f, 1f);
				}
			}
		}

		public int kernelSize
		{
			get
			{
				return m_KernelSize;
			}
			set
			{
				if (m_KernelSize != value)
				{
					m_KernelSize = Mathf.Clamp(value, 1, 32);
				}
			}
		}

		public float lightAngle
		{
			get
			{
				return m_LightAngle;
			}
			set
			{
				if (m_LightAngle != value)
				{
					m_LightAngle = Mathf.Clamp(value, 0f, 9f);
				}
			}
		}

		public float maxDepthBias
		{
			get
			{
				return m_MaxDepthBias;
			}
			set
			{
				if (m_MaxDepthBias != value)
				{
					m_MaxDepthBias = Mathf.Clamp(value, 0.0001f, 0.01f);
				}
			}
		}

		public float range
		{
			get
			{
				return legacyLight.range;
			}
			set
			{
				legacyLight.range = value;
			}
		}

		public Color color
		{
			get
			{
				return legacyLight.color;
			}
			set
			{
				legacyLight.color = value;
				UpdateAreaLightEmissiveMesh();
			}
		}

		public IntScalableSettingValue shadowResolution => m_ShadowResolution;

		public float shadowDimmer
		{
			get
			{
				return m_ShadowDimmer;
			}
			set
			{
				if (m_ShadowDimmer != value)
				{
					m_ShadowDimmer = Mathf.Clamp01(value);
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).shadowDimmer = m_ShadowDimmer;
					}
				}
			}
		}

		public float volumetricShadowDimmer
		{
			get
			{
				if (!useVolumetric)
				{
					return 0f;
				}
				return m_VolumetricShadowDimmer;
			}
			set
			{
				if (m_VolumetricShadowDimmer != value)
				{
					m_VolumetricShadowDimmer = Mathf.Clamp01(value);
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).volumetricShadowDimmer = m_VolumetricShadowDimmer;
					}
				}
			}
		}

		public float shadowFadeDistance
		{
			get
			{
				return m_ShadowFadeDistance;
			}
			set
			{
				if (m_ShadowFadeDistance != value)
				{
					m_ShadowFadeDistance = Mathf.Clamp(value, 0f, float.MaxValue);
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).shadowFadeDistance = m_ShadowFadeDistance;
					}
				}
			}
		}

		public BoolScalableSettingValue useContactShadow => m_UseContactShadow;

		public bool rayTraceContactShadow
		{
			get
			{
				return m_RayTracedContactShadow;
			}
			set
			{
				if (m_RayTracedContactShadow != value)
				{
					m_RayTracedContactShadow = value;
				}
			}
		}

		public Color shadowTint
		{
			get
			{
				return m_ShadowTint;
			}
			set
			{
				if (!(m_ShadowTint == value))
				{
					m_ShadowTint = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).shadowTint = m_ShadowTint;
					}
				}
			}
		}

		public bool penumbraTint
		{
			get
			{
				return m_PenumbraTint;
			}
			set
			{
				if (m_PenumbraTint != value)
				{
					m_PenumbraTint = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).penumbraTint = m_PenumbraTint;
					}
				}
			}
		}

		public float normalBias
		{
			get
			{
				return m_NormalBias;
			}
			set
			{
				if (m_NormalBias != value)
				{
					m_NormalBias = value;
				}
			}
		}

		public float slopeBias
		{
			get
			{
				return m_SlopeBias;
			}
			set
			{
				if (m_SlopeBias != value)
				{
					m_SlopeBias = value;
				}
			}
		}

		public ShadowUpdateMode shadowUpdateMode
		{
			get
			{
				return m_ShadowUpdateMode;
			}
			set
			{
				if (m_ShadowUpdateMode == value)
				{
					return;
				}
				if (m_ShadowUpdateMode != 0 && value == ShadowUpdateMode.EveryFrame)
				{
					if (!preserveCachedShadow)
					{
						HDShadowManager.cachedShadowManager.EvictLight(this);
					}
				}
				else if (legacyLight.shadows != 0 && m_ShadowUpdateMode == ShadowUpdateMode.EveryFrame && value != 0 && (shadowUpdateMode != ShadowUpdateMode.OnDemand || onDemandShadowRenderOnPlacement))
				{
					HDShadowManager.cachedShadowManager.RegisterLight(this);
				}
				m_ShadowUpdateMode = value;
			}
		}

		public bool alwaysDrawDynamicShadows
		{
			get
			{
				return m_AlwaysDrawDynamicShadows;
			}
			set
			{
				m_AlwaysDrawDynamicShadows = value;
			}
		}

		public bool updateUponLightMovement
		{
			get
			{
				return m_UpdateShadowOnLightMovement;
			}
			set
			{
				if (m_UpdateShadowOnLightMovement != value)
				{
					if (m_UpdateShadowOnLightMovement)
					{
						HDShadowManager.cachedShadowManager.RegisterTransformToCache(this);
					}
					else
					{
						HDShadowManager.cachedShadowManager.RegisterTransformToCache(this);
					}
					m_UpdateShadowOnLightMovement = value;
				}
			}
		}

		public float cachedShadowTranslationUpdateThreshold
		{
			get
			{
				return m_CachedShadowTranslationThreshold;
			}
			set
			{
				if (m_CachedShadowTranslationThreshold != value)
				{
					m_CachedShadowTranslationThreshold = value;
				}
			}
		}

		public float cachedShadowAngleUpdateThreshold
		{
			get
			{
				return m_CachedShadowAngularThreshold;
			}
			set
			{
				if (m_CachedShadowAngularThreshold != value)
				{
					m_CachedShadowAngularThreshold = value;
				}
			}
		}

		public float barnDoorAngle
		{
			get
			{
				return m_BarnDoorAngle;
			}
			set
			{
				if (m_BarnDoorAngle != value)
				{
					m_BarnDoorAngle = Mathf.Clamp(value, 0f, 90f);
					UpdateAllLightValues();
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).barnDoorAngle = m_BarnDoorAngle;
					}
				}
			}
		}

		public float barnDoorLength
		{
			get
			{
				return m_BarnDoorLength;
			}
			set
			{
				if (m_BarnDoorLength != value)
				{
					m_BarnDoorLength = Mathf.Clamp(value, 0f, float.MaxValue);
					UpdateAllLightValues();
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).barnDoorLength = m_BarnDoorLength;
					}
				}
			}
		}

		public bool preserveCachedShadow
		{
			get
			{
				return m_preserveCachedShadow;
			}
			set
			{
				if (m_preserveCachedShadow != value)
				{
					m_preserveCachedShadow = value;
				}
			}
		}

		public bool onDemandShadowRenderOnPlacement
		{
			get
			{
				return m_OnDemandShadowRenderOnPlacement;
			}
			set
			{
				if (m_OnDemandShadowRenderOnPlacement != value)
				{
					m_OnDemandShadowRenderOnPlacement = value;
				}
			}
		}

		public bool affectsVolumetric
		{
			get
			{
				return useVolumetric;
			}
			set
			{
				useVolumetric = value;
				if (lightEntity.valid)
				{
					HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).affectVolumetric = useVolumetric;
				}
			}
		}

		internal float[] shadowCascadeRatios
		{
			get
			{
				return m_ShadowCascadeRatios;
			}
			set
			{
				m_ShadowCascadeRatios = value;
			}
		}

		internal float[] shadowCascadeBorders
		{
			get
			{
				return m_ShadowCascadeBorders;
			}
			set
			{
				m_ShadowCascadeBorders = value;
			}
		}

		internal int shadowAlgorithm
		{
			get
			{
				return m_ShadowAlgorithm;
			}
			set
			{
				m_ShadowAlgorithm = value;
			}
		}

		internal int shadowVariant
		{
			get
			{
				return m_ShadowVariant;
			}
			set
			{
				m_ShadowVariant = value;
			}
		}

		internal int shadowPrecision
		{
			get
			{
				return m_ShadowPrecision;
			}
			set
			{
				m_ShadowPrecision = value;
			}
		}

		internal Light legacyLight
		{
			get
			{
				if (m_Light == null)
				{
					TryGetComponent<Light>(out m_Light);
				}
				return m_Light;
			}
		}

		[field: ExcludeCopy]
		internal MeshRenderer emissiveMeshRenderer { get; private set; }

		public ShadowCastingMode areaLightEmissiveMeshShadowCastingMode
		{
			get
			{
				return m_AreaLightEmissiveMeshShadowCastingMode;
			}
			set
			{
				if (m_AreaLightEmissiveMeshShadowCastingMode != value)
				{
					m_AreaLightEmissiveMeshShadowCastingMode = value;
					if (emissiveMeshRenderer != null && !emissiveMeshRenderer.Equals(null))
					{
						emissiveMeshRenderer.shadowCastingMode = m_AreaLightEmissiveMeshShadowCastingMode;
					}
				}
			}
		}

		public MotionVectorGenerationMode areaLightEmissiveMeshMotionVectorGenerationMode
		{
			get
			{
				return m_AreaLightEmissiveMeshMotionVectorGenerationMode;
			}
			set
			{
				if (m_AreaLightEmissiveMeshMotionVectorGenerationMode != value)
				{
					m_AreaLightEmissiveMeshMotionVectorGenerationMode = value;
					if (emissiveMeshRenderer != null && !emissiveMeshRenderer.Equals(null))
					{
						emissiveMeshRenderer.motionVectorGenerationMode = m_AreaLightEmissiveMeshMotionVectorGenerationMode;
					}
				}
			}
		}

		public int areaLightEmissiveMeshLayer
		{
			get
			{
				return m_AreaLightEmissiveMeshLayer;
			}
			set
			{
				if (m_AreaLightEmissiveMeshLayer != value)
				{
					m_AreaLightEmissiveMeshLayer = value;
					if (emissiveMeshRenderer != null && !emissiveMeshRenderer.Equals(null) && m_AreaLightEmissiveMeshLayer != -1)
					{
						emissiveMeshRenderer.gameObject.layer = m_AreaLightEmissiveMeshLayer;
					}
				}
			}
		}

		internal bool useColorTemperature
		{
			get
			{
				return legacyLight.useColorTemperature;
			}
			set
			{
				if (legacyLight.useColorTemperature != value)
				{
					legacyLight.useColorTemperature = value;
				}
			}
		}

		private ShadowMapType shadowMapType
		{
			get
			{
				if (type != HDLightType.Area || areaLightShape != 0)
				{
					if (type == HDLightType.Directional)
					{
						return ShadowMapType.CascadedDirectional;
					}
					return ShadowMapType.PunctualAtlas;
				}
				return ShadowMapType.AreaLightAtlas;
			}
		}

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

		public HDLightType type
		{
			get
			{
				return ComputeLightType(legacyLight);
			}
			set
			{
				if (type == value)
				{
					return;
				}
				if (m_ShadowUpdateMode != 0)
				{
					HDShadowManager.cachedShadowManager.EvictLight(this);
				}
				switch (value)
				{
				case HDLightType.Directional:
					legacyLight.type = LightType.Directional;
					m_PointlightHDType = PointLightHDType.Punctual;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).pointLightType = m_PointlightHDType;
					}
					break;
				case HDLightType.Spot:
					legacyLight.type = LightType.Spot;
					m_PointlightHDType = PointLightHDType.Punctual;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).pointLightType = m_PointlightHDType;
					}
					break;
				case HDLightType.Point:
					legacyLight.type = LightType.Point;
					m_PointlightHDType = PointLightHDType.Punctual;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).pointLightType = m_PointlightHDType;
					}
					break;
				case HDLightType.Area:
					ResolveAreaShape();
					break;
				}
				if (legacyLight.shadows != 0 && m_ShadowUpdateMode != 0)
				{
					HDShadowManager.cachedShadowManager.RegisterLight(this);
				}
				LightUnit[] supportedLightUnits = GetSupportedLightUnits(value, m_SpotLightShape);
				if (!supportedLightUnits.Any((LightUnit u) => u == lightUnit))
				{
					lightUnit = supportedLightUnits.First();
				}
				UpdateAllLightValues();
			}
		}

		public SpotLightShape spotLightShape
		{
			get
			{
				return m_SpotLightShape;
			}
			set
			{
				if (m_SpotLightShape != value)
				{
					m_SpotLightShape = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).spotLightShape = m_SpotLightShape;
					}
					LightUnit[] supportedLightUnits = GetSupportedLightUnits(type, value);
					if (!supportedLightUnits.Any((LightUnit u) => u == lightUnit))
					{
						lightUnit = supportedLightUnits.First();
					}
					UpdateAllLightValues();
				}
			}
		}

		public AreaLightShape areaLightShape
		{
			get
			{
				return m_AreaLightShape;
			}
			set
			{
				if (m_AreaLightShape != value)
				{
					m_AreaLightShape = value;
					if (lightEntity.valid)
					{
						HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).areaLightShape = m_AreaLightShape;
					}
					if (type == HDLightType.Area)
					{
						ResolveAreaShape();
					}
					UpdateAllLightValues();
				}
			}
		}

		public uint GetLightLayers()
		{
			int num = (int)lightlayersMask;
			if (num >= 0)
			{
				return (uint)num;
			}
			return 255u;
		}

		public uint GetShadowLayers()
		{
			int num = RenderingLayerMaskToLightLayer(legacyLight.renderingLayerMask);
			if (num >= 0)
			{
				return (uint)num;
			}
			return 255u;
		}

		private void CreateChildEmissiveMeshViewerIfNeeded()
		{
			bool flag = m_ChildEmissiveMeshViewer != null && !m_ChildEmissiveMeshViewer.Equals(null);
			if (!flag)
			{
				foreach (Transform item in base.transform)
				{
					item.GetComponents(typeof(Component));
					if (item.name == "EmissiveMeshViewer" && item.hideFlags == (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild) && item.GetComponents(typeof(MeshFilter)).Length == 1 && item.GetComponents(typeof(MeshRenderer)).Length == 1 && item.GetComponents(typeof(Component)).Length == 3)
					{
						m_ChildEmissiveMeshViewer = item.gameObject;
						m_ChildEmissiveMeshViewer.transform.localPosition = Vector3.zero;
						m_ChildEmissiveMeshViewer.transform.localRotation = Quaternion.identity;
						m_ChildEmissiveMeshViewer.transform.localScale = Vector3.one;
						m_ChildEmissiveMeshViewer.layer = ((areaLightEmissiveMeshLayer == -1) ? base.gameObject.layer : areaLightEmissiveMeshLayer);
						m_EmissiveMeshFilter = m_ChildEmissiveMeshViewer.GetComponent<MeshFilter>();
						emissiveMeshRenderer = m_ChildEmissiveMeshViewer.GetComponent<MeshRenderer>();
						emissiveMeshRenderer.shadowCastingMode = m_AreaLightEmissiveMeshShadowCastingMode;
						emissiveMeshRenderer.motionVectorGenerationMode = m_AreaLightEmissiveMeshMotionVectorGenerationMode;
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				m_ChildEmissiveMeshViewer = new GameObject("EmissiveMeshViewer", typeof(MeshFilter), typeof(MeshRenderer));
				m_ChildEmissiveMeshViewer.hideFlags = HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild;
				m_ChildEmissiveMeshViewer.transform.SetParent(base.transform);
				m_ChildEmissiveMeshViewer.transform.localPosition = Vector3.zero;
				m_ChildEmissiveMeshViewer.transform.localRotation = Quaternion.identity;
				m_ChildEmissiveMeshViewer.transform.localScale = Vector3.one;
				m_ChildEmissiveMeshViewer.layer = ((areaLightEmissiveMeshLayer == -1) ? base.gameObject.layer : areaLightEmissiveMeshLayer);
				m_EmissiveMeshFilter = m_ChildEmissiveMeshViewer.GetComponent<MeshFilter>();
				emissiveMeshRenderer = m_ChildEmissiveMeshViewer.GetComponent<MeshRenderer>();
				emissiveMeshRenderer.shadowCastingMode = m_AreaLightEmissiveMeshShadowCastingMode;
				emissiveMeshRenderer.motionVectorGenerationMode = m_AreaLightEmissiveMeshMotionVectorGenerationMode;
			}
		}

		private void DestroyChildEmissiveMeshViewer()
		{
			m_EmissiveMeshFilter = null;
			emissiveMeshRenderer.enabled = false;
			emissiveMeshRenderer = null;
			CoreUtils.Destroy(m_ChildEmissiveMeshViewer);
			m_ChildEmissiveMeshViewer = null;
		}

		private void OnDestroy()
		{
			if (lightIdxForCachedShadows >= 0)
			{
				HDShadowManager.cachedShadowManager.EvictLight(this);
			}
			DestroyHDLightRenderEntity();
		}

		internal void DestroyHDLightRenderEntity()
		{
			if (lightEntity.valid)
			{
				HDLightRenderDatabase.instance.DestroyEntity(lightEntity);
				lightEntity = HDLightRenderEntity.Invalid;
			}
		}

		private void OnDisable()
		{
			if (!preserveCachedShadow && lightIdxForCachedShadows >= 0)
			{
				HDShadowManager.cachedShadowManager.EvictLight(this);
			}
			SetEmissiveMeshRendererEnabled(enabled: false);
			s_overlappingHDLights.Remove(this);
			DestroyHDLightRenderEntity();
		}

		private void SetEmissiveMeshRendererEnabled(bool enabled)
		{
			if (displayAreaLightEmissiveMesh && (bool)emissiveMeshRenderer)
			{
				emissiveMeshRenderer.enabled = enabled;
			}
		}

		private int GetShadowRequestCount(HDShadowSettings shadowSettings, HDLightType lightType)
		{
			return lightType switch
			{
				HDLightType.Directional => shadowSettings.cascadeShadowSplitCount.value, 
				HDLightType.Point => 6, 
				_ => 1, 
			};
		}

		public void RequestShadowMapRendering()
		{
			if (shadowUpdateMode == ShadowUpdateMode.OnDemand)
			{
				HDShadowManager.cachedShadowManager.ScheduleShadowUpdate(this);
			}
		}

		public void RequestSubShadowMapRendering(int shadowIndex)
		{
			if (shadowUpdateMode == ShadowUpdateMode.OnDemand)
			{
				HDShadowManager.cachedShadowManager.ScheduleShadowUpdate(this, shadowIndex);
			}
		}

		internal bool ShadowIsUpdatedEveryFrame()
		{
			return shadowUpdateMode == ShadowUpdateMode.EveryFrame;
		}

		internal ShadowMapUpdateType GetShadowUpdateType(HDLightType lightType)
		{
			if (ShadowIsUpdatedEveryFrame())
			{
				return ShadowMapUpdateType.Dynamic;
			}
			if (m_AlwaysDrawDynamicShadows)
			{
				if (lightType != HDLightType.Directional)
				{
					return ShadowMapUpdateType.Mixed;
				}
				if (HDCachedShadowManager.instance.DirectionalHasCachedAtlas())
				{
					return ShadowMapUpdateType.Mixed;
				}
			}
			return ShadowMapUpdateType.Cached;
		}

		internal int GetResolutionFromSettings(ShadowMapType shadowMapType, HDShadowInitParameters initParameters)
		{
			return shadowMapType switch
			{
				ShadowMapType.CascadedDirectional => Math.Min(m_ShadowResolution.Value(initParameters.shadowResolutionDirectional), initParameters.maxDirectionalShadowMapResolution), 
				ShadowMapType.PunctualAtlas => Math.Min(m_ShadowResolution.Value(initParameters.shadowResolutionPunctual), initParameters.maxPunctualShadowMapResolution), 
				ShadowMapType.AreaLightAtlas => Math.Min(m_ShadowResolution.Value(initParameters.shadowResolutionArea), initParameters.maxAreaShadowMapResolution), 
				_ => 0, 
			};
		}

		internal int GetResolutionFromSettings(HDLightType lightType, HDShadowInitParameters initParameters)
		{
			return GetResolutionFromSettings(GetShadowMapType(lightType), initParameters);
		}

		internal void ReserveShadowMap(Camera camera, HDShadowManager shadowManager, HDShadowSettings shadowSettings, in HDShadowInitParameters initParameters, in VisibleLight visibleLight, HDLightType lightType)
		{
			if (shadowRequests == null || m_ShadowRequestIndices == null || m_CachedViewPositions == null)
			{
				shadowRequests = new HDShadowRequest[6];
				m_ShadowRequestIndices = new int[6];
				m_CachedViewPositions = new Vector3[6];
				for (int i = 0; i < 6; i++)
				{
					shadowRequests[i] = new HDShadowRequest();
				}
			}
			ShadowMapType shadowMapType = GetShadowMapType(lightType);
			int resolutionFromSettings = GetResolutionFromSettings(shadowMapType, initParameters);
			Vector2 vector = new Vector2(resolutionFromSettings, resolutionFromSettings);
			int num = 0 | ((shadowMapType == ShadowMapType.PunctualAtlas && initParameters.punctualLightShadowAtlas.useDynamicViewportRescale) ? 1 : 0) | ((shadowMapType == ShadowMapType.AreaLightAtlas && initParameters.areaLightShadowAtlas.useDynamicViewportRescale) ? 1 : 0);
			bool flag = !ShadowIsUpdatedEveryFrame();
			if (num != 0 && !flag)
			{
				float f = Mathf.Clamp01(Vector3.Distance(camera.transform.position, visibleLight.GetPosition()) / shadowSettings.maxShadowDistance.value);
				f = 1f - Mathf.Pow(f, 2f);
				float b = Mathf.Clamp01(visibleLight.range / Vector3.Distance(camera.transform.position, visibleLight.GetPosition()));
				float num2 = Mathf.Max(f, b);
				num2 = (float)Mathf.RoundToInt(num2 * 64f) / 64f;
				vector = Vector2.Lerp(16f * Vector2.one, vector, num2);
			}
			vector = Vector2.Max(vector, new Vector2(16f, 16f));
			if (lightType == HDLightType.Directional)
			{
				shadowManager.UpdateDirectionalShadowResolution((int)vector.x, shadowSettings.cascadeShadowSplitCount.value);
			}
			int shadowRequestCount = GetShadowRequestCount(shadowSettings, lightType);
			ShadowMapUpdateType shadowUpdateType = GetShadowUpdateType(lightType);
			for (int j = 0; j < shadowRequestCount; j++)
			{
				m_ShadowRequestIndices[j] = shadowManager.ReserveShadowResolutions(flag ? new Vector2(resolutionFromSettings, resolutionFromSettings) : vector, this.shadowMapType, GetInstanceID(), j, shadowUpdateType);
			}
		}

		internal static float GetAreaLightOffsetForShadows(Vector2 shapeSize, float coneAngle)
		{
			float num = Mathf.Min(shapeSize.x, shapeSize.y) * 0.5f;
			float num2 = coneAngle * 0.5f;
			float num3 = 1f / Mathf.Tan(num2 * (MathF.PI / 180f));
			return 0f - num * num3;
		}

		private void UpdateDirectionalShadowRequest(HDShadowManager manager, HDShadowSettings shadowSettings, VisibleLight visibleLight, CullingResults cullResults, Vector2 viewportSize, int requestIndex, int lightIndex, Vector3 cameraPos, HDShadowRequest shadowRequest, out Matrix4x4 invViewProjection)
		{
			float shadowNearPlaneOffset = QualitySettings.shadowNearPlaneOffset;
			HDShadowUtils.ExtractDirectionalLightData(visibleLight, viewportSize, (uint)requestIndex, shadowSettings.cascadeShadowSplitCount.value, shadowSettings.cascadeShadowSplits, shadowNearPlaneOffset, cullResults, lightIndex, out shadowRequest.view, out invViewProjection, out shadowRequest.projection, out shadowRequest.deviceProjection, out shadowRequest.deviceProjectionYFlip, out shadowRequest.splitData);
			Vector4 cullingSphere = shadowRequest.splitData.cullingSphere;
			if (ShaderConfig.s_CameraRelativeRendering != 0)
			{
				cullingSphere.x -= cameraPos.x;
				cullingSphere.y -= cameraPos.y;
				cullingSphere.z -= cameraPos.z;
			}
			manager.UpdateCascade(requestIndex, cullingSphere, shadowSettings.cascadeShadowBorders[requestIndex]);
		}

		internal void UpdateShadowRequestData(HDCamera hdCamera, HDShadowManager manager, HDShadowSettings shadowSettings, VisibleLight visibleLight, CullingResults cullResults, int lightIndex, LightingDebugSettings lightingDebugSettings, HDShadowFilteringQuality filteringQuality, HDAreaShadowFilteringQuality areaFilteringQuality, Vector2 viewportSize, HDLightType lightType, int shadowIndex, ref HDShadowRequest shadowRequest)
		{
			Matrix4x4 invViewProjection = Matrix4x4.identity;
			Vector3 worldSpaceCameraPos = hdCamera.mainViewConstants.worldSpaceCameraPos;
			float forwardOffset = 0f;
			switch (lightType)
			{
			case HDLightType.Point:
				HDShadowUtils.ExtractPointLightData(visibleLight, viewportSize, shadowNearPlane, normalBias, (uint)shadowIndex, filteringQuality, out shadowRequest.view, out invViewProjection, out shadowRequest.projection, out shadowRequest.deviceProjection, out shadowRequest.deviceProjectionYFlip, out shadowRequest.splitData);
				shadowRequest.projectionType = BatchCullingProjectionType.Perspective;
				break;
			case HDLightType.Spot:
			{
				float spotAngle = (useCustomSpotLightShadowCone ? Math.Min(customSpotLightShadowCone, visibleLight.light.spotAngle) : visibleLight.light.spotAngle);
				HDShadowUtils.ExtractSpotLightData(spotLightShape, spotAngle, shadowNearPlane, aspectRatio, shapeWidth, shapeHeight, visibleLight, viewportSize, normalBias, filteringQuality, out shadowRequest.view, out invViewProjection, out shadowRequest.projection, out shadowRequest.deviceProjection, out shadowRequest.deviceProjectionYFlip, out shadowRequest.splitData);
				shadowRequest.projectionType = ((spotLightShape != SpotLightShape.Box) ? BatchCullingProjectionType.Perspective : BatchCullingProjectionType.Orthographic);
				if (CustomViewCallbackEvent != null)
				{
					shadowRequest.view = CustomViewCallbackEvent(visibleLight.localToWorldMatrix);
				}
				break;
			}
			case HDLightType.Directional:
				UpdateDirectionalShadowRequest(manager, shadowSettings, visibleLight, cullResults, viewportSize, shadowIndex, lightIndex, worldSpaceCameraPos, shadowRequest, out invViewProjection);
				shadowRequest.projectionType = BatchCullingProjectionType.Orthographic;
				break;
			case HDLightType.Area:
				switch (areaLightShape)
				{
				case AreaLightShape.Rectangle:
				{
					Vector2 shapeSize = new Vector2(shapeWidth, m_ShapeHeight);
					forwardOffset = GetAreaLightOffsetForShadows(shapeSize, areaLightShadowCone);
					HDShadowUtils.ExtractRectangleAreaLightData(visibleLight, forwardOffset, areaLightShadowCone, shadowNearPlane, shapeSize, viewportSize, normalBias, areaFilteringQuality, out shadowRequest.view, out invViewProjection, out shadowRequest.projection, out shadowRequest.deviceProjection, out shadowRequest.deviceProjectionYFlip, out shadowRequest.splitData);
					shadowRequest.projectionType = BatchCullingProjectionType.Perspective;
					break;
				}
				}
				break;
			}
			SetCommonShadowRequestSettings(shadowRequest, visibleLight, forwardOffset, worldSpaceCameraPos, invViewProjection, viewportSize, lightIndex, lightType, filteringQuality, areaFilteringQuality);
		}

		internal int UpdateShadowRequest(HDCamera hdCamera, HDShadowManager manager, HDShadowSettings shadowSettings, VisibleLight visibleLight, CullingResults cullResults, int lightIndex, LightingDebugSettings lightingDebugSettings, HDShadowFilteringQuality filteringQuality, HDAreaShadowFilteringQuality areaFilteringQuality, out int shadowRequestCount)
		{
			int num = -1;
			Vector3 worldSpaceCameraPos = hdCamera.mainViewConstants.worldSpaceCameraPos;
			shadowRequestCount = 0;
			HDLightType hDLightType = type;
			int shadowRequestCount2 = GetShadowRequestCount(shadowSettings, hDLightType);
			ShadowMapUpdateType shadowUpdateType = GetShadowUpdateType(hDLightType);
			bool flag = !ShadowIsUpdatedEveryFrame();
			bool flag2 = shadowUpdateType == ShadowMapUpdateType.Cached;
			bool flag3 = false;
			bool flag4 = true;
			if (flag)
			{
				flag4 = !HDShadowManager.cachedShadowManager.LightIsPendingPlacement(this, shadowMapType) && lightIdxForCachedShadows != -1;
				flag3 = HDShadowManager.cachedShadowManager.NeedRenderingDueToTransformChange(this, hDLightType);
			}
			for (int i = 0; i < shadowRequestCount2; i++)
			{
				HDShadowRequest shadowRequest = shadowRequests[i];
				Matrix4x4 invViewProjection = Matrix4x4.identity;
				int num2 = m_ShadowRequestIndices[i];
				HDShadowResolutionRequest request = manager.GetResolutionRequest(num2);
				if (request == null)
				{
					continue;
				}
				int shadowIdx = lightIdxForCachedShadows + i;
				bool flag5 = false;
				bool flag6 = !flag2;
				bool flag7 = false;
				if (flag && flag4)
				{
					flag5 = flag3 || HDShadowManager.cachedShadowManager.ShadowIsPendingUpdate(shadowIdx, shadowMapType);
					HDShadowManager.cachedShadowManager.UpdateResolutionRequest(ref request, shadowIdx, shadowMapType);
				}
				shadowRequest.isInCachedAtlas = flag2;
				shadowRequest.isMixedCached = shadowUpdateType == ShadowMapUpdateType.Mixed;
				shadowRequest.shouldUseCachedShadowData = false;
				Vector2 resolution = request.resolution;
				if (num2 == -1)
				{
					continue;
				}
				shadowRequest.dynamicAtlasViewport = request.dynamicAtlasViewport;
				shadowRequest.cachedAtlasViewport = request.cachedAtlasViewport;
				if (flag5)
				{
					m_CachedViewPositions[i] = worldSpaceCameraPos;
					shadowRequest.cachedShadowData.cacheTranslationDelta = new Vector3(0f, 0f, 0f);
					UpdateShadowRequestData(hdCamera, manager, shadowSettings, visibleLight, cullResults, lightIndex, lightingDebugSettings, filteringQuality, areaFilteringQuality, resolution, hDLightType, i, ref shadowRequest);
					flag7 = true;
					shadowRequest.shouldUseCachedShadowData = false;
					shadowRequest.shouldRenderCachedComponent = true;
				}
				else if (flag)
				{
					shadowRequest.cachedShadowData.cacheTranslationDelta = worldSpaceCameraPos - m_CachedViewPositions[i];
					shadowRequest.shouldUseCachedShadowData = true;
					shadowRequest.shouldRenderCachedComponent = false;
					if (hDLightType == HDLightType.Directional)
					{
						Matrix4x4 view = shadowRequest.view;
						Matrix4x4 deviceProjectionYFlip = shadowRequest.deviceProjectionYFlip;
						_ = shadowRequest.slopeBias;
						UpdateDirectionalShadowRequest(manager, shadowSettings, visibleLight, cullResults, resolution, i, lightIndex, worldSpaceCameraPos, shadowRequest, out invViewProjection);
						shadowRequest.view = view;
						shadowRequest.deviceProjectionYFlip = deviceProjectionYFlip;
					}
				}
				if (!(hDLightType == HDLightType.Directional && flag) && flag6 && !flag7)
				{
					shadowRequest.shouldUseCachedShadowData = false;
					shadowRequest.cachedShadowData.cacheTranslationDelta = new Vector3(0f, 0f, 0f);
					UpdateShadowRequestData(hdCamera, manager, shadowSettings, visibleLight, cullResults, lightIndex, lightingDebugSettings, filteringQuality, areaFilteringQuality, resolution, hDLightType, i, ref shadowRequest);
				}
				manager.UpdateShadowRequest(num2, shadowRequest, shadowUpdateType);
				if (flag5 && (hDLightType != HDLightType.Directional || hdCamera.camera.cameraType != CameraType.Reflection))
				{
					HDShadowManager.cachedShadowManager.MarkShadowAsRendered(shadowIdx, shadowMapType);
				}
				if (num == -1)
				{
					num = num2;
				}
				shadowRequestCount++;
			}
			if (!flag4)
			{
				return -1;
			}
			return num;
		}

		private void SetCommonShadowRequestSettings(HDShadowRequest shadowRequest, VisibleLight visibleLight, float forwardOffset, Vector3 cameraPos, Matrix4x4 invViewProjection, Vector2 viewportSize, int lightIndex, HDLightType lightType, HDShadowFilteringQuality filteringQuality, HDAreaShadowFilteringQuality areaFilteringQuality)
		{
			float num = legacyLight.range;
			float num2 = ((lightType == HDLightType.Area || (lightType == HDLightType.Spot && spotLightShape == SpotLightShape.Box)) ? shadowNearPlane : Mathf.Max(shadowNearPlane, HDShadowUtils.k_MinShadowNearPlane));
			shadowRequest.zBufferParam = new Vector4((num - num2) / num2, 1f, (num - num2) / (num2 * num), 1f / num);
			shadowRequest.worldTexelSize = 2f / shadowRequest.deviceProjectionYFlip.m00 / viewportSize.x * Mathf.Sqrt(2f);
			shadowRequest.normalBias = normalBias;
			if (ShaderConfig.s_CameraRelativeRendering != 0)
			{
				CoreMatrixUtils.MatrixTimesTranslation(ref shadowRequest.view, cameraPos);
				CoreMatrixUtils.TranslationTimesMatrix(ref invViewProjection, -cameraPos);
			}
			bool orthoCentered = false;
			if (lightType == HDLightType.Directional || (lightType == HDLightType.Spot && spotLightShape == SpotLightShape.Box))
			{
				orthoCentered = true;
				shadowRequest.position = new Vector3(shadowRequest.view.m03, shadowRequest.view.m13, shadowRequest.view.m23);
			}
			else
			{
				VisibleLightExtensionMethods.VisibleLightAxisAndPosition axisAndPosition = visibleLight.GetAxisAndPosition();
				shadowRequest.position = axisAndPosition.Position + axisAndPosition.Forward * forwardOffset;
				if (ShaderConfig.s_CameraRelativeRendering != 0)
				{
					shadowRequest.position -= cameraPos;
				}
			}
			shadowRequest.shadowToWorld = invViewProjection.transpose;
			shadowRequest.zClip = lightType != HDLightType.Directional;
			shadowRequest.lightIndex = lightIndex;
			if (lightType == HDLightType.Directional)
			{
				shadowRequest.shadowMapType = ShadowMapType.CascadedDirectional;
			}
			else if (lightType == HDLightType.Area && areaLightShape == AreaLightShape.Rectangle)
			{
				shadowRequest.shadowMapType = ShadowMapType.AreaLightAtlas;
			}
			else
			{
				shadowRequest.shadowMapType = ShadowMapType.PunctualAtlas;
			}
			GeometryUtility.CalculateFrustumPlanes(CoreMatrixUtils.MultiplyProjectionMatrix(shadowRequest.projection, shadowRequest.view, orthoCentered), m_ShadowFrustumPlanes);
			Vector4[] frustumPlanes = shadowRequest.frustumPlanes;
			if (frustumPlanes == null || frustumPlanes.Length != 6)
			{
				shadowRequest.frustumPlanes = new Vector4[6];
			}
			for (int i = 0; i < 6; i++)
			{
				shadowRequest.frustumPlanes[i] = new Vector4(m_ShadowFrustumPlanes[i].normal.x, m_ShadowFrustumPlanes[i].normal.y, m_ShadowFrustumPlanes[i].normal.z, m_ShadowFrustumPlanes[i].distance);
			}
			float num3 = 0f;
			if (lightType == HDLightType.Directional)
			{
				Matrix4x4 deviceProjection = shadowRequest.deviceProjection;
				float num4 = Vector4.Dot(new Vector4(deviceProjection.m32, 0f - deviceProjection.m32, 0f - deviceProjection.m22, deviceProjection.m22), new Vector4(deviceProjection.m22, deviceProjection.m32, deviceProjection.m23, deviceProjection.m33)) / (deviceProjection.m22 * (deviceProjection.m22 - deviceProjection.m32));
				num3 = Mathf.Abs(Mathf.Tan(MathF.PI / 360f * (softnessScale * m_AngularDiameter) / 2f) * num4 / (2f * shadowRequest.splitData.cullingSphere.w));
				float x = Mathf.Abs(2f * (1f / deviceProjection.m22)) / 100f;
				shadowRequest.zBufferParam.x = x;
			}
			else
			{
				float num5 = m_ShapeRadius * softnessScale;
				float num6 = num5 * num5;
				num3 = 0.02403461f + 3.452916f * num5 - 1.362672f * num6 + 0.6700115f * num6 * num5 + 0.2159474f * num6 * num6;
				num3 /= 100f;
			}
			float num7 = (shadowRequest.isInCachedAtlas ? shadowRequest.cachedAtlasViewport.width : shadowRequest.dynamicAtlasViewport.width);
			num3 *= num7 / 512f;
			float num8 = 5f;
			if (((lightType != HDLightType.Area && filteringQuality == HDShadowFilteringQuality.High) || (lightType == HDLightType.Area && areaFilteringQuality == HDAreaShadowFilteringQuality.High)) && num3 > 0.01f)
			{
				float b = 18f;
				num8 = Mathf.Lerp(num8, b, Mathf.Min(1f, num3 * 100f / 5f));
			}
			shadowRequest.slopeBias = HDShadowUtils.GetSlopeBias(num8, slopeBias);
			shadowRequest.shadowSoftness = num3;
			shadowRequest.blockerSampleCount = blockerSampleCount;
			shadowRequest.filterSampleCount = filterSampleCount;
			shadowRequest.minFilterSize = minFilterSize * 0.001f;
			shadowRequest.kernelSize = (uint)kernelSize;
			shadowRequest.lightAngle = lightAngle * MathF.PI / 180f;
			shadowRequest.maxDepthBias = maxDepthBias;
			shadowRequest.evsmParams.x = evsmExponent * 1.442695f;
			shadowRequest.evsmParams.y = evsmLightLeakBias;
			shadowRequest.evsmParams.z = m_EvsmVarianceBias;
			shadowRequest.evsmParams.w = evsmBlurPasses;
		}

		private void Start()
		{
			m_Animated = GetComponent<Animator>() != null;
		}

		private void LateUpdate()
		{
			if (HDRenderPipeline.currentPipeline != null && m_Animated)
			{
				if (areaLightEmissiveMeshLayer == -1 && m_ChildEmissiveMeshViewer != null && !m_ChildEmissiveMeshViewer.Equals(null) && m_ChildEmissiveMeshViewer.gameObject.layer != base.gameObject.layer)
				{
					m_ChildEmissiveMeshViewer.gameObject.layer = base.gameObject.layer;
				}
				if (needRefreshEmissiveMeshesFromTimeLineUpdate)
				{
					needRefreshEmissiveMeshesFromTimeLineUpdate = false;
					UpdateAreaLightEmissiveMesh();
				}
				new Vector3(shapeWidth, m_ShapeHeight, shapeRadius);
				if (legacyLight.enabled != timelineWorkaround.lightEnabled)
				{
					SetEmissiveMeshRendererEnabled(legacyLight.enabled);
					timelineWorkaround.lightEnabled = legacyLight.enabled;
				}
				if (timelineWorkaround.oldLossyScale != base.transform.lossyScale || intensity != timelineWorkaround.oldIntensity || legacyLight.colorTemperature != timelineWorkaround.oldLightColorTemperature)
				{
					UpdateLightIntensity();
					UpdateAreaLightEmissiveMesh();
					timelineWorkaround.oldLossyScale = base.transform.lossyScale;
					timelineWorkaround.oldIntensity = intensity;
					timelineWorkaround.oldLightColorTemperature = legacyLight.colorTemperature;
				}
				if (type == HDLightType.Spot && timelineWorkaround.oldSpotAngle != legacyLight.spotAngle)
				{
					UpdateLightIntensity();
					timelineWorkaround.oldSpotAngle = legacyLight.spotAngle;
				}
				if (legacyLight.color != timelineWorkaround.oldLightColor || timelineWorkaround.oldLossyScale != base.transform.lossyScale || displayAreaLightEmissiveMesh != timelineWorkaround.oldDisplayAreaLightEmissiveMesh || legacyLight.colorTemperature != timelineWorkaround.oldLightColorTemperature)
				{
					UpdateAreaLightEmissiveMesh();
					timelineWorkaround.oldLightColor = legacyLight.color;
					timelineWorkaround.oldLossyScale = base.transform.lossyScale;
					timelineWorkaround.oldDisplayAreaLightEmissiveMesh = displayAreaLightEmissiveMesh;
					timelineWorkaround.oldLightColorTemperature = legacyLight.colorTemperature;
				}
			}
		}

		private void OnDidApplyAnimationProperties()
		{
			UpdateAllLightValues(fromTimeLine: true);
			UpdateRenderEntity();
		}

		public void CopyTo(HDAdditionalLightData data)
		{
			data.m_Intensity = m_Intensity;
			data.m_EnableSpotReflector = m_EnableSpotReflector;
			data.m_LuxAtDistance = m_LuxAtDistance;
			data.m_InnerSpotPercent = m_InnerSpotPercent;
			data.m_SpotIESCutoffPercent = m_SpotIESCutoffPercent;
			data.m_LightDimmer = m_LightDimmer;
			data.m_VolumetricDimmer = m_VolumetricDimmer;
			data.m_LightUnit = m_LightUnit;
			data.m_FadeDistance = m_FadeDistance;
			data.m_VolumetricFadeDistance = m_VolumetricFadeDistance;
			data.m_AffectDiffuse = m_AffectDiffuse;
			data.m_AffectSpecular = m_AffectSpecular;
			data.m_NonLightmappedOnly = m_NonLightmappedOnly;
			data.m_PointlightHDType = m_PointlightHDType;
			data.m_SpotLightShape = m_SpotLightShape;
			data.m_AreaLightShape = m_AreaLightShape;
			data.m_ShapeWidth = m_ShapeWidth;
			data.m_ShapeHeight = m_ShapeHeight;
			data.m_AspectRatio = m_AspectRatio;
			data.m_ShapeRadius = m_ShapeRadius;
			data.m_SoftnessScale = m_SoftnessScale;
			data.m_UseCustomSpotLightShadowCone = m_UseCustomSpotLightShadowCone;
			data.m_CustomSpotLightShadowCone = m_CustomSpotLightShadowCone;
			data.m_MaxSmoothness = m_MaxSmoothness;
			data.m_ApplyRangeAttenuation = m_ApplyRangeAttenuation;
			data.m_DisplayAreaLightEmissiveMesh = m_DisplayAreaLightEmissiveMesh;
			data.m_AreaLightCookie = m_AreaLightCookie;
			data.m_IESPoint = m_IESPoint;
			data.m_IESSpot = m_IESSpot;
			data.m_IncludeForRayTracing = m_IncludeForRayTracing;
			data.m_AreaLightShadowCone = m_AreaLightShadowCone;
			data.m_UseScreenSpaceShadows = m_UseScreenSpaceShadows;
			data.m_InteractsWithSky = m_InteractsWithSky;
			data.m_AngularDiameter = m_AngularDiameter;
			data.m_FlareSize = m_FlareSize;
			data.m_FlareTint = m_FlareTint;
			data.m_FlareFalloff = m_FlareFalloff;
			data.m_SurfaceTexture = m_SurfaceTexture;
			data.m_SurfaceTint = m_SurfaceTint;
			data.m_Distance = m_Distance;
			data.m_UseRayTracedShadows = m_UseRayTracedShadows;
			data.m_NumRayTracingSamples = m_NumRayTracingSamples;
			data.m_FilterTracedShadow = m_FilterTracedShadow;
			data.m_FilterSizeTraced = m_FilterSizeTraced;
			data.m_SunLightConeAngle = m_SunLightConeAngle;
			data.m_LightShadowRadius = m_LightShadowRadius;
			data.m_SemiTransparentShadow = m_SemiTransparentShadow;
			data.m_ColorShadow = m_ColorShadow;
			data.m_DistanceBasedFiltering = m_DistanceBasedFiltering;
			data.m_EvsmExponent = m_EvsmExponent;
			data.m_EvsmLightLeakBias = m_EvsmLightLeakBias;
			data.m_EvsmVarianceBias = m_EvsmVarianceBias;
			data.m_EvsmBlurPasses = m_EvsmBlurPasses;
			data.m_LightlayersMask = m_LightlayersMask;
			data.m_LinkShadowLayers = m_LinkShadowLayers;
			data.m_ShadowNearPlane = m_ShadowNearPlane;
			data.m_BlockerSampleCount = m_BlockerSampleCount;
			data.m_FilterSampleCount = m_FilterSampleCount;
			data.m_MinFilterSize = m_MinFilterSize;
			data.m_KernelSize = m_KernelSize;
			data.m_LightAngle = m_LightAngle;
			data.m_MaxDepthBias = m_MaxDepthBias;
			m_ShadowResolution.CopyTo(data.m_ShadowResolution);
			data.m_ShadowDimmer = m_ShadowDimmer;
			data.m_VolumetricShadowDimmer = m_VolumetricShadowDimmer;
			data.m_ShadowFadeDistance = m_ShadowFadeDistance;
			m_UseContactShadow.CopyTo(data.m_UseContactShadow);
			data.m_RayTracedContactShadow = m_RayTracedContactShadow;
			data.m_ShadowTint = m_ShadowTint;
			data.m_PenumbraTint = m_PenumbraTint;
			data.m_NormalBias = m_NormalBias;
			data.m_SlopeBias = m_SlopeBias;
			data.m_ShadowUpdateMode = m_ShadowUpdateMode;
			data.m_AlwaysDrawDynamicShadows = m_AlwaysDrawDynamicShadows;
			data.m_UpdateShadowOnLightMovement = m_UpdateShadowOnLightMovement;
			data.m_CachedShadowTranslationThreshold = m_CachedShadowTranslationThreshold;
			data.m_CachedShadowAngularThreshold = m_CachedShadowAngularThreshold;
			data.m_BarnDoorLength = m_BarnDoorLength;
			data.m_BarnDoorAngle = m_BarnDoorAngle;
			data.m_preserveCachedShadow = m_preserveCachedShadow;
			data.m_OnDemandShadowRenderOnPlacement = m_OnDemandShadowRenderOnPlacement;
			data.forceRenderOnPlacement = forceRenderOnPlacement;
			data.m_ShadowCascadeRatios = new float[m_ShadowCascadeRatios.Length];
			m_ShadowCascadeRatios.CopyTo(data.m_ShadowCascadeRatios, 0);
			data.m_ShadowCascadeBorders = new float[m_ShadowCascadeBorders.Length];
			m_ShadowCascadeBorders.CopyTo(data.m_ShadowCascadeBorders, 0);
			data.m_ShadowAlgorithm = m_ShadowAlgorithm;
			data.m_ShadowVariant = m_ShadowVariant;
			data.m_ShadowPrecision = m_ShadowPrecision;
			data.useOldInspector = useOldInspector;
			data.useVolumetric = useVolumetric;
			data.featuresFoldout = featuresFoldout;
			data.m_AreaLightEmissiveMeshShadowCastingMode = m_AreaLightEmissiveMeshShadowCastingMode;
			data.m_AreaLightEmissiveMeshMotionVectorGenerationMode = m_AreaLightEmissiveMeshMotionVectorGenerationMode;
			data.m_AreaLightEmissiveMeshLayer = m_AreaLightEmissiveMeshLayer;
			data.UpdateAllLightValues();
			data.UpdateRenderEntity();
		}

		public static void InitDefaultHDAdditionalLightData(HDAdditionalLightData lightData)
		{
			Light component = lightData.gameObject.GetComponent<Light>();
			switch (lightData.type)
			{
			case HDLightType.Directional:
				lightData.lightUnit = LightUnit.Lux;
				lightData.intensity = 100000f;
				break;
			case HDLightType.Area:
				switch (lightData.areaLightShape)
				{
				case AreaLightShape.Rectangle:
					lightData.lightUnit = LightUnit.Lumen;
					lightData.intensity = 200f;
					lightData.shadowNearPlane = 0f;
					component.shadows = LightShadows.None;
					break;
				}
				break;
			case HDLightType.Spot:
			case HDLightType.Point:
				lightData.lightUnit = LightUnit.Lumen;
				lightData.intensity = 600f;
				break;
			}
			component.lightShadowCasterMode = LightShadowCasterMode.Everything;
			lightData.normalBias = 0.75f;
			lightData.slopeBias = 0.5f;
			lightData.useColorTemperature = true;
		}

		private void OnValidate()
		{
			UpdateBounds();
			RefreshCachedShadow();
			shapeWidth = Mathf.Max(shapeWidth, 0.01f);
			shapeHeight = Mathf.Max(shapeHeight, 0.01f);
			shapeRadius = Mathf.Max(shapeRadius, 0f);
		}

		private void SetLightIntensityPunctual(float intensity)
		{
			switch (type)
			{
			case HDLightType.Directional:
				legacyLight.intensity = intensity;
				break;
			case HDLightType.Point:
				if (lightUnit == LightUnit.Candela)
				{
					legacyLight.intensity = intensity;
				}
				else
				{
					legacyLight.intensity = LightUtils.ConvertPointLightLumenToCandela(intensity);
				}
				break;
			case HDLightType.Spot:
				if (lightUnit == LightUnit.Candela)
				{
					legacyLight.intensity = intensity;
				}
				else if (enableSpotReflector)
				{
					if (spotLightShape == SpotLightShape.Cone)
					{
						legacyLight.intensity = LightUtils.ConvertSpotLightLumenToCandela(intensity, legacyLight.spotAngle * (MathF.PI / 180f), exact: true);
					}
					else if (spotLightShape == SpotLightShape.Pyramid)
					{
						LightUtils.CalculateAnglesForPyramid(aspectRatio, legacyLight.spotAngle * (MathF.PI / 180f), out var angleA, out var angleB);
						legacyLight.intensity = LightUtils.ConvertFrustrumLightLumenToCandela(intensity, angleA, angleB);
					}
					else
					{
						legacyLight.intensity = LightUtils.ConvertPointLightLumenToCandela(intensity);
					}
				}
				else
				{
					legacyLight.intensity = LightUtils.ConvertPointLightLumenToCandela(intensity);
				}
				break;
			}
		}

		private void UpdateLightIntensity()
		{
			if (lightUnit == LightUnit.Lumen)
			{
				if (m_PointlightHDType == PointLightHDType.Punctual)
				{
					SetLightIntensityPunctual(intensity);
				}
				else
				{
					legacyLight.intensity = LightUtils.ConvertAreaLightLumenToLuminance(areaLightShape, intensity, shapeWidth, m_ShapeHeight);
				}
				return;
			}
			if (lightUnit == LightUnit.Ev100)
			{
				legacyLight.intensity = LightUtils.ConvertEvToLuminance(m_Intensity);
				return;
			}
			HDLightType hDLightType = type;
			if ((hDLightType == HDLightType.Spot || hDLightType == HDLightType.Point) && lightUnit == LightUnit.Lux)
			{
				if (hDLightType == HDLightType.Spot && spotLightShape == SpotLightShape.Box)
				{
					legacyLight.intensity = m_Intensity;
				}
				else
				{
					legacyLight.intensity = LightUtils.ConvertLuxToCandela(m_Intensity, luxAtDistance);
				}
			}
			else
			{
				legacyLight.intensity = m_Intensity;
			}
		}

		private void Awake()
		{
			Migrate();
			UpdateAreaLightEmissiveMesh();
		}

		internal void UpdateAreaLightEmissiveMesh(bool fromTimeLine = false)
		{
			bool num = type == HDLightType.Area;
			bool flag = num && displayAreaLightEmissiveMesh;
			if (!num || !flag)
			{
				if ((bool)m_ChildEmissiveMeshViewer)
				{
					if (fromTimeLine)
					{
						emissiveMeshRenderer.enabled = false;
						needRefreshEmissiveMeshesFromTimeLineUpdate = true;
					}
					else
					{
						DestroyChildEmissiveMeshViewer();
					}
				}
				return;
			}
			CreateChildEmissiveMeshViewerIfNeeded();
			if (HDRenderPipelineGlobalSettings.instance != null && !HDRenderPipelineGlobalSettings.instance.Equals(null))
			{
				AreaLightShape areaLightShape = this.areaLightShape;
				if (areaLightShape != 0 && areaLightShape == AreaLightShape.Tube)
				{
					if (m_EmissiveMeshFilter.sharedMesh != HDRenderPipelineGlobalSettings.instance.renderPipelineResources.assets.emissiveCylinderMesh)
					{
						m_EmissiveMeshFilter.sharedMesh = HDRenderPipelineGlobalSettings.instance.renderPipelineResources.assets.emissiveCylinderMesh;
					}
				}
				else if (m_EmissiveMeshFilter.sharedMesh != HDRenderPipelineGlobalSettings.instance.renderPipelineResources.assets.emissiveQuadMesh)
				{
					m_EmissiveMeshFilter.sharedMesh = HDRenderPipelineGlobalSettings.instance.renderPipelineResources.assets.emissiveQuadMesh;
				}
			}
			Vector3 rhs = new Vector3(m_ShapeWidth, m_ShapeHeight, 0f);
			if (this.areaLightShape == AreaLightShape.Tube)
			{
				rhs.y = 0f;
			}
			rhs = Vector3.Max(Vector3.one * 0.01f, rhs);
			switch (this.areaLightShape)
			{
			case AreaLightShape.Rectangle:
				m_ShapeWidth = rhs.x;
				m_ShapeHeight = rhs.y;
				break;
			case AreaLightShape.Tube:
				m_ShapeWidth = rhs.x;
				break;
			}
			if (lightEntity.valid)
			{
				ref HDLightRenderData reference = ref HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity);
				reference.shapeWidth = m_ShapeWidth;
				reference.shapeHeight = m_ShapeHeight;
			}
			Vector3 vector = emissiveMeshRenderer.transform.localRotation * base.transform.lossyScale;
			emissiveMeshRenderer.transform.localScale = new Vector3(rhs.x / vector.x, rhs.y / vector.y, 0.01f / vector.z);
			if (emissiveMeshRenderer.sharedMaterial == null || emissiveMeshRenderer.sharedMaterial.name != base.gameObject.name)
			{
				emissiveMeshRenderer.sharedMaterial = new Material(Shader.Find("HDRP/Unlit"));
				emissiveMeshRenderer.sharedMaterial.SetFloat("_IncludeIndirectLighting", 0f);
				emissiveMeshRenderer.sharedMaterial.name = base.gameObject.name;
			}
			emissiveMeshRenderer.sharedMaterial.SetColor("_UnlitColor", Color.black);
			Color value = legacyLight.color.linear * legacyLight.intensity;
			value *= lightDimmer;
			emissiveMeshRenderer.sharedMaterial.SetColor("_EmissiveColor", value);
			bool state = false;
			if (flag && areaLightCookie != null && areaLightCookie != Texture2D.whiteTexture)
			{
				emissiveMeshRenderer.sharedMaterial.SetTexture("_EmissiveColorMap", areaLightCookie);
				state = true;
			}
			else if (flag && IESSpot != null && IESSpot != Texture2D.whiteTexture)
			{
				emissiveMeshRenderer.sharedMaterial.SetTexture("_EmissiveColorMap", IESSpot);
				state = true;
			}
			else
			{
				emissiveMeshRenderer.sharedMaterial.SetTexture("_EmissiveColorMap", Texture2D.whiteTexture);
			}
			CoreUtils.SetKeyword(emissiveMeshRenderer.sharedMaterial, "_EMISSIVE_COLOR_MAP", state);
			if (m_AreaLightEmissiveMeshLayer != -1)
			{
				emissiveMeshRenderer.gameObject.layer = m_AreaLightEmissiveMeshLayer;
			}
		}

		private void UpdateRectangleLightBounds()
		{
			legacyLight.useShadowMatrixOverride = false;
			legacyLight.useBoundingSphereOverride = true;
			float num = m_ShapeWidth * 0.5f;
			float num2 = m_ShapeHeight * 0.5f;
			float b = Mathf.Sqrt(num * num + num2 * num2);
			legacyLight.boundingSphereOverride = new Vector4(0f, 0f, 0f, Mathf.Max(range, b));
		}

		private void UpdateTubeLightBounds()
		{
			legacyLight.useShadowMatrixOverride = false;
			legacyLight.useBoundingSphereOverride = true;
			legacyLight.boundingSphereOverride = new Vector4(0f, 0f, 0f, Mathf.Max(range, m_ShapeWidth * 0.5f));
		}

		private void UpdateBoxLightBounds()
		{
			legacyLight.useShadowMatrixOverride = true;
			legacyLight.useBoundingSphereOverride = true;
			Matrix4x4 matrix4x = Matrix4x4.Scale(new Vector3(1f, 1f, -1f));
			legacyLight.shadowMatrixOverride = HDShadowUtils.ExtractBoxLightProjectionMatrix(legacyLight.range, shapeWidth, m_ShapeHeight, shadowNearPlane) * matrix4x;
			float magnitude = new Vector3(shapeWidth * 0.5f, m_ShapeHeight * 0.5f, legacyLight.range * 0.5f).magnitude;
			legacyLight.boundingSphereOverride = new Vector4(0f, 0f, legacyLight.range * 0.5f, magnitude);
		}

		private void UpdatePyramidLightBounds()
		{
			legacyLight.useShadowMatrixOverride = true;
			legacyLight.useBoundingSphereOverride = true;
			Matrix4x4 matrix4x = Matrix4x4.Scale(new Vector3(1f, 1f, -1f));
			legacyLight.shadowMatrixOverride = HDShadowUtils.ExtractSpotLightProjectionMatrix(legacyLight.range, legacyLight.spotAngle, shadowNearPlane, aspectRatio, 0f) * matrix4x;
			legacyLight.boundingSphereOverride = new Vector4(0f, 0f, 0f, legacyLight.range);
		}

		private void UpdateBounds()
		{
			switch (type)
			{
			case HDLightType.Spot:
				switch (spotLightShape)
				{
				case SpotLightShape.Box:
					UpdateBoxLightBounds();
					break;
				case SpotLightShape.Pyramid:
					UpdatePyramidLightBounds();
					break;
				default:
					legacyLight.useBoundingSphereOverride = false;
					legacyLight.useShadowMatrixOverride = false;
					break;
				}
				break;
			case HDLightType.Area:
				switch (areaLightShape)
				{
				case AreaLightShape.Rectangle:
					UpdateRectangleLightBounds();
					break;
				case AreaLightShape.Tube:
					UpdateTubeLightBounds();
					break;
				}
				break;
			default:
				legacyLight.useBoundingSphereOverride = false;
				legacyLight.useShadowMatrixOverride = false;
				break;
			}
		}

		private void UpdateShapeSize()
		{
			shapeWidth = m_ShapeWidth;
			shapeHeight = m_ShapeHeight;
		}

		public void UpdateAllLightValues()
		{
			UpdateAllLightValues(fromTimeLine: false);
		}

		internal void UpdateAllLightValues(bool fromTimeLine)
		{
			UpdateShapeSize();
			UpdateLightIntensity();
			UpdateBounds();
			UpdateAreaLightEmissiveMesh(fromTimeLine);
		}

		internal void RefreshCachedShadow()
		{
			if (lightIdxForCachedShadows >= 0)
			{
				HDShadowManager.cachedShadowManager.EvictLight(this);
			}
			if (!ShadowIsUpdatedEveryFrame() && legacyLight.shadows != 0 && (shadowUpdateMode != ShadowUpdateMode.OnDemand || onDemandShadowRenderOnPlacement))
			{
				HDShadowManager.cachedShadowManager.RegisterLight(this);
			}
		}

		public void SetColor(Color color, float colorTemperature = -1f)
		{
			if (colorTemperature != -1f)
			{
				legacyLight.colorTemperature = colorTemperature;
				useColorTemperature = true;
			}
			this.color = color;
		}

		public void EnableColorTemperature(bool enable)
		{
			useColorTemperature = enable;
		}

		public void SetIntensity(float intensity)
		{
			this.intensity = intensity;
		}

		public void SetIntensity(float intensity, LightUnit unit)
		{
			lightUnit = unit;
			this.intensity = intensity;
		}

		public void SetSpotLightLuxAt(float luxIntensity, float distance)
		{
			lightUnit = LightUnit.Lux;
			luxAtDistance = distance;
			intensity = luxIntensity;
		}

		public void SetCookie(Texture cookie, Vector2 directionalLightCookieSize)
		{
			HDLightType hDLightType = type;
			switch (hDLightType)
			{
			case HDLightType.Area:
				if (cookie.dimension != TextureDimension.Tex2D)
				{
					Debug.LogError("Texture dimension " + cookie.dimension.ToString() + " is not supported for area lights.");
				}
				else
				{
					areaLightCookie = cookie;
				}
				return;
			case HDLightType.Point:
				if (cookie.dimension != TextureDimension.Cube)
				{
					Debug.LogError("Texture dimension " + cookie.dimension.ToString() + " is not supported for point lights.");
					return;
				}
				break;
			}
			if ((hDLightType == HDLightType.Directional || hDLightType == HDLightType.Spot) && cookie.dimension != TextureDimension.Tex2D)
			{
				Debug.LogError("Texture dimension " + cookie.dimension.ToString() + " is not supported for Directional/Spot lights.");
				return;
			}
			if (hDLightType == HDLightType.Directional)
			{
				shapeWidth = directionalLightCookieSize.x;
				shapeHeight = directionalLightCookieSize.y;
			}
			legacyLight.cookie = cookie;
		}

		public void SetCookie(Texture cookie)
		{
			SetCookie(cookie, Vector2.zero);
		}

		public void SetSpotAngle(float angle, float innerSpotPercent = 0f)
		{
			legacyLight.spotAngle = angle;
			this.innerSpotPercent = innerSpotPercent;
		}

		public void SetLightDimmer(float dimmer = 1f, float volumetricDimmer = 1f)
		{
			lightDimmer = dimmer;
			this.volumetricDimmer = volumetricDimmer;
		}

		public void SetLightUnit(LightUnit unit)
		{
			lightUnit = unit;
		}

		public void EnableShadows(bool enabled)
		{
			legacyLight.shadows = (enabled ? LightShadows.Soft : LightShadows.None);
		}

		internal bool ShadowsEnabled()
		{
			return legacyLight.shadows != LightShadows.None;
		}

		public void SetShadowResolution(int resolution)
		{
			if (shadowResolution.@override != resolution)
			{
				shadowResolution.@override = resolution;
				RefreshCachedShadow();
			}
		}

		public void SetShadowResolutionLevel(int level)
		{
			if (shadowResolution.level != level)
			{
				shadowResolution.level = level;
				RefreshCachedShadow();
			}
		}

		public void SetShadowResolutionOverride(bool useOverride)
		{
			if (shadowResolution.useOverride != useOverride)
			{
				shadowResolution.useOverride = useOverride;
				RefreshCachedShadow();
			}
		}

		public void SetShadowNearPlane(float nearPlaneDistance)
		{
			shadowNearPlane = nearPlaneDistance;
		}

		public void SetPCSSParams(int blockerSampleCount = 16, int filterSampleCount = 24, float minFilterSize = 0.01f, float radiusScaleForSoftness = 1f)
		{
			this.blockerSampleCount = blockerSampleCount;
			this.filterSampleCount = filterSampleCount;
			this.minFilterSize = minFilterSize;
			softnessScale = radiusScaleForSoftness;
		}

		public void SetLightLayer(LightLayerEnum lightLayerMask, LightLayerEnum shadowLayerMask)
		{
			linkShadowLayers = false;
			legacyLight.renderingLayerMask = LightLayerToRenderingLayerMask((int)shadowLayerMask, legacyLight.renderingLayerMask);
			lightlayersMask = lightLayerMask;
		}

		public void SetShadowDimmer(float shadowDimmer = 1f, float volumetricShadowDimmer = 1f)
		{
			this.shadowDimmer = shadowDimmer;
			this.volumetricShadowDimmer = volumetricShadowDimmer;
		}

		public void SetShadowFadeDistance(float distance)
		{
			shadowFadeDistance = distance;
		}

		public void SetDirectionalShadowTint(Color tint)
		{
			shadowTint = tint;
		}

		public void SetShadowUpdateMode(ShadowUpdateMode updateMode)
		{
			shadowUpdateMode = updateMode;
		}

		public void SetRange(float range)
		{
			legacyLight.range = range;
		}

		public void SetShadowLightLayer(LightLayerEnum shadowLayerMask)
		{
			legacyLight.renderingLayerMask = LightLayerToRenderingLayerMask((int)shadowLayerMask, legacyLight.renderingLayerMask);
		}

		public void SetCullingMask(int cullingMask)
		{
			legacyLight.cullingMask = cullingMask;
		}

		public float[] SetLayerShadowCullDistances(float[] layerShadowCullDistances)
		{
			return legacyLight.layerShadowCullDistances = layerShadowCullDistances;
		}

		public LightUnit[] GetSupportedLightUnits()
		{
			return GetSupportedLightUnits(type, m_SpotLightShape);
		}

		public void SetAreaLightSize(Vector2 size)
		{
			if (type == HDLightType.Area)
			{
				m_ShapeWidth = size.x;
				m_ShapeHeight = size.y;
				if (lightEntity.valid)
				{
					ref HDLightRenderData reference = ref HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity);
					reference.shapeWidth = m_ShapeWidth;
					reference.shapeHeight = m_ShapeHeight;
				}
				UpdateAllLightValues();
			}
		}

		public void SetBoxSpotSize(Vector2 size)
		{
			if (type == HDLightType.Spot)
			{
				shapeWidth = size.x;
				shapeHeight = size.y;
			}
		}

		internal static int LightLayerToRenderingLayerMask(int lightLayer, int renderingLayerMask)
		{
			byte b = (byte)lightLayer;
			return (renderingLayerMask & -256) | b;
		}

		internal static int RenderingLayerMaskToLightLayer(int renderingLayerMask)
		{
			return (byte)renderingLayerMask;
		}

		internal void UpdateRenderEntity()
		{
			HDLightRenderDatabase instance = HDLightRenderDatabase.instance;
			if (instance.IsValid(lightEntity))
			{
				ref HDLightRenderData reference = ref instance.EditLightDataAsRef(in lightEntity);
				reference.pointLightType = m_PointlightHDType;
				reference.spotLightShape = m_SpotLightShape;
				reference.areaLightShape = m_AreaLightShape;
				reference.lightLayer = m_LightlayersMask;
				reference.fadeDistance = m_FadeDistance;
				reference.distance = m_Distance;
				reference.angularDiameter = m_AngularDiameter;
				reference.volumetricFadeDistance = m_VolumetricFadeDistance;
				reference.includeForRayTracing = m_IncludeForRayTracing;
				reference.useScreenSpaceShadows = m_UseScreenSpaceShadows;
				if (legacyLight.bakingOutput.lightmapBakeType == LightmapBakeType.Mixed)
				{
					reference.useRayTracedShadows = !m_NonLightmappedOnly && m_UseRayTracedShadows;
				}
				else
				{
					reference.useRayTracedShadows = m_UseRayTracedShadows;
				}
				reference.colorShadow = m_ColorShadow;
				reference.lightDimmer = m_LightDimmer;
				reference.volumetricDimmer = m_VolumetricDimmer;
				reference.shadowDimmer = m_ShadowDimmer;
				reference.shadowFadeDistance = m_ShadowFadeDistance;
				reference.volumetricShadowDimmer = m_VolumetricShadowDimmer;
				reference.shapeWidth = m_ShapeWidth;
				reference.shapeHeight = m_ShapeHeight;
				reference.flareSize = m_FlareSize;
				reference.flareFalloff = m_FlareFalloff;
				reference.aspectRatio = m_AspectRatio;
				reference.innerSpotPercent = m_InnerSpotPercent;
				reference.spotIESCutoffPercent = m_SpotIESCutoffPercent;
				reference.shapeRadius = m_ShapeRadius;
				reference.barnDoorLength = m_BarnDoorLength;
				reference.affectVolumetric = useVolumetric;
				reference.affectDiffuse = m_AffectDiffuse;
				reference.affectSpecular = m_AffectSpecular;
				reference.applyRangeAttenuation = m_ApplyRangeAttenuation;
				reference.penumbraTint = m_PenumbraTint;
				reference.interactsWithSky = m_InteractsWithSky;
				reference.surfaceTint = m_SurfaceTint;
				reference.shadowTint = m_ShadowTint;
				reference.flareTint = m_FlareTint;
			}
		}

		internal void CreateHDLightRenderEntity(bool autoDestroy = false)
		{
			if (!lightEntity.valid)
			{
				HDLightRenderDatabase instance = HDLightRenderDatabase.instance;
				lightEntity = instance.CreateEntity(autoDestroy);
				instance.AttachGameObjectData(lightEntity, legacyLight.GetInstanceID(), this, legacyLight.gameObject);
			}
			UpdateRenderEntity();
		}

		private void OnEnable()
		{
			if (!ShadowIsUpdatedEveryFrame() && legacyLight.shadows != 0 && (shadowUpdateMode != ShadowUpdateMode.OnDemand || onDemandShadowRenderOnPlacement))
			{
				HDShadowManager.cachedShadowManager.RegisterLight(this);
			}
			SetEmissiveMeshRendererEnabled(enabled: true);
			CreateHDLightRenderEntity();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (!(m_Light == null) && !m_Light.Equals(null))
			{
				UpdateBounds();
			}
		}

		private void Reset()
		{
			UpdateBounds();
		}

		internal ShadowMapType GetShadowMapType(HDLightType lightType)
		{
			if (lightType != HDLightType.Area || areaLightShape != 0)
			{
				if (lightType == HDLightType.Directional)
				{
					return ShadowMapType.CascadedDirectional;
				}
				return ShadowMapType.PunctualAtlas;
			}
			return ShadowMapType.AreaLightAtlas;
		}

		internal bool IsOverlapping()
		{
			LightBakingOutput bakingOutput = GetComponent<Light>().bakingOutput;
			bool flag = bakingOutput.occlusionMaskChannel != -1;
			if (bakingOutput.mixedLightingMode == MixedLightingMode.Shadowmask || bakingOutput.mixedLightingMode == MixedLightingMode.Subtractive)
			{
				return !flag;
			}
			return false;
		}

		private void Migrate()
		{
			k_HDLightMigrationSteps.Migrate(this);
			OnValidate();
		}

		private void ResolveAreaShape()
		{
			m_PointlightHDType = PointLightHDType.Area;
			if (lightEntity.valid)
			{
				HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).pointLightType = m_PointlightHDType;
			}
			if (areaLightShape == AreaLightShape.Disc)
			{
				legacyLight.type = LightType.Disc;
			}
			else if (areaLightShape != AreaLightShape.Tube)
			{
				legacyLight.type = LightType.Point;
			}
		}

		public void SetLightTypeAndShape(HDLightTypeAndShape typeAndShape)
		{
			switch (typeAndShape)
			{
			case HDLightTypeAndShape.Point:
				type = HDLightType.Point;
				break;
			case HDLightTypeAndShape.Directional:
				type = HDLightType.Directional;
				break;
			case HDLightTypeAndShape.ConeSpot:
				type = HDLightType.Spot;
				spotLightShape = SpotLightShape.Cone;
				break;
			case HDLightTypeAndShape.PyramidSpot:
				type = HDLightType.Spot;
				spotLightShape = SpotLightShape.Pyramid;
				break;
			case HDLightTypeAndShape.BoxSpot:
				type = HDLightType.Spot;
				spotLightShape = SpotLightShape.Box;
				break;
			case HDLightTypeAndShape.RectangleArea:
				type = HDLightType.Area;
				areaLightShape = AreaLightShape.Rectangle;
				break;
			case HDLightTypeAndShape.TubeArea:
				type = HDLightType.Area;
				areaLightShape = AreaLightShape.Tube;
				break;
			case HDLightTypeAndShape.DiscArea:
				type = HDLightType.Area;
				areaLightShape = AreaLightShape.Disc;
				break;
			}
		}

		public HDLightTypeAndShape GetLightTypeAndShape()
		{
			return type switch
			{
				HDLightType.Directional => HDLightTypeAndShape.Directional, 
				HDLightType.Point => HDLightTypeAndShape.Point, 
				HDLightType.Spot => spotLightShape switch
				{
					SpotLightShape.Cone => HDLightTypeAndShape.ConeSpot, 
					SpotLightShape.Box => HDLightTypeAndShape.BoxSpot, 
					SpotLightShape.Pyramid => HDLightTypeAndShape.PyramidSpot, 
					_ => throw new Exception($"Unknown {typeof(SpotLightShape)}: {spotLightShape}"), 
				}, 
				HDLightType.Area => areaLightShape switch
				{
					AreaLightShape.Rectangle => HDLightTypeAndShape.RectangleArea, 
					AreaLightShape.Tube => HDLightTypeAndShape.TubeArea, 
					AreaLightShape.Disc => HDLightTypeAndShape.DiscArea, 
					_ => throw new Exception($"Unknown {typeof(AreaLightShape)}: {areaLightShape}"), 
				}, 
				_ => throw new Exception($"Unknown {typeof(HDLightType)}: {type}"), 
			};
		}

		private string GetLightTypeName()
		{
			if (type == HDLightType.Area)
			{
				return $"{areaLightShape}AreaLight";
			}
			if (legacyLight.type == LightType.Spot)
			{
				return $"{spotLightShape}SpotLight";
			}
			return $"{legacyLight.type}Light";
		}

		public static LightUnit[] GetSupportedLightUnits(HDLightType type, SpotLightShape spotLightShape)
		{
			int num = (int)(type & (HDLightType)255);
			num |= (int)(spotLightShape & (SpotLightShape)255) << 8;
			if (supportedLightTypeCache.TryGetValue(num, out var value))
			{
				return value;
			}
			value = ((type == HDLightType.Area) ? Enum.GetValues(typeof(AreaLightUnit)).Cast<LightUnit>().ToArray() : ((type != HDLightType.Directional && (type != 0 || spotLightShape != SpotLightShape.Box)) ? Enum.GetValues(typeof(PunctualLightUnit)).Cast<LightUnit>().ToArray() : Enum.GetValues(typeof(DirectionalLightUnit)).Cast<LightUnit>().ToArray()));
			supportedLightTypeCache[num] = value;
			return value;
		}

		public static bool IsValidLightUnitForType(HDLightType type, SpotLightShape spotLightShape, LightUnit unit)
		{
			return GetSupportedLightUnits(type, spotLightShape).Any((LightUnit u) => u == unit);
		}

		internal static HDLightType TranslateLightType(LightType lightType, PointLightHDType pointLightType)
		{
			return lightType switch
			{
				LightType.Spot => HDLightType.Spot, 
				LightType.Directional => HDLightType.Directional, 
				LightType.Point => pointLightType switch
				{
					PointLightHDType.Punctual => HDLightType.Point, 
					PointLightHDType.Area => HDLightType.Area, 
					_ => HDLightType.Point, 
				}, 
				LightType.Disc => HDLightType.Area, 
				LightType.Area => HDLightType.Area, 
				_ => HDLightType.Point, 
			};
		}

		internal HDLightType ComputeLightType(Light attachedLight)
		{
			if (attachedLight == null)
			{
				return HDLightType.Point;
			}
			HDLightType result = TranslateLightType(attachedLight.type, m_PointlightHDType);
			if (attachedLight.type == LightType.Area && this != HDUtils.s_DefaultHDAdditionalLightData)
			{
				legacyLight.type = LightType.Point;
				m_PointlightHDType = PointLightHDType.Area;
				if (lightEntity.valid)
				{
					HDLightRenderDatabase.instance.EditLightDataAsRef(in lightEntity).pointLightType = m_PointlightHDType;
				}
				m_AreaLightShape = AreaLightShape.Rectangle;
			}
			return result;
		}
	}
}
