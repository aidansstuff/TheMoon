using System.Collections.Generic;

namespace Unity.Services.Qos.ErrorMitigation
{
	internal class StatusCodePolicyConfig
	{
		private IDictionary<long, bool> _statusCodesToHandleDict = new Dictionary<long, bool>
		{
			{ 408L, true },
			{ 429L, true },
			{ 502L, true },
			{ 503L, true },
			{ 504L, true }
		};

		public void HandleStatusCode(long code)
		{
			if (_statusCodesToHandleDict.ContainsKey(code))
			{
				_statusCodesToHandleDict[code] = true;
			}
			else
			{
				_statusCodesToHandleDict.Add(new KeyValuePair<long, bool>(code, value: true));
			}
		}

		public void DontHandleStatusCode(long code)
		{
			if (_statusCodesToHandleDict.ContainsKey(code))
			{
				_statusCodesToHandleDict[code] = false;
			}
			else
			{
				_statusCodesToHandleDict.Add(new KeyValuePair<long, bool>(code, value: false));
			}
		}

		public void Clear()
		{
			_statusCodesToHandleDict.Clear();
		}

		public bool IsHandledStatusCode(long code)
		{
			return _statusCodesToHandleDict.Contains(new KeyValuePair<long, bool>(code, value: true));
		}
	}
}
