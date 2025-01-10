using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.HighDefinition.Attributes;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class Builtin
	{
		[GenerateHLSL(PackingRules.Exact, false, false, true, 100, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Builtin\\BuiltinData.cs")]
		public struct BuiltinData
		{
			[MaterialSharedPropertyMapping(MaterialSharedProperty.Alpha)]
			[SurfaceDataAttributes("Opacity", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float opacity;

			[SurfaceDataAttributes("AlphaClipTreshold", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float alphaClipTreshold;

			[SurfaceDataAttributes("Baked Diffuse Lighting", false, true, FieldPrecision.Real, false, "")]
			public Vector3 bakeDiffuseLighting;

			[SurfaceDataAttributes("Back Baked Diffuse Lighting", false, true, FieldPrecision.Real, false, "")]
			public Vector3 backBakeDiffuseLighting;

			[SurfaceDataAttributes("Shadowmask 0", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float shadowMask0;

			[SurfaceDataAttributes("Shadowmask 1", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float shadowMask1;

			[SurfaceDataAttributes("Shadowmask 2", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float shadowMask2;

			[SurfaceDataAttributes("Shadowmask 3", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float shadowMask3;

			[SurfaceDataAttributes("Emissive Color", false, false, FieldPrecision.Real, false, "")]
			public Vector3 emissiveColor;

			[SurfaceDataAttributes("Motion Vector", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public Vector2 motionVector;

			[SurfaceDataAttributes("Distortion", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public Vector2 distortion;

			[SurfaceDataAttributes("Distortion Blur", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float distortionBlur;

			[SurfaceDataAttributes("Is Lightmap", false, false, FieldPrecision.Default, false, "")]
			public uint isLightmap;

			[SurfaceDataAttributes("Rendering Layers", false, false, FieldPrecision.Default, false, "")]
			public uint renderingLayers;

			[SurfaceDataAttributes("Depth Offset", false, false, FieldPrecision.Default, false, "")]
			public float depthOffset;

			[SurfaceDataAttributes("VT Packed Feedback", false, false, FieldPrecision.Real, false, "defined(UNITY_VIRTUAL_TEXTURING)")]
			public Vector4 vtPackedFeedback;
		}

		[GenerateHLSL(PackingRules.Exact, false, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Builtin\\BuiltinData.cs")]
		public struct LightTransportData
		{
			[SurfaceDataAttributes("", false, true, FieldPrecision.Real, false, "")]
			public Vector3 diffuseColor;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public Vector3 emissiveColor;
		}

		public static GraphicsFormat GetLightingBufferFormat()
		{
			return GraphicsFormat.B10G11R11_UFloatPack32;
		}

		public static GraphicsFormat GetShadowMaskBufferFormat()
		{
			return GraphicsFormat.R8G8B8A8_UNorm;
		}

		public static GraphicsFormat GetMotionVectorFormat()
		{
			return GraphicsFormat.R16G16_SFloat;
		}

		public static GraphicsFormat GetDistortionBufferFormat()
		{
			return GraphicsFormat.R16G16B16A16_SFloat;
		}
	}
}
