using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "JoinResponseBody")]
	public class JoinResponseBody
	{
		[Preserve]
		[DataMember(Name = "meta", IsRequired = true, EmitDefaultValue = true)]
		public ResponseMeta Meta { get; }

		[Preserve]
		[DataMember(Name = "data", IsRequired = true, EmitDefaultValue = true)]
		public JoinData Data { get; }

		[Preserve]
		public JoinResponseBody(ResponseMeta meta, JoinData data)
		{
			Meta = meta;
			Data = data;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (Meta != null)
			{
				text = text + "meta," + Meta.ToString() + ",";
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
