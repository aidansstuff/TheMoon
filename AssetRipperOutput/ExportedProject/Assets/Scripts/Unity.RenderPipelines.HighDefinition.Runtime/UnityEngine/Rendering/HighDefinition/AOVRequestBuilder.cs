using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	public class AOVRequestBuilder : IDisposable
	{
		private List<AOVRequestData> m_AOVRequestDataData;

		public AOVRequestBuilder Add(AOVRequest settings, AOVRequestBufferAllocator bufferAllocator, List<GameObject> includedLightList, AOVBuffers[] aovBuffers, FramePassCallback callback)
		{
			(m_AOVRequestDataData ?? (m_AOVRequestDataData = ListPool<AOVRequestData>.Get())).Add(new AOVRequestData(settings, bufferAllocator, includedLightList, aovBuffers, callback));
			return this;
		}

		public AOVRequestBuilder Add(AOVRequest settings, AOVRequestBufferAllocator bufferAllocator, List<GameObject> includedLightList, AOVBuffers[] aovBuffers, CustomPassAOVBuffers[] customPassAovBuffers, AOVRequestCustomPassBufferAllocator customPassbufferAllocator, FramePassCallbackEx callback)
		{
			(m_AOVRequestDataData ?? (m_AOVRequestDataData = ListPool<AOVRequestData>.Get())).Add(new AOVRequestData(settings, bufferAllocator, includedLightList, aovBuffers, customPassAovBuffers, customPassbufferAllocator, callback));
			return this;
		}

		public AOVRequestDataCollection Build()
		{
			AOVRequestDataCollection result = new AOVRequestDataCollection(m_AOVRequestDataData);
			m_AOVRequestDataData = null;
			return result;
		}

		public void Dispose()
		{
			if (m_AOVRequestDataData != null)
			{
				ListPool<AOVRequestData>.Release(m_AOVRequestDataData);
				m_AOVRequestDataData = null;
			}
		}
	}
}
