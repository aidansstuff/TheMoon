using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class IntScalableSetting : ScalableSetting<int>
	{
		public IntScalableSetting(int[] values, ScalableSettingSchemaId schemaId)
			: base(values, schemaId)
		{
		}
	}
}
