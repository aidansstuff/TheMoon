using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "ResponseLinks")]
	public class ResponseLinks
	{
		[Preserve]
		[DataMember(Name = "next", EmitDefaultValue = false)]
		public string Next { get; }

		[Preserve]
		public ResponseLinks(string next = null)
		{
			Next = next;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (Next != null)
			{
				text = text + "next," + Next;
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (Next != null)
			{
				string value = Next.ToString();
				dictionary.Add("next", value);
			}
			return dictionary;
		}
	}
}
