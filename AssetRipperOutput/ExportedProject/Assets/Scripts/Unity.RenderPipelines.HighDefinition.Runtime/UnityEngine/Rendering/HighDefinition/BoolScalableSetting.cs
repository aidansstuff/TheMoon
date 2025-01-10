using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class BoolScalableSetting : ScalableSetting<bool>
	{
		public BoolScalableSetting(bool[] values, ScalableSettingSchemaId schemaId)
			: base(values, schemaId)
		{
		}
	}
}
