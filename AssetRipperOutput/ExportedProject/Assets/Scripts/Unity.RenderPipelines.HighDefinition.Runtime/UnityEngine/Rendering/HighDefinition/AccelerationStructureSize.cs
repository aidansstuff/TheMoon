namespace UnityEngine.Rendering.HighDefinition
{
	internal struct AccelerationStructureSize
	{
		public ulong memUsage;

		public uint instCount;

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is AccelerationStructureSize accelerationStructureSize))
			{
				return false;
			}
			if (memUsage == accelerationStructureSize.memUsage)
			{
				return instCount == accelerationStructureSize.instCount;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(AccelerationStructureSize lhs, AccelerationStructureSize rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(AccelerationStructureSize lhs, AccelerationStructureSize rhs)
		{
			return !(lhs == rhs);
		}
	}
}
