namespace Unity.Services.Core.Internal
{
	public readonly struct CoreRegistration
	{
		private readonly IPackageRegistry m_Registry;

		private readonly int m_PackageHash;

		internal CoreRegistration(IPackageRegistry registry, int packageHash)
		{
			m_Registry = registry;
			m_PackageHash = packageHash;
		}

		public CoreRegistration DependsOn<T>() where T : IServiceComponent
		{
			m_Registry.RegisterDependency<T>(m_PackageHash);
			return this;
		}

		public CoreRegistration OptionallyDependsOn<T>() where T : IServiceComponent
		{
			m_Registry.RegisterOptionalDependency<T>(m_PackageHash);
			return this;
		}

		public CoreRegistration ProvidesComponent<T>() where T : IServiceComponent
		{
			m_Registry.RegisterProvision<T>(m_PackageHash);
			return this;
		}
	}
}
