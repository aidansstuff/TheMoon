using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.Common
{
	[Serializable]
	internal class LogNormalRandomWalk
	{
		[field: SerializeField]
		public float Rate { get; set; } = 1f;


		[field: SerializeField]
		public float Min { get; set; } = 0.01f;


		[field: SerializeField]
		public float Max { get; set; } = 10f;


		public float Value { get; private set; } = 1f;


		public float NextFloat(System.Random random)
		{
			float num = Mathf.Exp(Rate * (float)(random.NextDouble() - 0.5));
			Value *= num;
			Value = Mathf.Clamp(Value, Min, Max);
			return Value;
		}

		public int NextInt(System.Random random)
		{
			return (int)Mathf.Round(NextFloat(random));
		}

		public void Repeat(System.Random random, Action action)
		{
			int num = NextInt(random);
			for (int i = 0; i < num; i++)
			{
				action();
			}
		}
	}
}
