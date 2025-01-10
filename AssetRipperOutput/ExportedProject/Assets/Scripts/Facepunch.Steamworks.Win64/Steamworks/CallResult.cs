using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal struct CallResult<T> : INotifyCompletion where T : struct, ICallbackData
	{
		private SteamAPICall_t call;

		private ISteamUtils utils;

		private bool server;

		public bool IsCompleted
		{
			get
			{
				bool pbFailed = false;
				if (utils.IsAPICallCompleted(call, ref pbFailed) || pbFailed)
				{
					return true;
				}
				return false;
			}
		}

		public CallResult(SteamAPICall_t call, bool server)
		{
			this.call = call;
			this.server = server;
			utils = (server ? SteamSharedClass<SteamUtils>.InterfaceServer : SteamSharedClass<SteamUtils>.InterfaceClient) as ISteamUtils;
			if (utils == null)
			{
				utils = SteamSharedClass<SteamUtils>.Interface as ISteamUtils;
			}
		}

		public void OnCompleted(Action continuation)
		{
			Dispatch.OnCallComplete<T>(call, continuation, server);
		}

		public T? GetResult()
		{
			bool pbFailed = false;
			if (!utils.IsAPICallCompleted(call, ref pbFailed) || pbFailed)
			{
				return null;
			}
			T val = default(T);
			int dataSize = val.DataSize;
			IntPtr intPtr = Marshal.AllocHGlobal(dataSize);
			try
			{
				if (!utils.GetAPICallResult(call, intPtr, dataSize, (int)val.CallbackType, ref pbFailed) || pbFailed)
				{
					Dispatch.OnDebugCallback?.Invoke(val.CallbackType, "!GetAPICallResult or failed", server);
					return null;
				}
				Dispatch.OnDebugCallback?.Invoke(val.CallbackType, Dispatch.CallbackToString(val.CallbackType, intPtr, dataSize), server);
				return (T)Marshal.PtrToStructure(intPtr, typeof(T));
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}

		internal CallResult<T> GetAwaiter()
		{
			return this;
		}
	}
}
