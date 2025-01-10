using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace AmazingAssets.TerrainToMesh
{
	public static class Utilities
	{
		public enum RenderPipeline
		{
			Builtin = 0,
			Universal = 1,
			HighDefinition = 2
		}

		public static RenderPipeline GetCurrentRenderPipeline()
		{
			if (_206B_202B_202E_206A_202E_206E_206F_202C_200B_202C_202C_202A_200F_202E_200C_206E_202B_200C_200F_206B_202B_200C_200D_202C_206F_200D_200D_206C_202A_206C_206E_202A_200F_200F_200B_206F_202A_202B_206A_202B_202E((UnityEngine.Object)_202D_200F_200D_202B_202B_206A_202B_206B_206E_202E_202E_202E_206C_200C_200F_202E_202B_200C_206B_206A_202D_206A_206B_202D_206E_200D_202D_206C_206A_202E_200B_202A_200F_200F_202B_206F_202B_200B_206C_202E_202E(), (UnityEngine.Object)null))
			{
				goto IL_000d;
			}
			goto IL_005c;
			IL_000d:
			int num = 2007372912;
			goto IL_0012;
			IL_0012:
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0x7F23F709u) % 7)
				{
				case 6u:
					break;
				case 1u:
					return RenderPipeline.Builtin;
				case 4u:
					return RenderPipeline.Universal;
				case 3u:
					goto IL_005c;
				case 0u:
				{
					int num5;
					int num6;
					if (!_200D_206A_200F_200F_200E_202D_202E_200C_200D_200E_206C_202E_202D_202E_206B_200E_200F_206B_200E_206B_206D_200D_202D_206A_202C_206F_206B_200B_206F_206D_200B_206C_202D_202D_200F_202A_206E_206A_202B_202E_202E(_200C_200C_202D_202C_206F_200F_202B_200E_206C_202A_202C_200E_200D_200D_200F_206B_202C_200D_200D_206D_206A_202A_206E_200E_202B_202D_206D_206A_206D_206C_206D_202B_206C_202B_202A_200D_202E_200E_202C_200C_202E((UnityEngine.Object)_202D_200F_200D_202B_202B_206A_202B_206B_206E_202E_202E_202E_206C_200C_200F_202E_202B_200C_206B_206A_202D_206A_206B_202D_206E_200D_202D_206C_206A_202E_200B_202A_200F_200F_202B_206F_202B_200B_206C_202E_202E()), "URP"))
					{
						num5 = -1375438108;
						num6 = num5;
					}
					else
					{
						num5 = -651888824;
						num6 = num5;
					}
					num = num5 ^ (int)(num2 * 2039207087);
					continue;
				}
				case 2u:
				{
					int num3;
					int num4;
					if (_202C_206C_206F_202A_206D_206C_202C_200F_200F_202A_202E_202A_202D_202B_206B_200C_200B_206C_200E_206B_200B_206B_206D_206F_202B_200B_206F_206F_206C_206D_200B_200C_202D_202E_202E_202C_200D_206D_202A_200C_202E((UnityEngine.Object)_202A_202E_206F_206F_202E_200E_202A_206C_200D_202C_206E_206C_206B_202B_206B_202E_200D_200F_206C_202B_202B_206D_202B_200E_200F_200E_202C_200D_200C_200C_206B_200E_206C_202A_206F_206C_202E_202E_202D_206E_202E("Universal Render Pipeline/Lit"), (UnityEngine.Object)null))
					{
						num3 = -1246761953;
						num4 = num3;
					}
					else
					{
						num3 = -175686172;
						num4 = num3;
					}
					num = num3 ^ ((int)num2 * -1796407562);
					continue;
				}
				default:
					return RenderPipeline.HighDefinition;
				}
				break;
			}
			goto IL_000d;
			IL_005c:
			int num7;
			if (_200D_206A_200F_200F_200E_202D_202E_200C_200D_200E_206C_202E_202D_202E_206B_200E_200F_206B_200E_206B_206D_200D_202D_206A_202C_206F_206B_200B_206F_206D_200B_206C_202D_202D_200F_202A_206E_206A_202B_202E_202E(_200C_200C_202D_202C_206F_200F_202B_200E_206C_202A_202C_200E_200D_200D_200F_206B_202C_200D_200D_206D_206A_202A_206E_200E_202B_202D_206D_206A_206D_206C_206D_202B_206C_202B_202A_200D_202E_200E_202C_200C_202E((UnityEngine.Object)_202D_200F_200D_202B_202B_206A_202B_206B_206E_202E_202E_202E_206C_200C_200F_202E_202B_200C_206B_206A_202D_206A_206B_202D_206E_200D_202D_206C_206A_202E_200B_202A_200F_200F_202B_206F_202B_200B_206C_202E_202E()), "Universal"))
			{
				num = 1953051371;
				num7 = num;
			}
			else
			{
				num = 500741188;
				num7 = num;
			}
			goto IL_0012;
		}

		public static int GetRequiredVertexCount(int vertexCountHorizontal, int vertexCountVertical, int chunkCountHorizontal, int chunkCountVertical, bool hasEdgeFall)
		{
			if (vertexCountHorizontal >= 2)
			{
				goto IL_0007;
			}
			int num = 2;
			goto IL_010d;
			IL_0147:
			int num2 = vertexCountVertical;
			goto IL_014b;
			IL_0007:
			int num3 = -931539615;
			goto IL_000c;
			IL_000c:
			int num5 = default(int);
			while (true)
			{
				uint num4;
				int num6;
				int num9;
				switch ((num4 = (uint)num3 ^ 0x8C03B3E0u) % 16)
				{
				case 0u:
					break;
				case 9u:
					goto IL_0061;
				case 15u:
					vertexCountHorizontal += 2;
					vertexCountVertical += 2;
					num3 = ((int)num4 * -2092306441) ^ -1348526417;
					continue;
				case 2u:
					if (chunkCountHorizontal >= 1)
					{
						num3 = ((int)num4 * -777358529) ^ 0x330803DB;
						continue;
					}
					num6 = 1;
					goto IL_0121;
				case 10u:
					num9 = chunkCountVertical;
					goto IL_00af;
				case 4u:
				{
					int num7;
					int num8;
					if (hasEdgeFall)
					{
						num7 = -1646522988;
						num8 = num7;
					}
					else
					{
						num7 = -2105618336;
						num8 = num7;
					}
					num3 = num7 ^ ((int)num4 * -470721498);
					continue;
				}
				case 14u:
					num3 = (int)(num4 * 274096331) ^ -265843661;
					continue;
				case 12u:
					num5 += vertexCountHorizontal * 4 + vertexCountVertical * 4;
					num3 = ((int)num4 * -292555211) ^ -113598014;
					continue;
				case 1u:
					goto IL_0109;
				case 5u:
					num6 = chunkCountHorizontal;
					goto IL_0121;
				case 6u:
					num5 = vertexCountHorizontal * vertexCountVertical;
					num3 = ((int)num4 * -790902613) ^ 0x58FF08B;
					continue;
				case 7u:
					goto IL_0147;
				case 13u:
					num5 = vertexCountHorizontal * vertexCountVertical;
					num3 = (int)(num4 * 1999643482) ^ -1206877962;
					continue;
				case 11u:
					return num5;
				case 8u:
					goto IL_0181;
				default:
					{
						return 0;
					}
					IL_0121:
					chunkCountHorizontal = num6;
					if (chunkCountVertical < 1)
					{
						num9 = 1;
						goto IL_00af;
					}
					num3 = -159800806;
					continue;
					IL_00af:
					chunkCountVertical = num9;
					num3 = -500393891;
					continue;
				}
				break;
				IL_0181:
				int num10;
				if (chunkCountHorizontal * chunkCountVertical > 1)
				{
					num3 = -1237351697;
					num10 = num3;
				}
				else
				{
					num3 = -2088039015;
					num10 = num3;
				}
				continue;
				IL_0061:
				int num11;
				if (num5 < 0)
				{
					num3 = -691985549;
					num11 = num3;
				}
				else
				{
					num3 = -501220261;
					num11 = num3;
				}
			}
			goto IL_0007;
			IL_0109:
			num = vertexCountHorizontal;
			goto IL_010d;
			IL_010d:
			vertexCountHorizontal = num;
			if (vertexCountVertical >= 2)
			{
				num3 = -1499221865;
				goto IL_000c;
			}
			num2 = 2;
			goto IL_014b;
			IL_014b:
			vertexCountVertical = num2;
			num3 = -1471035886;
			goto IL_000c;
		}

		public static int GetGeneratedVertexCount(int vertexCountHorizontal, int vertexCountVertical, bool hasEdgeFall)
		{
			if (vertexCountHorizontal >= 2)
			{
				goto IL_0004;
			}
			int num = 2;
			goto IL_0058;
			IL_0058:
			vertexCountHorizontal = num;
			int num2 = -462662305;
			goto IL_0009;
			IL_0004:
			num2 = -937708422;
			goto IL_0009;
			IL_0009:
			int num5 = default(int);
			while (true)
			{
				uint num3;
				int num4;
				switch ((num3 = (uint)num2 ^ 0xE3CB0C07u) % 7)
				{
				case 3u:
					break;
				case 0u:
					num4 = vertexCountVertical;
					goto IL_003a;
				case 5u:
					return num5;
				case 4u:
					goto IL_0054;
				case 1u:
					if (vertexCountVertical < 2)
					{
						num4 = 2;
						goto IL_003a;
					}
					num2 = (int)(num3 * 432505355) ^ -152224830;
					continue;
				case 2u:
					goto IL_0074;
				default:
					{
						return 0;
					}
					IL_003a:
					vertexCountVertical = num4;
					num2 = -458986400;
					continue;
				}
				break;
				IL_0074:
				num5 = vertexCountHorizontal * vertexCountVertical + (hasEdgeFall ? (vertexCountHorizontal * 4 + vertexCountVertical * 4) : 0);
				int num6;
				if (num5 < 0)
				{
					num2 = -1059796891;
					num6 = num2;
				}
				else
				{
					num2 = -645750069;
					num6 = num2;
				}
			}
			goto IL_0004;
			IL_0054:
			num = vertexCountHorizontal;
			goto IL_0058;
		}

		public static void ConvertResolutionToVertexCount(TerrainData terrainData, int resolution, out int vertexCountHorizontal, out int vertexCountVertical)
		{
			if (resolution < 1)
			{
				goto IL_0004;
			}
			goto IL_0064;
			IL_0004:
			int num = -1505875703;
			goto IL_0009;
			IL_0009:
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0x8D834219u) % 13)
				{
				case 10u:
					break;
				default:
					return;
				case 12u:
					vertexCountVertical = 2;
					num = (int)((num2 * 971883794) ^ 0xB8F33C4);
					continue;
				case 0u:
					goto IL_0064;
				case 5u:
					vertexCountVertical = resolution + 1;
					num = ((int)num2 * -504146064) ^ 0x1FEFEAF;
					continue;
				case 3u:
					resolution = 1;
					num = (int)(num2 * 876854083) ^ -1752812654;
					continue;
				case 8u:
					vertexCountHorizontal = (int)(_206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(terrainData).x / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(terrainData).z) * resolution + 1;
					num = ((int)num2 * -304897416) ^ 0x1AA5E813;
					continue;
				case 7u:
					goto IL_00ec;
				case 4u:
					num = (int)((num2 * 535958344) ^ 0x2FEF81C4);
					continue;
				case 1u:
					vertexCountHorizontal = 2;
					num = (int)(num2 * 1055566600) ^ -1599839230;
					continue;
				case 6u:
					goto IL_012c;
				case 9u:
					vertexCountHorizontal = resolution + 1;
					num = -1344326430;
					continue;
				case 11u:
					vertexCountVertical = (int)(_206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(terrainData).z / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(terrainData).x) * resolution + 1;
					num = ((int)num2 * -1787465593) ^ -661166679;
					continue;
				case 2u:
					return;
				}
				break;
				IL_012c:
				int num3;
				if (vertexCountHorizontal >= 2)
				{
					num = -362196638;
					num3 = num;
				}
				else
				{
					num = -1949009867;
					num3 = num;
				}
				continue;
				IL_00ec:
				int num4;
				if (vertexCountVertical < 2)
				{
					num = -1202336201;
					num4 = num;
				}
				else
				{
					num = -335252744;
					num4 = num;
				}
			}
			goto IL_0004;
			IL_0064:
			int num5;
			if (_206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(terrainData).x > _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(terrainData).z)
			{
				num = -120498322;
				num5 = num;
			}
			else
			{
				num = -1703176787;
				num5 = num;
			}
			goto IL_0009;
		}

		internal static bool _200D_200F_202C_202D_206B_206B_202C_200D_202C_202A_200C_200C_202D_206B_200C_202B_202D_202B_200B_206D_206E_202B_206C_200D_200E_202B_200D_200B_202D_206F_206B_202E_206C_206E_202C_202D_206B_200B_202E_206D_202E(TerrainData P_0, Vector3 P_1, int P_2, int P_3, int P_4, int P_5)
		{
			float num = _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).x / (float)P_2;
			float num2 = _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).z / (float)P_3;
			float num3 = num * (float)P_4;
			float num4 = num * (float)(P_4 + 1);
			float num5 = num2 * (float)P_5;
			float num6 = num2 * (float)(P_5 + 1);
			Rect rect = default(Rect);
			while (true)
			{
				int num7 = -1487847195;
				while (true)
				{
					uint num8;
					switch ((num8 = (uint)num7 ^ 0x8452737Du) % 3)
					{
					case 2u:
						break;
					case 1u:
						goto IL_005c;
					default:
						return rect.Contains(new Vector2(P_1.x, P_1.z));
					}
					break;
					IL_005c:
					rect = new Rect(num3, num5, num4 - num3, num6 - num5);
					num7 = (int)(num8 * 1236365735) ^ -1167994416;
				}
			}
		}

		private static bool _202D_206C_202B_202A_202B_200B_206B_202A_202D_200E_202A_200E_202A_200E_200D_202A_206A_206A_200F_206A_200E_206F_202D_200E_200C_202C_200B_202B_202D_202C_202C_202E_202A_206B_200E_202A_202A_206B_202A_206C_202E(Vector3 P_0, Vector3 P_1, Vector3 P_2, Vector3 P_3)
		{
			float num = P_1.z * P_3.x - P_1.x * P_3.z + (P_3.z - P_1.z) * P_0.x + (P_1.x - P_3.x) * P_0.z;
			float num2 = P_1.x * P_2.z - P_1.z * P_2.x + (P_1.z - P_2.z) * P_0.x + (P_2.x - P_1.x) * P_0.z;
			float num5 = default(float);
			while (true)
			{
				int num3 = 2029107407;
				while (true)
				{
					uint num4;
					switch ((num4 = (uint)num3 ^ 0x25D52B0Bu) % 10)
					{
					case 0u:
						break;
					case 6u:
						return num + num2 >= num5;
					case 1u:
					{
						num5 = (0f - P_2.z) * P_3.x + P_1.z * (P_3.x - P_2.x) + P_1.x * (P_2.z - P_3.z) + P_2.x * P_3.z;
						int num8;
						if (!(num5 >= 0f))
						{
							num3 = 2057817974;
							num8 = num3;
						}
						else
						{
							num3 = 1461408838;
							num8 = num3;
						}
						continue;
					}
					case 5u:
						return false;
					case 7u:
					{
						int num9;
						if (num <= 0f)
						{
							num3 = 1837829569;
							num9 = num3;
						}
						else
						{
							num3 = 167419247;
							num9 = num3;
						}
						continue;
					}
					case 9u:
					{
						int num10;
						int num11;
						if (num < 0f)
						{
							num10 = -415303901;
							num11 = num10;
						}
						else
						{
							num10 = -760805842;
							num11 = num10;
						}
						num3 = num10 ^ ((int)num4 * -1459579916);
						continue;
					}
					case 2u:
					{
						int num6;
						int num7;
						if (num < 0f != num2 < 0f)
						{
							num6 = 1226858460;
							num7 = num6;
						}
						else
						{
							num6 = 246969278;
							num7 = num6;
						}
						num3 = num6 ^ (int)(num4 * 1334499082);
						continue;
					}
					case 4u:
						return false;
					case 3u:
						return num + num2 <= num5;
					default:
						return false;
					}
					break;
				}
			}
		}

		internal static void _200C_206C_200C_202B_200B_206E_206F_200C_200B_202C_206E_206D_206A_200F_202A_206F_200C_206B_206D_206D_206B_206D_206A_202E_200E_206F_206C_206F_206D_202A_200E_206F_202D_202D_206B_200B_200C_202D_202E_202E_202E(TerrainData P_0, int P_1, int P_2, ref Vector3 P_3, ref Vector3 P_4)
		{
			_206B_202E_206E_202C_206D_200D_202B_202E_202E_202B_206A_200E_206A_200C_202D_200C_206F_202B_200C_206A_200B_206A_206A_206D_206A_206D_202D_206B_206A_206D_200F_200C_206F_206C_206C_202E_202D_206F_202C_200C_202E(P_0, P_3, P_1, P_2, out var vector, out var vector2, out var vector3, out var c);
			Plane plane = default(Plane);
			while (true)
			{
				int num = -1390241084;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0x98D50465u) % 10)
					{
					case 6u:
						break;
					case 0u:
						vector2.y = _202C_206A_202D_200E_200D_206F_200C_206A_206D_200C_206F_202D_200D_206C_206C_200D_200B_206B_202E_202C_202C_206B_202D_206C_206E_200F_206D_206C_202A_206F_206E_200F_202D_206D_206E_202B_200C_200D_202E(P_0, vector2.x / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).x, vector2.z / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).z);
						num = ((int)num2 * -287932629) ^ 0x2C4BEB4D;
						continue;
					case 3u:
						vector3.y = _202C_206A_202D_200E_200D_206F_200C_206A_206D_200C_206F_202D_200D_206C_206C_200D_200B_206B_202E_202C_202C_206B_202D_206C_206E_200F_206D_206C_202A_206F_206E_200F_202D_206D_206E_202B_200C_200D_202E(P_0, vector3.x / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).x, vector3.z / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).z);
						vector.y = _202C_206A_202D_200E_200D_206F_200C_206A_206D_200C_206F_202D_200D_206C_206C_200D_200B_206B_202E_202C_202C_206B_202D_206C_206E_200F_206D_206C_202A_206F_206E_200F_202D_206D_206E_202B_200C_200D_202E(P_0, vector.x / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).x, vector.z / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).z);
						num = ((int)num2 * -134218458) ^ 0x3473B09F;
						continue;
					case 2u:
					{
						plane.Raycast(new Ray(P_3, Vector3.down), out var enter);
						P_3.y = _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).y + 100f - enter;
						num = (int)((num2 * 95412477) ^ 0x2F09F9DE);
						continue;
					}
					case 7u:
						P_3.y = _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).y + 100f;
						num = -478389839;
						continue;
					case 5u:
						vector3.y = _202C_206A_202D_200E_200D_206F_200C_206A_206D_200C_206F_202D_200D_206C_206C_200D_200B_206B_202E_202C_202C_206B_202D_206C_206E_200F_206D_206C_202A_206F_206E_200F_202D_206D_206E_202B_200C_200D_202E(P_0, vector3.x / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).x, vector3.z / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).z);
						num = -1273597509;
						continue;
					case 4u:
						plane = new Plane(vector3, vector, vector2);
						num = (int)((num2 * 1521619729) ^ 0xAEFCF4E);
						continue;
					case 9u:
					{
						int num3;
						int num4;
						if (!_202D_206C_202B_202A_202B_200B_206B_202A_202D_200E_202A_200E_202A_200E_200D_202A_206A_206A_200F_206A_200E_206F_202D_200E_200C_202C_200B_202B_202D_202C_202C_202E_202A_206B_200E_202A_202A_206B_202A_206C_202E(P_3, vector3, vector, vector2))
						{
							num3 = -624770779;
							num4 = num3;
						}
						else
						{
							num3 = -855182657;
							num4 = num3;
						}
						num = num3 ^ (int)(num2 * 133184541);
						continue;
					}
					case 8u:
						vector2.y = _202C_206A_202D_200E_200D_206F_200C_206A_206D_200C_206F_202D_200D_206C_206C_200D_200B_206B_202E_202C_202C_206B_202D_206C_206E_200F_206D_206C_202A_206F_206E_200F_202D_206D_206E_202B_200C_200D_202E(P_0, vector2.x / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).x, vector2.z / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).z);
						c.y = _202C_206A_202D_200E_200D_206F_200C_206A_206D_200C_206F_202D_200D_206C_206C_200D_200B_206B_202E_202C_202C_206B_202D_206C_206E_200F_206D_206C_202A_206F_206E_200F_202D_206D_206E_202B_200C_200D_202E(P_0, c.x / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).x, c.z / _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).z);
						plane = new Plane(vector3, vector2, c);
						num = ((int)num2 * -1447339555) ^ -1115228464;
						continue;
					default:
						P_4 = plane.normal;
						return;
					}
					break;
				}
			}
		}

		private static void _206B_202E_206E_202C_206D_200D_202B_202E_202E_202B_206A_200E_206A_200C_202D_200C_206F_202B_200C_206A_200B_206A_206A_206D_206A_206D_202D_206B_206A_206D_200F_200C_206F_206C_206C_202E_202D_206F_202C_200C_202E(TerrainData P_0, Vector3 P_1, int P_2, int P_3, out Vector3 P_4, out Vector3 P_5, out Vector3 P_6, out Vector3 P_7)
		{
			float num = _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).x / (float)P_2;
			float z = default(float);
			float num4 = default(float);
			float x2 = default(float);
			float x = default(float);
			float z2 = default(float);
			while (true)
			{
				int num2 = -1320035558;
				while (true)
				{
					uint num3;
					switch ((num3 = (uint)num2 ^ 0xD7994829u) % 9)
					{
					case 5u:
						break;
					default:
						return;
					case 6u:
						z = num4 * (float)(Mathf.FloorToInt(P_1.z / num4) + 1);
						P_4 = new Vector3(x2, 0f, z);
						num2 = (int)((num3 * 2019168122) ^ 0x1F582E9D);
						continue;
					case 1u:
						x2 = num * (float)Mathf.FloorToInt(P_1.x / num);
						num2 = ((int)num3 * -2019967759) ^ 0xC6B3E09;
						continue;
					case 7u:
						x = num * (float)(Mathf.FloorToInt(P_1.x / num) + 1);
						num2 = ((int)num3 * -241123352) ^ -1955747414;
						continue;
					case 3u:
						z2 = num4 * (float)Mathf.FloorToInt(P_1.z / num4);
						num2 = (int)(num3 * 959518613) ^ -775674220;
						continue;
					case 0u:
						P_6 = new Vector3(x2, 0f, z2);
						P_7 = new Vector3(x, 0f, z2);
						num2 = ((int)num3 * -820773150) ^ -1138259586;
						continue;
					case 8u:
						num4 = _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(P_0).z / (float)P_3;
						num2 = (int)((num3 * 653430526) ^ 0x4D0CA86D);
						continue;
					case 4u:
						P_5 = new Vector3(x, 0f, z);
						num2 = ((int)num3 * -95369920) ^ -1047584261;
						continue;
					case 2u:
						return;
					}
					break;
				}
			}
		}

		public static Mesh CreateGrassMesh(int sides)
		{
			List<Vector3> list = new List<Vector3>
			{
				new Vector3(-0.5f, 0f, 0f),
				new Vector3(0.5f, 0f, 0f),
				new Vector3(-0.5f, 1f, 0f),
				new Vector3(0.5f, 1f, 0f)
			};
			List<Vector2> list5 = default(List<Vector2>);
			Vector3[] array = default(Vector3[]);
			Vector3[] array3 = default(Vector3[]);
			int num9 = default(int);
			float num7 = default(float);
			List<Vector3> list3 = default(List<Vector3>);
			Vector4[] array6 = default(Vector4[]);
			List<Vector4> list4 = default(List<Vector4>);
			Vector2[] array4 = default(Vector2[]);
			Color[] array5 = default(Color[]);
			List<Color> list6 = default(List<Color>);
			int[] array2 = default(int[]);
			int num5 = default(int);
			List<int> list2 = default(List<int>);
			int num4 = default(int);
			int num3 = default(int);
			Quaternion quaternion = default(Quaternion);
			while (true)
			{
				int num = 1916001889;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0x2F9D5A5Fu) % 28)
					{
					case 25u:
						break;
					case 26u:
						list5 = new List<Vector2>
						{
							new Vector2(0f, 0f),
							new Vector2(1f, 0f),
							new Vector2(0f, 1f),
							new Vector2(1f, 1f)
						};
						num = (int)((num2 * 1025304043) ^ 0x7D9F62F6);
						continue;
					case 23u:
						list.AddRange(array);
						num = (int)(num2 * 541764480) ^ -1452365582;
						continue;
					case 14u:
						array3[num9] = Quaternion.AngleAxis(num7, Vector3.up) * list3[num9];
						array6[num9] = list4[num9];
						array4[num9] = list5[num9];
						array5[num9] = list6[num9];
						num = (int)(num2 * 943437874) ^ -1628375094;
						continue;
					case 3u:
						num9 = 0;
						num = ((int)num2 * -2073285417) ^ 0x22D60207;
						continue;
					case 6u:
						array2[num5] = list2[num5] + num4;
						num = 137092540;
						continue;
					case 0u:
						array = new Vector3[4];
						array2 = new int[6];
						array3 = new Vector3[4];
						num = (int)((num2 * 697613811) ^ 0x59A394DD);
						continue;
					case 10u:
					{
						int num10;
						if (num9 >= 4)
						{
							num = 482573054;
							num10 = num;
						}
						else
						{
							num = 1159016760;
							num10 = num;
						}
						continue;
					}
					case 9u:
						num9++;
						num = (int)(num2 * 406415440) ^ -777179351;
						continue;
					case 5u:
						num7 = 180 / sides * num3;
						num = 492728272;
						continue;
					case 2u:
						list2 = new List<int> { 0, 3, 1, 3, 0, 2 };
						list3 = new List<Vector3>
						{
							new Vector3(0f, 0f, -1f),
							new Vector3(0f, 0f, -1f),
							new Vector3(0f, 0f, -1f),
							new Vector3(0f, 0f, -1f)
						};
						num = (int)(num2 * 1709851758) ^ -1404115489;
						continue;
					case 18u:
						array6 = new Vector4[4];
						num = ((int)num2 * -1182799736) ^ 0x44479E4C;
						continue;
					case 24u:
						list6.AddRange(array5);
						num3++;
						num = ((int)num2 * -1548407533) ^ 0x366DBB13;
						continue;
					case 27u:
						array[num9] = quaternion * list[num9];
						num = 686973993;
						continue;
					case 16u:
					{
						int num8;
						if (num3 < sides)
						{
							num = 2112260330;
							num8 = num;
						}
						else
						{
							num = 1727234767;
							num8 = num;
						}
						continue;
					}
					case 8u:
						list3.AddRange(array3);
						list4.AddRange(array6);
						list5.AddRange(array4);
						num = ((int)num2 * -1862216029) ^ -37702961;
						continue;
					case 15u:
						num5++;
						num = (int)(num2 * 412442875) ^ -1745188536;
						continue;
					case 11u:
						quaternion = Quaternion.Euler(new Vector3(0f, num7, 0f));
						num = (int)(num2 * 1235020203) ^ -607228686;
						continue;
					case 21u:
						num = ((int)num2 * -713549342) ^ -1953046565;
						continue;
					case 4u:
						array5 = new Color[4];
						num = (int)((num2 * 1994181754) ^ 0x4671C6D0);
						continue;
					case 1u:
						list2.AddRange(array2);
						num = (int)((num2 * 1494955828) ^ 0x1B54D7B);
						continue;
					case 19u:
						array4 = new Vector2[4];
						num = ((int)num2 * -525308921) ^ 0x5205295A;
						continue;
					case 12u:
						list4 = new List<Vector4>
						{
							new Vector4(1f, 0f, 0f, -1f),
							new Vector4(1f, 0f, 0f, -1f),
							new Vector4(1f, 0f, 0f, -1f),
							new Vector4(1f, 0f, 0f, -1f)
						};
						num = ((int)num2 * -1367328375) ^ -297687511;
						continue;
					case 22u:
					{
						int num6;
						if (num5 >= 6)
						{
							num = 1604962488;
							num6 = num;
						}
						else
						{
							num = 929141301;
							num6 = num;
						}
						continue;
					}
					case 7u:
						list6 = new List<Color>
						{
							new Color(1f, 1f, 1f, 0f),
							new Color(1f, 1f, 1f, 0f),
							new Color(1f, 1f, 1f, 1f),
							new Color(1f, 1f, 1f, 1f)
						};
						sides = Mathf.Clamp(sides, 1, 6);
						num3 = 1;
						num = (int)((num2 * 2002169627) ^ 0x7055D8F6);
						continue;
					case 13u:
						num5 = 0;
						num = ((int)num2 * -1912936589) ^ -1174965518;
						continue;
					case 17u:
						num4 = 4 * num3;
						num = ((int)num2 * -1339386409) ^ -660389887;
						continue;
					default:
					{
						Mesh mesh = _202D_200E_200C_202E_206C_202C_206F_200E_200F_206B_206A_200B_206A_206B_206F_200F_202D_200E_200B_200D_200D_200C_200F_206A_202C_206C_200B_202A_202E_200B_206E_200F_200B_202A_206A_202D_200B_206E_202E_202E_202E();
						_202B_206E_206A_200B_202C_206A_206F_206D_202B_200F_206D_202B_202C_200E_206C_200C_200F_200B_202B_202A_202C_200C_206B_206C_202D_200D_206E_206A_200E_202D_200F_206F_200E_200C_206E_200E_206A_200F_200E_206F_202E((UnityEngine.Object)mesh, "Grass");
						_200C_206F_200C_202C_200B_206F_206C_202C_200E_200B_206C_202C_202B_200D_206E_202B_206A_202E_206F_202D_206D_206D_200B_206B_200B_206E_206C_200E_202A_200C_206B_206A_206A_206F_206E_206F_200D_206F_206D_202E_202E(mesh, list.ToArray());
						_200E_206C_202D_200C_206D_200B_200B_202C_200C_202C_206E_202A_200E_206D_200F_206F_206E_206C_202A_202D_200C_206F_206A_202D_202D_202E_206A_206F_200C_206C_202A_206C_206F_206D_200E_200B_202E_206A_202D_206F_202E(mesh, list2.ToArray());
						_200F_202A_206D_206C_200B_206F_202C_202E_206E_206A_202E_202E_200B_206C_200F_200C_202C_200E_200C_206E_200B_202D_206F_200E_206E_200B_206C_202C_202D_202E_202D_206E_202C_200F_206E_200E_206A_206B_200B_200C_202E(mesh, list3.ToArray());
						_206E_200F_206E_200F_202E_206E_206E_206E_202B_206F_202E_200F_206E_206F_202A_200C_202E_202A_200F_206A_206A_206B_202A_200E_202D_200B_206E_200C_200F_206D_206D_200E_200B_206E_206E_202C_206D_200C_206E_206A_202E(mesh, list4.ToArray());
						_200F_202B_202C_202B_206B_202D_202D_206D_206D_200D_202E_200F_200B_200B_202B_206C_202B_200F_202D_206B_200F_202C_206A_202E_206F_206E_206F_200F_206A_206F_206B_206E_206D_206D_202A_200F_200D_202D_206E_200F_202E(mesh, list5.ToArray());
						_206B_200D_202E_200B_206D_200C_200C_202E_206B_206B_200D_206E_202C_202D_202E_200B_200E_200C_206B_202C_206B_200C_200F_202B_206F_202D_202C_206E_202D_200D_200B_200F_202C_206C_202D_200D_206C_206C_206A_200E_202E(mesh, list6.ToArray());
						return mesh;
					}
					}
					break;
				}
			}
		}

		public static List<Mesh> CombineGameObjects(GameObject parentGameObject, Material material, string meshName, string objectName, int maxVertexCountPerMesh = 65535)
		{
			maxVertexCountPerMesh = Mathf.Clamp(maxVertexCountPerMesh, 16, 100000000);
			IndexFormat indexFormat = IndexFormat.UInt16;
			MeshFilter[] componentsInChildren = default(MeshFilter[]);
			int num6 = default(int);
			List<Mesh> list2 = default(List<Mesh>);
			List<CombineInstance> list = default(List<CombineInstance>);
			string text3 = default(string);
			int num8 = default(int);
			int num9 = default(int);
			string text2 = default(string);
			CombineInstance item = default(CombineInstance);
			while (true)
			{
				int num = -904102404;
				while (true)
				{
					uint num2;
					string text;
					switch ((num2 = (uint)num ^ 0xB0642C07u) % 35)
					{
					case 9u:
						break;
					case 22u:
						_206D_202C_206B_202B_206E_200E_200F_200C_206B_202D_206A_206F_202C_202D_202A_200C_206B_206F_206C_200B_200C_202D_202A_206F_206A_202C_202A_206B_200B_200C_206E_202E_206B_206E_206A_200B_206D_206F_202A_206C_202E((UnityEngine.Object)_206C_206F_206B_200C_200C_206F_206D_202E_200D_202E_202C_206E_200F_202D_200F_200D_200C_202C_200B_206A_202A_206E_202A_200B_202A_200C_200B_206B_200F_206E_202B_202C_206E_206F_202A_206F_202A_200B_200F_200E_202E((Component)componentsInChildren[num6]));
						num = ((int)num2 * -1688778161) ^ 0x4F6BB3B7;
						continue;
					case 16u:
						num = (int)(num2 * 857165951) ^ -583670300;
						continue;
					case 31u:
						_202C_206A_200E_206C_202A_206B_202D_202D_200C_202D_206A_202B_200D_206F_206A_206A_200B_206F_202A_200F_200B_202E_202A_202A_202B_202E_200B_200F_202D_200F_202B_202C_202D_206F_200E_200F_206A_200B_200F_202C_202E((UnityEngine.Object)_206C_206F_206B_200C_200C_206F_206D_202E_200D_202E_202C_206E_200F_202D_200F_200D_200C_202C_200B_206A_202A_206E_202A_200B_202A_200C_200B_206B_200F_206E_202B_202C_206E_206F_202A_206F_202A_200B_200F_200E_202E((Component)componentsInChildren[num6]));
						num = -2133100099;
						continue;
					case 1u:
						list2.Add(_202A_206C_206E_202C_202B_202E_202C_202C_206A_206D_200E_202C_202C_202D_206D_206C_206C_206D_200F_206D_200E_206C_206D_202C_200B_202E_202D_202B_202E_200F_200D_202E_202C_206C_200D_206A_200E_202A_202D_202E_202E(parentGameObject, list, material, _200B_206E_202D_206A_206B_206A_200C_202E_200B_200B_202E_206B_206F_206F_206F_202B_202A_200D_206D_206C_202C_206D_200F_202C_206E_206A_206C_206D_206B_200D_206A_206D_206D_206D_206D_202D_200D_200D_202C_200D_202E(meshName, text3), _200B_206E_202D_206A_206B_206A_200C_202E_200B_200B_202E_206B_206F_206F_206F_202B_202A_200D_206D_206C_202C_206D_200F_202C_206E_206A_206C_206D_206B_200D_206A_206D_206D_206D_206D_202D_200D_200D_202C_200D_202E(objectName, text3), indexFormat));
						num = ((int)num2 * -1828034662) ^ 0x5E742016;
						continue;
					case 27u:
						text = _206C_206C_200B_202C_206A_202C_202E_200D_200B_206F_202B_202A_202C_200E_206D_200E_202B_206E_206A_202B_206E_206E_206E_202E_202D_206E_200E_202B_206A_202D_202D_202A_206E_206D_206B_202B_202B_202C_206D_202B_202E(" [Part {0}]", (object)num8);
						goto IL_014f;
					case 5u:
					{
						int num23;
						int num24;
						if (num9 + _202D_206C_202E_202C_206B_200E_200E_206C_200D_200B_206A_202A_206E_202A_200D_206D_200B_206F_200E_206C_202D_206B_202A_200D_200D_200B_202B_202A_206E_200F_206E_202C_202A_202B_200D_206D_206A_202B_206C_206E_202E(_202C_202B_202A_206F_202A_200B_206E_202C_206C_202E_206A_200E_200C_202C_206A_200E_206C_206C_200D_206E_200D_206B_200E_206D_202B_202D_200F_202D_206B_206F_202E_206D_200E_206F_200D_202A_200E_202D_200F_202A_202E(componentsInChildren[num6])) > maxVertexCountPerMesh)
						{
							num23 = 588059911;
							num24 = num23;
						}
						else
						{
							num23 = 1576402923;
							num24 = num23;
						}
						num = num23 ^ (int)(num2 * 1974565351);
						continue;
					}
					case 18u:
					{
						int num14;
						int num15;
						if (num9 != 0)
						{
							num14 = -1994820990;
							num15 = num14;
						}
						else
						{
							num14 = -270588792;
							num15 = num14;
						}
						num = num14 ^ ((int)num2 * -1988769822);
						continue;
					}
					case 29u:
					{
						int num7;
						if (num6 < componentsInChildren.Length)
						{
							num = -1004372471;
							num7 = num;
						}
						else
						{
							num = -1580041758;
							num7 = num;
						}
						continue;
					}
					case 24u:
						num9 += _202D_206C_202E_202C_206B_200E_200E_206C_200D_200B_206A_202A_206E_202A_200D_206D_200B_206F_200E_206C_202D_206B_202A_200D_200D_200B_202B_202A_206E_200F_206E_202C_202A_202B_200D_206D_206A_202B_206C_206E_202E(_202C_202B_202A_206F_202A_200B_206E_202C_206C_202E_206A_200E_200C_202C_206A_200E_206C_206C_200D_206E_200D_206B_200E_206D_202B_202D_200F_202D_206B_206F_202E_206D_200E_206F_200D_202A_200E_202D_200F_202A_202E(componentsInChildren[num6]));
						num = ((int)num2 * -1139294698) ^ -82743209;
						continue;
					case 33u:
					{
						int num19;
						int num20;
						if (list.Count == 0)
						{
							num19 = 502053991;
							num20 = num19;
						}
						else
						{
							num19 = 1922561625;
							num20 = num19;
						}
						num = num19 ^ (int)(num2 * 1731920015);
						continue;
					}
					case 17u:
					{
						int num16;
						if (!_202C_206C_206F_202A_206D_206C_202C_200F_200F_202A_202E_202A_202D_202B_206B_200C_200B_206C_200E_206B_200B_206B_206D_206F_202B_200B_206F_206F_206C_206D_200B_200C_202D_202E_202E_202C_200D_206D_202A_200C_202E((UnityEngine.Object)componentsInChildren[num6], (UnityEngine.Object)null))
						{
							num = -2133100099;
							num16 = num;
						}
						else
						{
							num = -44249888;
							num16 = num;
						}
						continue;
					}
					case 23u:
						if (num8 == 1)
						{
							text = string.Empty;
							goto IL_014f;
						}
						num = ((int)num2 * -506189828) ^ 0xE9E7010;
						continue;
					case 34u:
					{
						int num4;
						int num5;
						if (maxVertexCountPerMesh <= Constants.vertexLimitIn16BitsIndexBuffer)
						{
							num4 = -913759169;
							num5 = num4;
						}
						else
						{
							num4 = -922895644;
							num5 = num4;
						}
						num = num4 ^ ((int)num2 * -356821012);
						continue;
					}
					case 30u:
					{
						int num21;
						int num22;
						if (maxVertexCountPerMesh <= Constants.vertexLimitIn16BitsIndexBuffer - 100)
						{
							num21 = -647404913;
							num22 = num21;
						}
						else
						{
							num21 = -232535064;
							num22 = num21;
						}
						num = num21 ^ ((int)num2 * -1263096593);
						continue;
					}
					case 11u:
						text2 = _206C_206C_200B_202C_206A_202C_202E_200D_200B_206F_202B_202A_202C_200E_206D_200E_202B_206E_206A_202B_206E_206E_206E_202E_202D_206E_200E_202B_206A_202D_202D_202A_206E_206D_206B_202B_202B_202C_206D_202B_202E(" [Part {0}]", (object)num8);
						num = (int)((num2 * 219912946) ^ 0x532FD8FC);
						continue;
					case 28u:
					{
						int num17;
						int num18;
						if (_202C_206C_206F_202A_206D_206C_202C_200F_200F_202A_202E_202A_202D_202B_206B_200C_200B_206C_200E_206B_200B_206B_206D_206F_202B_200B_206F_206F_206C_206D_200B_200C_202D_202E_202E_202C_200D_206D_202A_200C_202E((UnityEngine.Object)_202C_202B_202A_206F_202A_200B_206E_202C_206C_202E_206A_200E_200C_202C_206A_200E_206C_206C_200D_206E_200D_206B_200E_206D_202B_202D_200F_202D_206B_206F_202E_206D_200E_206F_200D_202A_200E_202D_200F_202A_202E(componentsInChildren[num6]), (UnityEngine.Object)null))
						{
							num17 = 1066613524;
							num18 = num17;
						}
						else
						{
							num17 = 1616320547;
							num18 = num17;
						}
						num = num17 ^ ((int)num2 * -80554062);
						continue;
					}
					case 20u:
						maxVertexCountPerMesh = Constants.vertexLimitIn16BitsIndexBuffer;
						num = -760626085;
						continue;
					case 3u:
						list2.Add(_202A_206C_206E_202C_202B_202E_202C_202C_206A_206D_200E_202C_202C_202D_206D_206C_206C_206D_200F_206D_200E_206C_206D_202C_200B_202E_202D_202B_202E_200F_200D_202E_202C_206C_200D_206A_200E_202A_202D_202E_202E(parentGameObject, list, material, _200B_206E_202D_206A_206B_206A_200C_202E_200B_200B_202E_206B_206F_206F_206F_202B_202A_200D_206D_206C_202C_206D_200F_202C_206E_206A_206C_206D_206B_200D_206A_206D_206D_206D_206D_202D_200D_200D_202C_200D_202E(meshName, text2), _200B_206E_202D_206A_206B_206A_200C_202E_200B_200B_202E_206B_206F_206F_206F_202B_202A_200D_206D_206C_202C_206D_200F_202C_206E_206A_206C_206D_206B_200D_206A_206D_206D_206D_206D_202D_200D_200D_202C_200D_202E(objectName, text2), indexFormat));
						list.Clear();
						num9 = 0;
						num = ((int)num2 * -1061510717) ^ 0x30D99F77;
						continue;
					case 10u:
						indexFormat = IndexFormat.UInt32;
						num = (int)((num2 * 1596091608) ^ 0x37957A93);
						continue;
					case 14u:
						item.mesh = _202C_202B_202A_206F_202A_200B_206E_202C_206C_202E_206A_200E_200C_202C_206A_200E_206C_206C_200D_206E_200D_206B_200E_206D_202B_202D_200F_202D_206B_206F_202E_206D_200E_206F_200D_202A_200E_202D_200F_202A_202E(componentsInChildren[num6]);
						item.transform = _206B_200C_200B_200C_200E_206B_206A_200F_200C_200D_202A_206B_200B_200B_200D_200D_206A_200E_206F_206E_206C_202C_200C_200E_202D_200F_202C_206A_202A_200B_206A_200E_206C_202B_206D_202B_202B_206D_206E_200E_202E(_202E_206A_202E_202C_200D_206F_200F_202C_202D_202A_202A_202E_206C_202A_200F_206D_206B_206C_202A_206A_200B_200F_200F_206C_206A_206E_202B_202D_200C_202D_206D_202E_202A_206F_206C_202B_206A_206D_202E_206D_202E((Component)componentsInChildren[num6]));
						num = ((int)num2 * -1009206162) ^ -663364372;
						continue;
					case 32u:
						list = new List<CombineInstance>();
						num = (int)((num2 * 149716654) ^ 0x4903EE97);
						continue;
					case 25u:
						num6++;
						num = -1716189009;
						continue;
					case 15u:
						maxVertexCountPerMesh -= 100;
						num = (int)(num2 * 61185103) ^ -1252518020;
						continue;
					case 13u:
						list2 = new List<Mesh>();
						num = ((int)num2 * -969875495) ^ 0x64883D;
						continue;
					case 0u:
					{
						int num12;
						int num13;
						if (!_206A_206B_202E_206B_206C_206F_200C_200C_206A_200D_206C_206E_202C_200C_200E_202B_206A_202E_202A_206F_206E_200B_200D_206A_206C_202C_200C_206D_200F_200E_200B_202A_202A_200D_206E_200B_206B_200F_200E_206F_202E())
						{
							num12 = -1301187033;
							num13 = num12;
						}
						else
						{
							num12 = -362756044;
							num13 = num12;
						}
						num = num12 ^ (int)(num2 * 1315443958);
						continue;
					}
					case 7u:
						item = default(CombineInstance);
						num = -165409405;
						continue;
					case 12u:
					{
						int num10;
						int num11;
						if (_200B_202B_200C_202E_202C_206B_206A_200C_206C_200F_200D_200E_200C_206C_202B_202C_202A_200B_206E_206F_200C_206C_200C_202C_200E_202B_206F_206F_206C_200F_200C_206C_206D_206B_200B_200E_206A_200E_202D_200C_202E())
						{
							num10 = -961081144;
							num11 = num10;
						}
						else
						{
							num10 = -296442458;
							num11 = num10;
						}
						num = num10 ^ (int)(num2 * 825923852);
						continue;
					}
					case 21u:
						num8++;
						num = ((int)num2 * -332681278) ^ -1244528316;
						continue;
					case 26u:
						componentsInChildren = parentGameObject.GetComponentsInChildren<MeshFilter>();
						num = -1279501685;
						continue;
					case 4u:
						num8 = 1;
						num6 = 0;
						num9 = 0;
						num = (int)((num2 * 440371257) ^ 0x47DFDA00);
						continue;
					case 2u:
					{
						int num3;
						if (indexFormat != 0)
						{
							num = -231763946;
							num3 = num;
						}
						else
						{
							num = -839957264;
							num3 = num;
						}
						continue;
					}
					case 19u:
						num = ((int)num2 * -2008479869) ^ -237030090;
						continue;
					case 8u:
						list.Add(item);
						num = (int)((num2 * 785099701) ^ 0x3703CF05);
						continue;
					default:
						{
							list.Clear();
							return list2;
						}
						IL_014f:
						text3 = text;
						num = -1172418170;
						continue;
					}
					break;
				}
			}
		}

		private static Mesh _202A_206C_206E_202C_202B_202E_202C_202C_206A_206D_200E_202C_202C_202D_206D_206C_206C_206D_200F_206D_200E_206C_206D_202C_200B_202E_202D_202B_202E_200F_200D_202E_202C_206C_200D_206A_200E_202A_202D_202E_202E(GameObject P_0, List<CombineInstance> P_1, Material P_2, string P_3, string P_4, IndexFormat P_5)
		{
			GameObject gameObject = _202B_206F_206B_202E_202D_206A_200C_206A_202B_206B_200D_206C_200F_206A_206F_200C_206E_202D_206A_202D_206D_200B_202A_202B_200F_202E_200B_206D_200F_200B_206D_202A_200E_200B_206F_206A_206A_200D_202B_206C_202E(P_4);
			_200D_206D_200F_206A_200E_202A_202E_202E_202D_200D_200E_200B_200E_202A_202E_206D_200E_206C_206F_206D_200E_206F_200F_200F_206E_200D_200D_202E_206B_206F_200E_200F_202E_202B_206A_200B_202C_200F_202A_202E_202E(_202B_202D_206C_200F_206D_206F_200C_206C_206F_200F_200F_202C_202D_206E_200C_200E_206D_202E_202A_206F_200E_200B_206E_200F_202C_200B_206B_206F_202B_200B_206B_206F_202E_206E_200E_200E_206E_206F_206D_200C_202E(gameObject), _202B_202D_206C_200F_206D_206F_200C_206C_206F_200F_200F_202C_202D_206E_200C_200E_206D_202E_202A_206F_200E_200B_206E_200F_202C_200B_206B_206F_202B_200B_206B_206F_202E_206E_200E_200E_206E_206F_206D_200C_202E(P_0));
			_206A_206C_206D_200F_200F_206E_202B_200D_200D_200C_206F_206D_200F_200B_202B_202E_206D_202B_206F_202B_202E_200B_206D_202A_206E_202D_200C_206F_206B_202B_200F_200F_200E_206B_202B_202C_200F_206B_206D_202A_202E((Renderer)gameObject.AddComponent<MeshRenderer>(), P_2);
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			_202E_206F_206A_200C_200B_200F_200E_202B_200F_202C_202B_200F_206B_206C_202B_202A_200E_200B_202A_206D_200B_202C_200F_202D_202C_206E_202E_206D_206F_200B_206E_200B_200B_200D_200D_206D_202A_200B_202D_206B_202E(meshFilter, _202D_200E_200C_202E_206C_202C_206F_200E_200F_206B_206A_200B_206A_206B_206F_200F_202D_200E_200B_200D_200D_200C_200F_206A_202C_206C_200B_202A_202E_200B_206E_200F_200B_202A_206A_202D_200B_206E_202E_202E_202E());
			_206C_206C_202A_206F_206D_200E_202A_202B_200C_200C_202E_206E_200F_200F_200B_202A_202E_202D_206F_200B_200B_206A_202C_206D_206B_202B_206F_206A_206B_206B_206F_200E_206B_200B_206D_202B_206E_200E_200D_200C_202E(_202C_202B_202A_206F_202A_200B_206E_202C_206C_202E_206A_200E_200C_202C_206A_200E_206C_206C_200D_206E_200D_206B_200E_206D_202B_202D_200F_202D_206B_206F_202E_206D_200E_206F_200D_202A_200E_202D_200F_202A_202E(meshFilter), P_5);
			_200E_200C_206C_202D_206B_200E_202C_202A_200E_206F_202E_206E_202C_202B_206D_202B_206A_206C_200E_200D_200F_202A_200D_202E_206D_202A_202A_200B_206B_206E_206C_200E_206A_206A_206F_202A_206F_200E_202C_200F_202E(_202C_202B_202A_206F_202A_200B_206E_202C_206C_202E_206A_200E_200C_202C_206A_200E_206C_206C_200D_206E_200D_206B_200E_206D_202B_202D_200F_202D_206B_206F_202E_206D_200E_206F_200D_202A_200E_202D_200F_202A_202E(meshFilter), P_1.ToArray(), true, true);
			_202B_206E_206A_200B_202C_206A_206F_206D_202B_200F_206D_202B_202C_200E_206C_200C_200F_200B_202B_202A_202C_200C_206B_206C_202D_200D_206E_206A_200E_202D_200F_206F_200E_200C_206E_200E_206A_200F_200E_206F_202E((UnityEngine.Object)_202C_202B_202A_206F_202A_200B_206E_202C_206C_202E_206A_200E_200C_202C_206A_200E_206C_206C_200D_206E_200D_206B_200E_206D_202B_202D_200F_202D_206B_206F_202E_206D_200E_206F_200D_202A_200E_202D_200F_202A_202E(meshFilter), P_3);
			return _202C_202B_202A_206F_202A_200B_206E_202C_206C_202E_206A_200E_200C_202C_206A_200E_206C_206C_200D_206E_200D_206B_200E_206D_202B_202D_200F_202D_206B_206F_202E_206D_200E_206F_200D_202A_200E_202D_200F_202A_202E(meshFilter);
		}

		public static bool HasHoles(TerrainData terrainData)
		{
			if (!_206B_202B_202E_206A_202E_206E_206F_202C_200B_202C_202C_202A_200F_202E_200C_206E_202B_200C_200F_206B_202B_200C_200D_202C_206F_200D_200D_206C_202A_206C_206E_202A_200F_200F_200B_206F_202A_202B_206A_202B_202E((UnityEngine.Object)terrainData, (UnityEngine.Object)null))
			{
				int num14 = default(int);
				int num7 = default(int);
				bool[,] array = default(bool[,]);
				int num8 = default(int);
				int num9 = default(int);
				while (true)
				{
					int num = -1564063085;
					while (true)
					{
						uint num2;
						switch ((num2 = (uint)num ^ 0x8A88F9F9u) % 18)
						{
						case 11u:
							break;
						case 15u:
							num14 = 0;
							num = -946146989;
							continue;
						case 16u:
							num7 = _206F_202A_202D_206E_206C_200F_206F_200C_202B_202C_206E_206B_206C_206A_206A_202A_200F_202A_202D_206A_200F_200F_202E_200F_202E_206F_202D_202B_200F_202A_206E_200F_206D_200C_202D_206D_202E_200B_202D_206D_202E((Array)array, 1);
							num = ((int)num2 * -1654817126) ^ -1706546091;
							continue;
						case 0u:
							goto IL_0091;
						case 5u:
						{
							int num12;
							int num13;
							if (_206A_202B_200F_206A_206B_200F_200C_202D_200C_202B_200C_202D_200B_206B_206E_206A_200E_206E_202D_200E_200B_206F_206A_206C_202A_200F_200B_202A_206D_202A_206F_206C_200D_206A_202B_206D_200D_202E_202C_206B_202E(_202A_206B_206A_200E_206E_206B_200B_202D_200F_202D_200B_206F_206E_206A_206B_202B_200F_206D_202B_202C_202E_206D_206A_200B_200D_200C_200C_202B_200D_206C_200B_202C_206E_206B_200D_200F_202E_206E_202B_206A_202E(terrainData)) >= 1)
							{
								num12 = -1367401456;
								num13 = num12;
							}
							else
							{
								num12 = -1659113526;
								num13 = num12;
							}
							num = num12 ^ (int)(num2 * 1762179270);
							continue;
						}
						case 9u:
							array = _206E_200B_202C_206D_202C_206B_202A_200C_200C_206D_200F_200D_202A_206F_200B_200F_200B_206D_200D_202E_206E_202C_202E_202A_206F_200E_206B_202C_206C_200E_200D_202A_202D_202B_206C_200E_200E_200C_202A_206E_202E(terrainData, 0, 0, _206A_202B_200F_206A_206B_200F_200C_202D_200C_202B_200C_202D_200B_206B_206E_206A_200E_206E_202D_200E_200B_206F_206A_206C_202A_200F_200B_202A_206D_202A_206F_206C_200D_206A_202B_206D_200D_202E_202C_206B_202E(_202A_206B_206A_200E_206E_206B_200B_202D_200F_202D_200B_206F_206E_206A_206B_202B_200F_206D_202B_202C_202E_206D_206A_200B_200D_200C_200C_202B_200D_206C_200B_202C_206E_206B_200D_200F_202E_206E_202B_206A_202E(terrainData)), _202D_206A_206F_202D_202B_200B_202A_202E_200F_202D_202C_206C_206C_206B_200C_206B_202C_206F_206B_200F_200F_200B_206F_200F_202A_202E_206F_202A_206C_202C_202A_206F_200C_200C_200D_206A_200C_206B_206D_202B_202E(_202A_206B_206A_200E_206E_206B_200B_202D_200F_202D_200B_206F_206E_206A_206B_202B_200F_206D_202B_202C_202E_206D_206A_200B_200D_200C_200C_202B_200D_206C_200B_202C_206E_206B_200D_200F_202E_206E_202B_206A_202E(terrainData)));
							num = -1216795399;
							continue;
						case 1u:
							goto end_IL_000c;
						case 14u:
						{
							int num5;
							int num6;
							if (_206A_206E_200C_200C_202D_200C_206F_206D_206B_206B_206B_200E_206B_206B_200B_200B_202E_202D_202B_202B_200D_202B_206B_206E_206F_202D_202A_202A_206E_200B_200E_200E_202E_206B_202D_206A_200C_202A_206A_200C_202E(terrainData) < 1)
							{
								num5 = -399431908;
								num6 = num5;
							}
							else
							{
								num5 = -402897668;
								num6 = num5;
							}
							num = num5 ^ ((int)num2 * -1824390860);
							continue;
						}
						case 8u:
							num8 = _206F_202A_202D_206E_206C_200F_206F_200C_202B_202C_206E_206B_206C_206A_206A_202A_200F_202A_202D_206A_200F_200F_202E_200F_202E_206F_202D_202B_200F_202A_206E_200F_206D_200C_202D_206D_202E_200B_202D_206D_202E((Array)array, 0);
							num = (int)(num2 * 1423356553) ^ -42990907;
							continue;
						case 17u:
							goto IL_014b;
						case 2u:
							return true;
						case 10u:
							num9 = 0;
							num = (int)((num2 * 1283523234) ^ 0x2ABEA976);
							continue;
						case 6u:
							num14++;
							num = -946146989;
							continue;
						case 3u:
						{
							int num10;
							int num11;
							if (_202D_206A_206F_202D_202B_200B_202A_202E_200F_202D_202C_206C_206C_206B_200C_206B_202C_206F_206B_200F_200F_200B_206F_200F_202A_202E_206F_202A_206C_202C_202A_206F_200C_200C_200D_206A_200C_206B_206D_202B_202E(_202A_206B_206A_200E_206E_206B_200B_202D_200F_202D_200B_206F_206E_206A_206B_202B_200F_206D_202B_202C_202E_206D_206A_200B_200D_200C_200C_202B_200D_206C_200B_202C_206E_206B_200D_200F_202E_206E_202B_206A_202E(terrainData)) >= 1)
							{
								num10 = -857608198;
								num11 = num10;
							}
							else
							{
								num10 = -1472405594;
								num11 = num10;
							}
							num = num10 ^ ((int)num2 * -627114962);
							continue;
						}
						case 13u:
							goto IL_01cf;
						case 12u:
							num9++;
							num = (int)((num2 * 152437466) ^ 0x294DBB42);
							continue;
						case 4u:
						{
							int num3;
							int num4;
							if (!_206B_202B_202E_206A_202E_206E_206F_202C_200B_202C_202C_202A_200F_202E_200C_206E_202B_200C_200F_206B_202B_200C_200D_202C_206F_200D_200D_206C_202A_206C_206E_202A_200F_200F_200B_206F_202A_202B_206A_202B_202E((UnityEngine.Object)_202A_206B_206A_200E_206E_206B_200B_202D_200F_202D_200B_206F_206E_206A_206B_202B_200F_206D_202B_202C_202E_206D_206A_200B_200D_200C_200C_202B_200D_206C_200B_202C_206E_206B_200D_200F_202E_206E_202B_206A_202E(terrainData), (UnityEngine.Object)null))
							{
								num3 = 1406276429;
								num4 = num3;
							}
							else
							{
								num3 = 871918534;
								num4 = num3;
							}
							num = num3 ^ (int)(num2 * 858649773);
							continue;
						}
						default:
							return false;
						}
						break;
						IL_01cf:
						int num15;
						if (num9 >= num8)
						{
							num = -1179001590;
							num15 = num;
						}
						else
						{
							num = -136522956;
							num15 = num;
						}
						continue;
						IL_014b:
						int num16;
						if (!array[num9, num14])
						{
							num = -80999273;
							num16 = num;
						}
						else
						{
							num = -1894344273;
							num16 = num;
						}
						continue;
						IL_0091:
						int num17;
						if (num14 < num7)
						{
							num = -763321222;
							num17 = num;
						}
						else
						{
							num = -659901409;
							num17 = num;
						}
					}
					continue;
					end_IL_000c:
					break;
				}
			}
			return false;
		}

		public static bool HasTextureAlphaChannel(Texture2D texture)
		{
			return _200F_202C_202D_202A_206A_200B_200B_202C_200E_200C_200F_206F_200D_200C_206C_206E_200E_206F_206B_206B_202C_200F_206B_200F_200C_206F_200D_202D_206E_206C_202C_200F_202B_202A_200C_202A_202A_206C_200D_206F_202E._206D_202C_200D_206F_200B_206C_202E_206B_202C_206E_200B_200F_202B_202B_206A_202A_202A_202D_200D_202C_202D_206C_202D_200B_202A_200D_206D_206D_206C_206B_200D_200C_206D_206D_202A_200B_202D_206D_206F_202E_202E._206F_206F_200B_206C_202C_202D_200B_206F_200F_202E_202B_202E_206B_202E_206D_202B_206B_200F_206E_200E_200C_206A_206D_200C_202D_206F_200F_202A_200F_202B_202B_200D_200F_202A_200C_200E_202E_200F_202D_206F_202E(texture);
		}

		public static void SetupAlphaTestProperties(Material material)
		{
			if (_206B_202B_202E_206A_202E_206E_206F_202C_200B_202C_202C_202A_200F_202E_200C_206E_202B_200C_200F_206B_202B_200C_200D_202C_206F_200D_200D_206C_202A_206C_206E_202A_200F_200F_200B_206F_202A_202B_206A_202B_202E((UnityEngine.Object)material, (UnityEngine.Object)null))
			{
				goto IL_000c;
			}
			goto IL_0121;
			IL_000c:
			int num = -1811309588;
			goto IL_0011;
			IL_0011:
			while (true)
			{
				uint num2;
				switch ((num2 = (uint)num ^ 0x8EB6C17Au) % 12)
				{
				case 0u:
					break;
				default:
					return;
				case 2u:
					_202C_202D_206B_206D_200F_200E_200F_202A_200F_202B_202A_206A_202D_202B_200C_206B_206C_200D_200B_202A_202D_206E_200F_202D_200E_202D_202D_206E_200F_202C_206B_202A_206E_206E_200F_202C_200F_206D_202A_206F_202E(material, "_AlphaCutoffEnable", 1f);
					num = -829734049;
					continue;
				case 11u:
					_200B_202B_200C_200B_202B_200B_200E_202B_200C_206E_200F_202C_202B_200F_206B_206E_202C_206D_202B_202E_202D_206F_200B_202D_202E_202E_206D_206E_206D_200B_202D_202C_206F_206A_206C_206E_200D_206F_202C_200D_202E(material, 2450);
					switch (GetCurrentRenderPipeline())
					{
					case RenderPipeline.HighDefinition:
						goto IL_00d2;
					case RenderPipeline.Builtin:
						goto IL_00f3;
					case RenderPipeline.Universal:
						goto IL_0158;
					}
					num = ((int)num2 * -935564496) ^ 0x30E8FC66;
					continue;
				case 5u:
					_202C_200B_206E_202C_202A_200F_200B_206A_206B_202A_202D_206C_206D_206C_206A_200B_202A_202E_202C_206B_202D_206A_200E_200B_200B_202A_202A_206E_200E_200F_202C_200F_206A_202A_200B_206B_202B_202A_206E_200F_202E(material, "_DISABLE_SSR_TRANSPARENT");
					num = ((int)num2 * -399825339) ^ 0x31BC4E8D;
					continue;
				case 8u:
					return;
				case 9u:
					goto IL_00d2;
				case 6u:
					goto IL_00f3;
				case 10u:
					return;
				case 3u:
					goto IL_0121;
				case 1u:
					_202C_202D_206B_206D_200F_200E_200F_202A_200F_202B_202A_206A_202D_202B_200C_206B_206C_200D_200B_202A_202D_206E_200F_202D_200E_202D_202D_206E_200F_202C_206B_202A_206E_206E_200F_202C_200F_206D_202A_206F_202E(material, "_ZTestGBuffer", 3f);
					num = (int)(num2 * 1598498325) ^ -1079640528;
					continue;
				case 4u:
					goto IL_0158;
				case 7u:
					return;
					IL_0158:
					_202C_202D_206B_206D_200F_200E_200F_202A_200F_202B_202A_206A_202D_202B_200C_206B_206C_200D_200B_202A_202D_206E_200F_202D_200E_202D_202D_206E_200F_202C_206B_202A_206E_206E_200F_202C_200F_206D_202A_206F_202E(material, "_AlphaClip", 1f);
					return;
					IL_00f3:
					_202C_202D_206B_206D_200F_200E_200F_202A_200F_202B_202A_206A_202D_202B_200C_206B_206C_200D_200B_202A_202D_206E_200F_202D_200E_202D_202D_206E_200F_202C_206B_202A_206E_206E_200F_202C_200F_206D_202A_206F_202E(material, "_Mode", 1f);
					return;
				}
				break;
				IL_00d2:
				int num3;
				if (!_200C_206B_202C_200B_200C_206A_206C_200C_200D_200B_206A_206C_206B_206B_200D_202A_200F_206F_206B_206F_206A_202A_206F_206F_206D_206D_200B_202A_206A_200F_200D_202B_206C_200D_200B_206D_206F_200B_202D_206A_202E(material, "_DISABLE_SSR_TRANSPARENT"))
				{
					num = -1984873253;
					num3 = num;
				}
				else
				{
					num = -1486615320;
					num3 = num;
				}
			}
			goto IL_000c;
			IL_0121:
			_202C_200B_206E_202C_202A_200F_200B_206A_206B_202A_202D_206C_206D_206C_206A_200B_202A_202E_202C_206B_202D_206A_200E_200B_200B_202A_202A_206E_200E_200F_202C_200F_206A_202A_200B_206B_202B_202A_206E_200F_202E(material, "_ALPHATEST_ON");
			num = -1573474495;
			goto IL_0011;
		}

		public static string ConvertMeshToOBJ(Mesh mesh)
		{
			return ConvertMeshToOBJ(new Mesh[1] { mesh });
		}

		public static string ConvertMeshToOBJ(Mesh[] mesh)
		{
			if (mesh != null)
			{
				int num3 = default(int);
				int num4 = default(int);
				Vector3[] array = default(Vector3[]);
				int[] array2 = default(int[]);
				Vector3[] array3 = default(Vector3[]);
				Vector2[] array4 = default(Vector2[]);
				string text = default(string);
				StringBuilder stringBuilder = default(StringBuilder);
				while (true)
				{
					int num = -2073238850;
					while (true)
					{
						uint num2;
						switch ((num2 = (uint)num ^ 0x8A270303u) % 13)
						{
						case 8u:
							break;
						case 11u:
							goto IL_0052;
						case 6u:
							goto end_IL_0003;
						case 9u:
						{
							int num5;
							int num6;
							if (mesh.Length == 0)
							{
								num5 = 1491132810;
								num6 = num5;
							}
							else
							{
								num5 = 576801098;
								num6 = num5;
							}
							num = num5 ^ (int)(num2 * 1590441514);
							continue;
						}
						case 0u:
							num3 = 0;
							num4 = 0;
							num = (int)(num2 * 43440092) ^ -2089394855;
							continue;
						case 1u:
							_200D_206F_202A_202C_202B_202A_202B_206E_206D_206F_202E_202B_206B_200D_200B_206A_200D_206D_202C_206D_202A_202D_200C_202B_202D_202A_206D_202E_200C_206C_206C_206C_200E_202E_202D_200D_200E_202C_202C_200F_202E(mesh[num4], false);
							num = ((int)num2 * -1387282739) ^ 0x688575F4;
							continue;
						case 3u:
							num = (int)(num2 * 1297685008) ^ -1685366493;
							continue;
						case 2u:
							array = _206C_202C_200F_206D_202A_202E_206B_200C_200E_202D_202D_206A_206D_206B_202D_200C_200C_202C_200F_206E_202B_206B_206A_202C_206F_206A_200C_206E_206B_202C_200F_206B_206A_202C_206B_202D_202C_206E_206C_200D_202E(mesh[num4]);
							array2 = _202E_200D_206E_200D_200D_206E_200F_206C_200C_206C_202B_200E_200F_200B_200C_202C_200D_206B_206B_206B_206A_202B_200C_206C_206A_202E_200C_206E_206C_206C_202B_200B_200B_206F_202E_200C_200C_200C_200D_200C_202E(mesh[num4]);
							array3 = _200B_206A_202B_202D_202C_200B_200F_200B_202C_206A_200E_202B_202E_202E_200C_202B_200F_200D_202C_200E_200F_200F_202E_200F_200B_202C_202D_202E_202D_200D_206B_206C_200F_200B_202E_206E_202D_202A_206E_206E_202E(mesh[num4]);
							array4 = _200B_206D_202B_206B_206C_200F_200D_202B_206F_200B_200C_202B_200C_202D_202A_200F_206C_206C_206C_200C_200E_206A_202D_202E_206F_200F_200F_202C_206D_200E_200F_202B_200B_206E_200C_202C_206A_206A_200E_202A_202E(mesh[num4]);
							num = (int)(num2 * 1846584585) ^ -683515891;
							continue;
						case 10u:
							text = _200C_200C_202D_202C_206F_200F_202B_200E_206C_202A_202C_200E_200D_200D_200F_206B_202C_200D_200D_206D_206A_202A_206E_200E_202B_202D_206D_206A_206D_206C_206D_202B_206C_202B_202A_200D_202E_200E_202C_200C_202E((UnityEngine.Object)mesh[num4]);
							num = -1470519789;
							continue;
						case 4u:
							stringBuilder = _202C_206D_206F_206C_202C_206D_206B_202C_200E_206A_200E_202E_200B_206B_202C_206C_200E_206B_200D_202C_200C_202A_202C_202E_206B_200C_206E_200C_206A_206C_202E_202E_206D_206B_202D_206B_200B_202E_202C_206D_202E();
							_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _206B_206E_200B_202E_202A_206E_202B_206F_202B_202C_206E_206D_206F_202C_202D_200B_200C_200D_206F_206E_202B_202B_200C_206C_206D_206A_206E_206D_200D_206F_200E_202E_206B_200C_202C_206B_200E_206F_206E_206F_202E());
							num = -1616986115;
							continue;
						case 12u:
							mesh[num4] = null;
							num4++;
							num = (int)(num2 * 664273673) ^ -684224721;
							continue;
						case 5u:
							_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _202E_206A_206B_202D_202D_202B_200C_202B_206C_200E_200D_206A_200C_200B_206C_202A_206D_200B_206E_200F_202B_206C_200B_202D_206A_202D_206E_200D_202B_200C_202E_206C_202B_206F_206A_206B_200D_206C_202B_202D_202E(null, text, num3, array, array2, array3, array4));
							num3 += _202D_206C_202E_202C_206B_200E_200E_206C_200D_200B_206A_202A_206E_202A_200D_206D_200B_206F_200E_206C_202D_206B_202A_200D_200D_200B_202B_202A_206E_200F_206E_202C_202A_202B_200D_206D_206A_202B_206C_206E_202E(mesh[num4]);
							num = ((int)num2 * -705318478) ^ -288082440;
							continue;
						default:
							return _206C_206C_206D_206D_202B_206F_200F_200E_202D_200E_202E_206D_206F_202E_200C_202D_200F_202C_202C_202D_200F_206D_206D_206C_206B_200D_206F_202B_200F_200F_202B_206C_206A_200F_202C_206C_206B_200C_206D_206E_202E((object)stringBuilder);
						}
						break;
						IL_0052:
						int num7;
						if (num4 < mesh.Length)
						{
							num = -2099929622;
							num7 = num;
						}
						else
						{
							num = -1715464138;
							num7 = num;
						}
					}
					continue;
					end_IL_0003:
					break;
				}
			}
			return string.Empty;
		}

		public static void ConvertMeshToOBJ(Mesh mesh, StreamWriter streamWriter)
		{
			ConvertMeshToOBJ(new Mesh[1] { mesh }, streamWriter);
		}

		public static void ConvertMeshToOBJ(Mesh[] meshes, StreamWriter streamWriter)
		{
			if (meshes == null)
			{
				return;
			}
			string text = default(string);
			int num4 = default(int);
			Vector3[] array4 = default(Vector3[]);
			int num3 = default(int);
			while (true)
			{
				int num = -1731514160;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0x8EDA8A61u) % 11)
					{
					case 6u:
						break;
					default:
						return;
					case 8u:
					{
						int num6;
						int num7;
						if (meshes.Length != 0)
						{
							num6 = 292003130;
							num7 = num6;
						}
						else
						{
							num6 = 211995875;
							num7 = num6;
						}
						num = num6 ^ (int)(num2 * 53520256);
						continue;
					}
					case 0u:
						text = _200C_200C_202D_202C_206F_200F_202B_200E_206C_202A_202C_200E_200D_200D_200F_206B_202C_200D_200D_206D_206A_202A_206E_200E_202B_202D_206D_206A_206D_206C_206D_202B_206C_202B_202A_200D_202E_200E_202C_200C_202E((UnityEngine.Object)meshes[num4]);
						array4 = _206C_202C_200F_206D_202A_202E_206B_200C_200E_202D_202D_206A_206D_206B_202D_200C_200C_202C_200F_206E_202B_206B_206A_202C_206F_206A_200C_206E_206B_202C_200F_206B_206A_202C_206B_202D_202C_206E_206C_200D_202E(meshes[num4]);
						num = -1733481;
						continue;
					case 4u:
						_206E_202B_202C_200C_200C_202C_206F_202C_200E_200D_202A_202D_200C_202B_202B_200C_202C_202B_206F_202D_202E_206A_206F_200C_200C_206A_206B_206D_206A_206F_206E_202D_200F_200D_206A_206F_200F_200C_206E_206F_202E((TextWriter)streamWriter, _206B_206E_200B_202E_202A_206E_202B_206F_202B_202C_206E_206D_206F_202C_202D_200B_200C_200D_206F_206E_202B_202B_200C_206C_206D_206A_206E_206D_200D_206F_200E_202E_206B_200C_202C_206B_200E_206F_206E_206F_202E());
						num4 = 0;
						num = ((int)num2 * -544904891) ^ -1483169366;
						continue;
					case 2u:
					{
						int num5;
						if (num4 >= meshes.Length)
						{
							num = -380766916;
							num5 = num;
						}
						else
						{
							num = -615743495;
							num5 = num;
						}
						continue;
					}
					case 10u:
					{
						int[] array = _202E_200D_206E_200D_200D_206E_200F_206C_200C_206C_202B_200E_200F_200B_200C_202C_200D_206B_206B_206B_206A_202B_200C_206C_206A_202E_200C_206E_206C_206C_202B_200B_200B_206F_202E_200C_200C_200C_200D_200C_202E(meshes[num4]);
						Vector3[] array2 = _200B_206A_202B_202D_202C_200B_200F_200B_202C_206A_200E_202B_202E_202E_200C_202B_200F_200D_202C_200E_200F_200F_202E_200F_200B_202C_202D_202E_202D_200D_206B_206C_200F_200B_202E_206E_202D_202A_206E_206E_202E(meshes[num4]);
						Vector2[] array3 = _200B_206D_202B_206B_206C_200F_200D_202B_206F_200B_200C_202B_200C_202D_202A_200F_206C_206C_206C_200C_200E_206A_202D_202E_206F_200F_200F_202C_206D_200E_200F_202B_200B_206E_200C_202C_206A_206A_200E_202A_202E(meshes[num4]);
						_202E_206A_206B_202D_202D_202B_200C_202B_206C_200E_200D_206A_200C_200B_206C_202A_206D_200B_206E_200F_202B_206C_200B_202D_206A_202D_206E_200D_202B_200C_202E_206C_202B_206F_206A_206B_200D_206C_202B_202D_202E(streamWriter, text, num3, array4, array, array2, array3);
						num = ((int)num2 * -291729831) ^ -419384779;
						continue;
					}
					case 7u:
						num4++;
						num = ((int)num2 * -258633484) ^ 0x3110BB8C;
						continue;
					case 9u:
						num3 += _202D_206C_202E_202C_206B_200E_200E_206C_200D_200B_206A_202A_206E_202A_200D_206D_200B_206F_200E_206C_202D_206B_202A_200D_200D_200B_202B_202A_206E_200F_206E_202C_202A_202B_200D_206D_206A_202B_206C_206E_202E(meshes[num4]);
						num = (int)((num2 * 1443126389) ^ 0x7B70A2C);
						continue;
					case 3u:
						return;
					case 1u:
						num3 = 0;
						num = -507054941;
						continue;
					case 5u:
						return;
					}
					break;
				}
			}
		}

		private static string _206B_206E_200B_202E_202A_206E_202B_206F_202B_202C_206E_206D_206F_202C_202D_200B_200C_200D_206F_206E_202B_202B_200C_206C_206D_206A_206E_206D_200D_206F_200E_202E_206B_200C_202C_206B_200E_206F_206E_206F_202E()
		{
			return _200E_206B_206D_206E_206B_206A_200B_200B_202D_200E_206D_206F_200E_200E_206A_206B_200C_200F_200D_200D_200B_202C_200F_200C_200C_206B_200F_206B_206B_202A_206B_206C_200E_206E_206F_200C_202A_206E_206D_200E_202E(new string[6]
			{
				"# Amazing Assets - Terrain To OBJ",
				_200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E(),
				"# File Created: ",
				DateTime.Now.ToString("dd/MMMM/yyyy HH:mm:ss"),
				_200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E(),
				_200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E()
			});
		}

		private static string _202E_206A_206B_202D_202D_202B_200C_202B_206C_200E_200D_206A_200C_200B_206C_202A_206D_200B_206E_200F_202B_206C_200B_202D_206A_202D_206E_200D_202B_200C_202E_206C_202B_206F_206A_206B_200D_206C_202B_202D_202E(StreamWriter P_0, string P_1, int P_2, Vector3[] P_3, int[] P_4, Vector3[] P_5, Vector2[] P_6)
		{
			string empty = string.Empty;
			string text = " ";
			int num11 = default(int);
			int num12 = default(int);
			StringBuilder stringBuilder = default(StringBuilder);
			int num3 = default(int);
			int num6 = default(int);
			string text2 = default(string);
			while (true)
			{
				int num = -337033304;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0xA6FE7534u) % 69)
					{
					case 67u:
						break;
					case 41u:
					{
						int num16;
						int num17;
						if (P_0 == null)
						{
							num16 = 1489937763;
							num17 = num16;
						}
						else
						{
							num16 = 1488081598;
							num17 = num16;
						}
						num = num16 ^ (int)(num2 * 32169194);
						continue;
					}
					case 26u:
					{
						int num24;
						if (num11 >= P_5.Length)
						{
							num = -535272147;
							num24 = num;
						}
						else
						{
							num = -1371935387;
							num24 = num;
						}
						continue;
					}
					case 21u:
						num12 = 0;
						num = ((int)num2 * -713133871) ^ 0x3292302E;
						continue;
					case 0u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						num = ((int)num2 * -1865541153) ^ 0xD69207A;
						continue;
					case 58u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text);
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _206C_206B_206E_206D_206C_200C_200C_200B_206D_200D_200F_206E_206F_206E_200B_206E_202D_200E_202A_202D_202A_206C_206A_206B_200F_200C_206F_206D_206F_202E_200D_200D_200D_202D_202D_200F_202A_202A_200D_202B_202E(P_3[num3].z, empty));
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						num = ((int)num2 * -2122213963) ^ -780114964;
						continue;
					case 23u:
						_206E_202B_202C_200C_200C_202C_206F_202C_200E_200D_202A_202D_200C_202B_202B_200C_202C_202B_206F_202D_202E_206A_206F_200C_200C_206A_206B_206D_206A_206F_206E_202D_200F_200D_206A_206F_200F_200C_206E_206F_202E((TextWriter)P_0, _206C_206C_206D_206D_202B_206F_200F_200E_202D_200E_202E_206D_206F_202E_200C_202D_200F_202C_202C_202D_200F_206D_206D_206C_206B_200D_206F_202B_200F_200F_202B_206C_206A_200F_202C_206C_206B_200C_206D_206E_202E((object)stringBuilder));
						num = ((int)num2 * -1707076417) ^ -1041627031;
						continue;
					case 65u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200C_200D_200E_200D_206C_202E_206A_202E_202A_206A_206C_206A_202E_202B_206A_200C_200D_206F_200E_200B_200F_200F_200F_200E_206C_206D_202B_206C_202A_202A_202C_200D_200D_200D_206A_206B_206C_202B_202C_202A_202E("o ", P_1, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E()));
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200C_200D_200E_200D_206C_202E_206A_202E_202A_206A_206C_206A_202E_202B_206A_200C_200D_206F_200E_200B_200F_200F_200F_200E_206C_206D_202B_206C_202A_202A_202C_200D_200D_200D_206A_206B_206C_202B_202C_202A_202E("g ", P_1, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E()));
						num = ((int)num2 * -246214421) ^ -1701469188;
						continue;
					case 22u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, "f ");
						_202C_202E_202D_206B_202B_206D_206C_206C_200D_206A_206F_206D_200E_200D_202E_202C_200F_206B_200E_206D_202B_202B_200D_200F_206F_206B_200E_202C_202A_206A_200C_202E_202A_206C_202B_202E_202B_206C_206A_206A_202E(stringBuilder, P_4[num12 + 2] + 1 + P_2);
						num = -1993770294;
						continue;
					case 49u:
					{
						int num7;
						int num8;
						if (num6 % 10 == 0)
						{
							num7 = -1264324710;
							num8 = num7;
						}
						else
						{
							num7 = -1984686669;
							num8 = num7;
						}
						num = num7 ^ ((int)num2 * -1861037624);
						continue;
					}
					case 30u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text2);
						num = (int)(num2 * 1108877881) ^ -1266713758;
						continue;
					case 18u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _206C_206B_206E_206D_206C_200C_200C_200B_206D_200D_200F_206E_206F_206E_200B_206E_202D_200E_202A_202D_202A_206C_206A_206B_200F_200C_206F_206D_206F_202E_200D_200D_200D_202D_202D_200F_202A_202A_200D_202B_202E(P_3[num3].y, empty));
						num = (int)((num2 * 523143142) ^ 0x545B2DB7);
						continue;
					case 50u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text);
						_202C_202E_202D_206B_202B_206D_206C_206C_200D_206A_206F_206D_200E_200D_202E_202C_200F_206B_200E_206D_202B_202B_200D_200F_206F_206B_200E_202C_202A_206A_200C_202E_202A_206C_202B_202E_202B_206C_206A_206A_202E(stringBuilder, P_4[num12] + 1 + P_2);
						num = (int)((num2 * 521121381) ^ 0x350E12E8);
						continue;
					case 63u:
					{
						int num30;
						if (num12 >= P_4.Length)
						{
							num = -2030955288;
							num30 = num;
						}
						else
						{
							num = -1297825929;
							num30 = num;
						}
						continue;
					}
					case 38u:
						_202C_202E_202D_206B_202B_206D_206C_206C_200D_206A_206F_206D_200E_200D_202E_202C_200F_206B_200E_206D_202B_202B_200D_200F_206F_206B_200E_202C_202A_206A_200C_202E_202A_206C_202B_202E_202B_206C_206A_206A_202E(stringBuilder, P_4[num12 + 1] + 1 + P_2);
						num = (int)((num2 * 1123125451) ^ 0x5AC55FEA);
						continue;
					case 48u:
						_206B_200E_202B_200D_206F_200D_202E_200C_200F_202C_200B_200D_200B_200B_206E_202E_202C_206B_206D_202B_206A_200C_200F_206F_206B_202B_206A_200B_200F_200E_200E_206B_202C_200B_200C_202B_202B_206C_202D_200D_202E(stringBuilder, 0);
						num = ((int)num2 * -221752058) ^ 0x5C16C065;
						continue;
					case 3u:
						num3++;
						num = -1034008515;
						continue;
					case 53u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						num = ((int)num2 * -695142199) ^ 0x1B6238E3;
						continue;
					case 37u:
					{
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, "0");
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						int num22;
						int num23;
						if (P_0 != null)
						{
							num22 = -479841905;
							num23 = num22;
						}
						else
						{
							num22 = -1891544082;
							num23 = num22;
						}
						num = num22 ^ ((int)num2 * -742902903);
						continue;
					}
					case 42u:
						num6++;
						num = -1600691812;
						continue;
					case 1u:
					{
						int num18;
						int num19;
						if (num11 % 10 != 0)
						{
							num18 = 952380377;
							num19 = num18;
						}
						else
						{
							num18 = 1754413916;
							num19 = num18;
						}
						num = num18 ^ ((int)num2 * -1080645667);
						continue;
					}
					case 4u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, "vt ");
						num = -2137429672;
						continue;
					case 24u:
					{
						int num9;
						int num10;
						if (P_5.Length == P_3.Length)
						{
							num9 = 653728464;
							num10 = num9;
						}
						else
						{
							num9 = 715128091;
							num10 = num9;
						}
						num = num9 ^ (int)(num2 * 1855022372);
						continue;
					}
					case 19u:
						_206E_202B_202C_200C_200C_202C_206F_202C_200E_200D_202A_202D_200C_202B_202B_200C_202C_202B_206F_202D_202E_206A_206F_200C_200C_206A_206B_206D_206A_206F_206E_202D_200F_200D_206A_206F_200F_200C_206E_206F_202E((TextWriter)P_0, _206C_206C_206D_206D_202B_206F_200F_200E_202D_200E_202E_206D_206F_202E_200C_202D_200F_202C_202C_202D_200F_206D_206D_206C_206B_200D_206F_202B_200F_200F_202B_206C_206A_200F_202C_206C_206B_200C_206D_206E_202E((object)stringBuilder));
						_206B_200E_202B_200D_206F_200D_202E_200C_200F_202C_200B_200D_200B_200B_206E_202E_202C_206B_206D_202B_206A_200C_200F_206F_206B_202B_206A_200B_200F_200E_200E_206B_202C_200B_200C_202B_202B_206C_202D_200D_202E(stringBuilder, 0);
						num = (int)((num2 * 1457546308) ^ 0x594ADB77);
						continue;
					case 46u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text);
						num = ((int)num2 * -731662644) ^ -443327359;
						continue;
					case 28u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, "#"), _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						num = ((int)num2 * -1234434781) ^ 0x6BAE57C2;
						continue;
					case 5u:
						_206E_202B_202C_200C_200C_202C_206F_202C_200E_200D_202A_202D_200C_202B_202B_200C_202C_202B_206F_202D_202E_206A_206F_200C_200C_206A_206B_206D_206A_206F_206E_202D_200F_200D_206A_206F_200F_200C_206E_206F_202E((TextWriter)P_0, _206C_206C_206D_206D_202B_206F_200F_200E_202D_200E_202E_206D_206F_202E_200C_202D_200F_202C_202C_202D_200F_206D_206D_206C_206B_200D_206F_202B_200F_200F_202B_206C_206A_200F_202C_206C_206B_200C_206D_206E_202E((object)stringBuilder));
						_206B_200E_202B_200D_206F_200D_202E_200C_200F_202C_200B_200D_200B_200B_206E_202E_202C_206B_206D_202B_206A_200C_200F_206F_206B_202B_206A_200B_200F_200E_200E_206B_202C_200B_200C_202B_202B_206C_202D_200D_202E(stringBuilder, 0);
						num = ((int)num2 * -1350418243) ^ 0x6EDF1741;
						continue;
					case 17u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text2);
						num = ((int)num2 * -668120025) ^ -762551360;
						continue;
					case 40u:
						num3 = 0;
						num = ((int)num2 * -966600673) ^ 0x54C7F71E;
						continue;
					case 52u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _206C_206B_206E_206D_206C_200C_200C_200B_206D_200D_200F_206E_206F_206E_200B_206E_202D_200E_202A_202D_202A_206C_206A_206B_200F_200C_206F_206D_206F_202E_200D_200D_200D_202D_202D_200F_202A_202A_200D_202B_202E(P_6[num6].x, empty));
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text);
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _206C_206B_206E_206D_206C_200C_200C_200B_206D_200D_200F_206E_206F_206E_200B_206E_202D_200E_202A_202D_202A_206C_206A_206B_200F_200C_206F_206D_206F_202E_200D_200D_200D_202D_202D_200F_202A_202A_200D_202B_202E(P_6[num6].y, empty));
						num = ((int)num2 * -421325067) ^ 0x2EC37E9C;
						continue;
					case 15u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200C_200D_200E_200D_206C_202E_206A_202E_202A_206A_206C_206A_202E_202B_206A_200C_200D_206F_200E_200B_200F_200F_200F_200E_206C_206D_202B_206C_202A_202A_202C_200D_200D_200D_206A_206B_206C_202B_202C_202A_202E("# ", P_3.Length.ToString(), " vertices"));
						num = ((int)num2 * -659749109) ^ -715776265;
						continue;
					case 56u:
						num11 = 0;
						num = (int)(num2 * 1047713128) ^ -275067418;
						continue;
					case 68u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _206C_206B_206E_206D_206C_200C_200C_200B_206D_200D_200F_206E_206F_206E_200B_206E_202D_200E_202A_202D_202A_206C_206A_206B_200F_200C_206F_206D_206F_202E_200D_200D_200D_202D_202D_200F_202A_202A_200D_202B_202E(P_5[num11].y, empty));
						num = (int)((num2 * 1295462174) ^ 0x601494E);
						continue;
					case 9u:
						_206E_202B_202C_200C_200C_202C_206F_202C_200E_200D_202A_202D_200C_202B_202B_200C_202C_202B_206F_202D_202E_206A_206F_200C_200C_206A_206B_206D_206A_206F_206E_202D_200F_200D_206A_206F_200F_200C_206E_206F_202E((TextWriter)P_0, _206C_206C_206D_206D_202B_206F_200F_200E_202D_200E_202E_206D_206F_202E_200C_202D_200F_202C_202C_202D_200F_206D_206D_206C_206B_200D_206F_202B_200F_200F_202B_206C_206A_200F_202C_206C_206B_200C_206D_206E_202E((object)stringBuilder));
						_206B_200E_202B_200D_206F_200D_202E_200C_200F_202C_200B_200D_200B_200B_206E_202E_202C_206B_206D_202B_206A_200C_200F_206F_206B_202B_206A_200B_200F_200E_200E_206B_202C_200B_200C_202B_202B_206C_202D_200D_202E(stringBuilder, 0);
						num = ((int)num2 * -644464805) ^ -1930629207;
						continue;
					case 57u:
						return string.Empty;
					case 62u:
						num6 = 0;
						num = -1883136413;
						continue;
					case 60u:
					{
						int num31;
						int num32;
						if (num12 % 10 == 0)
						{
							num31 = -165938077;
							num32 = num31;
						}
						else
						{
							num31 = -579106365;
							num32 = num31;
						}
						num = num31 ^ (int)(num2 * 736089944);
						continue;
					}
					case 6u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, "#"), _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						num = (int)(num2 * 748392925) ^ -771645641;
						continue;
					case 66u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text2);
						num = (int)(num2 * 1574981009) ^ -1214890809;
						continue;
					case 34u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200C_200D_200E_200D_206C_202E_206A_202E_202A_206A_206C_206A_202E_202B_206A_200C_200D_206F_200E_200B_200F_200F_200F_200E_206C_206D_202B_206C_202A_202A_202C_200D_200D_200D_206A_206B_206C_202B_202C_202A_202E("# ", (P_4.Length / 3).ToString(), " faces"));
						num = (int)((num2 * 2599062) ^ 0x65A61A73);
						continue;
					case 25u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, "v ");
						num = -1834524905;
						continue;
					case 12u:
						num11++;
						num = -963086367;
						continue;
					case 45u:
						_202C_202E_202D_206B_202B_206D_206C_206C_200D_206A_206F_206D_200E_200D_202E_202C_200F_206B_200E_206D_202B_202B_200D_200F_206F_206B_200E_202C_202A_206A_200C_202E_202A_206C_202B_202E_202B_206C_206A_206A_202E(stringBuilder, P_4[num12 + 1] + 1 + P_2);
						num = (int)((num2 * 331953354) ^ 0x7527AA4D);
						continue;
					case 59u:
					{
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						int num28;
						int num29;
						if (P_0 == null)
						{
							num28 = 1363584183;
							num29 = num28;
						}
						else
						{
							num28 = 561426996;
							num29 = num28;
						}
						num = num28 ^ (int)(num2 * 1619925411);
						continue;
					}
					case 10u:
						_202C_202E_202D_206B_202B_206D_206C_206C_200D_206A_206F_206D_200E_200D_202E_202C_200F_206B_200E_206D_202B_202B_200D_200F_206F_206B_200E_202C_202A_206A_200C_202E_202A_206C_202B_202E_202B_206C_206A_206A_202E(stringBuilder, P_4[num12 + 2] + 1 + P_2);
						_202C_202E_202D_206B_202B_206D_206C_206C_200D_206A_206F_206D_200E_200D_202E_202C_200F_206B_200E_206D_202B_202B_200D_200F_206F_206B_200E_202C_202A_206A_200C_202E_202A_206C_202B_202E_202B_206C_206A_206A_202E(_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text2), P_4[num12 + 2] + 1 + P_2);
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text);
						_202C_202E_202D_206B_202B_206D_206C_206C_200D_206A_206F_206D_200E_200D_202E_202C_200F_206B_200E_206D_202B_202B_200D_200F_206F_206B_200E_202C_202A_206A_200C_202E_202A_206C_202B_202E_202B_206C_206A_206A_202E(stringBuilder, P_4[num12 + 1] + 1 + P_2);
						num = (int)(num2 * 1261278868) ^ -1842433810;
						continue;
					case 31u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, "# object "), P_1), _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						num = ((int)num2 * -1560713872) ^ 0x28189D56;
						continue;
					case 64u:
						_202C_202E_202D_206B_202B_206D_206C_206C_200D_206A_206F_206D_200E_200D_202E_202C_200F_206B_200E_206D_202B_202B_200D_200F_206F_206B_200E_202C_202A_206A_200C_202E_202A_206C_202B_202E_202B_206C_206A_206A_202E(stringBuilder, P_4[num12] + 1 + P_2);
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text2);
						_202C_202E_202D_206B_202B_206D_206C_206C_200D_206A_206F_206D_200E_200D_202E_202C_200F_206B_200E_206D_202B_202B_200D_200F_206F_206B_200E_202C_202A_206A_200C_202E_202A_206C_202B_202E_202B_206C_206A_206A_202E(stringBuilder, P_4[num12] + 1 + P_2);
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						num = ((int)num2 * -317087476) ^ 0x6B130160;
						continue;
					case 14u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, "vn ");
						num = -1045455888;
						continue;
					case 33u:
						_206B_200E_202B_200D_206F_200D_202E_200C_200F_202C_200B_200D_200B_200B_206E_202E_202C_206B_206D_202B_206A_200C_200F_206F_206B_202B_206A_200B_200F_200E_200E_206B_202C_200B_200C_202B_202B_206C_202D_200D_202E(stringBuilder, 0);
						num = (int)((num2 * 263788322) ^ 0x50276269);
						continue;
					case 8u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200C_200D_200E_200D_206C_202E_206A_202E_202A_206A_206C_206A_202E_202B_206A_200C_200D_206F_200E_200B_200F_200F_200F_200E_206C_206D_202B_206C_202A_202A_202C_200D_200D_200D_206A_206B_206C_202B_202C_202A_202E("# ", P_5.Length.ToString(), " normals"));
						num = (int)(num2 * 650340820) ^ -1900854668;
						continue;
					case 43u:
					{
						int num26;
						int num27;
						if (P_0 == null)
						{
							num26 = 30148683;
							num27 = num26;
						}
						else
						{
							num26 = 1303562471;
							num27 = num26;
						}
						num = num26 ^ (int)(num2 * 914345933);
						continue;
					}
					case 54u:
					{
						int num25;
						if (num3 >= P_3.Length)
						{
							num = -2013892188;
							num25 = num;
						}
						else
						{
							num = -367089246;
							num25 = num;
						}
						continue;
					}
					case 61u:
					{
						int num20;
						int num21;
						if (num3 % 10 != 0)
						{
							num20 = -250443701;
							num21 = num20;
						}
						else
						{
							num20 = -247891129;
							num21 = num20;
						}
						num = num20 ^ (int)(num2 * 1759910467);
						continue;
					}
					case 11u:
						num = (int)(num2 * 1670641503) ^ -260394289;
						continue;
					case 39u:
						text2 = "/";
						stringBuilder = _202C_206D_206F_206C_202C_206D_206B_202C_200E_206A_200E_202E_200B_206B_202C_206C_200E_206B_200D_202C_200C_202A_202C_202E_206B_200C_206E_200C_206A_206C_202E_202E_206D_206B_202D_206B_200B_202E_202C_206D_202E();
						num = ((int)num2 * -914955768) ^ 0x2E069D28;
						continue;
					case 51u:
						num = (int)(num2 * 2087078182) ^ -489300874;
						continue;
					case 7u:
						_206E_202B_202C_200C_200C_202C_206F_202C_200E_200D_202A_202D_200C_202B_202B_200C_202C_202B_206F_202D_202E_206A_206F_200C_200C_206A_206B_206D_206A_206F_206E_202D_200F_200D_206A_206F_200F_200C_206E_206F_202E((TextWriter)P_0, _206C_206C_206D_206D_202B_206F_200F_200E_202D_200E_202E_206D_206F_202E_200C_202D_200F_202C_202C_202D_200F_206D_206D_206C_206B_200D_206F_202B_200F_200F_202B_206C_206A_200F_202C_206C_206B_200C_206D_206E_202E((object)stringBuilder));
						num = ((int)num2 * -1803986441) ^ -1874687313;
						continue;
					case 36u:
					{
						int num14;
						int num15;
						if (P_0 != null)
						{
							num14 = 446603708;
							num15 = num14;
						}
						else
						{
							num14 = 1144206679;
							num15 = num14;
						}
						num = num14 ^ (int)(num2 * 284906874);
						continue;
					}
					case 47u:
					{
						int num13;
						if (num6 >= P_6.Length)
						{
							num = -128833142;
							num13 = num;
						}
						else
						{
							num = -607215096;
							num13 = num;
						}
						continue;
					}
					case 55u:
						num = ((int)num2 * -1982236956) ^ -303098903;
						continue;
					case 2u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text);
						num = ((int)num2 * -1806837419) ^ -1082643890;
						continue;
					case 35u:
						num12 += 3;
						num = -1020625856;
						continue;
					case 44u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200C_200D_200E_200D_206C_202E_206A_202E_202A_206A_206C_206A_202E_202B_206A_200C_200D_206F_200E_200B_200F_200F_200F_200E_206C_206D_202B_206C_202A_202A_202C_200D_200D_200D_206A_206B_206C_202B_202C_202A_202E("# ", P_6.Length.ToString(), " texture coords"));
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						num = (int)(num2 * 1058151554) ^ -436166502;
						continue;
					case 27u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _206C_206B_206E_206D_206C_200C_200C_200B_206D_200D_200F_206E_206F_206E_200B_206E_202D_200E_202A_202D_202A_206C_206A_206B_200F_200C_206F_206D_206F_202E_200D_200D_200D_202D_202D_200F_202A_202A_200D_202B_202E(P_5[num11].x * -1f, empty));
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text);
						num = (int)(num2 * 652521951) ^ -419040651;
						continue;
					case 29u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text2);
						num = (int)((num2 * 1549384404) ^ 0x88742B2);
						continue;
					case 13u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _206C_206B_206E_206D_206C_200C_200C_200B_206D_200D_200F_206E_206F_206E_200B_206E_202D_200E_202A_202D_202A_206C_206A_206B_200F_200C_206F_206D_206F_202E_200D_200D_200D_202D_202D_200F_202A_202A_200D_202B_202E(P_5[num11].z, empty));
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						num = (int)((num2 * 602481126) ^ 0x3BBADF6B);
						continue;
					case 16u:
					{
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E());
						int num4;
						int num5;
						if (P_5 == null)
						{
							num4 = -684767758;
							num5 = num4;
						}
						else
						{
							num4 = -1899800846;
							num5 = num4;
						}
						num = num4 ^ ((int)num2 * -526969433);
						continue;
					}
					case 20u:
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, _206C_206B_206E_206D_206C_200C_200C_200B_206D_200D_200F_206E_206F_206E_200B_206E_202D_200E_202A_202D_202A_206C_206A_206B_200F_200C_206F_206D_206F_202E_200D_200D_200D_202D_202D_200F_202A_202A_200D_202B_202E(P_3[num3].x * -1f, empty));
						_206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(stringBuilder, text);
						num = ((int)num2 * -1653336819) ^ 0x2C99DE68;
						continue;
					default:
						return _206C_206C_206D_206D_202B_206F_200F_200E_202D_200E_202E_206D_206F_202E_200C_202D_200F_202C_202C_202D_200F_206D_206D_206C_206B_200D_206F_202B_200F_200F_202B_206C_206A_200F_202C_206C_206B_200C_206D_206E_202E((object)stringBuilder);
					}
					break;
				}
			}
		}

		private static string _206C_206B_206E_206D_206C_200C_200C_200B_206D_200D_200F_206E_206F_206E_200B_206E_202D_200E_202A_202D_202A_206C_206A_206B_200F_200C_206F_206D_206F_202E_200D_200D_200D_202D_202D_200F_202A_202A_200D_202B_202E(float P_0, string P_1)
		{
			return P_0.ToString(P_1, _206B_206B_200B_206D_202A_206E_202D_200B_206E_206C_200C_202C_206A_200F_200D_206C_202A_202A_200E_202C_206C_200E_206E_200D_200B_206D_206B_206D_206A_200D_202C_206E_202C_206D_200E_206A_206B_200C_202E_202A_202E());
		}

		public static string GetUnityDefaultShader()
		{
			switch (GetCurrentRenderPipeline())
			{
			default:
				while (true)
				{
					int num = -598708136;
					while (true)
					{
						uint num2;
						switch ((num2 = (uint)num ^ 0xFCBA32F7u) % 5)
						{
						case 3u:
							break;
						case 2u:
							num = ((int)num2 * -1607158101) ^ -840428816;
							continue;
						case 1u:
							goto end_IL_0018;
						case 0u:
							goto IL_005e;
						default:
							goto end_IL_0007;
						}
						break;
					}
					continue;
					end_IL_0018:
					break;
				}
				goto case RenderPipeline.Universal;
			case RenderPipeline.Universal:
				return "Universal Render Pipeline/Lit";
			case RenderPipeline.HighDefinition:
				goto IL_005e;
			case RenderPipeline.Builtin:
				break;
				IL_005e:
				return "HDRP/Lit";
				end_IL_0007:
				break;
			}
			return "Standard";
		}

		public static string GetMaterailPropMainTex()
		{
			switch (GetCurrentRenderPipeline())
			{
			default:
				while (true)
				{
					int num = 1422989863;
					while (true)
					{
						uint num2;
						switch ((num2 = (uint)num ^ 0x4980547Au) % 5)
						{
						case 2u:
							break;
						case 1u:
							num = (int)(num2 * 1493133117) ^ -186608437;
							continue;
						case 4u:
							goto end_IL_0018;
						case 3u:
							goto IL_005e;
						default:
							goto end_IL_0007;
						}
						break;
					}
					continue;
					end_IL_0018:
					break;
				}
				goto case RenderPipeline.Universal;
			case RenderPipeline.Universal:
				return "_BaseMap";
			case RenderPipeline.HighDefinition:
				goto IL_005e;
			case RenderPipeline.Builtin:
				break;
				IL_005e:
				return "_BaseColorMap";
				end_IL_0007:
				break;
			}
			return "_MainTex";
		}

		public static string GetMaterailPropBumpMap()
		{
			RenderPipeline currentRenderPipeline = GetCurrentRenderPipeline();
			while (true)
			{
				int num = 508935812;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0x7ED4252Bu) % 6)
					{
					case 0u:
						break;
					case 5u:
						return "_BumpMap";
					case 1u:
						num = (int)(num2 * 1936988585) ^ -1920024160;
						continue;
					case 3u:
						switch (currentRenderPipeline)
						{
						case RenderPipeline.Universal:
							break;
						default:
							goto IL_0062;
						case RenderPipeline.HighDefinition:
							goto IL_0071;
						case RenderPipeline.Builtin:
							goto IL_007e;
						}
						goto case 5u;
					case 2u:
						goto IL_0071;
					default:
						goto IL_007e;
						IL_007e:
						return "_BumpMap";
						IL_0071:
						return "_NormalMap";
						IL_0062:
						num = (int)(num2 * 2107869397) ^ -519801295;
						continue;
					}
					break;
				}
			}
		}

		public static string GetMaterailPropOcclusionMap()
		{
			RenderPipeline currentRenderPipeline = GetCurrentRenderPipeline();
			while (true)
			{
				int num = -557322637;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0xA811F440u) % 6)
					{
					case 4u:
						break;
					case 1u:
						switch (currentRenderPipeline)
						{
						case RenderPipeline.Universal:
							goto IL_0064;
						case RenderPipeline.HighDefinition:
							goto IL_0071;
						case RenderPipeline.Builtin:
							goto IL_007e;
						}
						num = (int)(num2 * 1281338781) ^ -74929222;
						continue;
					case 5u:
						num = ((int)num2 * -1438990440) ^ -2067994412;
						continue;
					case 3u:
						goto IL_0064;
					case 2u:
						goto IL_0071;
					default:
						goto IL_007e;
						IL_007e:
						return "_OcclusionMap";
						IL_0071:
						return string.Empty;
						IL_0064:
						return "_OcclusionMap";
					}
					break;
				}
			}
		}

		public static string GetMaterailPropMetallicSmoothnessMap()
		{
			RenderPipeline currentRenderPipeline = GetCurrentRenderPipeline();
			while (true)
			{
				int num = -804244163;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0xF6DB7C5Eu) % 6)
					{
					case 3u:
						break;
					case 0u:
						return "_MetallicGlossMap";
					case 2u:
						goto IL_0041;
					case 5u:
						switch (currentRenderPipeline)
						{
						case RenderPipeline.Universal:
							break;
						case RenderPipeline.HighDefinition:
							goto IL_0041;
						default:
							goto IL_0060;
						case RenderPipeline.Builtin:
							goto IL_007e;
						}
						goto case 0u;
					case 1u:
						num = (int)((num2 * 2078256802) ^ 0x7DBD165A);
						continue;
					default:
						goto IL_007e;
						IL_007e:
						return "_MetallicGlossMap";
						IL_0060:
						num = ((int)num2 * -1576701717) ^ 0x5444961E;
						continue;
						IL_0041:
						return "_MaskMap";
					}
					break;
				}
			}
		}

		public static void Debug(LogType logType, string message, Exception exception, UnityEngine.Object context = null)
		{
			message = _200B_206E_202D_206A_206B_206A_200C_202E_200B_200B_202E_206B_206F_206F_206F_202B_202A_200D_206D_206C_202C_206D_200F_202C_206E_206A_206C_206D_206B_200D_206A_206D_206D_206D_206D_202D_200D_200D_202C_200D_202E("[Terrain To Mesh] ", message);
			StackTraceLogType stackTraceLogType = default(StackTraceLogType);
			while (true)
			{
				int num = 1518511187;
				while (true)
				{
					uint num2;
					int num3;
					int num4;
					int num5;
					int num6;
					int num7;
					switch ((num2 = (uint)num ^ 0x9981D40u) % 23)
					{
					case 14u:
						break;
					case 13u:
						stackTraceLogType = _200B_206A_206D_200D_206A_202B_202A_200E_200E_202E_202E_200C_202B_202A_206D_206C_200D_202C_200C_200D_200D_206F_200B_202A_200D_202B_206A_200F_202C_206B_200F_202D_202B_206B_202C_200C_202D_202A_206B_200C_202E(logType);
						_202C_202E_206B_206C_206E_200E_200E_200D_202A_200F_200E_206E_206B_202D_202C_200D_202A_200E_200F_202A_202D_202E_200F_206D_206D_202C_202D_202B_200C_200D_202E_202E_200F_200D_206C_206A_200B_202E_206F_206F_202E(logType, StackTraceLogType.None);
						switch (logType)
						{
						case LogType.Exception:
							goto IL_010a;
						case LogType.Error:
							goto IL_013f;
						case LogType.Assert:
							goto IL_01f6;
						case LogType.Warning:
							goto IL_0225;
						case LogType.Log:
							goto IL_0253;
						}
						num = (int)(num2 * 1622174237) ^ -2033168495;
						continue;
					case 10u:
						num = (int)(num2 * 1292805734) ^ -676946385;
						continue;
					case 3u:
						_202D_206A_206F_200E_202E_200C_200D_200B_200B_200F_202B_206D_200D_200F_200F_200E_206D_206A_202A_200B_200F_200F_200B_202C_206B_202D_206F_202A_202D_206B_206E_200E_206D_206A_200C_200E_206A_206F_206C_206F_202E((object)message, context);
						num = 650561875;
						continue;
					case 20u:
						_206A_202E_202A_200E_206A_206F_206E_206D_206C_206F_202A_206C_202A_202C_200C_202C_202D_202B_206D_200E_206B_200B_206D_200C_202B_200F_206E_202B_200B_200F_200C_200F_206F_200F_200B_206F_200C_202B_206F_206D_202E(exception);
						num = (int)((num2 * 1410873950) ^ 0x431B02A5);
						continue;
					case 4u:
						num = ((int)num2 * -545147549) ^ 0x17312C17;
						continue;
					case 21u:
						goto IL_010a;
					case 5u:
						_202A_202B_200D_202B_202A_202A_202D_206C_200E_202D_206B_202B_206F_202A_206D_206D_200E_200F_206A_206C_202D_206C_202E_200D_202E_200E_202A_202B_202E_206B_202B_206E_200C_202E_200B_206D_200D_200D_206E_202A_202E((object)message);
						num = ((int)num2 * -1142701946) ^ 0x5C907B2C;
						continue;
					case 18u:
						goto IL_013f;
					case 8u:
						num = (int)(num2 * 1452181361) ^ -1659912473;
						continue;
					case 19u:
						num = ((int)num2 * -731001038) ^ -1608524297;
						continue;
					case 0u:
						_202A_206A_200F_200F_206F_202B_206A_200F_200C_202D_206A_202C_206F_206C_200C_202C_200F_206B_202D_202E_206F_206C_202D_200C_206C_206B_200C_202B_206D_202A_206A_200F_200C_200B_206C_206B_200C_206A_200E_206A_202E((object)message);
						num = (int)((num2 * 1012252660) ^ 0x11D9FF5);
						continue;
					case 15u:
						_200C_206E_206A_206E_202B_202E_200E_206D_206F_200B_200D_206F_206C_200C_200C_206A_206B_206E_200B_206B_200F_202B_202E_200C_206D_200D_202E_206E_206C_206F_200C_202D_206B_202E_200D_202A_200C_200E_200C_206A_202E((object)message, context);
						num = 679704087;
						continue;
					case 6u:
						_202E_200F_206A_200C_202A_206A_206A_200E_206B_202D_206B_202A_202D_202B_200D_200E_200E_206C_200B_200B_200D_200F_200F_202C_200E_202E_202A_202C_202D_202C_202A_206D_202A_206C_202B_202C_200F_206C_206D_206C_202E((object)message);
						num = (int)((num2 * 623442215) ^ 0x135D7FD0);
						continue;
					case 12u:
						num = ((int)num2 * -66565408) ^ -1148953347;
						continue;
					case 2u:
						_206B_200F_206C_206A_200F_206A_206B_206F_200D_206E_206D_206D_202D_200D_206D_206C_200B_202D_206C_202C_202B_202A_206A_200C_202E_206C_200B_202D_206E_206A_200E_200C_206B_202E_200E_200E_202E_202C_202D_202D_202E((object)message, context);
						num = 2062569405;
						continue;
					case 16u:
						num = (int)(num2 * 755856863) ^ -233495638;
						continue;
					case 22u:
						goto IL_01f6;
					case 1u:
						num = (int)((num2 * 1536587949) ^ 0x71221E76);
						continue;
					case 17u:
						goto IL_0225;
					case 7u:
						_206D_202D_206F_200C_200B_200D_202C_206C_206C_202D_202C_200C_200C_202B_206E_200E_206C_200B_200D_206D_200F_202D_200E_200B_206B_202E_202E_206A_202C_200E_206E_202D_206D_200F_200F_202E_206A_206F_200B_200F_202E(exception, context);
						num = 821766030;
						continue;
					case 11u:
						goto IL_0253;
					default:
						{
							_202C_202E_206B_206C_206E_200E_200E_200D_202A_200F_200E_206E_206B_202D_202C_200D_202A_200E_200F_202A_202D_202E_200F_206D_206D_202C_202D_202B_200C_200D_202E_202E_200F_200D_206C_206A_200B_202E_206F_206F_202E(logType, stackTraceLogType);
							return;
						}
						IL_0253:
						if (!_206B_202B_202E_206A_202E_206E_206F_202C_200B_202C_202C_202A_200F_202E_200C_206E_202B_200C_200F_206B_202B_200C_200D_202C_206F_200D_200D_206C_202A_206C_206E_202A_200F_200F_200B_206F_202A_202B_206A_202B_202E(context, (UnityEngine.Object)null))
						{
							num = 1285317581;
							num3 = num;
						}
						else
						{
							num = 635790091;
							num3 = num;
						}
						continue;
						IL_01f6:
						if (_206B_202B_202E_206A_202E_206E_206F_202C_200B_202C_202C_202A_200F_202E_200C_206E_202B_200C_200F_206B_202B_200C_200D_202C_206F_200D_200D_206C_202A_206C_206E_202A_200F_200F_200B_206F_202A_202B_206A_202B_202E(context, (UnityEngine.Object)null))
						{
							num = 1643134345;
							num4 = num;
						}
						else
						{
							num = 2062569405;
							num4 = num;
						}
						continue;
						IL_010a:
						if (_206B_202B_202E_206A_202E_206E_206F_202C_200B_202C_202C_202A_200F_202E_200C_206E_202B_200C_200F_206B_202B_200C_200D_202C_206F_200D_200D_206C_202A_206C_206E_202A_200F_200F_200B_206F_202A_202B_206A_202B_202E(context, (UnityEngine.Object)null))
						{
							num = 647989876;
							num5 = num;
						}
						else
						{
							num = 1499734214;
							num5 = num;
						}
						continue;
						IL_0225:
						if (_206B_202B_202E_206A_202E_206E_206F_202C_200B_202C_202C_202A_200F_202E_200C_206E_202B_200C_200F_206B_202B_200C_200D_202C_206F_200D_200D_206C_202A_206C_206E_202A_200F_200F_200B_206F_202A_202B_206A_202B_202E(context, (UnityEngine.Object)null))
						{
							num = 1424027118;
							num6 = num;
						}
						else
						{
							num = 666362038;
							num6 = num;
						}
						continue;
						IL_013f:
						if (!_206B_202B_202E_206A_202E_206E_206F_202C_200B_202C_202C_202A_200F_202E_200C_206E_202B_200C_200F_206B_202B_200C_200D_202C_206F_200D_200D_206C_202A_206C_206E_202A_200F_200F_200B_206F_202A_202B_206A_202B_202E(context, (UnityEngine.Object)null))
						{
							num = 1363721243;
							num7 = num;
						}
						else
						{
							num = 1438956185;
							num7 = num;
						}
						continue;
					}
					break;
				}
			}
		}

		static RenderPipelineAsset _202D_200F_200D_202B_202B_206A_202B_206B_206E_202E_202E_202E_206C_200C_200F_202E_202B_200C_206B_206A_202D_206A_206B_202D_206E_200D_202D_206C_206A_202E_200B_202A_200F_200F_202B_206F_202B_200B_206C_202E_202E()
		{
			return GraphicsSettings.renderPipelineAsset;
		}

		static bool _206B_202B_202E_206A_202E_206E_206F_202C_200B_202C_202C_202A_200F_202E_200C_206E_202B_200C_200F_206B_202B_200C_200D_202C_206F_200D_200D_206C_202A_206C_206E_202A_200F_200F_200B_206F_202A_202B_206A_202B_202E(UnityEngine.Object P_0, UnityEngine.Object P_1)
		{
			return P_0 == P_1;
		}

		static string _200C_200C_202D_202C_206F_200F_202B_200E_206C_202A_202C_200E_200D_200D_200F_206B_202C_200D_200D_206D_206A_202A_206E_200E_202B_202D_206D_206A_206D_206C_206D_202B_206C_202B_202A_200D_202E_200E_202C_200C_202E(UnityEngine.Object P_0)
		{
			return P_0.name;
		}

		static bool _200D_206A_200F_200F_200E_202D_202E_200C_200D_200E_206C_202E_202D_202E_206B_200E_200F_206B_200E_206B_206D_200D_202D_206A_202C_206F_206B_200B_206F_206D_200B_206C_202D_202D_200F_202A_206E_206A_202B_202E_202E(string P_0, string P_1)
		{
			return P_0.Contains(P_1);
		}

		static Shader _202A_202E_206F_206F_202E_200E_202A_206C_200D_202C_206E_206C_206B_202B_206B_202E_200D_200F_206C_202B_202B_206D_202B_200E_200F_200E_202C_200D_200C_200C_206B_200E_206C_202A_206F_206C_202E_202E_202D_206E_202E(string P_0)
		{
			return Shader.Find(P_0);
		}

		static bool _202C_206C_206F_202A_206D_206C_202C_200F_200F_202A_202E_202A_202D_202B_206B_200C_200B_206C_200E_206B_200B_206B_206D_206F_202B_200B_206F_206F_206C_206D_200B_200C_202D_202E_202E_202C_200D_206D_202A_200C_202E(UnityEngine.Object P_0, UnityEngine.Object P_1)
		{
			return P_0 != P_1;
		}

		static Vector3 _206E_200C_200B_200D_202B_202A_202E_202A_206E_206D_202E_202E_206F_202E_202A_206B_200B_200C_202E_206D_200D_206B_206F_202D_202C_202B_202E_202D_202E_200D_200E_200F_202E_200F_202C_200B_200C_206D_202B_200E_202E(TerrainData P_0)
		{
			return P_0.size;
		}

		static float _202C_206A_202D_200E_200D_206F_200C_206A_206D_200C_206F_202D_200D_206C_206C_200D_200B_206B_202E_202C_202C_206B_202D_206C_206E_200F_206D_206C_202A_206F_206E_200F_202D_206D_206E_202B_200C_200D_202E(TerrainData P_0, float P_1, float P_2)
		{
			return P_0.GetInterpolatedHeight(P_1, P_2);
		}

		static Mesh _202D_200E_200C_202E_206C_202C_206F_200E_200F_206B_206A_200B_206A_206B_206F_200F_202D_200E_200B_200D_200D_200C_200F_206A_202C_206C_200B_202A_202E_200B_206E_200F_200B_202A_206A_202D_200B_206E_202E_202E_202E()
		{
			return new Mesh();
		}

		static void _202B_206E_206A_200B_202C_206A_206F_206D_202B_200F_206D_202B_202C_200E_206C_200C_200F_200B_202B_202A_202C_200C_206B_206C_202D_200D_206E_206A_200E_202D_200F_206F_200E_200C_206E_200E_206A_200F_200E_206F_202E(UnityEngine.Object P_0, string P_1)
		{
			P_0.name = P_1;
		}

		static void _200C_206F_200C_202C_200B_206F_206C_202C_200E_200B_206C_202C_202B_200D_206E_202B_206A_202E_206F_202D_206D_206D_200B_206B_200B_206E_206C_200E_202A_200C_206B_206A_206A_206F_206E_206F_200D_206F_206D_202E_202E(Mesh P_0, Vector3[] P_1)
		{
			P_0.vertices = P_1;
		}

		static void _200E_206C_202D_200C_206D_200B_200B_202C_200C_202C_206E_202A_200E_206D_200F_206F_206E_206C_202A_202D_200C_206F_206A_202D_202D_202E_206A_206F_200C_206C_202A_206C_206F_206D_200E_200B_202E_206A_202D_206F_202E(Mesh P_0, int[] P_1)
		{
			P_0.triangles = P_1;
		}

		static void _200F_202A_206D_206C_200B_206F_202C_202E_206E_206A_202E_202E_200B_206C_200F_200C_202C_200E_200C_206E_200B_202D_206F_200E_206E_200B_206C_202C_202D_202E_202D_206E_202C_200F_206E_200E_206A_206B_200B_200C_202E(Mesh P_0, Vector3[] P_1)
		{
			P_0.normals = P_1;
		}

		static void _206E_200F_206E_200F_202E_206E_206E_206E_202B_206F_202E_200F_206E_206F_202A_200C_202E_202A_200F_206A_206A_206B_202A_200E_202D_200B_206E_200C_200F_206D_206D_200E_200B_206E_206E_202C_206D_200C_206E_206A_202E(Mesh P_0, Vector4[] P_1)
		{
			P_0.tangents = P_1;
		}

		static void _200F_202B_202C_202B_206B_202D_202D_206D_206D_200D_202E_200F_200B_200B_202B_206C_202B_200F_202D_206B_200F_202C_206A_202E_206F_206E_206F_200F_206A_206F_206B_206E_206D_206D_202A_200F_200D_202D_206E_200F_202E(Mesh P_0, Vector2[] P_1)
		{
			P_0.uv = P_1;
		}

		static void _206B_200D_202E_200B_206D_200C_200C_202E_206B_206B_200D_206E_202C_202D_202E_200B_200E_200C_206B_202C_206B_200C_200F_202B_206F_202D_202C_206E_202D_200D_200B_200F_202C_206C_202D_200D_206C_206C_206A_200E_202E(Mesh P_0, Color[] P_1)
		{
			P_0.colors = P_1;
		}

		static bool _200B_202B_200C_202E_202C_206B_206A_200C_206C_200F_200D_200E_200C_206C_202B_202C_202A_200B_206E_206F_200C_206C_200C_202C_200E_202B_206F_206F_206C_200F_200C_206C_206D_206B_200B_200E_206A_200E_202D_200C_202E()
		{
			return SystemInfo.supports32bitsIndexBuffer;
		}

		static Mesh _202C_202B_202A_206F_202A_200B_206E_202C_206C_202E_206A_200E_200C_202C_206A_200E_206C_206C_200D_206E_200D_206B_200E_206D_202B_202D_200F_202D_206B_206F_202E_206D_200E_206F_200D_202A_200E_202D_200F_202A_202E(MeshFilter P_0)
		{
			return P_0.sharedMesh;
		}

		static int _202D_206C_202E_202C_206B_200E_200E_206C_200D_200B_206A_202A_206E_202A_200D_206D_200B_206F_200E_206C_202D_206B_202A_200D_200D_200B_202B_202A_206E_200F_206E_202C_202A_202B_200D_206D_206A_202B_206C_206E_202E(Mesh P_0)
		{
			return P_0.vertexCount;
		}

		static string _206C_206C_200B_202C_206A_202C_202E_200D_200B_206F_202B_202A_202C_200E_206D_200E_202B_206E_206A_202B_206E_206E_206E_202E_202D_206E_200E_202B_206A_202D_202D_202A_206E_206D_206B_202B_202B_202C_206D_202B_202E(string P_0, object P_1)
		{
			return string.Format(P_0, P_1);
		}

		static string _200B_206E_202D_206A_206B_206A_200C_202E_200B_200B_202E_206B_206F_206F_206F_202B_202A_200D_206D_206C_202C_206D_200F_202C_206E_206A_206C_206D_206B_200D_206A_206D_206D_206D_206D_202D_200D_200D_202C_200D_202E(string P_0, string P_1)
		{
			return P_0 + P_1;
		}

		static Transform _202E_206A_202E_202C_200D_206F_200F_202C_202D_202A_202A_202E_206C_202A_200F_206D_206B_206C_202A_206A_200B_200F_200F_206C_206A_206E_202B_202D_200C_202D_206D_202E_202A_206F_206C_202B_206A_206D_202E_206D_202E(Component P_0)
		{
			return P_0.transform;
		}

		static Matrix4x4 _206B_200C_200B_200C_200E_206B_206A_200F_200C_200D_202A_206B_200B_200B_200D_200D_206A_200E_206F_206E_206C_202C_200C_200E_202D_200F_202C_206A_202A_200B_206A_200E_206C_202B_206D_202B_202B_206D_206E_200E_202E(Transform P_0)
		{
			return P_0.localToWorldMatrix;
		}

		static bool _206A_206B_202E_206B_206C_206F_200C_200C_206A_200D_206C_206E_202C_200C_200E_202B_206A_202E_202A_206F_206E_200B_200D_206A_206C_202C_200C_206D_200F_200E_200B_202A_202A_200D_206E_200B_206B_200F_200E_206F_202E()
		{
			return Application.isEditor;
		}

		static GameObject _206C_206F_206B_200C_200C_206F_206D_202E_200D_202E_202C_206E_200F_202D_200F_200D_200C_202C_200B_206A_202A_206E_202A_200B_202A_200C_200B_206B_200F_206E_202B_202C_206E_206F_202A_206F_202A_200B_200F_200E_202E(Component P_0)
		{
			return P_0.gameObject;
		}

		static void _206D_202C_206B_202B_206E_200E_200F_200C_206B_202D_206A_206F_202C_202D_202A_200C_206B_206F_206C_200B_200C_202D_202A_206F_206A_202C_202A_206B_200B_200C_206E_202E_206B_206E_206A_200B_206D_206F_202A_206C_202E(UnityEngine.Object P_0)
		{
			UnityEngine.Object.DestroyImmediate(P_0);
		}

		static void _202C_206A_200E_206C_202A_206B_202D_202D_200C_202D_206A_202B_200D_206F_206A_206A_200B_206F_202A_200F_200B_202E_202A_202A_202B_202E_200B_200F_202D_200F_202B_202C_202D_206F_200E_200F_206A_200B_200F_202C_202E(UnityEngine.Object P_0)
		{
			UnityEngine.Object.Destroy(P_0);
		}

		static GameObject _202B_206F_206B_202E_202D_206A_200C_206A_202B_206B_200D_206C_200F_206A_206F_200C_206E_202D_206A_202D_206D_200B_202A_202B_200F_202E_200B_206D_200F_200B_206D_202A_200E_200B_206F_206A_206A_200D_202B_206C_202E(string P_0)
		{
			return new GameObject(P_0);
		}

		static Transform _202B_202D_206C_200F_206D_206F_200C_206C_206F_200F_200F_202C_202D_206E_200C_200E_206D_202E_202A_206F_200E_200B_206E_200F_202C_200B_206B_206F_202B_200B_206B_206F_202E_206E_200E_200E_206E_206F_206D_200C_202E(GameObject P_0)
		{
			return P_0.transform;
		}

		static void _200D_206D_200F_206A_200E_202A_202E_202E_202D_200D_200E_200B_200E_202A_202E_206D_200E_206C_206F_206D_200E_206F_200F_200F_206E_200D_200D_202E_206B_206F_200E_200F_202E_202B_206A_200B_202C_200F_202A_202E_202E(Transform P_0, Transform P_1)
		{
			P_0.parent = P_1;
		}

		static void _206A_206C_206D_200F_200F_206E_202B_200D_200D_200C_206F_206D_200F_200B_202B_202E_206D_202B_206F_202B_202E_200B_206D_202A_206E_202D_200C_206F_206B_202B_200F_200F_200E_206B_202B_202C_200F_206B_206D_202A_202E(Renderer P_0, Material P_1)
		{
			P_0.sharedMaterial = P_1;
		}

		static void _202E_206F_206A_200C_200B_200F_200E_202B_200F_202C_202B_200F_206B_206C_202B_202A_200E_200B_202A_206D_200B_202C_200F_202D_202C_206E_202E_206D_206F_200B_206E_200B_200B_200D_200D_206D_202A_200B_202D_206B_202E(MeshFilter P_0, Mesh P_1)
		{
			P_0.sharedMesh = P_1;
		}

		static void _206C_206C_202A_206F_206D_200E_202A_202B_200C_200C_202E_206E_200F_200F_200B_202A_202E_202D_206F_200B_200B_206A_202C_206D_206B_202B_206F_206A_206B_206B_206F_200E_206B_200B_206D_202B_206E_200E_200D_200C_202E(Mesh P_0, IndexFormat P_1)
		{
			P_0.indexFormat = P_1;
		}

		static void _200E_200C_206C_202D_206B_200E_202C_202A_200E_206F_202E_206E_202C_202B_206D_202B_206A_206C_200E_200D_200F_202A_200D_202E_206D_202A_202A_200B_206B_206E_206C_200E_206A_206A_206F_202A_206F_200E_202C_200F_202E(Mesh P_0, CombineInstance[] P_1, bool P_2, bool P_3)
		{
			P_0.CombineMeshes(P_1, P_2, P_3);
		}

		static Texture _202A_206B_206A_200E_206E_206B_200B_202D_200F_202D_200B_206F_206E_206A_206B_202B_200F_206D_202B_202C_202E_206D_206A_200B_200D_200C_200C_202B_200D_206C_200B_202C_206E_206B_200D_200F_202E_206E_202B_206A_202E(TerrainData P_0)
		{
			return P_0.holesTexture;
		}

		static int _206A_206E_200C_200C_202D_200C_206F_206D_206B_206B_206B_200E_206B_206B_200B_200B_202E_202D_202B_202B_200D_202B_206B_206E_206F_202D_202A_202A_206E_200B_200E_200E_202E_206B_202D_206A_200C_202A_206A_200C_202E(TerrainData P_0)
		{
			return P_0.holesResolution;
		}

		static int _206A_202B_200F_206A_206B_200F_200C_202D_200C_202B_200C_202D_200B_206B_206E_206A_200E_206E_202D_200E_200B_206F_206A_206C_202A_200F_200B_202A_206D_202A_206F_206C_200D_206A_202B_206D_200D_202E_202C_206B_202E(Texture P_0)
		{
			return P_0.width;
		}

		static int _202D_206A_206F_202D_202B_200B_202A_202E_200F_202D_202C_206C_206C_206B_200C_206B_202C_206F_206B_200F_200F_200B_206F_200F_202A_202E_206F_202A_206C_202C_202A_206F_200C_200C_200D_206A_200C_206B_206D_202B_202E(Texture P_0)
		{
			return P_0.height;
		}

		static bool[,] _206E_200B_202C_206D_202C_206B_202A_200C_200C_206D_200F_200D_202A_206F_200B_200F_200B_206D_200D_202E_206E_202C_202E_202A_206F_200E_206B_202C_206C_200E_200D_202A_202D_202B_206C_200E_200E_200C_202A_206E_202E(TerrainData P_0, int P_1, int P_2, int P_3, int P_4)
		{
			return P_0.GetHoles(P_1, P_2, P_3, P_4);
		}

		static int _206F_202A_202D_206E_206C_200F_206F_200C_202B_202C_206E_206B_206C_206A_206A_202A_200F_202A_202D_206A_200F_200F_202E_200F_202E_206F_202D_202B_200F_202A_206E_200F_206D_200C_202D_206D_202E_200B_202D_206D_202E(Array P_0, int P_1)
		{
			return P_0.GetLength(P_1);
		}

		static void _202C_200B_206E_202C_202A_200F_200B_206A_206B_202A_202D_206C_206D_206C_206A_200B_202A_202E_202C_206B_202D_206A_200E_200B_200B_202A_202A_206E_200E_200F_202C_200F_206A_202A_200B_206B_202B_202A_206E_200F_202E(Material P_0, string P_1)
		{
			P_0.EnableKeyword(P_1);
		}

		static void _200B_202B_200C_200B_202B_200B_200E_202B_200C_206E_200F_202C_202B_200F_206B_206E_202C_206D_202B_202E_202D_206F_200B_202D_202E_202E_206D_206E_206D_200B_202D_202C_206F_206A_206C_206E_200D_206F_202C_200D_202E(Material P_0, int P_1)
		{
			P_0.renderQueue = P_1;
		}

		static void _202C_202D_206B_206D_200F_200E_200F_202A_200F_202B_202A_206A_202D_202B_200C_206B_206C_200D_200B_202A_202D_206E_200F_202D_200E_202D_202D_206E_200F_202C_206B_202A_206E_206E_200F_202C_200F_206D_202A_206F_202E(Material P_0, string P_1, float P_2)
		{
			P_0.SetFloat(P_1, P_2);
		}

		static bool _200C_206B_202C_200B_200C_206A_206C_200C_200D_200B_206A_206C_206B_206B_200D_202A_200F_206F_206B_206F_206A_202A_206F_206F_206D_206D_200B_202A_206A_200F_200D_202B_206C_200D_200B_206D_206F_200B_202D_206A_202E(Material P_0, string P_1)
		{
			return P_0.IsKeywordEnabled(P_1);
		}

		static StringBuilder _202C_206D_206F_206C_202C_206D_206B_202C_200E_206A_200E_202E_200B_206B_202C_206C_200E_206B_200D_202C_200C_202A_202C_202E_206B_200C_206E_200C_206A_206C_202E_202E_206D_206B_202D_206B_200B_202E_202C_206D_202E()
		{
			return new StringBuilder();
		}

		static StringBuilder _206B_202E_200B_200E_202D_200E_206C_200E_200B_200E_206A_206F_202C_202C_206E_200B_200F_200D_202B_202B_206C_202E_206F_206C_206B_206D_206A_200D_202A_206D_202B_202C_202C_202A_200E_202D_200E_200F_206F_202D_202E(StringBuilder P_0, string P_1)
		{
			return P_0.Append(P_1);
		}

		static Vector3[] _206C_202C_200F_206D_202A_202E_206B_200C_200E_202D_202D_206A_206D_206B_202D_200C_200C_202C_200F_206E_202B_206B_206A_202C_206F_206A_200C_206E_206B_202C_200F_206B_206A_202C_206B_202D_202C_206E_206C_200D_202E(Mesh P_0)
		{
			return P_0.vertices;
		}

		static int[] _202E_200D_206E_200D_200D_206E_200F_206C_200C_206C_202B_200E_200F_200B_200C_202C_200D_206B_206B_206B_206A_202B_200C_206C_206A_202E_200C_206E_206C_206C_202B_200B_200B_206F_202E_200C_200C_200C_200D_200C_202E(Mesh P_0)
		{
			return P_0.triangles;
		}

		static Vector3[] _200B_206A_202B_202D_202C_200B_200F_200B_202C_206A_200E_202B_202E_202E_200C_202B_200F_200D_202C_200E_200F_200F_202E_200F_200B_202C_202D_202E_202D_200D_206B_206C_200F_200B_202E_206E_202D_202A_206E_206E_202E(Mesh P_0)
		{
			return P_0.normals;
		}

		static Vector2[] _200B_206D_202B_206B_206C_200F_200D_202B_206F_200B_200C_202B_200C_202D_202A_200F_206C_206C_206C_200C_200E_206A_202D_202E_206F_200F_200F_202C_206D_200E_200F_202B_200B_206E_200C_202C_206A_206A_200E_202A_202E(Mesh P_0)
		{
			return P_0.uv;
		}

		static void _200D_206F_202A_202C_202B_202A_202B_206E_206D_206F_202E_202B_206B_200D_200B_206A_200D_206D_202C_206D_202A_202D_200C_202B_202D_202A_206D_202E_200C_206C_206C_206C_200E_202E_202D_200D_200E_202C_202C_200F_202E(Mesh P_0, bool P_1)
		{
			P_0.Clear(P_1);
		}

		static string _206C_206C_206D_206D_202B_206F_200F_200E_202D_200E_202E_206D_206F_202E_200C_202D_200F_202C_202C_202D_200F_206D_206D_206C_206B_200D_206F_202B_200F_200F_202B_206C_206A_200F_202C_206C_206B_200C_206D_206E_202E(object P_0)
		{
			return P_0.ToString();
		}

		static void _206E_202B_202C_200C_200C_202C_206F_202C_200E_200D_202A_202D_200C_202B_202B_200C_202C_202B_206F_202D_202E_206A_206F_200C_200C_206A_206B_206D_206A_206F_206E_202D_200F_200D_206A_206F_200F_200C_206E_206F_202E(TextWriter P_0, string P_1)
		{
			P_0.WriteLine(P_1);
		}

		static string _200B_200B_202C_202A_206E_200C_206E_200F_206B_200C_206F_202B_202B_202C_200E_202E_200D_200C_202D_202E_200D_200C_202D_202B_200C_202A_202E_206C_206A_202B_206B_206B_200C_206C_202E_202A_206C_200E_206B_200E_202E()
		{
			return Environment.NewLine;
		}

		static string _200E_206B_206D_206E_206B_206A_200B_200B_202D_200E_206D_206F_200E_200E_206A_206B_200C_200F_200D_200D_200B_202C_200F_200C_200C_206B_200F_206B_206B_202A_206B_206C_200E_206E_206F_200C_202A_206E_206D_200E_202E(string[] P_0)
		{
			return string.Concat(P_0);
		}

		static void _206B_200E_202B_200D_206F_200D_202E_200C_200F_202C_200B_200D_200B_200B_206E_202E_202C_206B_206D_202B_206A_200C_200F_206F_206B_202B_206A_200B_200F_200E_200E_206B_202C_200B_200C_202B_202B_206C_202D_200D_202E(StringBuilder P_0, int P_1)
		{
			P_0.Length = P_1;
		}

		static string _200C_200D_200E_200D_206C_202E_206A_202E_202A_206A_206C_206A_202E_202B_206A_200C_200D_206F_200E_200B_200F_200F_200F_200E_206C_206D_202B_206C_202A_202A_202C_200D_200D_200D_206A_206B_206C_202B_202C_202A_202E(string P_0, string P_1, string P_2)
		{
			return P_0 + P_1 + P_2;
		}

		static StringBuilder _202C_202E_202D_206B_202B_206D_206C_206C_200D_206A_206F_206D_200E_200D_202E_202C_200F_206B_200E_206D_202B_202B_200D_200F_206F_206B_200E_202C_202A_206A_200C_202E_202A_206C_202B_202E_202B_206C_206A_206A_202E(StringBuilder P_0, int P_1)
		{
			return P_0.Append(P_1);
		}

		static CultureInfo _206B_206B_200B_206D_202A_206E_202D_200B_206E_206C_200C_202C_206A_200F_200D_206C_202A_202A_200E_202C_206C_200E_206E_200D_200B_206D_206B_206D_206A_200D_202C_206E_202C_206D_200E_206A_206B_200C_202E_202A_202E()
		{
			return CultureInfo.InvariantCulture;
		}

		static StackTraceLogType _200B_206A_206D_200D_206A_202B_202A_200E_200E_202E_202E_200C_202B_202A_206D_206C_200D_202C_200C_200D_200D_206F_200B_202A_200D_202B_206A_200F_202C_206B_200F_202D_202B_206B_202C_200C_202D_202A_206B_200C_202E(LogType P_0)
		{
			return Application.GetStackTraceLogType(P_0);
		}

		static void _202C_202E_206B_206C_206E_200E_200E_200D_202A_200F_200E_206E_206B_202D_202C_200D_202A_200E_200F_202A_202D_202E_200F_206D_206D_202C_202D_202B_200C_200D_202E_202E_200F_200D_206C_206A_200B_202E_206F_206F_202E(LogType P_0, StackTraceLogType P_1)
		{
			Application.SetStackTraceLogType(P_0, P_1);
		}

		static void _202A_202B_200D_202B_202A_202A_202D_206C_200E_202D_206B_202B_206F_202A_206D_206D_200E_200F_206A_206C_202D_206C_202E_200D_202E_200E_202A_202B_202E_206B_202B_206E_200C_202E_200B_206D_200D_200D_206E_202A_202E(object P_0)
		{
			UnityEngine.Debug.LogError(P_0);
		}

		static void _200C_206E_206A_206E_202B_202E_200E_206D_206F_200B_200D_206F_206C_200C_200C_206A_206B_206E_200B_206B_200F_202B_202E_200C_206D_200D_202E_206E_206C_206F_200C_202D_206B_202E_200D_202A_200C_200E_200C_206A_202E(object P_0, UnityEngine.Object P_1)
		{
			UnityEngine.Debug.LogError(P_0, P_1);
		}

		static void _206A_202E_202A_200E_206A_206F_206E_206D_206C_206F_202A_206C_202A_202C_200C_202C_202D_202B_206D_200E_206B_200B_206D_200C_202B_200F_206E_202B_200B_200F_200C_200F_206F_200F_200B_206F_200C_202B_206F_206D_202E(Exception P_0)
		{
			UnityEngine.Debug.LogException(P_0);
		}

		static void _206D_202D_206F_200C_200B_200D_202C_206C_206C_202D_202C_200C_200C_202B_206E_200E_206C_200B_200D_206D_200F_202D_200E_200B_206B_202E_202E_206A_202C_200E_206E_202D_206D_200F_200F_202E_206A_206F_200B_200F_202E(Exception P_0, UnityEngine.Object P_1)
		{
			UnityEngine.Debug.LogException(P_0, P_1);
		}

		static void _202E_200F_206A_200C_202A_206A_206A_200E_206B_202D_206B_202A_202D_202B_200D_200E_200E_206C_200B_200B_200D_200F_200F_202C_200E_202E_202A_202C_202D_202C_202A_206D_202A_206C_202B_202C_200F_206C_206D_206C_202E(object P_0)
		{
			UnityEngine.Debug.Log(P_0);
		}

		static void _202D_206A_206F_200E_202E_200C_200D_200B_200B_200F_202B_206D_200D_200F_200F_200E_206D_206A_202A_200B_200F_200F_200B_202C_206B_202D_206F_202A_202D_206B_206E_200E_206D_206A_200C_200E_206A_206F_206C_206F_202E(object P_0, UnityEngine.Object P_1)
		{
			UnityEngine.Debug.Log(P_0, P_1);
		}

		static void _202A_206A_200F_200F_206F_202B_206A_200F_200C_202D_206A_202C_206F_206C_200C_202C_200F_206B_202D_202E_206F_206C_202D_200C_206C_206B_200C_202B_206D_202A_206A_200F_200C_200B_206C_206B_200C_206A_200E_206A_202E(object P_0)
		{
			UnityEngine.Debug.LogWarning(P_0);
		}

		static void _206B_200F_206C_206A_200F_206A_206B_206F_200D_206E_206D_206D_202D_200D_206D_206C_200B_202D_206C_202C_202B_202A_206A_200C_202E_206C_200B_202D_206E_206A_200E_200C_206B_202E_200E_200E_202E_202C_202D_202D_202E(object P_0, UnityEngine.Object P_1)
		{
			UnityEngine.Debug.LogWarning(P_0, P_1);
		}
	}
}
