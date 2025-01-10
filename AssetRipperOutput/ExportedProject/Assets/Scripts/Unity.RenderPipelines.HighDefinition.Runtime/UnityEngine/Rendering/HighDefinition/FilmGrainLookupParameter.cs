using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class FilmGrainLookupParameter : VolumeParameter<FilmGrainLookup>
	{
		public FilmGrainLookupParameter(FilmGrainLookup value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
