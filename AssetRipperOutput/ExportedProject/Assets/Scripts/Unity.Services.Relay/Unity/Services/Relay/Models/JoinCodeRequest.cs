using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "JoinCodeRequest")]
	public class JoinCodeRequest
	{
		[Preserve]
		[DataMember(Name = "allocationId", IsRequired = true, EmitDefaultValue = true)]
		public Guid AllocationId { get; }

		[Preserve]
		public JoinCodeRequest(Guid allocationId)
		{
			AllocationId = allocationId;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			_ = AllocationId;
			return text + "allocationId," + AllocationId.ToString();
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			_ = AllocationId;
			string value = AllocationId.ToString();
			dictionary.Add("allocationId", value);
			return dictionary;
		}
	}
}
