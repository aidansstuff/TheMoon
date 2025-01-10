Shader "Hidden/HDRP/Material/Decal/DecalNormalBuffer" {
	Properties {
		[HideInInspector] _DecalNormalBufferStencilRef ("_DecalNormalBufferStencilRef", Float) = 0
		[HideInInspector] _DecalNormalBufferStencilReadMask ("_DecalNormalBufferStencilReadMask", Float) = 0
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