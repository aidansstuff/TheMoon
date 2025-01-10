using Unity.Mathematics;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class GeometryUtils
	{
		public unsafe static bool Overlap(OrientedBBox obb, Frustum frustum, int numPlanes, int numCorners)
		{
			bool flag = true;
			int num = 0;
			while (flag && num < numPlanes)
			{
				Vector3 normal = frustum.planes[num].normal;
				float distance = frustum.planes[num].distance;
				float num2 = obb.extentX * Mathf.Abs(Vector3.Dot(normal, obb.right)) + obb.extentY * Mathf.Abs(Vector3.Dot(normal, obb.up)) + obb.extentZ * Mathf.Abs(Vector3.Dot(normal, obb.forward));
				float num3 = Vector3.Dot(normal, obb.center) + distance;
				flag = flag && num2 + num3 >= 0f;
				num++;
			}
			if (numCorners == 0)
			{
				return flag;
			}
			Plane* ptr = stackalloc Plane[3];
			ptr->normal = obb.right;
			ptr->distance = obb.extentX;
			ptr[1].normal = obb.up;
			ptr[1].distance = obb.extentY;
			ptr[2].normal = obb.forward;
			ptr[2].distance = obb.extentZ;
			int num4 = 0;
			while (flag && num4 < 3)
			{
				Plane plane = ptr[num4];
				bool flag2 = true;
				bool flag3 = true;
				for (int i = 0; i < numCorners; i++)
				{
					float num5 = Vector3.Dot(plane.normal, frustum.corners[i] - obb.center);
					flag2 = flag2 && num5 > plane.distance;
					flag3 = flag3 && 0f - num5 > plane.distance;
				}
				flag = flag && !(flag2 || flag3);
				num4++;
			}
			return flag;
		}

		public static Vector4 Plane(Vector3 position, Vector3 normal)
		{
			Vector3 lhs = normal;
			float w = 0f - Vector3.Dot(lhs, position);
			return new Vector4(lhs.x, lhs.y, lhs.z, w);
		}

		public static Vector4 CameraSpacePlane(Matrix4x4 worldToCamera, Vector3 positionWS, Vector3 normalWS, float sideSign = 1f, float clipPlaneOffset = 0f)
		{
			Vector3 point = positionWS + normalWS * clipPlaneOffset;
			Vector3 lhs = worldToCamera.MultiplyPoint(point);
			Vector3 rhs = worldToCamera.MultiplyVector(normalWS).normalized * sideSign;
			return new Vector4(rhs.x, rhs.y, rhs.z, 0f - Vector3.Dot(lhs, rhs));
		}

		public static Matrix4x4 CalculateWorldToCameraMatrixRHS(Vector3 position, Quaternion rotation)
		{
			return Matrix4x4.Scale(new Vector3(1f, 1f, -1f)) * Matrix4x4.TRS(position, rotation, Vector3.one).inverse;
		}

		public static Matrix4x4 CalculateWorldToCameraMatrixRHS(Transform transform)
		{
			return Matrix4x4.Scale(new Vector3(1f, 1f, -1f)) * transform.localToWorldMatrix.inverse;
		}

		public static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 sourceProjection, Vector4 clipPlane)
		{
			Matrix4x4 result = sourceProjection;
			Matrix4x4 inverse = sourceProjection.inverse;
			Vector4 vector = new Vector4(Mathf.Sign(clipPlane.x), Mathf.Sign(clipPlane.y), 1f, 1f);
			Vector4 b = inverse * vector;
			Vector4 a = new Vector4(result[3], result[7], result[11], result[15]);
			Vector4 vector2 = clipPlane * (2f * Vector4.Dot(a, b) / Vector4.Dot(clipPlane, b));
			result[2] = vector2.x - a.x;
			result[6] = vector2.y - a.y;
			result[10] = vector2.z - a.z;
			result[14] = vector2.w - a.w;
			return result;
		}

		public static Matrix4x4 CalculateReflectionMatrix(Vector3 position, Vector3 normal)
		{
			return CalculateReflectionMatrix(Plane(position, normal.normalized));
		}

		public static Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
		{
			Matrix4x4 result = default(Matrix4x4);
			result.m00 = 1f - 2f * plane[0] * plane[0];
			result.m01 = -2f * plane[0] * plane[1];
			result.m02 = -2f * plane[0] * plane[2];
			result.m03 = -2f * plane[3] * plane[0];
			result.m10 = -2f * plane[1] * plane[0];
			result.m11 = 1f - 2f * plane[1] * plane[1];
			result.m12 = -2f * plane[1] * plane[2];
			result.m13 = -2f * plane[3] * plane[1];
			result.m20 = -2f * plane[2] * plane[0];
			result.m21 = -2f * plane[2] * plane[1];
			result.m22 = 1f - 2f * plane[2] * plane[2];
			result.m23 = -2f * plane[3] * plane[2];
			result.m30 = 0f;
			result.m31 = 0f;
			result.m32 = 0f;
			result.m33 = 1f;
			return result;
		}

		public static bool IsProjectionMatrixOblique(Matrix4x4 projectionMatrix)
		{
			if (projectionMatrix[2] == 0f)
			{
				return projectionMatrix[6] != 0f;
			}
			return true;
		}

		public static Matrix4x4 CalculateProjectionMatrix(Camera camera)
		{
			if (camera.orthographic)
			{
				float orthographicSize = camera.orthographicSize;
				float num = camera.orthographicSize * camera.aspect;
				return Matrix4x4.Ortho(0f - num, num, 0f - orthographicSize, orthographicSize, camera.nearClipPlane, camera.farClipPlane);
			}
			return Matrix4x4.Perspective(camera.GetGateFittedFieldOfView(), camera.aspect, camera.nearClipPlane, camera.farClipPlane);
		}

		private static float DistanceToOriginAABB(Vector3 point, Vector3 aabbSize)
		{
			float3 x = math.abs(point) - math.float3(aabbSize);
			return math.length(math.max(x, 0f)) + math.min(math.max(x.x, math.max(x.y, x.z)), 0f);
		}

		public static float DistanceToOBB(OrientedBBox obb, Vector3 point)
		{
			float3 x = point - obb.center;
			float3 y = math.normalize(math.cross(obb.right, obb.up));
			return DistanceToOriginAABB(math.float3(math.dot(x, math.normalize(obb.right)), math.dot(x, math.normalize(obb.up)), math.dot(x, y)), math.float3(obb.extentX, obb.extentY, obb.extentZ));
		}
	}
}
