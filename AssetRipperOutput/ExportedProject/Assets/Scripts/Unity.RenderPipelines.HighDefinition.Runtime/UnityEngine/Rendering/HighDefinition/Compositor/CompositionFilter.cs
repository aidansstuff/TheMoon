using System;

namespace UnityEngine.Rendering.HighDefinition.Compositor
{
	[Serializable]
	internal class CompositionFilter
	{
		public enum FilterType
		{
			CHROMA_KEYING = 0,
			ALPHA_MASK = 1
		}

		public FilterType filterType;

		public Color maskColor;

		public float keyThreshold = 0.8f;

		public float keyTolerance = 0.5f;

		[Range(0f, 1f)]
		public float spillRemoval;

		public Texture alphaMask;

		public static CompositionFilter Create(FilterType type)
		{
			return new CompositionFilter
			{
				filterType = type
			};
		}
	}
}
