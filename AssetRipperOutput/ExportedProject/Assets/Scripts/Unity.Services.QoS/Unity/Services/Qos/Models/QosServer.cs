using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Qos.Models
{
	[Preserve]
	[DataContract(Name = "QosServer")]
	internal class QosServer
	{
		[Preserve]
		[DataMember(Name = "endpoints", IsRequired = true, EmitDefaultValue = true)]
		public List<string> Endpoints { get; }

		[Preserve]
		[DataMember(Name = "region", IsRequired = true, EmitDefaultValue = true)]
		public string Region { get; }

		[Preserve]
		[DataMember(Name = "services", EmitDefaultValue = false)]
		public List<string> Services { get; }

		[Preserve]
		public QosServer(List<string> endpoints, string region, List<string> services = null)
		{
			Endpoints = endpoints;
			Region = region;
			Services = services;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (Endpoints != null)
			{
				text = text + "endpoints," + Endpoints.ToString() + ",";
			}
			if (Region != null)
			{
				text = text + "region," + Region + ",";
			}
			if (Services != null)
			{
				text = text + "services," + Services.ToString();
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (Endpoints != null)
			{
				string value = Endpoints.ToString();
				dictionary.Add("endpoints", value);
			}
			if (Region != null)
			{
				string value2 = Region.ToString();
				dictionary.Add("region", value2);
			}
			if (Services != null)
			{
				string value3 = Services.ToString();
				dictionary.Add("services", value3);
			}
			return dictionary;
		}
	}
}
