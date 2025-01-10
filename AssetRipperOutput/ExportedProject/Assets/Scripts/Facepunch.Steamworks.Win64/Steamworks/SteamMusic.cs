using System;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamMusic : SteamClientClass<SteamMusic>
	{
		internal static ISteamMusic Internal => SteamClientClass<SteamMusic>.Interface as ISteamMusic;

		public static bool IsEnabled => Internal.BIsEnabled();

		public static bool IsPlaying => Internal.BIsPlaying();

		public static MusicStatus Status => Internal.GetPlaybackStatus();

		public static float Volume
		{
			get
			{
				return Internal.GetVolume();
			}
			set
			{
				Internal.SetVolume(value);
			}
		}

		public static event Action OnPlaybackChanged;

		public static event Action<float> OnVolumeChanged;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamMusic(server));
			InstallEvents();
		}

		internal static void InstallEvents()
		{
			Dispatch.Install<PlaybackStatusHasChanged_t>(delegate
			{
				SteamMusic.OnPlaybackChanged?.Invoke();
			});
			Dispatch.Install(delegate(VolumeHasChanged_t x)
			{
				SteamMusic.OnVolumeChanged?.Invoke(x.NewVolume);
			});
		}

		public static void Play()
		{
			Internal.Play();
		}

		public static void Pause()
		{
			Internal.Pause();
		}

		public static void PlayPrevious()
		{
			Internal.PlayPrevious();
		}

		public static void PlayNext()
		{
			Internal.PlayNext();
		}
	}
}
