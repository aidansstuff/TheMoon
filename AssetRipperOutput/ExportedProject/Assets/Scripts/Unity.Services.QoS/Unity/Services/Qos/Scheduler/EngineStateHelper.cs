using UnityEngine;

namespace Unity.Services.Qos.Scheduler
{
	internal static class EngineStateHelper
	{
		public static bool IsPlaying;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Init()
		{
			IsPlaying = Application.isPlaying;
		}
	}
}
