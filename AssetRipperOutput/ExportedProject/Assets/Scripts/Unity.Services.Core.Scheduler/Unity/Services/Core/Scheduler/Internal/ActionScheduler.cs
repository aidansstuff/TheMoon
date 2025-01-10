using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Services.Core.Internal;
using UnityEngine.LowLevel;

namespace Unity.Services.Core.Scheduler.Internal
{
	internal class ActionScheduler : IActionScheduler, IServiceComponent
	{
		private const long k_MinimumIdValue = 1L;

		internal readonly PlayerLoopSystem SchedulerLoopSystem;

		private readonly ITimeProvider m_TimeProvider;

		private readonly object m_Lock = new object();

		private readonly MinimumBinaryHeap<ScheduledInvocation> m_ScheduledActions = new MinimumBinaryHeap<ScheduledInvocation>(new ScheduledInvocationComparer());

		private readonly Dictionary<long, ScheduledInvocation> m_IdScheduledInvocationMap = new Dictionary<long, ScheduledInvocation>();

		private readonly List<ScheduledInvocation> m_ExpiredActions = new List<ScheduledInvocation>();

		private long m_NextId = 1L;

		public int ScheduledActionsCount => m_ScheduledActions.Count;

		public ActionScheduler()
			: this(new UtcTimeProvider())
		{
		}

		public ActionScheduler(ITimeProvider timeProvider)
		{
			m_TimeProvider = timeProvider;
			SchedulerLoopSystem = new PlayerLoopSystem
			{
				type = typeof(ActionScheduler),
				updateDelegate = ExecuteExpiredActions
			};
		}

		public long ScheduleAction([NotNull] Action action, double delaySeconds = 0.0)
		{
			if (delaySeconds < 0.0)
			{
				throw new ArgumentException("delaySeconds can not be negative");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			lock (m_Lock)
			{
				ScheduledInvocation scheduledInvocation = new ScheduledInvocation
				{
					Action = action,
					InvocationTime = m_TimeProvider.Now.AddSeconds(delaySeconds),
					ActionId = m_NextId++
				};
				if (m_NextId < 1)
				{
					m_NextId = 1L;
				}
				m_ScheduledActions.Insert(scheduledInvocation);
				m_IdScheduledInvocationMap.Add(scheduledInvocation.ActionId, scheduledInvocation);
				return scheduledInvocation.ActionId;
			}
		}

		public void CancelAction(long actionId)
		{
			lock (m_Lock)
			{
				if (m_IdScheduledInvocationMap.TryGetValue(actionId, out var value))
				{
					m_ScheduledActions.Remove(value);
					m_IdScheduledInvocationMap.Remove(value.ActionId);
				}
			}
		}

		internal void ExecuteExpiredActions()
		{
			lock (m_Lock)
			{
				m_ExpiredActions.Clear();
				while (m_ScheduledActions.Count > 0 && m_ScheduledActions.Min?.InvocationTime <= m_TimeProvider.Now)
				{
					ScheduledInvocation scheduledInvocation = m_ScheduledActions.ExtractMin();
					m_ExpiredActions.Add(scheduledInvocation);
					m_ScheduledActions.Remove(scheduledInvocation);
					m_IdScheduledInvocationMap.Remove(scheduledInvocation.ActionId);
				}
				foreach (ScheduledInvocation expiredAction in m_ExpiredActions)
				{
					try
					{
						expiredAction.Action();
					}
					catch (Exception exception)
					{
						CoreLogger.LogException(exception);
					}
				}
			}
		}

		internal static void UpdateCurrentPlayerLoopWith(List<PlayerLoopSystem> subSystemList, PlayerLoopSystem currentPlayerLoop)
		{
			currentPlayerLoop.subSystemList = subSystemList.ToArray();
			PlayerLoop.SetPlayerLoop(currentPlayerLoop);
		}

		public void JoinPlayerLoopSystem()
		{
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
			List<PlayerLoopSystem> list = new List<PlayerLoopSystem>(currentPlayerLoop.subSystemList);
			if (!list.Contains(SchedulerLoopSystem))
			{
				list.Add(SchedulerLoopSystem);
				UpdateCurrentPlayerLoopWith(list, currentPlayerLoop);
			}
		}

		public void QuitPlayerLoopSystem()
		{
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
			List<PlayerLoopSystem> list = new List<PlayerLoopSystem>(currentPlayerLoop.subSystemList);
			if (list.Remove(SchedulerLoopSystem))
			{
				UpdateCurrentPlayerLoopWith(list, currentPlayerLoop);
			}
		}
	}
}
