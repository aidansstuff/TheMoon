using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public static class Dispatch
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct CallbackMsg_t
		{
			public HSteamUser m_hSteamUser;

			public CallbackType Type;

			public IntPtr Data;

			public int DataSize;
		}

		private struct ResultCallback
		{
			public Action continuation;

			public bool server;
		}

		private struct Callback
		{
			public Action<IntPtr> action;

			public bool server;
		}

		public static Action<CallbackType, string, bool> OnDebugCallback;

		public static Action<Exception> OnException;

		private static bool runningFrame = false;

		private static List<Action<IntPtr>> actionsToCall = new List<Action<IntPtr>>();

		private static Dictionary<ulong, ResultCallback> ResultCallbacks = new Dictionary<ulong, ResultCallback>();

		private static Dictionary<CallbackType, List<Callback>> Callbacks = new Dictionary<CallbackType, List<Callback>>();

		internal static HSteamPipe ClientPipe { get; set; }

		internal static HSteamPipe ServerPipe { get; set; }

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamAPI_ManualDispatch_Init();

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamAPI_ManualDispatch_RunFrame(HSteamPipe pipe);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool SteamAPI_ManualDispatch_GetNextCallback(HSteamPipe pipe, [In][Out] ref CallbackMsg_t msg);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool SteamAPI_ManualDispatch_FreeLastCallback(HSteamPipe pipe);

		internal static void Init()
		{
			SteamAPI_ManualDispatch_Init();
		}

		internal static void Frame(HSteamPipe pipe)
		{
			if (runningFrame)
			{
				return;
			}
			try
			{
				runningFrame = true;
				SteamAPI_ManualDispatch_RunFrame(pipe);
				SteamNetworkingUtils.OutputDebugMessages();
				CallbackMsg_t msg = default(CallbackMsg_t);
				while (SteamAPI_ManualDispatch_GetNextCallback(pipe, ref msg))
				{
					try
					{
						ProcessCallback(msg, pipe == ServerPipe);
					}
					finally
					{
						SteamAPI_ManualDispatch_FreeLastCallback(pipe);
					}
				}
			}
			catch (Exception obj)
			{
				OnException?.Invoke(obj);
			}
			finally
			{
				runningFrame = false;
			}
		}

		private static void ProcessCallback(CallbackMsg_t msg, bool isServer)
		{
			OnDebugCallback?.Invoke(msg.Type, CallbackToString(msg.Type, msg.Data, msg.DataSize), isServer);
			if (msg.Type == CallbackType.SteamAPICallCompleted)
			{
				ProcessResult(msg);
			}
			else
			{
				if (!Callbacks.TryGetValue(msg.Type, out var value))
				{
					return;
				}
				actionsToCall.Clear();
				foreach (Callback item in value)
				{
					if (item.server == isServer)
					{
						actionsToCall.Add(item.action);
					}
				}
				foreach (Action<IntPtr> item2 in actionsToCall)
				{
					item2(msg.Data);
				}
				actionsToCall.Clear();
			}
		}

		internal static string CallbackToString(CallbackType type, IntPtr data, int expectedsize)
		{
			if (!CallbackTypeFactory.All.TryGetValue(type, out var value))
			{
				return $"[{type} not in sdk]";
			}
			object obj = data.ToType(value);
			if (obj == null)
			{
				return "[null]";
			}
			string text = "";
			FieldInfo[] fields = value.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fields.Length == 0)
			{
				return "[no fields]";
			}
			int num = fields.Max((FieldInfo x) => x.Name.Length) + 1;
			if (num < 10)
			{
				num = 10;
			}
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				int num2 = num - fieldInfo.Name.Length;
				if (num2 < 0)
				{
					num2 = 0;
				}
				text += $"{new string(' ', num2)}{fieldInfo.Name}: {fieldInfo.GetValue(obj)}\n";
			}
			return text.Trim(new char[1] { '\n' });
		}

		private static void ProcessResult(CallbackMsg_t msg)
		{
			SteamAPICallCompleted_t steamAPICallCompleted_t = msg.Data.ToType<SteamAPICallCompleted_t>();
			if (!ResultCallbacks.TryGetValue(steamAPICallCompleted_t.AsyncCall, out var value))
			{
				OnDebugCallback?.Invoke((CallbackType)steamAPICallCompleted_t.Callback, "[no callback waiting/required]", arg3: false);
				return;
			}
			ResultCallbacks.Remove(steamAPICallCompleted_t.AsyncCall);
			value.continuation();
		}

		internal static async void LoopClientAsync()
		{
			while (ClientPipe != 0)
			{
				Frame(ClientPipe);
				await Task.Delay(16);
			}
		}

		internal static async void LoopServerAsync()
		{
			while (ServerPipe != 0)
			{
				Frame(ServerPipe);
				await Task.Delay(32);
			}
		}

		internal static void OnCallComplete<T>(SteamAPICall_t call, Action continuation, bool server) where T : struct, ICallbackData
		{
			ResultCallbacks[call.Value] = new ResultCallback
			{
				continuation = continuation,
				server = server
			};
		}

		internal static void Install<T>(Action<T> p, bool server = false) where T : ICallbackData
		{
			CallbackType callbackType = default(T).CallbackType;
			if (!Callbacks.TryGetValue(callbackType, out var value))
			{
				value = new List<Callback>();
				Callbacks[callbackType] = value;
			}
			value.Add(new Callback
			{
				action = delegate(IntPtr x)
				{
					p(x.ToType<T>());
				},
				server = server
			});
		}

		internal static void ShutdownServer()
		{
			ServerPipe = 0;
			foreach (KeyValuePair<CallbackType, List<Callback>> callback in Callbacks)
			{
				Callbacks[callback.Key].RemoveAll((Callback x) => x.server);
			}
			ResultCallbacks = ResultCallbacks.Where((KeyValuePair<ulong, ResultCallback> x) => !x.Value.server).ToDictionary((KeyValuePair<ulong, ResultCallback> x) => x.Key, (KeyValuePair<ulong, ResultCallback> x) => x.Value);
		}

		internal static void ShutdownClient()
		{
			ClientPipe = 0;
			foreach (KeyValuePair<CallbackType, List<Callback>> callback in Callbacks)
			{
				Callbacks[callback.Key].RemoveAll((Callback x) => !x.server);
			}
			ResultCallbacks = ResultCallbacks.Where((KeyValuePair<ulong, ResultCallback> x) => x.Value.server).ToDictionary((KeyValuePair<ulong, ResultCallback> x) => x.Key, (KeyValuePair<ulong, ResultCallback> x) => x.Value);
		}
	}
}
