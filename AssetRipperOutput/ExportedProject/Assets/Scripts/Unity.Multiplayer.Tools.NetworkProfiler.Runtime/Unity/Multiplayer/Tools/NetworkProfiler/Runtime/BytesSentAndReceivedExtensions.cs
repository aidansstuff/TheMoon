using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
	internal static class BytesSentAndReceivedExtensions
	{
		public static BytesSentAndReceived Sum<T>(this IEnumerable<T> ts, Func<T, BytesSentAndReceived> f)
		{
			BytesSentAndReceived result = default(BytesSentAndReceived);
			foreach (T t in ts)
			{
				result += f(t);
			}
			return result;
		}
	}
}
