using System;

namespace UnityEngine.Rendering.HighDefinition.Compositor
{
	[Serializable]
	internal class ShaderProperty
	{
		public string propertyName;

		public ShaderPropertyType propertyType;

		public Vector4 value;

		public Vector2 rangeLimits;

		public ShaderPropertyFlags flags;

		public bool canBeUsedAsRT;

		public static ShaderProperty Create(Shader shader, Material material, int index)
		{
			ShaderProperty shaderProperty = new ShaderProperty();
			shaderProperty.propertyName = shader.GetPropertyName(index);
			shaderProperty.propertyType = shader.GetPropertyType(index);
			shaderProperty.flags = shader.GetPropertyFlags(index);
			shaderProperty.value = Vector4.zero;
			shaderProperty.canBeUsedAsRT = false;
			if (shaderProperty.propertyType == ShaderPropertyType.Texture)
			{
				shader.FindTextureStack(index, out var stackName, out var _);
				shaderProperty.canBeUsedAsRT = stackName.Length == 0;
				shaderProperty.canBeUsedAsRT &= shader.GetPropertyTextureDimension(index) == TextureDimension.Tex2D;
			}
			if (shaderProperty.propertyType == ShaderPropertyType.Range)
			{
				shaderProperty.rangeLimits = shader.GetPropertyRangeLimits(index);
				shaderProperty.value = new Vector4(material.GetFloat(Shader.PropertyToID(shader.GetPropertyName(index))), 0f, 0f, 0f);
			}
			else if (shaderProperty.propertyType == ShaderPropertyType.Color)
			{
				shaderProperty.value = material.GetColor(Shader.PropertyToID(shader.GetPropertyName(index)));
			}
			else if (shaderProperty.propertyType == ShaderPropertyType.Vector)
			{
				shaderProperty.value = material.GetVector(Shader.PropertyToID(shader.GetPropertyName(index)));
			}
			return shaderProperty;
		}
	}
}
