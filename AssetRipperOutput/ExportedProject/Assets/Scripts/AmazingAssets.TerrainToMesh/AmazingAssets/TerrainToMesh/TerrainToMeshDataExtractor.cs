using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace AmazingAssets.TerrainToMesh
{
	public class TerrainToMeshDataExtractor
	{
		private static bool _200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E;

		private static Dictionary<int, Vector3[]> _200F_206E_200C_206F_200B_200E_206F_202A_200E_202D_200E_202E_206A_206D_202A_200D_200B_200E_206D_206E_206E_200B_200E_202D_206C_200E_206A_200F_206F_206C_200C_200B_206D_200F_206A_202E_206C_202A_206A_206B_202E;

		private static Dictionary<int, int[]> _200F_200E_200C_202E_206B_206A_202B_202A_202A_206F_206A_202C_206D_200B_200B_202C_206B_200D_200D_200C_200C_202E_206D_206F_200C_206F_200E_202D_200F_200E_202C_202E_206E_202D_200E_200E_200E_200C_200F_200E_202E;

		private static Dictionary<int, Vector2[]> _202D_206C_202B_202C_202B_206B_206F_202C_202D_202E_200D_200C_206B_206D_206F_200E_206B_202D_202E_200C_202C_200F_202C_200F_202C_202A_202B_200F_202D_202B_202D_200D_206F_206E_200D_202E_200B_206D_202D_200E_202E;

		private static Dictionary<int, int[]> _206A_206D_202E_202C_206F_202B_200F_206F_200C_202B_202D_202A_200E_202B_206F_202B_202C_206C_206E_206F_206B_202E_206F_206A_202B_200F_202A_202B_206D_200E_200F_200F_202B_206B_202C_200B_206D_200E_202C_206A_202E;

		private static Dictionary<int, int[]> _202A_202E_202C_206A_206E_206F_202A_200D_206F_206E_202A_200F_200E_206B_200C_206A_200E_202D_200C_206D_202D_206E_202C_206A_200C_202E_200C_206F_200C_200F_202A_206E_206C_202E_202B_202D_200D_202E_206B_200E_202E;

		private static Dictionary<int, Vector2[]> _202B_200E_202B_200B_202D_200C_202B_200E_202E_200B_206D_202C_206D_206D_206A_200B_206E_206A_200E_206F_206A_206F_200F_200F_202A_202D_206F_200D_206D_206A_202C_200F_202A_200E_202E_200E_200F_202B_200B_202B_202E;

		private readonly TerrainData _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E;

		public TerrainToMeshDataExtractor(TerrainData P_0)
		{
			_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E = P_0;
		}

		public Mesh ExportMesh(int vertexCountHorizontal, int vertexCountVertical, Normal normalReconstruction, EdgeFall edgeFall = null)
		{
			if (!_206A_200E_200E_202C_200D_200B_202A_206E_200B_206B_200D_202B_202E_206F_202B_202E_202B_202C_202C_200B_200F_206B_200F_202B_202C_202D_200D_206F_200D_206A_202A_202D_202A_202D_202D_206D_206F_200B_202D_202D_202E(vertexCountHorizontal, vertexCountVertical, 1, 1, 0, 0, edgeFall))
			{
				while (true)
				{
					uint num;
					switch ((num = 0x17235212u ^ 0x53EC35E8u) % 3)
					{
					case 0u:
						continue;
					case 1u:
						return null;
					}
					break;
				}
			}
			return ExportMesh(vertexCountHorizontal, vertexCountVertical, 1, 1, 0, 0, perChunkUV: false, normalReconstruction, edgeFall);
		}

		public Mesh[] ExportMesh(int vertexCountHorizontal, int vertexCountVertical, int chunkCountHorizontal, int chunkCountVertical, bool perChunkUV, Normal normalReconstruction, EdgeFall edgeFall = null)
		{
			if (!_206A_200E_200E_202C_200D_200B_202A_206E_200B_206B_200D_202B_202E_206F_202B_202E_202B_202C_202C_200B_200F_206B_200F_202B_202C_202D_200D_206F_200D_206A_202A_202D_202A_202D_202D_206D_206F_200B_202D_202D_202E(vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, 0, 0, edgeFall))
			{
				goto IL_0014;
			}
			goto IL_0116;
			IL_0014:
			int num = 1815790608;
			goto IL_0019;
			IL_0019:
			int num5 = default(int);
			int num4 = default(int);
			Mesh[] array = default(Mesh[]);
			int num3 = default(int);
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0x6C2E8D4Au) % 12)
				{
				case 7u:
					break;
				case 11u:
					num5 = 0;
					num = 985910998;
					continue;
				case 0u:
					goto IL_0068;
				case 6u:
					return null;
				case 8u:
					num4++;
					num = (int)(num2 * 287610693) ^ -777327693;
					continue;
				case 5u:
					_202D_206D_202A_202B_206C_206A_200C_206E_202A_206B_206D_206D_200D_200E_200B_206C_206A_200E_202E_206A_206D_200D_206A_202D_206D_206E_200B_206F_206B_202C_206C_202D_206A_206F_202E_200B_206C_200F_206D_202E_202E();
					num = ((int)num2 * -1420298823) ^ -1691241867;
					continue;
				case 10u:
					array[num3++] = ExportMesh(vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, num4, num5, perChunkUV, normalReconstruction, edgeFall);
					num5++;
					num = 985910998;
					continue;
				case 4u:
					num3 = 0;
					num4 = 0;
					num = ((int)num2 * -1010963958) ^ 0x700AF3DF;
					continue;
				case 1u:
					goto IL_00fe;
				case 3u:
					goto IL_0116;
				case 9u:
					_200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E = true;
					num = ((int)num2 * -1781951719) ^ 0x1C958A0B;
					continue;
				default:
					return array;
				}
				break;
				IL_00fe:
				int num6;
				if (num4 < chunkCountHorizontal)
				{
					num = 748867605;
					num6 = num;
				}
				else
				{
					num = 1444090499;
					num6 = num;
				}
				continue;
				IL_0068:
				int num7;
				if (num5 >= chunkCountVertical)
				{
					num = 1619768646;
					num7 = num;
				}
				else
				{
					num = 75338580;
					num7 = num;
				}
			}
			goto IL_0014;
			IL_0116:
			array = new Mesh[chunkCountHorizontal * chunkCountVertical];
			num = 1669252731;
			goto IL_0019;
		}

		public Mesh ExportMesh(int vertexCountHorizontal, int vertexCountVertical, int chunkCountHorizontal, int chunkCountVertical, int positionX, int positionY, bool perChunkUV, Normal normalReconstruction, EdgeFall edgeFall = null)
		{
			if (!_206A_200E_200E_202C_200D_200B_202A_206E_200B_206B_200D_202B_202E_206F_202B_202E_202B_202C_202C_200B_200F_206B_200F_202B_202C_202D_200D_206F_200D_206A_202A_202D_202A_202D_202D_206D_206F_200B_202D_202D_202E(vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, positionX, positionY, edgeFall))
			{
				goto IL_0016;
			}
			goto IL_092e;
			IL_0016:
			int num = 976807829;
			goto IL_001b;
			IL_001b:
			int num7 = default(int);
			Vector4[] array11 = default(Vector4[]);
			Mesh mesh = default(Mesh);
			int num17 = default(int);
			Vector4[] array10 = default(Vector4[]);
			float num18 = default(float);
			float num13 = default(float);
			int[] array6 = default(int[]);
			Vector3[] first = default(Vector3[]);
			Vector4[] first2 = default(Vector4[]);
			Vector2[] array12 = default(Vector2[]);
			Vector2 vector = default(Vector2);
			Vector2 vector2 = default(Vector2);
			Vector2[] array2 = default(Vector2[]);
			Vector3[] array = default(Vector3[]);
			int num11 = default(int);
			Vector3[] array9 = default(Vector3[]);
			int num12 = default(int);
			Vector3[] array3 = default(Vector3[]);
			Vector3[] array13 = default(Vector3[]);
			Vector3[] second = default(Vector3[]);
			Vector3[] array8 = default(Vector3[]);
			int[] array5 = default(int[]);
			float num15 = default(float);
			float num10 = default(float);
			float num14 = default(float);
			int[] array7 = default(int[]);
			Vector3 vector6 = default(Vector3);
			int num16 = default(int);
			Vector3 vector5 = default(Vector3);
			Vector3[] array4 = default(Vector3[]);
			Vector3[] second2 = default(Vector3[]);
			Vector4[] second3 = default(Vector4[]);
			Vector3 vector3 = default(Vector3);
			int num8 = default(int);
			Vector2[] second4 = default(Vector2[]);
			float num9 = default(float);
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0x235ED3EFu) % 78)
				{
				case 7u:
					break;
				case 77u:
					num7 = 0;
					num = (int)((num2 * 1123149580) ^ 0x67F10144);
					continue;
				case 45u:
					array11 = _206D_202C_200C_202B_206C_206C_202E_206D_206E_206C_202D_206A_200B_202E_200F_202D_206A_206F_206E_200F_206D_206F_200B_206A_206A_200F_200E_202E_202B_200C_202A_200B_200C_206C_206E_206D_200C_202B_206A_200F_202E(mesh);
					num = ((int)num2 * -2127652757) ^ 0x6A2DACAB;
					continue;
				case 3u:
					num17 = 0;
					num = ((int)num2 * -1807100895) ^ -414888827;
					continue;
				case 34u:
					_206F_206B_206C_202D_206B_200F_202B_206B_206C_202E_200D_206A_200F_202D_200E_206C_206E_200F_206F_206E_206C_206A_200B_200F_206E_202E_202A_206C_206A_200B_202D_206C_200D_202A_200B_202C_202B_202A_200C_200E_202E(mesh, array10);
					num = ((int)num2 * -1959514466) ^ -1605966635;
					continue;
				case 70u:
					num18 = num13 / (float)(vertexCountHorizontal - 1);
					num = (int)(num2 * 534764206) ^ -2041373718;
					continue;
				case 0u:
					array6 = _202B_200C_202E_200E_206E_206E_206A_206B_202C_200D_206B_202D_202E_202C_200B_200B_206A_200F_206E_206E_200F_200F_200E_200F_202D_206B_206E_206B_200C_200D_200E_206B_202A_200D_206B_200D_202E_202C_206E_202A_202E(mesh);
					first = _206D_200E_206A_200B_202E_202B_200C_202D_200D_206B_200B_202D_200C_206A_202B_202A_200F_200E_200E_200E_200B_206F_200D_202E_206E_206A_200C_206E_206E_202C_202B_202C_202C_202E_200C_206D_206A_206E_206B_206F_202E(mesh);
					first2 = _206D_202C_200C_202B_206C_206C_202E_206D_206E_206C_202D_206A_200B_202E_200F_202D_206A_206F_206E_200F_206D_206F_200B_206A_206A_200F_200E_202E_202B_200C_202A_200B_200C_206C_206E_206D_200C_202B_206A_200F_202E(mesh);
					num = ((int)num2 * -1678840300) ^ -1661983938;
					continue;
				case 13u:
					array12 = _206D_206F_202E_206E_206C_202C_200E_202C_202A_202A_200D_206C_200D_206F_202E_200E_206F_206E_200E_206A_202C_206E_202B_202D_202C_200D_202B_202B_200C_200B_200D_206A_206F_202D_206E_206B_200F_202C_202E_206A_202E(mesh);
					num = (int)(num2 * 1505985257) ^ -1873594736;
					continue;
				case 69u:
					vector += vector2;
					array2[num7] = vector;
					num7++;
					num = ((int)num2 * -1042183683) ^ 0x60C3F4DB;
					continue;
				case 28u:
					array[num11] = array9[num12];
					array10[num11] = array11[num12];
					num = (int)((num2 * 1333369320) ^ 0x25473030);
					continue;
				case 39u:
					_206C_200D_206C_202A_202B_202D_200E_206D_206C_202E_200E_200B_206F_206D_206E_202B_206C_200D_202D_200B_200D_202C_200E_202C_206E_206C_206A_200D_200B_206C_202C_200F_200C_206C_202D_206B_200D_202C_202B_206D_202E(mesh, array3);
					num = (int)(num2 * 2142303951) ^ -1709732269;
					continue;
				case 58u:
				{
					_202A_202D_200D_200B_200C_202D_206D_202C_206F_202D_206F_202B_206D_202E_202A_200D_202A_200D_200D_202A_200E_202A_202B_200D_200E_206E_200B_200F_206B_202C_202E_200E_206C_202E_200B_202E_206B_202D_200B_200E_202E(mesh);
					_206C_200D_206C_202A_202B_202D_200E_206D_206C_202E_200E_200B_206F_206D_206E_202B_206C_200D_202D_200B_200D_202C_200E_202C_206E_206C_206A_200D_200B_206C_202C_200F_200C_206C_202D_206B_200D_202C_202B_206D_202E(mesh, array13.Concat(second).ToArray());
					int num27;
					int num28;
					if (normalReconstruction == Normal.None)
					{
						num27 = -1434015619;
						num28 = num27;
					}
					else
					{
						num27 = -435315754;
						num28 = num27;
					}
					num = num27 ^ ((int)num2 * -764117824);
					continue;
				}
				case 1u:
					_206D_200F_202E_200C_206E_206D_200B_200C_202B_206D_200D_202A_206A_206D_200B_206F_206D_206A_202D_206B_200B_202C_200E_200F_206A_206D_206E_202A_202A_206F_200F_206E_202A_206A_206C_202C_200F_202B_202B_200F_202E(mesh, (array8.Length > Constants.vertexLimitIn16BitsIndexBuffer) ? IndexFormat.UInt32 : IndexFormat.UInt16);
					_206E_200C_206F_200B_206C_202A_206B_202A_206A_206B_200E_200F_202D_202E_202C_202C_202E_202B_206D_202B_206D_206E_200C_200E_202C_200F_202D_206D_206F_200F_206A_200B_202D_206B_200C_200F_200D_200D_206C_206B_202E((UnityEngine.Object)mesh, _200C_206A_200F_200D_200C_206A_202C_200D_202C_202B_206D_200E_202B_200C_200C_206E_206F_200D_206D_200E_206F_206F_206D_202D_200D_206C_202B_202C_206F_202A_200F_206D_202D_202E_202D_200C_202A_206F_202E_206E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, chunkCountHorizontal, chunkCountVertical, positionX, positionY));
					_206C_200D_206C_202A_202B_202D_200E_206D_206C_202E_200E_200B_206F_206D_206E_202B_206C_200D_202D_200B_200D_202C_200E_202C_206E_206C_206A_200D_200B_206C_202C_200F_200C_206C_202D_206B_200D_202C_202B_206D_202E(mesh, array8);
					_200E_200F_200D_202C_202C_206B_202B_200C_202C_200D_200F_200C_206F_200C_200E_206E_200B_200F_206E_200E_202A_202A_200C_200D_206F_206B_206A_200D_200E_206A_206A_202C_202E_202A_200C_200F_206E_200C_206F_202D_202E(mesh, _200C_206F_206B_206C_202C_200C_202E_206F_200E_202B_206F_200C_206B_206B_202D_206B_206B_200B_206A_206A_200F_206A_206F_200C_202B_206E_206A_202C_206D_202B_206C_206A_206E_202E_202E_206B_206A_200E_200F_200D_202E(vertexCountHorizontal + num17, vertexCountVertical + num17));
					_206B_200E_206F_206A_202A_202D_200D_200D_200B_202E_206B_202A_200D_206D_202E_206A_202E_200E_206F_206E_202D_200E_202E_206F_202D_202C_202A_206D_206D_206A_200B_202D_202C_200C_206B_206E_206E_200D_206B_200D_202E(mesh, _206F_200B_206F_206F_200E_202B_200E_202C_206E_202D_202D_206A_206E_206C_200B_200E_200C_200C_202C_202C_200F_206D_202B_206E_206E_200D_202D_200B_200E_206B_200D_206D_200F_200F_206F_206F_200C_206A_202B_200E_202E(vertexCountHorizontal + num17, vertexCountVertical + num17));
					num = 886295007;
					continue;
				case 29u:
					_202D_206D_202A_202B_206C_206A_200C_206E_202A_206B_206D_206D_200D_200E_200B_206C_206A_200E_202E_206A_206D_200D_206A_202D_206D_206E_200B_206F_206B_202C_206C_202D_206A_206F_202E_200B_206C_200F_206D_202E_202E();
					num = ((int)num2 * -37807098) ^ -618753810;
					continue;
				case 15u:
					array10 = new Vector4[array5.Length];
					num = (int)(num2 * 119113238) ^ -2071694639;
					continue;
				case 8u:
					array11 = null;
					num = (int)((num2 * 1470998414) ^ 0x6DD4CF15);
					continue;
				case 72u:
					array10 = null;
					num = ((int)num2 * -308521790) ^ -1275995987;
					continue;
				case 50u:
					_206D_206C_206D_202C_202D_206E_200B_206A_200C_200B_206D_200D_206D_200E_206D_200D_206A_200B_206A_206F_200D_202A_206C_206B_200F_202A_202C_200B_200F_206F_202E_206D_202B_200F_200C_200B_206F_206B_206E_202A_202E(mesh);
					num = (int)((num2 * 173978932) ^ 0x5C61BFB4);
					continue;
				case 53u:
				{
					_206B_200E_206F_206A_202A_202D_200D_200D_200B_202E_206B_202A_200D_206D_202E_206A_202E_200E_206F_206E_202D_200E_202E_206F_202D_202C_202A_206D_206D_206A_200B_202D_202C_200C_206B_206E_206E_200D_206B_200D_202E(mesh, array2);
					int num5;
					int num6;
					if (normalReconstruction == Normal.None)
					{
						num5 = 1862900244;
						num6 = num5;
					}
					else
					{
						num5 = 1949293254;
						num6 = num5;
					}
					num = num5 ^ ((int)num2 * -104192869);
					continue;
				}
				case 52u:
					switch (normalReconstruction)
					{
					case Normal.CalculateFromTerrain:
						goto IL_05de;
					case Normal.CalculateFromMesh:
						goto IL_07aa;
					case Normal.None:
						goto IL_0b2b;
					}
					num = ((int)num2 * -924193366) ^ -53489565;
					continue;
				case 49u:
					_202C_202E_200F_206B_202B_202D_202C_206D_206D_200B_200E_200C_200B_202C_206A_206F_202C_200C_200F_200C_206F_200C_202B_206E_206D_202A_206D_206C_202E_202B_206D_206F_206A_200C_202B_206C_206C_200E_206D_200E_202E(mesh, false);
					num = 1388313626;
					continue;
				case 60u:
					num15 = 0f - num10;
					num = ((int)num2 * -1733356697) ^ -1410546307;
					continue;
				case 51u:
					array13 = _202A_206C_206E_200D_206F_200D_200E_202B_206A_202B_206A_206F_202B_206D_200F_206C_206E_200D_206B_206B_200C_202E_200E_202C_202B_200E_200F_200E_206A_206D_206A_200C_200E_206B_206E_206E_202D_206E_202A_202C_202E(mesh);
					num = ((int)num2 * -906674273) ^ -258660820;
					continue;
				case 30u:
					num14 = 0f - num18;
					num = ((int)num2 * -1762013563) ^ 0x7F9CC0EB;
					continue;
				case 71u:
					array9 = _206D_200E_206A_200B_202E_202B_200C_202D_200D_206B_200B_202D_200C_206A_202B_202A_200F_200E_200E_200E_200B_206F_200D_202E_206E_206A_200C_206E_206E_202C_202B_202C_202C_202E_200C_206D_206A_206E_206B_206F_202E(mesh);
					num = ((int)num2 * -1186299843) ^ -388441259;
					continue;
				case 73u:
					_200E_200F_200D_202C_202C_206B_202B_200C_202C_200D_200F_200C_206F_200C_200E_206E_200B_200F_206E_200E_202A_202A_200C_200D_206F_206B_206A_200D_200E_206A_206A_202C_202E_202A_200C_200F_206E_200C_206F_202D_202E(mesh, _200C_206F_206B_206C_202C_200C_202E_206F_200E_202B_206F_200C_206B_206B_202D_206B_206B_200B_206A_206A_200F_206A_206F_200C_202B_206E_206A_202C_206D_202B_206C_206A_206E_202E_202E_206B_206A_200E_200F_200D_202E(vertexCountHorizontal, vertexCountVertical));
					num = (int)((num2 * 1448345625) ^ 0x74EE0443);
					continue;
				case 67u:
					array5 = _200B_200B_200D_202A_200D_206F_202E_206A_206F_206A_202C_206C_202D_206F_202A_200C_200F_200F_202C_200D_202D_206A_202D_206F_206A_202B_202A_206C_202C_206B_200D_206B_202A_202A_200D_202C_200C_202D_206C_206E_202E(vertexCountHorizontal + num17, vertexCountVertical + num17);
					array9 = null;
					num = ((int)num2 * -1399174182) ^ -682994117;
					continue;
				case 23u:
					goto IL_04e4;
				case 75u:
					goto IL_0500;
				case 27u:
				{
					int num29;
					int num30;
					if (normalReconstruction != 0)
					{
						num29 = 1886116928;
						num30 = num29;
					}
					else
					{
						num29 = 595175181;
						num30 = num29;
					}
					num = num29 ^ (int)(num2 * 787994953);
					continue;
				}
				case 14u:
					perChunkUV = false;
					num = (int)(num2 * 1761310740) ^ -1826964031;
					continue;
				case 4u:
					_202D_200E_206E_202A_206D_206C_206B_206D_200E_200C_206F_206C_206E_200F_200E_206B_200F_206E_202E_200D_200D_202C_206A_200B_206D_206A_200B_206E_200C_200D_206A_200D_206D_200C_206C_206C_202E_206F_206C_206F_202E(mesh, 2);
					_200F_206E_206B_202C_202A_206A_206F_202A_206C_200F_202C_200C_202D_202B_200B_200F_202E_202E_202A_202E_202B_202E_206A_202A_206B_206D_200C_202E_202D_200C_206C_202E_202C_202A_202E_206D_202B_206E_206D_200F_202E(mesh, array6, 0);
					num = (int)((num2 * 1705192428) ^ 0x77ACEF0);
					continue;
				case 76u:
					return null;
				case 43u:
					num11++;
					num = 6167895;
					continue;
				case 55u:
					array3[num11] = array8[num12];
					num = (int)(num2 * 558844799) ^ -863276431;
					continue;
				case 11u:
					_200F_206E_206B_202C_202A_206A_206F_202A_206C_200F_202C_200C_202D_202B_200B_200F_202E_202E_202A_202E_202B_202E_206A_202A_206B_206D_200C_202E_202D_200C_206C_202E_202C_202A_202E_206D_202B_206E_206D_200F_202E(mesh, array7, 1);
					num = ((int)num2 * -392145231) ^ -487471381;
					continue;
				case 65u:
					goto IL_05de;
				case 36u:
					num17 = 2;
					num = (int)(num2 * 2040491980) ^ -587132971;
					continue;
				case 21u:
					num = (int)(num2 * 513914170) ^ -715507555;
					continue;
				case 9u:
					goto IL_061c;
				case 6u:
					vector6 = array8[num16] + vector5;
					vector6.y = _202C_200F_200E_200E_200D_200D_206D_200F_200E_200E_200E_202D_200B_206A_206A_200C_202D_206F_200E_202C_202C_200C_206A_202B_206A_202E_200F_202D_206E_206C_200F_206E_200D_202D_200B_202D_200D_200B_200D_206F_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, vector6.x / _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E).x, vector6.z / _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E).z);
					num = 1546632153;
					continue;
				case 40u:
					array3 = new Vector3[array5.Length];
					array = null;
					num = (int)((num2 * 324518659) ^ 0x64C53843);
					continue;
				case 17u:
					num = ((int)num2 * -1052662042) ^ -1398308760;
					continue;
				case 35u:
					array = new Vector3[array5.Length];
					num = ((int)num2 * -595210808) ^ -1030005132;
					continue;
				case 10u:
					vector2 = new Vector2((float)positionX / (float)chunkCountHorizontal, (float)positionY / (float)chunkCountVertical);
					num = ((int)num2 * -192822913) ^ 0x3D819352;
					continue;
				case 59u:
					_206E_200B_206C_206F_202D_202B_202E_206A_202A_202B_200B_206C_202E_202C_202B_202E_202B_202B_202C_206E_200E_200B_202B_206B_202A_206B_202C_206C_200D_206D_202E_200D_202C_200E_206C_206D_202E_200C_200D_206C_202E(mesh, array4);
					_206D_206C_206D_202C_202D_206E_200B_206A_200C_200B_206D_200D_206D_200E_206D_200D_206A_200B_206A_206F_200D_202A_206C_206B_200F_202A_202C_200B_200F_206F_202E_206D_202B_200F_200C_200B_206F_206B_206E_202A_202E(mesh);
					num = (int)(num2 * 847958152) ^ -1300437621;
					continue;
				case 22u:
					num = ((int)num2 * -1389195218) ^ -1179348837;
					continue;
				case 66u:
					num = ((int)num2 * -27715673) ^ -2049509105;
					continue;
				case 18u:
					mesh = _206E_206B_206F_206A_206B_206C_200D_200B_206B_202C_206B_200D_206E_200F_202B_200F_206A_206E_206B_206B_202D_206C_202D_200C_202C_202D_206B_200B_200E_202E_202B_202C_206D_200C_200C_202B_200E_202B_202D_200E_202E();
					num = ((int)num2 * -1126289745) ^ -320832374;
					continue;
				case 32u:
					goto IL_0772;
				case 64u:
					goto IL_078e;
				case 56u:
					goto IL_07aa;
				case 33u:
					_206E_200B_206C_206F_202D_202B_202E_206A_202A_202B_200B_206C_202E_202C_202B_202E_202B_202B_202C_206E_200E_200B_202B_206B_202A_206B_202C_206C_200D_206D_202E_200D_202C_200E_206C_206D_202E_200C_200D_206C_202E(mesh, first.Concat(second2).ToArray());
					_206F_206B_206C_202D_206B_200F_202B_206B_206C_202E_200D_206A_200F_202D_200E_206C_206E_200F_206F_206E_206C_206A_200B_200F_206E_202E_202A_206C_206A_200B_202D_206C_200D_202A_200B_202C_202B_202A_200C_200E_202E(mesh, first2.Concat(second3).ToArray());
					num = ((int)num2 * -1288681747) ^ 0x4F39AB8;
					continue;
				case 19u:
				{
					int num25;
					int num26;
					if (!edgeFall.saveInSubmesh)
					{
						num25 = -1600119809;
						num26 = num25;
					}
					else
					{
						num25 = -1048819056;
						num26 = num25;
					}
					num = num25 ^ (int)(num2 * 1894546625);
					continue;
				}
				case 31u:
					vector3 = array8[num8];
					num = 421139074;
					continue;
				case 12u:
					num8 = 0;
					num = (int)((num2 * 1313057899) ^ 0x3DA2D71D);
					continue;
				case 38u:
					_206B_200E_206F_206A_202A_202D_200D_200D_200B_202E_206B_202A_200D_206D_202E_206A_202E_200E_206F_206E_202D_200E_202E_206F_202D_202C_202A_206D_206D_206A_200B_202D_202C_200C_206B_206E_206E_200D_206B_200D_202E(mesh, array12.Concat(second4).ToArray());
					num = 838970980;
					continue;
				case 48u:
				{
					int num23;
					int num24;
					if (chunkCountHorizontal * chunkCountVertical == 1)
					{
						num23 = 2083293353;
						num24 = num23;
					}
					else
					{
						num23 = 624756375;
						num24 = num23;
					}
					num = num23 ^ ((int)num2 * -1919631901);
					continue;
				}
				case 54u:
					_202A_206E_202E_202B_206B_200E_206F_206B_202A_206B_206F_206F_200E_206C_200C_200E_200D_206A_206C_206F_206D_206F_202E_206B_202B_200E_200C_200D_200D_200D_200F_206A_206F_206F_200E_206D_200C_200F_206E_202D_202E(vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, positionX, positionY, array13, array12, edgeFall.saveInSubmesh, perChunkUV, edgeFall.yValue, out second, out array7, out second2, out second3, out second4);
					num = (int)((num2 * 1780761067) ^ 0x5C8C4B6D);
					continue;
				case 26u:
					array8[num16] = vector6;
					num16++;
					num = (int)(num2 * 1950143267) ^ -316185478;
					continue;
				case 2u:
					num11 = 0;
					num = 6167895;
					continue;
				case 25u:
				{
					num14 = 0f;
					num15 = 0f;
					int num21;
					int num22;
					if (chunkCountHorizontal * chunkCountVertical > 1)
					{
						num21 = 1849890725;
						num22 = num21;
					}
					else
					{
						num21 = 812798501;
						num22 = num21;
					}
					num = num21 ^ (int)(num2 * 567262654);
					continue;
				}
				case 37u:
					goto IL_092e;
				case 20u:
				{
					int num19;
					int num20;
					if (normalReconstruction != 0)
					{
						num19 = -35628892;
						num20 = num19;
					}
					else
					{
						num19 = -61051833;
						num20 = num19;
					}
					num = num19 ^ (int)(num2 * 209561350);
					continue;
				}
				case 44u:
					vector = array2[num7];
					num = 397439378;
					continue;
				case 42u:
					array8 = (Vector3[])_202B_202B_206D_202D_206B_206F_200F_200D_200D_202E_206B_200B_200F_206E_200F_200D_202C_206D_202C_202D_206D_200B_202E_200B_200D_200D_202D_202C_200C_206E_206D_206A_206C_200F_202D_206A_200C_200E_206A_206A_202E((Array)_206A_206A_202B_206A_200E_200F_200F_202D_202B_200B_200C_206A_200F_200F_200E_206F_200B_206F_202B_202E_206A_200C_202B_200E_200F_200F_206C_202C_202C_200C_202D_206B_200E_202E_200F_206F_206A_206C_200E_200C_202E(vertexCountHorizontal + num17, vertexCountVertical + num17, num18, num10));
					num = 968388659;
					continue;
				case 63u:
					_200E_200F_200D_202C_202C_206B_202B_200C_202C_200D_200F_200C_206F_200C_200E_206E_200B_200F_206E_200E_202A_202A_200C_200D_206F_206B_206A_200D_200E_206A_206A_202C_202E_202A_200C_200F_206E_200C_206F_202D_202E(mesh, array6.Concat(array7).ToArray());
					num = 236744148;
					continue;
				case 24u:
					vector5 = new Vector3(num14 + num13 * (float)positionX, 0f, num15 + num9 * (float)positionY);
					num16 = 0;
					num = (int)(num2 * 1984428305) ^ -543197052;
					continue;
				case 62u:
					num13 = _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E).x / (float)chunkCountHorizontal;
					num9 = _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E).z / (float)chunkCountVertical;
					num = 1678481421;
					continue;
				case 5u:
					num12 = array5[num11];
					num = 1224342410;
					continue;
				case 57u:
					num10 = num9 / (float)(vertexCountVertical - 1);
					num = (int)((num2 * 177047106) ^ 0x326AAAD0);
					continue;
				case 41u:
				{
					Vector3 vector4 = _202A_202C_206C_200B_202B_206E_200C_206E_200B_202E_206A_206E_206D_206E_200B_200D_206D_206C_202A_200D_206E_200D_202B_206D_206B_200B_202C_206D_200B_200C_206E_202C_202E_206E_200E_200F_200C_206E_206D_206B_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, vector3.x / _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E).x, vector3.z / _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E).z);
					array4[num8] = vector4;
					num8++;
					num = ((int)num2 * -1647280310) ^ -180769919;
					continue;
				}
				case 61u:
					vector.x /= chunkCountHorizontal;
					vector.y /= chunkCountVertical;
					num = (int)(num2 * 1742993316) ^ -76669802;
					continue;
				case 46u:
				{
					array2 = (Vector2[])_202B_202B_206D_202D_206B_206F_200F_200D_200D_202E_206B_200B_200F_206E_200F_200D_202C_206D_202C_202D_206D_200B_202E_200B_200D_200D_202D_202C_200C_206E_206D_206A_206C_200F_202D_206A_200C_200E_206A_206A_202E((Array)_206F_200B_206F_206F_200E_202B_200E_202C_206E_202D_202D_206A_206E_206C_200B_200E_200C_200C_202C_202C_200F_206D_202B_206E_206E_200D_202D_200B_200E_206B_200D_206D_200F_200F_206F_206F_200C_206A_202B_200E_202E(vertexCountHorizontal, vertexCountVertical));
					int num3;
					int num4;
					if (perChunkUV)
					{
						num3 = -330412448;
						num4 = num3;
					}
					else
					{
						num3 = -1844733117;
						num4 = num3;
					}
					num = num3 ^ (int)(num2 * 1212182194);
					continue;
				}
				case 16u:
					goto IL_0b13;
				case 68u:
					goto IL_0b2b;
				case 74u:
					_206E_200B_206C_206F_202D_202B_202E_206A_202A_202B_200B_206C_202E_202C_202B_202E_202B_202B_202C_206E_200E_200B_202B_206B_202A_206B_202C_206C_200D_206D_202E_200D_202C_200E_206C_206D_202E_200C_200D_206C_202E(mesh, array);
					num = (int)((num2 * 403442500) ^ 0x4E9B62A7);
					continue;
				default:
					{
						return mesh;
					}
					IL_07aa:
					_202C_202E_202D_202E_206B_202B_202B_200F_202C_200E_206D_202E_200E_206E_206C_206F_202A_206F_202E_202E_200F_200F_200D_206E_202E_202E_200E_202B_200C_206A_200C_200D_206C_206F_206E_200C_206D_206A_200F_200C_202E(mesh);
					num = 618775273;
					continue;
					IL_05de:
					array4 = new Vector3[array8.Length];
					num = 46719709;
					continue;
				}
				break;
				IL_0b13:
				int num31;
				if (edgeFall != null)
				{
					num = 305404258;
					num31 = num;
				}
				else
				{
					num = 236744148;
					num31 = num;
				}
				continue;
				IL_061c:
				int num32;
				if (num7 >= array2.Length)
				{
					num = 1336160968;
					num32 = num;
				}
				else
				{
					num = 526982171;
					num32 = num;
				}
				continue;
				IL_04e4:
				int num33;
				if (num16 >= array8.Length)
				{
					num = 1900146165;
					num33 = num;
				}
				else
				{
					num = 322485703;
					num33 = num;
				}
				continue;
				IL_078e:
				int num34;
				if (num8 < array8.Length)
				{
					num = 1053687014;
					num34 = num;
				}
				else
				{
					num = 1516586800;
					num34 = num;
				}
				continue;
				IL_0b2b:
				int num35;
				if (chunkCountHorizontal * chunkCountVertical <= 1)
				{
					num = 1995092069;
					num35 = num;
				}
				else
				{
					num = 1846917718;
					num35 = num;
				}
				continue;
				IL_0500:
				int num36;
				if (_200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E)
				{
					num = 1685097980;
					num36 = num;
				}
				else
				{
					num = 2136994412;
					num36 = num;
				}
				continue;
				IL_0772:
				int num37;
				if (num11 < array5.Length)
				{
					num = 1093440388;
					num37 = num;
				}
				else
				{
					num = 914155931;
					num37 = num;
				}
			}
			goto IL_0016;
			IL_092e:
			int num38;
			if (!perChunkUV)
			{
				num = 1294132097;
				num38 = num;
			}
			else
			{
				num = 2094129565;
				num38 = num;
			}
			goto IL_001b;
		}

		private static void _202A_206E_202E_202B_206B_200E_206F_206B_202A_206B_206F_206F_200E_206C_200C_200E_200D_206A_206C_206F_206D_206F_202E_206B_202B_200E_200C_200D_200D_200D_200F_206A_206F_206F_200E_206D_200C_200F_206E_202D_202E(int P_0, int P_1, int P_2, int P_3, int P_4, int P_5, Vector3[] P_6, Vector2[] P_7, bool P_8, bool P_9, float P_10, out Vector3[] P_11, out int[] P_12, out Vector3[] P_13, out Vector4[] P_14, out Vector2[] P_15)
		{
			_202D_206D_200E_202C_200F_206C_200E_202B_202B_206A_206B_202C_200D_200B_206E_206A_206C_202E_206A_202B_202D_206F_202E_200B_206B_202B_206B_202A_200D_202B_206A_206B_202C_200E_200D_206E_200F_206A_202A_200E_202E(P_0, P_1, out var array, out var array2, out var array3, out var array4);
			Vector3[] array5 = _202C_200E_206C_202E_202B_200B_200E_206A_200E_206E_206F_202E_206B_206D_200D_206C_206F_206B_206F_200B_206D_206F_206C_202A_202C_202A_206E_202B_206C_200E_202A_206C_206F_206F_206F_206A_202E_200B_200E_202E(P_6, array, P_10);
			Vector2[] second3 = default(Vector2[]);
			int num10 = default(int);
			Vector2[] first = default(Vector2[]);
			Vector2[] second = default(Vector2[]);
			Vector3[] array6 = default(Vector3[]);
			int num3 = default(int);
			Vector3 vector5 = default(Vector3);
			int num9 = default(int);
			Vector4[] array7 = default(Vector4[]);
			int num5 = default(int);
			Vector4 vector = default(Vector4);
			int[] first2 = default(int[]);
			int[] second5 = default(int[]);
			int[] second6 = default(int[]);
			int[] second7 = default(int[]);
			int num22 = default(int);
			int num6 = default(int);
			Vector4[] array8 = default(Vector4[]);
			Vector4[] array13 = default(Vector4[]);
			Vector4[] array14 = default(Vector4[]);
			int num7 = default(int);
			Vector3 vector6 = default(Vector3);
			Vector3 vector3 = default(Vector3);
			Vector3[] array10 = default(Vector3[]);
			int num12 = default(int);
			Vector4 vector8 = default(Vector4);
			Vector3[] array15 = default(Vector3[]);
			Vector3[] array9 = default(Vector3[]);
			Vector3[] array11 = default(Vector3[]);
			Vector3[] array12 = default(Vector3[]);
			Vector3 vector7 = default(Vector3);
			Vector2[] second2 = default(Vector2[]);
			int num16 = default(int);
			Vector4 vector4 = default(Vector4);
			int num8 = default(int);
			Vector4 vector2 = default(Vector4);
			while (true)
			{
				int num = 1168450755;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0xFB9AC07u) % 63)
					{
					case 33u:
						break;
					case 36u:
						second3 = _206B_206E_206E_206C_206F_202B_200B_206F_206B_206B_206D_202A_206E_202B_202B_206D_200C_200D_202E_206B_200D_202A_206E_206E_206F_200B_206F_202E_206A_206C_206F_200D_206F_206A_202A_206A_206D_202E_202E_200C_202E(array4.Length, P_3, P_5);
						num = ((int)num2 * -106809483) ^ -1955045195;
						continue;
					case 40u:
						num10 = 0;
						num = (int)((num2 * 1109846156) ^ 0x7DF072A3);
						continue;
					case 27u:
						first = _202A_200B_202A_200D_202E_206C_202C_206D_202A_202C_206F_200D_206D_206B_202E_200F_206A_200D_206C_202D_206C_200F_200C_206B_200F_206E_206F_206C_206D_200D_206A_202D_202E_202C_206F_206B_206B_202B_200C_200C_202E(P_7, array, P_8, P_9);
						second = _202A_200B_202A_200D_202E_206C_202C_206D_202A_202C_206F_200D_206D_206B_202E_200F_206A_200D_206C_202D_206C_200F_200C_206B_200F_206E_206F_206C_206D_200D_206A_202D_202E_202C_206F_206B_206B_202B_200C_200C_202E(P_7, array2, P_8, P_9);
						num = 1209898039;
						continue;
					case 46u:
						array6[num3] = vector5;
						num = 1477209345;
						continue;
					case 61u:
						num9 = 0;
						num = ((int)num2 * -574938567) ^ -1658185569;
						continue;
					case 50u:
						array7[num5] = vector;
						num = 726604266;
						continue;
					case 62u:
						P_12 = first2.Concat(second5).Concat(second6).Concat(second7)
							.ToArray();
						num22 = P_6.Length;
						num = ((int)num2 * -242479124) ^ 0xB74FFF4;
						continue;
					case 7u:
						array6 = new Vector3[array4.Length * 2];
						num6 = 0;
						num = (int)(num2 * 495087715) ^ -1236799540;
						continue;
					case 41u:
					{
						P_14 = array7.Concat(array8).Concat(array13).Concat(array14)
							.ToArray();
						int num20;
						int num21;
						if (P_9)
						{
							num20 = -1147422939;
							num21 = num20;
						}
						else
						{
							num20 = -786674186;
							num21 = num20;
						}
						num = num20 ^ (int)(num2 * 1611224230);
						continue;
					}
					case 47u:
						num7++;
						num = ((int)num2 * -380059709) ^ -793551962;
						continue;
					case 16u:
						num7 = 0;
						num = (int)(num2 * 244994361) ^ -109809082;
						continue;
					case 9u:
						vector6 = new Vector3(0f, 0f, 1f);
						vector3 = new Vector3(1f, 0f, 0f);
						num = (int)((num2 * 393787402) ^ 0x3693EF24);
						continue;
					case 54u:
					{
						int num15;
						if (num7 < array8.Length)
						{
							num = 981669253;
							num15 = num;
						}
						else
						{
							num = 660930999;
							num15 = num;
						}
						continue;
					}
					case 26u:
						num = (int)(num2 * 1326580479) ^ -396358278;
						continue;
					case 0u:
						array10 = new Vector3[array2.Length * 2];
						num = (int)(num2 * 1252304802) ^ -2055362960;
						continue;
					case 17u:
						num12 = 0;
						num = (int)(num2 * 1827566599) ^ -536224919;
						continue;
					case 53u:
						num = ((int)num2 * -418460725) ^ 0x6B893DA8;
						continue;
					case 15u:
					{
						int num24;
						int num25;
						if (P_8)
						{
							num24 = -1948752616;
							num25 = num24;
						}
						else
						{
							num24 = -404182459;
							num25 = num24;
						}
						num = num24 ^ ((int)num2 * -479126340);
						continue;
					}
					case 1u:
						vector8 = new Vector4(1f, 0f, 0f, -1f);
						num = (int)((num2 * 1145714175) ^ 0x79DCB099);
						continue;
					case 29u:
						P_13 = array15.Concat(array10).Concat(array9).Concat(array6)
							.ToArray();
						array7 = new Vector4[array.Length * 2];
						array8 = new Vector4[array2.Length * 2];
						num = (int)(num2 * 117178094) ^ -976524543;
						continue;
					case 28u:
						num = (int)((num2 * 1342799414) ^ 0x5AB14F5D);
						continue;
					case 14u:
						array14[num12] = vector8;
						num12++;
						num = 204114202;
						continue;
					case 58u:
						num = (int)(num2 * 1372758075) ^ -1544360132;
						continue;
					case 2u:
						array11 = _202C_200E_206C_202E_202B_200B_200E_206A_200E_206E_206F_202E_206B_206D_200D_206C_206F_206B_206F_200B_206D_206F_206C_202A_202C_202A_206E_202B_206C_200E_202A_206C_206F_206F_206F_206A_202E_200B_200E_202E(P_6, array2, P_10);
						array12 = _202C_200E_206C_202E_202B_200B_200E_206A_200E_206E_206F_202E_206B_206D_200D_206C_206F_206B_206F_200B_206D_206F_206C_202A_202C_202A_206E_202B_206C_200E_202A_206C_206F_206F_206F_206A_202E_200B_200E_202E(P_6, array3, P_10);
						num = (int)((num2 * 1500381561) ^ 0x4CDD426A);
						continue;
					case 60u:
						num = (int)(num2 * 476970243) ^ -1561334685;
						continue;
					case 31u:
						vector7 = new Vector3(0f, 0f, -1f);
						vector5 = new Vector3(-1f, 0f, 0f);
						array15 = new Vector3[array.Length * 2];
						num = ((int)num2 * -1813839373) ^ -608028614;
						continue;
					case 57u:
						first = _206B_206E_206E_206C_206F_202B_200B_206F_206B_206B_206D_202A_206E_202B_202B_206D_200C_200D_202E_206B_200D_202A_206E_206E_206F_200B_206F_202E_206A_206C_206F_200D_206F_206A_202A_206A_206D_202E_202E_200C_202E(array.Length, P_2, P_4);
						second = _206B_206E_206E_206C_206F_202B_200B_206F_206B_206B_206D_202A_206E_202B_202B_206D_200C_200D_202E_206B_200D_202A_206E_206E_206F_200B_206F_202E_206A_206C_206F_200D_206F_206A_202A_206A_206D_202E_202E_200C_202E(array2.Length, P_3, P_3 - P_5 - 1);
						second2 = _206B_206E_206E_206C_206F_202B_200B_206F_206B_206B_206D_202A_206E_202B_202B_206D_200C_200D_202E_206B_200D_202A_206E_206E_206F_200B_206F_202E_206A_206C_206F_200D_206F_206A_202A_206A_206D_202E_202E_200C_202E(array3.Length, P_2, P_2 - P_4 - 1);
						num = ((int)num2 * -1100842138) ^ -2055438128;
						continue;
					case 39u:
					{
						int num23;
						if (num6 >= array15.Length)
						{
							num = 14942147;
							num23 = num;
						}
						else
						{
							num = 49605134;
							num23 = num;
						}
						continue;
					}
					case 32u:
						P_12[num9] += num22;
						num = 545959966;
						continue;
					case 45u:
						array9[num10] = vector7;
						num = 152896072;
						continue;
					case 34u:
						num = ((int)num2 * -1879042794) ^ 0x62F57068;
						continue;
					case 25u:
						second5 = _200F_206C_206E_200D_202B_200E_202A_200F_206C_200B_202D_202A_202A_200C_206B_206E_206D_200C_200E_206C_202C_206C_206D_200C_200C_206A_206E_206F_206F_206F_202E_206B_200F_206F_206C_206F_202E_206B_202D_206B_202E(array2.Length, array5.Length);
						second6 = _200F_206C_206E_200D_202B_200E_202A_200F_206C_200B_202D_202A_202A_200C_206B_206E_206D_200C_200E_206C_202C_206C_206D_200C_200C_206A_206E_206F_206F_206F_202E_206B_200F_206F_206C_206F_202E_206B_202D_206B_202E(array3.Length, array5.Length + array11.Length);
						second7 = _200F_206C_206E_200D_202B_200E_202A_200F_206C_200B_202D_202A_202A_200C_206B_206E_206D_200C_200E_206C_202C_206C_206D_200C_200C_206A_206E_206F_206F_206F_202E_206B_200F_206F_206C_206F_202E_206B_202D_206B_202E(array4.Length, array5.Length + array11.Length + array12.Length);
						num = ((int)num2 * -1317554533) ^ -961301074;
						continue;
					case 3u:
						num10++;
						num = ((int)num2 * -2073421545) ^ -1299232968;
						continue;
					case 42u:
					{
						int num19;
						if (num9 >= P_12.Length)
						{
							num = 2036148411;
							num19 = num;
						}
						else
						{
							num = 1178486152;
							num19 = num;
						}
						continue;
					}
					case 22u:
						array15[num6] = vector6;
						num6++;
						num = 388077586;
						continue;
					case 18u:
						num16 = 0;
						num = ((int)num2 * -589907746) ^ 0xE230FB4;
						continue;
					case 20u:
						num = (int)(num2 * 1195795656) ^ -274726495;
						continue;
					case 44u:
						num3++;
						num = ((int)num2 * -541870878) ^ 0x2282B096;
						continue;
					case 12u:
					{
						int num18;
						if (num12 < array14.Length)
						{
							num = 882038125;
							num18 = num;
						}
						else
						{
							num = 1663785301;
							num18 = num;
						}
						continue;
					}
					case 43u:
						vector4 = new Vector4(1f, 0f, 0f, -1f);
						num = ((int)num2 * -2022664817) ^ 0xCAB931F;
						continue;
					case 10u:
						num = ((int)num2 * -448324253) ^ 0x5446D755;
						continue;
					case 51u:
						num3 = 0;
						num = (int)(num2 * 532450299) ^ -1103209951;
						continue;
					case 13u:
					{
						int num17;
						if (num16 < array13.Length)
						{
							num = 393318612;
							num17 = num;
						}
						else
						{
							num = 1692101916;
							num17 = num;
						}
						continue;
					}
					case 21u:
						array13[num16] = vector4;
						num16++;
						num = 1144767161;
						continue;
					case 49u:
					{
						int num14;
						if (num5 >= array7.Length)
						{
							num = 1065967893;
							num14 = num;
						}
						else
						{
							num = 201313524;
							num14 = num;
						}
						continue;
					}
					case 48u:
					{
						int num13;
						if (num8 < array10.Length)
						{
							num = 2026681643;
							num13 = num;
						}
						else
						{
							num = 63346264;
							num13 = num;
						}
						continue;
					}
					case 6u:
						second2 = _202A_200B_202A_200D_202E_206C_202C_206D_202A_202C_206F_200D_206D_206B_202E_200F_206A_200D_206C_202D_206C_200F_200C_206B_200F_206E_206F_206C_206D_200D_206A_202D_202E_202C_206F_206B_206B_202B_200C_200C_202E(P_7, array3, P_8, P_9);
						num = ((int)num2 * -816053623) ^ -2041083035;
						continue;
					case 11u:
						array13 = new Vector4[array3.Length * 2];
						array14 = new Vector4[array4.Length * 2];
						vector = new Vector4(1f, 0f, 0f, -1f);
						num = ((int)num2 * -1740810584) ^ -169876256;
						continue;
					case 35u:
					{
						Vector3[] second4 = _202C_200E_206C_202E_202B_200B_200E_206A_200E_206E_206F_202E_206B_206D_200D_206C_206F_206B_206F_200B_206D_206F_206C_202A_202C_202A_206E_202B_206C_200E_202A_206C_206F_206F_206F_206A_202E_200B_200E_202E(P_6, array4, P_10);
						P_11 = array5.Concat(array11).Concat(array12).Concat(second4)
							.ToArray();
						num = ((int)num2 * -1929187161) ^ 0x784658FE;
						continue;
					}
					case 8u:
						num5 = 0;
						num = ((int)num2 * -1201037797) ^ -467050307;
						continue;
					case 59u:
						second3 = _202A_200B_202A_200D_202E_206C_202C_206D_202A_202C_206F_200D_206D_206B_202E_200F_206A_200D_206C_202D_206C_200F_200C_206B_200F_206E_206F_206C_206D_200D_206A_202D_202E_202C_206F_206B_206B_202B_200C_200C_202E(P_7, array4, P_8, P_9);
						num = ((int)num2 * -1537826390) ^ 0x69B6715B;
						continue;
					case 4u:
					{
						int num11;
						if (num10 >= array9.Length)
						{
							num = 1853259280;
							num11 = num;
						}
						else
						{
							num = 19613025;
							num11 = num;
						}
						continue;
					}
					case 55u:
						num9++;
						num = ((int)num2 * -1319085167) ^ -995292134;
						continue;
					case 30u:
						num8 = 0;
						num = ((int)num2 * -226740583) ^ 0x52C978DE;
						continue;
					case 56u:
						array10[num8] = vector3;
						num8++;
						num = 1197347066;
						continue;
					case 38u:
						first2 = _200F_206C_206E_200D_202B_200E_202A_200F_206C_200B_202D_202A_202A_200C_206B_206E_206D_200C_200E_206C_202C_206C_206D_200C_200C_206A_206E_206F_206F_206F_202E_206B_200F_206F_206C_206F_202E_206B_202D_206B_202E(array.Length, 0);
						num = ((int)num2 * -1975885478) ^ 0x2A0130A9;
						continue;
					case 52u:
						vector2 = new Vector4(1f, 0f, 0f, -1f);
						num = (int)((num2 * 1345081895) ^ 0x79B781AE);
						continue;
					case 23u:
						array9 = new Vector3[array3.Length * 2];
						num = (int)(num2 * 752940205) ^ -271263514;
						continue;
					case 24u:
						array8[num7] = vector2;
						num = 1000597950;
						continue;
					case 19u:
						num5++;
						num = (int)((num2 * 1926561150) ^ 0x4EF3EE21);
						continue;
					case 37u:
					{
						int num4;
						if (num3 >= array6.Length)
						{
							num = 753495070;
							num4 = num;
						}
						else
						{
							num = 881424512;
							num4 = num;
						}
						continue;
					}
					default:
						P_15 = first.Concat(second).Concat(second2).Concat(second3)
							.ToArray();
						return;
					}
					break;
				}
			}
		}

		private static Vector3[] _206A_206A_202B_206A_200E_200F_200F_202D_202B_200B_200C_206A_200F_200F_200E_206F_200B_206F_202B_202E_206A_200C_202B_200E_200F_200F_206C_202C_202C_200C_202D_206B_200E_202E_200F_206F_206A_206C_200E_200C_202E(int P_0, int P_1, float P_2, float P_3)
		{
			if (P_0 >= 2)
			{
				goto IL_0007;
			}
			int num = 2;
			goto IL_0252;
			IL_0252:
			P_0 = num;
			int num2 = 1745886657;
			goto IL_000c;
			IL_0007:
			num2 = 33676174;
			goto IL_000c;
			IL_000c:
			int num6 = default(int);
			int num17 = default(int);
			Vector3[] array = default(Vector3[]);
			int num12 = default(int);
			int key = default(int);
			while (true)
			{
				uint num3;
				int num9;
				switch ((num3 = (uint)num2 ^ 0x4FFC75CBu) % 27)
				{
				case 6u:
					break;
				case 23u:
					num9 = P_1;
					goto IL_0092;
				case 5u:
					num6++;
					num2 = (int)(num3 * 642948469) ^ -899329423;
					continue;
				case 9u:
					_200F_206E_200C_206F_200B_200E_206F_202A_200E_202D_200E_202E_206A_206D_202A_200D_200B_200E_206D_206E_206E_200B_200E_202D_206C_200E_206A_200F_206F_206C_200C_200B_206D_200F_206A_202E_206C_202A_206A_206B_202E = new Dictionary<int, Vector3[]>();
					num2 = (int)((num3 * 1332448966) ^ 0x6FF321A5);
					continue;
				case 1u:
					goto IL_00d2;
				case 22u:
				{
					int num15;
					int num16;
					if (_200F_206E_200C_206F_200B_200E_206F_202A_200E_202D_200E_202E_206A_206D_202A_200D_200B_200E_206D_206E_206E_200B_200E_202D_206C_200E_206A_200F_206F_206C_200C_200B_206D_200F_206A_202E_206C_202A_206A_206B_202E == null)
					{
						num15 = -93270056;
						num16 = num15;
					}
					else
					{
						num15 = -1482054282;
						num16 = num15;
					}
					num2 = num15 ^ ((int)num3 * -1040161813);
					continue;
				}
				case 20u:
					goto IL_010e;
				case 21u:
					num17 = 0;
					num2 = 608087517;
					continue;
				case 19u:
					goto IL_0134;
				case 8u:
					array = new Vector3[P_0 * P_1];
					num12 = 0;
					num2 = 633196011;
					continue;
				case 4u:
					key = _200D_200D_202D_200B_202D_202E_206D_200E_202C_200B_200C_206D_206C_202D_206B_200E_202C_206A_202E_202E_206B_202D_200C_200F_200F_206B_202E_202E_202B_206D_200E_206E_200D_200D_200C_200E_206F_206B_202E_206D_202E(P_0, P_1);
					num2 = 1264927204;
					continue;
				case 7u:
					array = _200F_206E_200C_206F_200B_200E_206F_202A_200E_202D_200E_202E_206A_206D_202A_200D_200B_200E_206D_206E_206E_200B_200E_202D_206C_200E_206A_200F_206F_206C_200C_200B_206D_200F_206A_202E_206C_202A_206A_206B_202E[key];
					num2 = (int)((num3 * 1376643825) ^ 0x421BA91D);
					continue;
				case 15u:
					array = null;
					num2 = (int)((num3 * 1583471121) ^ 0x20303F2C);
					continue;
				case 25u:
				{
					int num7;
					int num8;
					if (!_200F_206E_200C_206F_200B_200E_206F_202A_200E_202D_200E_202E_206A_206D_202A_200D_200B_200E_206D_206E_206E_200B_200E_202D_206C_200E_206A_200F_206F_206C_200C_200B_206D_200F_206A_202E_206C_202A_206A_206B_202E.ContainsKey(key))
					{
						num7 = 1255462479;
						num8 = num7;
					}
					else
					{
						num7 = 1898594393;
						num8 = num7;
					}
					num2 = num7 ^ (int)(num3 * 1103357666);
					continue;
				}
				case 10u:
					num2 = (int)((num3 * 1634613168) ^ 0x38A47007);
					continue;
				case 17u:
					return array;
				case 14u:
					key = 0;
					num2 = ((int)num3 * -2135849163) ^ -1883948564;
					continue;
				case 18u:
					array[num12++] = new Vector3((float)num6 * P_2, 0f, (float)num17 * P_3);
					num2 = 224562986;
					continue;
				case 3u:
					num17++;
					num2 = (int)(num3 * 467037950) ^ -123652893;
					continue;
				case 12u:
					goto IL_024e;
				case 13u:
				{
					int num13;
					int num14;
					if (array.Length == P_0 * P_1)
					{
						num13 = -1277846879;
						num14 = num13;
					}
					else
					{
						num13 = -1826735729;
						num14 = num13;
					}
					num2 = num13 ^ ((int)num3 * -1906321309);
					continue;
				}
				case 0u:
				{
					int num10;
					int num11;
					if (!_200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E)
					{
						num10 = -1337946857;
						num11 = num10;
					}
					else
					{
						num10 = -53804544;
						num11 = num10;
					}
					num2 = num10 ^ ((int)num3 * -486597360);
					continue;
				}
				case 11u:
					_200F_206E_200C_206F_200B_200E_206F_202A_200E_202D_200E_202E_206A_206D_202A_200D_200B_200E_206D_206E_206E_200B_200E_202D_206C_200E_206A_200F_206F_206C_200C_200B_206D_200F_206A_202E_206C_202A_206A_206B_202E[key] = array;
					num2 = ((int)num3 * -1912880618) ^ 0x5A6AD8E5;
					continue;
				case 24u:
					num6 = 0;
					num2 = ((int)num3 * -1724990946) ^ 0x607EBF25;
					continue;
				case 2u:
					if (P_1 < 2)
					{
						num9 = 2;
						goto IL_0092;
					}
					num2 = ((int)num3 * -604625141) ^ 0x62CA1D85;
					continue;
				case 16u:
				{
					int num4;
					int num5;
					if (_200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E)
					{
						num4 = -2098908302;
						num5 = num4;
					}
					else
					{
						num4 = -1499899983;
						num5 = num4;
					}
					num2 = num4 ^ (int)(num3 * 1779595664);
					continue;
				}
				default:
					{
						return array;
					}
					IL_0092:
					P_1 = num9;
					num2 = 157077151;
					continue;
				}
				break;
				IL_0134:
				int num18;
				if (array != null)
				{
					num2 = 2124499589;
					num18 = num2;
				}
				else
				{
					num2 = 360018597;
					num18 = num2;
				}
				continue;
				IL_010e:
				int num19;
				if (num17 < P_1)
				{
					num2 = 571086128;
					num19 = num2;
				}
				else
				{
					num2 = 1467420277;
					num19 = num2;
				}
				continue;
				IL_00d2:
				int num20;
				if (num6 < P_0)
				{
					num2 = 94721021;
					num20 = num2;
				}
				else
				{
					num2 = 1501652115;
					num20 = num2;
				}
			}
			goto IL_0007;
			IL_024e:
			num = P_0;
			goto IL_0252;
		}

		private static int[] _200C_206F_206B_206C_202C_200C_202E_206F_200E_202B_206F_200C_206B_206B_202D_206B_206B_200B_206A_206A_200F_206A_206F_200C_202B_206E_206A_202C_206D_202B_206C_206A_206E_202E_202E_206B_206A_200E_200F_200D_202E(int P_0, int P_1)
		{
			if (P_0 >= 2)
			{
				goto IL_0007;
			}
			int num = 2;
			goto IL_015f;
			IL_015b:
			num = P_0;
			goto IL_015f;
			IL_0007:
			int num2 = 1861689059;
			goto IL_000c;
			IL_000c:
			int num16 = default(int);
			int num13 = default(int);
			int num18 = default(int);
			int num17 = default(int);
			int[] array = default(int[]);
			int num8 = default(int);
			int num19 = default(int);
			int num20 = default(int);
			int key = default(int);
			while (true)
			{
				uint num3;
				int num7;
				int num4;
				switch ((num3 = (uint)num2 ^ 0x2AB618C8u) % 33)
				{
				case 22u:
					break;
				case 4u:
					num7 = 0;
					num16 = 0;
					num2 = ((int)num3 * -746354734) ^ -1809804178;
					continue;
				case 9u:
					num13 = num16;
					num18 = num13 + 1;
					num2 = 1638391880;
					continue;
				case 5u:
					num16++;
					num17++;
					num2 = (int)((num3 * 1548298846) ^ 0x24A63C61);
					continue;
				case 18u:
					goto IL_00ed;
				case 28u:
					goto IL_0108;
				case 6u:
					num2 = ((int)num3 * -1698594851) ^ 0x65D90EC;
					continue;
				case 32u:
					array[num7++] = num18;
					array[num7++] = num8;
					num2 = (int)(num3 * 2117424523) ^ -1667140177;
					continue;
				case 29u:
					goto IL_015b;
				case 30u:
					num17 = 0;
					num2 = 991358825;
					continue;
				case 7u:
					num8 = num19 + 1;
					num2 = (int)((num3 * 174236038) ^ 0x520D052D);
					continue;
				case 15u:
					num20 = 0;
					num2 = ((int)num3 * -540320151) ^ -1231670462;
					continue;
				case 25u:
				{
					key = 0;
					int num11;
					int num12;
					if (_200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E)
					{
						num11 = 1978949709;
						num12 = num11;
					}
					else
					{
						num11 = 321909106;
						num12 = num11;
					}
					num2 = num11 ^ ((int)num3 * -1577991528);
					continue;
				}
				case 13u:
					_200F_200E_200C_202E_206B_206A_202B_202A_202A_206F_206A_202C_206D_200B_200B_202C_206B_200D_200D_200C_200C_202E_206D_206F_200C_206F_200E_202D_200F_200E_202C_202E_206E_202D_200E_200E_200E_200C_200F_200E_202E = new Dictionary<int, int[]>();
					num2 = (int)((num3 * 983358918) ^ 0x536CB531);
					continue;
				case 17u:
					array[num7++] = num13;
					num2 = ((int)num3 * -115070692) ^ -11341387;
					continue;
				case 8u:
					goto IL_0206;
				case 1u:
					array[num7++] = num8;
					num2 = ((int)num3 * -1852258134) ^ -1216112140;
					continue;
				case 11u:
					return array;
				case 24u:
					_200F_200E_200C_202E_206B_206A_202B_202A_202A_206F_206A_202C_206D_200B_200B_202C_206B_200D_200D_200C_200C_202E_206D_206F_200C_206F_200E_202D_200F_200E_202C_202E_206E_202D_200E_200E_200E_200C_200F_200E_202E[key] = array;
					num2 = ((int)num3 * -1788100980) ^ 0x17F68157;
					continue;
				case 31u:
				{
					int num21;
					int num22;
					if (array.Length != (P_0 - 1) * (P_1 - 1) * 6)
					{
						num21 = 76459727;
						num22 = num21;
					}
					else
					{
						num21 = 1130715691;
						num22 = num21;
					}
					num2 = num21 ^ ((int)num3 * -1032450829);
					continue;
				}
				case 3u:
					num19 = num16 + P_1;
					num2 = ((int)num3 * -213321160) ^ 0x221E1526;
					continue;
				case 23u:
					if (P_1 >= 2)
					{
						num2 = (int)((num3 * 2120822292) ^ 0x41EECB47);
						continue;
					}
					num4 = 2;
					goto IL_03e1;
				case 27u:
					array = _200F_200E_200C_202E_206B_206A_202B_202A_202A_206F_206A_202C_206D_200B_200B_202C_206B_200D_200D_200C_200C_202E_206D_206F_200C_206F_200E_202D_200F_200E_202C_202E_206E_202D_200E_200E_200E_200C_200F_200E_202E[key];
					num2 = ((int)num3 * -381064415) ^ 0x5F6BE22E;
					continue;
				case 2u:
					num16++;
					num20++;
					num2 = ((int)num3 * -1999383958) ^ -546327946;
					continue;
				case 0u:
					array = new int[(P_0 - 1) * (P_1 - 1) * 6];
					num2 = 29259775;
					continue;
				case 19u:
					array = null;
					num2 = (int)((num3 * 602030553) ^ 0x5896E61A);
					continue;
				case 10u:
					key = _200D_200D_202D_200B_202D_202E_206D_200E_202C_200B_200C_206D_206C_202D_206B_200E_202C_206A_202E_202E_206B_202D_200C_200F_200F_206B_202E_202E_202B_206D_200E_206E_200D_200D_200C_200E_206F_206B_202E_206D_202E(P_0, P_1);
					num2 = 2065148146;
					continue;
				case 12u:
					array[num7++] = num19;
					array[num7++] = num13;
					num2 = ((int)num3 * -1563434338) ^ 0x7E47332D;
					continue;
				case 14u:
				{
					int num14;
					int num15;
					if (_200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E)
					{
						num14 = 1184745294;
						num15 = num14;
					}
					else
					{
						num14 = 886111799;
						num15 = num14;
					}
					num2 = num14 ^ (int)(num3 * 904632004);
					continue;
				}
				case 16u:
				{
					int num9;
					int num10;
					if (_200F_200E_200C_202E_206B_206A_202B_202A_202A_206F_206A_202C_206D_200B_200B_202C_206B_200D_200D_200C_200C_202E_206D_206F_200C_206F_200E_202D_200F_200E_202C_202E_206E_202D_200E_200E_200E_200C_200F_200E_202E != null)
					{
						num9 = 667904277;
						num10 = num9;
					}
					else
					{
						num9 = 836782337;
						num10 = num9;
					}
					num2 = num9 ^ (int)(num3 * 1764996710);
					continue;
				}
				case 20u:
				{
					int num5;
					int num6;
					if (!_200F_200E_200C_202E_206B_206A_202B_202A_202A_206F_206A_202C_206D_200B_200B_202C_206B_200D_200D_200C_200C_202E_206D_206F_200C_206F_200E_202D_200F_200E_202C_202E_206E_202D_200E_200E_200E_200C_200F_200E_202E.ContainsKey(key))
					{
						num5 = -15211854;
						num6 = num5;
					}
					else
					{
						num5 = -114276396;
						num6 = num5;
					}
					num2 = num5 ^ (int)(num3 * 1354797996);
					continue;
				}
				case 21u:
					num4 = P_1;
					goto IL_03e1;
				default:
					{
						return array;
					}
					IL_03e1:
					P_1 = num4;
					num2 = 538032815;
					continue;
				}
				break;
				IL_0206:
				int num23;
				if (array != null)
				{
					num2 = 370398839;
					num23 = num2;
				}
				else
				{
					num2 = 1703976322;
					num23 = num2;
				}
				continue;
				IL_0108:
				int num24;
				if (num20 < P_0 - 1)
				{
					num2 = 1739546252;
					num24 = num2;
				}
				else
				{
					num2 = 45944026;
					num24 = num2;
				}
				continue;
				IL_00ed:
				int num25;
				if (num17 >= P_1 - 1)
				{
					num2 = 682519696;
					num25 = num2;
				}
				else
				{
					num2 = 400332011;
					num25 = num2;
				}
			}
			goto IL_0007;
			IL_015f:
			P_0 = num;
			num2 = 315050194;
			goto IL_000c;
		}

		private static Vector2[] _206F_200B_206F_206F_200E_202B_200E_202C_206E_202D_202D_206A_206E_206C_200B_200E_200C_200C_202C_202C_200F_206D_202B_206E_206E_200D_202D_200B_200E_206B_200D_206D_200F_200F_206F_206F_200C_206A_202B_200E_202E(int P_0, int P_1)
		{
			if (P_0 < 2)
			{
				goto IL_0007;
			}
			goto IL_019e;
			IL_0007:
			int num = -1822763807;
			goto IL_000c;
			IL_000c:
			int key = default(int);
			int num5 = default(int);
			int num6 = default(int);
			Vector2[] array = default(Vector2[]);
			int num7 = default(int);
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0xFE774776u) % 24)
				{
				case 14u:
					break;
				case 23u:
					P_0 = 2;
					num = ((int)num2 * -266212299) ^ -1651395284;
					continue;
				case 19u:
				{
					int num10;
					int num11;
					if (!_202D_206C_202B_202C_202B_206B_206F_202C_202D_202E_200D_200C_206B_206D_206F_200E_206B_202D_202E_200C_202C_200F_202C_200F_202C_202A_202B_200F_202D_202B_202D_200D_206F_206E_200D_202E_200B_206D_202D_200E_202E.ContainsKey(key))
					{
						num10 = -1001430976;
						num11 = num10;
					}
					else
					{
						num10 = -625398919;
						num11 = num10;
					}
					num = num10 ^ (int)(num2 * 2136577131);
					continue;
				}
				case 5u:
					num5 = 0;
					num = (int)(num2 * 840410594) ^ -91934497;
					continue;
				case 3u:
					num5++;
					num = (int)((num2 * 35409219) ^ 0x2ECEF744);
					continue;
				case 16u:
					num6 = 0;
					num = -1317704322;
					continue;
				case 20u:
				{
					int num12;
					int num13;
					if (array.Length == P_0 * P_1)
					{
						num12 = -956306915;
						num13 = num12;
					}
					else
					{
						num12 = -885600202;
						num13 = num12;
					}
					num = num12 ^ (int)(num2 * 1112126099);
					continue;
				}
				case 21u:
				{
					int num14;
					int num15;
					if (_202D_206C_202B_202C_202B_206B_206F_202C_202D_202E_200D_200C_206B_206D_206F_200E_206B_202D_202E_200C_202C_200F_202C_200F_202C_202A_202B_200F_202D_202B_202D_200D_206F_206E_200D_202E_200B_206D_202D_200E_202E == null)
					{
						num14 = -335425294;
						num15 = num14;
					}
					else
					{
						num14 = -919163013;
						num15 = num14;
					}
					num = num14 ^ ((int)num2 * -372389516);
					continue;
				}
				case 4u:
					array = new Vector2[P_0 * P_1];
					num7 = 0;
					num = -1152038837;
					continue;
				case 11u:
					goto IL_0159;
				case 7u:
					goto IL_0171;
				case 2u:
					P_1 = 2;
					num = (int)((num2 * 515937649) ^ 0x6476E550);
					continue;
				case 17u:
					goto IL_019e;
				case 18u:
				{
					int num8;
					int num9;
					if (!_200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E)
					{
						num8 = 388002867;
						num9 = num8;
					}
					else
					{
						num8 = 1902170399;
						num9 = num8;
					}
					num = num8 ^ (int)(num2 * 659836130);
					continue;
				}
				case 12u:
					array = null;
					num = -2145900220;
					continue;
				case 22u:
					array[num7++] = new Vector2((float)num5 / (float)(P_0 - 1), (float)num6 / (float)(P_1 - 1));
					num6++;
					num = -1317704322;
					continue;
				case 0u:
					_202D_206C_202B_202C_202B_206B_206F_202C_202D_202E_200D_200C_206B_206D_206F_200E_206B_202D_202E_200C_202C_200F_202C_200F_202C_202A_202B_200F_202D_202B_202D_200D_206F_206E_200D_202E_200B_206D_202D_200E_202E = new Dictionary<int, Vector2[]>();
					num = ((int)num2 * -827461409) ^ 0x3DF7EB3F;
					continue;
				case 9u:
					key = _200D_200D_202D_200B_202D_202E_206D_200E_202C_200B_200C_206D_206C_202D_206B_200E_202C_206A_202E_202E_206B_202D_200C_200F_200F_206B_202E_202E_202B_206D_200E_206E_200D_200D_200C_200E_206F_206B_202E_206D_202E(P_0, P_1);
					num = -1718518835;
					continue;
				case 6u:
					array = _202D_206C_202B_202C_202B_206B_206F_202C_202D_202E_200D_200C_206B_206D_206F_200E_206B_202D_202E_200C_202C_200F_202C_200F_202C_202A_202B_200F_202D_202B_202D_200D_206F_206E_200D_202E_200B_206D_202D_200E_202E[key];
					num = (int)((num2 * 1945138566) ^ 0x4D79C38D);
					continue;
				case 15u:
					return array;
				case 10u:
				{
					key = 0;
					int num3;
					int num4;
					if (!_200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E)
					{
						num3 = 419866585;
						num4 = num3;
					}
					else
					{
						num3 = 2146044123;
						num4 = num3;
					}
					num = num3 ^ ((int)num2 * -1298065768);
					continue;
				}
				case 13u:
					_202D_206C_202B_202C_202B_206B_206F_202C_202D_202E_200D_200C_206B_206D_206F_200E_206B_202D_202E_200C_202C_200F_202C_200F_202C_202A_202B_200F_202D_202B_202D_200D_206F_206E_200D_202E_200B_206D_202D_200E_202E[key] = array;
					num = (int)(num2 * 1234649463) ^ -718279972;
					continue;
				case 8u:
					goto IL_02bd;
				default:
					return array;
				}
				break;
				IL_02bd:
				int num16;
				if (num6 < P_1)
				{
					num = -252846968;
					num16 = num;
				}
				else
				{
					num = -729185187;
					num16 = num;
				}
				continue;
				IL_0171:
				int num17;
				if (array != null)
				{
					num = -1116580038;
					num17 = num;
				}
				else
				{
					num = -169876590;
					num17 = num;
				}
				continue;
				IL_0159:
				int num18;
				if (num5 < P_0)
				{
					num = -2017684170;
					num18 = num;
				}
				else
				{
					num = -496463804;
					num18 = num;
				}
			}
			goto IL_0007;
			IL_019e:
			int num19;
			if (P_1 < 2)
			{
				num = -1085281244;
				num19 = num;
			}
			else
			{
				num = -1152598942;
				num19 = num;
			}
			goto IL_000c;
		}

		internal static void _202D_206D_200E_202C_200F_206C_200E_202B_202B_206A_206B_202C_200D_200B_206E_206A_206C_202E_206A_202B_202D_206F_202E_200B_206B_202B_206B_202A_200D_202B_206A_206B_202C_200E_200D_206E_200F_206A_202A_200E_202E(int P_0, int P_1, out int[] P_2, out int[] P_3, out int[] P_4, out int[] P_5)
		{
			if (P_0 < 2)
			{
				goto IL_0007;
			}
			goto IL_0234;
			IL_0007:
			int num = -2018769714;
			goto IL_000c;
			IL_000c:
			int num6 = default(int);
			int num3 = default(int);
			int num5 = default(int);
			int num4 = default(int);
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0xAB483D94u) % 23)
				{
				case 6u:
					break;
				default:
					return;
				case 14u:
					num = (int)(num2 * 452010614) ^ -720036985;
					continue;
				case 3u:
					P_3[num6] = P_0 * P_1 - 1 - num6;
					num6++;
					num = -1840545512;
					continue;
				case 12u:
					P_4[num3] = P_0 * P_1 - P_1 - num3 * P_1;
					num = -1665362649;
					continue;
				case 4u:
					P_4 = new int[P_0];
					P_5 = new int[P_1];
					num = ((int)num2 * -280028822) ^ -524939560;
					continue;
				case 7u:
					P_5[num5] = num5;
					num5++;
					num = -2063895365;
					continue;
				case 1u:
					num6 = 0;
					num = ((int)num2 * -1665946057) ^ 0x7F39571A;
					continue;
				case 10u:
					P_1 = 2;
					num = ((int)num2 * -941339311) ^ -1086531972;
					continue;
				case 11u:
					goto IL_0126;
				case 0u:
					P_2[num4] = P_1 - 1 + P_1 * num4;
					num4++;
					num = -1373466272;
					continue;
				case 16u:
					goto IL_0157;
				case 15u:
					num3 = 0;
					num = (int)(num2 * 485139351) ^ -1466964161;
					continue;
				case 19u:
					num5 = 0;
					num = ((int)num2 * -1774451355) ^ 0x329E8A3;
					continue;
				case 8u:
					num = (int)((num2 * 673037217) ^ 0x4DBB1A5E);
					continue;
				case 20u:
					num4 = 0;
					num = (int)((num2 * 1879163647) ^ 0x34E0A6DC);
					continue;
				case 22u:
					P_2 = new int[P_0];
					P_3 = new int[P_1];
					num = -1869116413;
					continue;
				case 18u:
					goto IL_01db;
				case 5u:
					P_0 = 2;
					num = ((int)num2 * -1409609512) ^ -427965907;
					continue;
				case 13u:
					num = ((int)num2 * -539328337) ^ 0x6D1668AA;
					continue;
				case 9u:
					goto IL_021c;
				case 2u:
					goto IL_0234;
				case 17u:
					num3++;
					num = ((int)num2 * -1612301370) ^ -150096371;
					continue;
				case 21u:
					return;
				}
				break;
				IL_021c:
				int num7;
				if (num4 >= P_0)
				{
					num = -2088528742;
					num7 = num;
				}
				else
				{
					num = -381601553;
					num7 = num;
				}
				continue;
				IL_0157:
				int num8;
				if (num6 < P_1)
				{
					num = -1829362299;
					num8 = num;
				}
				else
				{
					num = -170822355;
					num8 = num;
				}
				continue;
				IL_0126:
				int num9;
				if (num3 < P_0)
				{
					num = -618274866;
					num9 = num;
				}
				else
				{
					num = -1347331844;
					num9 = num;
				}
				continue;
				IL_01db:
				int num10;
				if (num5 < P_1)
				{
					num = -70757424;
					num10 = num;
				}
				else
				{
					num = -124412945;
					num10 = num;
				}
			}
			goto IL_0007;
			IL_0234:
			int num11;
			if (P_1 >= 2)
			{
				num = -330580838;
				num11 = num;
			}
			else
			{
				num = -946268526;
				num11 = num;
			}
			goto IL_000c;
		}

		private static int[] _200B_200B_200D_202A_200D_206F_202E_206A_206F_206A_202C_206C_202D_206F_202A_200C_200F_200F_202C_200D_202D_206A_202D_206F_206A_202B_202A_206C_202C_206B_200D_206B_202A_202A_200D_202C_200C_202D_206C_206E_202E(int P_0, int P_1)
		{
			if (P_0 >= 4)
			{
				goto IL_0007;
			}
			goto IL_01a3;
			IL_0007:
			int num = 767375861;
			goto IL_000c;
			IL_000c:
			int num3 = default(int);
			int num4 = default(int);
			int key = default(int);
			int num6 = default(int);
			int[] array = default(int[]);
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0x699B7618u) % 27)
				{
				case 14u:
					break;
				case 22u:
					num = (int)(num2 * 565573165) ^ -1746343203;
					continue;
				case 18u:
					goto IL_00a1;
				case 16u:
					num3 = 0;
					num = ((int)num2 * -1959531613) ^ 0x36E0FCA1;
					continue;
				case 15u:
					num4++;
					num = ((int)num2 * -1294931520) ^ -389133871;
					continue;
				case 9u:
					_206A_206D_202E_202C_206F_202B_200F_206F_200C_202B_202D_202A_200E_202B_206F_202B_202C_206C_206E_206F_206B_202E_206F_206A_202B_200F_202A_202B_206D_200E_200F_200F_202B_206B_202C_200B_206D_200E_202C_206A_202E = new Dictionary<int, int[]>();
					num = (int)((num2 * 978795545) ^ 0x6715772);
					continue;
				case 17u:
					key = 0;
					num = ((int)num2 * -492079813) ^ 0x73072996;
					continue;
				case 3u:
					num6 = -1;
					num = (int)((num2 * 288232586) ^ 0x3C565363);
					continue;
				case 6u:
				{
					int num13;
					int num14;
					if (array.Length != (P_0 - 2) * (P_1 - 2))
					{
						num13 = 1684391182;
						num14 = num13;
					}
					else
					{
						num13 = 1925829098;
						num14 = num13;
					}
					num = num13 ^ (int)(num2 * 970325564);
					continue;
				}
				case 1u:
					goto IL_0156;
				case 24u:
				{
					int num9;
					int num10;
					if (!_200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E)
					{
						num9 = 1455372589;
						num10 = num9;
					}
					else
					{
						num9 = 1734567365;
						num10 = num9;
					}
					num = num9 ^ (int)(num2 * 62122556);
					continue;
				}
				case 8u:
					goto IL_01a3;
				case 19u:
					goto IL_01ba;
				case 20u:
					_206A_206D_202E_202C_206F_202B_200F_206F_200C_202B_202D_202A_200E_202B_206F_202B_202C_206C_206E_206F_206B_202E_206F_206A_202B_200F_202A_202B_206D_200E_200F_200F_202B_206B_202C_200B_206D_200E_202C_206A_202E[key] = array;
					num = ((int)num2 * -1862406274) ^ -2106300333;
					continue;
				case 5u:
				{
					int num15;
					int num16;
					if (_206A_206D_202E_202C_206F_202B_200F_206F_200C_202B_202D_202A_200E_202B_206F_202B_202C_206C_206E_206F_206B_202E_206F_206A_202B_200F_202A_202B_206D_200E_200F_200F_202B_206B_202C_200B_206D_200E_202C_206A_202E != null)
					{
						num15 = 938969491;
						num16 = num15;
					}
					else
					{
						num15 = 1347533233;
						num16 = num15;
					}
					num = num15 ^ (int)(num2 * 1313207421);
					continue;
				}
				case 10u:
					return array;
				case 0u:
					array = new int[(P_0 - 2) * (P_1 - 2)];
					num = 975742458;
					continue;
				case 11u:
				{
					int num11;
					int num12;
					if (!_200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E)
					{
						num11 = 1279570510;
						num12 = num11;
					}
					else
					{
						num11 = 526674511;
						num12 = num11;
					}
					num = num11 ^ (int)(num2 * 769550387);
					continue;
				}
				case 2u:
					num4 = 0;
					num = 2093467556;
					continue;
				case 12u:
					array = _206A_206D_202E_202C_206F_202B_200F_206F_200C_202B_202D_202A_200E_202B_206F_202B_202C_206C_206E_206F_206B_202E_206F_206A_202B_200F_202A_202B_206D_200E_200F_200F_202B_206B_202C_200B_206D_200E_202C_206A_202E[key];
					num = (int)(num2 * 166581727) ^ -1965062324;
					continue;
				case 4u:
					num3++;
					num = ((int)num2 * -22663597) ^ 0x695F2C84;
					continue;
				case 23u:
				{
					int num7;
					int num8;
					if (P_1 < 4)
					{
						num7 = -1532102639;
						num8 = num7;
					}
					else
					{
						num7 = -1620857832;
						num8 = num7;
					}
					num = num7 ^ (int)(num2 * 856454506);
					continue;
				}
				case 26u:
				{
					int num5 = P_1 + 1 + num3 * P_1 + num4;
					array[++num6] = num5;
					num = 544037588;
					continue;
				}
				case 21u:
					return null;
				case 25u:
					goto IL_02ff;
				case 13u:
					array = null;
					num = 475801669;
					continue;
				default:
					return array;
				}
				break;
				IL_02ff:
				int num17;
				if (num4 >= P_1 - 2)
				{
					num = 1359609216;
					num17 = num;
				}
				else
				{
					num = 599428267;
					num17 = num;
				}
				continue;
				IL_0156:
				key = _200D_200D_202D_200B_202D_202E_206D_200E_202C_200B_200C_206D_206C_202D_206B_200E_202C_206A_202E_202E_206B_202D_200C_200F_200F_206B_202E_202E_202B_206D_200E_206E_200D_200D_200C_200E_206F_206B_202E_206D_202E(P_0, P_1);
				int num18;
				if (!_206A_206D_202E_202C_206F_202B_200F_206F_200C_202B_202D_202A_200E_202B_206F_202B_202C_206C_206E_206F_206B_202E_206F_206A_202B_200F_202A_202B_206D_200E_200F_200F_202B_206B_202C_200B_206D_200E_202C_206A_202E.ContainsKey(key))
				{
					num = 310735773;
					num18 = num;
				}
				else
				{
					num = 482205015;
					num18 = num;
				}
				continue;
				IL_00a1:
				int num19;
				if (array != null)
				{
					num = 1252461753;
					num19 = num;
				}
				else
				{
					num = 1084800690;
					num19 = num;
				}
				continue;
				IL_01ba:
				int num20;
				if (num3 >= P_0 - 2)
				{
					num = 569233694;
					num20 = num;
				}
				else
				{
					num = 506469666;
					num20 = num;
				}
			}
			goto IL_0007;
			IL_01a3:
			Utilities.Debug(LogType.Error, "GenerateGridInnerIndex: wrong arguments", null);
			num = 1749953810;
			goto IL_000c;
		}

		private static Vector3[] _202C_200E_206C_202E_202B_200B_200E_206A_200E_206E_206F_202E_206B_206D_200D_206C_206F_206B_206F_200B_206D_206F_206C_202A_202C_202A_206E_202B_206C_200E_202A_206C_206F_206F_206F_206A_202E_200B_200E_202E(Vector3[] P_0, int[] P_1, float P_2)
		{
			Vector3[] array = new Vector3[P_1.Length * 2];
			int num3 = default(int);
			Vector3 vector = default(Vector3);
			while (true)
			{
				int num = -1662457471;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0xEE922BB7u) % 8)
					{
					case 3u:
						break;
					case 6u:
						num3 = 0;
						num = ((int)num2 * -1385055720) ^ -1650114000;
						continue;
					case 0u:
						vector.y = P_2;
						num = (int)(num2 * 1502104339) ^ -220161949;
						continue;
					case 4u:
						array[num3 * 2 + 1] = vector;
						num3++;
						num = (int)((num2 * 366649257) ^ 0x987C48C);
						continue;
					case 5u:
						vector = P_0[P_1[num3]];
						num = -860131291;
						continue;
					case 2u:
						array[num3 * 2] = vector;
						num = ((int)num2 * -2090995317) ^ -783053447;
						continue;
					case 7u:
					{
						int num4;
						if (num3 >= P_1.Length)
						{
							num = -274590186;
							num4 = num;
						}
						else
						{
							num = -899534254;
							num4 = num;
						}
						continue;
					}
					default:
						return array;
					}
					break;
				}
			}
		}

		private static int[] _200F_206C_206E_200D_202B_200E_202A_200F_206C_200B_202D_202A_202A_200C_206B_206E_206D_200C_200E_206C_202C_206C_206D_200C_200C_206A_206E_206F_206F_206F_202E_206B_200F_206F_206C_206F_202E_206B_202D_206B_202E(int P_0, int P_1)
		{
			int num = P_0 - 1;
			int[] array = null;
			int key = 0;
			int num7 = default(int);
			int num15 = default(int);
			int num17 = default(int);
			int num18 = default(int);
			int num14 = default(int);
			int num16 = default(int);
			while (true)
			{
				int num2 = -1465221916;
				while (true)
				{
					uint num3;
					int num13;
					switch ((num3 = (uint)num2 ^ 0xA21B8AD6u) % 24)
					{
					case 12u:
						break;
					case 18u:
					{
						int num11;
						int num12;
						if (!_200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E)
						{
							num11 = 841465998;
							num12 = num11;
						}
						else
						{
							num11 = 77713237;
							num12 = num11;
						}
						num2 = num11 ^ ((int)num3 * -1116835551);
						continue;
					}
					case 14u:
						_202A_202E_202C_206A_206E_206F_202A_200D_206F_206E_202A_200F_200E_206B_200C_206A_200E_202D_200C_206D_202D_206E_202C_206A_200C_202E_200C_206F_200C_200F_202A_206E_206C_202E_202B_202D_200D_202E_206B_200E_202E[key] = array;
						num2 = (int)((num3 * 687966389) ^ 0x598A8F27);
						continue;
					case 22u:
					{
						int num8;
						if (num7 >= num)
						{
							num2 = -1554148779;
							num8 = num2;
						}
						else
						{
							num2 = -671033454;
							num8 = num2;
						}
						continue;
					}
					case 15u:
						num7++;
						num2 = ((int)num3 * -969420181) ^ -859241251;
						continue;
					case 9u:
					{
						int num20;
						int num21;
						if (_202A_202E_202C_206A_206E_206F_202A_200D_206F_206E_202A_200F_200E_206B_200C_206A_200E_202D_200C_206D_202D_206E_202C_206A_200C_202E_200C_206F_200C_200F_202A_206E_206C_202E_202B_202D_200D_202E_206B_200E_202E == null)
						{
							num20 = -1955948018;
							num21 = num20;
						}
						else
						{
							num20 = -1668114961;
							num21 = num20;
						}
						num2 = num20 ^ (int)(num3 * 712081988);
						continue;
					}
					case 1u:
						array = new int[num * 6];
						num13 = 0;
						num2 = -1685926706;
						continue;
					case 11u:
					{
						int num5;
						int num6;
						if (array.Length == num * 6)
						{
							num5 = 711959468;
							num6 = num5;
						}
						else
						{
							num5 = 1634260739;
							num6 = num5;
						}
						num2 = num5 ^ (int)(num3 * 88011156);
						continue;
					}
					case 16u:
						num15 = 0;
						num2 = (int)((num3 * 1586911854) ^ 0x5873EB9B);
						continue;
					case 10u:
						array[num13++] = num17;
						num2 = ((int)num3 * -1776267607) ^ 0x65C5F27F;
						continue;
					case 7u:
						num15 += 2;
						num2 = ((int)num3 * -630075574) ^ 0x1472FA7;
						continue;
					case 20u:
						_202A_202E_202C_206A_206E_206F_202A_200D_206F_206E_202A_200F_200E_206B_200C_206A_200E_202D_200C_206D_202D_206E_202C_206A_200C_202E_200C_206F_200C_200F_202A_206E_206C_202E_202B_202D_200D_202E_206B_200E_202E = new Dictionary<int, int[]>();
						num2 = ((int)num3 * -823243141) ^ -1961082785;
						continue;
					case 3u:
						array[num13++] = num18;
						array[num13++] = num17;
						num2 = ((int)num3 * -1777563917) ^ -1921522634;
						continue;
					case 5u:
					{
						key = _200D_200D_202D_200B_202D_202E_206D_200E_202C_200B_200C_206D_206C_202D_206B_200E_202C_206A_202E_202E_206B_202D_200C_200F_200F_206B_202E_202E_202B_206D_200E_206E_200D_200D_200C_200E_206F_206B_202E_206D_202E(P_0, P_1);
						int num19;
						if (_202A_202E_202C_206A_206E_206F_202A_200D_206F_206E_202A_200F_200E_206B_200C_206A_200E_202D_200C_206D_202D_206E_202C_206A_200C_202E_200C_206F_200C_200F_202A_206E_206C_202E_202B_202D_200D_202E_206B_200E_202E.ContainsKey(key))
						{
							num2 = -1469055970;
							num19 = num2;
						}
						else
						{
							num2 = -1874915588;
							num19 = num2;
						}
						continue;
					}
					case 4u:
						num18 = P_1 + num15;
						num14 = P_1 + num15 + 2;
						num2 = -1403250933;
						continue;
					case 21u:
						num7 = 0;
						num2 = ((int)num3 * -510511149) ^ 0x103C59B7;
						continue;
					case 8u:
						num17 = P_1 + num15 + 3;
						array[num13++] = num18;
						array[num13++] = num16;
						num2 = (int)((num3 * 495479254) ^ 0x732ABDF4);
						continue;
					case 13u:
						num16 = P_1 + num15 + 1;
						num2 = ((int)num3 * -1031938417) ^ 0x51B8362D;
						continue;
					case 17u:
						array[num13++] = num14;
						num2 = (int)(num3 * 160908056) ^ -1546450975;
						continue;
					case 6u:
						return array;
					case 0u:
						array = _202A_202E_202C_206A_206E_206F_202A_200D_206F_206E_202A_200F_200E_206B_200C_206A_200E_202D_200C_206D_202D_206E_202C_206A_200C_202E_200C_206F_200C_200F_202A_206E_206C_202E_202B_202D_200D_202E_206B_200E_202E[key];
						num2 = ((int)num3 * -2061441110) ^ 0x59BA562C;
						continue;
					case 19u:
					{
						int num9;
						int num10;
						if (_200E_202D_206C_200F_206E_206A_202C_202C_202E_206A_206B_206B_200F_200D_200C_206F_202A_200F_200C_206E_206C_202B_200C_206B_200F_200B_206F_200D_200D_200E_200B_206A_202D_202D_200E_202C_206B_200E_202C_200D_202E)
						{
							num9 = -1683663562;
							num10 = num9;
						}
						else
						{
							num9 = -1555942865;
							num10 = num9;
						}
						num2 = num9 ^ ((int)num3 * -2141523254);
						continue;
					}
					case 2u:
					{
						int num4;
						if (array == null)
						{
							num2 = -1485907681;
							num4 = num2;
						}
						else
						{
							num2 = -223405651;
							num4 = num2;
						}
						continue;
					}
					default:
						return array;
					}
					break;
				}
			}
		}

		private static Vector2[] _202A_200B_202A_200D_202E_206C_202C_206D_202A_202C_206F_200D_206D_206B_202E_200F_206A_200D_206C_202D_206C_200F_200C_206B_200F_206E_206F_206C_206D_200D_206A_202D_202E_202C_206F_206B_206B_202B_200C_200C_202E(Vector2[] P_0, int[] P_1, bool P_2, bool P_3)
		{
			Vector2[] array = new Vector2[P_1.Length * 2];
			float num6 = default(float);
			int num3 = default(int);
			float x = default(float);
			while (true)
			{
				int num = -1327210877;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0xEBC305EAu) % 10)
					{
					case 3u:
						break;
					case 1u:
						num6 = 1f / (float)(P_1.Length - 1);
						num3 = 0;
						num = ((int)num2 * -1079656685) ^ -1289959100;
						continue;
					case 6u:
					{
						int num5;
						if (!(P_3 && P_2))
						{
							num = -111961594;
							num5 = num;
						}
						else
						{
							num = -1371573123;
							num5 = num;
						}
						continue;
					}
					case 5u:
						x = 1f - (float)num3 * num6;
						num = ((int)num2 * -957573262) ^ 0x123BDB79;
						continue;
					case 9u:
						num = ((int)num2 * -1856844882) ^ -1753511946;
						continue;
					case 2u:
						array[num3 * 2] = (array[num3 * 2 + 1] = P_0[P_1[num3]]);
						num = -1245834996;
						continue;
					case 4u:
						num3++;
						num = -1978711472;
						continue;
					case 7u:
						array[num3 * 2] = new Vector2(x, 1f);
						array[num3 * 2 + 1] = new Vector2(x, 0f);
						num = ((int)num2 * -1990261730) ^ 0x38A9C94A;
						continue;
					case 8u:
					{
						int num4;
						if (num3 >= P_1.Length)
						{
							num = -942418204;
							num4 = num;
						}
						else
						{
							num = -1490492690;
							num4 = num;
						}
						continue;
					}
					default:
						return array;
					}
					break;
				}
			}
		}

		private static Vector2[] _206B_206E_206E_206C_206F_202B_200B_206F_206B_206B_206D_202A_206E_202B_202B_206D_200C_200D_202E_206B_200D_202A_206E_206E_206F_200B_206F_202E_206A_206C_206F_200D_206F_206A_202A_206A_206D_202E_202E_200C_202E(int P_0, int P_1, int P_2)
		{
			if (_202B_200E_202B_200B_202D_200C_202B_200E_202E_200B_206D_202C_206D_206D_206A_200B_206E_206A_200E_206F_206A_206F_200F_200F_202A_202D_206F_200D_206D_206A_202C_200F_202A_200E_202E_200E_200F_202B_200B_202B_202E == null)
			{
				goto IL_000a;
			}
			goto IL_009c;
			IL_000a:
			int num = 2074011231;
			goto IL_000f;
			IL_000f:
			int key = default(int);
			Vector2[] array = default(Vector2[]);
			float y = default(float);
			float num8 = default(float);
			int num6 = default(int);
			float num7 = default(float);
			int num9 = default(int);
			float num10 = default(float);
			float num3 = default(float);
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0x7B86BE83u) % 22)
				{
				case 14u:
					break;
				case 6u:
					_202B_200E_202B_200B_202D_200C_202B_200E_202E_200B_206D_202C_206D_206D_206A_200B_206E_206A_200E_206F_206A_206F_200F_200F_202A_202D_206F_200D_206D_206A_202C_200F_202A_200E_202E_200E_200F_202B_200B_202B_202E[key] = array;
					num = (int)(num2 * 1949853607) ^ -926200714;
					continue;
				case 12u:
					goto IL_009c;
				case 10u:
					goto IL_00b1;
				case 9u:
					array = _202B_200E_202B_200B_202D_200C_202B_200E_202E_200B_206D_202C_206D_206D_206A_200B_206E_206A_200E_206F_206A_206F_200F_200F_202A_202D_206F_200D_206D_206A_202C_200F_202A_200E_202E_200E_200F_202B_200B_202B_202E[key];
					num = ((int)num2 * -526530049) ^ 0x4F31DD3B;
					continue;
				case 3u:
				{
					int num13;
					int num14;
					if (array.Length != P_0 * 2)
					{
						num13 = 1124335823;
						num14 = num13;
					}
					else
					{
						num13 = 786673450;
						num14 = num13;
					}
					num = num13 ^ (int)(num2 * 149182319);
					continue;
				}
				case 2u:
					y = 0f;
					num = ((int)num2 * -1519461955) ^ 0x513CF4F0;
					continue;
				case 21u:
					num8 = (float)num6 * num7;
					y = 1f;
					num = (int)((num2 * 1828301834) ^ 0x5A8243C6);
					continue;
				case 1u:
					array[num9] = new Vector2(num8, y);
					num9++;
					num = (int)((num2 * 976249914) ^ 0x5676E675);
					continue;
				case 11u:
					num10 = (float)P_2 / (float)P_1;
					num = (int)((num2 * 1475942528) ^ 0x5C277105);
					continue;
				case 7u:
				{
					int num11;
					int num12;
					if (num9 % 2 == 0)
					{
						num11 = -1890321681;
						num12 = num11;
					}
					else
					{
						num11 = -1587819399;
						num12 = num11;
					}
					num = num11 ^ ((int)num2 * -1430127293);
					continue;
				}
				case 8u:
					num9 = 0;
					num = ((int)num2 * -1247691137) ^ -112633351;
					continue;
				case 18u:
					_202B_200E_202B_200B_202D_200C_202B_200E_202E_200B_206D_202C_206D_206D_206A_200B_206E_206A_200E_206F_206A_206F_200F_200F_202A_202D_206F_200D_206D_206A_202C_200F_202A_200E_202E_200E_200F_202B_200B_202B_202E = new Dictionary<int, Vector2[]>();
					num = ((int)num2 * -1175204808) ^ -644282259;
					continue;
				case 17u:
					array = new Vector2[P_0 * 2];
					num6 = 0;
					num = 1278227849;
					continue;
				case 0u:
					num7 = 1f / (float)(P_0 - 1);
					num = 1319577696;
					continue;
				case 16u:
					return array;
				case 4u:
					num8 = 1f - (num10 + num8 * num3);
					num = ((int)num2 * -1650358175) ^ 0x17E79164;
					continue;
				case 13u:
					num8 = (float)num6++ * num7;
					num = 1744630743;
					continue;
				case 20u:
				{
					int num4;
					int num5;
					if (!_202B_200E_202B_200B_202D_200C_202B_200E_202E_200B_206D_202C_206D_206D_206A_200B_206E_206A_200E_206F_206A_206F_200F_200F_202A_202D_206F_200D_206D_206A_202C_200F_202A_200E_202E_200E_200F_202B_200B_202B_202E.ContainsKey(key))
					{
						num4 = -1604428568;
						num5 = num4;
					}
					else
					{
						num4 = -1481816402;
						num5 = num4;
					}
					num = num4 ^ (int)(num2 * 1784752746);
					continue;
				}
				case 19u:
					goto IL_0281;
				case 5u:
					num3 = 1f / (float)P_1;
					num = 2128794636;
					continue;
				default:
					return array;
				}
				break;
				IL_0281:
				int num15;
				if (array == null)
				{
					num = 1341779950;
					num15 = num;
				}
				else
				{
					num = 985130220;
					num15 = num;
				}
				continue;
				IL_00b1:
				int num16;
				if (num9 < array.Length)
				{
					num = 1443556399;
					num16 = num;
				}
				else
				{
					num = 192565175;
					num16 = num;
				}
			}
			goto IL_000a;
			IL_009c:
			key = _206A_202C_200B_202D_206B_206B_202E_202C_200F_200B_200C_200C_206A_206A_200C_202D_202A_206A_200B_206E_200C_202E_202E_206E_200D_202C_206E_206E_206E_200C_206D_206D_200B_202A_200F_200C_202A_200C_202A_206B_202E(P_0, P_1, P_2);
			array = null;
			num = 326178945;
			goto IL_000f;
		}

		private static int _200D_200D_202D_200B_202D_202E_206D_200E_202C_200B_200C_206D_206C_202D_206B_200E_202C_206A_202E_202E_206B_202D_200C_200F_200F_206B_202E_202E_202B_206D_200E_206E_200D_200D_200C_200E_206F_206B_202E_206D_202E(int P_0, int P_1)
		{
			return _206B_202B_200B_206C_202B_206C_200E_200F_200F_206B_206A_200B_202B_200D_200B_200D_202B_200F_200E_206C_206E_200F_200B_202E_206E_206E_202E_206D_202E_206A_200E_202B_202A_200D_206A_206D_202B_206A_206C_206F_202E((object)_206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E(P_0.ToString(), "x", P_1.ToString()));
		}

		private static int _206A_202C_200B_202D_206B_206B_202E_202C_200F_200B_200C_200C_206A_206A_200C_202D_202A_206A_200B_206E_200C_202E_202E_206E_200D_202C_206E_206E_206E_200C_206D_206D_200B_202A_200F_200C_202A_200C_202A_206B_202E(int P_0, int P_1, int P_2)
		{
			return _206B_202B_200B_206C_202B_206C_200E_200F_200F_206B_206A_200B_202B_200D_200B_200D_202B_200F_200E_206C_206E_200F_200B_202E_206E_206E_202E_206D_202E_206A_200E_202B_202A_200D_206A_206D_202B_206A_206C_206F_202E((object)_206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[5]
			{
				P_0.ToString(),
				"x",
				P_1.ToString(),
				"x",
				P_2.ToString()
			}));
		}

		private bool _206A_200E_200E_202C_200D_200B_202A_206E_200B_206B_200D_202B_202E_206F_202B_202E_202B_202C_202C_200B_200F_206B_200F_202B_202C_202D_200D_206F_200D_206A_202A_202D_202A_202D_202D_206D_206F_200B_202D_202D_202E(int P_0, int P_1, int P_2, int P_3, int P_4, int P_5, EdgeFall P_6)
		{
			if (_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, (UnityEngine.Object)null))
			{
				goto IL_0011;
			}
			goto IL_0576;
			IL_0011:
			int num = -1689246967;
			goto IL_0016;
			IL_0016:
			int requiredVertexCount = default(int);
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0xDAE80BC5u) % 31)
				{
				case 3u:
					break;
				case 8u:
					goto IL_00a7;
				case 21u:
					return false;
				case 0u:
					Utilities.Debug(LogType.Error, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[5]
					{
						"Can not generate mesh with [",
						P_0.ToString(),
						", ",
						P_1.ToString(),
						"] vertices. Target device does not support 32 bits index buffers. Try reduse veretx count."
					}), null);
					return false;
				case 20u:
					goto IL_0127;
				case 16u:
					goto IL_0143;
				case 13u:
					return false;
				case 2u:
				{
					int num5;
					int num6;
					if (_200F_202D_202D_206E_200B_206A_206B_202B_206E_206F_206A_206E_206A_200F_200F_206F_206A_200B_200B_202E_200D_202B_206A_206A_200E_202E_206C_202A_200F_206E_202C_202B_200E_200F_206F_200E_200E_202C_202A_206F_202E())
					{
						num5 = -761933670;
						num6 = num5;
					}
					else
					{
						num5 = -1202726820;
						num6 = num5;
					}
					num = num5 ^ (int)(num2 * 1828882684);
					continue;
				}
				case 15u:
					Utilities.Debug(LogType.Warning, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[7]
					{
						"Can not convert '",
						_200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E),
						"' to mesh. PositionX '",
						P_4.ToString(),
						"' can not be more than '",
						P_2.ToString(),
						"'."
					}), null, _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E);
					num = (int)((num2 * 1331043056) ^ 0x1D96EEB0);
					continue;
				case 26u:
					requiredVertexCount = Utilities.GetRequiredVertexCount(P_0, P_1, P_2, P_3, P_6 != null);
					num = -1880692971;
					continue;
				case 11u:
					goto IL_0218;
				case 19u:
					return false;
				case 27u:
					Utilities.Debug(LogType.Warning, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[7]
					{
						"Can not convert '",
						_200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E),
						"' to mesh. PositionX '",
						P_5.ToString(),
						"' can not be more than '",
						P_3.ToString(),
						"'."
					}), null, _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E);
					return false;
				case 9u:
				{
					int num7;
					int num8;
					if (_206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E).z < 1f)
					{
						num7 = 941771402;
						num8 = num7;
					}
					else
					{
						num7 = 919231979;
						num8 = num7;
					}
					num = num7 ^ ((int)num2 * -2111163562);
					continue;
				}
				case 12u:
				{
					int num3;
					int num4;
					if (requiredVertexCount <= 0)
					{
						num3 = 549363674;
						num4 = num3;
					}
					else
					{
						num3 = 135188970;
						num4 = num3;
					}
					num = num3 ^ ((int)num2 * -2026152135);
					continue;
				}
				case 6u:
					return false;
				case 14u:
					Utilities.Debug(LogType.Warning, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[5]
					{
						"Can not convert '",
						_200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E),
						"' to mesh. Vertex count horizontal ",
						P_0.ToString(),
						" can not be less than 2."
					}), null, _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E);
					num = ((int)num2 * -2139560180) ^ -33415515;
					continue;
				case 24u:
					return false;
				case 22u:
					Utilities.Debug(LogType.Error, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[9]
					{
						"Can not generate mesh with [",
						P_0.ToString(),
						", ",
						P_1.ToString(),
						"] vertices and [",
						P_2.ToString(),
						", ",
						P_3.ToString(),
						"] chunks. Int value overflow."
					}), null);
					num = (int)(num2 * 1150484725) ^ -770670913;
					continue;
				case 7u:
					return false;
				case 18u:
					Utilities.Debug(LogType.Warning, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[5]
					{
						"Can not convert '",
						_200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E),
						"' to mesh. Chunk count horizontal ",
						P_2.ToString(),
						" can not be less than 1."
					}), null, _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E);
					num = ((int)num2 * -355150829) ^ -1145142259;
					continue;
				case 28u:
					Utilities.Debug(LogType.Warning, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[5]
					{
						"Can not convert '",
						_200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E),
						"' to mesh. Vertex count vertical ",
						P_1.ToString(),
						" can not be less than 1."
					}), null, _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E);
					return false;
				case 10u:
					Utilities.Debug(LogType.Warning, "terrainData == null", null);
					num = ((int)num2 * -1536369494) ^ 0x7DBA5DDF;
					continue;
				case 17u:
					Utilities.Debug(LogType.Warning, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[5]
					{
						"'",
						_200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E),
						"' size [",
						_206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E).ToString(),
						"] is not supported."
					}), null, _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E);
					num = -403915004;
					continue;
				case 30u:
					goto IL_054a;
				case 4u:
					return false;
				case 5u:
					goto IL_0576;
				case 1u:
					goto IL_05a1;
				case 25u:
					Utilities.Debug(LogType.Warning, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[5]
					{
						"Can not convert '",
						_200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E),
						"' to mesh. Chunk count vertical ",
						P_3.ToString(),
						" can not be less than 1."
					}), null, _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E);
					num = (int)(num2 * 607729595) ^ -787223684;
					continue;
				case 29u:
					goto IL_0616;
				default:
					return true;
				}
				break;
				IL_0616:
				int num9;
				if (requiredVertexCount > Constants.vertexLimitIn16BitsIndexBuffer)
				{
					num = -245599515;
					num9 = num;
				}
				else
				{
					num = -1428821222;
					num9 = num;
				}
				continue;
				IL_00a7:
				int num10;
				if (P_3 < 1)
				{
					num = -217932102;
					num10 = num;
				}
				else
				{
					num = -1286785997;
					num10 = num;
				}
				continue;
				IL_0143:
				int num11;
				if (P_2 < 1)
				{
					num = -2077000015;
					num11 = num;
				}
				else
				{
					num = -1309583068;
					num11 = num;
				}
				continue;
				IL_05a1:
				int num12;
				if (P_4 <= P_2 - 1)
				{
					num = -1894909451;
					num12 = num;
				}
				else
				{
					num = -1078306931;
					num12 = num;
				}
				continue;
				IL_0218:
				int num13;
				if (P_1 >= 2)
				{
					num = -1565624488;
					num13 = num;
				}
				else
				{
					num = -1843000468;
					num13 = num;
				}
				continue;
				IL_0127:
				int num14;
				if (P_5 <= P_3 - 1)
				{
					num = -689312087;
					num14 = num;
				}
				else
				{
					num = -7489813;
					num14 = num;
				}
				continue;
				IL_054a:
				int num15;
				if (P_0 < 2)
				{
					num = -677614646;
					num15 = num;
				}
				else
				{
					num = -408650920;
					num15 = num;
				}
			}
			goto IL_0011;
			IL_0576:
			int num16;
			if (!(_206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E).x >= 1f))
			{
				num = -145177586;
				num16 = num;
			}
			else
			{
				num = -267432957;
				num16 = num;
			}
			goto IL_0016;
		}

		private static string _200C_206A_200F_200D_200C_206A_202C_200D_202C_202B_206D_200E_202B_200C_200C_206E_206F_200D_206D_200E_206F_206F_206D_202D_200D_206C_202B_202C_206F_202A_200F_206D_202D_202E_202D_200C_202A_206F_202E_206E_202E(TerrainData P_0, int P_1, int P_2, int P_3, int P_4)
		{
			string text = _200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)P_0);
			if (!_202D_202E_206C_202B_202A_200E_202A_200F_206C_202B_200D_202E_206B_206C_206B_202D_206C_202D_200B_206E_200F_200D_202A_202E_200C_200C_202B_200E_202B_202E_200C_202C_206C_200B_200D_206F_202D_202D_200F_202C_202E(text))
			{
				goto IL_000f;
			}
			goto IL_0066;
			IL_000f:
			int num = -558347989;
			goto IL_0014;
			IL_0014:
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0x80445F36u) % 6)
				{
				case 0u:
					break;
				case 1u:
				{
					int num3;
					int num4;
					if (!_202D_202E_206C_202B_202A_200E_202A_200F_206C_202B_200D_202E_206B_206C_206B_202D_206C_202D_200B_206E_200F_200D_202A_202E_200C_200C_202B_200E_202B_202E_200C_202C_206C_200B_200D_206F_202D_202D_200F_202C_202E(_202C_200F_200D_206C_206D_206B_200E_200B_206E_200C_202E_206D_200F_202D_202B_206B_200B_202A_200B_206E_200B_206B_200D_200F_202C_206B_206E_202E_202D_206A_206B_206E_200E_202D_206C_202A_200B_206C_206F_202A_202E(text)))
					{
						num3 = -1639241624;
						num4 = num3;
					}
					else
					{
						num3 = -1992191881;
						num4 = num3;
					}
					num = num3 ^ ((int)num2 * -128376875);
					continue;
				}
				case 2u:
					goto IL_0066;
				case 4u:
					text = _206A_202B_200D_202B_206E_200F_200C_202E_202C_200E_202E_200B_200C_200E_202A_200F_206D_206F_202C_206A_200B_206A_206B_206C_200D_202C_202E_200B_202A_200E_206F_206C_200D_202A_200E_202D_200E_202D_206B_206C_202E(text, _202D_202B_206A_200F_200F_202A_200E_200D_202E_200C_202E_202E_202D_206F_206D_200C_200B_202A_206A_206B_200C_200F_200D_206A_200E_206E_202C_200D_206A_202B_206E_206F_206F_200B_202A_200E_206F_202E_200C_200C_202E(" x[{0}] y[{1}]", (object)P_3, (object)P_4));
					num = ((int)num2 * -1850245742) ^ 0x28248C7B;
					continue;
				case 5u:
					goto IL_00b6;
				default:
					return text;
				}
				break;
				IL_00b6:
				int num5;
				if (P_1 * P_2 > 1)
				{
					num = -111743162;
					num5 = num;
				}
				else
				{
					num = -1795439717;
					num5 = num;
				}
			}
			goto IL_000f;
			IL_0066:
			text = _206A_202B_200D_202B_206E_200F_200C_202E_202C_200E_202E_200B_200C_200E_202A_200F_206D_206F_202C_206A_200B_206A_206B_206C_200D_202C_202E_200B_202A_200E_206F_206C_200D_202A_200E_202D_200E_202D_206B_206C_202E("TerrainData_", _206D_200E_206D_206B_202D_200C_200C_202C_202A_202A_200C_202B_202C_202A_206A_206E_206A_206D_202B_206E_206E_200C_202A_200D_206B_200C_202E_200C_200D_206A_206F_200E_200C_202C_206A_202D_202B_202B_206C_202B_202E((UnityEngine.Object)P_0).ToString());
			num = -926170807;
			goto IL_0014;
		}

		private static void _202D_206D_202A_202B_206C_206A_200C_206E_202A_206B_206D_206D_200D_200E_200B_206C_206A_200E_202E_206A_206D_200D_206A_202D_206D_206E_200B_206F_206B_202C_206C_202D_206A_206F_202E_200B_206C_200F_206D_202E_202E()
		{
			if (_200F_206E_200C_206F_200B_200E_206F_202A_200E_202D_200E_202E_206A_206D_202A_200D_200B_200E_206D_206E_206E_200B_200E_202D_206C_200E_206A_200F_206F_206C_200C_200B_206D_200F_206A_202E_206C_202A_206A_206B_202E != null)
			{
				goto IL_000a;
			}
			goto IL_0166;
			IL_000a:
			int num = -1476413680;
			goto IL_000f;
			IL_000f:
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0xCEC9E08Du) % 13)
				{
				case 9u:
					break;
				default:
					return;
				case 8u:
					goto IL_0058;
				case 5u:
					goto IL_0070;
				case 11u:
					_200F_206E_200C_206F_200B_200E_206F_202A_200E_202D_200E_202E_206A_206D_202A_200D_200B_200E_206D_206E_206E_200B_200E_202D_206C_200E_206A_200F_206F_206C_200C_200B_206D_200F_206A_202E_206C_202A_206A_206B_202E.Clear();
					num = (int)(num2 * 1699588997) ^ -838937841;
					continue;
				case 1u:
					_206A_206D_202E_202C_206F_202B_200F_206F_200C_202B_202D_202A_200E_202B_206F_202B_202C_206C_206E_206F_206B_202E_206F_206A_202B_200F_202A_202B_206D_200E_200F_200F_202B_206B_202C_200B_206D_200E_202C_206A_202E.Clear();
					num = (int)(num2 * 2120898492) ^ -1479082085;
					continue;
				case 12u:
					goto IL_00c0;
				case 0u:
					_202A_202E_202C_206A_206E_206F_202A_200D_206F_206E_202A_200F_200E_206B_200C_206A_200E_202D_200C_206D_202D_206E_202C_206A_200C_202E_200C_206F_200C_200F_202A_206E_206C_202E_202B_202D_200D_202E_206B_200E_202E.Clear();
					num = ((int)num2 * -1677002137) ^ 0x7A79F6FE;
					continue;
				case 6u:
					_200F_200E_200C_202E_206B_206A_202B_202A_202A_206F_206A_202C_206D_200B_200B_202C_206B_200D_200D_200C_200C_202E_206D_206F_200C_206F_200E_202D_200F_200E_202C_202E_206E_202D_200E_200E_200E_200C_200F_200E_202E.Clear();
					num = (int)(num2 * 419256649) ^ -1893490255;
					continue;
				case 3u:
					goto IL_0113;
				case 7u:
					_202D_206C_202B_202C_202B_206B_206F_202C_202D_202E_200D_200C_206B_206D_206F_200E_206B_202D_202E_200C_202C_200F_202C_200F_202C_202A_202B_200F_202D_202B_202D_200D_206F_206E_200D_202E_200B_206D_202D_200E_202E.Clear();
					num = ((int)num2 * -526390664) ^ -274428921;
					continue;
				case 2u:
					_202B_200E_202B_200B_202D_200C_202B_200E_202E_200B_206D_202C_206D_206D_206A_200B_206E_206A_200E_206F_206A_206F_200F_200F_202A_202D_206F_200D_206D_206A_202C_200F_202A_200E_202E_200E_200F_202B_200B_202B_202E.Clear();
					num = ((int)num2 * -1279822506) ^ 0x46DA8DDE;
					continue;
				case 4u:
					goto IL_0166;
				case 10u:
					return;
				}
				break;
				IL_0113:
				int num3;
				if (_202A_202E_202C_206A_206E_206F_202A_200D_206F_206E_202A_200F_200E_206B_200C_206A_200E_202D_200C_206D_202D_206E_202C_206A_200C_202E_200C_206F_200C_200F_202A_206E_206C_202E_202B_202D_200D_202E_206B_200E_202E == null)
				{
					num = -24140804;
					num3 = num;
				}
				else
				{
					num = -572062429;
					num3 = num;
				}
				continue;
				IL_0070:
				int num4;
				if (_206A_206D_202E_202C_206F_202B_200F_206F_200C_202B_202D_202A_200E_202B_206F_202B_202C_206C_206E_206F_206B_202E_206F_206A_202B_200F_202A_202B_206D_200E_200F_200F_202B_206B_202C_200B_206D_200E_202C_206A_202E != null)
				{
					num = -855418712;
					num4 = num;
				}
				else
				{
					num = -2062339657;
					num4 = num;
				}
				continue;
				IL_0058:
				int num5;
				if (_202D_206C_202B_202C_202B_206B_206F_202C_202D_202E_200D_200C_206B_206D_206F_200E_206B_202D_202E_200C_202C_200F_202C_200F_202C_202A_202B_200F_202D_202B_202D_200D_206F_206E_200D_202E_200B_206D_202D_200E_202E != null)
				{
					num = -1699345131;
					num5 = num;
				}
				else
				{
					num = -215432377;
					num5 = num;
				}
				continue;
				IL_00c0:
				int num6;
				if (_202B_200E_202B_200B_202D_200C_202B_200E_202E_200B_206D_202C_206D_206D_206A_200B_206E_206A_200E_206F_206A_206F_200F_200F_202A_202D_206F_200D_206D_206A_202C_200F_202A_200E_202E_200E_200F_202B_200B_202B_202E != null)
				{
					num = -910711833;
					num6 = num;
				}
				else
				{
					num = -824534462;
					num6 = num;
				}
			}
			goto IL_000a;
			IL_0166:
			int num7;
			if (_200F_200E_200C_202E_206B_206A_202B_202A_202A_206F_206A_202C_206D_200B_200B_202C_206B_200D_200D_200C_200C_202E_206D_206F_200C_206F_200E_202D_200F_200E_202C_202E_206E_202D_200E_200E_200E_200C_200F_200E_202E != null)
			{
				num = -888074999;
				num7 = num;
			}
			else
			{
				num = -1077854443;
				num7 = num;
			}
			goto IL_000f;
		}

		private static void _206F_206A_202E_202D_200E_200C_206F_202A_206E_202C_202D_200C_200C_200E_202E_206E_206E_200B_200B_200B_206E_202A_202E_202B_206D_202B_206B_200C_200B_202A_202B_206A_202B_206C_202E_202C_206E_202C_206F_206E_202E(UnityEngine.Object P_0)
		{
			if (_202C_202E_202B_202E_206E_206A_206E_206B_202C_200C_202A_200E_206B_206A_202A_206C_200E_206C_200B_200B_202C_200D_200B_206A_206D_206C_206B_200B_202B_200C_200D_202D_206D_206F_200C_206B_202C_206D_206F_202A_202E())
			{
				goto IL_0007;
			}
			goto IL_0043;
			IL_0007:
			int num = -452997779;
			goto IL_000c;
			IL_000c:
			uint num2;
			switch ((num2 = (uint)num ^ 0xA9598BF0u) % 4)
			{
			case 3u:
				break;
			default:
				return;
			case 1u:
				_202E_200D_202B_202C_206A_206B_206C_206D_206A_202E_206F_206B_206F_202B_200C_200E_202E_206D_202D_202A_200C_202C_200D_202A_206C_202E_206C_202E_206C_206B_200F_200D_206C_202A_202E_202A_202E_206C_200E_206C_202E(P_0);
				return;
			case 2u:
				goto IL_0043;
			case 0u:
				return;
			}
			goto IL_0007;
			IL_0043:
			_206F_200C_200C_202A_200D_202B_202E_206F_206A_206B_202C_202E_206A_200E_200E_206B_206D_206F_206D_206C_200B_206C_206C_206C_202B_202A_200F_200E_206A_202B_202E_202A_200B_202C_200B_206B_202E_206C_202C_206D_202E(P_0);
			num = -1515631720;
			goto IL_000c;
		}

		public TerrainLayer[] ExportTerrainLayers()
		{
			return _200F_202C_202D_202A_206A_200B_200B_202C_200E_200C_200F_206F_200D_200C_206C_206E_200E_206F_206B_206B_202C_200F_206B_200F_200C_206F_200D_202D_206E_206C_202C_200F_202B_202A_200C_202A_202A_206C_200D_206F_202E._206D_202C_200D_206F_200B_206C_202E_206B_202C_206E_200B_200F_202B_202B_206A_202A_202A_202D_200D_202C_202D_206C_202D_200B_202A_200D_206D_206D_206C_206B_200D_200C_206D_206D_202A_200B_202D_206D_206F_202E_202E._200E_206B_206C_206E_206F_206C_202B_206D_200F_206B_202E_202D_202D_202C_200C_206B_202D_202E_206B_206F_200C_202E_202D_200C_202E_200B_200F_200E_202B_206B_200D_206C_206A_206A_202E_200E_200B_202D_202A_206F_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E);
		}

		public Texture2D[] ExportSplatmapTextures(int resolution, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._206F_206D_202C_200C_206F_206F_200B_206A_202A_200C_206C_202E_206A_202B_200E_200F_206E_206D_200E_200C_206F_202A_200E_200E_200D_206D_202A_206D_200B_202E_206F_202D_202E_202A_206A_202E_206C_202A_202E_202D_202E._200D_202D_206A_202D_206E_206E_206E_200F_206C_206C_202A_200F_200F_202A_206A_202E_206F_206F_206C_202E_206C_200C_206B_202E_206A_200F_206B_200C_200F_200D_200E_202B_200F_200C_206C_206D_202C_206F_202A_200E_202E(resolution);
		}

		public Texture2D ExportHolesmapTexture(int resolution, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._206F_206D_202C_200C_206F_206F_200B_206A_202A_200C_206C_202E_206A_202B_200E_200F_206E_206D_200E_200C_206F_202A_200E_200E_200D_206D_202A_206D_200B_202E_206F_202D_202E_202A_206A_202E_206C_202A_202E_202D_202E._200D_202C_206B_200E_202B_200B_202D_206C_206F_202C_202D_206A_202E_200B_202B_202E_202A_200F_206A_202A_200C_206A_202D_200C_206B_202B_206C_206C_200B_206C_200F_202E_200F_200E_200F_206E_200E_206D_200F_200E_202E(resolution);
		}

		public Texture2D ExportHolesmapTexture(int resolution, int chunkCountHorizontal, int chunkCountVertical, int positionX, int positionY, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._206F_206D_202C_200C_206F_206F_200B_206A_202A_200C_206C_202E_206A_202B_200E_200F_206E_206D_200E_200C_206F_202A_200E_200E_200D_206D_202A_206D_200B_202E_206F_202D_202E_202A_206A_202E_206C_202A_202E_202D_202E._200C_202C_206D_200E_206F_206A_202D_206A_206E_202C_202B_206D_200C_206F_202C_200C_200F_200C_202D_200F_200B_206B_200F_206B_202A_200D_200E_202C_202B_200E_200E_202B_206E_200B_200E_206C_202A_200F_200D_206A_202E(resolution, chunkCountHorizontal, chunkCountVertical, positionX, positionY);
		}

		public Texture2D[] ExportHolesmapTexture(int resolution, int chunkCountHorizontal, int chunkCountVertical, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._206F_206D_202C_200C_206F_206F_200B_206A_202A_200C_206C_202E_206A_202B_200E_200F_206E_206D_200E_200C_206F_202A_200E_200E_200D_206D_202A_206D_200B_202E_206F_202D_202E_202A_206A_202E_206C_202A_202E_202D_202E._200E_200F_206F_202D_202E_200D_202B_202D_206D_206B_200B_202D_200E_202A_200B_200B_206D_200E_200B_200C_202E_202C_202D_202A_202C_202E_200E_206A_206D_202E_206F_200D_200E_206C_200C_202C_202B_202C_202C_202B_202E(resolution, chunkCountHorizontal, chunkCountVertical);
		}

		public Texture2D ExportBasemapDiffuseTexture(int resolution, bool includeHolesmap, bool unpack)
		{
			if (includeHolesmap)
			{
				while (true)
				{
					uint num;
					switch ((num = 0xFEC6099Fu ^ 0xB91A6C67u) % 3)
					{
					case 0u:
						continue;
					case 1u:
					{
						Texture2D texture2D = _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._202D_206A_202E_206A_206A_202C_202C_202E_202D_200E_200D_202C_206F_200F_206A_200E_200C_206D_202A_206C_202E_206E_202C_202B_206D_206D_206F_200B_202C_200F_200D_200E_202A_200E_202C_200B_206B_202E_202C_206C_202E(resolution);
						_206A_200E_202E_200E_202A_202A_202E_202D_200E_202E_206F_200E_206F_202A_206C_202E_206C_206E_202E_202E_206F_206B_206A_206F_202C_202C_202E_200C_202D_200F_200C_206D_206C_200D_202E_202C_200E_206E_202C_200F_202E(texture2D, _206E_206A_200C_206A_202C_200E_202E_200B_200C_200D_202A_202E_200B_202C_200B_200C_200F_202D_206A_206C_200F_206B_200F_202E_202A_206F_202C_200F_200D_202E_206C_202A_206F_206A_206A_206B_200C_202C_206D_200F_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E));
						return texture2D;
					}
					}
					break;
				}
			}
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._202D_206A_202E_206A_206A_202C_202C_202E_202D_200E_200D_202C_206F_200F_206A_200E_200C_206D_202A_206C_202E_206E_202C_202B_206D_206D_206F_200B_202C_200F_200D_200E_202A_200E_202C_200B_206B_202E_202C_206C_202E(resolution);
		}

		public Texture2D ExportBasemapDiffuseTexture(int resolution, int chunkCountHorizontal, int chunkCountVertical, int positionX, int positionY, bool includeHolesmap, bool unpack)
		{
			if (includeHolesmap)
			{
				while (true)
				{
					uint num;
					switch ((num = 0xDA229E60u ^ 0xB40157C2u) % 3)
					{
					case 2u:
						continue;
					case 1u:
					{
						Texture2D texture2D = _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._200C_200E_202A_206B_202C_200E_202A_206B_206C_200F_206A_202A_202D_206B_206C_200D_202D_206D_206A_206D_202E_206F_206E_200F_200C_202E_200D_206A_200C_202B_206B_206B_206E_202D_202A_200F_202D_202E_206D_200D_202E(resolution, chunkCountHorizontal, chunkCountVertical, positionX, positionY);
						Texture2D texture2D2 = ExportHolesmapTexture(resolution, chunkCountHorizontal, chunkCountVertical, positionX, positionY, unpack);
						_206A_200E_202E_200E_202A_202A_202E_202D_200E_202E_206F_200E_206F_202A_206C_202E_206C_206E_202E_202E_206F_206B_206A_206F_202C_202C_202E_200C_202D_200F_200C_206D_206C_200D_202E_202C_200E_206E_202C_200F_202E(texture2D, texture2D2);
						return texture2D;
					}
					}
					break;
				}
			}
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._200C_200E_202A_206B_202C_200E_202A_206B_206C_200F_206A_202A_202D_206B_206C_200D_202D_206D_206A_206D_202E_206F_206E_200F_200C_202E_200D_206A_200C_202B_206B_206B_206E_202D_202A_200F_202D_202E_206D_200D_202E(resolution, chunkCountHorizontal, chunkCountVertical, positionX, positionY);
		}

		public Texture2D[] ExportBasemapDiffuseTexture(int resolution, int chunkCountHorizontal, int chunkCountVertical, bool includeHolesmap, bool unpack)
		{
			if (includeHolesmap)
			{
				Texture2D[] array = default(Texture2D[]);
				int num3 = default(int);
				Texture2D[] array2 = default(Texture2D[]);
				while (true)
				{
					int num = -1654295693;
					while (true)
					{
						uint num2;
						switch ((num2 = (uint)num ^ 0xEBE9F881u) % 12)
						{
						case 10u:
							break;
						case 2u:
						{
							int num4;
							int num5;
							if (array.Length == 0)
							{
								num4 = 776124767;
								num5 = num4;
							}
							else
							{
								num4 = 849603134;
								num5 = num4;
							}
							num = num4 ^ (int)(num2 * 2044227535);
							continue;
						}
						case 8u:
							num3 = 0;
							num = ((int)num2 * -1757970736) ^ -1357035243;
							continue;
						case 11u:
							num3++;
							num = ((int)num2 * -1130843960) ^ -1759298644;
							continue;
						case 3u:
						{
							int num6;
							int num7;
							if (array == null)
							{
								num6 = 1797321307;
								num7 = num6;
							}
							else
							{
								num6 = 1650739993;
								num7 = num6;
							}
							num = num6 ^ ((int)num2 * -746580542);
							continue;
						}
						case 1u:
							array2 = _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._206F_206D_202C_200C_206F_206F_200B_206A_202A_200C_206C_202E_206A_202B_200E_200F_206E_206D_200E_200C_206F_202A_200E_200E_200D_206D_202A_206D_200B_202E_206F_202D_202E_202A_206A_202E_206C_202A_202E_202D_202E._200E_200F_206F_202D_202E_200D_202B_202D_206D_206B_200B_202D_200E_202A_200B_200B_206D_200E_200B_200C_202E_202C_202D_202A_202C_202E_200E_206A_206D_202E_206F_200D_200E_206C_200C_202C_202B_202C_202C_202B_202E(resolution, chunkCountHorizontal, chunkCountVertical);
							num = ((int)num2 * -1315867539) ^ -1276475420;
							continue;
						case 9u:
							goto IL_00e2;
						case 6u:
							array = _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._200B_206D_202C_202E_200D_200F_200C_206F_206A_206B_200D_206F_200F_200C_206F_200F_200B_202C_200C_206F_202B_202D_206F_206C_200D_206B_202E_200D_206E_202E_206D_206A_206E_202E_202A_202D_206F_202E_202E_202B_202E(resolution, chunkCountHorizontal, chunkCountVertical);
							num = ((int)num2 * -1363049893) ^ 0x4BF6661C;
							continue;
						case 5u:
							_206A_200E_202E_200E_202A_202A_202E_202D_200E_202E_206F_200E_206F_202A_206C_202E_206C_206E_202E_202E_206F_206B_206A_206F_202C_202C_202E_200C_202D_200F_200C_206D_206C_200D_202E_202C_200E_206E_202C_200F_202E(array[num3], _202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)array2[num3], (UnityEngine.Object)null) ? _202C_202D_206D_200E_200B_200F_200F_202A_202B_206B_202B_200D_202E_202A_206C_206A_200B_202E_206B_206F_200D_206F_202C_206A_206B_200C_206B_206A_202D_206D_206C_206B_206A_202B_200C_206B_206B_206E_202D_202E() : array2[num3]);
							_206F_206A_202E_202D_200E_200C_206F_202A_206E_202C_202D_200C_200C_200E_202E_206E_206E_200B_200B_200B_206E_202A_202E_202B_206D_202B_206B_200C_200B_202A_202B_206A_202B_206C_202E_202C_206E_202C_206F_206E_202E(array2[num3]);
							num = -2112701978;
							continue;
						case 4u:
							num = ((int)num2 * -1225465866) ^ 0x1E3B0B6C;
							continue;
						case 0u:
							return array;
						default:
							goto end_IL_0007;
						}
						break;
						IL_00e2:
						int num8;
						if (num3 < array.Length)
						{
							num = -2128661500;
							num8 = num;
						}
						else
						{
							num = -1403859;
							num8 = num;
						}
					}
					continue;
					end_IL_0007:
					break;
				}
			}
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._200B_206D_202C_202E_200D_200F_200C_206F_206A_206B_200D_206F_200F_200C_206F_200F_200B_202C_200C_206F_202B_202D_206F_206C_200D_206B_202E_200D_206E_202E_206D_206A_206E_202E_202A_202D_206F_202E_202E_202B_202E(resolution, chunkCountHorizontal, chunkCountVertical);
		}

		public Texture2D ExportBasemapNormalTexture(int resolution, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._206E_206A_206A_200D_200E_206B_200F_202E_200B_200F_202A_200F_206C_206B_206A_206B_202E_206F_200F_206F_206B_206F_202C_206F_206D_206A_200F_202D_200E_202D_200B_202C_202D_200F_202E_206E_200C_200F_202C_200F_202E(resolution);
		}

		public Texture2D ExportBasemapNormalTexture(int resolution, int chunkCountHorizontal, int chunkCountVertical, int positionX, int positionY, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._200F_202C_200F_200C_202D_206F_202E_202C_202E_206F_202C_202A_206D_202E_200D_202B_202D_206D_206D_200B_200C_206A_200F_206E_200B_200D_206F_206F_206F_202C_200E_200C_206E_202A_206C_206C_200F_202C_200C_200D_202E(resolution, chunkCountHorizontal, chunkCountVertical, positionX, positionY);
		}

		public Texture2D[] ExportBasemapNormalTexture(int resolution, int chunkCountHorizontal, int chunkCountVertical, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._206E_206B_202A_206A_202E_202D_206F_202E_206F_202B_206E_202B_206C_206F_200E_206E_206F_202D_206C_206F_206D_202B_206D_202B_200B_200E_200F_206B_206F_206F_206D_202D_200F_200D_200C_206E_202C_202C_206E_200C_202E(resolution, chunkCountHorizontal, chunkCountVertical);
		}

		public Texture2D ExportBasemapMaskTexture(int resolution, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._200F_206E_206A_200C_206D_206E_206E_206B_200D_200D_200F_200F_200C_200D_200C_200B_200D_206D_200F_200F_206C_200F_206D_202A_206B_206A_206A_202C_202D_206F_206C_206B_200C_200C_200D_200C_206D_206C_202E_202C_202E(resolution);
		}

		public Texture2D ExportBasemapMaskTexture(int resolution, int chunkCountHorizontal, int chunkCountVertical, int positionX, int positionY, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._206D_206D_206A_206B_200B_202E_200C_202A_206A_206D_202E_206D_206E_200E_200C_202C_202B_200D_206E_206A_200E_206F_200F_206D_202D_206E_202E_202B_202D_200E_206A_206D_202B_206D_206D_202C_200C_200C_200E_200D_202E(resolution, chunkCountHorizontal, chunkCountVertical, positionX, positionY);
		}

		public Texture2D[] ExportBasemapMaskTexture(int resolution, int chunkCountHorizontal, int chunkCountVertical, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._200F_202D_202A_206F_200D_200C_200D_202C_200B_202A_202A_206B_206D_206B_206B_206F_202B_206B_206A_206B_206A_206D_206B_206A_206B_206B_200B_206B_202C_200B_200D_206F_202C_202B_200D_206B_206E_200E_202A_206E_202E(resolution, chunkCountHorizontal, chunkCountVertical);
		}

		public Texture2D ExportBasemapOcclusionTexture(int resolution, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._200F_200C_202A_206E_206B_206B_200D_202E_206B_206E_200B_206A_200B_206D_200B_202D_206A_200F_202E_202E_202D_206F_200E_206F_200E_206F_200F_206B_206F_206C_206C_206E_206D_200D_206C_200C_202B_206C_206C_206E_202E(resolution);
		}

		public Texture2D ExportBasemapOcclusionTexture(int resolution, int chunkCountHorizontal, int chunkCountVertical, int positionX, int positionY, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._206F_200B_200E_206A_200E_200B_202C_200E_206C_202E_200F_202A_206B_206D_200C_200C_206C_206A_200E_206C_202C_200E_202C_200B_200C_206F_206A_200B_206D_202C_200F_206F_200D_200F_200C_206C_200D_200F_206E_206A_202E(resolution, chunkCountHorizontal, chunkCountVertical, positionX, positionY);
		}

		public Texture2D[] ExportBasemapOcclusionTexture(int resolution, int chunkCountHorizontal, int chunkCountVertical, bool unpack)
		{
			return _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E._206F_202B_200F_206E_206E_202C_202C_206D_206B_206D_206C_206F_202D_200D_206B_200D_202A_206C_200C_206C_206B_200C_200F_202A_202B_200F_202C_206D_200B_202E_202D_206B_206A_200B_200F_206A_200E_200E_200E_202E(false, unpack)._200D_202B_202A_206D_206C_206B_200C_200F_200F_206D_202E_200F_200F_200B_206F_206E_200D_206B_202C_202A_200E_200D_202D_200F_206E_206A_206D_206B_202B_206E_200D_202B_206C_206F_202C_206F_200B_206F_202A_202A_202E._202D_200D_200C_200B_206D_206C_206D_206A_202C_200F_206D_200E_200E_206D_206A_202B_200E_206D_206B_206C_202C_206F_200D_206B_202B_202A_206A_202D_206D_202C_200F_206F_202A_200C_200C_202A_202E_200B_206D_206E_202E(resolution, chunkCountHorizontal, chunkCountVertical);
		}

		private static void _206A_200E_202E_200E_202A_202A_202E_202D_200E_202E_206F_200E_206F_202A_206C_202E_206C_206E_202E_202E_206F_206B_206A_206F_202C_202C_202E_200C_202D_200F_200C_206D_206C_200D_202E_202C_200E_206E_202C_200F_202E(Texture2D P_0, Texture P_1)
		{
			if (!_206B_206A_202B_206E_200C_200F_202B_206B_200C_200C_206A_202C_206E_206B_202C_200D_206D_206C_206A_206F_206B_202B_206A_200B_206C_202E_200D_202C_206D_206C_202D_206B_200E_200D_202D_206B_200B_206A_200D_200E_202E((UnityEngine.Object)P_0, (UnityEngine.Object)null))
			{
				return;
			}
			RenderTexture renderTexture2 = default(RenderTexture);
			Material material = default(Material);
			RenderTexture renderTexture = default(RenderTexture);
			Shader shader = default(Shader);
			while (true)
			{
				int num = -1327387717;
				while (true)
				{
					uint num2;
					object obj;
					switch ((num2 = (uint)num ^ 0x92FC8276u) % 17)
					{
					case 2u:
						break;
					default:
						return;
					case 13u:
						if (!_206E_202A_200C_200B_206D_200D_206D_206B_206B_202C_202E_200C_206A_202E_206E_206F_206A_206C_200C_200B_200C_200F_206A_206F_202D_206C_206B_200D_200C_202D_202B_206F_202B_200B_202E_200E_206C_200B_206E_200C_202E())
						{
							num = (int)(num2 * 721733916) ^ -273891602;
							continue;
						}
						obj = renderTexture2;
						goto IL_0086;
					case 0u:
						obj = null;
						goto IL_0086;
					case 3u:
					{
						int num5;
						int num6;
						if (_206B_206A_202B_206E_200C_200F_202B_206B_200C_200C_206A_202C_206E_206B_202C_200D_206D_206C_206A_206F_206B_202B_206A_200B_206C_202E_200D_202C_206D_206C_202D_206B_200E_200D_202D_206B_200B_206A_200D_200E_202E((UnityEngine.Object)P_1, (UnityEngine.Object)null))
						{
							num5 = 596499197;
							num6 = num5;
						}
						else
						{
							num5 = 1670873838;
							num6 = num5;
						}
						num = num5 ^ (int)(num2 * 1893144261);
						continue;
					}
					case 14u:
						_202B_202E_206D_200E_202E_200E_200D_202B_200E_206E_206E_200B_202C_200E_206E_200C_206F_202C_200F_206D_200C_202E_206B_206B_202C_206F_202E_202D_202C_206F_200D_202E_200E_202C_202B_206A_200E_206C_200D_202B_202E(material, "_ControlMap", P_1);
						num = (int)((num2 * 242422123) ^ 0x320D9FE1);
						continue;
					case 5u:
						renderTexture2 = _206C_206B_202B_206D_202D_202D_206A_202B_202A_200B_202C_206B_202D_206F_200E_206F_206D_206D_206C_206E_206E_206A_206E_206C_200B_206C_202C_206A_206B_200B_202E_202D_206F_200F_202B_200E_200E_200C_206E_202B_202E();
						num = ((int)num2 * -2126316208) ^ -700954904;
						continue;
					case 6u:
						_202B_200C_200B_202A_200C_206B_206A_206C_202B_200F_202E_206B_202E_202A_200D_206E_200C_200D_206E_206B_206B_202E_206D_200D_202C_206F_206A_202A_206B_200C_202B_202C_206B_200B_206E_200F_206E_206D_200B_200D_202E((Texture)P_0, renderTexture, material, 12);
						num = ((int)num2 * -1624822909) ^ 0x4F2D257C;
						continue;
					case 16u:
						material = _202B_206D_206E_200D_202C_202C_202E_200D_200D_200F_206A_206B_206F_206B_206F_202D_202A_206B_202A_202A_206D_200B_200F_206F_202B_206C_200E_200F_202D_200B_200E_200E_202E_200D_202E_202A_200C_206E_200C_200D_202E(shader);
						num = -1985664088;
						continue;
					case 7u:
						_206F_206A_202E_202D_200E_200C_206F_202A_206E_202C_202D_200C_200C_200E_202E_206E_206E_200B_200B_200B_206E_202A_202E_202B_206D_202B_206B_200C_200B_202A_202B_206A_202B_206C_202E_202C_206E_202C_206F_206E_202E(material);
						num = (int)((num2 * 352439307) ^ 0x135DA2D5);
						continue;
					case 9u:
						return;
					case 11u:
						_202A_202D_206F_206B_200F_202A_202C_200D_202A_200D_206E_202E_202A_200D_202A_202D_202C_206E_200F_200B_206D_200E_200E_202A_200E_202D_200D_202C_200F_202B_206C_202B_206B_206E_200F_200E_200D_206C_206F_206E_202E(P_0, new Rect(0f, 0f, _200D_206F_206C_206B_202E_206D_206F_200B_200C_206B_206B_206A_206A_202E_200D_206D_206B_200E_206C_206C_200F_202E_202A_200C_200C_200E_202D_202E_200E_202C_202D_206D_206E_206D_202B_200D_200D_206D_202A_202E((Texture)P_0), _200D_202D_200C_202D_202E_202C_206A_200B_206C_202D_202C_200F_206F_200C_206B_206C_206E_200B_206D_200B_200B_206A_202B_206A_202E_206E_200B_200D_202B_206F_202B_202E_202D_206F_202D_202A_206C_206A_202C_202A_202E((Texture)P_0)), 0, 0);
						_200B_206C_202C_200D_200D_200C_202D_202E_206C_202B_200B_206D_200C_202C_206F_202C_202E_206D_206D_200E_206A_200E_206D_200F_206E_206E_206A_202B_206E_206D_202B_202B_200F_200F_206B_200D_202B_202D_200F_202E_202E(P_0);
						num = ((int)num2 * -391269442) ^ 0x698CCB7B;
						continue;
					case 1u:
						_200F_202D_206C_202B_200E_206C_206D_206A_202E_206E_200C_200E_202E_206D_202A_202C_206F_202A_206C_200C_202B_200D_202A_202E_206A_206B_206D_206F_206E_206E_206D_200F_200F_206F_206B_200F_200F_200D_200C_200D_202E(renderTexture);
						num = (int)(num2 * 998664006) ^ -1888232915;
						continue;
					case 4u:
						shader = _200B_200F_206A_200E_206A_202B_202D_206E_206D_200D_200F_206C_202E_200E_202E_206A_200C_200F_202E_206F_200B_206B_202B_206E_206A_200B_206D_206E_202D_200C_200C_206A_202B_200B_200E_202E_206D_200B_206C_202E(Constants.shaderAllTerrainTexture);
						num = ((int)num2 * -6165460) ^ -2112990536;
						continue;
					case 12u:
					{
						int num3;
						int num4;
						if (!_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)shader, (UnityEngine.Object)null))
						{
							num3 = -1052581246;
							num4 = num3;
						}
						else
						{
							num3 = -146318213;
							num4 = num3;
						}
						num = num3 ^ (int)(num2 * 1577548141);
						continue;
					}
					case 10u:
						Utilities.Debug(LogType.Warning, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("Required shader '", Constants.shaderAllTerrainTexture, "' is missing."), null);
						num = ((int)num2 * -1409337750) ^ 0x505028FE;
						continue;
					case 15u:
						renderTexture = _202B_206A_200E_200E_200F_202B_200C_202B_206A_206B_200E_200D_200B_202A_200B_200D_202E_202D_200B_206C_202C_202D_202E_200F_206B_206A_200E_206C_206C_202B_202E_206D_202E_202B_200E_202C_200D_202C_206E_202C_202E(_200D_206F_206C_206B_202E_206D_206F_200B_200C_206B_206B_206A_206A_202E_200D_206D_206B_200E_206C_206C_200F_202E_202A_200C_200C_200E_202D_202E_200E_202C_202D_206D_206E_206D_202B_200D_200D_206D_202A_202E((Texture)P_0), _200D_202D_200C_202D_202E_202C_206A_200B_206C_202D_202C_200F_206F_200C_206B_206C_206E_200B_206D_200B_200B_206A_202B_206A_202E_206E_200B_200D_202B_206F_202B_202E_202D_206F_202D_202A_206C_206A_202C_202A_202E((Texture)P_0));
						_206E_206F_200B_202A_200C_202D_200D_206E_206C_200F_202D_206E_206D_206C_202B_200C_202B_200F_202E_202B_206D_206D_202B_206D_200F_202C_202B_202D_202D_206B_202A_200D_202B_202E_206C_202B_206F_202E_206D_200D_202E(renderTexture);
						num = ((int)num2 * -1120142425) ^ -1591339475;
						continue;
					case 8u:
						return;
						IL_0086:
						_200F_202D_206C_202B_200E_206C_206D_206A_202E_206E_200C_200E_202E_206D_202A_202C_206F_202A_206C_200C_202B_200D_202A_202E_206A_206B_206D_206F_206E_206E_206D_200F_200F_206F_206B_200F_200F_200D_200C_200D_202E((RenderTexture)obj);
						_206E_202D_206B_202E_202B_202B_206E_202A_200D_206E_202B_206E_200D_202B_206F_200B_200C_202E_202D_206E_200B_200D_202E_206F_206A_202C_200D_202E_206F_206E_206F_206E_200D_206E_206E_206C_202D_206A_202E(renderTexture);
						num = -467493032;
						continue;
					}
					break;
				}
			}
		}

		public Material ExportSplatmapMaterial(bool hasHolesmap)
		{
			if (_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, (UnityEngine.Object)null))
			{
				goto IL_0011;
			}
			goto IL_0810;
			IL_0011:
			int num = -1785055251;
			goto IL_0016;
			IL_0016:
			int num3 = default(int);
			int num6 = default(int);
			Material material = default(Material);
			TerrainLayer[] array = default(TerrainLayer[]);
			bool flag = default(bool);
			Vector4 vector = default(Vector4);
			Shader shader = default(Shader);
			int num12 = default(int);
			while (true)
			{
				uint num2;
				int num11;
				switch ((num2 = (uint)num ^ 0xE760EF92u) % 45)
				{
				case 44u:
					break;
				case 16u:
					num = (int)(num2 * 365372066) ^ -1312867620;
					continue;
				case 27u:
					num3 = 0;
					num = ((int)num2 * -1175454532) ^ 0xDF70577;
					continue;
				case 21u:
					num6++;
					num = -1549472862;
					continue;
				case 0u:
					num = (int)((num2 * 829388434) ^ 0x23618427);
					continue;
				case 25u:
					num6 = 0;
					num = (int)(num2 * 2131057388) ^ -1902505467;
					continue;
				case 20u:
					return material;
				case 8u:
					Utilities.Debug(LogType.Warning, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[5]
					{
						"'",
						_200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E),
						"' uses ",
						array.Length.ToString(),
						" layers. Only first 16 will be used in Splatmap shader."
					}), null, _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E);
					num = (int)((num2 * 26310734) ^ 0x59929EED);
					continue;
				case 29u:
					goto IL_01ae;
				case 28u:
					_202B_202E_206D_200E_202E_200E_200D_202B_200E_206E_206E_200B_202C_200E_206E_200C_206F_202C_200F_206D_200C_202E_206B_206B_202C_206F_202E_202D_202C_206F_200D_202E_200E_202C_202B_206A_200E_206C_200D_202B_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num6.ToString(), "_NormalMap"), (Texture)_200E_206A_206D_202C_200B_206E_200F_206A_206F_202E_206F_206D_200F_202A_200E_202C_206B_202E_200F_206D_202D_206D_202C_202B_200B_206F_200B_200B_200E_202B_202B_200B_200F_202E_202A_202C_206B_206F_206D_200C_202E(array[num6]));
					num = (int)((num2 * 1197973256) ^ 0x7735B2C3);
					continue;
				case 38u:
					goto IL_0204;
				case 1u:
					goto IL_024e;
				case 13u:
				{
					int num15;
					int num16;
					if (_200D_200B_202E_200C_202D_200F_202A_200D_206B_200E_206C_206C_206E_200B_202E_202D_202D_200D_202E_202E_200C_206E_200E_202A_200D_200E_200E_200C_206D_206B_206B_206B_206A_206A_200E_202A_206D_200F_202E_202B_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E).Length != 0)
					{
						num15 = 291734800;
						num16 = num15;
					}
					else
					{
						num15 = 1793140104;
						num16 = num15;
					}
					num = num15 ^ ((int)num2 * -1157333860);
					continue;
				}
				case 34u:
					return null;
				case 12u:
					goto IL_02a0;
				case 31u:
					return _202B_206D_206E_200D_202C_202C_202E_200D_200D_200F_206A_206B_206F_206B_206F_202D_202A_206B_202A_202A_206D_200B_200F_206F_202B_206C_200E_200F_202D_200B_200E_200E_202E_200D_202E_202A_200C_206E_200C_200D_202E(_200B_200F_206A_200E_206A_202B_202D_206E_206D_200D_200F_206C_202E_200E_202E_206A_200C_200F_202E_206F_200B_206B_202B_206E_206A_200B_206D_206E_202D_200C_200C_206A_202B_200B_200E_202E_206D_200B_206C_202E(Utilities.GetUnityDefaultShader()));
				case 15u:
					Utilities.SetupAlphaTestProperties(material);
					num = (int)((num2 * 917001393) ^ 0x51157DCD);
					continue;
				case 30u:
					if (_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)_206D_202D_202E_206C_206F_206F_200D_206A_202E_200B_206F_206D_202C_206B_200B_200D_206A_206A_200E_202A_202A_200D_202E_202A_200B_200E_202C_206D_206B_202A_206E_202C_200B_200F_200C_200C_202C_200B_200E_202D_202E(array[num6]), (UnityEngine.Object)null))
					{
						num = (int)(num2 * 197128676) ^ -1974295190;
						continue;
					}
					num11 = 0;
					goto IL_04ec;
				case 11u:
					_202A_200E_200C_206C_200F_206E_202C_200E_206C_200D_206E_206F_202D_206E_206B_202C_206B_200D_206F_202A_202D_206A_206E_200F_200F_206A_200F_202D_206F_202B_206C_200C_200D_202B_202B_202E_206F_200D_202D_200F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num6.ToString(), "_SmoothnessFromDiffuseAlpha"), (float)(flag ? 1 : 0));
					_202B_202C_200E_200B_206F_202B_200E_202C_206C_200B_200D_202C_202D_206B_202E_202D_200D_200D_206A_206B_206F_206A_206D_200D_206A_206D_206D_206E_206A_200B_200D_206D_206D_200C_202C_206B_202A_202B_200E_206F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num6.ToString(), "_MaskMapRemapMin"), _202B_206A_206F_206E_202C_206E_200B_206C_202C_202A_200C_206D_206E_200B_202B_206B_206B_206A_202E_202E_200D_206E_200C_202C_200E_200F_202E_202E_202E_206A_206E_200C_206E_206B_200E_202B_200D_202E_202B_202A_202E(array[num6]));
					_202B_202C_200E_200B_206F_202B_200E_202C_206C_200B_200D_202C_202D_206B_202E_202D_200D_200D_206A_206B_206F_206A_206D_200D_206A_206D_206D_206E_206A_200B_200D_206D_206D_200C_202C_206B_202A_202B_200E_206F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num6.ToString(), "_MaskMapRemapMax"), _202E_200E_202C_202B_200E_206B_206D_202C_200B_202A_206F_202B_206C_200F_206B_202D_200C_200D_206C_200F_200F_206A_206B_202D_200D_200E_200F_206A_200C_202E_200E_202A_206F_202A_206C_206C_200C_200F_200E_202C_202E(array[num6]));
					num = -544795385;
					continue;
				case 39u:
					goto IL_03a8;
				case 5u:
					_202B_202E_206D_200E_202E_200E_200D_202B_200E_206E_206E_200B_202C_200E_206E_200C_206F_202C_200F_206D_200C_202E_206B_206B_202C_206F_202E_202D_202C_206F_200D_202E_200E_202C_202B_206A_200E_206C_200D_202B_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num6.ToString(), "_Diffuse"), (Texture)_202B_200E_206D_206D_200D_202A_206B_200E_202A_206A_206E_200D_202B_206B_202B_206F_206F_206B_200E_200D_202A_206E_200C_200E_200B_200B_202C_200F_202D_202B_200E_202D_206A_206F_206C_206D_206F_206C_206A_206F_202E(array[num6]));
					num = ((int)num2 * -1116899687) ^ 0x5799FC72;
					continue;
				case 18u:
				{
					int num17;
					int num18;
					if (!_206B_206A_202B_206E_200C_200F_202B_206B_200C_200C_206A_202C_206E_206B_202C_200D_206D_206C_206A_206F_206B_202B_206A_200B_206C_202E_200D_202C_206D_206C_202D_206B_200E_200D_202D_206B_200B_206A_200D_200E_202E((UnityEngine.Object)_206E_206B_206E_206B_206A_202E_206E_202C_206D_202E_206C_206C_202C_200D_206E_206F_206B_200F_202B_202C_200E_206E_206A_200D_206A_206E_202A_200C_206B_202D_202C_202A_202B_202C_200D_206D_202C_206E_206A_202C_202E(), (UnityEngine.Object)null))
					{
						num17 = -998886059;
						num18 = num17;
					}
					else
					{
						num17 = -649212517;
						num18 = num17;
					}
					num = num17 ^ (int)(num2 * 798121311);
					continue;
				}
				case 14u:
				{
					vector = _200D_200C_202B_202E_202D_200E_206D_206F_202B_206C_202B_206E_202C_200B_200F_200E_206A_200D_206F_202B_206A_206A_200B_200D_202D_202B_200E_202A_206F_206E_200C_202D_206E_206F_206A_202D_206F_200D_206E_200C_202E(array[num6]);
					int num13;
					int num14;
					if (_206E_200F_200D_206C_200D_200C_206B_200C_206F_202D_206D_200C_206F_206D_202C_206E_200F_206D_206D_200F_206E_206C_200B_206F_202B_206B_206D_200F_206F_206A_206A_200C_200B_206D_206D_206D_202B_200E_202E_206D_202E() != ColorSpace.Linear)
					{
						num13 = 913693222;
						num14 = num13;
					}
					else
					{
						num13 = 623217347;
						num14 = num13;
					}
					num = num13 ^ ((int)num2 * -1713030452);
					continue;
				}
				case 2u:
					return null;
				case 36u:
					Utilities.Debug(LogType.Warning, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("Shader '", hasHolesmap ? Constants.shaderSplatmapWithHoles : Constants.shaderSplatmap, "' is not installed"), null);
					num = -539055530;
					continue;
				case 4u:
					_206F_206E_200E_202E_200F_200B_206B_200B_200E_206D_202B_202B_206F_200B_206C_202D_202E_202E_202B_200C_200B_206E_200E_206A_200D_200C_200B_206E_206B_206C_202D_202A_206A_206C_206B_202E_202D_206C_206B_202C_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num6.ToString(), "_ColorTint"), new Color(vector.x, vector.y, vector.z, 1f));
					num = -1393763464;
					continue;
				case 19u:
					num11 = (Utilities.HasTextureAlphaChannel(_202B_200E_206D_206D_200D_202A_206B_200E_202A_206A_206E_200D_202B_206B_202B_206F_206F_206B_200E_200D_202A_206E_200C_200E_200B_200B_202C_200F_202D_202B_200E_202D_206A_206F_206C_206D_206F_206C_206A_206F_202E(array[num6])) ? 1 : 0);
					goto IL_04ec;
				case 10u:
					material = _202B_206D_206E_200D_202C_202C_202E_200D_200D_200F_206A_206B_206F_206B_206F_202D_202A_206B_202A_202A_206D_200B_200F_206F_202B_206C_200E_200F_202D_200B_200E_200E_202E_200D_202E_202A_200C_206E_200C_200D_202E(shader);
					num = -1017115541;
					continue;
				case 42u:
					_202B_202C_200E_200B_206F_202B_200E_202C_206C_200B_200D_202C_202D_206B_202E_202D_200D_200D_206A_206B_206F_206A_206D_200D_206A_206D_206D_206E_206A_200B_200D_206D_206D_200C_202C_206B_202A_202B_200E_206F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num6.ToString(), "_MetallicOcclusionSmoothness"), new Vector4(_200B_202A_200F_200C_202E_200E_206D_200F_206F_200B_206F_206E_206A_200E_206A_200C_200B_202E_202E_206F_202B_202D_202E_206D_202E_206A_202C_200E_202B_206E_206D_202E_202D_206C_206D_206B_206E_206A_200D_206F_202E(array[num6]), _202E_200E_202C_202B_200E_206B_206D_202C_200B_202A_206F_202B_206C_200F_206B_202D_200C_200D_206C_200F_200F_206A_206B_202D_200D_200E_200F_206A_200C_202E_200E_202A_206F_202A_206C_206C_200C_200F_200E_202C_202E(array[num6]).y, 0f, _200D_202C_202B_206F_200D_200D_200F_202A_200F_206A_200F_206B_200F_206F_206B_206D_206E_206C_200F_202B_206D_206B_200D_202D_200C_202D_200C_200F_202D_202D_200F_202E_206A_200F_200F_206F_206B_206E_202B_202C_202E(array[num6])));
					num = -1400929928;
					continue;
				case 26u:
					goto IL_0559;
				case 17u:
					_202A_206D_206E_200C_206A_206B_206A_200B_202C_206C_200D_202E_202D_202B_202E_206C_206A_206D_206C_206D_206F_206D_206B_206F_202B_206C_200D_200C_206B_202B_206F_200C_200C_206C_200E_200F_202B_202C_200F_200C_202E(material, _202E_200D_206C_206C_206B_206A_202D_200F_202E_206F_200E_206B_202A_206F_206D_202B_206D_202E_206D_202C_206E_206D_202E_206C_200D_206C_206B_200F_200E_206B_200C_202A_202A_206C_206C_206E_206F_206F_206E_206C_202E("_T2M_LAYER_{0}_MASK", (object)num6));
					num = (int)((num2 * 1027961915) ^ 0x703FB978);
					continue;
				case 37u:
					_202B_202E_206D_200E_202E_200E_200D_202B_200E_206E_206E_200B_202C_200E_206E_200C_206F_202C_200F_206D_200C_202E_206B_206B_202C_206F_202E_202D_202C_206F_200D_202E_200E_202C_202B_206A_200E_206C_200D_202B_202E(material, _206A_202B_200D_202B_206E_200F_200C_202E_202C_200E_202E_200B_200C_200E_202A_200F_206D_206F_202C_206A_200B_206A_206B_206C_200D_202C_202E_200B_202A_200E_206F_206C_200D_202A_200E_202D_200E_202D_206B_206C_202E("_T2M_SplatMap_", num3.ToString()), (Texture)_200D_200B_202E_200C_202D_200F_202A_200D_206B_200E_206C_206C_206E_200B_202E_202D_202D_200D_202E_202E_200C_206E_200E_202A_200D_200E_200E_200C_206D_206B_206B_206B_206A_206A_200E_202A_206D_200F_202E_202B_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E)[num3]);
					num = -1959817002;
					continue;
				case 7u:
					goto IL_05cb;
				case 23u:
					goto IL_05d8;
				case 24u:
					_202B_202E_206D_200E_202E_200E_200D_202B_200E_206E_206E_200B_202C_200E_206E_200C_206F_202C_200F_206D_200C_202E_206B_206B_202C_206F_202E_202D_202C_206F_200D_202E_200E_202C_202B_206A_200E_206C_200D_202B_202E(material, "_T2M_HolesMap", _206E_206A_200C_206A_202C_200E_202E_200B_200C_200D_202A_202E_200B_202C_200B_200C_200F_202D_206A_206C_200F_206B_200F_202E_202A_206F_202C_200F_200D_202E_206C_202A_206F_206A_206A_206B_200C_202C_206D_200F_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E));
					num = (int)((num2 * 1982966481) ^ 0x48AA3531);
					continue;
				case 35u:
					_206F_206E_200E_202E_200F_200B_206B_200B_200E_206D_202B_202B_206F_200B_206C_202D_202E_202E_202B_200C_200B_206E_200E_206A_200D_200C_200B_206E_206B_206C_202D_202A_206A_206C_206B_202E_202D_206C_206B_202C_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num6.ToString(), "_ColorTint"), new Color(vector.x, vector.y, vector.z, 1f).gamma);
					num = (int)((num2 * 318729847) ^ 0x79D586EF);
					continue;
				case 41u:
					_202B_202C_200E_200B_206F_202B_200E_202C_206C_200B_200D_202C_202D_206B_202E_202D_200D_200D_206A_206B_206F_206A_206D_200D_206A_206D_206D_206E_206A_200B_200D_206D_206D_200C_202C_206B_202A_202B_200E_206F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num6.ToString(), "_uvScaleOffset"), new Vector4(_200F_200E_206D_206F_200E_206B_202E_202D_202E_202A_206B_206D_206E_206B_202E_206B_200D_202A_206D_206A_200F_200C_206C_202B_202D_202A_206F_200D_202A_202C_206F_206E_200D_202D_202C_200F_206E_202A_206A_206D_202E(array[num6]).x, _200F_200E_206D_206F_200E_206B_202E_202D_202E_202A_206B_206D_206E_206B_202E_206B_200D_202A_206D_206A_200F_200C_206C_202B_202D_202A_206F_200D_202A_202C_206F_206E_200D_202D_202C_200F_206E_202A_206A_206D_202E(array[num6]).y, _200C_200C_206B_202E_200D_202E_206A_202C_200E_206B_206E_206E_206B_200B_206D_202B_202C_206F_206A_206E_202C_202A_202D_200E_202D_206A_206F_202A_206A_200B_202C_200F_200D_206A_200E_202C_206B_200D_206D_202E_202E(array[num6]).x, _200C_200C_206B_202E_200D_202E_206A_202C_200E_206B_206E_206E_206B_200B_206D_202B_202C_206F_206A_206E_202C_202A_202D_200E_202D_206A_206F_202A_206A_200B_202C_200F_200D_206A_200E_202C_206B_200D_206D_202E_202E(array[num6]).y));
					num = ((int)num2 * -169065768) ^ -847713319;
					continue;
				case 3u:
					_202A_206D_206E_200C_206A_206B_206A_200B_202C_206C_200D_202E_202D_202B_202E_206C_206A_206D_206C_206D_206F_206D_206B_206F_202B_206C_200D_200C_206B_202B_206F_200C_200C_206C_200E_200F_202B_202C_200F_200C_202E(material, _202E_200D_206C_206C_206B_206A_202D_200F_202E_206F_200E_206B_202A_206F_206D_202B_206D_202E_206D_202C_206E_206D_202E_206C_200D_206C_206B_200F_200E_206B_200C_202A_202A_206C_206C_206E_206F_206F_206E_206C_202E("_T2M_LAYER_{0}_NORMAL", (object)num6));
					num = ((int)num2 * -250700325) ^ -1598062128;
					continue;
				case 6u:
					num12 = Mathf.Clamp(array.Length, 2, 16);
					_202A_206D_206E_200C_206A_206B_206A_200B_202C_206C_200D_202E_202D_202B_202E_206C_206A_206D_206C_206D_206F_206D_206B_206F_202B_206C_200D_200C_206B_202B_206F_200C_200C_206C_200E_200F_202B_202C_200F_200C_202E(material, _206A_202B_200D_202B_206E_200F_200C_202E_202C_200E_202E_200B_200C_200E_202A_200F_206D_206F_202C_206A_200B_206A_206B_206C_200D_202C_202E_200B_202A_200E_206F_206C_200D_202A_200E_202D_200E_202D_206B_206C_202E("_T2M_LAYER_COUNT_", num12.ToString()));
					_202A_200E_200C_206C_200F_206E_202C_200E_206C_200D_206E_206F_202D_206E_206B_202C_206B_200D_206F_202A_202D_206A_206E_200F_200F_206A_200F_202D_206F_202B_206C_200C_200D_202B_202B_202E_206F_200D_202D_200F_202E(material, "_T2M_Layer_Count", (float)num12);
					num = -1471105445;
					continue;
				case 40u:
				{
					_202A_200E_200C_206C_200F_206E_202C_200E_206C_200D_206E_206F_202D_206E_206B_202C_206B_200D_206F_202A_202D_206A_206E_200F_200F_206A_200F_202D_206F_202B_206C_200C_200D_202B_202B_202E_206F_200D_202D_200F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num6.ToString(), "_NormalScale"), _206A_202E_202B_202A_206D_200F_200F_200B_202A_202E_206E_200B_206A_200F_200F_202E_200B_200F_202E_202B_206B_200E_206F_202D_206E_206C_206A_202E_206A_206F_200C_206E_200E_206F_206D_206A_200D_200D_200E_200F_202E(array[num6]));
					int num9;
					int num10;
					if (_206B_206A_202B_206E_200C_200F_202B_206B_200C_200C_206A_202C_206E_206B_202C_200D_206D_206C_206A_206F_206B_202B_206A_200B_206C_202E_200D_202C_206D_206C_202D_206B_200E_200D_202D_206B_200B_206A_200D_200E_202E((UnityEngine.Object)_200E_206A_206D_202C_200B_206E_200F_206A_206F_202E_206F_206D_200F_202A_200E_202C_206B_202E_200F_206D_202D_206D_202C_202B_200B_206F_200B_200B_200E_202B_202B_200B_200F_202E_202A_202C_206B_206F_206D_200C_202E(array[num6]), (UnityEngine.Object)null))
					{
						num9 = -846057822;
						num10 = num9;
					}
					else
					{
						num9 = -949636622;
						num10 = num9;
					}
					num = num9 ^ (int)(num2 * 1054281971);
					continue;
				}
				case 9u:
				{
					int num7;
					int num8;
					if (array.Length != 0)
					{
						num7 = -1155359214;
						num8 = num7;
					}
					else
					{
						num7 = -391992260;
						num8 = num7;
					}
					num = num7 ^ (int)(num2 * 1338025309);
					continue;
				}
				case 32u:
				{
					array = _200F_202C_202D_202A_206A_200B_200B_202C_200E_200C_200F_206F_200D_200C_206C_206E_200E_206F_206B_206B_202C_200F_206B_200F_200C_206F_200D_202D_206E_206C_202C_200F_202B_202A_200C_202A_202A_206C_200D_206F_202E._206D_202C_200D_206F_200B_206C_202E_206B_202C_206E_200B_200F_202B_202B_206A_202A_202A_202D_200D_202C_202D_206C_202D_200B_202A_200D_206D_206D_206C_206B_200D_200C_206D_206D_202A_200B_202D_206D_206F_202E_202E._200E_206B_206C_206E_206F_206C_202B_206D_200F_206B_202E_202D_202D_202C_200C_206B_202D_202E_206B_206F_200C_202E_202D_200C_202E_200B_200F_200E_202B_206B_200D_206C_206A_206A_202E_200E_200B_202D_202A_206F_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E);
					int num4;
					int num5;
					if (array == null)
					{
						num4 = -381936691;
						num5 = num4;
					}
					else
					{
						num4 = -979459745;
						num5 = num4;
					}
					num = num4 ^ ((int)num2 * -1670530254);
					continue;
				}
				case 22u:
					num3++;
					num = ((int)num2 * -2144011324) ^ 0x37954535;
					continue;
				case 43u:
					goto IL_0810;
				default:
					{
						return material;
					}
					IL_04ec:
					flag = (byte)num11 != 0;
					num = -1741233873;
					continue;
				}
				break;
				IL_05d8:
				string text = Constants.shaderSplatmap;
				goto IL_05e4;
				IL_05cb:
				if (!hasHolesmap)
				{
					num = -1780421051;
					continue;
				}
				text = Constants.shaderSplatmapWithHoles;
				goto IL_05e4;
				IL_0204:
				_202B_202E_206D_200E_202E_200E_200D_202B_200E_206E_206E_200B_202C_200E_206E_200C_206F_202C_200F_206D_200C_202E_206B_206B_202C_206F_202E_202D_202C_206F_200D_202E_200E_202C_202B_206A_200E_206C_200D_202B_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num6.ToString(), "_Mask"), (Texture)_206D_202D_202E_206C_206F_206F_200D_206A_202E_200B_206F_206D_202C_206B_200B_200D_206A_206A_200E_202A_202A_200D_202E_202A_200B_200E_202C_206D_206B_202A_206E_202C_200B_200F_200C_200C_202C_200B_200E_202D_202E(array[num6]));
				int num19;
				if (_206B_206A_202B_206E_200C_200F_202B_206B_200C_200C_206A_202C_206E_206B_202C_200D_206D_206C_206A_206F_206B_202B_206A_200B_206C_202E_200D_202C_206D_206C_202D_206B_200E_200D_202D_206B_200B_206A_200D_200E_202E((UnityEngine.Object)_206D_202D_202E_206C_206F_206F_200D_206A_202E_200B_206F_206D_202C_206B_200B_200D_206A_206A_200E_202A_202A_200D_202E_202A_200B_200E_202C_206D_206B_202A_206E_202C_200B_200F_200C_200C_202C_200B_200E_202D_202E(array[num6]), (UnityEngine.Object)null))
				{
					num = -779611080;
					num19 = num;
				}
				else
				{
					num = -525160106;
					num19 = num;
				}
				continue;
				IL_03a8:
				int num20;
				if (!hasHolesmap)
				{
					num = -81051632;
					num20 = num;
				}
				else
				{
					num = -39988868;
					num20 = num;
				}
				continue;
				IL_05e4:
				shader = _200B_200F_206A_200E_206A_202B_202D_206E_206D_200D_200F_206C_202E_200E_202E_206A_200C_200F_202E_206F_200B_206B_202B_206E_206A_200B_206D_206E_202D_200C_200C_206A_202B_200B_200E_202E_206D_200B_206C_202E(text);
				int num21;
				if (!_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)shader, (UnityEngine.Object)null))
				{
					num = -1557657127;
					num21 = num;
				}
				else
				{
					num = -1192671210;
					num21 = num;
				}
				continue;
				IL_01ae:
				int num22;
				if (array.Length <= Constants.maxLayersCount)
				{
					num = -594502655;
					num22 = num;
				}
				else
				{
					num = -216387320;
					num22 = num;
				}
				continue;
				IL_024e:
				int num23;
				if (num6 < array.Length)
				{
					num = -755614166;
					num23 = num;
				}
				else
				{
					num = -1850439968;
					num23 = num;
				}
				continue;
				IL_0559:
				int num24;
				if (num6 >= num12)
				{
					num = -1850439968;
					num24 = num;
				}
				else
				{
					num = -1496598506;
					num24 = num;
				}
				continue;
				IL_02a0:
				int num25;
				if (num3 < Mathf.Clamp(_200D_200B_202E_200C_202D_200F_202A_200D_206B_200E_206C_206C_206E_200B_202E_202D_202D_200D_202E_202E_200C_206E_200E_202A_200D_200E_200E_200C_206D_206B_206B_206B_206A_206A_200E_202A_206D_200F_202E_202B_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E).Length, 1, 4))
				{
					num = -1984929399;
					num25 = num;
				}
				else
				{
					num = -1482281856;
					num25 = num;
				}
			}
			goto IL_0011;
			IL_0810:
			int num26;
			if (_200D_200B_202E_200C_202D_200F_202A_200D_206B_200E_206C_206C_206E_200B_202E_202D_202D_200D_202E_202E_200C_206E_200E_202A_200D_200E_200E_200C_206D_206B_206B_206B_206A_206A_200E_202A_206D_200F_202E_202B_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E) != null)
			{
				num = -754206401;
				num26 = num;
			}
			else
			{
				num = -1007044380;
				num26 = num;
			}
			goto IL_0016;
		}

		public Material ExportSplatmapMaterial2DArray(Texture2DArray splatmaps, Texture2DArray diffusemaps, Texture2DArray normalmaps, Texture2DArray maskmaps, bool hasHolesmap)
		{
			if (_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, (UnityEngine.Object)null))
			{
				goto IL_0011;
			}
			goto IL_02ed;
			IL_0011:
			int num = 1915477600;
			goto IL_0016;
			IL_0016:
			Material material = default(Material);
			int num3 = default(int);
			TerrainLayer[] array = default(TerrainLayer[]);
			int num8 = default(int);
			Vector4 vector = default(Vector4);
			Shader shader = default(Shader);
			while (true)
			{
				uint num2;
				int num9;
				string text;
				bool flag;
				switch ((num2 = (uint)num ^ 0x4D2216EBu) % 43)
				{
				case 6u:
					break;
				case 23u:
					_202B_202C_200E_200B_206F_202B_200E_202C_206C_200B_200D_202C_202D_206B_202E_202D_200D_200D_206A_206B_206F_206A_206D_200D_206A_206D_206D_206E_206A_200B_200D_206D_206D_200C_202C_206B_202A_202B_200E_206F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num3.ToString(), "_MaskMapRemapMax"), _202E_200E_202C_202B_200E_206B_206D_202C_200B_202A_206F_202B_206C_200F_206B_202D_200C_200D_206C_200F_200F_206A_206B_202D_200D_200E_200F_206A_200C_202E_200E_202A_206F_202A_206C_206C_200C_200F_200E_202C_202E(array[num3]));
					num = ((int)num2 * -1660924924) ^ -1505598624;
					continue;
				case 9u:
					_202B_202E_206D_200E_202E_200E_200D_202B_200E_206E_206E_200B_202C_200E_206E_200C_206F_202C_200F_206D_200C_202E_206B_206B_202C_206F_202E_202D_202C_206F_200D_202E_200E_202C_202B_206A_200E_206C_200D_202B_202E(material, "_T2M_MaskMaps2DArray", (Texture)maskmaps);
					num = (int)(num2 * 2031081006) ^ -1540234204;
					continue;
				case 18u:
					num3 = 0;
					num = ((int)num2 * -1150266362) ^ 0x57C646EF;
					continue;
				case 32u:
					_202B_202C_200E_200B_206F_202B_200E_202C_206C_200B_200D_202C_202D_206B_202E_202D_200D_200D_206A_206B_206F_206A_206D_200D_206A_206D_206D_206E_206A_200B_200D_206D_206D_200C_202C_206B_202A_202B_200E_206F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num3.ToString(), "_MapsUsage"), new Vector4((!_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)_202B_200E_206D_206D_200D_202A_206B_200E_202A_206A_206E_200D_202B_206B_202B_206F_206F_206B_200E_200D_202A_206E_200C_200E_200B_200B_202C_200F_202D_202B_200E_202D_206A_206F_206C_206D_206F_206C_206A_206F_202E(array[num3]), (UnityEngine.Object)null)) ? 1 : 0, (!_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)_200E_206A_206D_202C_200B_206E_200F_206A_206F_202E_206F_206D_200F_202A_200E_202C_206B_202E_200F_206D_202D_206D_202C_202B_200B_206F_200B_200B_200E_202B_202B_200B_200F_202E_202A_202C_206B_206F_206D_200C_202E(array[num3]), (UnityEngine.Object)null)) ? 1 : 0, (!_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)_206D_202D_202E_206C_206F_206F_200D_206A_202E_200B_206F_206D_202C_206B_200B_200D_206A_206A_200E_202A_202A_200D_202E_202A_200B_200E_202C_206D_206B_202A_206E_202C_200B_200F_200C_200C_202C_200B_200E_202D_202E(array[num3]), (UnityEngine.Object)null)) ? 1 : 0, 0f));
					_202B_202C_200E_200B_206F_202B_200E_202C_206C_200B_200D_202C_202D_206B_202E_202D_200D_200D_206A_206B_206F_206A_206D_200D_206A_206D_206D_206E_206A_200B_200D_206D_206D_200C_202C_206B_202A_202B_200E_206F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num3.ToString(), "_MetallicOcclusionSmoothness"), new Vector4(_200B_202A_200F_200C_202E_200E_206D_200F_206F_200B_206F_206E_206A_200E_206A_200C_200B_202E_202E_206F_202B_202D_202E_206D_202E_206A_202C_200E_202B_206E_206D_202E_202D_206C_206D_206B_206E_206A_200D_206F_202E(array[num3]), _202E_200E_202C_202B_200E_206B_206D_202C_200B_202A_206F_202B_206C_200F_206B_202D_200C_200D_206C_200F_200F_206A_206B_202D_200D_200E_200F_206A_200C_202E_200E_202A_206F_202A_206C_206C_200C_200F_200E_202C_202E(array[num3]).y, 0f, _200D_202C_202B_206F_200D_200D_200F_202A_200F_206A_200F_206B_200F_206F_206B_206D_206E_206C_200F_202B_206D_206B_200D_202D_200C_202D_200C_200F_202D_202D_200F_202E_206A_200F_200F_206F_206B_206E_202B_202C_202E(array[num3])));
					if (_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)_206D_202D_202E_206C_206F_206F_200D_206A_202E_200B_206F_206D_202C_206B_200B_200D_206A_206A_200E_202A_202A_200D_202E_202A_200B_200E_202C_206D_206B_202A_206E_202C_200B_200F_200C_200C_202C_200B_200E_202D_202E(array[num3]), (UnityEngine.Object)null))
					{
						num = 1901363355;
						continue;
					}
					num9 = 0;
					goto IL_0793;
				case 0u:
					_202A_200E_200C_206C_200F_206E_202C_200E_206C_200D_206E_206F_202D_206E_206B_202C_206B_200D_206F_202A_202D_206A_206E_200F_200F_206A_200F_202D_206F_202B_206C_200C_200D_202B_202B_202E_206F_200D_202D_200F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num3.ToString(), "_NormalScale"), _206A_202E_202B_202A_206D_200F_200F_200B_202A_202E_206E_200B_206A_200F_200F_202E_200B_200F_202E_202B_206B_200E_206F_202D_206E_206C_206A_202E_206A_206F_200C_206E_200E_206F_206D_206A_200D_200D_200E_200F_202E(array[num3]));
					num = ((int)num2 * -1382268379) ^ -1061855290;
					continue;
				case 12u:
				{
					int num12;
					int num13;
					if (array.Length == 0)
					{
						num12 = 1468217159;
						num13 = num12;
					}
					else
					{
						num12 = 1574312680;
						num13 = num12;
					}
					num = num12 ^ (int)(num2 * 1225125534);
					continue;
				}
				case 10u:
					_202A_206D_206E_200C_206A_206B_206A_200B_202C_206C_200D_202E_202D_202B_202E_206C_206A_206D_206C_206D_206F_206D_206B_206F_202B_206C_200D_200C_206B_202B_206F_200C_200C_206C_200E_200F_202B_202C_200F_200C_202E(material, _206A_202B_200D_202B_206E_200F_200C_202E_202C_200E_202E_200B_200C_200E_202A_200F_206D_206F_202C_206A_200B_206A_206B_206C_200D_202C_202E_200B_202A_200E_206F_206C_200D_202A_200E_202D_200E_202D_206B_206C_202E("_T2M_LAYER_COUNT_", num8.ToString()));
					_202A_200E_200C_206C_200F_206E_202C_200E_206C_200D_206E_206F_202D_206E_206B_202C_206B_200D_206F_202A_202D_206A_206E_200F_200F_206A_200F_202D_206F_202B_206C_200C_200D_202B_202B_202E_206F_200D_202D_200F_202E(material, "_T2M_Layer_Count", (float)num8);
					_202A_206D_206E_200C_206A_206B_206A_200B_202C_206C_200D_202E_202D_202B_202E_206C_206A_206D_206C_206D_206F_206D_206B_206F_202B_206C_200D_200C_206B_202B_206F_200C_200C_206C_200E_200F_202B_202C_200F_200C_202E(material, "_T2M_TEXTURE_SAMPLE_TYPE_ARRAY");
					_202B_202E_206D_200E_202E_200E_200D_202B_200E_206E_206E_200B_202C_200E_206E_200C_206F_202C_200F_206D_200C_202E_206B_206B_202C_206F_202E_202D_202C_206F_200D_202E_200E_202C_202B_206A_200E_206C_200D_202B_202E(material, "_T2M_SplatMaps2DArray", (Texture)splatmaps);
					num = (int)((num2 * 309009757) ^ 0x11D87BB3);
					continue;
				case 11u:
					num8 = Mathf.Clamp(array.Length, 2, 16);
					num = 1260429685;
					continue;
				case 14u:
					goto IL_02cf;
				case 36u:
					goto IL_02ed;
				case 42u:
					num3++;
					num = 2079497929;
					continue;
				case 13u:
				{
					_202B_202C_200E_200B_206F_202B_200E_202C_206C_200B_200D_202C_202D_206B_202E_202D_200D_200D_206A_206B_206F_206A_206D_200D_206A_206D_206D_206E_206A_200B_200D_206D_206D_200C_202C_206B_202A_202B_200E_206F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num3.ToString(), "_uvScaleOffset"), new Vector4(_200F_200E_206D_206F_200E_206B_202E_202D_202E_202A_206B_206D_206E_206B_202E_206B_200D_202A_206D_206A_200F_200C_206C_202B_202D_202A_206F_200D_202A_202C_206F_206E_200D_202D_202C_200F_206E_202A_206A_206D_202E(array[num3]).x, _200F_200E_206D_206F_200E_206B_202E_202D_202E_202A_206B_206D_206E_206B_202E_206B_200D_202A_206D_206A_200F_200C_206C_202B_202D_202A_206F_200D_202A_202C_206F_206E_200D_202D_202C_200F_206E_202A_206A_206D_202E(array[num3]).y, _200C_200C_206B_202E_200D_202E_206A_202C_200E_206B_206E_206E_206B_200B_206D_202B_202C_206F_206A_206E_202C_202A_202D_200E_202D_206A_206F_202A_206A_200B_202C_200F_200D_206A_200E_202C_206B_200D_206D_202E_202E(array[num3]).x, _200C_200C_206B_202E_200D_202E_206A_202C_200E_206B_206E_206E_206B_200B_206D_202B_202C_206F_206A_206E_202C_202A_202D_200E_202D_206A_206F_202A_206A_200B_202C_200F_200D_206A_200E_202C_206B_200D_206D_202E_202E(array[num3]).y));
					int num20;
					int num21;
					if (_206B_206A_202B_206E_200C_200F_202B_206B_200C_200C_206A_202C_206E_206B_202C_200D_206D_206C_206A_206F_206B_202B_206A_200B_206C_202E_200D_202C_206D_206C_202D_206B_200E_200D_202D_206B_200B_206A_200D_200E_202E((UnityEngine.Object)_206E_206B_206E_206B_206A_202E_206E_202C_206D_202E_206C_206C_202C_200D_206E_206F_206B_200F_202B_202C_200E_206E_206A_200D_206A_206E_202A_200C_206B_202D_202C_202A_202B_202C_200D_206D_202C_206E_206A_202C_202E(), (UnityEngine.Object)null))
					{
						num20 = -382081505;
						num21 = num20;
					}
					else
					{
						num20 = -2109861873;
						num21 = num20;
					}
					num = num20 ^ (int)(num2 * 557401961);
					continue;
				}
				case 21u:
					goto IL_03a1;
				case 16u:
					_206F_206E_200E_202E_200F_200B_206B_200B_200E_206D_202B_202B_206F_200B_206C_202D_202E_202E_202B_200C_200B_206E_200E_206A_200D_200C_200B_206E_206B_206C_202D_202A_206A_206C_206B_202E_202D_206C_206B_202C_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num3.ToString(), "_ColorTint"), new Color(vector.x, vector.y, vector.z, 1f));
					num = 635862504;
					continue;
				case 7u:
					_206F_206E_200E_202E_200F_200B_206B_200B_200E_206D_202B_202B_206F_200B_206C_202D_202E_202E_202B_200C_200B_206E_200E_206A_200D_200C_200B_206E_206B_206C_202D_202A_206A_206C_206B_202E_202D_206C_206B_202C_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num3.ToString(), "_ColorTint"), new Color(vector.x, vector.y, vector.z, 1f).gamma);
					num = ((int)num2 * -2042077740) ^ -391498460;
					continue;
				case 38u:
				{
					int num16;
					int num17;
					if (!_206B_206A_202B_206E_200C_200F_202B_206B_200C_200C_206A_202C_206E_206B_202C_200D_206D_206C_206A_206F_206B_202B_206A_200B_206C_202E_200D_202C_206D_206C_202D_206B_200E_200D_202D_206B_200B_206A_200D_200E_202E((UnityEngine.Object)normalmaps, (UnityEngine.Object)null))
					{
						num16 = 961665245;
						num17 = num16;
					}
					else
					{
						num16 = 2036759283;
						num17 = num16;
					}
					num = num16 ^ ((int)num2 * -386539129);
					continue;
				}
				case 35u:
					_202A_206D_206E_200C_206A_206B_206A_200B_202C_206C_200D_202E_202D_202B_202E_206C_206A_206D_206C_206D_206F_206D_206B_206F_202B_206C_200D_200C_206B_202B_206F_200C_200C_206C_200E_200F_202B_202C_200F_200C_202E(material, _202E_200D_206C_206C_206B_206A_202D_200F_202E_206F_200E_206B_202A_206F_206D_202B_206D_202E_206D_202C_206E_206D_202E_206C_200D_206C_206B_200F_200E_206B_200C_202A_202A_206C_206C_206E_206F_206F_206E_206C_202E("_T2M_LAYER_{0}_NORMAL", (object)num3));
					num = (int)((num2 * 1848081612) ^ 0x665B812);
					continue;
				case 25u:
				{
					int num6;
					int num7;
					if (_200D_200B_202E_200C_202D_200F_202A_200D_206B_200E_206C_206C_206E_200B_202E_202D_202D_200D_202E_202E_200C_206E_200E_202A_200D_200E_200E_200C_206D_206B_206B_206B_206A_206A_200E_202A_206D_200F_202E_202B_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E).Length == 0)
					{
						num6 = 1752313655;
						num7 = num6;
					}
					else
					{
						num6 = 480501928;
						num7 = num6;
					}
					num = num6 ^ ((int)num2 * -863639143);
					continue;
				}
				case 29u:
					goto IL_04dd;
				case 3u:
					Utilities.Debug(LogType.Warning, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[5]
					{
						"'",
						_200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E),
						"' uses ",
						array.Length.ToString(),
						" layers. Only first 16 will be used in Splatmap shader."
					}), null, _202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E);
					num = ((int)num2 * -1169610615) ^ 0x4D3A50FD;
					continue;
				case 37u:
					text = Constants.shaderSplatmap;
					goto IL_0562;
				case 41u:
					goto IL_0572;
				case 1u:
					_202B_202E_206D_200E_202E_200E_200D_202B_200E_206E_206E_200B_202C_200E_206E_200C_206F_202C_200F_206D_200C_202E_206B_206B_202C_206F_202E_202D_202C_206F_200D_202E_200E_202C_202B_206A_200E_206C_200D_202B_202E(material, "_T2M_DiffuseMaps2DArray", (Texture)diffusemaps);
					num = ((int)num2 * -2084082305) ^ -76727214;
					continue;
				case 2u:
					return null;
				case 8u:
					_202B_202C_200E_200B_206F_202B_200E_202C_206C_200B_200D_202C_202D_206B_202E_202D_200D_200D_206A_206B_206F_206A_206D_200D_206A_206D_206D_206E_206A_200B_200D_206D_206D_200C_202C_206B_202A_202B_200E_206F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num3.ToString(), "_MaskMapRemapMin"), _202B_206A_206F_206E_202C_206E_200B_206C_202C_202A_200C_206D_206E_200B_202B_206B_206B_206A_202E_202E_200D_206E_200C_202C_200E_200F_202E_202E_202E_206A_206E_200C_206E_206B_200E_202B_200D_202E_202B_202A_202E(array[num3]));
					num = ((int)num2 * -743528822) ^ 0x9E4F084;
					continue;
				case 24u:
					_202A_206D_206E_200C_206A_206B_206A_200B_202C_206C_200D_202E_202D_202B_202E_206C_206A_206D_206C_206D_206F_206D_206B_206F_202B_206C_200D_200C_206B_202B_206F_200C_200C_206C_200E_200F_202B_202C_200F_200C_202E(material, _202E_200D_206C_206C_206B_206A_202D_200F_202E_206F_200E_206B_202A_206F_206D_202B_206D_202E_206D_202C_206E_206D_202E_206C_200D_206C_206B_200F_200E_206B_200C_202A_202A_206C_206C_206E_206F_206F_206E_206C_202E("_T2M_LAYER_{0}_MASK", (object)num3));
					num = (int)((num2 * 123545651) ^ 0xA4F21FD);
					continue;
				case 33u:
					goto IL_0620;
				case 40u:
					return null;
				case 17u:
					if (hasHolesmap)
					{
						text = Constants.shaderSplatmapWithHoles;
						goto IL_0562;
					}
					num = 831160245;
					continue;
				case 39u:
					num = ((int)num2 * -1792463989) ^ 0x4823B7AF;
					continue;
				case 34u:
				{
					int num18;
					int num19;
					if (!_206B_206A_202B_206E_200C_200F_202B_206B_200C_200C_206A_202C_206E_206B_202C_200D_206D_206C_206A_206F_206B_202B_206A_200B_206C_202E_200D_202C_206D_206C_202D_206B_200E_200D_202D_206B_200B_206A_200D_200E_202E((UnityEngine.Object)_206D_202D_202E_206C_206F_206F_200D_206A_202E_200B_206F_206D_202C_206B_200B_200D_206A_206A_200E_202A_202A_200D_202E_202A_200B_200E_202C_206D_206B_202A_206E_202C_200B_200F_200C_200C_202C_200B_200E_202D_202E(array[num3]), (UnityEngine.Object)null))
					{
						num18 = 1942148577;
						num19 = num18;
					}
					else
					{
						num18 = 1071861763;
						num19 = num18;
					}
					num = num18 ^ ((int)num2 * -347061275);
					continue;
				}
				case 20u:
					_202B_202E_206D_200E_202E_200E_200D_202B_200E_206E_206E_200B_202C_200E_206E_200C_206F_202C_200F_206D_200C_202E_206B_206B_202C_206F_202E_202D_202C_206F_200D_202E_200E_202C_202B_206A_200E_206C_200D_202B_202E(material, "_T2M_HolesMap", _206E_206A_200C_206A_202C_200E_202E_200B_200C_200D_202A_202E_200B_202C_200B_200C_200F_202D_206A_206C_200F_206B_200F_202E_202A_206F_202C_200F_200D_202E_206C_202A_206F_206A_206A_206B_200C_202C_206D_200F_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E));
					num = ((int)num2 * -2101236284) ^ 0x5303AF47;
					continue;
				case 26u:
					Utilities.Debug(LogType.Warning, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("Shader '", hasHolesmap ? Constants.shaderSplatmapWithHoles : Constants.shaderSplatmap, "' is not installed"), null);
					return _202B_206D_206E_200D_202C_202C_202E_200D_200D_200F_206A_206B_206F_206B_206F_202D_202A_206B_202A_202A_206D_200B_200F_206F_202B_206C_200E_200F_202D_200B_200E_200E_202E_200D_202E_202A_200C_206E_200C_200D_202E(_200B_200F_206A_200E_206A_202B_202D_206E_206D_200D_200F_206C_202E_200E_202E_206A_200C_200F_202E_206F_200B_206B_202B_206E_206A_200B_206D_206E_202D_200C_200C_206A_202B_200B_200E_202E_206D_200B_206C_202E(Utilities.GetUnityDefaultShader()));
				case 30u:
				{
					int num14;
					int num15;
					if (_206B_206A_202B_206E_200C_200F_202B_206B_200C_200C_206A_202C_206E_206B_202C_200D_206D_206C_206A_206F_206B_202B_206A_200B_206C_202E_200D_202C_206D_206C_202D_206B_200E_200D_202D_206B_200B_206A_200D_200E_202E((UnityEngine.Object)_200E_206A_206D_202C_200B_206E_200F_206A_206F_202E_206F_206D_200F_202A_200E_202C_206B_202E_200F_206D_202D_206D_202C_202B_200B_206F_200B_200B_200E_202B_202B_200B_200F_202E_202A_202C_206B_206F_206D_200C_202E(array[num3]), (UnityEngine.Object)null))
					{
						num14 = 1571170583;
						num15 = num14;
					}
					else
					{
						num14 = 1973012340;
						num15 = num14;
					}
					num = num14 ^ (int)(num2 * 2046806434);
					continue;
				}
				case 15u:
					return material;
				case 4u:
				{
					int num10;
					int num11;
					if (_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)shader, (UnityEngine.Object)null))
					{
						num10 = 1734574070;
						num11 = num10;
					}
					else
					{
						num10 = 369795860;
						num11 = num10;
					}
					num = num10 ^ (int)(num2 * 1714842138);
					continue;
				}
				case 22u:
					_202B_202E_206D_200E_202E_200E_200D_202B_200E_206E_206E_200B_202C_200E_206E_200C_206F_202C_200F_206D_200C_202E_206B_206B_202C_206F_202E_202D_202C_206F_200D_202E_200E_202C_202B_206A_200E_206C_200D_202B_202E(material, "_T2M_NormalMaps2DArray", (Texture)normalmaps);
					num = (int)((num2 * 1419690998) ^ 0x27796CDC);
					continue;
				case 19u:
					num9 = (Utilities.HasTextureAlphaChannel(_202B_200E_206D_206D_200D_202A_206B_200E_202A_206A_206E_200D_202B_206B_202B_206F_206F_206B_200E_200D_202A_206E_200C_200E_200B_200B_202C_200F_202D_202B_200E_202D_206A_206F_206C_206D_206F_206C_206A_206F_202E(array[num3])) ? 1 : 0);
					goto IL_0793;
				case 31u:
				{
					vector = _200D_200C_202B_202E_202D_200E_206D_206F_202B_206C_202B_206E_202C_200B_200F_200E_206A_200D_206F_202B_206A_206A_200B_200D_202D_202B_200E_202A_206F_206E_200C_202D_206E_206F_206A_202D_206F_200D_206E_200C_202E(array[num3]);
					int num4;
					int num5;
					if (_206E_200F_200D_206C_200D_200C_206B_200C_206F_202D_206D_200C_206F_206D_202C_206E_200F_206D_206D_200F_206E_206C_200B_206F_202B_206B_206D_200F_206F_206A_206A_200C_200B_206D_206D_206D_202B_200E_202E_206D_202E() != ColorSpace.Linear)
					{
						num4 = -1776249542;
						num5 = num4;
					}
					else
					{
						num4 = -1752918376;
						num5 = num4;
					}
					num = num4 ^ ((int)num2 * -2043116996);
					continue;
				}
				case 5u:
					Utilities.SetupAlphaTestProperties(material);
					num = (int)((num2 * 428332541) ^ 0x2750242E);
					continue;
				case 27u:
					goto IL_080d;
				default:
					{
						return material;
					}
					IL_0793:
					flag = (byte)num9 != 0;
					_202A_200E_200C_206C_200F_206E_202C_200E_206C_200D_206E_206F_202D_206E_206B_202C_206B_200D_206F_202A_202D_206A_206E_200F_200F_206A_200F_202D_206F_202B_206C_200C_200D_202B_202B_202E_206F_200D_202D_200F_202E(material, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("_T2M_Layer_", num3.ToString(), "_SmoothnessFromDiffuseAlpha"), (float)(flag ? 1 : 0));
					num = 1239488760;
					continue;
					IL_0562:
					shader = _200B_200F_206A_200E_206A_202B_202D_206E_206D_200D_200F_206C_202E_200E_202E_206A_200C_200F_202E_206F_200B_206B_202B_206E_206A_200B_206D_206E_202D_200C_200C_206A_202B_200B_200E_202E_206D_200B_206C_202E(text);
					num = 149224197;
					continue;
				}
				break;
				IL_080d:
				int num22;
				if (_206B_206A_202B_206E_200C_200F_202B_206B_200C_200C_206A_202C_206E_206B_202C_200D_206D_206C_206A_206F_206B_202B_206A_200B_206C_202E_200D_202C_206D_206C_202D_206B_200E_200D_202D_206B_200B_206A_200D_200E_202E((UnityEngine.Object)maskmaps, (UnityEngine.Object)null))
				{
					num = 290988397;
					num22 = num;
				}
				else
				{
					num = 1652123967;
					num22 = num;
				}
				continue;
				IL_0572:
				int num23;
				if (!hasHolesmap)
				{
					num = 150317578;
					num23 = num;
				}
				else
				{
					num = 2134593949;
					num23 = num;
				}
				continue;
				IL_03a1:
				material = _202B_206D_206E_200D_202C_202C_202E_200D_200D_200F_206A_206B_206F_206B_206F_202D_202A_206B_202A_202A_206D_200B_200F_206F_202B_206C_200E_200F_202D_200B_200E_200E_202E_200D_202E_202A_200C_206E_200C_200D_202E(shader);
				array = ExportTerrainLayers();
				int num24;
				if (array != null)
				{
					num = 1528661074;
					num24 = num;
				}
				else
				{
					num = 599964521;
					num24 = num;
				}
				continue;
				IL_0620:
				int num25;
				if (num3 < array.Length)
				{
					num = 1107456358;
					num25 = num;
				}
				else
				{
					num = 575297474;
					num25 = num;
				}
				continue;
				IL_02cf:
				int num26;
				if (array.Length > Constants.maxLayersCount)
				{
					num = 589994070;
					num26 = num;
				}
				else
				{
					num = 1945943000;
					num26 = num;
				}
				continue;
				IL_04dd:
				int num27;
				if (num3 < num8)
				{
					num = 99498887;
					num27 = num;
				}
				else
				{
					num = 575297474;
					num27 = num;
				}
			}
			goto IL_0011;
			IL_02ed:
			int num28;
			if (_200D_200B_202E_200C_202D_200F_202A_200D_206B_200E_206C_206C_206E_200B_202E_202D_202D_200D_202E_202E_200C_206E_200E_202A_200D_200E_200E_200C_206D_206B_206B_206B_206A_206A_200E_202A_206D_200F_202E_202B_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E) != null)
			{
				num = 1547141110;
				num28 = num;
			}
			else
			{
				num = 49944674;
				num28 = num;
			}
			goto IL_0016;
		}

		public TreePrototypesData[] ExportTreeData()
		{
			return _200F_200C_202B_200D_206E_200D_202B_206A_206C_202B_206B_200D_200B_202C_202A_206C_206C_206E_206C_206F_206D_200B_202B_200F_200C_206C_200D_202D_202B_206D_200B_206F_200C_206C_206E_206C_206D_206B_202B_200E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, 2, 2, 1, 1, 0, 0, false, false);
		}

		public TreePrototypesData[] ExportTreeData(int vertexCountHorizontal, int vertexCountVertical, int chunkCountHorizontal, int chunkCountVertical)
		{
			if (!_206A_200E_200E_202C_200D_200B_202A_206E_200B_206B_200D_202B_202E_206F_202B_202E_202B_202C_202C_200B_200F_206B_200F_202B_202C_202D_200D_206F_200D_206A_202A_202D_202A_202D_202D_206D_206F_200B_202D_202D_202E(vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, 0, 0, null))
			{
				while (true)
				{
					uint num;
					switch ((num = 0x1EDF5D74u ^ 0x24BF6880u) % 3)
					{
					case 0u:
						continue;
					case 1u:
						return null;
					}
					break;
				}
			}
			return _200F_200C_202B_200D_206E_200D_202B_206A_206C_202B_206B_200D_200B_202C_202A_206C_206C_206E_206C_206F_206D_200B_202B_200F_200C_206C_200D_202D_202B_206D_200B_206F_200C_206C_206E_206C_206D_206B_202B_200E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, 0, 0, false, true);
		}

		public TreePrototypesData[] ExportTreeData(int vertexCountHorizontal, int vertexCountVertical, int chunkCountHorizontal, int chunkCountVertical, int positionX, int positionY)
		{
			if (!_206A_200E_200E_202C_200D_200B_202A_206E_200B_206B_200D_202B_202E_206F_202B_202E_202B_202C_202C_200B_200F_206B_200F_202B_202C_202D_200D_206F_200D_206A_202A_202D_202A_202D_202D_206D_206F_200B_202D_202D_202E(vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, positionX, positionY, null))
			{
				while (true)
				{
					uint num;
					switch ((num = 0x4C2433AAu ^ 0x4A42FD70u) % 3)
					{
					case 2u:
						continue;
					case 1u:
						return null;
					}
					break;
				}
			}
			return _200F_200C_202B_200D_206E_200D_202B_206A_206C_202B_206B_200D_200B_202C_202A_206C_206C_206E_206C_206F_206D_200B_202B_200F_200C_206C_200D_202D_202B_206D_200B_206F_200C_206C_206E_206C_206D_206B_202B_200E_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, positionX, positionY, true, true);
		}

		private static TreePrototypesData[] _200F_200C_202B_200D_206E_200D_202B_206A_206C_202B_206B_200D_200B_202C_202A_206C_206C_206E_206C_206F_206D_200B_202B_200F_200C_206C_200D_202D_202B_206D_200B_206F_200C_206C_206E_206C_206D_206B_202B_200E_202E(TerrainData P_0, int P_1, int P_2, int P_3, int P_4, int P_5, int P_6, bool P_7, bool P_8)
		{
			if (_206F_202E_200B_206F_200C_202D_200B_200E_200E_206B_202C_202A_202A_206E_206E_202D_206F_200E_202A_200B_202D_206C_206E_206F_206B_202C_206C_206F_206F_206A_206E_206C_206A_202C_200E_200C_200C_206B_202C_202E_202E(P_0) != null)
			{
				goto IL_000b;
			}
			goto IL_0625;
			IL_000b:
			int num = -994225285;
			goto IL_0010;
			IL_0010:
			Dictionary<int, TreePrototypesData> dictionary = default(Dictionary<int, TreePrototypesData>);
			int prototypeIndex = default(int);
			Vector3 position = default(Vector3);
			Vector3 vector2 = default(Vector3);
			Vector3 one = default(Vector3);
			TreeInstance treeInstance = default(TreeInstance);
			bool[,] array = default(bool[,]);
			int num3 = default(int);
			Vector3 vector = default(Vector3);
			int num8 = default(int);
			int num13 = default(int);
			int num7 = default(int);
			Vector3 vector3 = default(Vector3);
			int num4 = default(int);
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0xFB29B849u) % 40)
				{
				case 35u:
					break;
				case 39u:
					goto IL_00c6;
				case 24u:
					dictionary[prototypeIndex].Add(position, vector2, one);
					num = -778376435;
					continue;
				case 21u:
					goto IL_0103;
				case 37u:
				{
					int num18;
					int num19;
					if (_206F_206F_202B_200D_200E_202D_206D_200E_202D_206F_200B_202C_206A_200B_206F_206C_206E_202E_206E_206A_200C_202C_206B_206C_200B_206D_206C_200D_200E_200B_206B_206C_206A_206A_206A_202B_206E_206E_200E_202A_202E(P_0) != null)
					{
						num18 = -1814382209;
						num19 = num18;
					}
					else
					{
						num18 = -1450007049;
						num19 = num18;
					}
					num = num18 ^ ((int)num2 * -611746499);
					continue;
				}
				case 30u:
				{
					int num28;
					int num29;
					if (!dictionary.ContainsKey(prototypeIndex))
					{
						num28 = -1163339070;
						num29 = num28;
					}
					else
					{
						num28 = -146634083;
						num29 = num28;
					}
					num = num28 ^ (int)(num2 * 2091682510);
					continue;
				}
				case 19u:
					num = (int)(num2 * 1212218796) ^ -1382614704;
					continue;
				case 36u:
				{
					int num16;
					int num17;
					if (!_206B_206A_202B_206E_200C_200F_202B_206B_200C_200C_206A_202C_206E_206B_202C_200D_206D_206C_206A_206F_206B_202B_206A_200B_206C_202E_200D_202C_206D_206C_202D_206B_200E_200D_202D_206B_200B_206A_200D_200E_202E((UnityEngine.Object)_200D_206D_202A_206B_206C_200F_202B_206B_200E_200B_200F_200E_202A_206C_206F_200D_200D_202A_206C_206A_200F_200B_206B_200D_202E_202D_202D_202C_202B_206D_200C_202B_202B_200F_206C_206E_200E_206B_200F_206C_202E(_206F_202E_200B_206F_200C_202D_200B_200E_200E_206B_202C_202A_202A_206E_206E_202D_206F_200E_202A_200B_202D_206C_206E_206F_206B_202C_206C_206F_206F_206A_206E_206C_206A_202C_200E_200C_200C_206B_202C_202E_202E(P_0)[treeInstance.prototypeIndex]).GetComponent<LODGroup>(), (UnityEngine.Object)null))
					{
						num16 = 987761174;
						num17 = num16;
					}
					else
					{
						num16 = 451651173;
						num17 = num16;
					}
					num = num16 ^ (int)(num2 * 1278606691);
					continue;
				}
				case 6u:
					position.z *= _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).z;
					num = (int)(num2 * 1156689078) ^ -645115540;
					continue;
				case 28u:
					array = null;
					num = -135468843;
					continue;
				case 27u:
					prototypeIndex = treeInstance.prototypeIndex;
					num = -1661001489;
					continue;
				case 12u:
					num3++;
					num = -2071820658;
					continue;
				case 32u:
				{
					int num24;
					int num25;
					if (dictionary.Count == 0)
					{
						num24 = -103754510;
						num25 = num24;
					}
					else
					{
						num24 = -1385208351;
						num25 = num24;
					}
					num = num24 ^ (int)(num2 * 1919631638);
					continue;
				}
				case 29u:
					goto IL_024f;
				case 23u:
					position.y *= _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).y;
					num = (int)((num2 * 20268380) ^ 0x2AD4FC3B);
					continue;
				case 18u:
				{
					int num20;
					int num21;
					if (Utilities._200D_200F_202C_202D_206B_206B_202C_200D_202C_202A_200C_200C_202D_206B_200C_202B_202D_202B_200B_206D_206E_202B_206C_200D_200E_202B_200D_200B_202D_206F_206B_202E_206C_206E_202C_202D_206B_200B_202E_206D_202E(P_0, position, P_3, P_4, P_5, P_6))
					{
						num20 = 1592973261;
						num21 = num20;
					}
					else
					{
						num20 = 584929675;
						num21 = num20;
					}
					num = num20 ^ ((int)num2 * -51256261);
					continue;
				}
				case 14u:
					vector = position;
					vector.x = Mathf.Clamp01(vector.x / _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).x);
					vector.z = Mathf.Clamp01(vector.z / _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).z);
					num8 = Mathf.Clamp((int)(vector.x * (float)num13), 0, num13 - 1);
					num = (int)(num2 * 338344161) ^ -918211234;
					continue;
				case 15u:
					dictionary.Add(prototypeIndex, new TreePrototypesData(_200D_206D_202A_206B_206C_200F_202B_206B_200E_200B_200F_200E_202A_206C_206F_200D_200D_202A_206C_206A_200F_200B_206B_200D_202E_202D_202D_202C_202B_206D_200C_202B_202B_200F_206C_206E_200E_206B_200F_206C_202E(_206F_202E_200B_206F_200C_202D_200B_200E_200E_206B_202C_202A_202A_206E_206E_202D_206F_200E_202A_200B_202D_206C_206E_206F_206B_202C_206C_206F_206F_206A_206E_206C_206A_202C_200E_200C_200C_206B_202C_202E_202E(P_0)[treeInstance.prototypeIndex]), treeInstance.prototypeIndex, position, vector2, one));
					num = ((int)num2 * -1236581901) ^ -427585183;
					continue;
				case 1u:
					goto IL_038c;
				case 10u:
				{
					int num9;
					int num10;
					if (array[num7, num8])
					{
						num9 = -347950415;
						num10 = num9;
					}
					else
					{
						num9 = -202392857;
						num10 = num9;
					}
					num = num9 ^ (int)(num2 * 236363797);
					continue;
				}
				case 13u:
					Utilities._200C_206C_200C_202B_200B_206E_206F_200C_200B_202C_206E_206D_206A_200F_202A_206F_200C_206B_206D_206D_206B_206D_206A_202E_200E_206F_206C_206F_206D_202A_200E_206F_202D_202D_206B_200B_200C_202D_202E_202E_202E(P_0, (P_1 - 1) * P_3, (P_2 - 1) * P_4, ref position, ref vector2);
					num = (int)((num2 * 1399913062) ^ 0x3F729494);
					continue;
				case 0u:
					return null;
				case 17u:
				{
					int num26;
					int num27;
					if (array != null)
					{
						num26 = -889596498;
						num27 = num26;
					}
					else
					{
						num26 = -1111007990;
						num27 = num26;
					}
					num = num26 ^ ((int)num2 * -1205048815);
					continue;
				}
				case 26u:
				{
					int num22;
					int num23;
					if (_206F_202E_200B_206F_200C_202D_200B_200E_200E_206B_202C_202A_202A_206E_206E_202D_206F_200E_202A_200B_202D_206C_206E_206F_206B_202C_206C_206F_206F_206A_206E_206C_206A_202C_200E_200C_200C_206B_202C_202E_202E(P_0).Length == 0)
					{
						num22 = -1173549016;
						num23 = num22;
					}
					else
					{
						num22 = -717505310;
						num23 = num22;
					}
					num = num22 ^ ((int)num2 * -1429949697);
					continue;
				}
				case 9u:
				{
					int num14;
					int num15;
					if (!_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)_200D_206D_202A_206B_206C_200F_202B_206B_200E_200B_200F_200E_202A_206C_206F_200D_200D_202A_206C_206A_200F_200B_206B_200D_202E_202D_202D_202C_202B_206D_200C_202B_202B_200F_206C_206E_200E_206B_200F_206C_202E(_206F_202E_200B_206F_200C_202D_200B_200E_200E_206B_202C_202A_202A_206E_206E_202D_206F_200E_202A_200B_202D_206C_206E_206F_206B_202C_206C_206F_206F_206A_206E_206C_206A_202C_200E_200C_200C_206B_202C_202E_202E(P_0)[treeInstance.prototypeIndex]), (UnityEngine.Object)null))
					{
						num14 = 1534379168;
						num15 = num14;
					}
					else
					{
						num14 = 2105238210;
						num15 = num14;
					}
					num = num14 ^ (int)(num2 * 2098746087);
					continue;
				}
				case 38u:
					position = treeInstance.position;
					num = (int)((num2 * 2110865340) ^ 0x184A24B0);
					continue;
				case 2u:
					goto IL_04d9;
				case 5u:
					num = (int)(num2 * 1703321025) ^ -1964230136;
					continue;
				case 4u:
					vector2 = _202A_202C_206C_200B_202B_206E_200C_206E_200B_202E_206A_206E_206D_206E_200B_200D_206D_206C_202A_200D_206E_200D_202B_206D_206B_200B_202C_206D_200B_200C_206E_202C_202E_206E_200E_200F_200C_206E_206D_206B_202E(P_0, position.x, position.z);
					num = -54658540;
					continue;
				case 22u:
					one.y *= treeInstance.heightScale * vector3.y;
					one.z *= treeInstance.widthScale * vector3.z;
					num = (int)(num2 * 1112531689) ^ -239956348;
					continue;
				case 8u:
					vector3 = _200E_202C_200B_202D_200E_206D_206D_200F_202D_200B_206A_202A_200B_206B_206F_206A_206B_206F_206E_206D_202C_200B_200C_200C_206B_202B_200F_200E_202C_206D_206C_200C_200F_200C_206C_206C_202D_206C_206E_202E(_200E_206C_206E_200C_202A_202C_202B_206E_206F_206E_202C_200C_200C_200D_200E_206E_202E_202D_206C_200B_200D_202A_202C_206E_202E_200D_206E_206B_202D_206B_202B_206B_202D_206A_202A_206A_206E_206C_206D_200C_202E(_200D_206D_202A_206B_206C_200F_202B_206B_200E_200B_200F_200E_202A_206C_206F_200D_200D_202A_206C_206A_200F_200B_206B_200D_202E_202D_202D_202C_202B_206D_200C_202B_202B_200F_206C_206E_200E_206B_200F_206C_202E(_206F_202E_200B_206F_200C_202D_200B_200E_200E_206B_202C_202A_202A_206E_206E_202D_206F_200E_202A_200B_202D_206C_206E_206F_206B_202C_206C_206F_206F_206A_206E_206C_206A_202C_200E_200C_200C_206B_202C_202E_202E(P_0)[treeInstance.prototypeIndex])));
					one.x *= treeInstance.widthScale * vector3.x;
					num = (int)(num2 * 1385487280) ^ -398253873;
					continue;
				case 31u:
				{
					int num11;
					int num12;
					if (_206F_206F_202B_200D_200E_202D_206D_200E_202D_206F_200B_202C_206A_200B_206F_206C_206E_202E_206E_206A_200C_202C_206B_206C_200B_206D_206C_200D_200E_200B_206B_206C_206A_206A_206A_202B_206E_206E_200E_202A_202E(P_0).Length != 0)
					{
						num11 = -644231516;
						num12 = num11;
					}
					else
					{
						num11 = -1842965694;
						num12 = num11;
					}
					num = num11 ^ ((int)num2 * -1758456900);
					continue;
				}
				case 34u:
					goto IL_05e4;
				case 33u:
					num7 = Mathf.Clamp((int)(vector.z * (float)num4), 0, num4 - 1);
					num = (int)(num2 * 606630022) ^ -52740787;
					continue;
				case 7u:
					goto IL_0625;
				case 3u:
				{
					int num5;
					int num6;
					if (num4 <= 1)
					{
						num5 = 568094990;
						num6 = num5;
					}
					else
					{
						num5 = 1810567638;
						num6 = num5;
					}
					num = num5 ^ (int)(num2 * 955782929);
					continue;
				}
				case 11u:
					return null;
				case 25u:
					position.x *= _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).x;
					num = (int)(num2 * 1789144) ^ -83947354;
					continue;
				case 20u:
					num3 = 0;
					num = -2071820658;
					continue;
				default:
					return dictionary.Values.ToArray();
				}
				break;
				IL_05e4:
				int num30;
				if (!P_7)
				{
					num = -1380618933;
					num30 = num;
				}
				else
				{
					num = -1990869637;
					num30 = num;
				}
				continue;
				IL_038c:
				dictionary = new Dictionary<int, TreePrototypesData>();
				array = _200B_206F_206C_200D_200F_206D_206F_200B_206F_206A_206B_206A_200F_206F_202B_206F_202C_206E_202D_200B_206C_206A_206B_206E_206F_200E_200F_202A_206E_206D_206E_202A_200E_202C_206F_200D_202D_200B_206B_200E_202E(P_0, 0, 0, _200D_206F_206C_206B_202E_206D_206F_200B_200C_206B_206B_206A_206A_202E_200D_206D_206B_200E_206C_206C_200F_202E_202A_200C_200C_200E_202D_202E_200E_202C_202D_206D_206E_206D_202B_200D_200D_206D_202A_202E(_206E_206A_200C_206A_202C_200E_202E_200B_200C_200D_202A_202E_200B_202C_200B_200C_200F_202D_206A_206C_200F_206B_200F_202E_202A_206F_202C_200F_200D_202E_206C_202A_206F_206A_206A_206B_200C_202C_206D_200F_202E(P_0)), _200D_202D_200C_202D_202E_202C_206A_200B_206C_202D_202C_200F_206F_200C_206B_206C_206E_200B_206D_200B_200B_206A_202B_206A_202E_206E_200B_200D_202B_206F_202B_202E_202D_206F_202D_202A_206C_206A_202C_202A_202E(_206E_206A_200C_206A_202C_200E_202E_200B_200C_200D_202A_202E_200B_202C_200B_200C_200F_202D_206A_206C_200F_206B_200F_202E_202A_206F_202C_200F_200D_202E_206C_202A_206F_206A_206A_206B_200C_202C_206D_200F_202E(P_0)));
				num13 = _206B_202D_202A_202C_202B_200C_202A_206E_200E_200C_200E_202E_202B_206E_202E_202E_202E_202C_200F_206D_202D_202E_206F_202B_206C_206C_200D_200F_200E_206C_200F_206F_206A_202C_202C_206F_206E_200B_206D_206E_202E((Array)array, 0);
				num4 = _206B_202D_202A_202C_202B_200C_202A_206E_200E_200C_200E_202E_202B_206E_202E_202E_202E_202C_200F_206D_202D_202E_206F_202B_206C_206C_200D_200F_200E_206C_200F_206F_206A_202C_202C_206F_206E_200B_206D_206E_202E((Array)array, 1);
				int num31;
				if (num13 <= 1)
				{
					num = -1109686259;
					num31 = num;
				}
				else
				{
					num = -934269286;
					num31 = num;
				}
				continue;
				IL_0103:
				treeInstance = _206F_206F_202B_200D_200E_202D_206D_200E_202D_206F_200B_202C_206A_200B_206F_206C_206E_202E_206E_206A_200C_202C_206B_206C_200B_206D_206C_200D_200E_200B_206B_206C_206A_206A_206A_202B_206E_206E_200E_202A_202E(P_0)[num3];
				int num32;
				if (_206F_202E_200B_206F_200C_202D_200B_200E_200E_206B_202C_202A_202A_206E_206E_202D_206F_200E_202A_200B_202D_206C_206E_206F_206B_202C_206C_206F_206F_206A_206E_206C_206A_202C_200E_200C_200C_206B_202C_202E_202E(P_0)[treeInstance.prototypeIndex] != null)
				{
					num = -1529371504;
					num32 = num;
				}
				else
				{
					num = -778376435;
					num32 = num;
				}
				continue;
				IL_04d9:
				vector2 = Vector3.up;
				int num33;
				if (!P_8)
				{
					num = -914443011;
					num33 = num;
				}
				else
				{
					num = -1647798492;
					num33 = num;
				}
				continue;
				IL_00c6:
				int num34;
				if (num3 >= _206F_206F_202B_200D_200E_202D_206D_200E_202D_206F_200B_202C_206A_200B_206F_206C_206E_202E_206E_206A_200C_202C_206B_206C_200B_206D_206C_200D_200E_200B_206B_206C_206A_206A_206A_202B_206E_206E_200E_202A_202E(P_0).Length)
				{
					num = -2113491751;
					num34 = num;
				}
				else
				{
					num = -2112136932;
					num34 = num;
				}
				continue;
				IL_024f:
				one = Vector3.one;
				int num35;
				if (_206C_206C_206A_202C_200C_200E_202A_202E_200B_206A_202D_200B_202B_206E_200E_202B_206C_200F_200B_200F_200E_206D_206B_200F_206C_202E_206F_206D_200E_202A_202D_200F_202E_200C_206A_200F_206A_200F_202E_206B_202E(_200E_206C_206E_200C_202A_202C_202B_206E_206F_206E_202C_200C_200C_200D_200E_206E_202E_202D_206C_200B_200D_202A_202C_206E_202E_200D_206E_206B_202D_206B_202B_206B_202D_206A_202A_206A_206E_206C_206D_200C_202E(_200D_206D_202A_206B_206C_200F_202B_206B_200E_200B_200F_200E_202A_206C_206F_200D_200D_202A_206C_206A_200F_200B_206B_200D_202E_202D_202D_202C_202B_206D_200C_202B_202B_200F_206C_206E_200E_206B_200F_206C_202E(_206F_202E_200B_206F_200C_202D_200B_200E_200E_206B_202C_202A_202A_206E_206E_202D_206F_200E_202A_200B_202D_206C_206E_206F_206B_202C_206C_206F_206F_206A_206E_206C_206A_202C_200E_200C_200C_206B_202C_202E_202E(P_0)[treeInstance.prototypeIndex]))) > 0)
				{
					num = -1390805883;
					num35 = num;
				}
				else
				{
					num = -335266574;
					num35 = num;
				}
			}
			goto IL_000b;
			IL_0625:
			Utilities.Debug(LogType.Warning, _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E("'", _200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)P_0), "' has no trees to export"), null, P_0);
			num = -181972063;
			goto IL_0010;
		}

		public DetailPrototypesData[] ExportGrassData(int maxCountPerPatch, float countMultiplier)
		{
			return _200D_202C_206B_206E_200B_202D_202C_202A_202A_206A_200E_202E_200E_206B_200D_200E_202C_202A_202A_206D_202D_202C_202E_206D_202C_206B_200E_202D_200E_200D_206F_206E_202C_206C_200D_206E_200D_202D_202B_206A_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, true, 2, 2, 1, 1, 0, 0, false, false, maxCountPerPatch, countMultiplier);
		}

		public DetailPrototypesData[] ExportGrassData(int vertexCountHorizontal, int vertexCountVertical, int chunkCountHorizontal, int chunkCountVertical, int maxCountPerPatch, float countMultiplier)
		{
			return _200D_202C_206B_206E_200B_202D_202C_202A_202A_206A_200E_202E_200E_206B_200D_200E_202C_202A_202A_206D_202D_202C_202E_206D_202C_206B_200E_202D_200E_200D_206F_206E_202C_206C_200D_206E_200D_202D_202B_206A_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, true, vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, 0, 0, false, true, maxCountPerPatch, countMultiplier);
		}

		public DetailPrototypesData[] ExportGrassData(int vertexCountHorizontal, int vertexCountVertical, int chunkCountHorizontal, int chunkCountVertical, int positionX, int positionY, int maxCountPerPatch, float countMultiplier)
		{
			return _200D_202C_206B_206E_200B_202D_202C_202A_202A_206A_200E_202E_200E_206B_200D_200E_202C_202A_202A_206D_202D_202C_202E_206D_202C_206B_200E_202D_200E_200D_206F_206E_202C_206C_200D_206E_200D_202D_202B_206A_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, true, vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, positionX, positionY, true, true, maxCountPerPatch, countMultiplier);
		}

		public DetailPrototypesData[] ExportDetailMeshData(int maxCountPerPatch, float countMultiplier)
		{
			return _200D_202C_206B_206E_200B_202D_202C_202A_202A_206A_200E_202E_200E_206B_200D_200E_202C_202A_202A_206D_202D_202C_202E_206D_202C_206B_200E_202D_200E_200D_206F_206E_202C_206C_200D_206E_200D_202D_202B_206A_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, false, 2, 2, 1, 1, 0, 0, false, false, maxCountPerPatch, countMultiplier);
		}

		public DetailPrototypesData[] ExportDetailMeshData(int vertexCountHorizontal, int vertexCountVertical, int chunkCountHorizontal, int chunkCountVertical, int maxCountPerPatch, float countMultiplier)
		{
			return _200D_202C_206B_206E_200B_202D_202C_202A_202A_206A_200E_202E_200E_206B_200D_200E_202C_202A_202A_206D_202D_202C_202E_206D_202C_206B_200E_202D_200E_200D_206F_206E_202C_206C_200D_206E_200D_202D_202B_206A_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, false, vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, 0, 0, false, true, maxCountPerPatch, countMultiplier);
		}

		public DetailPrototypesData[] ExportDetailMeshData(int vertexCountHorizontal, int vertexCountVertical, int chunkCountHorizontal, int chunkCountVertical, int positionX, int positionY, int maxCountPerPatch, float countMultiplier)
		{
			return _200D_202C_206B_206E_200B_202D_202C_202A_202A_206A_200E_202E_200E_206B_200D_200E_202C_202A_202A_206D_202D_202C_202E_206D_202C_206B_200E_202D_200E_200D_206F_206E_202C_206C_200D_206E_200D_202D_202B_206A_202E(_202A_206C_200E_206D_202D_202D_202D_200F_202B_206D_202A_206A_202D_206C_206C_200D_202D_202B_200B_200F_202D_202C_206B_202C_200B_200C_206C_200E_202A_200E_200C_202D_206D_202C_206F_200B_206E_202A_202A_200F_202E, false, vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, positionX, positionY, true, true, maxCountPerPatch, countMultiplier);
		}

		private static DetailPrototypesData[] _200D_202C_206B_206E_200B_202D_202C_202A_202A_206A_200E_202E_200E_206B_200D_200E_202C_202A_202A_206D_202D_202C_202E_206D_202C_206B_200E_202D_200E_200D_206F_206E_202C_206C_200D_206E_200D_202D_202B_206A_202E(TerrainData P_0, bool P_1, int P_2, int P_3, int P_4, int P_5, int P_6, int P_7, bool P_8, bool P_9, int P_10, float P_11)
		{
			P_10 = Mathf.Clamp(P_10, 1, 64);
			if (P_11 <= 0f)
			{
				goto IL_0018;
			}
			goto IL_01e8;
			IL_0018:
			int num = -1827498537;
			goto IL_001d;
			IL_001d:
			int[,] array2 = default(int[,]);
			Dictionary<int, DetailPrototypesData> dictionary = default(Dictionary<int, DetailPrototypesData>);
			Vector3 vector3 = default(Vector3);
			Vector3 vector = default(Vector3);
			int num35 = default(int);
			int num7 = default(int);
			int num32 = default(int);
			DetailPrototype detailPrototype = default(DetailPrototype);
			int num18 = default(int);
			float num5 = default(float);
			float num4 = default(float);
			int num6 = default(int);
			int[] array3 = default(int[]);
			int num42 = default(int);
			int num21 = default(int);
			bool[,] array = default(bool[,]);
			Vector3 vector2 = default(Vector3);
			int num3 = default(int);
			int num52 = default(int);
			int num43 = default(int);
			int num8 = default(int);
			int num17 = default(int);
			float num19 = default(float);
			while (true)
			{
				uint num2;
				float num29;
				switch ((num2 = (uint)num ^ 0xAD984911u) % 81)
				{
				case 15u:
					break;
				case 20u:
				{
					int num13;
					int num14;
					if (_200F_200D_202A_202E_200B_206C_200D_206C_206C_206A_200D_200F_206A_200B_206C_206C_200B_206E_200C_206B_206A_202B_202A_206C_200D_206B_202E_202B_200C_202D_200E_202E_206F_206F_202D_200F_200F_200F_206D_206C_202E((Array)array2) <= 0)
					{
						num13 = 1458634261;
						num14 = num13;
					}
					else
					{
						num13 = 1009479359;
						num14 = num13;
					}
					num = num13 ^ (int)(num2 * 1749737558);
					continue;
				}
				case 64u:
					num = ((int)num2 * -959213783) ^ 0x5DCF6BBB;
					continue;
				case 33u:
				{
					int num25;
					int num26;
					if (_200E_206B_202A_206E_202B_206A_202A_206E_206F_200D_200D_206D_206F_200B_206B_206E_202D_206A_206B_200F_206B_206A_206B_206E_206B_206D_206A_206D_202E_202B_206E_206D_200D_202E_206E_202D_200E_206D_206D_202E_202E() <= P_11)
					{
						num25 = -41404632;
						num26 = num25;
					}
					else
					{
						num25 = -1997191578;
						num26 = num25;
					}
					num = num25 ^ ((int)num2 * -2019656187);
					continue;
				}
				case 49u:
					dictionary = new Dictionary<int, DetailPrototypesData>();
					num = -578498488;
					continue;
				case 36u:
					goto IL_01e8;
				case 61u:
					Utilities.Debug(LogType.Warning, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[5]
					{
						"'",
						_200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)P_0),
						"' has no ",
						P_1 ? "grass" : "detail mesh",
						" to export."
					}), null, P_0);
					num = -1811484674;
					continue;
				case 72u:
					Utilities._200C_206C_200C_202B_200B_206E_206F_200C_200B_202C_206E_206D_206A_200F_202A_206F_200C_206B_206D_206D_206B_206D_206A_202E_200E_206F_206C_206F_206D_202A_200E_206F_202D_202D_206B_200B_200C_202D_202E_202E_202E(P_0, (P_2 - 1) * P_4, (P_3 - 1) * P_5, ref vector3, ref vector);
					num = (int)(num2 * 1999174267) ^ -1296860027;
					continue;
				case 51u:
				{
					int num15;
					int num16;
					if (P_11 >= 1f)
					{
						num15 = 304385070;
						num16 = num15;
					}
					else
					{
						num15 = 1598410622;
						num16 = num15;
					}
					num = num15 ^ ((int)num2 * -1903982055);
					continue;
				}
				case 0u:
					goto IL_02a4;
				case 14u:
				{
					int num50;
					int num51;
					if (!Utilities._200D_200F_202C_202D_206B_206B_202C_200D_202C_202A_200C_200C_202D_206B_200C_202B_202D_202B_200B_206D_206E_202B_206C_200D_200E_202B_200D_200B_202D_206F_206B_202E_206C_206E_202C_202D_206B_200B_202E_206D_202E(P_0, vector3, P_4, P_5, P_6, P_7))
					{
						num50 = -1701309898;
						num51 = num50;
					}
					else
					{
						num50 = -735042447;
						num51 = num50;
					}
					num = num50 ^ ((int)num2 * -94000038);
					continue;
				}
				case 74u:
					num35++;
					num = -501068077;
					continue;
				case 48u:
					num7 = 0;
					num = -173109368;
					continue;
				case 43u:
					dictionary.Add(num32, new DetailPrototypesData(detailPrototype, num32, vector3, vector));
					num = (int)(num2 * 235450326) ^ -1546921782;
					continue;
				case 54u:
					goto IL_0333;
				case 63u:
					detailPrototype = _202A_200C_206A_206D_200B_200C_202C_200F_206F_206E_206E_202B_202C_200C_200E_206D_206C_202D_206D_206B_206B_200D_202C_200D_200F_206C_200F_200E_202C_202C_202B_202C_202A_200F_202C_202C_202B_202D_202B_202D_202E(P_0)[num32];
					num = (int)(num2 * 1749368847) ^ -391492845;
					continue;
				case 73u:
					num18 = Mathf.FloorToInt(P_11);
					num = (int)((num2 * 1814772941) ^ 0x4EBB4F96);
					continue;
				case 1u:
					num = ((int)num2 * -1348103277) ^ 0x1F1B83FF;
					continue;
				case 3u:
					vector3 = new Vector3(num5 * (float)num7, 0f, num4 * (float)num6);
					vector3.x += _200E_206B_202A_206E_202B_206A_202A_206E_206F_200D_200D_206D_206F_200B_206B_206E_202D_206A_206B_200F_206B_206A_206B_206E_206B_206D_206A_206D_202E_202B_206E_206D_200D_202E_206E_202D_200E_206D_206D_202E_202E() * num5;
					num = -1138404584;
					continue;
				case 80u:
					goto IL_03c7;
				case 44u:
					num6++;
					num = (int)(num2 * 77179496) ^ -1597902978;
					continue;
				case 41u:
					num4 = _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).x / (float)_206E_200D_206F_200C_206E_202C_200C_202B_202A_202A_200C_206B_206B_202D_202C_206F_200F_202E_200B_202C_202A_202E_200D_202A_202B_206D_202A_206F_200D_200B_200E_206B_202B_206D_200F_202C_202C_200B_202E_206B_202E(P_0);
					num = ((int)num2 * -497418581) ^ -1861422718;
					continue;
				case 22u:
					goto IL_0446;
				case 60u:
					num = ((int)num2 * -1544512482) ^ 0x567D18B8;
					continue;
				case 31u:
					Utilities.Debug(LogType.Warning, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[5]
					{
						"'",
						_200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)P_0),
						"' has no ",
						P_1 ? "grass" : "detail mesh",
						" to export."
					}), null, P_0);
					num = -481298954;
					continue;
				case 67u:
					goto IL_04c4;
				case 28u:
					goto IL_04de;
				case 78u:
				{
					int num55;
					int num56;
					if (array3.Length != 0)
					{
						num55 = 233498292;
						num56 = num55;
					}
					else
					{
						num55 = 469322861;
						num56 = num55;
					}
					num = num55 ^ (int)(num2 * 885970299);
					continue;
				}
				case 40u:
					num4 *= _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).z / _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).x;
					num5 /= _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).z / _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).x;
					num = (int)(num2 * 1999015262) ^ -1280771025;
					continue;
				case 13u:
				{
					int num46;
					int num47;
					if (_200B_200D_200D_206E_202C_200B_200F_200B_202D_206B_200D_200B_202D_202C_200E_202B_200F_202B_202B_202A_206B_200F_202A_200F_202D_202A_206D_202C_202E_206E_206D_206D_200F_206C_202E_206C_202E_206D_200E_206E_202E(P_0) >= 1)
					{
						num46 = 1127202357;
						num47 = num46;
					}
					else
					{
						num46 = 1805479280;
						num47 = num46;
					}
					num = num46 ^ ((int)num2 * -1714064175);
					continue;
				}
				case 65u:
				{
					int num40;
					int num41;
					if (detailPrototype != null)
					{
						num40 = 1592024974;
						num41 = num40;
					}
					else
					{
						num40 = 1789364793;
						num41 = num40;
					}
					num = num40 ^ ((int)num2 * -804320684);
					continue;
				}
				case 12u:
					num42++;
					num = -1454202336;
					continue;
				case 11u:
				{
					int num33;
					int num34;
					if (num21 > 1)
					{
						num33 = 1673159853;
						num34 = num33;
					}
					else
					{
						num33 = 63835760;
						num34 = num33;
					}
					num = num33 ^ ((int)num2 * -1748554547);
					continue;
				}
				case 34u:
					array2 = _200B_206A_206A_206E_200F_206D_206A_206D_206C_202B_200B_206B_206E_202E_206C_206A_200F_202C_200D_200F_202A_200D_206B_206F_206F_206A_200D_200F_202D_206A_202E_200D_202B_200F_200E_200E_206E_200C_202B_200D_202E(P_0, 0, 0, _200C_200E_200D_202D_206D_202C_200F_200F_206F_206F_200B_206C_202E_200C_200C_206E_202E_202A_206D_200B_206D_206D_202A_206D_200F_206E_202D_200E_206E_206D_200C_200F_200F_206F_206E_200E_200C_202D_206A_202C_202E(P_0), _200B_200D_200D_206E_202C_200B_200F_200B_202D_206B_200D_200B_202D_202C_200E_202B_200F_202B_202B_202A_206B_200F_202A_200F_202D_202A_206D_202C_202E_206E_206D_206D_200F_206C_202E_206C_202E_206D_200E_206E_202E(P_0), num32);
					num = -1157980859;
					continue;
				case 52u:
					num21 = _206B_202D_202A_202C_202B_200C_202A_206E_200E_200C_200E_202E_202B_206E_202E_202E_202E_202C_200F_206D_202D_202E_206F_202B_206C_206C_200D_200F_200E_206C_200F_206F_206A_202C_202C_206F_206E_200B_206D_206E_202E((Array)array, 1);
					num = ((int)num2 * -1945343685) ^ -552685399;
					continue;
				case 45u:
					return null;
				case 68u:
					vector2 = vector3;
					num = ((int)num2 * -957130429) ^ 0x29E5653D;
					continue;
				case 32u:
				{
					vector2.z = Mathf.Clamp01(vector2.z / _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).z);
					int num20 = Mathf.Clamp((int)(vector2.x * (float)num3), 0, num3 - 1);
					int num22 = Mathf.Clamp((int)(vector2.z * (float)num21), 0, num21 - 1);
					int num23;
					int num24;
					if (!array[num22, num20])
					{
						num23 = -784676398;
						num24 = num23;
					}
					else
					{
						num23 = -1731941008;
						num24 = num23;
					}
					num = num23 ^ ((int)num2 * -942869209);
					continue;
				}
				case 5u:
				{
					int num11;
					int num12;
					if (!P_1)
					{
						num11 = -788761777;
						num12 = num11;
					}
					else
					{
						num11 = -1776389571;
						num12 = num11;
					}
					num = num11 ^ (int)(num2 * 325358508);
					continue;
				}
				case 66u:
				{
					int num57;
					int num58;
					if (P_9)
					{
						num57 = 1036813730;
						num58 = num57;
					}
					else
					{
						num57 = 1484499239;
						num58 = num57;
					}
					num = num57 ^ (int)(num2 * 1916906318);
					continue;
				}
				case 76u:
					_206E_206E_200F_206C_200E_202A_206B_200E_206A_200E_206A_202E_200F_200E_206F_200D_200D_206E_202B_202D_200B_202D_202E_202A_206D_202B_200E_202C_202A_206D_202B_206F_206E_206F_200D_206E_206A_200E_206D_202A_202E(0);
					num = -1567232653;
					continue;
				case 35u:
					array = null;
					num = -1267784504;
					continue;
				case 53u:
				{
					int num53;
					int num54;
					if (!_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)_200F_200D_202E_200E_202A_202E_206E_200E_200F_200D_200B_206A_202C_200D_202D_206F_200C_200B_206A_202C_206A_206E_206D_200C_202B_206C_202B_206F_202C_206C_202D_202C_206D_202B_202D_200C_200C_206C_202D_206A_202E(detailPrototype), (UnityEngine.Object)null))
					{
						num53 = 1736603091;
						num54 = num53;
					}
					else
					{
						num53 = 572447881;
						num54 = num53;
					}
					num = num53 ^ (int)(num2 * 298882023);
					continue;
				}
				case 16u:
					num = (int)((num2 * 749976798) ^ 0x31430F2C);
					continue;
				case 71u:
					num52 = _206B_202D_202A_202C_202B_200C_202A_206E_200E_200C_200E_202E_202B_206E_202E_202E_202E_202C_200F_206D_202D_202E_206F_202B_206C_206C_200D_200F_200E_206C_200F_206F_206A_202C_202C_206F_206E_200B_206D_206E_202E((Array)array2, 1);
					num = ((int)num2 * -1033961696) ^ 0x117592C;
					continue;
				case 7u:
					dictionary[num32].Add(vector3, vector);
					num = -1769099918;
					continue;
				case 24u:
				{
					int num48;
					int num49;
					if (!_202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E((UnityEngine.Object)_200D_202E_206E_206C_200B_202C_200C_202C_200E_206E_202B_202E_202E_206D_202E_206C_202D_206C_202D_206B_206F_200B_202E_200B_206B_206A_202A_200E_206A_200F_206F_206C_200D_200E_200C_206A_206E_200D_200B_206B_202E(detailPrototype), (UnityEngine.Object)null))
					{
						num48 = -1257585933;
						num49 = num48;
					}
					else
					{
						num48 = -1396555681;
						num49 = num48;
					}
					num = num48 ^ ((int)num2 * -958168534);
					continue;
				}
				case 79u:
				{
					int num44;
					int num45;
					if (array != null)
					{
						num44 = 1256111239;
						num45 = num44;
					}
					else
					{
						num44 = 1266013594;
						num45 = num44;
					}
					num = num44 ^ ((int)num2 * -1132721942);
					continue;
				}
				case 18u:
					num29 = P_11 - (float)num18;
					goto IL_07e9;
				case 19u:
					return null;
				case 27u:
					Utilities.Debug(LogType.Warning, _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(new string[5]
					{
						"'",
						_200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E((UnityEngine.Object)P_0),
						"' grass count multiplier '",
						P_11.ToString(),
						"' can not be less or equal to zero."
					}), null, P_0);
					num = ((int)num2 * -142770563) ^ 0x779C5B8;
					continue;
				case 56u:
					num32 = array3[num42];
					num = -943445019;
					continue;
				case 21u:
					num43 = _206B_202D_202A_202C_202B_200C_202A_206E_200E_200C_200E_202E_202B_206E_202E_202E_202E_202C_200F_206D_202D_202E_206F_202B_206C_206C_200D_200F_200E_206C_200F_206F_206A_202C_202C_206F_206E_200B_206D_206E_202E((Array)array2, 0);
					num = (int)(num2 * 1335997843) ^ -1814716473;
					continue;
				case 55u:
				{
					num5 = _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).z / (float)_206E_200D_206F_200C_206E_202C_200C_202B_202A_202A_200C_206B_206B_202D_202C_206F_200F_202E_200B_202C_202A_202E_200D_202A_202B_206D_202A_206F_200D_200B_200E_206B_202B_206D_200F_202C_202C_200B_202E_206B_202E(P_0);
					int num38;
					int num39;
					if (_206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).x <= _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).z)
					{
						num38 = 2066153801;
						num39 = num38;
					}
					else
					{
						num38 = 781963385;
						num39 = num38;
					}
					num = num38 ^ (int)(num2 * 1959060478);
					continue;
				}
				case 70u:
					num = (int)(num2 * 1908403224) ^ -1374856178;
					continue;
				case 37u:
				{
					int num36;
					int num37;
					if (num8 > 0)
					{
						num36 = -1468680015;
						num37 = num36;
					}
					else
					{
						num36 = -239752648;
						num37 = num36;
					}
					num = num36 ^ (int)(num2 * 767330715);
					continue;
				}
				case 46u:
					num7++;
					num = -753886334;
					continue;
				case 38u:
					vector = _202A_202C_206C_200B_202B_206E_200C_206E_200B_202E_206A_206E_206D_206E_200B_200D_206D_206C_202A_200D_206E_200D_202B_206D_206B_200B_202C_206D_200B_200C_206E_202C_202E_206E_200E_200F_200C_206E_206D_206B_202E(P_0, vector3.x, vector3.z);
					num = -637055368;
					continue;
				case 39u:
					num17 = Mathf.Clamp(num8, 1, P_10);
					num = (int)((num2 * 1966230331) ^ 0x5F3FDBD8);
					continue;
				case 23u:
					return null;
				case 42u:
					num35 = 0;
					num = -948689351;
					continue;
				case 57u:
					num6 = 0;
					num = (int)(num2 * 270855955) ^ -1484588023;
					continue;
				case 17u:
				{
					int num30;
					int num31;
					if (dictionary.Count != 0)
					{
						num30 = -679800823;
						num31 = num30;
					}
					else
					{
						num30 = -2092528436;
						num31 = num30;
					}
					num = num30 ^ ((int)num2 * -1859783058);
					continue;
				}
				case 62u:
					if (P_11 <= 1f)
					{
						num29 = 0f;
						goto IL_07e9;
					}
					num = (int)((num2 * 975996646) ^ 0x43398444);
					continue;
				case 30u:
				{
					int num27;
					int num28;
					if (array2 == null)
					{
						num27 = -2098145299;
						num28 = num27;
					}
					else
					{
						num27 = -619303213;
						num28 = num27;
					}
					num = num27 ^ (int)(num2 * 844763783);
					continue;
				}
				case 25u:
					goto IL_09f6;
				case 26u:
					num17 = num17 * num18 + num17 * ((_200E_206B_202A_206E_202B_206A_202A_206E_206F_200D_200D_206D_206F_200B_206B_206E_202D_206A_206B_200F_206B_206A_206B_206E_206B_206D_206A_206D_202E_202B_206E_206D_200D_202E_206E_202D_200E_206D_206D_202E_202E() <= num19) ? 1 : 0);
					num = -1743991488;
					continue;
				case 4u:
				{
					int num9;
					int num10;
					if (num3 > 1)
					{
						num9 = 1987404160;
						num10 = num9;
					}
					else
					{
						num9 = 1414727549;
						num10 = num9;
					}
					num = num9 ^ ((int)num2 * -201178072);
					continue;
				}
				case 29u:
					num = ((int)num2 * -237887157) ^ 0x4E079B69;
					continue;
				case 10u:
					num = ((int)num2 * -1748606693) ^ 0x526F05E4;
					continue;
				case 58u:
					vector3.z += _200E_206B_202A_206E_202B_206A_202A_206E_206F_200D_200D_206D_206F_200B_206B_206E_202D_206A_206B_200F_206B_206A_206B_206E_206B_206D_206A_206D_202E_202B_206E_206D_200D_202E_206E_202D_200E_206D_206D_202E_202E() * num4;
					num = (int)((num2 * 235449015) ^ 0x58F6338F);
					continue;
				case 59u:
					num8 = array2[num6, num7];
					num = -1435389545;
					continue;
				case 2u:
					num4 /= _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).x / _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).z;
					num5 *= _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).x / _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).z;
					num = -2066588806;
					continue;
				case 77u:
					num3 = _206B_202D_202A_202C_202B_200C_202A_206E_200E_200C_200E_202E_202B_206E_202E_202E_202E_202C_200F_206D_202D_202E_206F_202B_206C_206C_200D_200F_200E_206C_200F_206F_206A_202C_202C_206F_206E_200B_206D_206E_202E((Array)array, 0);
					num = ((int)num2 * -915784111) ^ 0x16A2189;
					continue;
				case 9u:
					return null;
				case 8u:
					vector2.x = Mathf.Clamp01(vector2.x / _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(P_0).x);
					num = (int)(num2 * 348904314) ^ -517684031;
					continue;
				case 6u:
					goto IL_0b55;
				case 75u:
					vector = Vector3.up;
					num = -1415280763;
					continue;
				case 47u:
					goto IL_0b7d;
				case 69u:
					goto IL_0ba9;
				default:
					{
						return dictionary.Values.ToArray();
					}
					IL_07e9:
					num19 = num29;
					num42 = 0;
					num = -1141649878;
					continue;
				}
				break;
				IL_0ba9:
				int num59;
				if (dictionary.ContainsKey(num32))
				{
					num = -292901346;
					num59 = num;
				}
				else
				{
					num = -120701819;
					num59 = num;
				}
				continue;
				IL_0446:
				int num60;
				if (num42 < array3.Length)
				{
					num = -51079038;
					num60 = num;
				}
				else
				{
					num = -416816404;
					num60 = num;
				}
				continue;
				IL_04de:
				int num61;
				if (num35 < num17)
				{
					num = -437895737;
					num61 = num;
				}
				else
				{
					num = -526088934;
					num61 = num;
				}
				continue;
				IL_0b7d:
				array3 = _200B_206D_206B_202C_206B_200E_206B_202A_200B_200D_206E_206D_206D_202C_202B_200D_206F_202D_200D_202E_202E_206A_206C_200E_202C_206D_206E_200F_200F_200B_206B_202B_202A_206C_202A_200B_206F_206E_200F_200D_202E(P_0, 0, 0, _200C_200E_200D_202D_206D_202C_200F_200F_206F_206F_200B_206C_202E_200C_200C_206E_202E_202A_206D_200B_206D_206D_202A_206D_200F_206E_202D_200E_206E_206D_200C_200F_200F_206F_206E_200E_200C_202D_206A_202C_202E(P_0), _200B_200D_200D_206E_202C_200B_200F_200B_202D_206B_200D_200B_202D_202C_200E_202B_200F_202B_202B_202A_206B_200F_202A_200F_202D_202A_206D_202C_202E_206E_206D_206D_200F_206C_202E_206C_202E_206D_200E_206E_202E(P_0));
				int num62;
				if (array3 != null)
				{
					num = -940787634;
					num62 = num;
				}
				else
				{
					num = -493924408;
					num62 = num;
				}
				continue;
				IL_03e8:
				object obj;
				array = (bool[,])obj;
				num3 = 1;
				num21 = 1;
				int num63;
				if (array == null)
				{
					num = -1267784504;
					num63 = num;
				}
				else
				{
					num = -8857670;
					num63 = num;
				}
				continue;
				IL_02a4:
				int num64;
				if (num6 < num43)
				{
					num = -679819093;
					num64 = num;
				}
				else
				{
					num = -681363551;
					num64 = num;
				}
				continue;
				IL_0b55:
				int num65;
				if (!P_1)
				{
					num = -1523053358;
					num65 = num;
				}
				else
				{
					num = -827607795;
					num65 = num;
				}
				continue;
				IL_04c4:
				int num66;
				if (num7 >= num52)
				{
					num = -565843602;
					num66 = num;
				}
				else
				{
					num = -840607381;
					num66 = num;
				}
				continue;
				IL_03c7:
				obj = null;
				goto IL_03e8;
				IL_09f6:
				int num67;
				if (!P_8)
				{
					num = -668624075;
					num67 = num;
				}
				else
				{
					num = -654910341;
					num67 = num;
				}
				continue;
				IL_0333:
				if (!Utilities.HasHoles(P_0))
				{
					num = -254673891;
					continue;
				}
				obj = _200B_206F_206C_200D_200F_206D_206F_200B_206F_206A_206B_206A_200F_206F_202B_206F_202C_206E_202D_200B_206C_206A_206B_206E_206F_200E_200F_202A_206E_206D_206E_202A_200E_202C_206F_200D_202D_200B_206B_200E_202E(P_0, 0, 0, _200D_206F_206C_206B_202E_206D_206F_200B_200C_206B_206B_206A_206A_202E_200D_206D_206B_200E_206C_206C_200F_202E_202A_200C_200C_200E_202D_202E_200E_202C_202D_206D_206E_206D_202B_200D_200D_206D_202A_202E(_206E_206A_200C_206A_202C_200E_202E_200B_200C_200D_202A_202E_200B_202C_200B_200C_200F_202D_206A_206C_200F_206B_200F_202E_202A_206F_202C_200F_200D_202E_206C_202A_206F_206A_206A_206B_200C_202C_206D_200F_202E(P_0)), _200D_202D_200C_202D_202E_202C_206A_200B_206C_202D_202C_200F_206F_200C_206B_206C_206E_200B_206D_200B_200B_206A_202B_206A_202E_206E_200B_200D_202B_206F_202B_202E_202D_206F_202D_202A_206C_206A_202C_202A_202E(_206E_206A_200C_206A_202C_200E_202E_200B_200C_200D_202A_202E_200B_202C_200B_200C_200F_202D_206A_206C_200F_206B_200F_202E_202A_206F_202C_200F_200D_202E_206C_202A_206F_206A_206A_206B_200C_202C_206D_200F_202E(P_0)));
				goto IL_03e8;
			}
			goto IL_0018;
			IL_01e8:
			int num68;
			if (_200C_200E_200D_202D_206D_202C_200F_200F_206F_206F_200B_206C_202E_200C_200C_206E_202E_202A_206D_200B_206D_206D_202A_206D_200F_206E_202D_200E_206E_206D_200C_200F_200F_206F_206E_200E_200C_202D_206A_202C_202E(P_0) >= 1)
			{
				num = -1797552266;
				num68 = num;
			}
			else
			{
				num = -1358412953;
				num68 = num;
			}
			goto IL_001d;
		}

		static Vector3 _206E_202E_206F_202E_206A_206A_206D_200B_200C_206C_200B_202D_206A_202A_200C_206D_202A_206B_206E_206E_202E_200F_206F_206B_206E_206E_200C_206F_206C_200E_202E_206F_202B_202C_200C_200E_206A_206D_206F_202E_202E(TerrainData P_0)
		{
			return P_0.size;
		}

		static object _202B_202B_206D_202D_206B_206F_200F_200D_200D_202E_206B_200B_200F_206E_200F_200D_202C_206D_202C_202D_206D_200B_202E_200B_200D_200D_202D_202C_200C_206E_206D_206A_206C_200F_202D_206A_200C_200E_206A_206A_202E(Array P_0)
		{
			return P_0.Clone();
		}

		static float _202C_200F_200E_200E_200D_200D_206D_200F_200E_200E_200E_202D_200B_206A_206A_200C_202D_206F_200E_202C_202C_200C_206A_202B_206A_202E_200F_202D_206E_206C_200F_206E_200D_202D_200B_202D_200D_200B_200D_206F_202E(TerrainData P_0, float P_1, float P_2)
		{
			return P_0.GetInterpolatedHeight(P_1, P_2);
		}

		static Mesh _206E_206B_206F_206A_206B_206C_200D_200B_206B_202C_206B_200D_206E_200F_202B_200F_206A_206E_206B_206B_202D_206C_202D_200C_202C_202D_206B_200B_200E_202E_202B_202C_206D_200C_200C_202B_200E_202B_202D_200E_202E()
		{
			return new Mesh();
		}

		static void _206D_200F_202E_200C_206E_206D_200B_200C_202B_206D_200D_202A_206A_206D_200B_206F_206D_206A_202D_206B_200B_202C_200E_200F_206A_206D_206E_202A_202A_206F_200F_206E_202A_206A_206C_202C_200F_202B_202B_200F_202E(Mesh P_0, IndexFormat P_1)
		{
			P_0.indexFormat = P_1;
		}

		static void _206E_200C_206F_200B_206C_202A_206B_202A_206A_206B_200E_200F_202D_202E_202C_202C_202E_202B_206D_202B_206D_206E_200C_200E_202C_200F_202D_206D_206F_200F_206A_200B_202D_206B_200C_200F_200D_200D_206C_206B_202E(UnityEngine.Object P_0, string P_1)
		{
			P_0.name = P_1;
		}

		static void _206C_200D_206C_202A_202B_202D_200E_206D_206C_202E_200E_200B_206F_206D_206E_202B_206C_200D_202D_200B_200D_202C_200E_202C_206E_206C_206A_200D_200B_206C_202C_200F_200C_206C_202D_206B_200D_202C_202B_206D_202E(Mesh P_0, Vector3[] P_1)
		{
			P_0.vertices = P_1;
		}

		static void _200E_200F_200D_202C_202C_206B_202B_200C_202C_200D_200F_200C_206F_200C_200E_206E_200B_200F_206E_200E_202A_202A_200C_200D_206F_206B_206A_200D_200E_206A_206A_202C_202E_202A_200C_200F_206E_200C_206F_202D_202E(Mesh P_0, int[] P_1)
		{
			P_0.triangles = P_1;
		}

		static void _206B_200E_206F_206A_202A_202D_200D_200D_200B_202E_206B_202A_200D_206D_202E_206A_202E_200E_206F_206E_202D_200E_202E_206F_202D_202C_202A_206D_206D_206A_200B_202D_202C_200C_206B_206E_206E_200D_206B_200D_202E(Mesh P_0, Vector2[] P_1)
		{
			P_0.uv = P_1;
		}

		static void _202C_202E_202D_202E_206B_202B_202B_200F_202C_200E_206D_202E_200E_206E_206C_206F_202A_206F_202E_202E_200F_200F_200D_206E_202E_202E_200E_202B_200C_206A_200C_200D_206C_206F_206E_200C_206D_206A_200F_200C_202E(Mesh P_0)
		{
			P_0.RecalculateNormals();
		}

		static void _206D_206C_206D_202C_202D_206E_200B_206A_200C_200B_206D_200D_206D_200E_206D_200D_206A_200B_206A_206F_200D_202A_206C_206B_200F_202A_202C_200B_200F_206F_202E_206D_202B_200F_200C_200B_206F_206B_206E_202A_202E(Mesh P_0)
		{
			P_0.RecalculateTangents();
		}

		static Vector3 _202A_202C_206C_200B_202B_206E_200C_206E_200B_202E_206A_206E_206D_206E_200B_200D_206D_206C_202A_200D_206E_200D_202B_206D_206B_200B_202C_206D_200B_200C_206E_202C_202E_206E_200E_200F_200C_206E_206D_206B_202E(TerrainData P_0, float P_1, float P_2)
		{
			return P_0.GetInterpolatedNormal(P_1, P_2);
		}

		static void _206E_200B_206C_206F_202D_202B_202E_206A_202A_202B_200B_206C_202E_202C_202B_202E_202B_202B_202C_206E_200E_200B_202B_206B_202A_206B_202C_206C_200D_206D_202E_200D_202C_200E_206C_206D_202E_200C_200D_206C_202E(Mesh P_0, Vector3[] P_1)
		{
			P_0.normals = P_1;
		}

		static Vector3[] _206D_200E_206A_200B_202E_202B_200C_202D_200D_206B_200B_202D_200C_206A_202B_202A_200F_200E_200E_200E_200B_206F_200D_202E_206E_206A_200C_206E_206E_202C_202B_202C_202C_202E_200C_206D_206A_206E_206B_206F_202E(Mesh P_0)
		{
			return P_0.normals;
		}

		static Vector4[] _206D_202C_200C_202B_206C_206C_202E_206D_206E_206C_202D_206A_200B_202E_200F_202D_206A_206F_206E_200F_206D_206F_200B_206A_206A_200F_200E_202E_202B_200C_202A_200B_200C_206C_206E_206D_200C_202B_206A_200F_202E(Mesh P_0)
		{
			return P_0.tangents;
		}

		static void _202C_202E_200F_206B_202B_202D_202C_206D_206D_200B_200E_200C_200B_202C_206A_206F_202C_200C_200F_200C_206F_200C_202B_206E_206D_202A_206D_206C_202E_202B_206D_206F_206A_200C_202B_206C_206C_200E_206D_200E_202E(Mesh P_0, bool P_1)
		{
			P_0.Clear(P_1);
		}

		static void _206F_206B_206C_202D_206B_200F_202B_206B_206C_202E_200D_206A_200F_202D_200E_206C_206E_200F_206F_206E_206C_206A_200B_200F_206E_202E_202A_206C_206A_200B_202D_206C_200D_202A_200B_202C_202B_202A_200C_200E_202E(Mesh P_0, Vector4[] P_1)
		{
			P_0.tangents = P_1;
		}

		static Vector3[] _202A_206C_206E_200D_206F_200D_200E_202B_206A_202B_206A_206F_202B_206D_200F_206C_206E_200D_206B_206B_200C_202E_200E_202C_202B_200E_200F_200E_206A_206D_206A_200C_200E_206B_206E_206E_202D_206E_202A_202C_202E(Mesh P_0)
		{
			return P_0.vertices;
		}

		static int[] _202B_200C_202E_200E_206E_206E_206A_206B_202C_200D_206B_202D_202E_202C_200B_200B_206A_200F_206E_206E_200F_200F_200E_200F_202D_206B_206E_206B_200C_200D_200E_206B_202A_200D_206B_200D_202E_202C_206E_202A_202E(Mesh P_0)
		{
			return P_0.triangles;
		}

		static Vector2[] _206D_206F_202E_206E_206C_202C_200E_202C_202A_202A_200D_206C_200D_206F_202E_200E_206F_206E_200E_206A_202C_206E_202B_202D_202C_200D_202B_202B_200C_200B_200D_206A_206F_202D_206E_206B_200F_202C_202E_206A_202E(Mesh P_0)
		{
			return P_0.uv;
		}

		static void _202A_202D_200D_200B_200C_202D_206D_202C_206F_202D_206F_202B_206D_202E_202A_200D_202A_200D_200D_202A_200E_202A_202B_200D_200E_206E_200B_200F_206B_202C_202E_200E_206C_202E_200B_202E_206B_202D_200B_200E_202E(Mesh P_0)
		{
			P_0.Clear();
		}

		static void _202D_200E_206E_202A_206D_206C_206B_206D_200E_200C_206F_206C_206E_200F_200E_206B_200F_206E_202E_200D_200D_202C_206A_200B_206D_206A_200B_206E_200C_200D_206A_200D_206D_200C_206C_206C_202E_206F_206C_206F_202E(Mesh P_0, int P_1)
		{
			P_0.subMeshCount = P_1;
		}

		static void _200F_206E_206B_202C_202A_206A_206F_202A_206C_200F_202C_200C_202D_202B_200B_200F_202E_202E_202A_202E_202B_202E_206A_202A_206B_206D_200C_202E_202D_200C_206C_202E_202C_202A_202E_206D_202B_206E_206D_200F_202E(Mesh P_0, int[] P_1, int P_2)
		{
			P_0.SetTriangles(P_1, P_2);
		}

		static string _206F_200D_200B_200D_202A_206B_200B_200F_202A_200B_200C_200B_200E_202A_202D_206F_206D_206B_200D_200E_202B_206F_206D_202C_206F_200E_200E_202A_200F_202A_202A_200F_200D_200F_206A_202C_202E_206D_202B_202E(string P_0, string P_1, string P_2)
		{
			return P_0 + P_1 + P_2;
		}

		static int _206B_202B_200B_206C_202B_206C_200E_200F_200F_206B_206A_200B_202B_200D_200B_200D_202B_200F_200E_206C_206E_200F_200B_202E_206E_206E_202E_206D_202E_206A_200E_202B_202A_200D_206A_206D_202B_206A_206C_206F_202E(object P_0)
		{
			return P_0.GetHashCode();
		}

		static string _206F_206F_200F_206E_202E_200C_206B_200F_202C_206A_206F_206A_206F_200D_206D_202B_202C_206D_206D_206C_200F_206C_200C_206F_206D_200E_206F_206F_200E_206F_206A_202E_206A_202E_200D_206C_202C_200D_200C_202B_202E(string[] P_0)
		{
			return string.Concat(P_0);
		}

		static bool _202B_206C_200F_202E_206B_200C_206E_200B_206A_202C_206E_200F_206A_206F_206C_206F_200F_206B_202B_206B_206D_206C_202B_200C_206A_200B_206B_206E_202D_200B_200F_202B_200C_200E_200E_206E_202D_206D_200B_202A_202E(UnityEngine.Object P_0, UnityEngine.Object P_1)
		{
			return P_0 == P_1;
		}

		static string _200D_200C_200F_202A_202E_202D_206D_202C_202D_206D_206A_202B_206A_200C_206C_200C_202E_200D_202B_200B_206B_202C_200D_200B_206E_200C_206A_206E_206B_200F_200E_206B_202E_202B_200D_200F_200C_206D_206C_206E_202E(UnityEngine.Object P_0)
		{
			return P_0.name;
		}

		static bool _200F_202D_202D_206E_200B_206A_206B_202B_206E_206F_206A_206E_206A_200F_200F_206F_206A_200B_200B_202E_200D_202B_206A_206A_200E_202E_206C_202A_200F_206E_202C_202B_200E_200F_206F_200E_200E_202C_202A_206F_202E()
		{
			return SystemInfo.supports32bitsIndexBuffer;
		}

		static bool _202D_202E_206C_202B_202A_200E_202A_200F_206C_202B_200D_202E_206B_206C_206B_202D_206C_202D_200B_206E_200F_200D_202A_202E_200C_200C_202B_200E_202B_202E_200C_202C_206C_200B_200D_206F_202D_202D_200F_202C_202E(string P_0)
		{
			return string.IsNullOrEmpty(P_0);
		}

		static string _202C_200F_200D_206C_206D_206B_200E_200B_206E_200C_202E_206D_200F_202D_202B_206B_200B_202A_200B_206E_200B_206B_200D_200F_202C_206B_206E_202E_202D_206A_206B_206E_200E_202D_206C_202A_200B_206C_206F_202A_202E(string P_0)
		{
			return P_0.Trim();
		}

		static int _206D_200E_206D_206B_202D_200C_200C_202C_202A_202A_200C_202B_202C_202A_206A_206E_206A_206D_202B_206E_206E_200C_202A_200D_206B_200C_202E_200C_200D_206A_206F_200E_200C_202C_206A_202D_202B_202B_206C_202B_202E(UnityEngine.Object P_0)
		{
			return P_0.GetInstanceID();
		}

		static string _206A_202B_200D_202B_206E_200F_200C_202E_202C_200E_202E_200B_200C_200E_202A_200F_206D_206F_202C_206A_200B_206A_206B_206C_200D_202C_202E_200B_202A_200E_206F_206C_200D_202A_200E_202D_200E_202D_206B_206C_202E(string P_0, string P_1)
		{
			return P_0 + P_1;
		}

		static string _202D_202B_206A_200F_200F_202A_200E_200D_202E_200C_202E_202E_202D_206F_206D_200C_200B_202A_206A_206B_200C_200F_200D_206A_200E_206E_202C_200D_206A_202B_206E_206F_206F_200B_202A_200E_206F_202E_200C_200C_202E(string P_0, object P_1, object P_2)
		{
			return string.Format(P_0, P_1, P_2);
		}

		static bool _202C_202E_202B_202E_206E_206A_206E_206B_202C_200C_202A_200E_206B_206A_202A_206C_200E_206C_200B_200B_202C_200D_200B_206A_206D_206C_206B_200B_202B_200C_200D_202D_206D_206F_200C_206B_202C_206D_206F_202A_202E()
		{
			return Application.isEditor;
		}

		static void _202E_200D_202B_202C_206A_206B_206C_206D_206A_202E_206F_206B_206F_202B_200C_200E_202E_206D_202D_202A_200C_202C_200D_202A_206C_202E_206C_202E_206C_206B_200F_200D_206C_202A_202E_202A_202E_206C_200E_206C_202E(UnityEngine.Object P_0)
		{
			UnityEngine.Object.DestroyImmediate(P_0);
		}

		static void _206F_200C_200C_202A_200D_202B_202E_206F_206A_206B_202C_202E_206A_200E_200E_206B_206D_206F_206D_206C_200B_206C_206C_206C_202B_202A_200F_200E_206A_202B_202E_202A_200B_202C_200B_206B_202E_206C_202C_206D_202E(UnityEngine.Object P_0)
		{
			UnityEngine.Object.Destroy(P_0);
		}

		static Texture _206E_206A_200C_206A_202C_200E_202E_200B_200C_200D_202A_202E_200B_202C_200B_200C_200F_202D_206A_206C_200F_206B_200F_202E_202A_206F_202C_200F_200D_202E_206C_202A_206F_206A_206A_206B_200C_202C_206D_200F_202E(TerrainData P_0)
		{
			return P_0.holesTexture;
		}

		static Texture2D _202C_202D_206D_200E_200B_200F_200F_202A_202B_206B_202B_200D_202E_202A_206C_206A_200B_202E_206B_206F_200D_206F_202C_206A_206B_200C_206B_206A_202D_206D_206C_206B_206A_202B_200C_206B_206B_206E_202D_202E()
		{
			return Texture2D.whiteTexture;
		}

		static bool _206B_206A_202B_206E_200C_200F_202B_206B_200C_200C_206A_202C_206E_206B_202C_200D_206D_206C_206A_206F_206B_202B_206A_200B_206C_202E_200D_202C_206D_206C_202D_206B_200E_200D_202D_206B_200B_206A_200D_200E_202E(UnityEngine.Object P_0, UnityEngine.Object P_1)
		{
			return P_0 != P_1;
		}

		static Shader _200B_200F_206A_200E_206A_202B_202D_206E_206D_200D_200F_206C_202E_200E_202E_206A_200C_200F_202E_206F_200B_206B_202B_206E_206A_200B_206D_206E_202D_200C_200C_206A_202B_200B_200E_202E_206D_200B_206C_202E(string P_0)
		{
			return Shader.Find(P_0);
		}

		static Material _202B_206D_206E_200D_202C_202C_202E_200D_200D_200F_206A_206B_206F_206B_206F_202D_202A_206B_202A_202A_206D_200B_200F_206F_202B_206C_200E_200F_202D_200B_200E_200E_202E_200D_202E_202A_200C_206E_200C_200D_202E(Shader P_0)
		{
			return new Material(P_0);
		}

		static void _202B_202E_206D_200E_202E_200E_200D_202B_200E_206E_206E_200B_202C_200E_206E_200C_206F_202C_200F_206D_200C_202E_206B_206B_202C_206F_202E_202D_202C_206F_200D_202E_200E_202C_202B_206A_200E_206C_200D_202B_202E(Material P_0, string P_1, Texture P_2)
		{
			P_0.SetTexture(P_1, P_2);
		}

		static int _200D_206F_206C_206B_202E_206D_206F_200B_200C_206B_206B_206A_206A_202E_200D_206D_206B_200E_206C_206C_200F_202E_202A_200C_200C_200E_202D_202E_200E_202C_202D_206D_206E_206D_202B_200D_200D_206D_202A_202E(Texture P_0)
		{
			return P_0.width;
		}

		static int _200D_202D_200C_202D_202E_202C_206A_200B_206C_202D_202C_200F_206F_200C_206B_206C_206E_200B_206D_200B_200B_206A_202B_206A_202E_206E_200B_200D_202B_206F_202B_202E_202D_206F_202D_202A_206C_206A_202C_202A_202E(Texture P_0)
		{
			return P_0.height;
		}

		static RenderTexture _202B_206A_200E_200E_200F_202B_200C_202B_206A_206B_200E_200D_200B_202A_200B_200D_202E_202D_200B_206C_202C_202D_202E_200F_206B_206A_200E_206C_206C_202B_202E_206D_202E_202B_200E_202C_200D_202C_206E_202C_202E(int P_0, int P_1)
		{
			return RenderTexture.GetTemporary(P_0, P_1);
		}

		static bool _206E_206F_200B_202A_200C_202D_200D_206E_206C_200F_202D_206E_206D_206C_202B_200C_202B_200F_202E_202B_206D_206D_202B_206D_200F_202C_202B_202D_202D_206B_202A_200D_202B_202E_206C_202B_206F_202E_206D_200D_202E(RenderTexture P_0)
		{
			return P_0.Create();
		}

		static void _202B_200C_200B_202A_200C_206B_206A_206C_202B_200F_202E_206B_202E_202A_200D_206E_200C_200D_206E_206B_206B_202E_206D_200D_202C_206F_206A_202A_206B_200C_202B_202C_206B_200B_206E_200F_206E_206D_200B_200D_202E(Texture P_0, RenderTexture P_1, Material P_2, int P_3)
		{
			Graphics.Blit(P_0, P_1, P_2, P_3);
		}

		static RenderTexture _206C_206B_202B_206D_202D_202D_206A_202B_202A_200B_202C_206B_202D_206F_200E_206F_206D_206D_206C_206E_206E_206A_206E_206C_200B_206C_202C_206A_206B_200B_202E_202D_206F_200F_202B_200E_200E_200C_206E_202B_202E()
		{
			return RenderTexture.active;
		}

		static void _200F_202D_206C_202B_200E_206C_206D_206A_202E_206E_200C_200E_202E_206D_202A_202C_206F_202A_206C_200C_202B_200D_202A_202E_206A_206B_206D_206F_206E_206E_206D_200F_200F_206F_206B_200F_200F_200D_200C_200D_202E(RenderTexture P_0)
		{
			RenderTexture.active = P_0;
		}

		static void _202A_202D_206F_206B_200F_202A_202C_200D_202A_200D_206E_202E_202A_200D_202A_202D_202C_206E_200F_200B_206D_200E_200E_202A_200E_202D_200D_202C_200F_202B_206C_202B_206B_206E_200F_200E_200D_206C_206F_206E_202E(Texture2D P_0, Rect P_1, int P_2, int P_3)
		{
			P_0.ReadPixels(P_1, P_2, P_3);
		}

		static void _200B_206C_202C_200D_200D_200C_202D_202E_206C_202B_200B_206D_200C_202C_206F_202C_202E_206D_206D_200E_206A_200E_206D_200F_206E_206E_206A_202B_206E_206D_202B_202B_200F_200F_206B_200D_202B_202D_200F_202E_202E(Texture2D P_0)
		{
			P_0.Apply();
		}

		static bool _206E_202A_200C_200B_206D_200D_206D_206B_206B_202C_202E_200C_206A_202E_206E_206F_206A_206C_200C_200B_200C_200F_206A_206F_202D_206C_206B_200D_200C_202D_202B_206F_202B_200B_202E_200E_206C_200B_206E_200C_202E()
		{
			return Application.isPlaying;
		}

		static void _206E_202D_206B_202E_202B_202B_206E_202A_200D_206E_202B_206E_200D_202B_206F_200B_200C_202E_202D_206E_200B_200D_202E_206F_206A_202C_200D_202E_206F_206E_206F_206E_200D_206E_206E_206C_202D_206A_202E(RenderTexture P_0)
		{
			RenderTexture.ReleaseTemporary(P_0);
		}

		static Texture2D[] _200D_200B_202E_200C_202D_200F_202A_200D_206B_200E_206C_206C_206E_200B_202E_202D_202D_200D_202E_202E_200C_206E_200E_202A_200D_200E_200E_200C_206D_206B_206B_206B_206A_206A_200E_202A_206D_200F_202E_202B_202E(TerrainData P_0)
		{
			return P_0.alphamapTextures;
		}

		static void _202A_206D_206E_200C_206A_206B_206A_200B_202C_206C_200D_202E_202D_202B_202E_206C_206A_206D_206C_206D_206F_206D_206B_206F_202B_206C_200D_200C_206B_202B_206F_200C_200C_206C_200E_200F_202B_202C_200F_200C_202E(Material P_0, string P_1)
		{
			P_0.EnableKeyword(P_1);
		}

		static void _202A_200E_200C_206C_200F_206E_202C_200E_206C_200D_206E_206F_202D_206E_206B_202C_206B_200D_206F_202A_202D_206A_206E_200F_200F_206A_200F_202D_206F_202B_206C_200C_200D_202B_202B_202E_206F_200D_202D_200F_202E(Material P_0, string P_1, float P_2)
		{
			P_0.SetFloat(P_1, P_2);
		}

		static Texture2D _202B_200E_206D_206D_200D_202A_206B_200E_202A_206A_206E_200D_202B_206B_202B_206F_206F_206B_200E_200D_202A_206E_200C_200E_200B_200B_202C_200F_202D_202B_200E_202D_206A_206F_206C_206D_206F_206C_206A_206F_202E(TerrainLayer P_0)
		{
			return P_0.diffuseTexture;
		}

		static Texture2D _200E_206A_206D_202C_200B_206E_200F_206A_206F_202E_206F_206D_200F_202A_200E_202C_206B_202E_200F_206D_202D_206D_202C_202B_200B_206F_200B_200B_200E_202B_202B_200B_200F_202E_202A_202C_206B_206F_206D_200C_202E(TerrainLayer P_0)
		{
			return P_0.normalMapTexture;
		}

		static float _206A_202E_202B_202A_206D_200F_200F_200B_202A_202E_206E_200B_206A_200F_200F_202E_200B_200F_202E_202B_206B_200E_206F_202D_206E_206C_206A_202E_206A_206F_200C_206E_200E_206F_206D_206A_200D_200D_200E_200F_202E(TerrainLayer P_0)
		{
			return P_0.normalScale;
		}

		static string _202E_200D_206C_206C_206B_206A_202D_200F_202E_206F_200E_206B_202A_206F_206D_202B_206D_202E_206D_202C_206E_206D_202E_206C_200D_206C_206B_200F_200E_206B_200C_202A_202A_206C_206C_206E_206F_206F_206E_206C_202E(string P_0, object P_1)
		{
			return string.Format(P_0, P_1);
		}

		static Texture2D _206D_202D_202E_206C_206F_206F_200D_206A_202E_200B_206F_206D_202C_206B_200B_200D_206A_206A_200E_202A_202A_200D_202E_202A_200B_200E_202C_206D_206B_202A_206E_202C_200B_200F_200C_200C_202C_200B_200E_202D_202E(TerrainLayer P_0)
		{
			return P_0.maskMapTexture;
		}

		static float _200B_202A_200F_200C_202E_200E_206D_200F_206F_200B_206F_206E_206A_200E_206A_200C_200B_202E_202E_206F_202B_202D_202E_206D_202E_206A_202C_200E_202B_206E_206D_202E_202D_206C_206D_206B_206E_206A_200D_206F_202E(TerrainLayer P_0)
		{
			return P_0.metallic;
		}

		static Vector4 _202E_200E_202C_202B_200E_206B_206D_202C_200B_202A_206F_202B_206C_200F_206B_202D_200C_200D_206C_200F_200F_206A_206B_202D_200D_200E_200F_206A_200C_202E_200E_202A_206F_202A_206C_206C_200C_200F_200E_202C_202E(TerrainLayer P_0)
		{
			return P_0.maskMapRemapMax;
		}

		static float _200D_202C_202B_206F_200D_200D_200F_202A_200F_206A_200F_206B_200F_206F_206B_206D_206E_206C_200F_202B_206D_206B_200D_202D_200C_202D_200C_200F_202D_202D_200F_202E_206A_200F_200F_206F_206B_206E_202B_202C_202E(TerrainLayer P_0)
		{
			return P_0.smoothness;
		}

		static void _202B_202C_200E_200B_206F_202B_200E_202C_206C_200B_200D_202C_202D_206B_202E_202D_200D_200D_206A_206B_206F_206A_206D_200D_206A_206D_206D_206E_206A_200B_200D_206D_206D_200C_202C_206B_202A_202B_200E_206F_202E(Material P_0, string P_1, Vector4 P_2)
		{
			P_0.SetVector(P_1, P_2);
		}

		static Vector4 _202B_206A_206F_206E_202C_206E_200B_206C_202C_202A_200C_206D_206E_200B_202B_206B_206B_206A_202E_202E_200D_206E_200C_202C_200E_200F_202E_202E_202E_206A_206E_200C_206E_206B_200E_202B_200D_202E_202B_202A_202E(TerrainLayer P_0)
		{
			return P_0.maskMapRemapMin;
		}

		static Vector2 _200F_200E_206D_206F_200E_206B_202E_202D_202E_202A_206B_206D_206E_206B_202E_206B_200D_202A_206D_206A_200F_200C_206C_202B_202D_202A_206F_200D_202A_202C_206F_206E_200D_202D_202C_200F_206E_202A_206A_206D_202E(TerrainLayer P_0)
		{
			return P_0.tileSize;
		}

		static Vector2 _200C_200C_206B_202E_200D_202E_206A_202C_200E_206B_206E_206E_206B_200B_206D_202B_202C_206F_206A_206E_202C_202A_202D_200E_202D_206A_206F_202A_206A_200B_202C_200F_200D_206A_200E_202C_206B_200D_206D_202E_202E(TerrainLayer P_0)
		{
			return P_0.tileOffset;
		}

		static RenderPipelineAsset _206E_206B_206E_206B_206A_202E_206E_202C_206D_202E_206C_206C_202C_200D_206E_206F_206B_200F_202B_202C_200E_206E_206A_200D_206A_206E_202A_200C_206B_202D_202C_202A_202B_202C_200D_206D_202C_206E_206A_202C_202E()
		{
			return GraphicsSettings.renderPipelineAsset;
		}

		static Vector4 _200D_200C_202B_202E_202D_200E_206D_206F_202B_206C_202B_206E_202C_200B_200F_200E_206A_200D_206F_202B_206A_206A_200B_200D_202D_202B_200E_202A_206F_206E_200C_202D_206E_206F_206A_202D_206F_200D_206E_200C_202E(TerrainLayer P_0)
		{
			return P_0.diffuseRemapMax;
		}

		static ColorSpace _206E_200F_200D_206C_200D_200C_206B_200C_206F_202D_206D_200C_206F_206D_202C_206E_200F_206D_206D_200F_206E_206C_200B_206F_202B_206B_206D_200F_206F_206A_206A_200C_200B_206D_206D_206D_202B_200E_202E_206D_202E()
		{
			return QualitySettings.activeColorSpace;
		}

		static void _206F_206E_200E_202E_200F_200B_206B_200B_200E_206D_202B_202B_206F_200B_206C_202D_202E_202E_202B_200C_200B_206E_200E_206A_200D_200C_200B_206E_206B_206C_202D_202A_206A_206C_206B_202E_202D_206C_206B_202C_202E(Material P_0, string P_1, Color P_2)
		{
			P_0.SetColor(P_1, P_2);
		}

		static TreePrototype[] _206F_202E_200B_206F_200C_202D_200B_200E_200E_206B_202C_202A_202A_206E_206E_202D_206F_200E_202A_200B_202D_206C_206E_206F_206B_202C_206C_206F_206F_206A_206E_206C_206A_202C_200E_200C_200C_206B_202C_202E_202E(TerrainData P_0)
		{
			return P_0.treePrototypes;
		}

		static TreeInstance[] _206F_206F_202B_200D_200E_202D_206D_200E_202D_206F_200B_202C_206A_200B_206F_206C_206E_202E_206E_206A_200C_202C_206B_206C_200B_206D_206C_200D_200E_200B_206B_206C_206A_206A_206A_202B_206E_206E_200E_202A_202E(TerrainData P_0)
		{
			return P_0.treeInstances;
		}

		static bool[,] _200B_206F_206C_200D_200F_206D_206F_200B_206F_206A_206B_206A_200F_206F_202B_206F_202C_206E_202D_200B_206C_206A_206B_206E_206F_200E_200F_202A_206E_206D_206E_202A_200E_202C_206F_200D_202D_200B_206B_200E_202E(TerrainData P_0, int P_1, int P_2, int P_3, int P_4)
		{
			return P_0.GetHoles(P_1, P_2, P_3, P_4);
		}

		static int _206B_202D_202A_202C_202B_200C_202A_206E_200E_200C_200E_202E_202B_206E_202E_202E_202E_202C_200F_206D_202D_202E_206F_202B_206C_206C_200D_200F_200E_206C_200F_206F_206A_202C_202C_206F_206E_200B_206D_206E_202E(Array P_0, int P_1)
		{
			return P_0.GetLength(P_1);
		}

		static GameObject _200D_206D_202A_206B_206C_200F_202B_206B_200E_200B_200F_200E_202A_206C_206F_200D_200D_202A_206C_206A_200F_200B_206B_200D_202E_202D_202D_202C_202B_206D_200C_202B_202B_200F_206C_206E_200E_206B_200F_206C_202E(TreePrototype P_0)
		{
			return P_0.prefab;
		}

		static Transform _200E_206C_206E_200C_202A_202C_202B_206E_206F_206E_202C_200C_200C_200D_200E_206E_202E_202D_206C_200B_200D_202A_202C_206E_202E_200D_206E_206B_202D_206B_202B_206B_202D_206A_202A_206A_206E_206C_206D_200C_202E(GameObject P_0)
		{
			return P_0.transform;
		}

		static int _206C_206C_206A_202C_200C_200E_202A_202E_200B_206A_202D_200B_202B_206E_200E_202B_206C_200F_200B_200F_200E_206D_206B_200F_206C_202E_206F_206D_200E_202A_202D_200F_202E_200C_206A_200F_206A_200F_202E_206B_202E(Transform P_0)
		{
			return P_0.childCount;
		}

		static Vector3 _200E_202C_200B_202D_200E_206D_206D_200F_202D_200B_206A_202A_200B_206B_206F_206A_206B_206F_206E_206D_202C_200B_200C_200C_206B_202B_200F_200E_202C_206D_206C_200C_200F_200C_206C_206C_202D_206C_206E_202E(Transform P_0)
		{
			return P_0.localScale;
		}

		static int _200C_200E_200D_202D_206D_202C_200F_200F_206F_206F_200B_206C_202E_200C_200C_206E_202E_202A_206D_200B_206D_206D_202A_206D_200F_206E_202D_200E_206E_206D_200C_200F_200F_206F_206E_200E_200C_202D_206A_202C_202E(TerrainData P_0)
		{
			return P_0.detailWidth;
		}

		static int _200B_200D_200D_206E_202C_200B_200F_200B_202D_206B_200D_200B_202D_202C_200E_202B_200F_202B_202B_202A_206B_200F_202A_200F_202D_202A_206D_202C_202E_206E_206D_206D_200F_206C_202E_206C_202E_206D_200E_206E_202E(TerrainData P_0)
		{
			return P_0.detailHeight;
		}

		static int[] _200B_206D_206B_202C_206B_200E_206B_202A_200B_200D_206E_206D_206D_202C_202B_200D_206F_202D_200D_202E_202E_206A_206C_200E_202C_206D_206E_200F_200F_200B_206B_202B_202A_206C_202A_200B_206F_206E_200F_200D_202E(TerrainData P_0, int P_1, int P_2, int P_3, int P_4)
		{
			return P_0.GetSupportedLayers(P_1, P_2, P_3, P_4);
		}

		static void _206E_206E_200F_206C_200E_202A_206B_200E_206A_200E_206A_202E_200F_200E_206F_200D_200D_206E_202B_202D_200B_202D_202E_202A_206D_202B_200E_202C_202A_206D_202B_206F_206E_206F_200D_206E_206A_200E_206D_202A_202E(int P_0)
		{
			UnityEngine.Random.InitState(P_0);
		}

		static int _206E_200D_206F_200C_206E_202C_200C_202B_202A_202A_200C_206B_206B_202D_202C_206F_200F_202E_200B_202C_202A_202E_200D_202A_202B_206D_202A_206F_200D_200B_200E_206B_202B_206D_200F_202C_202C_200B_202E_206B_202E(TerrainData P_0)
		{
			return P_0.detailResolution;
		}

		static DetailPrototype[] _202A_200C_206A_206D_200B_200C_202C_200F_206F_206E_206E_202B_202C_200C_200E_206D_206C_202D_206D_206B_206B_200D_202C_200D_200F_206C_200F_200E_202C_202C_202B_202C_202A_200F_202C_202C_202B_202D_202B_202D_202E(TerrainData P_0)
		{
			return P_0.detailPrototypes;
		}

		static Texture2D _200F_200D_202E_200E_202A_202E_206E_200E_200F_200D_200B_206A_202C_200D_202D_206F_200C_200B_206A_202C_206A_206E_206D_200C_202B_206C_202B_206F_202C_206C_202D_202C_206D_202B_202D_200C_200C_206C_202D_206A_202E(DetailPrototype P_0)
		{
			return P_0.prototypeTexture;
		}

		static GameObject _200D_202E_206E_206C_200B_202C_200C_202C_200E_206E_202B_202E_202E_206D_202E_206C_202D_206C_202D_206B_206F_200B_202E_200B_206B_206A_202A_200E_206A_200F_206F_206C_200D_200E_200C_206A_206E_200D_200B_206B_202E(DetailPrototype P_0)
		{
			return P_0.prototype;
		}

		static int[,] _200B_206A_206A_206E_200F_206D_206A_206D_206C_202B_200B_206B_206E_202E_206C_206A_200F_202C_200D_200F_202A_200D_206B_206F_206F_206A_200D_200F_202D_206A_202E_200D_202B_200F_200E_200E_206E_200C_202B_200D_202E(TerrainData P_0, int P_1, int P_2, int P_3, int P_4, int P_5)
		{
			return P_0.GetDetailLayer(P_1, P_2, P_3, P_4, P_5);
		}

		static int _200F_200D_202A_202E_200B_206C_200D_206C_206C_206A_200D_200F_206A_200B_206C_206C_200B_206E_200C_206B_206A_202B_202A_206C_200D_206B_202E_202B_200C_202D_200E_202E_206F_206F_202D_200F_200F_200F_206D_206C_202E(Array P_0)
		{
			return P_0.Length;
		}

		static float _200E_206B_202A_206E_202B_206A_202A_206E_206F_200D_200D_206D_206F_200B_206B_206E_202D_206A_206B_200F_206B_206A_206B_206E_206B_206D_206A_206D_202E_202B_206E_206D_200D_202E_206E_202D_200E_206D_206D_202E_202E()
		{
			return UnityEngine.Random.value;
		}
	}
}
