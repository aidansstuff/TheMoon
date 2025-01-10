using System.Text;
using Unity.Baselib.LowLevel;

namespace Unity.Networking.QoS
{
	internal struct NetworkEndPoint
	{
		internal Binding.Baselib_NetworkAddress rawNetworkAddress;

		private ushort Port => (ushort)(rawNetworkAddress.port1 | (rawNetworkAddress.port0 << 8));

		private NetworkFamily Family => FromBaselibFamily((Binding.Baselib_NetworkAddress_Family)rawNetworkAddress.family);

		internal string Address => AddressAsString();

		private bool IsValid => Family != NetworkFamily.Invalid;

		internal unsafe static bool TryParse(string address, ushort port, out NetworkEndPoint endpoint, NetworkFamily family = NetworkFamily.Ipv4)
		{
			endpoint = default(NetworkEndPoint);
			Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
			fixed (byte* ip = Encoding.UTF8.GetBytes(address + "\0"))
			{
				fixed (Binding.Baselib_NetworkAddress* dstAddress = &endpoint.rawNetworkAddress)
				{
					Binding.Baselib_NetworkAddress_Encode(dstAddress, ToBaselibFamily(family), ip, port, &baselib_ErrorState);
				}
			}
			if (baselib_ErrorState.code == Binding.Baselib_ErrorCode.Success)
			{
				return endpoint.IsValid;
			}
			return false;
		}

		private string AddressAsString()
		{
			return Family switch
			{
				NetworkFamily.Ipv4 => $"{rawNetworkAddress.data0}.{rawNetworkAddress.data1}.{rawNetworkAddress.data2}.{rawNetworkAddress.data3}:{Port}", 
				NetworkFamily.Ipv6 => $"[{rawNetworkAddress.data1 | (rawNetworkAddress.data0 << 8):x}:{rawNetworkAddress.data3 | (rawNetworkAddress.data2 << 8):x}:{rawNetworkAddress.data5 | (rawNetworkAddress.data4 << 8):x}:{rawNetworkAddress.data7 | (rawNetworkAddress.data6 << 8):x}:{rawNetworkAddress.data9 | (rawNetworkAddress.data8 << 8):x}:{rawNetworkAddress.data11 | (rawNetworkAddress.data10 << 8):x}:{rawNetworkAddress.data13 | (rawNetworkAddress.data12 << 8):x}:{rawNetworkAddress.data15 | (rawNetworkAddress.data14 << 8):x}]:{Port}", 
				_ => string.Empty, 
			};
		}

		private static NetworkFamily FromBaselibFamily(Binding.Baselib_NetworkAddress_Family family)
		{
			return family switch
			{
				Binding.Baselib_NetworkAddress_Family.IPv4 => NetworkFamily.Ipv4, 
				Binding.Baselib_NetworkAddress_Family.IPv6 => NetworkFamily.Ipv6, 
				_ => NetworkFamily.Invalid, 
			};
		}

		private static Binding.Baselib_NetworkAddress_Family ToBaselibFamily(NetworkFamily family)
		{
			return family switch
			{
				NetworkFamily.Ipv4 => Binding.Baselib_NetworkAddress_Family.IPv4, 
				NetworkFamily.Ipv6 => Binding.Baselib_NetworkAddress_Family.IPv6, 
				_ => Binding.Baselib_NetworkAddress_Family.Invalid, 
			};
		}
	}
}
