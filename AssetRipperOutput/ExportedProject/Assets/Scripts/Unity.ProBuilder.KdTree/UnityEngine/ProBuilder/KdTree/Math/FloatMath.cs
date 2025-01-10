using System;

namespace UnityEngine.ProBuilder.KdTree.Math
{
	[Serializable]
	internal class FloatMath : TypeMath<float>
	{
		public override float MinValue => float.MinValue;

		public override float MaxValue => float.MaxValue;

		public override float Zero => 0f;

		public override float NegativeInfinity => float.NegativeInfinity;

		public override float PositiveInfinity => float.PositiveInfinity;

		public override int Compare(float a, float b)
		{
			return a.CompareTo(b);
		}

		public override bool AreEqual(float a, float b)
		{
			return a == b;
		}

		public override float Add(float a, float b)
		{
			return a + b;
		}

		public override float Subtract(float a, float b)
		{
			return a - b;
		}

		public override float Multiply(float a, float b)
		{
			return a * b;
		}

		public override float DistanceSquaredBetweenPoints(float[] a, float[] b)
		{
			float num = Zero;
			int num2 = a.Length;
			for (int i = 0; i < num2; i++)
			{
				float num3 = Subtract(a[i], b[i]);
				float b2 = Multiply(num3, num3);
				num = Add(num, b2);
			}
			return num;
		}
	}
}
