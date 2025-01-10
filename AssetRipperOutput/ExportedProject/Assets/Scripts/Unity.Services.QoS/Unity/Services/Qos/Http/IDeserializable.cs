using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Qos.Http
{
	[Preserve]
	[JsonConverter(typeof(JsonObjectConverter))]
	internal interface IDeserializable
	{
		string GetAsString();

		T GetAs<T>(DeserializationSettings deserializationSettings = null);
	}
}
