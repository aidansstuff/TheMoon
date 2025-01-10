using System.Text;
using Unity.Baselib.LowLevel;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

namespace Unity.Networking.Transport
{
	public struct NetworkEndPoint
	{
		private enum AddressType
		{
			Any = 0,
			Loopback = 1
		}

		private const int rawIpv4Length = 4;

		private const int rawIpv6Length = 16;

		private const int rawDataLength = 16;

		private const int rawLength = 24;

		private static readonly bool IsLittleEndian;

		internal Binding.Baselib_NetworkAddress rawNetworkAddress;

		public int Length => Family switch
		{
			NetworkFamily.Ipv4 => 4, 
			NetworkFamily.Ipv6 => 16, 
			_ => 0, 
		};

		public ushort Port
		{
			get
			{
				return (ushort)(rawNetworkAddress.port1 | (rawNetworkAddress.port0 << 8));
			}
			set
			{
				rawNetworkAddress.port0 = (byte)((uint)(value >> 8) & 0xFFu);
				rawNetworkAddress.port1 = (byte)(value & 0xFFu);
			}
		}

		public NetworkFamily Family
		{
			get
			{
				return FromBaselibFamily((Binding.Baselib_NetworkAddress_Family)rawNetworkAddress.family);
			}
			set
			{
				rawNetworkAddress.family = (byte)ToBaselibFamily(value);
			}
		}

		public unsafe ushort RawPort
		{
			get
			{
				ushort* ptr = (ushort*)((byte*)UnsafeUtility.AddressOf(ref rawNetworkAddress) + 16);
				return *ptr;
			}
			set
			{
				ushort* ptr = (ushort*)((byte*)UnsafeUtility.AddressOf(ref rawNetworkAddress) + 16);
				*ptr = value;
			}
		}

		public string Address => AddressAsString();

		public bool IsValid => Family != NetworkFamily.Invalid;

		public static NetworkEndPoint AnyIpv4 => CreateAddress(0);

		public static NetworkEndPoint LoopbackIpv4 => CreateAddress(0, AddressType.Loopback);

		public static NetworkEndPoint AnyIpv6 => CreateAddress(0, AddressType.Any, NetworkFamily.Ipv6);

		public static NetworkEndPoint LoopbackIpv6 => CreateAddress(0, AddressType.Loopback, NetworkFamily.Ipv6);

		public bool IsLoopback
		{
			get
			{
				if (!(this == LoopbackIpv4.WithPort(Port)))
				{
					return this == LoopbackIpv6.WithPort(Port);
				}
				return true;
			}
		}

		public bool IsAny
		{
			get
			{
				if (!(this == AnyIpv4.WithPort(Port)))
				{
					return this == AnyIpv6.WithPort(Port);
				}
				return true;
			}
		}

		unsafe static NetworkEndPoint()
		{
			IsLittleEndian = true;
			uint num = 1u;
			byte* ptr = (byte*)(&num);
			IsLittleEndian = *ptr == 1;
		}

		public unsafe NativeArray<byte> GetRawAddressBytes()
		{
			NativeArray<byte> nativeArray = new NativeArray<byte>(Length, Allocator.Temp);
			UnsafeUtility.MemCpy(nativeArray.GetUnsafePtr(), UnsafeUtility.AddressOf(ref rawNetworkAddress), Length);
			return nativeArray;
		}

		public unsafe void SetRawAddressBytes(NativeArray<byte> bytes, NetworkFamily family = NetworkFamily.Ipv4)
		{
			if ((family == NetworkFamily.Ipv4 && bytes.Length != 4) || (family == NetworkFamily.Ipv6 && bytes.Length != 16))
			{
				Debug.LogError("Bad input length for given address family.");
				return;
			}
			switch (family)
			{
			case NetworkFamily.Ipv4:
				UnsafeUtility.MemCpy(UnsafeUtility.AddressOf(ref rawNetworkAddress), bytes.GetUnsafeReadOnlyPtr(), 4L);
				Family = family;
				break;
			case NetworkFamily.Ipv6:
				UnsafeUtility.MemCpy(UnsafeUtility.AddressOf(ref rawNetworkAddress), bytes.GetUnsafeReadOnlyPtr(), 16L);
				Family = family;
				break;
			}
		}

		public NetworkEndPoint WithPort(ushort port)
		{
			NetworkEndPoint result = this;
			result.Port = port;
			return result;
		}

		public unsafe static bool TryParse(string address, ushort port, out NetworkEndPoint endpoint, NetworkFamily family = NetworkFamily.Ipv4)
		{
			UnsafeUtility.SizeOf<Binding.Baselib_NetworkAddress>();
			endpoint = default(NetworkEndPoint);
			char c = '\0';
			Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
			fixed (byte* ip = Encoding.UTF8.GetBytes(address + c))
			{
				fixed (Binding.Baselib_NetworkAddress* dstAddress = &endpoint.rawNetworkAddress)
				{
					Binding.Baselib_NetworkAddress_Encode(dstAddress, ToBaselibFamily(family), ip, port, &baselib_ErrorState);
				}
			}
			if (baselib_ErrorState.code != 0)
			{
				return false;
			}
			return endpoint.IsValid;
		}

		public static NetworkEndPoint Parse(string address, ushort port, NetworkFamily family = NetworkFamily.Ipv4)
		{
			if (TryParse(address, port, out var endpoint, family))
			{
				return endpoint;
			}
			return default(NetworkEndPoint);
		}

		public static bool operator ==(NetworkEndPoint lhs, NetworkEndPoint rhs)
		{
			return lhs.Compare(rhs);
		}

		public static bool operator !=(NetworkEndPoint lhs, NetworkEndPoint rhs)
		{
			return !lhs.Compare(rhs);
		}

