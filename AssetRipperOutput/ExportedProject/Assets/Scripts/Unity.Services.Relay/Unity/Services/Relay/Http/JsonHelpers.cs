using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using UnityEngine;

namespace Unity.Services.Relay.Http
{
	internal static class JsonHelpers
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		internal static void RegisterTypesForAOT()
		{
			AotHelper.EnsureType<StringEnumConverter>();
			AotHelper.EnsureType<JsonObjectConverter>();
		}

		internal static bool TryParseJson<T>(this string @this, out T result)
		{
			bool success = true;
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				Error = delegate(object sender, ErrorEventArgs args)
				{
					success = false;
					args.ErrorContext.Handled = true;
				},
				MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
			result = JsonConvert.DeserializeObject<T>(@this, settings);
			return success;
		}
	}
}
