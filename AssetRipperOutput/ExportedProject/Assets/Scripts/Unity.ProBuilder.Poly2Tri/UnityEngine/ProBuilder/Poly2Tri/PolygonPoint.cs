namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class PolygonPoint : TriangulationPoint
	{
		public PolygonPoint Next { get; set; }

		public PolygonPoint Previous { get; set; }

		public PolygonPoint(double x, double y, int index = -1)
			: base(x, y, index)
		{
		}
	}
}
