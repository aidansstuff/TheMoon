namespace AmazingAssets.TerrainToMesh
{
	public static class Constants
	{
		public static int maxSplatmapsCount = 4;

		public static int maxLayersCount;

		public static int vertexLimitIn16BitsIndexBuffer;

		public static string shaderAllTerrainTexture;

		public static string shaderGrass;

		public static string shaderSplatmap;

		public static string shaderSplatmapWithHoles;

		public static string shaderSplatmapFallback;

		public static string shaderSplatmapWithHolesFallback;

		static Constants()
		{
			while (true)
			{
				int num = -276682090;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0xDFA6E0A5u) % 6)
					{
					case 2u:
						break;
					default:
						return;
					case 1u:
						maxLayersCount = 16;
						num = ((int)num2 * -746349696) ^ 0x5EF954D6;
						continue;
					case 0u:
						shaderAllTerrainTexture = "Hidden/Amazing Assets/Terrain To Mesh/AllTerrainTextures";
						shaderGrass = "Amazing Assets/Terrain To Mesh/Grass";
						num = ((int)num2 * -1536871070) ^ -2020957888;
						continue;
					case 3u:
						shaderSplatmap = "Amazing Assets/Terrain To Mesh/Splatmap";
						shaderSplatmapWithHoles = "Amazing Assets/Terrain To Mesh/Splatmap (Holes)";
						shaderSplatmapFallback = "Hidden/Amazing Assets/Terrain To Mesh/Fallback/Splatmap";
						shaderSplatmapWithHolesFallback = "Hidden/Amazing Assets/Terrain To Mesh/Fallback/Splatmap (Holes)";
						num = ((int)num2 * -1868144476) ^ 0x62BFE56F;
						continue;
					case 5u:
						vertexLimitIn16BitsIndexBuffer = 65535;
						num = (int)(num2 * 692152726) ^ -2124813437;
						continue;
					case 4u:
						return;
					}
					break;
				}
			}
		}
	}
}
