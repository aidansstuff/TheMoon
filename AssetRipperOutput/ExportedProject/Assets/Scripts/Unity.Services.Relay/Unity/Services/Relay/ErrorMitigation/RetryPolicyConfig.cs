using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Services.Relay.ErrorMitigation
{
	internal class RetryPolicyConfig
	{
		private float _jitterMagnitude = 1f;

		private float _delayScale = 1f;

		private float _maxDelayTime = 8f;

		private List<ExceptionPredicate> _exceptionsToHandle = new List<ExceptionPredicate>();

		public uint MaxRetries { get; set; } = 4u;


		public float JitterMagnitude
		{
			get
			{
				return _jitterMagnitude;
			}
			set
			{
				_jitterMagnitude = Mathf.Clamp(value, 0.001f, 1f);
			}
		}

		public float DelayScale
		{
			get
			{
				return _delayScale;
			}
			set
			{
				_delayScale = Mathf.Clamp(value, 0.05f, 1f);
			}
		}

		public float MaxDelayTime
		{
			get
			{
				return _maxDelayTime;
			}
			set
			{
				_maxDelayTime = Mathf.Clamp(value, 0.1f, 60f);
			}
		}

		public void HandleException<TException>() where TException : Exception
		{
			_exceptionsToHandle.Add((Exception exception) => (!(exception is TException)) ? null : exception);
		}

		public void HandleException<TException>(Func<TException, bool> condition) where TException : Exception
		{
			_exceptionsToHandle.Add((Exception exception) => (!(exception is TException arg) || !condition(arg)) ? null : exception);
		}

		public bool IsHandledException(Exception e)
		{
			if (_exceptionsToHandle != null)
			{
				foreach (ExceptionPredicate item in _exceptionsToHandle)
				{
					if (item(e) == e)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
