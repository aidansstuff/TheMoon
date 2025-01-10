using System;

namespace Unity.Multiplayer.Tools.Common
{
	internal class ContinuousExponentialMovingAverage
	{
		private const double k_DefaultInitialTime = double.NegativeInfinity;

		public static readonly double k_ln2 = Math.Log(2.0);

		public double DecayConstant { get; private set; }

		public double LastValue { get; private set; }

		public double LastTime { get; private set; }

		public static ContinuousExponentialMovingAverage CreateWithHalfLife(double halfLife)
		{
			return new ContinuousExponentialMovingAverage(GetDecayConstantForHalfLife(halfLife));
		}

		public static double GetDecayConstantForHalfLife(double halfLife)
		{
			return k_ln2 / halfLife;
		}

		public ContinuousExponentialMovingAverage(double decayConstant, double value = 0.0, double time = double.NegativeInfinity)
		{
			if (decayConstant < 0.0)
			{
				throw new ArgumentException($"ContinuousExponentialMovingAverage decay constant {decayConstant} should be >= 0; " + "otherwise it will grow exponentially over time.");
			}
			DecayConstant = decayConstant;
			LastValue = value;
			LastTime = time;
		}

		public void Reset()
		{
			DecayConstant = 0.0;
			LastValue = 0.0;
			LastTime = double.NegativeInfinity;
		}

		public void ClearValueAndTime()
		{
			LastValue = 0.0;
			LastTime = double.NegativeInfinity;
		}

		public void AddSampleForGauge(double sample, double time)
		{
			double num = Math.Exp((0.0 - (time - LastTime)) * DecayConstant);
			double num2 = 1.0 - num;
			LastValue += num2 * (sample - LastValue);
			LastTime = time;
		}

		public void AddSampleForCounter(double sample, double time)
		{
			double num = time - LastTime;
			double num2 = sample / num;
			double num3 = Math.Exp((0.0 - num) * DecayConstant);
			double num4 = 1.0 - num3;
			LastValue += num4 * (num2 - LastValue);
			LastTime = time;
		}

		public double GetGaugeValue()
		{
			return LastValue;
		}

		public double GetCounterValue(double time)
		{
			double num = Math.Exp((0.0 - (time - LastTime)) * DecayConstant);
			return LastValue * num;
		}
	}
}
