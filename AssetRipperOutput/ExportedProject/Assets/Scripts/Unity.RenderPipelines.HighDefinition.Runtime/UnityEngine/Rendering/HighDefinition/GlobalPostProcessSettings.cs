using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public struct GlobalPostProcessSettings
	{
		public const int k_MinLutSize = 16;

		public const int k_MaxLutSize = 65;

		[Range(16f, 65f)]
		[SerializeField]
		private int m_LutSize;

		[FormerlySerializedAs("m_LutFormat")]
		public GradingLutFormat lutFormat;

		public PostProcessBufferFormat bufferFormat;

		internal bool supportsAlpha => bufferFormat != PostProcessBufferFormat.R11G11B10;

		public int lutSize
		{
			get
			{
				return m_LutSize;
			}
			set
			{
				m_LutSize = Mathf.Clamp(value, 16, 65);
			}
		}

		internal static GlobalPostProcessSettings NewDefault()
		{
			GlobalPostProcessSettings result = default(GlobalPostProcessSettings);
			result.lutSize = 32;
			result.lutFormat = GradingLutFormat.R16G16B16A16;
			result.bufferFormat = PostProcessBufferFormat.R11G11B10;
			return result;
		}
	}
}
