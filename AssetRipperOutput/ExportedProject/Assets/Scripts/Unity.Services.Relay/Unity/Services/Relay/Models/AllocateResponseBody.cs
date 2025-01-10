using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "AllocateResponseBody")]
	public class AllocateResponseBody
	{
		[Preserve]
		[DataMember(Name = "meta", IsRequired = true, EmitDefaultValue = true)]
		public ResponseMeta Meta { get; }

		[Preserve]
		[DataMember(Name = "links", EmitDefaultValue = false)]
		public ResponseLinks Links { get; }

		[Preserve]
		[DataMember(Name = "data", IsRequired = true, EmitDefaultValue = true)]
		public AllocationData Data { get; }

		[Preserve]
		public AllocateResponseBody(ResponseMeta meta, AllocationData data, ResponseLinks links = null)
		{
			Meta = meta;
			Links = links;
			Data = data;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (Meta != null)
			{
				text = text + "meta," + Meta.ToString() + ",";
			}
			if (Links != null)
			{
				text = text + "links," + Links.ToString() + ",";
			}
			if (Data != null)
			{
				text = text + "data," + Data.ToString();
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			return new Dictionary<string, string>();
		}
	}
}
