using UnityEngine;

namespace Unity.Netcode
{
	public class BufferedLinearInterpolatorVector3 : BufferedLinearInterpolator<Vector3>
	{
		public bool IsSlerp;

		protected override Vector3 InterpolateUnclamped(Vector3 start, Vector3 end, float time)
		{
			if (IsSlerp)
			{
				return Vector3.Slerp(start, end, time);
			}
			return Vector3.Lerp(start, end, time);
		}

		protected override Vector3 Interpolate(Vector3 start, Vector3 end, float time)
		{
			if (IsSlerp)
			{
				return Vector3.Slerp(start, end, time);
			}
			return Vector3.Lerp(start, end, time);
		}
	}
}
