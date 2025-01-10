Shader "Hidden/HDRP/DeferredTile" {
	Properties {
		[HideInInspector] _StencilMask ("_StencilMask", Float) = 6
		[HideInInspector] _StencilRef ("_StencilRef", Float) = 0
		[HideInInspector] _StencilCmp ("_StencilCmp", Float) = 3
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