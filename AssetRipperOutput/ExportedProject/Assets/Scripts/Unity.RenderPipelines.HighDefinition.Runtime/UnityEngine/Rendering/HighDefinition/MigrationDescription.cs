using System;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class MigrationDescription
	{
		public static T LastVersion<T>() where T : struct, IConvertible
		{
			return TypeInfo.GetEnumLastValue<T>();
		}

		public static MigrationDescription<TVersion, TTarget> New<TVersion, TTarget>(params MigrationStep<TVersion, TTarget>[] steps) where TVersion : struct, IConvertible where TTarget : class, IVersionable<TVersion>
		{
			return new MigrationDescription<TVersion, TTarget>(steps);
		}
	}
	public struct MigrationDescription<TVersion, TTarget> where TVersion : struct, IConvertible where TTarget : class, IVersionable<TVersion>
	{
		private readonly MigrationStep<TVersion, TTarget>[] Steps;

		public MigrationDescription(params MigrationStep<TVersion, TTarget>[] steps)
		{
			Array.Sort(steps, (MigrationStep<TVersion, TTarget> l, MigrationStep<TVersion, TTarget> r) => Compare(l.Version, r.Version));
			Steps = steps;
		}

		public bool Migrate(TTarget target)
		{
			if (Equals(target.version, Steps[Steps.Length - 1].Version))
			{
				return false;
			}
			for (int i = 0; i < Steps.Length; i++)
			{
				if (Compare(target.version, Steps[i].Version) < 0)
				{
					Steps[i].Migrate(target);
					target.version = Steps[i].Version;
				}
			}
			return true;
		}

		public void ExecuteStep(TTarget target, TVersion stepVersion)
		{
			for (int i = 0; i < Steps.Length; i++)
			{
				if (Equals(Steps[i].Version, stepVersion))
				{
					Steps[i].Migrate(target);
					break;
				}
			}
		}

		private static bool Equals(TVersion l, TVersion r)
		{
			return Compare(l, r) == 0;
		}

		private static int Compare(TVersion l, TVersion r)
		{
			return (int)(object)l - (int)(object)r;
		}
	}
}
