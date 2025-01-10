using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "ResponseMeta")]
	public class ResponseMeta
	{
		[Preserve]
		[DataMember(Name = "requestId", IsRequired = true, EmitDefaultValue = true)]
		public string RequestId { get; }

		[Preserve]
		[DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
		public int Status { get; }

		[Preserve]
		public ResponseMeta(string requestId, int status)
		{
			RequestId = requestId;
			Status = status;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (RequestId != null)
			{
				text = text + "requestId," + RequestId + ",";
			}
			return text + "status," + Status;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (RequestId != null)
			{
				string value = RequestId.ToString();
				dictionary.Add("requestId", value);
			}
			string value2 = Status.ToString();
			dictionary.Add("status", value2);
			return dictionary;
		}
	}
}
