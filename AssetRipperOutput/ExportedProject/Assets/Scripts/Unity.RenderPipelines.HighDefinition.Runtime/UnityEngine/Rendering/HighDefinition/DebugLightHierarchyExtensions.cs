using System;
using Unity.Burst.CompilerServices;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class DebugLightHierarchyExtensions
	{
		[IgnoreWarning(1370)]
		public static bool IsEnabledFor(this DebugLightFilterMode mode, GPULightType gpuLightType, SpotLightShape spotLightShape)
		{
			switch (gpuLightType)
			{
			case GPULightType.Spot:
			case GPULightType.ProjectorPyramid:
			case GPULightType.ProjectorBox:
				return spotLightShape switch
				{
					SpotLightShape.Box => (mode & DebugLightFilterMode.DirectSpotBox) != 0, 
					SpotLightShape.Cone => (mode & DebugLightFilterMode.DirectSpotCone) != 0, 
					SpotLightShape.Pyramid => (mode & DebugLightFilterMode.DirectSpotPyramid) != 0, 
					_ => throw new ArgumentOutOfRangeException("spotLightShape"), 
				};
			case GPULightType.Tube:
				return (mode & DebugLightFilterMode.DirectTube) != 0;
			case GPULightType.Point:
				return (mode & DebugLightFilterMode.DirectPunctual) != 0;
			case GPULightType.Rectangle:
				return (mode & DebugLightFilterMode.DirectRectangle) != 0;
			case GPULightType.Directional:
				return (mode & DebugLightFilterMode.DirectDirectional) != 0;
			default:
				throw new ArgumentOutOfRangeException("gpuLightType");
			}
		}

		public static bool IsEnabledFor(this DebugLightFilterMode mode, ProbeSettings.ProbeType probeType)
		{
			return probeType switch
			{
				ProbeSettings.ProbeType.PlanarProbe => (mode & DebugLightFilterMode.IndirectPlanarProbe) != 0, 
				ProbeSettings.ProbeType.ReflectionProbe => (mode & DebugLightFilterMode.IndirectReflectionProbe) != 0, 
				_ => throw new ArgumentOutOfRangeException("probeType"), 
			};
		}
	}
}
