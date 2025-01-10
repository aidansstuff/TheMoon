using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
	[Serializable]
	public class PositionConfiguration
	{
		[Tooltip("The position of the Net Stats Monitor from left to right in the range from 0 to 1. 0 is flush left, 0.5 is centered, and 1 is flush right.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float m_PositionLeftToRight;

		[Tooltip("The position of the Net Stats Monitor from top to bottom in the range from 0 to 1. 0 is flush to the top, 0.5 is centered, and 1 is flush to the bottom.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float m_PositionTopToBottom;

		[field: Tooltip("If enabled, the position here will override the position set by the USS styling. Disable this options if you would like to use the position from the USS styling instead.")]
		[field: SerializeField]
		public bool OverridePosition { get; set; } = true;


		public float PositionLeftToRight
		{
			get
			{
				return m_PositionLeftToRight;
			}
			set
			{
				m_PositionLeftToRight = Mathf.Clamp(value, 0f, 1f);
			}
		}

		public float PositionTopToBottom
		{
			get
			{
				return m_PositionTopToBottom;
			}
			set
			{
				m_PositionTopToBottom = Mathf.Clamp(value, 0f, 1f);
			}
		}
	}
}
