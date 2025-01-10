using System;

namespace Unity.Multiplayer.Tools.Common
{
	internal class ExponentialMovingAverage
	{
		public float Parameter { get; private set; }

		public float Value { get; private set; }

		public static ExponentialMovingAverage ApproximatingSimpleMovingAverage(int sampleCount)
		{
			return new ExponentialMovingAverage(GetParameterApproximatingSimpleMovingAverage(sampleCount));
		}

		public static float GetParameterApproximatingSimpleMovingAverage(int sampleCount)
		{
			return 2f / (float)(sampleCount + 1);
		}

		public ExponentialMovingAverage(float parameter, float value = 0f)
		{
			if (!(0f <= parameter) || !(parameter <= 1f))
			{
				throw new ArgumentException($"ExponentialMovingAverage parameter {parameter} should be in range [0, 1]");
			}
			Parameter = parameter;
			Value = value;
		}

		public void ClearValue()
		{
			Value = 0f;
		}

		public void ClearValueAndParameter()
		{
			Parameter = 0f;
			Value = 0f;
		}

		public void AddSample(float x)
		{
			Value = Parameter * x + (1f - Parameter) * Value;
		}
	}
}
