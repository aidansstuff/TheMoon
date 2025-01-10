using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.Netcode
{
	public class SceneEvent
	{
		public AsyncOperation AsyncOperation;

		public SceneEventType SceneEventType;

		public LoadSceneMode LoadSceneMode;

		public string SceneName;

		public Scene Scene;

		public ulong ClientId;

		public List<ulong> ClientsThatCompleted;

		public List<ulong> ClientsThatTimedOut;
	}
}
