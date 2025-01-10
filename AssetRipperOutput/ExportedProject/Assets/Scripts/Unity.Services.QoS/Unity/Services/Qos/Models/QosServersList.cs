using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Qos.Models
{
	[Preserve]
	[DataContract(Name = "QosServersList")]
	internal class QosServersList
	{
		[Preserve]
		[DataMember(Name = "servers", IsRequired = true, EmitDefaultValue = true)]
		public List<QosServer> Servers { get; }

		[Preserve]
		public QosServersList(List<QosServer> servers)
		{
			Servers = servers;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (Servers != null)
			{
				text = text + "servers," + Servers.ToString();
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			return new Dictionary<string, string>();
		}
	}
}
