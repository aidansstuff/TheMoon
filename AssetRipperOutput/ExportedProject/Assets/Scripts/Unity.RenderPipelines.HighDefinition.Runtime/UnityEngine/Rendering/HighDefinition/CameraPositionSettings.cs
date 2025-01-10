using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public struct CameraPositionSettings
	{
		public enum Mode
		{
			ComputeWorldToCameraMatrix = 0,
			UseWorldToCameraMatrixField = 1
		}

		[Obsolete("Since 2019.3, use CameraPositionSettings.NewDefault() instead.")]
		public static readonly CameraPositionSettings @default;

		public Mode mode;

		public Vector3 position;

		public Quaternion rotation;

		public Matrix4x4 worldToCameraMatrix;

		public static CameraPositionSettings NewDefault()
		{
			CameraPositionSettings result = default(CameraPositionSettings);
			result.mode = Mode.ComputeWorldToCameraMatrix;
			result.position = Vector3.zero;
			result.rotation = Quaternion.identity;
			result.worldToCameraMatrix = Matrix4x4.identity;
			return result;
		}

		public Matrix4x4 ComputeWorldToCameraMatrix()
		{
			return GeometryUtils.CalculateWorldToCameraMatrixRHS(position, rotation);
		}

		public Matrix4x4 GetUsedWorldToCameraMatrix()
		{
			return mode switch
			{
				Mode.ComputeWorldToCameraMatrix => ComputeWorldToCameraMatrix(), 
				Mode.UseWorldToCameraMatrixField => worldToCameraMatrix, 
				_ => throw new ArgumentException(), 
			};
		}
	}
}
