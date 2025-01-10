namespace Unity.Networking.Transport
{
	public struct InboundRecvBuffer
	{
		public unsafe byte* buffer;

		public int bufferLength;

		public unsafe InboundRecvBuffer Slice(int offset)
		{
			InboundRecvBuffer result = default(InboundRecvBuffer);
			result.buffer = buffer + offset;
			result.bufferLength = bufferLength - offset;
			return result;
		}
	}
}
