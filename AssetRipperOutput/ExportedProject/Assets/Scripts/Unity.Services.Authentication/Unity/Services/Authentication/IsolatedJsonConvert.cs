using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Unity.Services.Authentication
{
	internal static class IsolatedJsonConvert
	{
		[DebuggerStepThrough]
		public static string SerializeObject(object value)
		{
			return SerializeObject(value, (Type)null, (JsonSerializerSettings)null);
		}

		[DebuggerStepThrough]
		public static string SerializeObject(object value, Formatting formatting)
		{
			return SerializeObject(value, formatting, (JsonSerializerSettings)null);
		}

		[DebuggerStepThrough]
		public static string SerializeObject(object value, params JsonConverter[] converters)
		{
			JsonSerializerSettings jsonSerializerSettings = ((converters != null && converters.Length != 0) ? new JsonSerializerSettings
			{
				Converters = converters
			} : null);
			JsonSerializerSettings settings = jsonSerializerSettings;
			return SerializeObject(value, null, settings);
		}

		[DebuggerStepThrough]
		public static string SerializeObject(object value, Formatting formatting, params JsonConverter[] converters)
		{
			JsonSerializerSettings jsonSerializerSettings = ((converters != null && converters.Length != 0) ? new JsonSerializerSettings
			{
				Converters = converters
			} : null);
			JsonSerializerSettings settings = jsonSerializerSettings;
			return SerializeObject(value, null, formatting, settings);
		}

		[DebuggerStepThrough]
		public static string SerializeObject(object value, JsonSerializerSettings settings)
		{
			return SerializeObject(value, null, settings);
		}

		[DebuggerStepThrough]
		public static string SerializeObject(object value, Type type, JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.Create(settings);
			return SerializeObjectInternal(value, type, jsonSerializer);
		}

		[DebuggerStepThrough]
		public static string SerializeObject(object value, Formatting formatting, JsonSerializerSettings settings)
		{
			return SerializeObject(value, null, formatting, settings);
		}

		[DebuggerStepThrough]
		public static string SerializeObject(object value, Type type, Formatting formatting, JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.Create(settings);
			jsonSerializer.Formatting = formatting;
			return SerializeObjectInternal(value, type, jsonSerializer);
		}

		private static string SerializeObjectInternal(object value, Type type, JsonSerializer jsonSerializer)
		{
			StringWriter stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.Formatting = jsonSerializer.Formatting;
				jsonSerializer.Serialize(jsonTextWriter, value, type);
			}
			return stringWriter.ToString();
		}

		[DebuggerStepThrough]
		public static object DeserializeObject(string value)
		{
			return DeserializeObject(value, (Type)null, (JsonSerializerSettings)null);
		}

		[DebuggerStepThrough]
		public static object DeserializeObject(string value, JsonSerializerSettings settings)
		{
			return DeserializeObject(value, null, settings);
		}

		[DebuggerStepThrough]
		public static object DeserializeObject(string value, Type type)
		{
			return DeserializeObject(value, type, (JsonSerializerSettings)null);
		}

		[DebuggerStepThrough]
		public static T DeserializeObject<T>(string value)
		{
			return IsolatedJsonConvert.DeserializeObject<T>(value, (JsonSerializerSettings)null);
		}

		[DebuggerStepThrough]
		public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject)
		{
			return DeserializeObject<T>(value);
		}

		[DebuggerStepThrough]
		public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject, JsonSerializerSettings settings)
		{
			return DeserializeObject<T>(value, settings);
		}

		[DebuggerStepThrough]
		public static T DeserializeObject<T>(string value, params JsonConverter[] converters)
		{
			return (T)DeserializeObject(value, typeof(T), converters);
		}

		[DebuggerStepThrough]
		public static T DeserializeObject<T>(string value, JsonSerializerSettings settings)
		{
			return (T)DeserializeObject(value, typeof(T), settings);
		}

		[DebuggerStepThrough]
		public static object DeserializeObject(string value, Type type, params JsonConverter[] converters)
		{
			JsonSerializerSettings jsonSerializerSettings = ((converters != null && converters.Length != 0) ? new JsonSerializerSettings
			{
				Converters = converters
			} : null);
			JsonSerializerSettings settings = jsonSerializerSettings;
			return DeserializeObject(value, type, settings);
		}

		public static object DeserializeObject(string value, Type type, JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.Create(settings);
			using JsonTextReader reader = new JsonTextReader(new StringReader(value));
			return jsonSerializer.Deserialize(reader, type);
		}

		[DebuggerStepThrough]
		public static void PopulateObject(string value, object target)
		{
			PopulateObject(value, target, null);
		}

		public static void PopulateObject(string value, object target, JsonSerializerSettings settings)
		{
			using JsonReader jsonReader = new JsonTextReader(new StringReader(value));
			JsonSerializer.Create(settings).Populate(jsonReader, target);
			if (settings == null || !settings.CheckAdditionalContent)
			{
				return;
			}
			while (jsonReader.Read())
			{
				if (jsonReader.TokenType != JsonToken.Comment)
				{
					throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
				}
			}
		}
	}
}
