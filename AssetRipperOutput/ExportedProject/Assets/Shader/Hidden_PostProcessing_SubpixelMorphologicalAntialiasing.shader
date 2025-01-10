Shader "Hidden/PostProcessing/SubpixelMorphologicalAntialiasing" {
	Properties {
		[HideInInspector] _StencilRef ("_StencilRef", Float) = 4
		[HideInInspector] _StencilMask ("_StencilMask", Float) = 4
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}