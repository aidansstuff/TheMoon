using UnityEngine;

namespace AmazingAssets.TerrainToMesh
{
	public static class Extensions
	{
		public static TerrainToMeshDataExtractor TerrainToMesh(this TerrainData terrainData)
		{
			return new TerrainToMeshDataExtractor(terrainData);
		}
	}
}
