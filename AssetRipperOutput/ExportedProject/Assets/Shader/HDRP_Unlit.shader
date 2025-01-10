Shader "HDRP/Unlit" {
	Properties {
		_UnlitColor ("Color", Vector) = (1,1,1,1)
		_UnlitColorMap ("ColorMap", 2D) = "white" {}
		[HideInInspector] _AlphaRemapMin ("AlphaRemapMin", Float) = 0
		[HideInInspector] _AlphaRemapMax ("AlphaRemapMax", Float) = 1
		[HDR] _EmissiveColor ("EmissiveColor", Vector) = (0,0,0,1)
		_EmissiveColorMap ("EmissiveColorMap", 2D) = "white" {}
		[HideInInspector] _EmissiveColorLDR ("EmissiveColor LDR", Vector) = (0,0,0,1)
		[ToggleUI] _AlbedoAffectEmissive ("Albedo Affect Emissive", Float) = 0
		[HideInInspector] _EmissiveIntensityUnit ("Emissive Mode", Float) = 0
		[ToggleUI] _UseEmissiveIntensity ("Use Emissive Intensity", Float) = 0
		_EmissiveIntensity ("Emissive Intensity", Float) = 1
		_EmissiveExposureWeight ("Emissive Pre Exposure", Range(0, 1)) = 1
		_DistortionVectorMap ("DistortionVectorMap", 2D) = "black" {}
		[ToggleUI] _DistortionEnable ("Enable Distortion", Float) = 0
		[ToggleUI] _DistortionOnly ("Distortion Only", Float) = 0
		[ToggleUI] _DistortionDepthTest ("Distortion Depth Test Enable", Float) = 1
		[Enum(Add, 0, Multiply, 1, Replace, 2)] _DistortionBlendMode ("Distortion Blend Mode", Float) = 0
		[HideInInspector] _DistortionSrcBlend ("Distortion Blend Src", Float) = 0
		[HideInInspector] _DistortionDstBlend ("Distortion Blend Dst", Float) = 0
		[HideInInspector] _DistortionBlurSrcBlend ("Distortion Blur Blend Src", Float) = 0
		[HideInInspector] _DistortionBlurDstBlend ("Distortion Blur Blend Dst", Float) = 0
		[HideInInspector] _DistortionBlurBlendMode ("Distortion Blur Blend Mode", Float) = 0
		_DistortionScale ("Distortion Scale", Float) = 1
		_DistortionVectorScale ("Distortion Vector Scale", Float) = 2
		_DistortionVectorBias ("Distortion Vector Bias", Float) = -1
		_DistortionBlurScale ("Distortion Blur Scale", Float) = 1
		_DistortionBlurRemapMin ("DistortionBlurRemapMin", Float) = 0
		_DistortionBlurRemapMax ("DistortionBlurRemapMax", Float) = 1
		[ToggleUI] _AlphaCutoffEnable ("Alpha Cutoff Enable", Float) = 0
		_AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
		_TransparentSortPriority ("_TransparentSortPriority", Float) = 0
		_SurfaceType ("__surfacetype", Float) = 0
		_BlendMode ("__blendmode", Float) = 0
		[HideInInspector] _SrcBlend ("__src", Float) = 1
		[HideInInspector] _DstBlend ("__dst", Float) = 0
		[HideInInspector] _AlphaSrcBlend ("__alphaSrc", Float) = 1
		[HideInInspector] _AlphaDstBlend ("__alphaDst", Float) = 0
		[ToggleUI] [HideInInspector] _ZWrite ("__zw", Float) = 1
		[ToggleUI] [HideInInspector] _TransparentZWrite ("_TransparentZWrite", Float) = 0
		[HideInInspector] _CullMode ("__cullmode", Float) = 2
		[Enum(UnityEditor.Rendering.HighDefinition.TransparentCullMode)] _TransparentCullMode ("_TransparentCullMode", Float) = 2
		[Enum(UnityEditor.Rendering.HighDefinition.OpaqueCullMode)] _OpaqueCullMode ("_OpaqueCullMode", Float) = 2
		[HideInInspector] _ZTestModeDistortion ("_ZTestModeDistortion", Float) = 8
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTestTransparent ("Transparent ZTest", Float) = 4
		[HideInInspector] _ZTestDepthEqualForOpaque ("_ZTestDepthEqualForOpaque", Float) = 4
		[ToggleUI] _EnableFogOnTransparent ("Enable Fog", Float) = 0
		[ToggleUI] _DoubleSidedEnable ("Double sided enable", Float) = 0
		[Enum(Auto, 0, On, 1, Off, 2)] _DoubleSidedGIMode ("Double sided GI mode", Float) = 0
		[HideInInspector] _StencilRef ("_StencilRef", Float) = 0
		[HideInInspector] _StencilWriteMask ("_StencilWriteMask", Float) = 3
		[HideInInspector] _StencilRefDepth ("_StencilRefDepth", Float) = 0
		[HideInInspector] _StencilWriteMaskDepth ("_StencilWriteMaskDepth", Float) = 8
		[HideInInspector] _StencilRefMV ("_StencilRefMV", Float) = 32
		[HideInInspector] _StencilWriteMaskMV ("_StencilWriteMaskMV", Float) = 32
		[ToggleUI] _AddPrecomputedVelocity ("AddPrecomputedVelocity", Float) = 0
		[HideInInspector] _StencilRefDistortionVec ("_StencilRefDistortionVec", Float) = 2
		[HideInInspector] _StencilWriteMaskDistortionVec ("_StencilWriteMaskDistortionVec", Float) = 2
		_EmissionColor ("Color", Vector) = (1,1,1,1)
		[HideInInspector] _IncludeIndirectLighting ("_IncludeIndirectLighting", Float) = 1
		[HideInInspector] _MainTex ("Albedo", 2D) = "white" {}
		[HideInInspector] _Color ("Color", Vector) = (1,1,1,1)
		[HideInInspector] _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
		[HideInInspector] _UnlitColorMap_MipInfo ("_UnlitColorMap_MipInfo", Vector) = (0,0,0,0)
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
	//CustomEditor "Rendering.HighDefinition.UnlitGUI"
}