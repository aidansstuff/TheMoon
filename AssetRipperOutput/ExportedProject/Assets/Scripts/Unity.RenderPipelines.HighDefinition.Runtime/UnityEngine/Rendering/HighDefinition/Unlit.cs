using UnityEngine.Rendering.HighDefinition.Attributes;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class Unlit : RenderPipelineMaterial
	{
		[GenerateHLSL(PackingRules.Exact, false, false, true, 300, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Unlit\\Unlit.cs")]
		public struct SurfaceData
		{
			[MaterialSharedPropertyMapping(MaterialSharedProperty.Albedo)]
			[SurfaceDataAttributes("Color", false, true, FieldPrecision.Default, false, "")]
			public Vector3 color;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Normal)]
			[SurfaceDataAttributes(new string[] { "Normal", "Normal View Space" }, true, false, FieldPrecision.Default, false, "")]
			public Vector3 normalWS;

			[SurfaceDataAttributes("Shadow Tint", false, true, FieldPrecision.Default, false, "defined(_ENABLE_SHADOW_MATTE) && (SHADERPASS == SHADERPASS_PATH_TRACING)")]
			public Vector4 shadowTint;
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 350, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Unlit\\Unlit.cs")]
		public struct BSDFData
		{
			[SurfaceDataAttributes("", false, true, FieldPrecision.Default, false, "")]
			public Vector3 color;
		}
	}
}
