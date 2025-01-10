namespace UnityEngine.Rendering.HighDefinition
{
	public static class HDMaterialProperties
	{
		public const string kZWrite = "_ZWrite";

		public const string kTransparentZWrite = "_TransparentZWrite";

		public const string kTransparentCullMode = "_TransparentCullMode";

		public const string kOpaqueCullMode = "_OpaqueCullMode";

		public const string kZTestTransparent = "_ZTestTransparent";

		public const string kRayTracing = "_RayTracing";

		public const string kSurfaceType = "_SurfaceType";

		public const string kSupportDecals = "_SupportDecals";

		public const string kAlphaCutoffEnabled = "_AlphaCutoffEnable";

		public const string kBlendMode = "_BlendMode";

		public const string kAlphaToMask = "_AlphaToMask";

		public const string kEnableFogOnTransparent = "_EnableFogOnTransparent";

		internal const string kDistortionDepthTest = "_DistortionDepthTest";

		public const string kDistortionEnable = "_DistortionEnable";

		public const string kZTestModeDistortion = "_ZTestModeDistortion";

		public const string kDistortionBlendMode = "_DistortionBlendMode";

		public const string kTransparentWritingMotionVec = "_TransparentWritingMotionVec";

		public const string kEnableBlendModePreserveSpecularLighting = "_EnableBlendModePreserveSpecularLighting";

		public const string kTransparentBackfaceEnable = "_TransparentBackfaceEnable";

		public const string kDoubleSidedEnable = "_DoubleSidedEnable";

		public const string kDoubleSidedNormalMode = "_DoubleSidedNormalMode";

		public const string kDoubleSidedGIMode = "_DoubleSidedGIMode";

		public const string kDistortionOnly = "_DistortionOnly";

		public const string kTransparentDepthPrepassEnable = "_TransparentDepthPrepassEnable";

		public const string kTransparentDepthPostpassEnable = "_TransparentDepthPostpassEnable";

		public const string kTransparentSortPriority = "_TransparentSortPriority";

		public const string kReceivesSSR = "_ReceivesSSR";

		public const string kReceivesSSRTransparent = "_ReceivesSSRTransparent";

		public const string kDepthOffsetEnable = "_DepthOffsetEnable";

		public const string kConservativeDepthOffsetEnable = "_ConservativeDepthOffsetEnable";

		public const string kAffectAlbedo = "_AffectAlbedo";

		public const string kAffectNormal = "_AffectNormal";

		public const string kAffectAO = "_AffectAO";

		public const string kAffectMetal = "_AffectMetal";

		public const string kAffectSmoothness = "_AffectSmoothness";

		public const string kAffectEmission = "_AffectEmission";

		internal const string kStencilRef = "_StencilRef";

		internal const string kStencilWriteMask = "_StencilWriteMask";

		internal const string kStencilRefDepth = "_StencilRefDepth";

		internal const string kStencilWriteMaskDepth = "_StencilWriteMaskDepth";

		internal const string kStencilRefGBuffer = "_StencilRefGBuffer";

		internal const string kStencilWriteMaskGBuffer = "_StencilWriteMaskGBuffer";

		internal const string kStencilRefMV = "_StencilRefMV";

		internal const string kStencilWriteMaskMV = "_StencilWriteMaskMV";

		internal const string kStencilRefDistortionVec = "_StencilRefDistortionVec";

		internal const string kStencilWriteMaskDistortionVec = "_StencilWriteMaskDistortionVec";

		internal const string kDecalStencilWriteMask = "_DecalStencilWriteMask";

		internal const string kDecalStencilRef = "_DecalStencilRef";

		internal const string kEnableGeometricSpecularAA = "_EnableGeometricSpecularAA";

		internal const string kUseSplitLighting = "_RequireSplitLighting";

		internal const string kDecalColorMask0 = "_DecalColorMask0";

		internal const string kDecalColorMask1 = "_DecalColorMask1";

		internal const string kDecalColorMask2 = "_DecalColorMask2";

		internal const string kDecalColorMask3 = "_DecalColorMask3";

		internal const string kEnableDecals = "_SupportDecals";

		internal const int kMaxLayerCount = 4;

		internal const string kLayerCount = "_LayerCount";

		internal const string kUVBase = "_UVBase";

		internal const string kTexWorldScale = "_TexWorldScale";

		internal const string kInvTilingScale = "_InvTilingScale";

		internal const string kUVMappingMask = "_UVMappingMask";

		internal const string kUVDetail = "_UVDetail";

		internal const string kUVDetailsMappingMask = "_UVDetailsMappingMask";

		internal const string kDecalLayerMaskFromDecal = "_DecalLayerMaskFromDecal";

		internal const string kObjectSpaceUVMapping = "_ObjectSpaceUVMapping";

		internal const string kDisplacementMode = "_DisplacementMode";

		internal const string kMaterialID = "_MaterialID";

		internal const string kTransmissionEnable = "_TransmissionEnable";

		internal const string kZTestGBuffer = "_ZTestGBuffer";

		internal const string kZTestDepthEqualForOpaque = "_ZTestDepthEqualForOpaque";

		internal const string kEmissionColor = "_EmissionColor";

		internal const string kEnableSSR = "_ReceivesSSR";

		internal const string kAddPrecomputedVelocity = "_AddPrecomputedVelocity";

		internal const string kShadowMatteFilter = "_ShadowMatteFilter";

		internal const string kTransmittanceColorMap = "_TransmittanceColorMap";

		internal const string kRefractionModel = "_RefractionModel";

		internal const string kSpecularOcclusionMode = "_SpecularOcclusionMode";

		internal const string kCutoff = "_Cutoff";

		internal const string kAlphaCutoff = "_AlphaCutoff";

		internal const string kUseShadowThreshold = "_UseShadowThreshold";

		internal const string kAlphaCutoffShadow = "_AlphaCutoffShadow";

		internal const string kAlphaCutoffPrepass = "_AlphaCutoffPrepass";

		internal const string kAlphaCutoffPostpass = "_AlphaCutoffPostpass";

		internal const string kBaseColor = "_BaseColor";

		internal const string kBaseColorMap = "_BaseColorMap";

		internal const string kMetallic = "_Metallic";

		internal const string kSmoothness = "_Smoothness";

		internal const string kUseEmissiveIntensity = "_UseEmissiveIntensity";

		internal const string kEmissiveExposureWeight = "_EmissiveExposureWeight";

		internal const string kEmissiveIntensity = "_EmissiveIntensity";

		internal const string kEmissiveIntensityUnit = "_EmissiveIntensityUnit";

		internal const string kForceForwardEmissive = "_ForceForwardEmissive";

		internal const string kEmissiveColor = "_EmissiveColor";

		internal const string kEmissiveColorLDR = "_EmissiveColorLDR";

		internal const string kEmissiveColorHDR = "_EmissiveColorHDR";

		internal const string kEmissiveColorMap = "_EmissiveColorMap";

		internal const string kUVEmissive = "_UVEmissive";

		internal const string kTessellationMode = "_TessellationMode";

		internal const string kTessellationFactor = "_TessellationFactor";

		internal const string kTessellationFactorMinDistance = "_TessellationFactorMinDistance";

		internal const string kTessellationFactorMaxDistance = "_TessellationFactorMaxDistance";

		internal const string kTessellationFactorTriangleSize = "_TessellationFactorTriangleSize";

		internal const string kTessellationShapeFactor = "_TessellationShapeFactor";

		internal const string kTessellationBackFaceCullEpsilon = "_TessellationBackFaceCullEpsilon";

		internal const string kTessellationMaxDisplacement = "_TessellationMaxDisplacement";

		internal const string kHeightMap = "_HeightMap";

		internal const string kHeightAmplitude = "_HeightAmplitude";

		internal const string kHeightCenter = "_HeightCenter";

		internal const string kHeightPoMAmplitude = "_HeightPoMAmplitude";

		internal const string kHeightTessCenter = "_HeightTessCenter";

		internal const string kHeightTessAmplitude = "_HeightTessAmplitude";

		internal const string kHeightMin = "_HeightMin";

		internal const string kHeightMax = "_HeightMax";

		internal const string kHeightOffset = "_HeightOffset";

		internal const string kHeightParametrization = "_HeightMapParametrization";

		internal const string kDisplacementLockObjectScale = "_DisplacementLockObjectScale";

		internal const string kDisplacementLockTilingScale = "_DisplacementLockTilingScale";

		internal const string kEnableHeightBlend = "_EnableHeightBlend";

		internal const string kHeightTransition = "_HeightTransition";

		internal const string kEnableInstancedPerPixelNormal = "_EnableInstancedPerPixelNormal";

		internal const string kMaskMap = "_MaskMap";

		internal const string kDetailMap = "_DetailMap";

		internal const string kNormalMap = "_NormalMap";

		internal const string kNormalMapOS = "_NormalMapOS";

		internal const string kNormalMapSpace = "_NormalMapSpace";

		internal const string kBentNormalMap = "_BentNormalMap";

		internal const string kBentNormalMapOS = "_BentNormalMapOS";

		internal const string kTangentMap = "_TangentMap";

		internal const string kTangentMapOS = "_TangentMapOS";

		internal const string kSubsurfaceMaskMap = "_SubsurfaceMaskMap";

		internal const string kTransmissionMaskMap = "_TransmissionMaskMap";

		internal const string kThicknessMap = "_ThicknessMap";

		internal const string kSpecularColorMap = "_SpecularColorMap";

		internal const string kAnisotropyMap = "_AnisotropyMap";

		internal const string kIridescenceThicknessMap = "_IridescenceThicknessMap";

		internal const string kCoatMask = "_CoatMask";

		internal const string kCoatMaskMap = "_CoatMaskMap";
	}
}
