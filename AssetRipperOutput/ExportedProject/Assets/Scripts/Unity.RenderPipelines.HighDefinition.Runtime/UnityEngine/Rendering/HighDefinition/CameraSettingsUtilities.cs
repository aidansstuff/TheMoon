namespace UnityEngine.Rendering.HighDefinition
{
	public static class CameraSettingsUtilities
	{
		public static void ApplySettings(this Camera cam, CameraSettings settings)
		{
			HDAdditionalCameraData obj = cam.GetComponent<HDAdditionalCameraData>() ?? cam.gameObject.AddComponent<HDAdditionalCameraData>();
			obj.defaultFrameSettings = settings.defaultFrameSettings;
			obj.renderingPathCustomFrameSettings = settings.renderingPathCustomFrameSettings;
			obj.renderingPathCustomFrameSettingsOverrideMask = settings.renderingPathCustomFrameSettingsOverrideMask;
			cam.nearClipPlane = settings.frustum.nearClipPlane;
			cam.farClipPlane = settings.frustum.farClipPlane;
			cam.fieldOfView = settings.frustum.fieldOfView;
			cam.aspect = settings.frustum.aspect;
			cam.projectionMatrix = settings.frustum.GetUsedProjectionMatrix();
			cam.useOcclusionCulling = settings.culling.useOcclusionCulling;
			cam.cullingMask = settings.culling.cullingMask;
			cam.overrideSceneCullingMask = settings.culling.sceneCullingMaskOverride;
			obj.clearColorMode = settings.bufferClearing.clearColorMode;
			obj.backgroundColorHDR = settings.bufferClearing.backgroundColorHDR;
			obj.clearDepth = settings.bufferClearing.clearDepth;
			obj.volumeLayerMask = settings.volumes.layerMask;
			obj.volumeAnchorOverride = settings.volumes.anchorOverride;
			obj.customRenderingSettings = settings.customRenderingSettings;
			obj.flipYMode = settings.flipYMode;
			obj.invertFaceCulling = settings.invertFaceCulling;
			obj.probeCustomFixedExposure = settings.probeRangeCompressionFactor;
		}

		public static void ApplySettings(this Camera cam, CameraPositionSettings settings)
		{
			cam.transform.position = settings.position;
			cam.transform.rotation = settings.rotation;
			cam.worldToCameraMatrix = settings.GetUsedWorldToCameraMatrix();
		}
	}
}
