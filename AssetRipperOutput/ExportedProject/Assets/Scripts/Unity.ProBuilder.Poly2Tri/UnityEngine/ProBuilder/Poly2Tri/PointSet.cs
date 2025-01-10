using System.Collections.Generic;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class PointSet : Triangulatable
	{
		public IList<TriangulationPoint> Points { get; private set; }

		public IList<DelaunayTriangle> Triangles { get; private set; }

		public virtual TriangulationMode TriangulationMode => TriangulationMode.Unconstrained;

		public PointSet(List<TriangulationPoint> points)
		{
			Points = new List<TriangulationPoint>(points);
		}

		public void AddTriangle(DelaunayTriangle t)
		{
			Triangles.Add(t);
		}

		public void AddTriangles(IEnumerable<DelaunayTriangle> list)
		{
			foreach (DelaunayTriangle item in list)
			{
				Triangles.Add(item);
			}
		}

		public void ClearTriangles()
		{
			Triangles.Clear();
		}

		public virtual void Prepare(TriangulationContext tcx)
		{
			if (Triangles == null)
			{
				Triangles = new List<DelaunayTriangle>(Points.Count);
			}
			else
			{
				Triangles.Clear();
			}
			tcx.Points.AddRange(Points);
		}
	}
}
