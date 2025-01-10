using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "KeyValuePair")]
	public class KeyValuePair
	{
		[Preserve]
		[DataMember(Name = "key", IsRequired = true, EmitDefaultValue = true)]
		public string Key { get; }

		[Preserve]
		[DataMember(Name = "value", IsRequired = true, EmitDefaultValue = true)]
		public string Value { get; }

		[Preserve]
		public KeyValuePair(string key, string value)
		{
			Key = key;
			Value = value;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (Key != null)
			{
				text = text + "key," + Key + ",";
			}
			if (Value != null)
			{
				text = text + "value," + Value;
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (Key != null)
			{
				string value = Key.ToString();
				dictionary.Add("key", value);
			}
			if (Value != null)
			{
				string value2 = Value.ToString();
				dictionary.Add("value", value2);
			}
			return dictionary;
		}
	}
}
