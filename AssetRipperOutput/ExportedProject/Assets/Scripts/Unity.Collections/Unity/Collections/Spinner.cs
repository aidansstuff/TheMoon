using System.Threading;

namespace Unity.Collections
{
	internal struct Spinner
	{
		private int m_value;

		public void Lock()
		{
			while (Interlocked.CompareExchange(ref m_value, 1, 0) != 0)
			{
			}
			Interlocked.MemoryBarrier();
		}

		public void Unlock()
		{
			Interlocked.MemoryBarrier();
			while (1 != Interlocked.CompareExchange(ref m_value, 0, 1))
			{
			}
		}
	}
}
