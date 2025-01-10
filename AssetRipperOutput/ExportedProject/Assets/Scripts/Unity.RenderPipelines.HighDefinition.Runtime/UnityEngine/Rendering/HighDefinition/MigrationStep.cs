using System;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class MigrationStep
	{
		public static MigrationStep<TVersion, TTarget> New<TVersion, TTarget>(TVersion version, Action<TTarget> action) where TVersion : struct, IConvertible where TTarget : class, IVersionable<TVersion>
		{
			return new MigrationStep<TVersion, TTarget>(version, action);
		}
	}
	public struct MigrationStep<TVersion, TTarget> : IEquatable<MigrationStep<TVersion, TTarget>> where TVersion : struct, IConvertible where TTarget : class, IVersionable<TVersion>
	{
		private readonly Action<TTarget> m_MigrationAction;

		public readonly TVersion Version;

		public MigrationStep(TVersion version, Action<TTarget> action)
		{
			Version = version;
			m_MigrationAction = action;
		}

		public void Migrate(TTarget target)
		{
			if ((int)(object)target.version < (int)(object)Version)
			{
				m_MigrationAction(target);
				target.version = Version;
			}
		}

		public bool Equals(MigrationStep<TVersion, TTarget> other)
		{
			return Version.Equals(other.Version);
		}
	}
}
