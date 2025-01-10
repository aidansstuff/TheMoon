using System.Collections.Generic;

namespace Unity.Services.Core
{
	public class InitializationOptions
	{
		internal IDictionary<string, object> Values { get; }

		public InitializationOptions()
			: this(new Dictionary<string, object>())
		{
		}

		internal InitializationOptions(IDictionary<string, object> values)
		{
			Values = values;
		}

		internal InitializationOptions(InitializationOptions source)
			: this(new Dictionary<string, object>(source.Values))
		{
		}

		public bool TryGetOption(string key, out bool option)
		{
			return TryGetOption<bool>(key, out option);
		}

		public bool TryGetOption(string key, out int option)
		{
			return TryGetOption<int>(key, out option);
		}

		public bool TryGetOption(string key, out float option)
		{
			return TryGetOption<float>(key, out option);
		}

		public bool TryGetOption(string key, out string option)
		{
			return TryGetOption<string>(key, out option);
		}

		private bool TryGetOption<T>(string key, out T option)
		{
			option = default(T);
			if (Values.TryGetValue(key, out var value) && value is T val)
			{
				option = val;
				return true;
			}
			return false;
		}

		public InitializationOptions SetOption(string key, bool value)
		{
			Values[key] = value;
			return this;
		}

		public InitializationOptions SetOption(string key, int value)
		{
			Values[key] = value;
			return this;
		}

		public InitializationOptions SetOption(string key, float value)
		{
			Values[key] = value;
			return this;
		}

		public InitializationOptions SetOption(string key, string value)
		{
			Values[key] = value;
			return this;
		}
	}
}
