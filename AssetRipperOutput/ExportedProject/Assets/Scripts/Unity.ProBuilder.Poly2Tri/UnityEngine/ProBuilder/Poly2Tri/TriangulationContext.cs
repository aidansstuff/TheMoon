using System.Collections.Generic;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal abstract class TriangulationContext
	{
		public readonly List<DelaunayTriangle> Triangles = new List<DelaunayTriangle>();

		public readonly List<TriangulationPoint> Points = new List<TriangulationPoint>();

		public TriangulationDebugContext DebugContext { get; protected set; }

		public TriangulationMode TriangulationMode { get; protected set; }

		public Triangulatable Triangulatable { get; private set; }

		public int StepCount { get; private set; }

		public abstract TriangulationAlgorithm Algorithm { get; }

		public virtual bool IsDebugEnabled { get; protected set; }

		public DTSweepDebugContext DTDebugContext => DebugContext as DTSweepDebugContext;

		public void Done()
		{
			StepCount++;
		}

		public virtual void PrepareTriangulation(Triangulatable t)
		{
			Triangulatable = t;
			TriangulationMode = t.TriangulationMode;
			t.Prepare(this);
		}

		public abstract TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b);

		public void Update(string message)
		{
		}

		public virtual void Clear()
		{
			Points.Clear();
			if (DebugContext != null)
			{
				DebugContext.Clear();
			}
			StepCount = 0;
		}
	}
}
