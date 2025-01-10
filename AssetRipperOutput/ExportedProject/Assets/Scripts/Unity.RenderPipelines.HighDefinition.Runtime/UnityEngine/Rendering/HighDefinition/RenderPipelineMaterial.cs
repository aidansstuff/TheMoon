namespace UnityEngine.Rendering.HighDefinition
{
	internal class RenderPipelineMaterial : Object
	{
		public virtual bool IsDefferedMaterial()
		{
			return false;
		}

		public virtual void Build(HDRenderPipelineAsset hdAsset, HDRenderPipelineRuntimeResources defaultResources)
		{
		}

		public virtual void Cleanup()
		{
		}

		public virtual void RenderInit(CommandBuffer cmd)
		{
		}

		public virtual void Bind(CommandBuffer cmd)
		{
		}
	}
}
