namespace UnityEngine.Rendering.HighDefinition
{
	internal sealed class ValueCopyAttribute : CopyFilterAttribute
	{
		public ValueCopyAttribute()
			: base(Filter.CheckContent)
		{
		}
	}
}
