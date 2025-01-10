using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "JoinRequest")]
	public class JoinRequest
	{
		[Preserve]
		[DataMember(Name = "joinCode", IsRequired = true, EmitDefaultValue = true)]
		public string JoinCode { get; }

		[Preserve]
		public JoinRequest(string joinCode)
		{
			JoinCode = joinCode;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (JoinCode != null)
			{
				text = text + "joinCode," + JoinCode;
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (JoinCode != null)
			{
				string value = JoinCode.ToString();
				dictionary.Add("joinCode", value);
			}
			return dictionary;
		}
	}
}
