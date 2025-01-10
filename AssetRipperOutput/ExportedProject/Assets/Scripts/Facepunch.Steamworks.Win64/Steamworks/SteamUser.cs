using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamUser : SteamClientClass<SteamUser>
	{
		private static Dictionary<string, string> richPresence;

		private static bool _recordingVoice;

		private static byte[] readBuffer = new byte[131072];

		private static uint sampleRate = 48000u;

		internal static ISteamUser Internal => SteamClientClass<SteamUser>.Interface as ISteamUser;

		public static bool VoiceRecord
		{
			get
			{
				return _recordingVoice;
			}
			set
			{
				_recordingVoice = value;
				if (value)
				{
					Internal.StartVoiceRecording();
				}
				else
				{
					Internal.StopVoiceRecording();
				}
			}
		}

		public static bool HasVoiceData
		{
			get
			{
				uint pcbCompressed = 0u;
				uint pcbUncompressed_Deprecated = 0u;
				if (Internal.GetAvailableVoice(ref pcbCompressed, ref pcbUncompressed_Deprecated, 0u) != 0)
				{
					return false;
				}
				return pcbCompressed != 0;
			}
		}

		public static uint SampleRate
		{
			get
			{
				return sampleRate;
			}
			set
			{
				if (SampleRate < 11025)
				{
					throw new Exception("Sample Rate must be between 11025 and 48000");
				}
				if (SampleRate > 48000)
				{
					throw new Exception("Sample Rate must be between 11025 and 48000");
				}
				sampleRate = value;
			}
		}

		public static uint OptimalSampleRate => Internal.GetVoiceOptimalSampleRate();

		public static bool IsBehindNAT => Internal.BIsBehindNAT();

		public static int SteamLevel => Internal.GetPlayerSteamLevel();

		public static bool IsPhoneVerified => Internal.BIsPhoneVerified();

		public static bool IsTwoFactorEnabled => Internal.BIsTwoFactorEnabled();

		public static bool IsPhoneIdentifying => Internal.BIsPhoneIdentifying();

		public static bool IsPhoneRequiringVerification => Internal.BIsPhoneRequiringVerification();

		public static event Action OnSteamServersConnected;

		public static event Action OnSteamServerConnectFailure;

		public static event Action OnSteamServersDisconnected;

		public static event Action OnClientGameServerDeny;

		public static event Action OnLicensesUpdated;

		public static event Action<SteamId, SteamId, AuthResponse> OnValidateAuthTicketResponse;

		internal static event Action<GetAuthSessionTicketResponse_t> OnGetAuthSessionTicketResponse;

		public static event Action<AppId, ulong, bool> OnMicroTxnAuthorizationResponse;

		public static event Action<string> OnGameWebCallback;

		public static event Action<DurationControl> OnDurationControl;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamUser(server));
			InstallEvents();
			richPresence = new Dictionary<string, string>();
			SampleRate = OptimalSampleRate;
		}

		internal static void InstallEvents()
		{
			Dispatch.Install<SteamServersConnected_t>(delegate
			{
				SteamUser.OnSteamServersConnected?.Invoke();
			});
			Dispatch.Install<SteamServerConnectFailure_t>(delegate
			{
				SteamUser.OnSteamServerConnectFailure?.Invoke();
			});
			Dispatch.Install<SteamServersDisconnected_t>(delegate
			{
				SteamUser.OnSteamServersDisconnected?.Invoke();
			});
			Dispatch.Install<ClientGameServerDeny_t>(delegate
			{
				SteamUser.OnClientGameServerDeny?.Invoke();
			});
			Dispatch.Install<LicensesUpdated_t>(delegate
			{
				SteamUser.OnLicensesUpdated?.Invoke();
			});
			Dispatch.Install(delegate(ValidateAuthTicketResponse_t x)
			{
				SteamUser.OnValidateAuthTicketResponse?.Invoke(x.SteamID, x.OwnerSteamID, x.AuthSessionResponse);
			});
			Dispatch.Install(delegate(MicroTxnAuthorizationResponse_t x)
			{
				SteamUser.OnMicroTxnAuthorizationResponse?.Invoke(x.AppID, x.OrderID, x.Authorized != 0);
			});
			Dispatch.Install(delegate(GameWebCallback_t x)
			{
				SteamUser.OnGameWebCallback?.Invoke(x.URLUTF8());
			});
			Dispatch.Install(delegate(GetAuthSessionTicketResponse_t x)
			{
				SteamUser.OnGetAuthSessionTicketResponse?.Invoke(x);
			});
			Dispatch.Install(delegate(DurationControl_t x)
			{
				SteamUser.OnDurationControl?.Invoke(new DurationControl
				{
					_inner = x
				});
			});
		}

		public unsafe static int ReadVoiceData(Stream stream)
		{
			if (!HasVoiceData)
			{
				return 0;
			}
			uint nBytesWritten = 0u;
			uint nUncompressBytesWritten_Deprecated = 0u;
			fixed (byte* ptr = readBuffer)
			{
				if (Internal.GetVoice(bWantCompressed: true, (IntPtr)ptr, (uint)readBuffer.Length, ref nBytesWritten, bWantUncompressed_Deprecated: false, IntPtr.Zero, 0u, ref nUncompressBytesWritten_Deprecated, 0u) != 0)
				{
					return 0;
				}
			}
			if (nBytesWritten == 0)
			{
				return 0;
			}
			stream.Write(readBuffer, 0, (int)nBytesWritten);
			return (int)nBytesWritten;
		}

		public unsafe static byte[] ReadVoiceDataBytes()
		{
			if (!HasVoiceData)
			{
				return null;
			}
			uint nBytesWritten = 0u;
			uint nUncompressBytesWritten_Deprecated = 0u;
			fixed (byte* ptr = readBuffer)
			{
				if (Internal.GetVoice(bWantCompressed: true, (IntPtr)ptr, (uint)readBuffer.Length, ref nBytesWritten, bWantUncompressed_Deprecated: false, IntPtr.Zero, 0u, ref nUncompressBytesWritten_Deprecated, 0u) != 0)
				{
					return null;
				}
			}
			if (nBytesWritten == 0)
			{
				return null;
			}
			byte[] array = new byte[nBytesWritten];
			Array.Copy(readBuffer, 0L, array, 0L, nBytesWritten);
			return array;
		}

		public unsafe static int DecompressVoice(Stream input, int length, Stream output)
		{
			byte[] array = Helpers.TakeBuffer(length);
			byte[] array2 = Helpers.TakeBuffer(65536);
			using (MemoryStream destination = new MemoryStream(array))
			{
				input.CopyTo(destination);
			}
			uint nBytesWritten = 0u;
			fixed (byte* ptr = array)
			{
				fixed (byte* ptr2 = array2)
				{
					if (Internal.DecompressVoice((IntPtr)ptr, (uint)length, (IntPtr)ptr2, (uint)array2.Length, ref nBytesWritten, SampleRate) != 0)
					{
						return 0;
					}
				}
			}
			if (nBytesWritten == 0)
			{
				return 0;
			}
			output.Write(array2, 0, (int)nBytesWritten);
			return (int)nBytesWritten;
		}

		public unsafe static int DecompressVoice(byte[] from, Stream output)
		{
			byte[] array = Helpers.TakeBuffer(65536);
			uint nBytesWritten = 0u;
			fixed (byte* ptr = from)
			{
				fixed (byte* ptr2 = array)
				{
					if (Internal.DecompressVoice((IntPtr)ptr, (uint)from.Length, (IntPtr)ptr2, (uint)array.Length, ref nBytesWritten, SampleRate) != 0)
					{
						return 0;
					}
				}
			}
			if (nBytesWritten == 0)
			{
				return 0;
			}
			output.Write(array, 0, (int)nBytesWritten);
			return (int)nBytesWritten;
		}

		public unsafe static AuthTicket GetAuthSessionTicket()
		{
			byte[] array = Helpers.TakeBuffer(1024);
			fixed (byte* ptr = array)
			{
				uint pcbTicket = 0u;
				uint num = Internal.GetAuthSessionTicket((IntPtr)ptr, array.Length, ref pcbTicket);
				if (num == 0)
				{
					return null;
				}
				return new AuthTicket
				{
					Data = array.Take((int)pcbTicket).ToArray(),
					Handle = num
				};
			}
		}

		public static async Task<AuthTicket> GetAuthSessionTicketAsync(double timeoutSeconds = 10.0)
		{
			Result result = Result.Pending;
			AuthTicket ticket = null;
			Stopwatch stopwatch = Stopwatch.StartNew();
			OnGetAuthSessionTicketResponse += f;
			try
			{
				ticket = GetAuthSessionTicket();
				if (ticket == null)
				{
					return null;
				}
				while (result == Result.Pending)
				{
					await Task.Delay(10);
					if (stopwatch.Elapsed.TotalSeconds > timeoutSeconds)
					{
						ticket.Cancel();
						return null;
					}
				}
				if (result == Result.OK)
				{
					return ticket;
				}
				ticket.Cancel();
				return null;
			}
			finally
			{
				OnGetAuthSessionTicketResponse -= f;
			}
			void f(GetAuthSessionTicketResponse_t t)
			{
				if (t.AuthTicket == ticket.Handle)
				{
					result = t.Result;
				}
			}
		}

		public unsafe static BeginAuthResult BeginAuthSession(byte[] ticketData, SteamId steamid)
		{
			fixed (byte* ptr = ticketData)
			{
				return Internal.BeginAuthSession((IntPtr)ptr, ticketData.Length, steamid);
			}
		}

		public static void EndAuthSession(SteamId steamid)
		{
			Internal.EndAuthSession(steamid);
		}

		public static async Task<string> GetStoreAuthUrlAsync(string url)
		{
			StoreAuthURLResponse_t? response = await Internal.RequestStoreAuthURL(url);
			if (!response.HasValue)
			{
				return null;
			}
			return response.Value.URLUTF8();
		}

		public static async Task<byte[]> RequestEncryptedAppTicketAsync(byte[] dataToInclude)
		{
			IntPtr dataPtr = Marshal.AllocHGlobal(dataToInclude.Length);
			Marshal.Copy(dataToInclude, 0, dataPtr, dataToInclude.Length);
			try
			{
				EncryptedAppTicketResponse_t? result = await Internal.RequestEncryptedAppTicket(dataPtr, dataToInclude.Length);
				if (!result.HasValue || result.Value.Result != Result.OK)
				{
					return null;
				}
				IntPtr ticketData = Marshal.AllocHGlobal(1024);
				uint outSize = 0u;
				byte[] data = null;
				if (Internal.GetEncryptedAppTicket(ticketData, 1024, ref outSize))
				{
					data = new byte[outSize];
					Marshal.Copy(ticketData, data, 0, (int)outSize);
				}
				Marshal.FreeHGlobal(ticketData);
				return data;
			}
			finally
			{
				Marshal.FreeHGlobal(dataPtr);
			}
		}

		public static async Task<byte[]> RequestEncryptedAppTicketAsync()
		{
			EncryptedAppTicketResponse_t? result = await Internal.RequestEncryptedAppTicket(IntPtr.Zero, 0);
			if (!result.HasValue || result.Value.Result != Result.OK)
			{
				return null;
			}
			IntPtr ticketData = Marshal.AllocHGlobal(1024);
			uint outSize = 0u;
			byte[] data = null;
			if (Internal.GetEncryptedAppTicket(ticketData, 1024, ref outSize))
			{
				data = new byte[outSize];
				Marshal.Copy(ticketData, data, 0, (int)outSize);
			}
			Marshal.FreeHGlobal(ticketData);
			return data;
		}

		public static async Task<DurationControl> GetDurationControl()
		{
			DurationControl_t? response = await Internal.GetDurationControl();
			if (!response.HasValue)
			{
				return default(DurationControl);
			}
			DurationControl result = default(DurationControl);
			result._inner = response.Value;
			return result;
		}
	}
}
