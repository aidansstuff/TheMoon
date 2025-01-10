using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "RelayServer")]
	public class RelayServer
	{
		[Preserve]
		[DataMember(Name = "ipV4", IsRequired = true, EmitDefaultValue = true)]
		public string IpV4 { get; }

		[Preserve]
		[DataMember(Name = "port", IsRequired = true, EmitDefaultValue = true)]
		public int Port { get; }

		[Preserve]
		public RelayServer(string ipV4, int port)
		{
			IpV4 = ipV4;
			Port = port;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (IpV4 != null)
			{
				text = text + "ipV4," + IpV4 + ",";
			}
			return text + "port," + Port;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (IpV4 != null)
			{
				string value = IpV4.ToString();
				dictionary.Add("ipV4", value);
			}
			string value2 = Port.ToString();
			dictionary.Add("port", value2);
			return dictionary;
		}
	}
}
