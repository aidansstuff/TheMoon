using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Material/Diffusion Profile List", new Type[] { typeof(HDRenderPipeline) })]
	public class DiffusionProfileList : VolumeComponent
	{
		[Tooltip("List of diffusion profiles used inside the volume.")]
		[SerializeField]
		public DiffusionProfileSettingsParameter diffusionProfiles = new DiffusionProfileSettingsParameter(null);
	}
}
