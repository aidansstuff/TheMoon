using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "RegionsData")]
	public class RegionsData
	{
		[Preserve]
		[DataMember(Name = "regions", IsRequired = true, EmitDefaultValue = true)]
		public List<Region> Regions { get; }

		[Preserve]
		public RegionsData(List<Region> regions)
		{
			Regions = regions;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (Regions != null)
			{
				text = text + "regions," + Regions.ToString();
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			return new Dictionary<string, string>();
		}
	}
}
