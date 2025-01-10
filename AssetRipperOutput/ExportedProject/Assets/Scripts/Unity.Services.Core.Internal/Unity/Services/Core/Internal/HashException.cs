using System;

namespace Unity.Services.Core.Internal
{
	internal class HashException : Exception
	{
		public int Hash { get; }

		public HashException(int hash)
		{
			Hash = hash;
		}

		public HashException(int hash, string message)
		{
			Hash = hash;
		}

		public HashException(int hash, string message, Exception inner)
			: base(message, inner)
		{
			Hash = hash;
		}
	}
}
