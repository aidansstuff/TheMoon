using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public struct CameraSettings
	{
		[Serializable]
		public struct BufferClearing
		{
			[Obsolete("Since 2019.3, use BufferClearing.NewDefault() instead.")]
			public static readonly BufferClearing @default;

			public HDAdditionalCameraData.ClearColorMode clearColorMode;

			[ColorUsage(true, true)]
			public Color backgroundColorHDR;

			public bool clearDepth;

			public static BufferClearing NewDefault()
			{
				BufferClearing result = default(BufferClearing);
				result.clearColorMode = HDAdditionalCameraData.ClearColorMode.Sky;
				result.backgroundColorHDR = new Color32(6, 18, 48, 0);
				result.clearDepth = true;
				return result;
			}
		}

		[Serializable]
		public struct Volumes
		{
			[Obsolete("Since 2019.3, use Volumes.NewDefault() instead.")]
			public static readonly Volumes @default;

			public LayerMask layerMask;

			public Transform anchorOverride;

			public static Volumes NewDefault()
			{
				Volumes result = default(Volumes);
				result.layerMask = -1;
				result.anchorOverride = null;
				return result;
			}
		}

		[Serializable]
		public struct Frustum
		{
			public enum Mode
			{
				ComputeProjectionMatrix = 0,
				UseProjectionMatrixField = 1
			}

			public const float MinNearClipPlane = 1E-05f;

			public const float MinFarClipPlane = 0.0001f;

			[Obsolete("Since 2019.3, use Frustum.NewDefault() instead.")]
			public static readonly Frustum @default;

			public Mode mode;

			public float aspect;

			[FormerlySerializedAs("farClipPlane")]
			public float farClipPlaneRaw;

			[FormerlySerializedAs("nearClipPlane")]
			public float nearClipPlaneRaw;

			[Range(1f, 179f)]
			public float fieldOfView;

			public Matrix4x4 projectionMatrix;

			public float farClipPlane => Mathf.Max(nearClipPlaneRaw + 0.0001f, farClipPlaneRaw);

			public float nearClipPlane => Mathf.Max(1E-05f, nearClipPlaneRaw);

			public static Frustum NewDefault()
			{
				Frustum result = default(Frustum);
				result.mode = Mode.ComputeProjectionMatrix;
				result.aspect = 1f;
				result.farClipPlaneRaw = 1000f;
				result.nearClipPlaneRaw = 0.1f;
				result.fieldOfView = 90f;
				result.projectionMatrix = Matrix4x4.identity;
				return result;
			}

			public Matrix4x4 ComputeProjectionMatrix()
			{
				return Matrix4x4.Perspective(HDUtils.ClampFOV(fieldOfView), aspect, nearClipPlane, farClipPlane);
			}

			public Matrix4x4 GetUsedProjectionMatrix()
			{
				return mode switch
				{
					Mode.ComputeProjectionMatrix => ComputeProjectionMatrix(), 
					Mode.UseProjectionMatrixField => projectionMatrix, 
					_ => throw new ArgumentException(), 
				};
			}
		}

		[Serializable]
		public struct Culling
		{
			[Obsolete("Since 2019.3, use Culling.NewDefault() instead.")]
			public static readonly Culling @default;

			public bool useOcclusionCulling;

			public LayerMask cullingMask;

			public ulong sceneCullingMaskOverride;

			public static Culling NewDefault()
			{
				Culling result = default(Culling);
				result.cullingMask = -1;
				result.useOcclusionCulling = true;
				result.sceneCullingMaskOverride = 0uL;
				return result;
			}
		}

		[Obsolete("Since 2019.3, use CameraSettings.defaultCameraSettingsNonAlloc instead.")]
		public static readonly CameraSettings @default = default(CameraSettings);

		public static readonly CameraSettings defaultCameraSettingsNonAlloc = NewDefault();

		public bool customRenderingSettings;

		public FrameSettings renderingPathCustomFrameSettings;

		public FrameSettingsOverrideMask renderingPathCustomFrameSettingsOverrideMask;

		public BufferClearing bufferClearing;

		public Volumes volumes;

		public Frustum frustum;

		public Culling culling;

		public bool invertFaceCulling;

		public HDAdditionalCameraData.FlipYMode flipYMode;

		public LayerMask probeLayerMask;

		public FrameSettingsRenderType defaultFrameSettings;

		internal float probeRangeCompressionFactor;

		[SerializeField]
		[FormerlySerializedAs("renderingPath")]
		[Obsolete("For data migration")]
		internal int m_ObsoleteRenderingPath;

		[SerializeField]
		[FormerlySerializedAs("frameSettings")]
		[Obsolete("For data migration")]
		internal ObsoleteFrameSettings m_ObsoleteFrameSettings;

		public static CameraSettings NewDefault()
		{
			CameraSettings result = default(CameraSettings);
			result.bufferClearing = BufferClearing.NewDefault();
			result.culling = Culling.NewDefault();
			result.renderingPathCustomFrameSettings = FrameSettings.NewDefaultCamera();
			result.frustum = Frustum.NewDefault();
			result.customRenderingSettings = false;
			result.volumes = Volumes.NewDefault();
			result.flipYMode = HDAdditionalCameraData.FlipYMode.Automatic;
			result.invertFaceCulling = false;
			result.probeLayerMask = -1;
			result.probeRangeCompressionFactor = 1f;
			return result;
		}

		public static CameraSettings From(HDCamera hdCamera)
		{
			CameraSettings result = defaultCameraSettingsNonAlloc;
			result.culling.cullingMask = hdCamera.camera.cullingMask;
			result.culling.useOcclusionCulling = hdCamera.camera.useOcclusionCulling;
			result.culling.sceneCullingMaskOverride = HDUtils.GetSceneCullingMaskFromCamera(hdCamera.camera);
			result.frustum.aspect = hdCamera.camera.aspect;
			result.frustum.farClipPlaneRaw = hdCamera.camera.farClipPlane;
			result.frustum.nearClipPlaneRaw = hdCamera.camera.nearClipPlane;
			result.frustum.fieldOfView = hdCamera.camera.fieldOfView;
			result.frustum.mode = Frustum.Mode.UseProjectionMatrixField;
			result.frustum.projectionMatrix = hdCamera.camera.projectionMatrix;
			result.invertFaceCulling = false;
			if (hdCamera.camera.TryGetComponent<HDAdditionalCameraData>(out var component))
			{
				result.customRenderingSettings = component.customRenderingSettings;
				result.bufferClearing.backgroundColorHDR = component.backgroundColorHDR;
				result.bufferClearing.clearColorMode = component.clearColorMode;
				result.bufferClearing.clearDepth = component.clearDepth;
				result.flipYMode = component.flipYMode;
				result.renderingPathCustomFrameSettings = component.renderingPathCustomFrameSettings;
				result.renderingPathCustomFrameSettingsOverrideMask = component.renderingPathCustomFrameSettingsOverrideMask;
				result.volumes = new Volumes
				{
					anchorOverride = component.volumeAnchorOverride,
					layerMask = component.volumeLayerMask
				};
				result.probeLayerMask = component.probeLayerMask;
				result.invertFaceCulling = component.invertFaceCulling;
			}
			bool num = hdCamera.camera.worldToCameraMatrix.determinant > 0f;
			bool flag = Mathf.Approximately(hdCamera.camera.projectionMatrix.m32, -1f);
			bool flag2 = Mathf.Approximately(hdCamera.camera.projectionMatrix.m00, 1f) && Mathf.Approximately(hdCamera.camera.projectionMatrix.m11, 1f);
			if (num && flag && flag2)
			{
				result.invertFaceCulling = true;
			}
			return result;
		}

		internal Hash128 GetHash()
		{
			Hash128 hash = default(Hash128);
			Hash128 hash2 = default(Hash128);
			HashUtilities.ComputeHash128(ref bufferClearing, ref hash);
			HashUtilities.ComputeHash128(ref culling, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref customRenderingSettings, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref defaultFrameSettings, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref flipYMode, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref frustum, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref invertFaceCulling, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref probeLayerMask, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref probeRangeCompressionFactor, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref renderingPathCustomFrameSettings, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref renderingPathCustomFrameSettingsOverrideMask, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			int hashCode = volumes.GetHashCode();
			hash2 = new Hash128((ulong)hashCode, 0uL);
			HashUtilities.AppendHash(ref hash2, ref hash);
			return hash;
		}
	}
}
