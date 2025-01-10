using System;

namespace Unity.Services.Core.Internal
{
	internal class DependencyTreeComponentHashException : HashException
	{
		public DependencyTreeComponentHashException(int hash)
			: base(hash)
		{
		}

		public DependencyTreeComponentHashException(int hash, string message)
			: base(hash, message)
		{
		}

		public DependencyTreeComponentHashException(int hash, string message, Exception inner)
			: base(hash, message, inner)
		{
		}
	}
}
