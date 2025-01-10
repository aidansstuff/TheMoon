using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class Decal
	{
		[GenerateHLSL(PackingRules.Exact, false, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Decal\\Decal.cs")]
		public struct DecalSurfaceData
		{
			[SurfaceDataAttributes("Base Color", false, true, FieldPrecision.Default, false, "")]
			public Vector4 baseColor;

			[SurfaceDataAttributes("Normal", true, false, FieldPrecision.Default, false, "")]
			public Vector4 normalWS;

			[SurfaceDataAttributes("Mask", true, false, FieldPrecision.Default, false, "")]
			public Vector4 mask;

			[SurfaceDataAttributes("Emissive", false, false, FieldPrecision.Default, false, "")]
			public Vector3 emissive;

			[SurfaceDataAttributes("AOSBlend", true, false, FieldPrecision.Default, false, "")]
			public Vector2 MAOSBlend;
		}

		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Decal\\Decal.cs")]
		public enum DBufferMaterial
		{
			Count = 4
		}

		private static GraphicsFormat[] m_RTFormat = new GraphicsFormat[4]
		{
			GraphicsFormat.R8G8B8A8_SRGB,
			GraphicsFormat.R8G8B8A8_UNorm,
			GraphicsFormat.R8G8B8A8_UNorm,
			GraphicsFormat.R8G8_UNorm
		};

		private static GraphicsFormat[] m_RTFormatHP = new GraphicsFormat[4]
		{
			GraphicsFormat.R8G8B8A8_SRGB,
			GraphicsFormat.R16G16B16A16_SFloat,
			GraphicsFormat.R8G8B8A8_UNorm,
			GraphicsFormat.R8G8_UNorm
		};

		public static int GetMaterialDBufferCount()
		{
			return 4;
		}

		public static void GetMaterialDBufferDescription(out GraphicsFormat[] RTFormat)
		{
			HDRenderPipeline hDRenderPipeline = RenderPipelineManager.currentPipeline as HDRenderPipeline;
			bool flag = hDRenderPipeline.currentPlatformRenderPipelineSettings.supportSurfaceGradient && hDRenderPipeline.currentPlatformRenderPipelineSettings.decalNormalBufferHP;
			RTFormat = (flag ? m_RTFormatHP : m_RTFormat);
		}
	}
}
