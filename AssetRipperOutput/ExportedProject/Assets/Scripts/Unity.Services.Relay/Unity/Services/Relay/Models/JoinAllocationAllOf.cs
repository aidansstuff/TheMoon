using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "JoinAllocation_allOf")]
	public class JoinAllocationAllOf
	{
		[Preserve]
		[DataMember(Name = "hostConnectionData", IsRequired = true, EmitDefaultValue = true)]
		public byte[] HostConnectionData { get; }

		[Preserve]
		public JoinAllocationAllOf(byte[] hostConnectionData)
		{
			HostConnectionData = hostConnectionData;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (HostConnectionData != null)
			{
				text = text + "hostConnectionData," + HostConnectionData.ToString();
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (HostConnectionData != null)
			{
				string value = HostConnectionData.ToString();
				dictionary.Add("hostConnectionData", value);
			}
			return dictionary;
		}
	}
}
