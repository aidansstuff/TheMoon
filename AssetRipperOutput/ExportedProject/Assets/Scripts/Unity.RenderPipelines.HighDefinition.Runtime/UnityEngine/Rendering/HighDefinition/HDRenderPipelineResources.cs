namespace UnityEngine.Rendering.HighDefinition
{
	internal abstract class HDRenderPipelineResources : RenderPipelineResources
	{
		protected override string packagePath => HDUtils.GetHDRenderPipelinePath();
	}
}
