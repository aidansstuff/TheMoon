using System;

namespace UnityEngine.Rendering.HighDefinition.Attributes
{
	internal class MaterialSharedPropertyMappingAttribute : Attribute
	{
		public readonly MaterialSharedProperty property;

		public MaterialSharedPropertyMappingAttribute(MaterialSharedProperty property)
		{
			this.property = property;
		}
	}
}
