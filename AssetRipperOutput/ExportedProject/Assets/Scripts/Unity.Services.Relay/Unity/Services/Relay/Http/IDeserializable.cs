using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Http
{
	[Preserve]
	[JsonConverter(typeof(JsonObjectConverter))]
	internal interface IDeserializable
	{
		string GetAsString();

		T GetAs<T>(DeserializationSettings deserializationSettings = null);
	}
}
