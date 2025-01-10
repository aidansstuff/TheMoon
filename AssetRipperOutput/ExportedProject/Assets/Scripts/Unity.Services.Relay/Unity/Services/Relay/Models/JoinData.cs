using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "JoinData")]
	public class JoinData
	{
		[Preserve]
		[DataMember(Name = "allocation", IsRequired = true, EmitDefaultValue = true)]
		public JoinAllocation Allocation { get; }

		[Preserve]
		public JoinData(JoinAllocation allocation)
		{
			Allocation = allocation;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (Allocation != null)
			{
				text = text + "allocation," + Allocation.ToString();
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			return new Dictionary<string, string>();
		}
	}
}
