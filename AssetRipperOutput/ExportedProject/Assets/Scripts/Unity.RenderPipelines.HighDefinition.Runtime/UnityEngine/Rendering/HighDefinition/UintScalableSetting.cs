using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class UintScalableSetting : ScalableSetting<uint>
	{
		public UintScalableSetting(uint[] values, ScalableSettingSchemaId schemaId)
			: base(values, schemaId)
		{
		}
	}
}
