using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	public static class HDRenderUtilities
	{
		private static readonly Vector3[] s_GenerateRenderingSettingsFor_Rotations = new Vector3[6]
		{
			new Vector3(0f, 90f, 0f),
			new Vector3(0f, 270f, 0f),
			new Vector3(270f, 0f, 0f),
			new Vector3(90f, 0f, 0f),
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 180f, 0f)
		};

		public static void Render(CameraSettings settings, CameraPositionSettings position, Texture target, uint staticFlags = 0u)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			RenderTexture renderTexture = target as RenderTexture;
			Cubemap cubemap = target as Cubemap;
			switch (target.dimension)
			{
			case TextureDimension.Tex2D:
				if (renderTexture == null)
				{
					throw new ArgumentException("'target' must be a RenderTexture when rendering into a 2D texture");
				}
				break;
			default:
				throw new ArgumentException("Rendering into a target of dimension " + $"{target.dimension} is not supported");
			case TextureDimension.Cube:
				break;
			}
			Camera camera = NewRenderingCamera();
			try
			{
				camera.ApplySettings(settings);
				camera.ApplySettings(position);
				switch (target.dimension)
				{
				case TextureDimension.Tex2D:
					camera.targetTexture = renderTexture;
					camera.Render();
					camera.targetTexture = null;
					target.IncrementUpdateCount();
					break;
				case TextureDimension.Cube:
				{
					bool flag = false;
					if (!flag || staticFlags == 0)
					{
						if (!flag && staticFlags != 0)
						{
							Debug.LogWarning("A static flags bitmask was provided but this is ignored in player builds");
						}
						if (renderTexture != null)
						{
							camera.RenderToCubemap(renderTexture);
						}
						if (cubemap != null)
						{
							camera.RenderToCubemap(cubemap);
						}
					}
					target.IncrementUpdateCount();
					break;
				}
				}
			}
			finally
			{
				CoreUtils.Destroy(camera.gameObject);
			}
		}

		public static void Render(ProbeSettings settings, ProbeCapturePositionSettings position, Texture target, bool forceFlipY = false, bool forceInvertBackfaceCulling = false, uint staticFlags = 0u, float referenceFieldOfView = 90f, float referenceAspect = 1f)
		{
			Render(settings, position, target, out var _, out var _, forceFlipY, forceInvertBackfaceCulling, staticFlags, referenceFieldOfView, referenceAspect);
		}

		public static void GenerateRenderingSettingsFor(ProbeSettings settings, ProbeCapturePositionSettings position, List<CameraSettings> cameras, List<CameraPositionSettings> cameraPositions, List<CubemapFace> cameraCubeFaces, ulong overrideSceneCullingMask, ProbeRenderSteps renderSteps, bool forceFlipY = false, float referenceFieldOfView = 90f, float referenceAspect = 1f)
		{
			ComputeCameraSettingsFromProbeSettings(settings, position, out var cameraSettings, out var cameraPositionSettings, overrideSceneCullingMask, referenceFieldOfView, referenceAspect);
			if (forceFlipY)
			{
				cameraSettings.flipYMode = HDAdditionalCameraData.FlipYMode.ForceFlipY;
			}
			switch (settings.type)
			{
			case ProbeSettings.ProbeType.PlanarProbe:
				cameras.Add(cameraSettings);
				cameraPositions.Add(cameraPositionSettings);
				cameraCubeFaces.Add(CubemapFace.Unknown);
				break;
			case ProbeSettings.ProbeType.ReflectionProbe:
			{
				for (int i = 0; i < 6; i++)
				{
					CubemapFace cubemapFace = (CubemapFace)i;
					if (renderSteps.HasCubeFace(cubemapFace))
					{
						CameraPositionSettings item = cameraPositionSettings;
						item.rotation *= Quaternion.Euler(s_GenerateRenderingSettingsFor_Rotations[i]);
						cameras.Add(cameraSettings);
						cameraPositions.Add(item);
						cameraCubeFaces.Add(cubemapFace);
					}
				}
				break;
			}
			}
		}

		public static void ComputeCameraSettingsFromProbeSettings(ProbeSettings settings, ProbeCapturePositionSettings position, out CameraSettings cameraSettings, out CameraPositionSettings cameraPositionSettings, ulong overrideSceneCullingMask, float referenceFieldOfView = 90f, float referenceAspect = 1f)
		{
			cameraSettings = settings.cameraSettings;
			cameraPositionSettings = CameraPositionSettings.NewDefault();
			ProbeSettingsUtilities.ApplySettings(ref settings, ref position, ref cameraSettings, ref cameraPositionSettings, referenceFieldOfView, referenceAspect);
			cameraSettings.culling.sceneCullingMaskOverride = overrideSceneCullingMask;
		}

		public static void Render(ProbeSettings settings, ProbeCapturePositionSettings position, Texture target, out CameraSettings cameraSettings, out CameraPositionSettings cameraPositionSettings, bool forceFlipY = false, bool forceInvertBackfaceCulling = false, uint staticFlags = 0u, float referenceFieldOfView = 90f, float referenceAspect = 1f)
		{
			ComputeCameraSettingsFromProbeSettings(settings, position, out cameraSettings, out cameraPositionSettings, 0uL, referenceFieldOfView, referenceAspect);
			if (forceFlipY)
			{
				cameraSettings.flipYMode = HDAdditionalCameraData.FlipYMode.ForceFlipY;
			}
			if (forceInvertBackfaceCulling)
			{
				cameraSettings.invertFaceCulling = true;
			}
			Render(cameraSettings, cameraPositionSettings, target, staticFlags);
		}

		[Obsolete("Use CreateReflectionProbeRenderTarget with explicit format instead", true)]
		public static RenderTexture CreateReflectionProbeRenderTarget(int cubemapSize)
		{
			RenderTexture renderTexture = new RenderTexture(cubemapSize, cubemapSize, 1, GraphicsFormat.R16G16B16A16_SFloat);
			renderTexture.dimension = TextureDimension.Cube;
			renderTexture.enableRandomWrite = true;
			renderTexture.useMipMap = true;
			renderTexture.autoGenerateMips = false;
			renderTexture.Create();
			return renderTexture;
		}

		public static RenderTexture CreateReflectionProbeRenderTarget(int cubemapSize, GraphicsFormat format)
		{
			RenderTexture renderTexture = new RenderTexture(cubemapSize, cubemapSize, 1, format);
			renderTexture.dimension = TextureDimension.Cube;
			renderTexture.enableRandomWrite = true;
			renderTexture.useMipMap = true;
			renderTexture.autoGenerateMips = false;
			renderTexture.depth = 0;
			renderTexture.Create();
			return renderTexture;
		}

		public static RenderTexture CreatePlanarProbeRenderTarget(int planarSize, GraphicsFormat format)
		{
			RenderTexture renderTexture = new RenderTexture(planarSize, planarSize, 1, format);
			renderTexture.dimension = TextureDimension.Tex2D;
			renderTexture.enableRandomWrite = true;
			renderTexture.useMipMap = true;
			renderTexture.autoGenerateMips = false;
			renderTexture.depth = 0;
			renderTexture.Create();
			return renderTexture;
		}

		public static RenderTexture CreatePlanarProbeDepthRenderTarget(int planarSize)
		{
			RenderTexture renderTexture = new RenderTexture(planarSize, planarSize, 1, GraphicsFormat.R32_SFloat);
			renderTexture.dimension = TextureDimension.Tex2D;
			renderTexture.enableRandomWrite = true;
			renderTexture.useMipMap = true;
			renderTexture.autoGenerateMips = false;
			renderTexture.Create();
			return renderTexture;
		}

		public static Cubemap CreateReflectionProbeTarget(int cubemapSize)
		{
			return new Cubemap(cubemapSize, GraphicsFormat.R16G16B16A16_SFloat, TextureCreationFlags.None);
		}

		private static Camera NewRenderingCamera()
		{
			GameObject gameObject = new GameObject("__Render Camera");
			Camera camera = gameObject.AddComponent<Camera>();
			camera.cameraType = CameraType.Reflection;
			gameObject.AddComponent<HDAdditionalCameraData>();
			return camera;
		}

		private static void FixSettings(Texture target, ref ProbeSettings settings, ref ProbeCapturePositionSettings position, ref CameraSettings cameraSettings, ref CameraPositionSettings cameraPositionSettings)
		{
			RenderTexture renderTexture = null;
			if ((renderTexture = target as RenderTexture) != null && renderTexture.dimension == TextureDimension.Cube && settings.type == ProbeSettings.ProbeType.ReflectionProbe && SystemInfo.graphicsUVStartsAtTop)
			{
				cameraSettings.flipYMode = HDAdditionalCameraData.FlipYMode.ForceFlipY;
			}
		}
	}
}
