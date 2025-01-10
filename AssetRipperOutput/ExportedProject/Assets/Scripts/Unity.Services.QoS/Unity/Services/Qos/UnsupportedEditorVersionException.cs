using System;

namespace Unity.Services.Qos
{
	internal class UnsupportedEditorVersionException : Exception
	{
		public UnsupportedEditorVersionException()
		{
		}

		public UnsupportedEditorVersionException(string message)
			: base(message)
		{
		}
	}
}
