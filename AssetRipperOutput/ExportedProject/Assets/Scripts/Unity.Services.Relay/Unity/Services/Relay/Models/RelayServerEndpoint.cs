using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "RelayServerEndpoint")]
	public class RelayServerEndpoint
	{
		[Preserve]
		[JsonConverter(typeof(StringEnumConverter))]
		public enum NetworkOptions
		{
			[EnumMember(Value = "udp")]
			Udp = 1,
			[EnumMember(Value = "tcp")]
			Tcp = 2
		}

		[Preserve]
		[DataMember(Name = "connectionType", IsRequired = true, EmitDefaultValue = true)]
		public string ConnectionType { get; }

		[Preserve]
		[JsonConverter(typeof(StringEnumConverter))]
		[DataMember(Name = "network", IsRequired = true, EmitDefaultValue = true)]
		public NetworkOptions Network { get; }

		[Preserve]
		[DataMember(Name = "reliable", IsRequired = true, EmitDefaultValue = true)]
		public bool Reliable { get; }

		[Preserve]
		[DataMember(Name = "secure", IsRequired = true, EmitDefaultValue = true)]
		public bool Secure { get; }

		[Preserve]
		[DataMember(Name = "host", IsRequired = true, EmitDefaultValue = true)]
		public string Host { get; }

		[Preserve]
		[DataMember(Name = "port", IsRequired = true, EmitDefaultValue = true)]
		public int Port { get; }

		[Preserve]
		public RelayServerEndpoint(string connectionType, NetworkOptions network, bool reliable, bool secure, string host, int port)
		{
			ConnectionType = connectionType;
			Network = network;
			Reliable = reliable;
			Secure = secure;
			Host = host;
			Port = port;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (ConnectionType != null)
			{
				text = text + "connectionType," + ConnectionType + ",";
			}
			text = text + "network," + Network.ToString() + ",";
			text = text + "reliable," + Reliable + ",";
			text = text + "secure," + Secure + ",";
			if (Host != null)
			{
				text = text + "host," + Host + ",";
			}
			return text + "port," + Port;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (ConnectionType != null)
			{
				string value = ConnectionType.ToString();
				dictionary.Add("connectionType", value);
			}
			string value2 = Network.ToString();
			dictionary.Add("network", value2);
			string value3 = Reliable.ToString();
			dictionary.Add("reliable", value3);
			string value4 = Secure.ToString();
			dictionary.Add("secure", value4);
			if (Host != null)
			{
				string value5 = Host.ToString();
				dictionary.Add("host", value5);
			}
			string value6 = Port.ToString();
			dictionary.Add("port", value6);
			return dictionary;
		}
	}
}
