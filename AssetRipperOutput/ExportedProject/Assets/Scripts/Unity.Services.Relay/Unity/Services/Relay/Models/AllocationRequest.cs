using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "AllocationRequest")]
	public class AllocationRequest
	{
		[Preserve]
		[DataMember(Name = "maxConnections", IsRequired = true, EmitDefaultValue = true)]
		public int MaxConnections { get; }

		[Preserve]
		[DataMember(Name = "region", EmitDefaultValue = false)]
		public string Region { get; }

		[Preserve]
		public AllocationRequest(int maxConnections, string region = null)
		{
			MaxConnections = maxConnections;
			Region = region;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			text = text + "maxConnections," + MaxConnections + ",";
			if (Region != null)
			{
				text = text + "region," + Region;
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string value = MaxConnections.ToString();
			dictionary.Add("maxConnections", value);
			if (Region != null)
			{
				string value2 = Region.ToString();
				dictionary.Add("region", value2);
			}
			return dictionary;
		}
	}
}
