Shader "HDRP/TerrainLit" {
	Properties {
		[ToggleUI] [HideInInspector] _EnableHeightBlend ("EnableHeightBlend", Float) = 0
		_HeightTransition ("Height Transition", Range(0, 1)) = 0
		[Enum(Off, 0, From Ambient Occlusion, 1)] [HideInInspector] _SpecularOcclusionMode ("Specular Occlusion Mode", Float) = 1
		[HideInInspector] _StencilRef ("_StencilRef", Float) = 0
		[HideInInspector] _StencilWriteMask ("_StencilWriteMask", Float) = 3
		[HideInInspector] _StencilRefGBuffer ("_StencilRefGBuffer", Float) = 2
		[HideInInspector] _StencilWriteMaskGBuffer ("_StencilWriteMaskGBuffer", Float) = 3
		[HideInInspector] _StencilRefDepth ("_StencilRefDepth", Float) = 0
		[HideInInspector] _StencilWriteMaskDepth ("_StencilWriteMaskDepth", Float) = 8
		[HideInInspector] _ZWrite ("__zw", Float) = 1
		[ToggleUI] [HideInInspector] _TransparentZWrite ("_TransparentZWrite", Float) = 0
		[HideInInspector] _CullMode ("__cullmode", Float) = 2
		[HideInInspector] _ZTestDepthEqualForOpaque ("_ZTestDepthEqualForOpaque", Float) = 4
		[HideInInspector] _ZTestGBuffer ("_ZTestGBuffer", Float) = 4
		[ToggleUI] _EnableInstancedPerPixelNormal ("Instanced per pixel normal", Float) = 1
		[HideInInspector] _TerrainHolesTexture ("Holes Map (RGB)", 2D) = "white" {}
		[HideInInspector] _EmissionColor ("Color", Vector) = (1,1,1,1)
		[HideInInspector] _MainTex ("Albedo", 2D) = "white" {}
		[HideInInspector] _Color ("Color", Vector) = (1,1,1,1)
		[ToggleUI] [HideInInspector] _SupportDecals ("Support Decals", Float) = 1
		[ToggleUI] [HideInInspector] _ReceivesSSR ("Receives SSR", Float) = 1
		[ToggleUI] [HideInInspector] _AddPrecomputedVelocity ("AddPrecomputedVelocity", Float) = 0
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
	Fallback "Hidden/HDRP/FallbackError"
	//CustomEditor "UnityEditor.Rendering.HighDefinition.TerrainLitGUI"
}