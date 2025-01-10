using System;
using System.Text;

namespace Steamworks.Data
{
	public struct Connection
	{
		public uint Id { get; set; }

		public long UserData
		{
			get
			{
				return SteamNetworkingSockets.Internal.GetConnectionUserData(this);
			}
			set
			{
				SteamNetworkingSockets.Internal.SetConnectionUserData(this, value);
			}
		}

		public string ConnectionName
		{
			get
			{
				if (!SteamNetworkingSockets.Internal.GetConnectionName(this, out var pszName))
				{
					return "ERROR";
				}
				return pszName;
			}
			set
			{
				SteamNetworkingSockets.Internal.SetConnectionName(this, value);
			}
		}

		public override string ToString()
		{
			return Id.ToString();
		}

		public static implicit operator Connection(uint value)
		{
			Connection result = default(Connection);
			result.Id = value;
			return result;
		}

		public static implicit operator uint(Connection value)
		{
			return value.Id;
		}

		public Result Accept()
		{
			return SteamNetworkingSockets.Internal.AcceptConnection(this);
		}

		public bool Close(bool linger = false, int reasonCode = 0, string debugString = "Closing Connection")
		{
			return SteamNetworkingSockets.Internal.CloseConnection(this, reasonCode, debugString, linger);
		}

		public Result SendMessage(IntPtr ptr, int size, SendType sendType = SendType.Reliable)
		{
			long pOutMessageNumber = 0L;
			return SteamNetworkingSockets.Internal.SendMessageToConnection(this, ptr, (uint)size, (int)sendType, ref pOutMessageNumber);
		}

		public unsafe Result SendMessage(byte[] data, SendType sendType = SendType.Reliable)
		{
			fixed (byte* ptr = data)
			{
				return SendMessage((IntPtr)ptr, data.Length, sendType);
			}
		}

		public unsafe Result SendMessage(byte[] data, int offset, int length, SendType sendType = SendType.Reliable)
		{
			fixed (byte* ptr = data)
			{
				return SendMessage((IntPtr)ptr + offset, length, sendType);
			}
		}

		public Result SendMessage(string str, SendType sendType = SendType.Reliable)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			return SendMessage(bytes, sendType);
		}

		public Result Flush()
		{
			return SteamNetworkingSockets.Internal.FlushMessagesOnConnection(this);
		}

		public string DetailedStatus()
		{
			if (SteamNetworkingSockets.Internal.GetDetailedConnectionStatus(this, out var pszBuf) != 0)
			{
				return null;
			}
			return pszBuf;
		}
	}
}
