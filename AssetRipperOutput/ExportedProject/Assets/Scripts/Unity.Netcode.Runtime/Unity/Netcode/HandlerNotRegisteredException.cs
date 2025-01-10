using System;

namespace Unity.Netcode
{
	internal class HandlerNotRegisteredException : SystemException
	{
		public HandlerNotRegisteredException()
		{
		}

		public HandlerNotRegisteredException(string issue)
			: base(issue)
		{
		}
	}
}
