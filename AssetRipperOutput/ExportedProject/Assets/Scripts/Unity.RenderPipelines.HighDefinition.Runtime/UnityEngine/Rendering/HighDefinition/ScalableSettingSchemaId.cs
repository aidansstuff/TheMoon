using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public struct ScalableSettingSchemaId : IEquatable<ScalableSettingSchemaId>
	{
		public static readonly ScalableSettingSchemaId With3Levels = new ScalableSettingSchemaId("With3Levels");

		public static readonly ScalableSettingSchemaId With4Levels = new ScalableSettingSchemaId("With4Levels");

		[SerializeField]
		private string m_Id;

		internal ScalableSettingSchemaId(string id)
		{
			m_Id = id;
		}

		public bool Equals(ScalableSettingSchemaId other)
		{
			return m_Id == other.m_Id;
		}

		public override bool Equals(object obj)
		{
			if (obj is ScalableSettingSchemaId scalableSettingSchemaId)
			{
				return scalableSettingSchemaId.m_Id == m_Id;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return m_Id?.GetHashCode() ?? 0;
		}

		public override string ToString()
		{
			return m_Id;
		}
	}
}
