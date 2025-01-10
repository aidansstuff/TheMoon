using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class HDProbeSystem
	{
		private static HDProbeSystemInternal s_Instance;

		public static ReflectionSystemParameters Parameters
		{
			get
			{
				return s_Instance.Parameters;
			}
			set
			{
				s_Instance.Parameters = value;
			}
		}

		public static IEnumerable<HDProbe> realtimeViewDependentProbes => s_Instance.realtimeViewDependentProbes;

		public static IEnumerable<HDProbe> realtimeViewIndependentProbes => s_Instance.realtimeViewIndependentProbes;

		public static IEnumerable<HDProbe> bakedProbes => s_Instance.bakedProbes;

		public static int bakedProbeCount => s_Instance.bakedProbeCount;

		static HDProbeSystem()
		{
			s_Instance = new HDProbeSystemInternal();
			Application.quitting += DisposeStaticInstance;
		}

		private static void DisposeStaticInstance()
		{
			s_Instance.Dispose();
		}

		public static void RegisterProbe(HDProbe probe)
		{
			s_Instance.RegisterProbe(probe);
		}

		public static void UnregisterProbe(HDProbe probe)
		{
			s_Instance.UnregisterProbe(probe);
		}

		public static void Render(HDProbe probe, Transform viewerTransform, Texture outTarget, out HDProbe.RenderData outRenderData, bool forceFlipY = false, float referenceFieldOfView = 90f, float referenceAspect = 1f)
		{
			ProbeCapturePositionSettings position = ProbeCapturePositionSettings.ComputeFrom(probe, viewerTransform);
			HDRenderUtilities.Render(probe.settings, position, outTarget, out var cameraSettings, out var cameraPositionSettings, forceFlipY, forceInvertBackfaceCulling: false, 0u, referenceFieldOfView, referenceAspect);
			outRenderData = new HDProbe.RenderData(cameraSettings, cameraPositionSettings);
		}

		public static void AssignRenderData(HDProbe probe, HDProbe.RenderData renderData, ProbeSettings.Mode targetMode)
		{
			switch (targetMode)
			{
			case ProbeSettings.Mode.Baked:
				probe.bakedRenderData = renderData;
				break;
			case ProbeSettings.Mode.Custom:
				probe.customRenderData = renderData;
				break;
			case ProbeSettings.Mode.Realtime:
				probe.realtimeRenderData = renderData;
				break;
			}
		}

		public static HDProbeCullState PrepareCull(Camera camera)
		{
			return s_Instance.PrepareCull(camera);
		}

		public static void QueryCullResults(HDProbeCullState state, ref HDProbeCullingResults results)
		{
			s_Instance.QueryCullResults(state, ref results);
		}

		public static Texture CreateRenderTargetForMode(HDProbe probe, ProbeSettings.Mode targetMode)
		{
			Texture result = null;
			HDRenderPipeline hDRenderPipeline = (HDRenderPipeline)RenderPipelineManager.currentPipeline;
			ProbeSettings settings = probe.settings;
			switch (targetMode)
			{
			case ProbeSettings.Mode.Realtime:
			{
				GraphicsFormat reflectionProbeFormat = (GraphicsFormat)hDRenderPipeline.currentPlatformRenderPipelineSettings.lightLoopSettings.reflectionProbeFormat;
				switch (settings.type)
				{
				case ProbeSettings.ProbeType.PlanarProbe:
					result = HDRenderUtilities.CreatePlanarProbeRenderTarget((int)probe.resolution, reflectionProbeFormat);
					break;
				case ProbeSettings.ProbeType.ReflectionProbe:
					result = HDRenderUtilities.CreateReflectionProbeRenderTarget((int)probe.cubeResolution, reflectionProbeFormat);
					break;
				}
				break;
			}
			case ProbeSettings.Mode.Baked:
			case ProbeSettings.Mode.Custom:
			{
				GraphicsFormat format = GraphicsFormat.R16G16B16A16_SFloat;
				switch (settings.type)
				{
				case ProbeSettings.ProbeType.PlanarProbe:
					result = HDRenderUtilities.CreatePlanarProbeRenderTarget((int)probe.resolution, format);
					break;
				case ProbeSettings.ProbeType.ReflectionProbe:
					result = HDRenderUtilities.CreateReflectionProbeRenderTarget((int)probe.cubeResolution, format);
					break;
				}
				break;
			}
			}
			return result;
		}

		private static Texture CreateAndSetRenderTargetIfRequired(HDProbe probe, ProbeSettings.Mode targetMode)
		{
			_ = probe.settings;
			Texture texture = probe.GetTexture(targetMode);
			if (texture != null)
			{
				return texture;
			}
			texture = CreateRenderTargetForMode(probe, targetMode);
			probe.SetTexture(targetMode, texture);
			return texture;
		}
	}
}
