using System;

namespace Unity.Services.Core.Internal
{
	internal class MissingComponent : IServiceComponent
	{
		public Type IntendedType { get; }

		internal MissingComponent(Type intendedType)
		{
			IntendedType = intendedType;
		}
	}
}
