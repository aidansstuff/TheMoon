using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class SubFrameManager
	{
		private float m_ShutterInterval;

		private float m_ShutterFullyOpen;

		private float m_ShutterBeginsClosing = 1f;

		private AnimationCurve m_ShutterCurve;

		private float m_OriginalCaptureDeltaTime;

		private float m_OriginalFixedDeltaTime;

		private float m_OriginalTimeScale;

		private Dictionary<int, CameraData> m_CameraCache = new Dictionary<int, CameraData>();

		private uint m_AccumulationSamples;

		private bool m_IsRecording;

		public uint subFrameCount
		{
			get
			{
				return m_AccumulationSamples;
			}
			set
			{
				m_AccumulationSamples = value;
			}
		}

		public bool isRecording => m_IsRecording;

		public float shutterInterval => m_ShutterInterval;

		internal CameraData GetCameraData(int camID)
		{
			if (!m_CameraCache.TryGetValue(camID, out var value))
			{
				value.ResetIteration();
				m_CameraCache.Add(camID, value);
			}
			return value;
		}

		internal void SetCameraData(int camID, CameraData camData)
		{
			m_CameraCache[camID] = camData;
		}

		internal void Reset(int camID)
		{
			CameraData cameraData = GetCameraData(camID);
			cameraData.ResetIteration();
			SetCameraData(camID, cameraData);
		}

		internal void Reset()
		{
			foreach (int item in m_CameraCache.Keys.ToList())
			{
				Reset(item);
			}
		}

		internal void Clear()
		{
			m_CameraCache.Clear();
		}

		internal void SelectiveReset(uint maxSamples)
		{
			foreach (int item in m_CameraCache.Keys.ToList())
			{
				CameraData cameraData = GetCameraData(item);
				if (cameraData.currentIteration >= maxSamples)
				{
					cameraData.ResetIteration();
					SetCameraData(item, cameraData);
				}
			}
		}

		private void Init(int samples, float shutterInterval)
		{
			m_AccumulationSamples = (uint)samples;
			m_ShutterInterval = ((samples > 1) ? shutterInterval : 0f);
			m_IsRecording = true;
			Clear();
			m_OriginalCaptureDeltaTime = Time.captureDeltaTime;
			m_OriginalFixedDeltaTime = Time.fixedDeltaTime;
			if (shutterInterval > 0f)
			{
				Time.captureDeltaTime = m_OriginalCaptureDeltaTime / (float)m_AccumulationSamples;
				Time.fixedDeltaTime = m_OriginalFixedDeltaTime / (float)m_AccumulationSamples;
			}
			else
			{
				Time.captureDeltaTime = 0f;
				Time.fixedDeltaTime = 0f;
			}
		}

		internal void BeginRecording(int samples, float shutterInterval, float shutterFullyOpen = 0f, float shutterBeginsClosing = 1f)
		{
			Init(samples, shutterInterval);
			m_ShutterFullyOpen = shutterFullyOpen;
			m_ShutterBeginsClosing = shutterBeginsClosing;
		}

		internal void BeginRecording(int samples, float shutterInterval, AnimationCurve shutterProfile)
		{
			Init(samples, shutterInterval);
			m_ShutterCurve = shutterProfile;
		}

		internal void EndRecording()
		{
			m_IsRecording = false;
			m_ShutterCurve = null;
			Time.captureDeltaTime = m_OriginalCaptureDeltaTime;
			Time.fixedDeltaTime = m_OriginalFixedDeltaTime;
			if ((double)m_OriginalTimeScale != 0.0)
			{
				Time.timeScale = m_OriginalTimeScale;
				m_OriginalTimeScale = 0f;
			}
		}

		internal void PrepareNewSubFrame()
		{
			uint num = 0u;
			foreach (int item in m_CameraCache.Keys.ToList())
			{
				num = Math.Max(num, GetCameraData(item).currentIteration);
			}
			if (m_ShutterInterval == 0f)
			{
				if (num == m_AccumulationSamples - 1)
				{
					Time.captureDeltaTime = m_OriginalCaptureDeltaTime;
					Time.fixedDeltaTime = m_OriginalFixedDeltaTime;
					Time.timeScale = m_OriginalTimeScale;
				}
				else
				{
					if (m_OriginalTimeScale == 0f)
					{
						m_OriginalTimeScale = Time.timeScale;
					}
					Time.captureDeltaTime = 0f;
					Time.fixedDeltaTime = 0f;
					Time.timeScale = 0f;
				}
			}
			if (num >= m_AccumulationSamples)
			{
				Reset();
			}
		}

		private float ShutterProfile(float time)
		{
			if (time > m_ShutterInterval)
			{
				return 0f;
			}
			time /= m_ShutterInterval;
			if (m_ShutterCurve != null)
			{
				return m_ShutterCurve.Evaluate(time);
			}
			if (time < m_ShutterFullyOpen)
			{
				return 1f / m_ShutterFullyOpen * time;
			}
			if (time > m_ShutterBeginsClosing)
			{
				float num = 1f / (1f - m_ShutterBeginsClosing);
				return 1f - num * (time - m_ShutterBeginsClosing);
			}
			return 1f;
		}

		internal Vector4 ComputeFrameWeights(int camID)
		{
			CameraData cameraData = GetCameraData(camID);
			float accumulatedWeight = cameraData.accumulatedWeight;
			float time = ((m_AccumulationSamples != 0) ? ((float)cameraData.currentIteration / (float)m_AccumulationSamples) : 0f);
			float num = ((isRecording && m_ShutterInterval > 0f) ? ShutterProfile(time) : 1f);
			if (cameraData.currentIteration < m_AccumulationSamples)
			{
				cameraData.accumulatedWeight += num;
			}
			SetCameraData(camID, cameraData);
			if (!(cameraData.accumulatedWeight > 0f))
			{
				return new Vector4(num, accumulatedWeight, 0f, 0f);
			}
			return new Vector4(num, accumulatedWeight, 1f / cameraData.accumulatedWeight, 0f);
		}
	}
}
