using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class FloatScalableSetting : ScalableSetting<float>
	{
		public FloatScalableSetting(float[] values, ScalableSettingSchemaId schemaId)
			: base(values, schemaId)
		{
		}
	}
}
