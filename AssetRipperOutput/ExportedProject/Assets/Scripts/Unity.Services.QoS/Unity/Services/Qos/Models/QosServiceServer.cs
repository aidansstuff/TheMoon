using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Qos.Models
{
	[Preserve]
	[DataContract(Name = "QosServiceServer")]
	internal class QosServiceServer
	{
		[Preserve]
		[DataMember(Name = "endpoints", IsRequired = true, EmitDefaultValue = true)]
		public List<string> Endpoints { get; }

		[Preserve]
		[DataMember(Name = "region", IsRequired = true, EmitDefaultValue = true)]
		public string Region { get; }

		[Preserve]
		[DataMember(Name = "annotations", EmitDefaultValue = false)]
		public Dictionary<string, List<string>> Annotations { get; }

		[Preserve]
		public QosServiceServer(List<string> endpoints, string region, Dictionary<string, List<string>> annotations = null)
		{
			Endpoints = endpoints;
			Region = region;
			Annotations = annotations;
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
			if (Annotations != null)
			{
				text = text + "annotations," + Annotations.ToString();
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
			if (Annotations != null)
			{
				string value3 = Annotations.ToString();
				dictionary.Add("annotations", value3);
			}
			return dictionary;
		}
	}
}
