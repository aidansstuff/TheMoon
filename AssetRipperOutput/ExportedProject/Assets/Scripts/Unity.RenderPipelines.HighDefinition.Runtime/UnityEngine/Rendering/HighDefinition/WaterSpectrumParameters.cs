namespace UnityEngine.Rendering.HighDefinition
{
	public struct WaterSpectrumParameters
	{
		internal int numActiveBands;

		internal Vector4 patchSizes;

		internal Vector4 patchWindSpeed;

		internal Vector4 patchWindDirDampener;

		internal Vector4 patchWindOrientation;

		public static bool operator ==(WaterSpectrumParameters a, WaterSpectrumParameters b)
		{
			if (a.numActiveBands == b.numActiveBands && a.patchSizes == b.patchSizes && a.patchWindSpeed == b.patchWindSpeed && a.patchWindDirDampener == b.patchWindDirDampener)
			{
				return a.patchWindOrientation == b.patchWindOrientation;
			}
			return false;
		}

		public static bool operator !=(WaterSpectrumParameters a, WaterSpectrumParameters b)
		{
			if (a.numActiveBands == b.numActiveBands && !(a.patchSizes != b.patchSizes) && !(a.patchWindSpeed != b.patchWindSpeed) && !(a.patchWindDirDampener != b.patchWindDirDampener))
			{
				return a.patchWindOrientation != b.patchWindOrientation;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object o)
		{
			if (o is WaterSpectrumParameters waterSpectrumParameters)
			{
				return this == waterSpectrumParameters;
			}
			return false;
		}
	}
}
