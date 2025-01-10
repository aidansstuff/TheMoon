using System;

namespace AmazingAssets.TerrainToMesh
{
	[Serializable]
	public class EdgeFall
	{
		public float yValue;

		public bool saveInSubmesh;

		public EdgeFall(float P_0, bool P_1)
		{
			while (true)
			{
				int num = -645912555;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0xBA021299u) % 3)
					{
					case 2u:
						break;
					case 1u:
						goto IL_0028;
					default:
						saveInSubmesh = P_1;
						return;
					}
					break;
					IL_0028:
					yValue = P_0;
					num = (int)(num2 * 572956635) ^ -658241782;
				}
			}
		}
	}
}
