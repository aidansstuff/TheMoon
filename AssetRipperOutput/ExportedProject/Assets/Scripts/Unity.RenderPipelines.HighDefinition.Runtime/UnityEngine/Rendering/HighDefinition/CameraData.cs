namespace UnityEngine.Rendering.HighDefinition
{
	internal struct CameraData
	{
		public uint width;

		public uint height;

		public bool skyEnabled;

		public bool fogEnabled;

		public AccelerationStructureSize accelSize;

		public float accumulatedWeight;

		public uint currentIteration;

		public void ResetIteration()
		{
			accumulatedWeight = 0f;
			currentIteration = 0u;
		}
	}
}
