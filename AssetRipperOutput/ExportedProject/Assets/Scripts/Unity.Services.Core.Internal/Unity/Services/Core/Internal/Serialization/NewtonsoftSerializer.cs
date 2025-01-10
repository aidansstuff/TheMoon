using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Unity.Services.Core.Internal.Serialization
{
	internal class NewtonsoftSerializer : IJsonSerializer
	{
		private readonly JsonSerializer m_Serializer;

		public NewtonsoftSerializer(JsonSerializerSettings settings = null)
			: this(JsonSerializer.Create(settings))
		{
		}

		internal NewtonsoftSerializer(JsonSerializer serializer)
		{
			m_Serializer = serializer;
		}

		public string SerializeObject<T>(T value)
		{
			using StringWriter stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
			using JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter);
			jsonTextWriter.Formatting = m_Serializer.Formatting;
			m_Serializer.Serialize(jsonTextWriter, value, typeof(T));
			return stringWriter.ToString();
		}

		public T DeserializeObject<T>(string value)
		{
			using JsonTextReader reader = new JsonTextReader(new StringReader(value));
			return (T)m_Serializer.Deserialize(reader, typeof(T));
		}
	}
}
