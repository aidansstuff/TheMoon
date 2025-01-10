using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class FalseColorDebugSettings
	{
		public bool falseColor;

		public float colorThreshold0;

		public float colorThreshold1 = 2f;

		public float colorThreshold2 = 10f;

		public float colorThreshold3 = 20f;
	}
}
