namespace Unity.Multiplayer.Tools.Common
{
	internal struct MinAndMax
	{
		public float Min { get; set; }

		public float Max { get; set; }

		public MinAndMax(float min, float max)
		{
			Min = min;
			Max = max;
		}
	}
}
