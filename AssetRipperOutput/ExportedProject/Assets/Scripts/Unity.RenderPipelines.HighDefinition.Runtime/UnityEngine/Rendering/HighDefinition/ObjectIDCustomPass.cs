using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class ObjectIDCustomPass : DrawRenderersCustomPass
	{
		private static readonly int k_ObjectColor = Shader.PropertyToID("ObjectColor");

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			base.Setup(renderContext, cmd);
			AssignObjectIDs();
			overrideMaterial = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaderGraphs.objectIDPS);
			overrideMaterialPassName = "ForwardOnly";
		}

		public virtual void AssignObjectIDs()
		{
			int sceneCount = SceneManager.sceneCount;
			List<Renderer> list = new List<Renderer>();
			for (int i = 0; i < sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.IsValid() && sceneAt.isLoaded)
				{
					GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
					foreach (GameObject gameObject in rootGameObjects)
					{
						list.AddRange(gameObject.GetComponentsInChildren<Renderer>());
					}
				}
			}
			int count = list.Count;
			for (int k = 0; k < count; k++)
			{
				Renderer renderer = list[k];
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				materialPropertyBlock.SetColor(value: Color.HSVToRGB((float)(k * 3 % count) / (float)count, 0.7f, 1f), nameID: k_ObjectColor);
				renderer.SetPropertyBlock(materialPropertyBlock);
			}
		}
	}
}
