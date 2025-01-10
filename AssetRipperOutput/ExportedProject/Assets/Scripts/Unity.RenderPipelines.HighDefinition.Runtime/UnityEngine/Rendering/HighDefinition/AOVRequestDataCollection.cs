using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Rendering.HighDefinition
{
	public class AOVRequestDataCollection : IEnumerable<AOVRequestData>, IEnumerable, IDisposable
	{
		private List<AOVRequestData> m_AOVRequestData;

		public AOVRequestDataCollection(List<AOVRequestData> aovRequestData)
		{
			m_AOVRequestData = aovRequestData;
		}

		public IEnumerator<AOVRequestData> GetEnumerator()
		{
			IEnumerable<AOVRequestData> aOVRequestData = m_AOVRequestData;
			return (aOVRequestData ?? Enumerable.Empty<AOVRequestData>()).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Dispose()
		{
			if (m_AOVRequestData != null)
			{
				ListPool<AOVRequestData>.Release(m_AOVRequestData);
				m_AOVRequestData = null;
			}
		}
	}
}
