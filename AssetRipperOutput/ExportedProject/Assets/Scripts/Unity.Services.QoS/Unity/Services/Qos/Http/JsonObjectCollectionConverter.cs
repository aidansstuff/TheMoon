using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace Unity.Services.Qos.Http
{
	[Preserve]
	internal class JsonObjectCollectionConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			object obj = value;
			Type type = value.GetType();
			if (type == typeof(Dictionary<string, IDeserializable>))
			{
				obj = (Dictionary<string, IDeserializable>)value;
			}
			else if (type == typeof(List<IDeserializable>))
			{
				obj = (List<IDeserializable>)value;
			}
			else if (type == typeof(List<List<IDeserializable>>))
			{
				obj = (List<List<IDeserializable>>)value;
			}
			if (obj == null)
			{
				writer.WriteNull();
			}
			else
			{
				JToken.FromObject(obj).WriteTo(writer);
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.Null)
			{
				List<object> obj = (List<object>)reader.Value;
				List<JsonObject> list = new List<JsonObject>();
				{
					foreach (object item in obj)
					{
						list.Add(new JsonObject(item));
					}
					return list;
				}
			}
			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}
