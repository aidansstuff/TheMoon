namespace UnityEngine.Rendering.HighDefinition
{
	internal static class VisibleLightExtensionMethods
	{
		public struct VisibleLightAxisAndPosition
		{
			public Vector3 Position;

			public Vector3 Forward;

			public Vector3 Up;

			public Vector3 Right;
		}

		public static Vector3 GetPosition(this VisibleLight value)
		{
			return value.localToWorldMatrix.GetColumn(3);
		}

		public static Vector3 GetForward(this VisibleLight value)
		{
			return value.localToWorldMatrix.GetColumn(2);
		}

		public static Vector3 GetUp(this VisibleLight value)
		{
			return value.localToWorldMatrix.GetColumn(1);
		}

		public static Vector3 GetRight(this VisibleLight value)
		{
			return value.localToWorldMatrix.GetColumn(0);
		}

		public static VisibleLightAxisAndPosition GetAxisAndPosition(this VisibleLight value)
		{
			Matrix4x4 localToWorldMatrix = value.localToWorldMatrix;
			VisibleLightAxisAndPosition result = default(VisibleLightAxisAndPosition);
			result.Position = localToWorldMatrix.GetColumn(3);
			result.Forward = localToWorldMatrix.GetColumn(2);
			result.Up = localToWorldMatrix.GetColumn(1);
			result.Right = localToWorldMatrix.GetColumn(0);
			return result;
		}
	}
}
