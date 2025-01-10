namespace UnityEngine.Rendering.HighDefinition
{
	public abstract class CloudRenderer
	{
		private int m_LastFrameUpdate = -1;

		public bool SupportDynamicSunLight = true;

		public abstract void Build();

		public abstract void Cleanup();

		public virtual bool GetSunLightCookieParameters(CloudSettings settings, ref CookieParameters cookieParams)
		{
			return false;
		}

		public virtual void RenderSunLightCookie(BuiltinSunCookieParameters builtinParams)
		{
		}

		protected virtual bool Update(BuiltinSkyParameters builtinParams)
		{
			return false;
		}

		public virtual void PreRenderClouds(BuiltinSkyParameters builtinParams, bool renderForCubemap)
		{
		}

		public virtual bool RequiresPreRenderClouds(BuiltinSkyParameters builtinParams)
		{
			return false;
		}

		public abstract void RenderClouds(BuiltinSkyParameters builtinParams, bool renderForCubemap);

		internal bool DoUpdate(BuiltinSkyParameters parameters)
		{
			if (m_LastFrameUpdate < parameters.frameIndex)
			{
				m_LastFrameUpdate = parameters.frameIndex;
				return Update(parameters);
			}
			return false;
		}

		internal void Reset()
		{
			m_LastFrameUpdate = -1;
		}
	}
}
