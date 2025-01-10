using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition.Compositor
{
	internal class CompositionProfile : ScriptableObject
	{
		[SerializeField]
		private List<ShaderProperty> m_ShaderProperties = new List<ShaderProperty>();

		public void AddPropertiesFromShaderAndMaterial(CompositionManager compositor, Shader shader, Material material)
		{
			List<string> list = new List<string>();
			int propertyCount = shader.GetPropertyCount();
			for (int k = 0; k < propertyCount; k++)
			{
				ShaderProperty shaderProperty = ShaderProperty.Create(shader, material, k);
				AddShaderProperty(compositor, shaderProperty);
				list.Add(shaderProperty.propertyName);
			}
			int j = m_ShaderProperties.Count - 1;
			while (j >= 0)
			{
				if (list.FindIndex((string x) => x == m_ShaderProperties[j].propertyName) < 0)
				{
					m_ShaderProperties.RemoveAt(j);
				}
				int num = j - 1;
				j = num;
			}
			int i = compositor.layers.Count - 1;
			while (i >= 0)
			{
				if (compositor.layers[i].outputTarget != CompositorLayer.OutputTarget.CameraStack && list.FindIndex((string x) => x == compositor.layers[i].name) < 0)
				{
					compositor.RemoveLayerAtIndex(i);
				}
				int num = i - 1;
				i = num;
			}
		}

		public void AddShaderProperty(CompositionManager compositor, ShaderProperty sp)
		{
			bool flag = (sp.flags & ShaderPropertyFlags.NonModifiableTextureData) != 0 || (sp.flags & ShaderPropertyFlags.HideInInspector) != 0;
			if (!flag && m_ShaderProperties.FindIndex((ShaderProperty s) => s.propertyName == sp.propertyName) < 0)
			{
				m_ShaderProperties.Add(sp);
			}
			if (sp.propertyType == ShaderPropertyType.Texture && sp.canBeUsedAsRT)
			{
				int num = compositor.layers.FindIndex((CompositorLayer s) => s.name == sp.propertyName);
				if (num < 0 && !flag)
				{
					CompositorLayer item = CompositorLayer.CreateOutputLayer(sp.propertyName);
					compositor.layers.Add(item);
				}
				else if (num >= 0 && flag)
				{
					compositor.RemoveLayerAtIndex(num);
				}
			}
		}

		public void CopyPropertiesToMaterial(Material material)
		{
			foreach (ShaderProperty shaderProperty in m_ShaderProperties)
			{
				if (shaderProperty.propertyType == ShaderPropertyType.Float)
				{
					material.SetFloat(shaderProperty.propertyName, shaderProperty.value.x);
				}
				else if (shaderProperty.propertyType == ShaderPropertyType.Vector)
				{
					material.SetVector(shaderProperty.propertyName, shaderProperty.value);
				}
				else if (shaderProperty.propertyType == ShaderPropertyType.Range)
				{
					material.SetFloat(shaderProperty.propertyName, shaderProperty.value.x);
				}
				else if (shaderProperty.propertyType == ShaderPropertyType.Color)
				{
					material.SetColor(shaderProperty.propertyName, shaderProperty.value);
				}
			}
		}
	}
}
