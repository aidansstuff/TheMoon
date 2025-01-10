using System.Collections.Generic;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class DTSweepPointComparator : IComparer<TriangulationPoint>
	{
		public int Compare(TriangulationPoint p1, TriangulationPoint p2)
		{
			if (p1.Y < p2.Y)
			{
				return -1;
			}
			if (p1.Y > p2.Y)
			{
				return 1;
			}
			if (p1.X < p2.X)
			{
				return -1;
			}
			if (p1.X > p2.X)
			{
				return 1;
			}
			return 0;
		}
	}
}
