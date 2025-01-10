using System;

namespace Unity.Netcode
{
	internal class InvalidMessageStructureException : SystemException
	{
		public InvalidMessageStructureException()
		{
		}

		public InvalidMessageStructureException(string issue)
			: base(issue)
		{
		}
	}
}
