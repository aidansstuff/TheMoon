using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	public class ScalableSettingSchema
	{
		internal static readonly Dictionary<ScalableSettingSchemaId, ScalableSettingSchema> Schemas = new Dictionary<ScalableSettingSchemaId, ScalableSettingSchema>
		{
			{
				ScalableSettingSchemaId.With3Levels,
				new ScalableSettingSchema(new GUIContent[3]
				{
					new GUIContent("Low"),
					new GUIContent("Medium"),
					new GUIContent("High")
				})
			},
			{
				ScalableSettingSchemaId.With4Levels,
				new ScalableSettingSchema(new GUIContent[4]
				{
					new GUIContent("Low"),
					new GUIContent("Medium"),
					new GUIContent("High"),
					new GUIContent("Ultra")
				})
			}
		};

		public readonly GUIContent[] levelNames;

		public int levelCount => levelNames.Length;

		internal static ScalableSettingSchema GetSchemaOrNull(ScalableSettingSchemaId id)
		{
			if (!Schemas.TryGetValue(id, out var value))
			{
				return null;
			}
			return value;
		}

		internal static ScalableSettingSchema GetSchemaOrNull(ScalableSettingSchemaId? id)
		{
			if (!id.HasValue || !Schemas.TryGetValue(id.Value, out var value))
			{
				return null;
			}
			return value;
		}

		public ScalableSettingSchema(GUIContent[] levelNames)
		{
			this.levelNames = levelNames;
		}
	}
}
