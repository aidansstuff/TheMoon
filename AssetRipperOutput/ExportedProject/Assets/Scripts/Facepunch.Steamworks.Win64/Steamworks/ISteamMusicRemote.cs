using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamMusicRemote : SteamInterface
	{
		internal ISteamMusicRemote(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamMusicRemote_v001();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamMusicRemote_v001();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_RegisterSteamMusicRemote")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _RegisterSteamMusicRemote(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName);

		internal bool RegisterSteamMusicRemote([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName)
		{
			return _RegisterSteamMusicRemote(Self, pchName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_DeregisterSteamMusicRemote")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _DeregisterSteamMusicRemote(IntPtr self);

		internal bool DeregisterSteamMusicRemote()
		{
			return _DeregisterSteamMusicRemote(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_BIsCurrentMusicRemote")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsCurrentMusicRemote(IntPtr self);

		internal bool BIsCurrentMusicRemote()
		{
			return _BIsCurrentMusicRemote(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_BActivationSuccess")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BActivationSuccess(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bValue);

		internal bool BActivationSuccess([MarshalAs(UnmanagedType.U1)] bool bValue)
		{
			return _BActivationSuccess(Self, bValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_SetDisplayName")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetDisplayName(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchDisplayName);

		internal bool SetDisplayName([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchDisplayName)
		{
			return _SetDisplayName(Self, pchDisplayName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_SetPNGIcon_64x64")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetPNGIcon_64x64(IntPtr self, IntPtr pvBuffer, uint cbBufferLength);

		internal bool SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength)
		{
			return _SetPNGIcon_64x64(Self, pvBuffer, cbBufferLength);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_EnablePlayPrevious")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _EnablePlayPrevious(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bValue);

		internal bool EnablePlayPrevious([MarshalAs(UnmanagedType.U1)] bool bValue)
		{
			return _EnablePlayPrevious(Self, bValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_EnablePlayNext")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _EnablePlayNext(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bValue);

		internal bool EnablePlayNext([MarshalAs(UnmanagedType.U1)] bool bValue)
		{
			return _EnablePlayNext(Self, bValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_EnableShuffled")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _EnableShuffled(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bValue);

		internal bool EnableShuffled([MarshalAs(UnmanagedType.U1)] bool bValue)
		{
			return _EnableShuffled(Self, bValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_EnableLooped")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _EnableLooped(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bValue);

		internal bool EnableLooped([MarshalAs(UnmanagedType.U1)] bool bValue)
		{
			return _EnableLooped(Self, bValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_EnableQueue")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _EnableQueue(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bValue);

		internal bool EnableQueue([MarshalAs(UnmanagedType.U1)] bool bValue)
		{
			return _EnableQueue(Self, bValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_EnablePlaylists")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _EnablePlaylists(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bValue);

		internal bool EnablePlaylists([MarshalAs(UnmanagedType.U1)] bool bValue)
		{
			return _EnablePlaylists(Self, bValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_UpdatePlaybackStatus")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _UpdatePlaybackStatus(IntPtr self, MusicStatus nStatus);

		internal bool UpdatePlaybackStatus(MusicStatus nStatus)
		{
			return _UpdatePlaybackStatus(Self, nStatus);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_UpdateShuffled")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _UpdateShuffled(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bValue);

		internal bool UpdateShuffled([MarshalAs(UnmanagedType.U1)] bool bValue)
		{
			return _UpdateShuffled(Self, bValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_UpdateLooped")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _UpdateLooped(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bValue);

		internal bool UpdateLooped([MarshalAs(UnmanagedType.U1)] bool bValue)
		{
			return _UpdateLooped(Self, bValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_UpdateVolume")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _UpdateVolume(IntPtr self, float flValue);

		internal bool UpdateVolume(float flValue)
		{
			return _UpdateVolume(Self, flValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_CurrentEntryWillChange")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _CurrentEntryWillChange(IntPtr self);

		internal bool CurrentEntryWillChange()
		{
			return _CurrentEntryWillChange(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_CurrentEntryIsAvailable")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _CurrentEntryIsAvailable(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bAvailable);

		internal bool CurrentEntryIsAvailable([MarshalAs(UnmanagedType.U1)] bool bAvailable)
		{
			return _CurrentEntryIsAvailable(Self, bAvailable);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_UpdateCurrentEntryText")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _UpdateCurrentEntryText(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchText);

		internal bool UpdateCurrentEntryText([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchText)
		{
			return _UpdateCurrentEntryText(Self, pchText);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _UpdateCurrentEntryElapsedSeconds(IntPtr self, int nValue);

		internal bool UpdateCurrentEntryElapsedSeconds(int nValue)
		{
			return _UpdateCurrentEntryElapsedSeconds(Self, nValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_UpdateCurrentEntryCoverArt")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _UpdateCurrentEntryCoverArt(IntPtr self, IntPtr pvBuffer, uint cbBufferLength);

		internal bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength)
		{
			return _UpdateCurrentEntryCoverArt(Self, pvBuffer, cbBufferLength);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_CurrentEntryDidChange")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _CurrentEntryDidChange(IntPtr self);

		internal bool CurrentEntryDidChange()
		{
			return _CurrentEntryDidChange(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_QueueWillChange")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _QueueWillChange(IntPtr self);

		internal bool QueueWillChange()
		{
			return _QueueWillChange(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_ResetQueueEntries")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ResetQueueEntries(IntPtr self);

		internal bool ResetQueueEntries()
		{
			return _ResetQueueEntries(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_SetQueueEntry")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetQueueEntry(IntPtr self, int nID, int nPosition, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchEntryText);

		internal bool SetQueueEntry(int nID, int nPosition, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchEntryText)
		{
			return _SetQueueEntry(Self, nID, nPosition, pchEntryText);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_SetCurrentQueueEntry")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetCurrentQueueEntry(IntPtr self, int nID);

		internal bool SetCurrentQueueEntry(int nID)
		{
			return _SetCurrentQueueEntry(Self, nID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_QueueDidChange")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _QueueDidChange(IntPtr self);

		internal bool QueueDidChange()
		{
			return _QueueDidChange(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_PlaylistWillChange")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _PlaylistWillChange(IntPtr self);

		internal bool PlaylistWillChange()
		{
			return _PlaylistWillChange(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_ResetPlaylistEntries")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ResetPlaylistEntries(IntPtr self);

		internal bool ResetPlaylistEntries()
		{
			return _ResetPlaylistEntries(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_SetPlaylistEntry")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetPlaylistEntry(IntPtr self, int nID, int nPosition, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchEntryText);

		internal bool SetPlaylistEntry(int nID, int nPosition, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchEntryText)
		{
			return _SetPlaylistEntry(Self, nID, nPosition, pchEntryText);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_SetCurrentPlaylistEntry")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetCurrentPlaylistEntry(IntPtr self, int nID);

		internal bool SetCurrentPlaylistEntry(int nID)
		{
			return _SetCurrentPlaylistEntry(Self, nID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMusicRemote_PlaylistDidChange")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _PlaylistDidChange(IntPtr self);

		internal bool PlaylistDidChange()
		{
			return _PlaylistDidChange(Self);
		}
	}
}
