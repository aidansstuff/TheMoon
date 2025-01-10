using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Playables;

namespace UnityEngine.VFX
{
	internal class VisualEffectControlTrackController
	{
		private struct Event
		{
			public enum ClipType
			{
				None = 0,
				Enter = 1,
				Exit = 2
			}

			public int nameId;

			public VFXEventAttribute attribute;

			public double time;

			public int clipIndex;

			public ClipType clipType;
		}

		private struct Clip
		{
			public int enter;

			public int exit;
		}

		private struct Chunk
		{
			public bool scrubbing;

			public bool reinitEnter;

			public bool reinitExit;

			public uint startSeed;

			public double begin;

			public double end;

			public uint prewarmCount;

			public float prewarmDeltaTime;

			public double prewarmOffset;

			public int prewarmEvent;

			public Event[] events;

			public Clip[] clips;
		}

		private class VisualEffectControlPlayableBehaviourComparer : IComparer<VisualEffectControlPlayableBehaviour>
		{
			public int Compare(VisualEffectControlPlayableBehaviour x, VisualEffectControlPlayableBehaviour y)
			{
				return x.clipStart.CompareTo(y.clipStart);
			}
		}

		private const int kErrorIndex = int.MinValue;

		private int m_LastChunk = int.MinValue;

		private int m_LastEvent = int.MinValue;

		private double m_LastPlayableTime = double.MinValue;

		private List<int> m_EventListIndexCache = new List<int>();

		private VisualEffect m_Target;

		private bool m_BackupReseedOnPlay;

		private uint m_BackupStartSeed;

		private Chunk[] m_Chunks;

		private void OnEnterChunk(int currentChunk)
		{
			Chunk chunk = m_Chunks[currentChunk];
			if (chunk.reinitEnter)
			{
				m_Target.resetSeedOnPlay = false;
				m_Target.startSeed = chunk.startSeed;
				m_Target.Reinit(sendInitialEventAndPrewarm: false);
				if (chunk.prewarmCount != 0)
				{
					m_Target.SendEvent(chunk.prewarmEvent);
					m_Target.Simulate(chunk.prewarmDeltaTime, chunk.prewarmCount);
				}
			}
		}

		private void OnLeaveChunk(int previousChunkIndex, bool leavingGoingBeforeClip)
		{
			Chunk chunk = m_Chunks[previousChunkIndex];
			if (chunk.reinitExit)
			{
				m_Target.Reinit(sendInitialEventAndPrewarm: false);
			}
			else
			{
				ProcessNoScrubbingEvents(chunk, m_LastPlayableTime, leavingGoingBeforeClip ? double.NegativeInfinity : double.PositiveInfinity);
			}
			RestoreVFXState(chunk.scrubbing, chunk.reinitEnter);
		}

		private bool IsTimeInChunk(double time, int index)
		{
			Chunk chunk = m_Chunks[index];
			if (chunk.begin <= time)
			{
				return time < chunk.end;
			}
			return false;
		}

