using System;

namespace Unity.Services.Core.Internal
{
	internal class DependencyTreePackageHashException : HashException
	{
		public DependencyTreePackageHashException(int hash)
			: base(hash)
		{
		}

		public DependencyTreePackageHashException(int hash, string message)
			: base(hash, message)
		{
		}

		public DependencyTreePackageHashException(int hash, string message, Exception inner)
			: base(hash, message, inner)
		{
		}
	}
}
