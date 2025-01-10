using System;

namespace UnityEngine.Rendering.HighDefinition
{
	public abstract class SkyRenderer
	{
		private int m_LastFrameUpdate = -1;

		public bool SupportDynamicSunLight = true;

		public abstract void Build();

		public abstract void Cleanup();

		protected virtual bool Update(BuiltinSkyParameters builtinParams)
		{
			return false;
		}

		[Obsolete("Please override PreRenderSky(BuiltinSkyParameters) instead.")]
		public virtual void PreRenderSky(BuiltinSkyParameters builtinParams, bool renderForCubemap, bool renderSunDisk)
		{
			PreRenderSky(builtinParams);
		}

		public virtual void PreRenderSky(BuiltinSkyParameters builtinParams)
		{
		}

		[Obsolete("Please implement RequiresPreRender instead")]
		public virtual bool RequiresPreRenderSky(BuiltinSkyParameters builtinParams)
		{
			return false;
		}

		public virtual bool RequiresPreRender(SkySettings skySettings)
		{
			return false;
		}

		public abstract void RenderSky(BuiltinSkyParameters builtinParams, bool renderForCubemap, bool renderSunDisk);

		protected static float GetSkyIntensity(SkySettings skySettings, DebugDisplaySettings debugSettings)
		{
			return skySettings.GetIntensityFromSettings();
		}

		public virtual void SetGlobalSkyData(CommandBuffer cmd, BuiltinSkyParameters builtinParams)
		{
		}

		internal bool DoUpdate(BuiltinSkyParameters parameters)
		{
			if (m_LastFrameUpdate < parameters.frameIndex)
			{
				CommandBuffer commandBuffer = parameters.commandBuffer;
				CommandBuffer buffer = (parameters.commandBuffer = CommandBufferPool.Get("SkyUpdate"));
				m_LastFrameUpdate = parameters.frameIndex;
				bool result = Update(parameters);
				Graphics.ExecuteCommandBuffer(buffer);
				CommandBufferPool.Release(buffer);
				parameters.commandBuffer = commandBuffer;
				return result;
			}
			return false;
		}

		internal void Reset()
		{
			m_LastFrameUpdate = -1;
		}
	}
}
