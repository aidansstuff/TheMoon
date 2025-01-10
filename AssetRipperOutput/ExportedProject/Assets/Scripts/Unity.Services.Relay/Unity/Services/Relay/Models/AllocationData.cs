using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "AllocationData")]
	public class AllocationData
	{
		[Preserve]
		[DataMember(Name = "allocation", IsRequired = true, EmitDefaultValue = true)]
		public Allocation Allocation { get; }

		[Preserve]
		public AllocationData(Allocation allocation)
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
