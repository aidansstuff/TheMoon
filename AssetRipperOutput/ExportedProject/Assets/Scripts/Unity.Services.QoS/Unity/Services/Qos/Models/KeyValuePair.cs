using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Qos.Models
{
	[Preserve]
	[DataContract(Name = "KeyValuePair")]
	internal class KeyValuePair
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
	}
}