		public void Update(double playableTime, float deltaTime)
		{
			bool flag = (double)deltaTime == 0.0;
			int num = int.MinValue;
			if (m_LastChunk != num && IsTimeInChunk(playableTime, m_LastChunk))
			{
				num = m_LastChunk;
			}
			if (num == int.MinValue)
			{
				uint num2 = ((m_LastChunk != int.MinValue) ? ((uint)m_LastEvent) : 0u);
				for (uint num3 = num2; num3 < num2 + m_Chunks.Length; num3++)
				{
					int num4 = (int)(num3 % m_Chunks.Length);
					if (IsTimeInChunk(playableTime, num4))
					{
						num = num4;
						break;
					}
				}
			}
			bool flag2 = false;
			if (m_LastChunk != num)
			{
				if (m_LastChunk != int.MinValue)
				{
					bool leavingGoingBeforeClip = playableTime < m_Chunks[m_LastChunk].begin;
					OnLeaveChunk(m_LastChunk, leavingGoingBeforeClip);
				}
				if (num != int.MinValue)
				{
					OnEnterChunk(num);
					flag2 = true;
				}
				m_LastChunk = num;
				m_LastEvent = int.MinValue;
			}
			if (num != int.MinValue)
			{
				Chunk chunk = m_Chunks[num];
				if (chunk.scrubbing)
				{
					m_Target.pause = flag;
					double num5 = chunk.begin + (double)m_Target.time;
					if (!flag2)
					{
						num5 -= chunk.prewarmOffset;
					}
					if (!(playableTime < m_LastPlayableTime))
					{
						if (Math.Abs(m_LastPlayableTime - num5) < (double)VFXManager.maxDeltaTime)
						{
							num5 = m_LastPlayableTime;
						}
					}
					else
					{
						num5 = chunk.begin;
						m_LastEvent = int.MinValue;
						OnEnterChunk(m_LastChunk);
					}
					double num6 = ((!flag) ? (playableTime - (double)VFXManager.fixedTimeStep) : playableTime);
					if (m_LastPlayableTime < num5)
					{
						List<int> eventListIndexCache = m_EventListIndexCache;
						GetEventsIndex(chunk, m_LastPlayableTime, num5, m_LastEvent, eventListIndexCache);
						foreach (int item in eventListIndexCache)
						{
							ProcessEvent(item, chunk);
						}
					}
					if (num5 < num6)
					{
						List<int> eventListIndexCache2 = m_EventListIndexCache;
						GetEventsIndex(chunk, num5, num6, m_LastEvent, eventListIndexCache2);
						int count = eventListIndexCache2.Count;
						int num7 = 0;
						float maxScrubTime = VFXManager.maxScrubTime;
						float num8 = VFXManager.maxDeltaTime;
						if (num6 - num5 > (double)maxScrubTime)
						{
							num8 = (float)((num6 - num5) * (double)VFXManager.maxDeltaTime / (double)maxScrubTime);
						}
						while (num5 < num6)
						{
							int num9 = int.MinValue;
							uint num10;
							if (num7 < count)
							{
								num9 = eventListIndexCache2.ElementAt(num7++);
								num10 = (uint)((chunk.events[num9].time - num5) / (double)num8);
							}
							else
							{
								num10 = (uint)((num6 - num5) / (double)num8);
								if (num10 == 0)
								{
									break;
								}
							}
							if (num10 != 0)
							{
								m_Target.Simulate(num8, num10);
								num5 += (double)(num8 * (float)num10);
							}
							ProcessEvent(num9, chunk);
						}
					}
					if (num5 < playableTime)
					{
						List<int> eventListIndexCache3 = m_EventListIndexCache;
						GetEventsIndex(chunk, num5, playableTime, m_LastEvent, eventListIndexCache3);
						foreach (int item2 in eventListIndexCache3)
						{
							ProcessEvent(item2, chunk);
						}
					}
				}
				else
				{
					m_Target.pause = false;
					ProcessNoScrubbingEvents(chunk, m_LastPlayableTime, playableTime);
				}
			}
			m_LastPlayableTime = playableTime;
		}

		private void ProcessNoScrubbingEvents(Chunk chunk, double oldTime, double newTime)
		{
			if (newTime < oldTime)
			{
				List<int> eventListIndexCache = m_EventListIndexCache;
				GetEventsIndex(chunk, newTime, oldTime, int.MinValue, eventListIndexCache);
				if (eventListIndexCache.Count <= 0)
				{
					return;
				}
				for (int num = eventListIndexCache.Count - 1; num >= 0; num--)
				{
					int num2 = eventListIndexCache[num];
					Event @event = chunk.events[num2];
					if (@event.clipType == Event.ClipType.Enter)
					{
						ProcessEvent(chunk.clips[@event.clipIndex].exit, chunk);
					}
					else if (@event.clipType == Event.ClipType.Exit)
					{
						ProcessEvent(chunk.clips[@event.clipIndex].enter, chunk);
					}
				}
				m_LastEvent = int.MinValue;
				return;
			}
			List<int> eventListIndexCache2 = m_EventListIndexCache;
			GetEventsIndex(chunk, oldTime, newTime, m_LastEvent, eventListIndexCache2);
			foreach (int item in eventListIndexCache2)
			{
				ProcessEvent(item, chunk);
			}
		}

		private void ProcessEvent(int eventIndex, Chunk currentChunk)
		{
			if (eventIndex != int.MinValue)
			{
				m_LastEvent = eventIndex;
				Event @event = currentChunk.events[eventIndex];
				m_Target.SendEvent(@event.nameId, @event.attribute);
			}
		}

		private static void GetEventsIndex(Chunk chunk, double minTime, double maxTime, int lastIndex, List<int> eventListIndex)
		{
			eventListIndex.Clear();
			for (int i = ((lastIndex != int.MinValue) ? (lastIndex + 1) : 0); i < chunk.events.Length; i++)
			{
				Event @event = chunk.events[i];
				if (!(@event.time >= maxTime))
				{
					if (minTime <= @event.time)
					{
						eventListIndex.Add(i);
					}
					continue;
				}
				break;
			}
		}

		private static VFXEventAttribute ComputeAttribute(VisualEffect vfx, EventAttributes attributes)
		{
			if (attributes.content == null || attributes.content.Length == 0)
			{
				return null;
			}
			VFXEventAttribute vfxAttribute = vfx.CreateVFXEventAttribute();
			if (attributes.content.Count((EventAttribute x) => x?.ApplyToVFX(vfxAttribute) ?? false) == 0)
			{
				return null;
			}
			return vfxAttribute;
		}

