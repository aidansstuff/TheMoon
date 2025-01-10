using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Configuration
{
	internal static class ConfigurationCollectionHelper
	{
		public static void FillWith(this IDictionary<string, ConfigurationEntry> self, SerializableProjectConfiguration config)
		{
			for (int i = 0; i < config.Keys.Length; i++)
			{
				string key = config.Keys[i];
				ConfigurationEntry entry = config.Values[i];
				self.SetOrCreateEntry(key, entry);
			}
		}

		public static void FillWith(this IDictionary<string, ConfigurationEntry> self, InitializationOptions options)
		{
			foreach (KeyValuePair<string, object> value in options.Values)
			{
				string text = Convert.ToString(value.Value, CultureInfo.InvariantCulture);
				self.SetOrCreateEntry(value.Key, text);
			}
		}

		private static void SetOrCreateEntry(this IDictionary<string, ConfigurationEntry> self, string key, ConfigurationEntry entry)
		{
			if (self.TryGetValue(key, out var value))
			{
				if (!value.TrySetValue(entry))
				{
					CoreLogger.LogWarning("You are attempting to initialize Operate Solution SDK with an option \"" + key + "\" which is readonly at runtime and can be modified only through Project Settings. The value provided as initialization option will be ignored. Please update InitializationOptions in order to remove this warning.");
				}
			}
			else
			{
				self[key] = entry;
			}
		}
	}
}
