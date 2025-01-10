namespace UnityEngine.Rendering.HighDefinition
{
	internal abstract class MRTBufferManager
	{
		protected int m_BufferCount;

		protected RenderTargetIdentifier[] m_RTIDs;

		protected RTHandle[] m_RTs;

		protected int[] m_TextureShaderIDs;

		public int bufferCount => m_BufferCount;

		public MRTBufferManager(int maxBufferCount)
		{
			m_BufferCount = maxBufferCount;
			m_RTIDs = new RenderTargetIdentifier[maxBufferCount];
			m_RTs = new RTHandle[maxBufferCount];
			m_TextureShaderIDs = new int[maxBufferCount];
		}

		public RenderTargetIdentifier[] GetBuffersRTI()
		{
			for (int i = 0; i < m_BufferCount; i++)
			{
				m_RTIDs[i] = m_RTs[i].nameID;
			}
			return m_RTIDs;
		}

		public RTHandle[] GetBuffers()
		{
			return m_RTs;
		}

		public RTHandle GetBuffer(int index)
		{
			return m_RTs[index];
		}

		public abstract void CreateBuffers();

		public virtual void BindBufferAsTextures(CommandBuffer cmd)
		{
			for (int i = 0; i < m_BufferCount; i++)
			{
				cmd.SetGlobalTexture(m_TextureShaderIDs[i], m_RTs[i]);
			}
		}

		public virtual void DestroyBuffers()
		{
			for (int i = 0; i < m_BufferCount; i++)
			{
				RTHandles.Release(m_RTs[i]);
				m_RTs[i] = null;
			}
		}
	}
}
