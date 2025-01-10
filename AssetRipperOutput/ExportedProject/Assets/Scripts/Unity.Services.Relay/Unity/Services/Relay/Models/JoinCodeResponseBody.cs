using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "JoinCodeResponseBody")]
	public class JoinCodeResponseBody
	{
		[Preserve]
		[DataMember(Name = "meta", IsRequired = true, EmitDefaultValue = true)]
		public ResponseMeta Meta { get; }

		[Preserve]
		[DataMember(Name = "links", IsRequired = true, EmitDefaultValue = true)]
		public ResponseLinks Links { get; }

		[Preserve]
		[DataMember(Name = "data", IsRequired = true, EmitDefaultValue = true)]
		public JoinCodeData Data { get; }

		[Preserve]
		public JoinCodeResponseBody(ResponseMeta meta, ResponseLinks links, JoinCodeData data)
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
