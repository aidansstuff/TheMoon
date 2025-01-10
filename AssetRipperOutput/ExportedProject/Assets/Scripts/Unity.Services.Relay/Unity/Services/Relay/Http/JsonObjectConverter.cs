using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Http
{
	[Preserve]
	internal class JsonObjectConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			JsonObject jsonObject = (JsonObject)value;
			if (jsonObject.obj == null)
			{
				writer.WriteNull();
			}
			else
			{
				JToken.FromObject(jsonObject.obj).WriteTo(writer);
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.Null)
			{
				if (reader.Value != null)
				{
					return new JsonObject(reader.Value);
				}
				try
				{
					return new JsonObject(JObject.Load(reader));
				}
				catch (JsonReaderException)
				{
					return new JsonObject(JArray.Load(reader));
				}
			}
			return new JsonObject(null);
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}
