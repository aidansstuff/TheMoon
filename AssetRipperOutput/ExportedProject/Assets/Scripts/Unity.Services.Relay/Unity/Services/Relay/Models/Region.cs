using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "Region")]
	public class Region
	{
		[Preserve]
		[DataMember(Name = "id", IsRequired = true, EmitDefaultValue = true)]
		public string Id { get; }

		[Preserve]
		[DataMember(Name = "description", IsRequired = true, EmitDefaultValue = true)]
		public string Description { get; }

		[Preserve]
		public Region(string id, string description)
		{
			Id = id;
			Description = description;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (Id != null)
			{
				text = text + "id," + Id + ",";
			}
			if (Description != null)
			{
				text = text + "description," + Description;
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (Id != null)
			{
				string value = Id.ToString();
				dictionary.Add("id", value);
			}
			if (Description != null)
			{
				string value2 = Description.ToString();
				dictionary.Add("description", value2);
			}
			return dictionary;
		}
	}
}
