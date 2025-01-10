using System;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.TLS.LowLevel;

namespace Unity.Networking.Transport.TLS
{
	internal static class ManagedSecureFunctions
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ManagedSecureFunctionsKey
		{
		}

		private const int UNITYTLS_ERR_SSL_WANT_READ = -26880;

		private const int UNITYTLS_ERR_SSL_WANT_WRITE = -26752;

		private static Binding.unitytls_client_data_send_callback s_sendCallback;

		private static Binding.unitytls_client_data_receive_callback s_recvCallback;

		private static bool IsInitialized;

		internal static readonly SharedStatic<FunctionPointer<Binding.unitytls_client_data_send_callback>> s_SendCallback = SharedStatic<FunctionPointer<Binding.unitytls_client_data_send_callback>>.GetOrCreateUnsafe(0u, -5978926962771261641L, 641209945766110788L);

		internal static readonly SharedStatic<FunctionPointer<Binding.unitytls_client_data_receive_callback>> s_RecvMethod = SharedStatic<FunctionPointer<Binding.unitytls_client_data_receive_callback>>.GetOrCreateUnsafe(0u, -5365965991464889048L, 641209945766110788L);

		internal unsafe static void Initialize()
		{
			if (!IsInitialized)
			{
				IsInitialized = true;
				s_sendCallback = SecureDataSendCallback;
				s_recvCallback = SecureDataReceiveCallback;
				s_SendCallback.Data = new FunctionPointer<Binding.unitytls_client_data_send_callback>(Marshal.GetFunctionPointerForDelegate(s_sendCallback));
				s_RecvMethod.Data = new FunctionPointer<Binding.unitytls_client_data_receive_callback>(Marshal.GetFunctionPointerForDelegate(s_recvCallback));
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(Binding.unitytls_client_data_send_callback))]
		private unsafe static int SecureDataSendCallback(IntPtr userData, byte* data, UIntPtr dataLen, uint status)
		{
			SecureUserData* ptr = (SecureUserData*)(void*)userData;
			if (ptr->Interface.BeginSendMessage.Ptr.Invoke(out var handle, ptr->Interface.UserData, (int)dataLen.ToUInt32()) != 0)
			{
				return -26752;
			}
			handle.size = (int)dataLen.ToUInt32();
			byte* destination = (byte*)(void*)handle.data;
			UnsafeUtility.MemCpy(destination, data, (long)dataLen.ToUInt64());
			return ptr->Interface.EndSendMessage.Ptr.Invoke(ref handle, ref ptr->Remote, ptr->Interface.UserData, ref ptr->QueueHandle);
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(Binding.unitytls_client_data_receive_callback))]
		private unsafe static int SecureDataReceiveCallback(IntPtr userData, byte* data, UIntPtr dataLen, uint status)
		{
			SecureUserData* ptr = (SecureUserData*)(void*)userData;
			byte* ptr2 = (byte*)(void*)ptr->StreamData;
			if (ptr2 == null || ptr->Size <= 0)
			{
				return -26880;
			}
			if (ptr->BytesProcessed != 0)
			{
				return -26880;
			}
			UnsafeUtility.MemCpy(data, ptr2, ptr->Size);
			ptr->BytesProcessed = ptr->Size;
			return ptr->Size;
		}
	}
}
