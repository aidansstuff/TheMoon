using System;

namespace UnityEngine.Rendering.HighDefinition
{
	public static class ProbeSettingsUtilities
	{
		internal enum PositionMode
		{
			UseProbeTransform = 0,
			MirrorReferenceTransformWithProbePlane = 1
		}

		public static void ApplySettings(ref ProbeSettings settings, ref ProbeCapturePositionSettings probePosition, ref CameraSettings cameraSettings, ref CameraPositionSettings cameraPosition, float referenceFieldOfView = 90f, float referenceAspect = 1f)
		{
			cameraSettings = settings.cameraSettings;
			PositionMode positionMode;
			bool flag;
			switch (settings.type)
			{
			case ProbeSettings.ProbeType.PlanarProbe:
				positionMode = PositionMode.MirrorReferenceTransformWithProbePlane;
				flag = true;
				ApplyPlanarFrustumHandling(ref settings, ref probePosition, ref cameraSettings, ref cameraPosition, referenceFieldOfView, referenceAspect);
				break;
			case ProbeSettings.ProbeType.ReflectionProbe:
				positionMode = PositionMode.UseProbeTransform;
				flag = false;
				cameraSettings.frustum.mode = CameraSettings.Frustum.Mode.ComputeProjectionMatrix;
				cameraSettings.frustum.aspect = 1f;
				cameraSettings.frustum.fieldOfView = 90f;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			switch (positionMode)
			{
			case PositionMode.UseProbeTransform:
			{
				cameraPosition.mode = CameraPositionSettings.Mode.ComputeWorldToCameraMatrix;
				Matrix4x4 matrix4x = Matrix4x4.TRS(probePosition.proxyPosition, probePosition.proxyRotation, Vector3.one);
				cameraPosition.position = matrix4x.MultiplyPoint(settings.proxySettings.capturePositionProxySpace);
				cameraPosition.rotation = matrix4x.rotation * settings.proxySettings.captureRotationProxySpace;
				if (settings.type == ProbeSettings.ProbeType.ReflectionProbe)
				{
					cameraPosition.rotation = Quaternion.identity;
				}
				break;
			}
			case PositionMode.MirrorReferenceTransformWithProbePlane:
				cameraPosition.mode = CameraPositionSettings.Mode.UseWorldToCameraMatrixField;
				ApplyMirroredReferenceTransform(ref settings, ref probePosition, ref cameraSettings, ref cameraPosition);
				break;
			}
			if (flag)
			{
				ApplyObliqueNearClipPlane(ref settings, ref probePosition, ref cameraSettings, ref cameraPosition);
			}
			cameraSettings.probeRangeCompressionFactor = settings.lighting.rangeCompressionFactor;
			switch (settings.mode)
			{
			default:
				cameraSettings.defaultFrameSettings = FrameSettingsRenderType.RealtimeReflection;
				break;
			case ProbeSettings.Mode.Baked:
			case ProbeSettings.Mode.Custom:
				cameraSettings.defaultFrameSettings = FrameSettingsRenderType.CustomOrBakedReflection;
				break;
			}
		}

		internal static void ApplyMirroredReferenceTransform(ref ProbeSettings settings, ref ProbeCapturePositionSettings probePosition, ref CameraSettings cameraSettings, ref CameraPositionSettings cameraPosition)
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(probePosition.proxyPosition, probePosition.proxyRotation, Vector3.one);
			Vector3 vector = matrix4x.MultiplyPoint(settings.proxySettings.mirrorPositionProxySpace);
			Vector3 vector2 = matrix4x.MultiplyVector(settings.proxySettings.mirrorRotationProxySpace * Vector3.forward);
			Matrix4x4 matrix4x2 = GeometryUtils.CalculateReflectionMatrix(vector, vector2);
			if (Vector3.Dot(vector2, probePosition.referencePosition - vector) < 0.0001f)
			{
				probePosition.referencePosition += 0.0001f * vector2;
			}
			Matrix4x4 matrix4x3 = GeometryUtils.CalculateWorldToCameraMatrixRHS(probePosition.referencePosition, probePosition.referenceRotation);
			cameraPosition.worldToCameraMatrix = matrix4x3 * matrix4x2;
			cameraSettings.invertFaceCulling = true;
			cameraPosition.position = matrix4x2.MultiplyPoint(probePosition.referencePosition);
			Vector3 forward = matrix4x2.MultiplyVector(probePosition.referenceRotation * Vector3.forward);
			Vector3 upwards = matrix4x2.MultiplyVector(probePosition.referenceRotation * Vector3.up);
			cameraPosition.rotation = Quaternion.LookRotation(forward, upwards);
		}

		internal static void ApplyPlanarFrustumHandling(ref ProbeSettings settings, ref ProbeCapturePositionSettings probePosition, ref CameraSettings cameraSettings, ref CameraPositionSettings cameraPosition, float referenceFieldOfView, float referenceAspect)
		{
			Vector3 lookAtPositionWS = Matrix4x4.TRS(probePosition.proxyPosition, probePosition.proxyRotation, Vector3.one).MultiplyPoint(settings.proxySettings.mirrorPositionProxySpace);
			cameraSettings.frustum.aspect = referenceAspect;
			switch (settings.frustum.fieldOfViewMode)
			{
			case ProbeSettings.Frustum.FOVMode.Fixed:
				cameraSettings.frustum.fieldOfView = settings.frustum.fixedValue;
				break;
			case ProbeSettings.Frustum.FOVMode.Viewer:
				cameraSettings.frustum.fieldOfView = Mathf.Min(referenceFieldOfView * settings.frustum.viewerScale, 170f);
				break;
			case ProbeSettings.Frustum.FOVMode.Automatic:
				cameraSettings.frustum.fieldOfView = Mathf.Min(settings.influence.ComputeFOVAt(probePosition.referencePosition, lookAtPositionWS, probePosition.influenceToWorld) * settings.frustum.automaticScale, 170f);
				break;
			}
		}

		internal static void ApplyObliqueNearClipPlane(ref ProbeSettings settings, ref ProbeCapturePositionSettings probePosition, ref CameraSettings cameraSettings, ref CameraPositionSettings cameraPosition)
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(probePosition.proxyPosition, probePosition.proxyRotation, Vector3.one);
			Vector3 positionWS = matrix4x.MultiplyPoint(settings.proxySettings.mirrorPositionProxySpace);
			Vector3 normalWS = matrix4x.MultiplyVector(settings.proxySettings.mirrorRotationProxySpace * Vector3.forward);
			Vector4 clipPlane = GeometryUtils.CameraSpacePlane(cameraPosition.worldToCameraMatrix, positionWS, normalWS);
			Matrix4x4 projectionMatrix = GeometryUtils.CalculateObliqueMatrix(Matrix4x4.Perspective(HDUtils.ClampFOV(cameraSettings.frustum.fieldOfView), cameraSettings.frustum.aspect, cameraSettings.frustum.nearClipPlane, cameraSettings.frustum.farClipPlane), clipPlane);
			cameraSettings.frustum.mode = CameraSettings.Frustum.Mode.UseProjectionMatrixField;
			cameraSettings.frustum.projectionMatrix = projectionMatrix;
		}
	}
}
