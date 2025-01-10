using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Networking.Transport
{
	public struct NetworkPacketReceiver
	{
		[Flags]
		public enum AppendPacketMode
		{
			None = 0,
			NoCopyNeeded = 1
		}

		internal NetworkDriver m_Driver;

		public long LastUpdateTime => m_Driver.LastUpdateTime;

		public int ReceiveErrorCode
		{
			set
			{
				m_Driver.ReceiveErrorCode = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IntPtr AllocateMemory(ref int dataLen)
		{
			return m_Driver.AllocateMemory(ref dataLen);
		}

		public unsafe bool AppendPacket(IntPtr data, ref NetworkInterfaceEndPoint address, int dataLen, AppendPacketMode mode = AppendPacketMode.None)
		{
			if ((mode & AppendPacketMode.NoCopyNeeded) != 0)
			{
				m_Driver.AppendPacket(data, ref address, dataLen);
				return true;
			}
			int dataLen2 = dataLen;
			IntPtr intPtr = m_Driver.AllocateMemory(ref dataLen2);
			if (intPtr == IntPtr.Zero || dataLen2 < dataLen)
			{
				OutOfMemoryError();
				return false;
			}
			UnsafeUtility.MemCpy(intPtr.ToPointer(), data.ToPointer(), dataLen);
			m_Driver.AppendPacket(intPtr, ref address, dataLen);
			return true;
		}

		public bool IsAddressUsed(NetworkInterfaceEndPoint address)
		{
			return m_Driver.IsAddressUsed(address);
		}

		private void OutOfMemoryError()
		{
			ReceiveErrorCode = 10040;
		}
	}
}