		public override bool Equals(object other)
		{
			return this == (NetworkEndPoint)other;
		}

		public unsafe override int GetHashCode()
		{
			byte* ptr = (byte*)UnsafeUtility.AddressOf(ref rawNetworkAddress);
			int num = 0;
			for (int i = 0; i < 24; i++)
			{
				num = (num * 31) ^ ptr[i];
			}
			return num;
		}

		private unsafe bool Compare(NetworkEndPoint other)
		{
			byte* ptr = (byte*)UnsafeUtility.AddressOf(ref rawNetworkAddress);
			byte* ptr2 = (byte*)UnsafeUtility.AddressOf(ref other.rawNetworkAddress);
			return UnsafeUtility.MemCmp(ptr, ptr2, 24L) == 0;
		}

		internal static FixedString128Bytes AddressToString(ref Binding.Baselib_NetworkAddress rawNetworkAddress)
		{
			FixedString128Bytes fs = default(FixedString128Bytes);
			FixedString32Bytes input = ".";
			FixedString32Bytes input2 = ":";
			FixedString32Bytes input3 = "[";
			FixedString32Bytes input4 = "]";
			switch ((Binding.Baselib_NetworkAddress_Family)rawNetworkAddress.family)
			{
			case Binding.Baselib_NetworkAddress_Family.IPv4:
				FixedStringMethods.Append(ref fs, rawNetworkAddress.data0);
				FixedStringMethods.Append(ref fs, in input);
				FixedStringMethods.Append(ref fs, rawNetworkAddress.data1);
				FixedStringMethods.Append(ref fs, in input);
				FixedStringMethods.Append(ref fs, rawNetworkAddress.data2);
				FixedStringMethods.Append(ref fs, in input);
				FixedStringMethods.Append(ref fs, rawNetworkAddress.data3);
				FixedStringMethods.Append(ref fs, in input2);
				FixedStringMethods.Append(ref fs, (ushort)(rawNetworkAddress.port1 | (rawNetworkAddress.port0 << 8)));
				break;
			case Binding.Baselib_NetworkAddress_Family.IPv6:
				FixedStringMethods.Append(ref fs, in input3);
				FixedStringHexExt.AppendHex(ref fs, (ushort)(rawNetworkAddress.data1 | (rawNetworkAddress.data0 << 8)));
				FixedStringMethods.Append(ref fs, in input2);
				FixedStringHexExt.AppendHex(ref fs, (ushort)(rawNetworkAddress.data3 | (rawNetworkAddress.data2 << 8)));
				FixedStringMethods.Append(ref fs, in input2);
				FixedStringHexExt.AppendHex(ref fs, (ushort)(rawNetworkAddress.data5 | (rawNetworkAddress.data4 << 8)));
				FixedStringMethods.Append(ref fs, in input2);
				FixedStringHexExt.AppendHex(ref fs, (ushort)(rawNetworkAddress.data7 | (rawNetworkAddress.data6 << 8)));
				FixedStringMethods.Append(ref fs, in input2);
				FixedStringHexExt.AppendHex(ref fs, (ushort)(rawNetworkAddress.data9 | (rawNetworkAddress.data8 << 8)));
				FixedStringMethods.Append(ref fs, in input2);
				FixedStringHexExt.AppendHex(ref fs, (ushort)(rawNetworkAddress.data11 | (rawNetworkAddress.data10 << 8)));
				FixedStringMethods.Append(ref fs, in input2);
				FixedStringHexExt.AppendHex(ref fs, (ushort)(rawNetworkAddress.data13 | (rawNetworkAddress.data12 << 8)));
				FixedStringMethods.Append(ref fs, in input2);
				FixedStringHexExt.AppendHex(ref fs, (ushort)(rawNetworkAddress.data15 | (rawNetworkAddress.data14 << 8)));
				FixedStringMethods.Append(ref fs, in input2);
				FixedStringMethods.Append(ref fs, in input4);
				FixedStringMethods.Append(ref fs, in input2);
				FixedStringMethods.Append(ref fs, (ushort)(rawNetworkAddress.port1 | (rawNetworkAddress.port0 << 8)));
				break;
			}
			return fs;
		}

		private string AddressAsString()
		{
			return AddressToString(ref rawNetworkAddress).ToString();
		}

		public override string ToString()
		{
			return AddressToString(ref rawNetworkAddress).ToString();
		}

		private static ushort ByteSwap(ushort val)
		{
			return (ushort)(((val & 0xFF) << 8) | (val >> 8));
		}

		private static uint ByteSwap(uint val)
		{
			return ((val & 0xFF) << 24) | ((val & 0xFF00) << 8) | ((val >> 8) & 0xFF00u) | (val >> 24);
		}

		private unsafe static NetworkEndPoint CreateAddress(ushort port, AddressType type = AddressType.Any, NetworkFamily family = NetworkFamily.Ipv4)
		{
			if (family == NetworkFamily.Invalid)
			{
				return default(NetworkEndPoint);
			}
			uint num = 2130706433u;
			if (IsLittleEndian)
			{
				port = ByteSwap(port);
				num = ByteSwap(num);
			}
			NetworkEndPoint networkEndPoint = default(NetworkEndPoint);
			networkEndPoint.Family = family;
			networkEndPoint.RawPort = port;
			NetworkEndPoint result = networkEndPoint;
			if (type == AddressType.Loopback)
			{
				switch (family)
				{
				case NetworkFamily.Ipv4:
					*(uint*)UnsafeUtility.AddressOf(ref result.rawNetworkAddress) = num;
					break;
				case NetworkFamily.Ipv6:
					result.rawNetworkAddress.data15 = 1;
					break;
				}
			}
			return result;
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
