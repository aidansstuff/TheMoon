using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class ScalableSetting<T> : ISerializationCallbackReceiver
	{
		[SerializeField]
		private T[] m_Values;

		[SerializeField]
		private ScalableSettingSchemaId m_SchemaId;

		public ScalableSettingSchemaId schemaId
		{
			get
			{
				return m_SchemaId;
			}
			set
			{
				m_SchemaId = value;
			}
		}

		public T this[int index]
		{
			get
			{
				if (m_Values == null || index < 0 || index >= m_Values.Length)
				{
					return default(T);
				}
				return m_Values[index];
			}
		}

		public ScalableSetting(T[] values, ScalableSettingSchemaId schemaId)
		{
			m_Values = values;
			m_SchemaId = schemaId;
		}

		public bool TryGet(int index, out T value)
		{
			if (index >= 0 && index < m_Values.Length)
			{
				value = m_Values[index];
				return true;
			}
			value = default(T);
			return false;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (ScalableSettingSchema.Schemas.TryGetValue(m_SchemaId, out var value))
			{
				Array.Resize(ref m_Values, value.levelCount);
			}
			else if (m_Values == null)
			{
				m_Values = new T[0];
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (ScalableSettingSchema.Schemas.TryGetValue(m_SchemaId, out var value))
			{
				Array.Resize(ref m_Values, value.levelCount);
			}
			else if (m_Values == null)
			{
				m_Values = new T[0];
			}
		}
	}
}
