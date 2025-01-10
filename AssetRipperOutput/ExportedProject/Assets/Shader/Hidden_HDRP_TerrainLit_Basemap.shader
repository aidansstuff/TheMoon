Shader "Hidden/HDRP/TerrainLit_Basemap" {
	Properties {
		[HideInInspector] _StencilRef ("_StencilRef", Float) = 0
		[HideInInspector] _StencilWriteMask ("_StencilWriteMask", Float) = 3
		[HideInInspector] _StencilRefGBuffer ("_StencilRefGBuffer", Float) = 2
		[HideInInspector] _StencilWriteMaskGBuffer ("_StencilWriteMaskGBuffer", Float) = 3
		[HideInInspector] _StencilRefDepth ("_StencilRefDepth", Float) = 0
		[HideInInspector] _StencilWriteMaskDepth ("_StencilWriteMaskDepth", Float) = 8
		[HideInInspector] _ZWrite ("__zw", Float) = 1
		[HideInInspector] _CullMode ("__cullmode", Float) = 2
		[HideInInspector] _ZTestDepthEqualForOpaque ("_ZTestDepthEqualForOpaque", Float) = 4
		[HideInInspector] _ZTestGBuffer ("_ZTestGBuffer", Float) = 4
		[HideInInspector] _TerrainHolesTexture ("Holes Map (RGB)", 2D) = "white" {}
		_EmissionColor ("Color", Vector) = (1,1,1,1)
		_MetallicTex ("Metallic (R)", 2D) = "white" {}
		_MainTex ("Albedo", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,1)
		[ToggleUI] _SupportDecals ("Support Decals", Float) = 1
		[ToggleUI] _ReceivesSSR ("Receives SSR", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}