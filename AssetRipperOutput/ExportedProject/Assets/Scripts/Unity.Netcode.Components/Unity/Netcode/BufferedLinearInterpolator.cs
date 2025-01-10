using System;
using System.Collections.Generic;

namespace Unity.Netcode
{
	public abstract class BufferedLinearInterpolator<T> where T : struct
	{
		private struct BufferedItem
		{
			public T Item;

			public double TimeSent;

			public BufferedItem(T item, double timeSent)
			{
				Item = item;
				TimeSent = timeSent;
			}
		}

		internal float MaxInterpolationBound = 3f;

		public float MaximumInterpolationTime = 0.1f;

		private const double k_SmallValue = 9.999999439624929E-11;

		private T m_InterpStartValue;

		private T m_CurrentInterpValue;

		private T m_InterpEndValue;

		private double m_EndTimeConsumed;

		private double m_StartTimeConsumed;

		private readonly List<BufferedItem> m_Buffer = new List<BufferedItem>(100);

		private const int k_BufferCountLimit = 100;

		private BufferedItem m_LastBufferedItemReceived;

		private int m_NbItemsReceivedThisFrame;

		private int m_LifetimeConsumedCount;

		private bool InvalidState
		{
			get
			{
				if (m_Buffer.Count == 0)
				{
					return m_LifetimeConsumedCount == 0;
				}
				return false;
			}
		}

		public void Clear()
		{
			m_Buffer.Clear();
			m_EndTimeConsumed = 0.0;
			m_StartTimeConsumed = 0.0;
		}

		public void ResetTo(T targetValue, double serverTime)
		{
			m_LifetimeConsumedCount = 1;
			m_InterpStartValue = targetValue;
			m_InterpEndValue = targetValue;
			m_CurrentInterpValue = targetValue;
			m_Buffer.Clear();
			m_EndTimeConsumed = 0.0;
			m_StartTimeConsumed = 0.0;
			Update(0f, serverTime, serverTime);
		}

		private void TryConsumeFromBuffer(double renderTime, double serverTime)
		{
			int num = 0;
			if (!(renderTime >= m_EndTimeConsumed))
			{
				return;
			}
			BufferedItem? bufferedItem = null;
			for (int num2 = m_Buffer.Count - 1; num2 >= 0; num2--)
			{
				BufferedItem value = m_Buffer[num2];
				if (value.TimeSent <= serverTime)
				{
					if (!bufferedItem.HasValue || value.TimeSent > bufferedItem.Value.TimeSent)
					{
						if (m_LifetimeConsumedCount == 0)
						{
							m_StartTimeConsumed = value.TimeSent;
							m_InterpStartValue = value.Item;
						}
						else if (num == 0)
						{
							m_StartTimeConsumed = m_EndTimeConsumed;
							m_InterpStartValue = m_InterpEndValue;
						}
						if (value.TimeSent > m_EndTimeConsumed)
						{
							bufferedItem = value;
							m_EndTimeConsumed = value.TimeSent;
							m_InterpEndValue = value.Item;
						}
					}
					m_Buffer.RemoveAt(num2);
					num++;
					m_LifetimeConsumedCount++;
				}
			}
		}

		public T Update(float deltaTime, NetworkTime serverTime)
		{
			return Update(deltaTime, serverTime.TimeTicksAgo(1).Time, serverTime.Time);
		}

		public T Update(float deltaTime, double renderTime, double serverTime)
		{
			TryConsumeFromBuffer(renderTime, serverTime);
			if (InvalidState)
			{
				throw new InvalidOperationException("trying to update interpolator when no data has been added to it yet");
			}
			if (m_LifetimeConsumedCount >= 1)
			{
				float num = 1f;
				double num2 = m_EndTimeConsumed - m_StartTimeConsumed;
				if (num2 > 9.999999439624929E-11)
				{
					num = (float)((renderTime - m_StartTimeConsumed) / num2);
					if (num < 0f)
					{
						if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
						{
							NetworkLog.LogError(string.Format("renderTime was before m_StartTimeConsumed. This should never happen. {0} is {1}, {2} is {3}", "renderTime", renderTime, "m_StartTimeConsumed", m_StartTimeConsumed));
						}
						num = 0f;
					}
					if (num > MaxInterpolationBound)
					{
						num = 1f;
					}
				}
				T end = InterpolateUnclamped(m_InterpStartValue, m_InterpEndValue, num);
				m_CurrentInterpValue = Interpolate(m_CurrentInterpValue, end, deltaTime / MaximumInterpolationTime);
			}
			m_NbItemsReceivedThisFrame = 0;
			return m_CurrentInterpValue;
		}

		public void AddMeasurement(T newMeasurement, double sentTime)
		{
			m_NbItemsReceivedThisFrame++;
			if (m_NbItemsReceivedThisFrame > 100)
			{
				if (m_LastBufferedItemReceived.TimeSent < sentTime)
				{
					m_LastBufferedItemReceived = new BufferedItem(newMeasurement, sentTime);
					ResetTo(newMeasurement, sentTime);
					m_Buffer.Add(m_LastBufferedItemReceived);
				}
			}
			else if (sentTime > m_EndTimeConsumed || m_LifetimeConsumedCount == 0)
			{
				m_LastBufferedItemReceived = new BufferedItem(newMeasurement, sentTime);
				m_Buffer.Add(m_LastBufferedItemReceived);
			}
		}

		public T GetInterpolatedValue()
		{
			return m_CurrentInterpValue;
		}

		protected abstract T Interpolate(T start, T end, float time);

		protected abstract T InterpolateUnclamped(T start, T end, float time);
	}
}
