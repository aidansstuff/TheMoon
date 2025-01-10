using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class PreIntegratedFGD
	{
		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\PreIntegratedFGD\\PreIntegratedFGD.cs")]
		public enum FGDTexture
		{
			Resolution = 0x40
		}

		public enum FGDIndex
		{
			FGD_GGXAndDisneyDiffuse = 0,
			FGD_CharlieAndFabricLambert = 1,
			FGD_Marschner = 2,
			Count = 3
		}

		private static PreIntegratedFGD s_Instance;

		private bool[] m_isInit = new bool[3];

		private int[] m_refCounting = new int[3];

		private Material[] m_PreIntegratedFGDMaterial = new Material[3];

		private RenderTexture[] m_PreIntegratedFGD = new RenderTexture[3];

		public static PreIntegratedFGD instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = new PreIntegratedFGD();
				}
				return s_Instance;
			}
		}

		private PreIntegratedFGD()
		{
			for (int i = 0; i < 3; i++)
			{
				m_isInit[i] = false;
				m_refCounting[i] = 0;
			}
		}

		public void Build(FGDIndex index)
		{
			if (m_refCounting[(int)index] == 0)
			{
				int num = 64;
				switch (index)
				{
				case FGDIndex.FGD_GGXAndDisneyDiffuse:
					m_PreIntegratedFGDMaterial[(int)index] = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.preIntegratedFGD_GGXDisneyDiffusePS);
					m_PreIntegratedFGD[(int)index] = new RenderTexture(num, num, 0, GraphicsFormat.A2B10G10R10_UNormPack32);
					m_PreIntegratedFGD[(int)index].hideFlags = HideFlags.HideAndDontSave;
					m_PreIntegratedFGD[(int)index].filterMode = FilterMode.Bilinear;
					m_PreIntegratedFGD[(int)index].wrapMode = TextureWrapMode.Clamp;
					m_PreIntegratedFGD[(int)index].name = CoreUtils.GetRenderTargetAutoName(num, num, 1, GraphicsFormat.A2B10G10R10_UNormPack32, "preIntegratedFGD_GGXDisneyDiffuse");
					m_PreIntegratedFGD[(int)index].Create();
					break;
				case FGDIndex.FGD_CharlieAndFabricLambert:
					m_PreIntegratedFGDMaterial[(int)index] = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.preIntegratedFGD_CharlieFabricLambertPS);
					m_PreIntegratedFGD[(int)index] = new RenderTexture(num, num, 0, GraphicsFormat.A2B10G10R10_UNormPack32);
					m_PreIntegratedFGD[(int)index].hideFlags = HideFlags.HideAndDontSave;
					m_PreIntegratedFGD[(int)index].filterMode = FilterMode.Bilinear;
					m_PreIntegratedFGD[(int)index].wrapMode = TextureWrapMode.Clamp;
					m_PreIntegratedFGD[(int)index].name = CoreUtils.GetRenderTargetAutoName(num, num, 1, GraphicsFormat.A2B10G10R10_UNormPack32, "preIntegratedFGD_CharlieFabricLambert");
					m_PreIntegratedFGD[(int)index].Create();
					break;
				case FGDIndex.FGD_Marschner:
					m_PreIntegratedFGDMaterial[(int)index] = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.preIntegratedFGD_MarschnerPS);
					m_PreIntegratedFGD[(int)index] = new RenderTexture(num, num, 0, GraphicsFormat.A2B10G10R10_UNormPack32);
					m_PreIntegratedFGD[(int)index].hideFlags = HideFlags.HideAndDontSave;
					m_PreIntegratedFGD[(int)index].filterMode = FilterMode.Bilinear;
					m_PreIntegratedFGD[(int)index].wrapMode = TextureWrapMode.Clamp;
					m_PreIntegratedFGD[(int)index].name = CoreUtils.GetRenderTargetAutoName(num, num, 1, GraphicsFormat.A2B10G10R10_UNormPack32, "preIntegratedFGD_Marschner");
					m_PreIntegratedFGD[(int)index].Create();
					break;
				}
				m_isInit[(int)index] = false;
			}
			m_refCounting[(int)index]++;
		}

		public void RenderInit(FGDIndex index, CommandBuffer cmd)
		{
			if (!m_isInit[(int)index] || !m_PreIntegratedFGD[(int)index].IsCreated())
			{
				if (GL.wireframe)
				{
					m_PreIntegratedFGD[(int)index].Create();
					return;
				}
				CoreUtils.DrawFullScreen(cmd, m_PreIntegratedFGDMaterial[(int)index], new RenderTargetIdentifier(m_PreIntegratedFGD[(int)index]));
				m_isInit[(int)index] = true;
			}
		}

		public void Cleanup(FGDIndex index)
		{
			m_refCounting[(int)index]--;
			if (m_refCounting[(int)index] == 0)
			{
				CoreUtils.Destroy(m_PreIntegratedFGDMaterial[(int)index]);
				CoreUtils.Destroy(m_PreIntegratedFGD[(int)index]);
				m_isInit[(int)index] = false;
			}
		}

		public void Bind(CommandBuffer cmd, FGDIndex index)
		{
			switch (index)
			{
			case FGDIndex.FGD_GGXAndDisneyDiffuse:
				cmd.SetGlobalTexture(HDShaderIDs._PreIntegratedFGD_GGXDisneyDiffuse, m_PreIntegratedFGD[(int)index]);
				break;
			case FGDIndex.FGD_CharlieAndFabricLambert:
				cmd.SetGlobalTexture(HDShaderIDs._PreIntegratedFGD_CharlieAndFabric, m_PreIntegratedFGD[(int)index]);
				break;
			case FGDIndex.FGD_Marschner:
				cmd.SetGlobalTexture(HDShaderIDs._PreIntegratedFGD_CharlieAndFabric, m_PreIntegratedFGD[(int)index]);
				break;
			}
		}
	}
}
