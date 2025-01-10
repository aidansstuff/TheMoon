using System;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class HDShadowUtils
	{
		public static readonly float k_MinShadowNearPlane = 0.01f;

		public static readonly float k_MaxShadowNearPlane = 10f;

		private static Plane[] s_CachedPlanes = new Plane[6];

		public static readonly Matrix4x4[] kCubemapFaces = new Matrix4x4[6]
		{
			new Matrix4x4(new Vector4(0f, 0f, -1f, 0f), new Vector4(0f, 1f, 0f, 0f), new Vector4(-1f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 1f)),
			new Matrix4x4(new Vector4(0f, 0f, 1f, 0f), new Vector4(0f, 1f, 0f, 0f), new Vector4(1f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 1f)),
			new Matrix4x4(new Vector4(1f, 0f, 0f, 0f), new Vector4(0f, 0f, -1f, 0f), new Vector4(0f, -1f, 0f, 0f), new Vector4(0f, 0f, 0f, 1f)),
			new Matrix4x4(new Vector4(1f, 0f, 0f, 0f), new Vector4(0f, 0f, 1f, 0f), new Vector4(0f, 1f, 0f, 0f), new Vector4(0f, 0f, 0f, 1f)),
			new Matrix4x4(new Vector4(1f, 0f, 0f, 0f), new Vector4(0f, 1f, 0f, 0f), new Vector4(0f, 0f, -1f, 0f), new Vector4(0f, 0f, 0f, 1f)),
			new Matrix4x4(new Vector4(-1f, 0f, 0f, 0f), new Vector4(0f, 1f, 0f, 0f), new Vector4(0f, 0f, 1f, 0f), new Vector4(0f, 0f, 0f, 1f))
		};

		public unsafe static float Asfloat(uint val)
		{
			return *(float*)(&val);
		}

		public unsafe static float Asfloat(int val)
		{
			return *(float*)(&val);
		}

		public unsafe static int Asint(float val)
		{
			return *(int*)(&val);
		}

		public unsafe static uint Asuint(float val)
		{
			return *(uint*)(&val);
		}

		private static float GetPunctualFilterWidthInTexels(HDShadowFilteringQuality quality)
		{
			return quality switch
			{
				HDShadowFilteringQuality.Low => 3f, 
				HDShadowFilteringQuality.Medium => 5f, 
				_ => 1f, 
			};
		}

		public static void ExtractPointLightData(VisibleLight visibleLight, Vector2 viewportSize, float nearPlane, float normalBiasMax, uint faceIndex, HDShadowFilteringQuality filteringQuality, out Matrix4x4 view, out Matrix4x4 invViewProjection, out Matrix4x4 projection, out Matrix4x4 deviceProjection, out Matrix4x4 deviceProjectionYFlip, out ShadowSplitData splitData)
		{
			float guardAngle = CalcGuardAnglePerspective(90f, viewportSize.x, GetPunctualFilterWidthInTexels(filteringQuality), normalBiasMax, 79f);
			ExtractPointLightMatrix(visibleLight, faceIndex, nearPlane, guardAngle, out view, out projection, out deviceProjection, out deviceProjectionYFlip, out invViewProjection, out var _, out splitData);
		}

		public static void ExtractSpotLightData(SpotLightShape shape, float spotAngle, float nearPlane, float aspectRatio, float shapeWidth, float shapeHeight, VisibleLight visibleLight, Vector2 viewportSize, float normalBiasMax, HDShadowFilteringQuality filteringQuality, out Matrix4x4 view, out Matrix4x4 invViewProjection, out Matrix4x4 projection, out Matrix4x4 deviceProjection, out Matrix4x4 deviceProjectionYFlip, out ShadowSplitData splitData)
		{
			if (shape != SpotLightShape.Pyramid)
			{
				aspectRatio = 1f;
			}
			if (shape != SpotLightShape.Box)
			{
				nearPlane = Mathf.Max(k_MinShadowNearPlane, nearPlane);
			}
			float guardAngle = CalcGuardAnglePerspective(spotAngle, viewportSize.x, GetPunctualFilterWidthInTexels(filteringQuality), normalBiasMax, 180f - spotAngle);
			ExtractSpotLightMatrix(visibleLight, 0f, spotAngle, nearPlane, guardAngle, aspectRatio, out view, out projection, out deviceProjection, out deviceProjectionYFlip, out invViewProjection, out var _, out splitData);
			if (shape == SpotLightShape.Box)
			{
				projection = ExtractBoxLightProjectionMatrix(visibleLight.range, shapeWidth, shapeHeight, nearPlane);
				InvertView(ref view, out var invview);
				Vector3 vector = invview.GetColumn(0);
				Vector3 vector2 = invview.GetColumn(1);
				Vector3 vector3 = -invview.GetColumn(2);
				Vector3 vector4 = invview.GetColumn(3);
				splitData.cullingPlaneCount = 6;
				splitData.SetCullingPlane(0, new Plane(vector, vector4 - vector * (0.5f * shapeWidth)));
				splitData.SetCullingPlane(1, new Plane(-vector, vector4 + vector * (0.5f * shapeWidth)));
				splitData.SetCullingPlane(2, new Plane(vector2, vector4 - vector2 * (0.5f * shapeHeight)));
				splitData.SetCullingPlane(3, new Plane(-vector2, vector4 + vector2 * (0.5f * shapeHeight)));
				splitData.SetCullingPlane(4, new Plane(vector3, vector4 + vector3 * nearPlane));
				splitData.SetCullingPlane(5, new Plane(-vector3, vector4 + vector3 * visibleLight.range));
				deviceProjection = GL.GetGPUProjectionMatrix(projection, renderIntoTexture: false);
				deviceProjectionYFlip = GL.GetGPUProjectionMatrix(projection, renderIntoTexture: true);
				InvertOrthographic(ref deviceProjectionYFlip, ref view, out invViewProjection);
				splitData.cullingMatrix = projection * view;
				splitData.cullingNearPlane = nearPlane;
			}
		}

		public static void ExtractDirectionalLightData(VisibleLight visibleLight, Vector2 viewportSize, uint cascadeIndex, int cascadeCount, float[] cascadeRatios, float nearPlaneOffset, CullingResults cullResults, int lightIndex, out Matrix4x4 view, out Matrix4x4 invViewProjection, out Matrix4x4 projection, out Matrix4x4 deviceProjection, out Matrix4x4 deviceProjectionYFlip, out ShadowSplitData splitData)
		{
			splitData = default(ShadowSplitData);
			splitData.cullingSphere.Set(0f, 0f, 0f, float.NegativeInfinity);
			splitData.cullingPlaneCount = 0;
			splitData.shadowCascadeBlendCullingFactor = 0.6f;
			_ = (Vector4)visibleLight.GetForward();
			Vector3 splitRatio = default(Vector3);
			int i = 0;
			for (int num = ((cascadeRatios.Length < 3) ? cascadeRatios.Length : 3); i < num; i++)
			{
				splitRatio[i] = cascadeRatios[i];
			}
			cullResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(lightIndex, (int)cascadeIndex, cascadeCount, splitRatio, (int)viewportSize.x, nearPlaneOffset, out view, out projection, out splitData);
			deviceProjection = GL.GetGPUProjectionMatrix(projection, renderIntoTexture: false);
			deviceProjectionYFlip = GL.GetGPUProjectionMatrix(projection, renderIntoTexture: true);
			InvertOrthographic(ref deviceProjection, ref view, out invViewProjection);
		}

		public static void ExtractRectangleAreaLightData(VisibleLight visibleLight, float forwardOffset, float areaLightShadowCone, float shadowNearPlane, Vector2 shapeSize, Vector2 viewportSize, float normalBiasMax, HDAreaShadowFilteringQuality filteringQuality, out Matrix4x4 view, out Matrix4x4 invViewProjection, out Matrix4x4 projection, out Matrix4x4 deviceProjection, out Matrix4x4 deviceProjectionYFlip, out ShadowSplitData splitData)
		{
			float aspectRatio = shapeSize.x / shapeSize.y;
			visibleLight.spotAngle = areaLightShadowCone;
			float guardAngle = CalcGuardAnglePerspective(visibleLight.spotAngle, viewportSize.x, 1f, normalBiasMax, 180f - visibleLight.spotAngle);
			ExtractSpotLightMatrix(visibleLight, forwardOffset, visibleLight.spotAngle, shadowNearPlane, guardAngle, aspectRatio, out view, out projection, out deviceProjection, out deviceProjectionYFlip, out invViewProjection, out var _, out splitData);
		}

		private static void InvertView(ref Matrix4x4 view, out Matrix4x4 invview)
		{
			invview = Matrix4x4.zero;
			invview.m00 = view.m00;
			invview.m01 = view.m10;
			invview.m02 = view.m20;
			invview.m10 = view.m01;
			invview.m11 = view.m11;
			invview.m12 = view.m21;
			invview.m20 = view.m02;
			invview.m21 = view.m12;
			invview.m22 = view.m22;
			invview.m33 = 1f;
			invview.m03 = 0f - (invview.m00 * view.m03 + invview.m01 * view.m13 + invview.m02 * view.m23);
			invview.m13 = 0f - (invview.m10 * view.m03 + invview.m11 * view.m13 + invview.m12 * view.m23);
			invview.m23 = 0f - (invview.m20 * view.m03 + invview.m21 * view.m13 + invview.m22 * view.m23);
		}

		private static void InvertOrthographic(ref Matrix4x4 proj, ref Matrix4x4 view, out Matrix4x4 vpinv)
		{
			InvertView(ref view, out var invview);
			Matrix4x4 zero = Matrix4x4.zero;
			zero.m00 = 1f / proj.m00;
			zero.m11 = 1f / proj.m11;
			zero.m22 = 1f / proj.m22;
			zero.m33 = 1f;
			zero.m03 = proj.m03 * zero.m00;
			zero.m13 = proj.m13 * zero.m11;
			zero.m23 = (0f - proj.m23) * zero.m22;
			vpinv = invview * zero;
		}

		private static void InvertPerspective(ref Matrix4x4 proj, ref Matrix4x4 view, out Matrix4x4 vpinv)
		{
			InvertView(ref view, out var invview);
			Matrix4x4 zero = Matrix4x4.zero;
			zero.m00 = 1f / proj.m00;
			zero.m03 = proj.m02 * zero.m00;
			zero.m11 = 1f / proj.m11;
			zero.m13 = proj.m12 * zero.m11;
			zero.m22 = 0f;
			zero.m23 = -1f;
			zero.m33 = proj.m22 / proj.m23;
			zero.m32 = zero.m33 / proj.m22;
			vpinv.m00 = invview.m00 * zero.m00;
			vpinv.m01 = invview.m01 * zero.m11;
			vpinv.m02 = invview.m03 * zero.m32;
			vpinv.m03 = invview.m00 * zero.m03 + invview.m01 * zero.m13 - invview.m02 + invview.m03 * zero.m33;
			vpinv.m10 = invview.m10 * zero.m00;
			vpinv.m11 = invview.m11 * zero.m11;
			vpinv.m12 = invview.m13 * zero.m32;
			vpinv.m13 = invview.m10 * zero.m03 + invview.m11 * zero.m13 - invview.m12 + invview.m13 * zero.m33;
			vpinv.m20 = invview.m20 * zero.m00;
			vpinv.m21 = invview.m21 * zero.m11;
			vpinv.m22 = invview.m23 * zero.m32;
			vpinv.m23 = invview.m20 * zero.m03 + invview.m21 * zero.m13 - invview.m22 + invview.m23 * zero.m33;
			vpinv.m30 = 0f;
			vpinv.m31 = 0f;
			vpinv.m32 = zero.m32;
			vpinv.m33 = zero.m33;
		}

		public static Matrix4x4 ExtractSpotLightProjectionMatrix(float range, float spotAngle, float nearPlane, float aspectRatio, float guardAngle)
		{
			float num = spotAngle + guardAngle;
			float num2 = Mathf.Max(nearPlane, k_MinShadowNearPlane);
			float num3 = 1f / Mathf.Tan(num / 180f * MathF.PI / 2f);
			float num4 = num2;
			float num5 = num4 + range;
			Matrix4x4 result = default(Matrix4x4);
			if (aspectRatio < 1f)
			{
				result.m00 = num3;
				result.m11 = num3 * aspectRatio;
			}
			else
			{
				result.m00 = num3 / aspectRatio;
				result.m11 = num3;
			}
			result.m22 = (0f - (num5 + num4)) / (num5 - num4);
			result.m23 = -2f * num5 * num4 / (num5 - num4);
			result.m32 = -1f;
			return result;
		}

		public static Matrix4x4 ExtractBoxLightProjectionMatrix(float range, float width, float height, float nearPlane)
		{
			return Matrix4x4.Ortho((0f - width) / 2f, width / 2f, (0f - height) / 2f, height / 2f, nearPlane, range);
		}

		private static Matrix4x4 ExtractSpotLightMatrix(VisibleLight vl, float forwardOffset, float spotAngle, float nearPlane, float guardAngle, float aspectRatio, out Matrix4x4 view, out Matrix4x4 proj, out Matrix4x4 deviceProj, out Matrix4x4 deviceProjYFlip, out Matrix4x4 vpinverse, out Vector4 lightDir, out ShadowSplitData splitData)
		{
			splitData = default(ShadowSplitData);
			splitData.cullingSphere.Set(0f, 0f, 0f, float.NegativeInfinity);
			splitData.cullingPlaneCount = 0;
			lightDir = vl.GetForward();
			Matrix4x4 inOutMatrix = vl.localToWorldMatrix;
			CoreMatrixUtils.MatrixTimesTranslation(ref inOutMatrix, Vector3.forward * forwardOffset);
			view = inOutMatrix.inverse;
			view.m20 *= -1f;
			view.m21 *= -1f;
			view.m22 *= -1f;
			view.m23 *= -1f;
			proj = ExtractSpotLightProjectionMatrix(vl.range - forwardOffset, spotAngle, nearPlane - forwardOffset, aspectRatio, guardAngle);
			deviceProj = GL.GetGPUProjectionMatrix(proj, renderIntoTexture: false);
			deviceProjYFlip = GL.GetGPUProjectionMatrix(proj, renderIntoTexture: true);
			InvertPerspective(ref deviceProj, ref view, out vpinverse);
			Matrix4x4 matrix = CoreMatrixUtils.MultiplyPerspectiveMatrix(proj, view);
			SetSplitDataCullingPlanesFromViewProjMatrix(ref splitData, matrix);
			Matrix4x4 result = (splitData.cullingMatrix = CoreMatrixUtils.MultiplyPerspectiveMatrix(deviceProj, view));
			splitData.cullingNearPlane = nearPlane - forwardOffset;
			return result;
		}

		private static Matrix4x4 ExtractPointLightMatrix(VisibleLight vl, uint faceIdx, float nearPlane, float guardAngle, out Matrix4x4 view, out Matrix4x4 proj, out Matrix4x4 deviceProj, out Matrix4x4 deviceProjYFlip, out Matrix4x4 vpinverse, out Vector4 lightDir, out ShadowSplitData splitData)
		{
			if (faceIdx > 5)
			{
				Debug.LogError("Tried to extract cubemap face " + faceIdx + ".");
			}
			splitData = default(ShadowSplitData);
			splitData.cullingSphere.Set(0f, 0f, 0f, float.NegativeInfinity);
			lightDir = vl.GetForward();
			Vector3 position = vl.GetPosition();
			view = kCubemapFaces[faceIdx];
			Vector3 vector = kCubemapFaces[faceIdx].MultiplyPoint(-position);
			view.SetColumn(3, new Vector4(vector.x, vector.y, vector.z, 1f));
			float num = Mathf.Max(nearPlane, k_MinShadowNearPlane);
			proj = Matrix4x4.Perspective(90f + guardAngle, 1f, num, vl.range);
			deviceProj = GL.GetGPUProjectionMatrix(proj, renderIntoTexture: false);
			deviceProjYFlip = GL.GetGPUProjectionMatrix(proj, renderIntoTexture: true);
			InvertPerspective(ref deviceProj, ref view, out vpinverse);
			Matrix4x4 matrix = CoreMatrixUtils.MultiplyPerspectiveMatrix(proj, view);
			SetSplitDataCullingPlanesFromViewProjMatrix(ref splitData, matrix);
			Matrix4x4 result = (splitData.cullingMatrix = CoreMatrixUtils.MultiplyPerspectiveMatrix(deviceProj, view));
			splitData.cullingNearPlane = num;
			return result;
		}

		private static float CalcGuardAnglePerspective(float angleInDeg, float resolution, float filterWidth, float normalBiasMax, float guardAngleMaxInDeg)
		{
			float num = angleInDeg * 0.5f * (MathF.PI / 180f);
			float num2 = 2f / resolution;
			float num3 = Mathf.Cos(num) * num2;
			float num4 = Mathf.Atan(normalBiasMax * num3 * 1.4142135f);
			num3 = Mathf.Tan(num + num4) * num2;
			num4 = Mathf.Atan((resolution + Mathf.Ceil(filterWidth)) * num3 * 0.5f) * 2f * 57.29578f - angleInDeg;
			num4 *= 2f;
			if (!(num4 < guardAngleMaxInDeg))
			{
				return guardAngleMaxInDeg;
			}
			return num4;
		}

		public static float GetSlopeBias(float baseBias, float normalizedSlopeBias)
		{
			return normalizedSlopeBias * baseBias;
		}

		private static void SetSplitDataCullingPlanesFromViewProjMatrix(ref ShadowSplitData splitData, Matrix4x4 matrix)
		{
			GeometryUtility.CalculateFrustumPlanes(matrix, s_CachedPlanes);
			if (SystemInfo.usesReversedZBuffer)
			{
				Plane plane = s_CachedPlanes[2];
				s_CachedPlanes[2] = s_CachedPlanes[3];
				s_CachedPlanes[3] = plane;
			}
			splitData.cullingPlaneCount = 6;
			for (int i = 0; i < 6; i++)
			{
				splitData.SetCullingPlane(i, s_CachedPlanes[i]);
			}
		}
	}
}