		private static IEnumerable<Event> ComputeRuntimeEvent(VisualEffectControlPlayableBehaviour behavior, VisualEffect vfx)
		{
			IEnumerable<VisualEffectPlayableSerializedEvent> eventNormalizedSpace = VFXTimeSpaceHelper.GetEventNormalizedSpace(PlayableTimeSpace.Absolute, behavior);
			foreach (VisualEffectPlayableSerializedEvent item in eventNormalizedSpace)
			{
				double time = Math.Max(behavior.clipStart, Math.Min(behavior.clipEnd, item.time));
				yield return new Event
				{
					attribute = ComputeAttribute(vfx, item.eventAttributes),
					nameId = item.name,
					time = time,
					clipIndex = -1,
					clipType = Event.ClipType.None
				};
			}
		}

		public void RestoreVFXState(bool restorePause = true, bool restoreSeedState = true)
		{
			if (!(m_Target == null))
			{
				if (restorePause)
				{
					m_Target.pause = false;
				}
				if (restoreSeedState)
				{
					m_Target.startSeed = m_BackupStartSeed;
					m_Target.resetSeedOnPlay = m_BackupReseedOnPlay;
				}
			}
		}

		public void Init(Playable playable, VisualEffect vfx, VisualEffectControlTrack parentTrack)
		{
			m_Target = vfx;
			m_BackupStartSeed = m_Target.startSeed;
			m_BackupReseedOnPlay = m_Target.resetSeedOnPlay;
			Stack<Chunk> stack = new Stack<Chunk>();
			int inputCount = playable.GetInputCount();
			List<VisualEffectControlPlayableBehaviour> list = new List<VisualEffectControlPlayableBehaviour>();
			for (int j = 0; j < inputCount; j++)
			{
				Playable input = playable.GetInput(j);
				if (!(input.GetPlayableType() != typeof(VisualEffectControlPlayableBehaviour)))
				{
					VisualEffectControlPlayableBehaviour behaviour = ((ScriptPlayable<VisualEffectControlPlayableBehaviour>)input).GetBehaviour();
					if (behaviour != null)
					{
						list.Add(behaviour);
					}
				}
			}
			list.Sort(new VisualEffectControlPlayableBehaviourComparer());
			foreach (VisualEffectControlPlayableBehaviour item2 in list)
			{
				if (!stack.Any() || item2.clipStart > stack.Peek().end || item2.scrubbing != stack.Peek().scrubbing || (!item2.scrubbing && (item2.reinitEnter || stack.Peek().reinitExit)) || item2.startSeed != stack.Peek().startSeed || item2.prewarmStepCount != 0)
				{
					stack.Push(new Chunk
					{
						begin = item2.clipStart,
						events = new Event[0],
						clips = new Clip[0],
						scrubbing = item2.scrubbing,
						startSeed = item2.startSeed,
						reinitEnter = item2.reinitEnter,
						reinitExit = item2.reinitExit,
						prewarmCount = item2.prewarmStepCount,
						prewarmDeltaTime = item2.prewarmDeltaTime,
						prewarmEvent = ((item2.prewarmEvent != null) ? ((int)item2.prewarmEvent) : 0),
						prewarmOffset = (double)item2.prewarmStepCount * (double)item2.prewarmDeltaTime
					});
				}
				Chunk item = stack.Peek();
				item.end = item2.clipEnd;
				IEnumerable<Event> source = ComputeRuntimeEvent(item2, vfx);
				if (!item.scrubbing)
				{
					var list2 = (from o in source.Select((Event e, int i) => new
						{
							evt = e,
							sourceIndex = i
						})
						orderby o.evt.time
						select o).ToList();
					Clip[] array = new Clip[item2.clipEventsCount];
					List<Event> list3 = new List<Event>();
					for (int k = 0; k < list2.Count; k++)
					{
						Event evt = list2[k].evt;
						int sourceIndex = list2[k].sourceIndex;
						if (sourceIndex < item2.clipEventsCount * 2)
						{
							int num = item.events.Length + k;
							int num2 = sourceIndex / 2;
							evt.clipIndex = num2 + item.clips.Length;
							if (sourceIndex % 2 == 0)
							{
								evt.clipType = Event.ClipType.Enter;
								array[num2].enter = num;
							}
							else
							{
								evt.clipType = Event.ClipType.Exit;
								array[num2].exit = num;
							}
							list3.Add(evt);
						}
						else
						{
							list3.Add(evt);
						}
					}
					item.clips = item.clips.Concat(array).ToArray();
					item.events = item.events.Concat(list3).ToArray();
				}
				else
				{
					source = source.OrderBy((Event o) => o.time);
					item.events = item.events.Concat(source).ToArray();
				}
				stack.Pop();
				stack.Push(item);
			}
			m_Chunks = stack.Reverse().ToArray();
		}

		public void Release()
		{
			RestoreVFXState();
		}
	}
}
