using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition.Compositor
{
	[AddComponentMenu("")]
	internal class AdditionalCompositorData : MonoBehaviour
	{
		public Texture clearColorTexture;

		public RenderTexture clearDepthTexture;

		public bool clearAlpha = true;

		public BackgroundFitMode imageFitMode;

		public List<CompositionFilter> layerFilters;

		public float alphaMax = 1f;

		public float alphaMin;

		public void Init(List<CompositionFilter> layerFilters, bool clearAlpha)
		{
			this.layerFilters = new List<CompositionFilter>(layerFilters);
			this.clearAlpha = clearAlpha;
		}

		public void ResetData()
		{
			clearColorTexture = null;
			clearDepthTexture = null;
			clearAlpha = true;
			imageFitMode = BackgroundFitMode.Stretch;
			if (layerFilters != null)
			{
				layerFilters.Clear();
				layerFilters = null;
			}
			alphaMax = 1f;
			alphaMin = 0f;
		}
	}
}
