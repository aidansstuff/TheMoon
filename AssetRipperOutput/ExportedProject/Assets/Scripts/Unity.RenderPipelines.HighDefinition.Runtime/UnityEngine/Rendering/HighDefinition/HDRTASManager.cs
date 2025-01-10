using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDRTASManager
	{
		public RayTracingAccelerationStructure rtas;

		public RayTracingInstanceCullingConfig cullingConfig;

		public List<RayTracingInstanceCullingTest> instanceTestArray = new List<RayTracingInstanceCullingTest>();

		internal Plane[] rtCullingPlaneArray = new Plane[6];

		private RayTracingInstanceCullingTest ShT_CT;

		private RayTracingInstanceCullingTest ShO_CT;

		private RayTracingInstanceCullingTest AO_CT;

		private RayTracingInstanceCullingTest Refl_CT;

		private RayTracingInstanceCullingTest GI_CT;

		private RayTracingInstanceCullingTest RR_CT;

		private RayTracingInstanceCullingTest SSS_CT;

		private RayTracingInstanceCullingTest PT_CT;

		public bool transformsDirty;

		public bool materialsDirty;

		public void Initialize()
		{
			cullingConfig.lodParameters.orthoSize = 0f;
			cullingConfig.lodParameters.isOrthographic = false;
			cullingConfig.subMeshFlagsConfig.opaqueMaterials = RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly;
			cullingConfig.subMeshFlagsConfig.transparentMaterials = RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.UniqueAnyHitCalls;
			cullingConfig.subMeshFlagsConfig.alphaTestedMaterials = RayTracingSubMeshFlags.Enabled;
			cullingConfig.triangleCullingConfig.checkDoubleSidedGIMaterial = true;
			cullingConfig.triangleCullingConfig.frontTriangleCounterClockwise = false;
			cullingConfig.triangleCullingConfig.optionalDoubleSidedShaderKeywords = new string[1];
			cullingConfig.triangleCullingConfig.optionalDoubleSidedShaderKeywords[0] = "_DOUBLESIDED_ON";
			cullingConfig.alphaTestedMaterialConfig.renderQueueLowerBound = HDRenderQueue.k_RenderQueue_OpaqueAlphaTest.lowerBound;
			cullingConfig.alphaTestedMaterialConfig.renderQueueUpperBound = HDRenderQueue.k_RenderQueue_OpaqueAlphaTest.upperBound;
			cullingConfig.alphaTestedMaterialConfig.optionalShaderKeywords = new string[1];
			cullingConfig.alphaTestedMaterialConfig.optionalShaderKeywords[0] = "_ALPHATEST_ON";
			cullingConfig.transparentMaterialConfig.renderQueueLowerBound = HDRenderQueue.k_RenderQueue_Transparent.lowerBound;
			cullingConfig.transparentMaterialConfig.renderQueueUpperBound = HDRenderQueue.k_RenderQueue_Transparent.upperBound;
			cullingConfig.transparentMaterialConfig.optionalShaderKeywords = new string[1];
			cullingConfig.transparentMaterialConfig.optionalShaderKeywords[0] = "_SURFACE_TYPE_TRANSPARENT";
			cullingConfig.materialTest.requiredShaderTags = new RayTracingInstanceCullingShaderTagConfig[1];
			cullingConfig.materialTest.requiredShaderTags[0].tagId = new ShaderTagId("RenderPipeline");
			cullingConfig.materialTest.requiredShaderTags[0].tagValueId = new ShaderTagId("HDRenderPipeline");
			cullingConfig.materialTest.deniedShaderPasses = DecalSystem.s_MaterialDecalPassNames;
			ShT_CT.allowOpaqueMaterials = true;
			ShT_CT.allowAlphaTestedMaterials = true;
			ShT_CT.allowTransparentMaterials = true;
			ShT_CT.layerMask = -1;
			ShT_CT.shadowCastingModeMask = 14;
			ShT_CT.instanceMask = 2u;
			ShO_CT.allowOpaqueMaterials = true;
			ShO_CT.allowAlphaTestedMaterials = true;
			ShO_CT.allowTransparentMaterials = false;
			ShO_CT.layerMask = -1;
			ShO_CT.shadowCastingModeMask = 14;
			ShO_CT.instanceMask = 4u;
			AO_CT.allowOpaqueMaterials = true;
			AO_CT.allowAlphaTestedMaterials = true;
			AO_CT.allowTransparentMaterials = false;
			AO_CT.layerMask = -1;
			AO_CT.shadowCastingModeMask = 7;
			AO_CT.instanceMask = 8u;
			Refl_CT.allowOpaqueMaterials = true;
			Refl_CT.allowAlphaTestedMaterials = true;
			Refl_CT.allowTransparentMaterials = false;
			Refl_CT.layerMask = -1;
			Refl_CT.shadowCastingModeMask = 7;
			Refl_CT.instanceMask = 16u;
			GI_CT.allowOpaqueMaterials = true;
			GI_CT.allowAlphaTestedMaterials = true;
			GI_CT.allowTransparentMaterials = false;
			GI_CT.layerMask = -1;
			GI_CT.shadowCastingModeMask = 7;
			GI_CT.instanceMask = 32u;
			RR_CT.allowOpaqueMaterials = true;
			RR_CT.allowAlphaTestedMaterials = true;
			RR_CT.allowTransparentMaterials = true;
			RR_CT.layerMask = -1;
			RR_CT.shadowCastingModeMask = 7;
			RR_CT.instanceMask = 64u;
			RR_CT.allowOpaqueMaterials = true;
			RR_CT.allowAlphaTestedMaterials = true;
			RR_CT.allowTransparentMaterials = true;
			RR_CT.layerMask = -1;
			RR_CT.shadowCastingModeMask = 7;
			RR_CT.instanceMask = 64u;
			SSS_CT.allowOpaqueMaterials = true;
			SSS_CT.allowAlphaTestedMaterials = true;
			SSS_CT.allowTransparentMaterials = false;
			SSS_CT.layerMask = -1;
			SSS_CT.shadowCastingModeMask = -1;
			SSS_CT.instanceMask = 1u;
			PT_CT.allowOpaqueMaterials = true;
			PT_CT.allowAlphaTestedMaterials = true;
			PT_CT.allowTransparentMaterials = true;
			PT_CT.layerMask = -1;
			PT_CT.shadowCastingModeMask = 7;
			PT_CT.instanceMask = 128u;
		}

		private void SetupCullingData(HDCamera hdCamera, bool pathTracingEnabled)
		{
			RayTracingSettings component = hdCamera.volumeStack.GetComponent<RayTracingSettings>();
			switch (component.cullingMode.value)
			{
			case RTASCullingMode.ExtendedFrustum:
			{
				cullingConfig.flags = RayTracingInstanceCullingFlags.EnablePlaneCulling;
				Vector3 position = hdCamera.camera.transform.position;
				Vector3 forward = hdCamera.camera.transform.forward;
				Vector3 right = hdCamera.camera.transform.right;
				Vector3 up = hdCamera.camera.transform.up;
				float farClipPlane = hdCamera.camera.farClipPlane;
				float num = Mathf.Tan(MathF.PI / 180f * hdCamera.camera.fieldOfView * 0.5f) * farClipPlane;
				float num2 = Camera.VerticalToHorizontalFieldOfView(hdCamera.camera.fieldOfView, hdCamera.camera.aspect);
				float num3 = Mathf.Tan(MathF.PI / 180f * num2 * 0.5f) * farClipPlane;
				rtCullingPlaneArray[0].normal = -forward;
				rtCullingPlaneArray[0].distance = 0f - Vector3.Dot(position + forward * farClipPlane, -forward);
				rtCullingPlaneArray[1].normal = forward;
				rtCullingPlaneArray[1].distance = 0f - Vector3.Dot(position - forward * farClipPlane, forward);
				rtCullingPlaneArray[2].normal = -right;
				rtCullingPlaneArray[2].distance = 0f - Vector3.Dot(position + right * num3, -right);
				rtCullingPlaneArray[3].normal = right;
				rtCullingPlaneArray[3].distance = 0f - Vector3.Dot(position - right * num3, right);
				rtCullingPlaneArray[4].normal = -up;
				rtCullingPlaneArray[4].distance = 0f - Vector3.Dot(position + up * num, -up);
				rtCullingPlaneArray[5].normal = up;
				rtCullingPlaneArray[5].distance = 0f - Vector3.Dot(position - up * num, up);
				cullingConfig.planes = rtCullingPlaneArray;
				break;
			}
			case RTASCullingMode.Sphere:
				cullingConfig.flags = RayTracingInstanceCullingFlags.EnableSphereCulling;
				cullingConfig.sphereRadius = component.cullingDistance.value;
				cullingConfig.sphereCenter = hdCamera.camera.transform.position;
				break;
			default:
				cullingConfig.flags = RayTracingInstanceCullingFlags.None;
				break;
			}
			cullingConfig.flags |= RayTracingInstanceCullingFlags.EnableLODCulling | RayTracingInstanceCullingFlags.IgnoreReflectionProbes;
			if (pathTracingEnabled)
			{
				cullingConfig.flags |= RayTracingInstanceCullingFlags.ComputeMaterialsCRC;
			}
		}

		public RayTracingInstanceCullingResults Cull(HDCamera hdCamera, in HDEffectsParameters parameters)
		{
			instanceTestArray.Clear();
			SetupCullingData(hdCamera, parameters.pathTracing);
			cullingConfig.lodParameters.fieldOfView = hdCamera.camera.fieldOfView;
			cullingConfig.lodParameters.cameraPosition = hdCamera.camera.transform.position;
			cullingConfig.lodParameters.cameraPixelHeight = hdCamera.camera.pixelHeight;
			if (parameters.pathTracing)
			{
				ShO_CT.layerMask = parameters.ptLayerMask;
				ShT_CT.layerMask = parameters.ptLayerMask;
			}
			if (parameters.shadows || parameters.pathTracing)
			{
				instanceTestArray.Add(ShO_CT);
				instanceTestArray.Add(ShT_CT);
			}
			if (parameters.ambientOcclusion)
			{
				AO_CT.layerMask = parameters.aoLayerMask;
				instanceTestArray.Add(AO_CT);
			}
			if (parameters.reflections)
			{
				Refl_CT.layerMask = parameters.reflLayerMask;
				instanceTestArray.Add(Refl_CT);
			}
			if (parameters.globalIllumination)
			{
				GI_CT.layerMask = parameters.giLayerMask;
				instanceTestArray.Add(GI_CT);
			}
			if (parameters.recursiveRendering)
			{
				RR_CT.layerMask = parameters.recursiveLayerMask;
				instanceTestArray.Add(RR_CT);
			}
			if (parameters.subSurface)
			{
				instanceTestArray.Add(SSS_CT);
			}
			if (parameters.pathTracing)
			{
				PT_CT.layerMask = parameters.ptLayerMask;
				instanceTestArray.Add(PT_CT);
			}
			cullingConfig.instanceTests = instanceTestArray.ToArray();
			return rtas.CullInstances(ref cullingConfig);
		}

		public void Build(HDCamera hdCamera)
		{
			if (ShaderConfig.s_CameraRelativeRendering != 0)
			{
				rtas.Build(hdCamera.mainViewConstants.worldSpaceCameraPos);
			}
			else
			{
				rtas.Build();
			}
		}

		public void Reset()
		{
			if (rtas != null)
			{
				rtas.ClearInstances();
			}
			else
			{
				rtas = new RayTracingAccelerationStructure();
			}
		}

		public void ReleaseResources()
		{
			if (rtas != null)
			{
				rtas.Dispose();
			}
		}
	}
}
