namespace UnityEngine.Rendering.HighDefinition
{
	internal sealed class ExcludeCopyAttribute : CopyFilterAttribute
	{
		public ExcludeCopyAttribute()
			: base(Filter.Exclude)
		{
		}
	}
}
