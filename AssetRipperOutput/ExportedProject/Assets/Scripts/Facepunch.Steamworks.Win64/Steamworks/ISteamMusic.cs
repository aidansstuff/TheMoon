using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamMusic : SteamInterface
	{
		internal ISteamMusic(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamMusic_v001();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamMusic_v001();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusic_BIsEnabled")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsEnabled(IntPtr self);

		internal bool BIsEnabled()
		{
			return _BIsEnabled(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusic_BIsPlaying")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsPlaying(IntPtr self);

		internal bool BIsPlaying()
		{
			return _BIsPlaying(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusic_GetPlaybackStatus")]
		private static extern MusicStatus _GetPlaybackStatus(IntPtr self);

		internal MusicStatus GetPlaybackStatus()
		{
			return _GetPlaybackStatus(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusic_Play")]
		private static extern void _Play(IntPtr self);

		internal void Play()
		{
			_Play(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusic_Pause")]
		private static extern void _Pause(IntPtr self);

		internal void Pause()
		{
			_Pause(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusic_PlayPrevious")]
		private static extern void _PlayPrevious(IntPtr self);

		internal void PlayPrevious()
		{
			_PlayPrevious(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusic_PlayNext")]
		private static extern void _PlayNext(IntPtr self);

		internal void PlayNext()
		{
			_PlayNext(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusic_SetVolume")]
		private static extern void _SetVolume(IntPtr self, float flVolume);

		internal void SetVolume(float flVolume)
		{
			_SetVolume(Self, flVolume);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusic_GetVolume")]
		private static extern float _GetVolume(IntPtr self);

		internal float GetVolume()
		{
			return _GetVolume(Self);
		}
	}
}
