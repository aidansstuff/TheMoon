using System;
using UnityEngine;

namespace Unity.Services.Relay.Scheduler
{
	public static class GameObjectFactory
	{
		public static GameObject CreateCoreSdkGameObject()
		{
			System.Random random = new System.Random();
			GameObject gameObject = new GameObject("_SdkCore-" + random.Next(0, int.MaxValue));
			gameObject.AddComponent<TaskSchedulerThreaded>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject;
		}
	}
}
