namespace UnityEngine.Rendering.HighDefinition
{
	public static class HDLightTypeExtension
	{
		public static bool IsSpot(this HDLightTypeAndShape type)
		{
			if (type != HDLightTypeAndShape.BoxSpot && type != HDLightTypeAndShape.PyramidSpot)
			{
				return type == HDLightTypeAndShape.ConeSpot;
			}
			return true;
		}

		public static bool IsArea(this HDLightTypeAndShape type)
		{
			if (type != HDLightTypeAndShape.TubeArea && type != HDLightTypeAndShape.RectangleArea)
			{
				return type == HDLightTypeAndShape.DiscArea;
			}
			return true;
		}

		public static bool SupportsRuntimeOnly(this HDLightTypeAndShape type)
		{
			return type != HDLightTypeAndShape.DiscArea;
		}

		public static bool SupportsBakedOnly(this HDLightTypeAndShape type)
		{
			return type != HDLightTypeAndShape.TubeArea;
		}

		public static bool SupportsMixed(this HDLightTypeAndShape type)
		{
			if (type != HDLightTypeAndShape.TubeArea)
			{
				return type != HDLightTypeAndShape.DiscArea;
			}
			return false;
		}
	}
}
