using System;

namespace UnityEngine.Rendering.HighDefinition
{
	public interface IVersionable<TVersion> where TVersion : struct, IConvertible
	{
		TVersion version { get; set; }
	}
}
