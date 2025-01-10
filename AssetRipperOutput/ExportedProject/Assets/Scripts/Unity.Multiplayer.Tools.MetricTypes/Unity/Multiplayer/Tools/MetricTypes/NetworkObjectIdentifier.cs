using System;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	[Serializable]
	internal struct NetworkObjectIdentifier
	{
		public FixedString64Bytes Name { get; }

		public ulong NetworkId { get; }

		public NetworkObjectIdentifier(string name, ulong networkId)
			: this(StringConversionUtility.ConvertToFixedString(name), networkId)
		{
		}

		public NetworkObjectIdentifier(FixedString64Bytes name, ulong networkId)
		{
			Name = name;
			NetworkId = networkId;
		}
	}
}
