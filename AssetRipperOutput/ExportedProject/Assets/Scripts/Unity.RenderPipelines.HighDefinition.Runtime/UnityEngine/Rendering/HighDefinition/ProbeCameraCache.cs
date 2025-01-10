using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class ProbeCameraCache<K> : IDisposable
	{
		private Stack<Camera> m_CameraPool = new Stack<Camera>();

		private Dictionary<K, (Camera camera, int lastFrame)> m_Cache = new Dictionary<K, (Camera, int)>();

		private K[] m_TempCameraKeysCache = new K[0];

		internal int cachedActiveCameraCount => m_CameraPool.Count;

		public Camera GetOrCreate(K key, int frameCount, CameraType cameraType = CameraType.Game)
		{
			if (m_Cache == null)
			{
				throw new ObjectDisposedException("ProbeCameraCache");
			}
			if (!m_Cache.TryGetValue(key, out (Camera, int) value) || value.Item1 == null || value.Item1.Equals(null))
			{
				value = ((m_CameraPool.Count != 0) ? (m_CameraPool.Pop(), frameCount) : (new GameObject().AddComponent<Camera>(), frameCount));
				value.Item1.cameraType = cameraType;
				m_Cache[key] = value;
			}
			else
			{
				value.Item2 = frameCount;
				m_Cache[key] = value;
			}
			return value.Item1;
		}

		public void ReleaseCamerasUnusedFor(int frameWindow, int frameCount)
		{
			if (m_Cache == null)
			{
				throw new ObjectDisposedException("ProbeCameraCache");
			}
			if (m_Cache.Count == 0)
			{
				return;
			}
			if (m_TempCameraKeysCache.Length != m_Cache.Count)
			{
				m_TempCameraKeysCache = new K[m_Cache.Count];
			}
			m_Cache.Keys.CopyTo(m_TempCameraKeysCache, 0);
			K[] tempCameraKeysCache = m_TempCameraKeysCache;
			foreach (K key in tempCameraKeysCache)
			{
				if (m_Cache.TryGetValue(key, out (Camera, int) value) && Math.Abs(frameCount - value.Item2) > frameWindow)
				{
					if (value.Item1 != null)
					{
						m_CameraPool.Push(value.Item1);
					}
					m_Cache.Remove(key);
				}
			}
		}

		public void Clear()
		{
			if (m_Cache == null)
			{
				throw new ObjectDisposedException("ProbeCameraCache");
			}
			foreach (KeyValuePair<K, (Camera, int)> item in m_Cache)
			{
				if (item.Value.Item1 != null)
				{
					CoreUtils.Destroy(item.Value.Item1.gameObject);
				}
			}
			m_Cache.Clear();
			foreach (Camera item2 in m_CameraPool)
			{
				CoreUtils.Destroy(item2.gameObject);
			}
			m_CameraPool.Clear();
		}

		public void Dispose()
		{
			Clear();
			m_Cache = null;
			m_CameraPool = null;
		}
	}
}
