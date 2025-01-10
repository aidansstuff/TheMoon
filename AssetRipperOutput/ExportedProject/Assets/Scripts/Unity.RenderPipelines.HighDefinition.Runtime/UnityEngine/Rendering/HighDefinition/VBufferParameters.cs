using System;

namespace UnityEngine.Rendering.HighDefinition
{
	internal struct VBufferParameters
	{
		public Vector3Int viewportSize;

		public float voxelSize;

		public Vector4 depthEncodingParams;

		public Vector4 depthDecodingParams;

		public VBufferParameters(Vector3Int viewportSize, float depthExtent, float camNear, float camFar, float camVFoV, float sliceDistributionUniformity, float voxelSize)
		{
			this.viewportSize = viewportSize;
			this.voxelSize = voxelSize;
			float num = (float)viewportSize.x / (float)viewportSize.y;
			float num2 = 2f * Mathf.Tan(0.5f * camVFoV) * camFar;
			float num3 = Mathf.Max(num2 * num, num2);
			float val = Mathf.Sqrt(camFar * camFar + 0.25f * num3 * num3);
			float farPlane = Math.Min(camNear + depthExtent, val);
			float a = 2f - 2f * sliceDistributionUniformity;
			a = Mathf.Max(a, 0.001f);
			depthEncodingParams = ComputeLogarithmicDepthEncodingParams(camNear, farPlane, a);
			depthDecodingParams = ComputeLogarithmicDepthDecodingParams(camNear, farPlane, a);
		}

		internal Vector3 ComputeViewportScale(Vector3Int bufferSize)
		{
			return new Vector3(HDUtils.ComputeViewportScale(viewportSize.x, bufferSize.x), HDUtils.ComputeViewportScale(viewportSize.y, bufferSize.y), HDUtils.ComputeViewportScale(viewportSize.z, bufferSize.z));
		}

		internal Vector3 ComputeViewportLimit(Vector3Int bufferSize)
		{
			return new Vector3(HDUtils.ComputeViewportLimit(viewportSize.x, bufferSize.x), HDUtils.ComputeViewportLimit(viewportSize.y, bufferSize.y), HDUtils.ComputeViewportLimit(viewportSize.z, bufferSize.z));
		}

		internal float ComputeLastSliceDistance(uint sliceCount)
		{
			float num = 1f - 0.5f / (float)sliceCount;
			float num2 = 0.6931472f;
			return depthDecodingParams.x * Mathf.Exp(num2 * num * depthDecodingParams.y) + depthDecodingParams.z;
		}

		private float EncodeLogarithmicDepthGeneralized(float z, Vector4 encodingParams)
		{
			return encodingParams.x + encodingParams.y * Mathf.Log(Mathf.Max(0f, z - encodingParams.z), 2f);
		}

		private float DecodeLogarithmicDepthGeneralized(float d, Vector4 decodingParams)
		{
			return decodingParams.x * Mathf.Pow(2f, d * decodingParams.y) + decodingParams.z;
		}

		internal int ComputeSliceIndexFromDistance(float distance, int maxSliceCount)
		{
			distance = Mathf.Clamp(distance, 0f, ComputeLastSliceDistance((uint)maxSliceCount));
			float num = DecodeLogarithmicDepthGeneralized(0f, depthDecodingParams);
			float z = distance + num;
			float num2 = EncodeLogarithmicDepthGeneralized(z, depthEncodingParams);
			float num3 = 1f / (float)maxSliceCount;
			return (int)((num2 - num3) / num3);
		}

		private static Vector4 ComputeLogarithmicDepthEncodingParams(float nearPlane, float farPlane, float c)
		{
			Vector4 result = default(Vector4);
			result.y = 1f / Mathf.Log(c * (farPlane - nearPlane) + 1f, 2f);
			result.x = Mathf.Log(c, 2f) * result.y;
			result.z = nearPlane - 1f / c;
			result.w = 0f;
			return result;
		}

		private static Vector4 ComputeLogarithmicDepthDecodingParams(float nearPlane, float farPlane, float c)
		{
			Vector4 result = default(Vector4);
			result.x = 1f / c;
			result.y = Mathf.Log(c * (farPlane - nearPlane) + 1f, 2f);
			result.z = nearPlane - 1f / c;
			result.w = 0f;
			return result;
		}
	}
}
