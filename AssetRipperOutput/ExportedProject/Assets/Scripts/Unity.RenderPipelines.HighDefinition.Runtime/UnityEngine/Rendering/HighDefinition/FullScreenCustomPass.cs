using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class FullScreenCustomPass : CustomPass
	{
		public Material fullscreenPassMaterial;

		[SerializeField]
		private int materialPassIndex;

		public string materialPassName = "Custom Pass 0";

		public bool fetchColorBuffer;

		private int fadeValueId;

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			fadeValueId = Shader.PropertyToID("_FadeValue");
			if (string.IsNullOrEmpty(materialPassName) && fullscreenPassMaterial != null)
			{
				materialPassName = fullscreenPassMaterial.GetPassName(materialPassIndex);
			}
		}

		protected override void Execute(CustomPassContext ctx)
		{
			if (fullscreenPassMaterial != null && fullscreenPassMaterial.passCount > 0)
			{
				if (fetchColorBuffer)
				{
					ResolveMSAAColorBuffer(ctx.cmd, ctx.hdCamera);
					SetRenderTargetAuto(ctx.cmd);
				}
				int num = fullscreenPassMaterial.FindPass(materialPassName);
				if (num == -1)
				{
					num = 0;
				}
				fullscreenPassMaterial.SetFloat(fadeValueId, base.fadeValue);
				CoreUtils.DrawFullScreen(ctx.cmd, fullscreenPassMaterial, null, num);
			}
		}

		public override IEnumerable<Material> RegisterMaterialForInspector()
		{
			yield return fullscreenPassMaterial;
		}
	}
}
