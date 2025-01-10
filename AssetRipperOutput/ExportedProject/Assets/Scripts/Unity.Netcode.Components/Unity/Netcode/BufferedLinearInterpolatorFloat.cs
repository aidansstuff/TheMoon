using UnityEngine;

namespace Unity.Netcode
{
	public class BufferedLinearInterpolatorFloat : BufferedLinearInterpolator<float>
	{
		protected override float InterpolateUnclamped(float start, float end, float time)
		{
			return Mathf.Lerp(start, end, time);
		}

		protected override float Interpolate(float start, float end, float time)
		{
			return Mathf.Lerp(start, end, time);
		}
	}
}
