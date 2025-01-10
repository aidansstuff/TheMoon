using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "Allocation")]
	public class Allocation
	{
		[Preserve]
		[DataMember(Name = "allocationId", IsRequired = true, EmitDefaultValue = true)]
		public Guid AllocationId { get; }

		[Preserve]
		[DataMember(Name = "serverEndpoints", IsRequired = true, EmitDefaultValue = true)]
		public List<RelayServerEndpoint> ServerEndpoints { get; }

		[Preserve]
		[DataMember(Name = "relayServer", IsRequired = true, EmitDefaultValue = true)]
		public RelayServer RelayServer { get; }

		[Preserve]
		[DataMember(Name = "key", IsRequired = true, EmitDefaultValue = true)]
		public byte[] Key { get; }

		[Preserve]
		[DataMember(Name = "connectionData", IsRequired = true, EmitDefaultValue = true)]
		public byte[] ConnectionData { get; }

		[Preserve]
		[DataMember(Name = "allocationIdBytes", IsRequired = true, EmitDefaultValue = true)]
		public byte[] AllocationIdBytes { get; }

		[Preserve]
		[DataMember(Name = "region", IsRequired = true, EmitDefaultValue = true)]
		public string Region { get; }

		[Preserve]
		public Allocation(Guid allocationId, List<RelayServerEndpoint> serverEndpoints, RelayServer relayServer, byte[] key, byte[] connectionData, byte[] allocationIdBytes, string region)
		{
			AllocationId = allocationId;
			ServerEndpoints = serverEndpoints;
			RelayServer = relayServer;
			Key = key;
			ConnectionData = connectionData;
			AllocationIdBytes = allocationIdBytes;
			Region = region;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			_ = AllocationId;
			text = text + "allocationId," + AllocationId.ToString() + ",";
			if (ServerEndpoints != null)
			{
				text = text + "serverEndpoints," + ServerEndpoints.ToString() + ",";
			}
			if (RelayServer != null)
			{
				text = text + "relayServer," + RelayServer.ToString() + ",";
			}
			if (Key != null)
			{
				text = text + "key," + Key.ToString() + ",";
			}
			if (ConnectionData != null)
			{
				text = text + "connectionData," + ConnectionData.ToString() + ",";
			}
			if (AllocationIdBytes != null)
			{
				text = text + "allocationIdBytes," + AllocationIdBytes.ToString() + ",";
			}
			if (Region != null)
			{
				text = text + "region," + Region;
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			_ = AllocationId;
			string value = AllocationId.ToString();
			dictionary.Add("allocationId", value);
			if (Key != null)
			{
				string value2 = Key.ToString();
				dictionary.Add("key", value2);
			}
			if (ConnectionData != null)
			{
				string value3 = ConnectionData.ToString();
				dictionary.Add("connectionData", value3);
			}
			if (AllocationIdBytes != null)
			{
				string value4 = AllocationIdBytes.ToString();
				dictionary.Add("allocationIdBytes", value4);
			}
			if (Region != null)
			{
				string value5 = Region.ToString();
				dictionary.Add("region", value5);
			}
			return dictionary;
		}
	}
}
