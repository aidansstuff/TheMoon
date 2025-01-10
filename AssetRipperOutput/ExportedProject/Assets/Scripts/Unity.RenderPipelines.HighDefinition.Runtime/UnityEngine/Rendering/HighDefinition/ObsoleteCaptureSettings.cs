using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[Obsolete]
	internal class ObsoleteCaptureSettings
	{
		public static ObsoleteCaptureSettings @default = new ObsoleteCaptureSettings();

		public ObsoleteCaptureSettingsOverrides overrides;

		public HDAdditionalCameraData.ClearColorMode clearColorMode;

		[ColorUsage(true, true)]
		public Color backgroundColorHDR = new Color32(6, 18, 48, 0);

		public bool clearDepth = true;

		public LayerMask cullingMask = -1;

		public bool useOcclusionCulling = true;

		public LayerMask volumeLayerMask = 1;

		public Transform volumeAnchorOverride;

		public CameraProjection projection;

		public float nearClipPlane = 0.3f;

		public float farClipPlane = 1000f;

		public float fieldOfView = 90f;

		public float orthographicSize = 5f;

		public int renderingPath;

		public float shadowDistance = 100f;
	}
}
