using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{mask.humanizedData}")]
	public struct FrameSettingsOverrideMask
	{
		[SerializeField]
		public BitArray128 mask;
	}
}
