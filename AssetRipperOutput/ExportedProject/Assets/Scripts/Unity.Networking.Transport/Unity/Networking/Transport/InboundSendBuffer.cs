namespace Unity.Networking.Transport
{
	public struct InboundSendBuffer
	{
		public unsafe byte* buffer;

		public unsafe byte* bufferWithHeaders;

		public int bufferLength;

		public int bufferWithHeadersLength;

		public int headerPadding;

		public unsafe void SetBufferFrombufferWithHeaders()
		{
			buffer = bufferWithHeaders + headerPadding;
			bufferLength = bufferWithHeadersLength - headerPadding;
		}
	}
}
