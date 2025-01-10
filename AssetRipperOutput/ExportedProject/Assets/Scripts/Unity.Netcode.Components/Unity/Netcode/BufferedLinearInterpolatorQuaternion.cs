using UnityEngine;

namespace Unity.Netcode
{
	public class BufferedLinearInterpolatorQuaternion : BufferedLinearInterpolator<Quaternion>
	{
		public bool IsSlerp;

		protected override Quaternion InterpolateUnclamped(Quaternion start, Quaternion end, float time)
		{
			if (IsSlerp)
			{
				return Quaternion.Slerp(start, end, time);
			}
			return Quaternion.Lerp(start, end, time);
		}

		protected override Quaternion Interpolate(Quaternion start, Quaternion end, float time)
		{
			if (IsSlerp)
			{
				return Quaternion.Slerp(start, end, time);
			}
			return Quaternion.Lerp(start, end, time);
		}
	}
}
