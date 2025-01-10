using UnityEngine;

namespace AmazingAssets.TerrainToMesh
{
	public class TerrainToMeshConversionDetails : MonoBehaviour
	{
		[HideInInspector]
		public string prefabProjectPath;

		public int vertexCountHorizontal;

		public int vertexCountVertical;

		public int chunkCountHorizontal;

		public int chunkCountVertical;

		[Space]
		public bool hasEdgeFall;

		public bool hasPerChunkUV;

		[Space]
		public bool meshIsCompressed;

		public Mesh[] meshes;

		private void Start()
		{
			if (meshes == null || !meshIsCompressed || meshes.Length == 1 || meshes.Length != chunkCountHorizontal * chunkCountVertical)
			{
				return;
			}
			for (int i = 0; i < meshes.Length; i++)
			{
				if (meshes[i] == null)
				{
					return;
				}
			}
			if (meshes[0].vertexCount == Utilities.GetGeneratedVertexCount(vertexCountHorizontal, vertexCountVertical, hasEdgeFall))
			{
				_202D_206E_200E_206D_202D_200D_206A_200E_206B_200F_200D_200C_200C_202E_202B_200F_206E_200E_206F_202A_200C_206A_202C_200E_202E_206F_202E_202D_206A_206F_206D_200E_206E_206C_200F_200F_200B_206E_200C_206C_202E();
			}
		}

		private void _202D_206E_200E_206D_202D_200D_206A_200E_206B_200F_200D_200C_200C_202E_202B_200F_206E_200E_206F_202A_200C_206A_202C_200E_202E_206F_202E_202D_206A_206F_206D_200E_206E_206C_200F_200F_200B_206E_200C_206C_202E()
		{
			int num = 0;
			int num4 = default(int);
			int num5 = default(int);
			int num6 = default(int);
			int num7 = default(int);
			while (true)
			{
				int num2 = 1044930282;
				while (true)
				{
					uint num3;
					switch ((num3 = (uint)num2 ^ 0x15C4636Bu) % 22)
					{
					case 20u:
						break;
					default:
						return;
					case 16u:
						num += chunkCountVertical;
						num2 = ((int)num3 * -2053570086) ^ 0x558CB78C;
						continue;
					case 6u:
						num4++;
						num2 = (int)(num3 * 1029061337) ^ -130958709;
						continue;
					case 4u:
					{
						int num13;
						if (num4 >= chunkCountVertical - 1)
						{
							num2 = 1953210467;
							num13 = num2;
						}
						else
						{
							num2 = 1157371182;
							num13 = num2;
						}
						continue;
					}
					case 21u:
						num = 0;
						num5 = 0;
						num2 = (int)((num3 * 1697432837) ^ 0x31B9CB11);
						continue;
					case 17u:
					{
						int num9;
						int num10;
						if (!hasEdgeFall)
						{
							num9 = -98221728;
							num10 = num9;
						}
						else
						{
							num9 = -107733953;
							num10 = num9;
						}
						num2 = num9 ^ (int)(num3 * 2033644497);
						continue;
					}
					case 11u:
						num2 = (int)((num3 * 1891033106) ^ 0x355E384B);
						continue;
					case 0u:
					{
						int num12;
						if (num5 < chunkCountHorizontal - 1)
						{
							num2 = 351922535;
							num12 = num2;
						}
						else
						{
							num2 = 1607912674;
							num12 = num2;
						}
						continue;
					}
					case 18u:
					{
						int num11;
						if (num6 >= chunkCountHorizontal)
						{
							num2 = 1994180438;
							num11 = num2;
						}
						else
						{
							num2 = 1700418324;
							num11 = num2;
						}
						continue;
					}
					case 14u:
						num6++;
						num2 = (int)((num3 * 1911872490) ^ 0x74472E19);
						continue;
					case 9u:
						num4 = 0;
						num2 = 1345462482;
						continue;
					case 13u:
					{
						int num8;
						if (num7 < chunkCountVertical)
						{
							num2 = 144194877;
							num8 = num2;
						}
						else
						{
							num2 = 1360528841;
							num8 = num2;
						}
						continue;
					}
					case 12u:
						num7 = 0;
						num2 = 1702336612;
						continue;
					case 15u:
						num2 = (int)(num3 * 440478410) ^ -440657693;
						continue;
					case 10u:
						_200E_206F_206C_202D_200C_206E_206A_206D_206C_200F_200D_200B_200C_202A_200F_202E_200B_206B_200D_206A_200D_202B_202B_202C_202E_200B_206C_206D_200D_206D_202C_206E_200B_206D_206A_200D_206E_202C_202E_206E_202E(meshes[num + num7], meshes[num + num7 + chunkCountVertical], false);
						num7++;
						num2 = 1702336612;
						continue;
					case 5u:
						num6 = 0;
						num2 = ((int)num3 * -1524230740) ^ 0x3E0AED1A;
						continue;
					case 7u:
						_200C_200D_200B_200C_200B_206D_200F_206E_206B_206A_202E_206F_200F_200C_202C_200E_202E_200D_202D_206E_200F_202C_200E_202C_200F_206D_200C_206B_206D_206F_202D_202C_202A_202D_206D_200D_206D_202A_200E_202E_202E();
						num2 = ((int)num3 * -290260971) ^ -850339544;
						continue;
					case 3u:
						num5++;
						num2 = ((int)num3 * -1372513741) ^ -1579588668;
						continue;
					case 19u:
						num2 = (int)(num3 * 83189562) ^ -1543989357;
						continue;
					case 2u:
						num += chunkCountVertical;
						num2 = (int)(num3 * 1668637143) ^ -524920497;
						continue;
					case 1u:
						_200E_206F_206C_202D_200C_206E_206A_206D_206C_200F_200D_200B_200C_202A_200F_202E_200B_206B_200D_206A_200D_202B_202B_202C_202E_200B_206C_206D_200D_206D_202C_206E_200B_206D_206A_200D_206E_202C_202E_206E_202E(meshes[num + num4], meshes[num + num4 + 1], true);
						num2 = 1372425001;
						continue;
					case 8u:
						return;
					}
					break;
				}
			}
		}

		private void _200E_206F_206C_202D_200C_206E_206A_206D_206C_200F_200D_200B_200C_202A_200F_202E_200B_206B_200D_206A_200D_202B_202B_202C_202E_200B_206C_206D_200D_206D_202C_206E_200B_206D_206A_200D_206E_202C_202E_206E_202E(Mesh P_0, Mesh P_1, bool P_2)
		{
			Vector3[] array = _200B_200D_202C_202C_202B_200F_202E_206F_200C_206A_206C_200F_206A_202D_200D_206F_206A_202C_200E_206F_202A_202B_202B_206F_202C_206E_206B_206A_202B_200B_202C_206F_202A_200E_206A_206E_200D_202E_200C_206E_202E(P_0);
			Vector2[] array2 = _202D_200C_202B_206D_200C_206A_202E_200C_200B_202E_200E_200F_206A_202A_200E_206D_206A_206C_202B_200B_206B_206E_200E_202E_200E_202C_200C_206D_206E_206C_200C_206A_206C_200B_206C_202D_206F_206D_206D_202E(P_0);
			int num7 = default(int);
			Vector3[] array8 = default(Vector3[]);
			int num8 = default(int);
			Vector2[] array6 = default(Vector2[]);
			int num5 = default(int);
			Vector4[] array4 = default(Vector4[]);
			bool flag3 = default(bool);
			bool flag2 = default(bool);
			Vector2[] array3 = default(Vector2[]);
			Vector3[] array9 = default(Vector3[]);
			int num6 = default(int);
			Vector3[] array12 = default(Vector3[]);
			int num10 = default(int);
			int[] array10 = default(int[]);
			int num12 = default(int);
			int[] array5 = default(int[]);
			Vector4[] array7 = default(Vector4[]);
			bool flag = default(bool);
			int[] array13 = default(int[]);
			int[] array11 = default(int[]);
			while (true)
			{
				int num = 107657470;
				while (true)
				{
					uint num2;
					int num9;
					int num15;
					int num20;
					int num11;
					int num16;
					switch ((num2 = (uint)num ^ 0x2AF2BBBEu) % 47)
					{
					case 8u:
						break;
					default:
						return;
					case 17u:
						array[num7] = array8[num8];
						num = (int)(num2 * 772386942) ^ -1941604650;
						continue;
					case 29u:
						array8 = _200B_200D_202C_202C_202B_200F_202E_206F_200C_206A_206C_200F_206A_202D_200D_206F_206A_202C_200E_206F_202A_202B_202B_206F_202C_206E_206B_206A_202B_200B_202C_206F_202A_200E_206A_206E_200D_202E_200C_206E_202E(P_1);
						array6 = _202D_200C_202B_206D_200C_206A_202E_200C_200B_202E_200E_200F_206A_202A_200E_206D_206A_206C_202B_200B_206B_206E_200E_202E_200E_202C_200C_206D_206E_206C_200C_206A_206C_200B_206C_202D_206F_206D_206D_202E(P_1);
						num = ((int)num2 * -1216139475) ^ 0x6DBE361C;
						continue;
					case 43u:
						num5++;
						num = 1155948315;
						continue;
					case 9u:
						num = ((int)num2 * -1822623902) ^ 0x278CFE79;
						continue;
					case 13u:
						if (array4 != null)
						{
							num = ((int)num2 * -1094580484) ^ -259355118;
							continue;
						}
						num9 = 0;
						goto IL_0536;
					case 42u:
					{
						int num22;
						if (flag3)
						{
							num = 426265992;
							num22 = num;
						}
						else
						{
							num = 1122350224;
							num22 = num;
						}
						continue;
					}
					case 45u:
					{
						int num26;
						int num27;
						if (hasPerChunkUV)
						{
							num26 = -772114189;
							num27 = num26;
						}
						else
						{
							num26 = -652493934;
							num27 = num26;
						}
						num = num26 ^ (int)(num2 * 233496403);
						continue;
					}
					case 33u:
					{
						int num23;
						if (flag2)
						{
							num = 763367025;
							num23 = num;
						}
						else
						{
							num = 1145550;
							num23 = num;
						}
						continue;
					}
					case 16u:
					{
						int num19;
						if (flag3)
						{
							num = 545462511;
							num19 = num;
						}
						else
						{
							num = 2133592292;
							num19 = num;
						}
						continue;
					}
					case 23u:
						if (array3 != null)
						{
							num = ((int)num2 * -517996411) ^ 0x1281EAD3;
							continue;
						}
						num15 = 0;
						goto IL_0408;
					case 37u:
						array3 = _200C_200F_200D_202D_206B_200E_200E_202D_206B_202D_202C_202B_200E_206D_202B_206A_200E_202D_206C_202D_206B_202B_202D_200C_200C_200F_202E_200B_200C_202A_202E_202C_202A_202A_200B_202E_202E_202C_202C_206D_202E(P_0);
						array9 = _206F_206B_206D_200B_206D_206D_206F_200B_200E_206B_206D_200C_206B_206F_200C_206C_202A_200D_200C_200B_202D_206E_202E_206F_206E_206C_206F_200C_202B_200F_200E_200D_206C_206B_206E_202D_202E_202D_202B_200D_202E(P_0);
						array4 = _202D_200E_206F_202D_200E_206B_202B_200F_206B_206C_206D_200E_202D_206E_202E_200F_200D_200F_200D_202D_200B_206A_206D_206B_206A_202D_200C_200C_202D_202A_206C_202C_206D_202A_200D_202E_206C_200E_202B_200E_202E(P_0);
						num = ((int)num2 * -591588408) ^ -393023024;
						continue;
					case 44u:
						array9[num6] = array12[num10];
						num = (int)(num2 * 1320570537) ^ -747860583;
						continue;
					case 39u:
						num5 = 0;
						num = 262725311;
						continue;
					case 21u:
						num8 = array10[array10.Length - 1 - num12];
						num = (int)(num2 * 292369752) ^ -1213049948;
						continue;
					case 36u:
						num6 = array5[num5];
						num = 69733507;
						continue;
					case 22u:
						_206E_202C_200F_200F_200D_200D_206C_206E_202A_202D_202B_200C_202D_202B_200B_206F_206A_202C_202E_202B_206F_206C_200C_206A_202B_200D_202D_200B_200D_206E_202A_200E_202B_202B_200E_206E_200C_202A_202B_200F_202E(P_0, array4);
						num = (int)((num2 * 1607357705) ^ 0x21428C47);
						continue;
					case 41u:
						array2[num7] = array6[num8];
						num = (int)(num2 * 1240894204) ^ -1708490175;
						continue;
					case 6u:
						_200B_200D_206C_200B_206D_200B_200E_200F_200D_200D_206B_202C_206A_200B_202C_206E_200D_202B_200F_206A_202B_206A_202A_206E_206D_202D_206A_206A_206E_206D_206C_202C_202E_202B_206A_202A_200B_206D_206A_202C_202E(P_0, array9);
						num = (int)((num2 * 1053865688) ^ 0x314664E7);
						continue;
					case 26u:
						array12 = _206F_206B_206D_200B_206D_206D_206F_200B_200E_206B_206D_200C_206B_206F_200C_206C_202A_200D_200C_200B_202D_206E_202E_206F_206E_206C_206F_200C_202B_200F_200E_200D_206C_206B_206E_202D_202E_202D_202B_200D_202E(P_1);
						array7 = _202D_200E_206F_202D_200E_206B_202B_200F_206B_206C_206D_200E_202D_206E_202E_200F_200D_200F_200D_202D_200B_206A_206D_206B_206A_202D_200C_200C_202D_202A_206C_202C_206D_202A_200D_202E_206C_200E_202B_200E_202E(P_1);
						num = (int)(num2 * 2009626271) ^ -2066202225;
						continue;
					case 2u:
						_200C_200F_200D_202D_206B_200E_200E_202D_206B_202D_202C_202B_200E_206D_202B_206A_200E_202D_206C_202D_206B_202B_202D_200C_200C_200F_202E_200B_200C_202A_202E_202C_202A_202A_200B_202E_202E_202C_202C_206D_202E(P_1);
						num = (int)(num2 * 203826388) ^ -1515065807;
						continue;
					case 7u:
					{
						int num24;
						int num25;
						if (flag)
						{
							num24 = -1077966659;
							num25 = num24;
						}
						else
						{
							num24 = -1118632599;
							num25 = num24;
						}
						num = num24 ^ ((int)num2 * -605193553);
						continue;
					}
					case 38u:
						if (array2 != null)
						{
							num = ((int)num2 * -1284000423) ^ -153116013;
							continue;
						}
						num20 = 0;
						goto IL_03a7;
					case 1u:
						array[num6] = array8[num10];
						num = ((int)num2 * -1189843686) ^ 0x7F6F691F;
						continue;
					case 34u:
					{
						int num21;
						if (num5 >= array5.Length)
						{
							num = 433270313;
							num21 = num;
						}
						else
						{
							num = 1431835837;
							num21 = num;
						}
						continue;
					}
					case 18u:
						num = (int)(num2 * 1484018197) ^ -531325965;
						continue;
					case 3u:
						num20 = ((array2.Length == array8.Length) ? 1 : 0);
						goto IL_03a7;
					case 11u:
						array9[num7] = array12[num8];
						num = ((int)num2 * -1848574133) ^ 0x68F50E6B;
						continue;
					case 30u:
					{
						int num17;
						int num18;
						if (!hasPerChunkUV)
						{
							num17 = -649200752;
							num18 = num17;
						}
						else
						{
							num17 = -548621108;
							num18 = num17;
						}
						num = num17 ^ ((int)num2 * -1575109305);
						continue;
					}
					case 19u:
						num15 = ((array3.Length == array8.Length) ? 1 : 0);
						goto IL_0408;
					case 27u:
						num7 = array13[num12];
						num = 1552483649;
						continue;
					case 4u:
					{
						int num14;
						if (num12 >= array13.Length)
						{
							num = 325426508;
							num14 = num;
						}
						else
						{
							num = 745378881;
							num14 = num;
						}
						continue;
					}
					case 25u:
						num12++;
						num = 836298040;
						continue;
					case 5u:
					{
						int num13;
						if (flag2)
						{
							num = 1140354307;
							num13 = num;
						}
						else
						{
							num = 916444508;
							num13 = num;
						}
						continue;
					}
					case 35u:
						num12 = 0;
						num = (int)(num2 * 586881029) ^ -362393121;
						continue;
					case 46u:
						num = ((int)num2 * -1470089327) ^ 0x39ABF64B;
						continue;
					case 24u:
						num10 = array11[array11.Length - 1 - num5];
						num = (int)((num2 * 555639936) ^ 0x2AE96DCB);
						continue;
					case 15u:
						num11 = ((array9.Length == array8.Length) ? 1 : 0);
						goto IL_04be;
					case 14u:
						array2[num6] = array6[num10];
						num = ((int)num2 * -1935615375) ^ -1298608305;
						continue;
					case 40u:
						if (array9 == null)
						{
							num11 = 0;
							goto IL_04be;
						}
						num = ((int)num2 * -1723590575) ^ -1772419356;
						continue;
					case 10u:
						array4[num6] = array7[num10];
						num = ((int)num2 * -1589948730) ^ -1749723820;
						continue;
					case 0u:
						num9 = ((array4.Length == array8.Length) ? 1 : 0);
						goto IL_0536;
					case 12u:
						array4[num7] = array7[num8];
						num = (int)((num2 * 457783895) ^ 0x179BD263);
						continue;
					case 31u:
					{
						int num3;
						int num4;
						if (flag)
						{
							num3 = 145673799;
							num4 = num3;
						}
						else
						{
							num3 = 1945089432;
							num4 = num3;
						}
						num = num3 ^ ((int)num2 * -307841544);
						continue;
					}
					case 20u:
						_202A_200B_206A_202B_206A_206D_202B_202D_206F_202D_202E_206C_206D_202A_200B_200E_200D_206F_200E_206B_200D_206C_206E_206C_202E_206C_202A_202D_206C_200F_206B_206E_200C_206D_202A_200C_202A_206E_200B_202C_202E(P_0, array2);
						_202E_206F_206E_202E_200C_206E_202D_202E_206B_202E_206E_202D_202E_206C_200F_206F_202D_206E_206C_202E_206F_200C_200E_202E_200C_200E_202C_206D_200D_200E_200E_200D_200D_202E_206B_206A_206C_206D_206F_206B_202E(P_0, array3);
						num = (int)((num2 * 859354568) ^ 0x7976348A);
						continue;
					case 28u:
						_206A_206A_206A_200E_200E_202D_200B_200B_202D_202E_206F_206E_206E_200F_200B_200E_206F_206B_206C_200E_202E_202A_202B_200C_206E_202E_200E_202A_200E_200F_206F_206A_200F_202E_206A_202B_206A_206D_206F_206F_202E(P_0, array);
						num = 201034541;
						continue;
					case 32u:
						return;
						IL_04be:
						flag2 = (byte)num11 != 0;
						num = 1772182060;
						continue;
						IL_0536:
						flag3 = (byte)num9 != 0;
						TerrainToMeshDataExtractor._202D_206D_200E_202C_200F_206C_200E_202B_202B_206A_206B_202C_200D_200B_206E_206A_206C_202E_206A_202B_202D_206F_202E_200B_206B_202B_206B_202A_200D_202B_206A_206B_202C_200E_200D_206E_200F_206A_202A_200E_202E(vertexCountHorizontal, vertexCountVertical, out array13, out array5, out array10, out array11);
						if (P_2)
						{
							num = 61091844;
							num16 = num;
						}
						else
						{
							num = 2100257882;
							num16 = num;
						}
						continue;
						IL_0408:
						num = 30554808;
						continue;
						IL_03a7:
						flag = (byte)num20 != 0;
						num = 1660906529;
						continue;
					}
					break;
				}
			}
		}

		private void _200C_200D_200B_200C_200B_206D_200F_206E_206B_206A_202E_206F_200F_200C_202C_200E_202E_200D_202D_206E_200F_202C_200E_202C_200F_206D_200C_206B_206D_206F_202D_202C_202A_202D_206D_200D_206D_202A_200E_202E_202E()
		{
			int num = vertexCountHorizontal * vertexCountVertical;
			int num8 = default(int);
			int[] array4 = default(int[]);
			Vector3[] array = default(Vector3[]);
			int num4 = default(int);
			int[] array5 = default(int[]);
			int num10 = default(int);
			int num9 = default(int);
			int num6 = default(int);
			int num5 = default(int);
			int[] array3 = default(int[]);
			int[] array2 = default(int[]);
			while (true)
			{
				int num2 = -73511557;
				while (true)
				{
					uint num3;
					switch ((num3 = (uint)num2 ^ 0x84A95803u) % 29)
					{
					case 20u:
						break;
					default:
						return;
					case 5u:
						num2 = ((int)num3 * -577478200) ^ 0x58CF1BA6;
						continue;
					case 28u:
					{
						int num14;
						if (num8 < array4.Length)
						{
							num2 = -1560049354;
							num14 = num2;
						}
						else
						{
							num2 = -477238983;
							num14 = num2;
						}
						continue;
					}
					case 1u:
						array[num4] = array[array5[num10]];
						num2 = -148558756;
						continue;
					case 27u:
						num4 += 2;
						num2 = ((int)num3 * -839753072) ^ 0x19F642AE;
						continue;
					case 14u:
						array = _200B_200D_202C_202C_202B_200F_202E_206F_200C_206A_206C_200F_206A_202D_200D_206F_206A_202C_200E_206F_202A_202B_202B_206F_202C_206E_206B_206A_202B_200B_202C_206F_202A_200E_206A_206E_200D_202E_200C_206E_202E(meshes[num9]);
						num4 = num;
						num6 = 0;
						num2 = -708834809;
						continue;
					case 26u:
						num4 += 2;
						num2 = (int)(num3 * 1233898953) ^ -129244736;
						continue;
					case 6u:
						num5 = 0;
						num2 = (int)((num3 * 1018756503) ^ 0x1C444EE3);
						continue;
					case 8u:
						num4 += 2;
						num2 = ((int)num3 * -762042413) ^ 0x2AE0A69D;
						continue;
					case 0u:
						num9++;
						num2 = ((int)num3 * -1426125773) ^ -1004693964;
						continue;
					case 24u:
						num10++;
						num2 = (int)(num3 * 1144686961) ^ -1685939408;
						continue;
					case 12u:
						num9 = 0;
						num2 = (int)(num3 * 1615921934) ^ -1212101419;
						continue;
					case 15u:
						_206B_206A_200D_206D_200D_202D_206C_202D_206A_200D_202D_200F_202C_206B_202A_206D_206D_206B_202C_202A_200C_200C_202B_200B_200D_206A_200D_206A_206A_202A_200C_200D_202B_202A_202A_200E_200D_202A_200B_202B_202E(meshes[0]);
						TerrainToMeshDataExtractor._202D_206D_200E_202C_200F_206C_200E_202B_202B_206A_206B_202C_200D_200B_206E_206A_206C_202E_206A_202B_202D_206F_202E_200B_206B_202B_206B_202A_200D_202B_206A_206B_202C_200E_200D_206E_200F_206A_202A_200E_202E(vertexCountHorizontal, vertexCountVertical, out array3, out array2, out array4, out array5);
						num2 = ((int)num3 * -81832990) ^ -1921954405;
						continue;
					case 25u:
						num5++;
						num2 = (int)(num3 * 2030529902) ^ -785297255;
						continue;
					case 18u:
						array[num4] = array[array3[num6]];
						num2 = -1359774040;
						continue;
					case 19u:
						num4 += 2;
						num2 = (int)((num3 * 1695709453) ^ 0x31D645D7);
						continue;
					case 3u:
					{
						int num13;
						if (num10 < array5.Length)
						{
							num2 = -962356058;
							num13 = num2;
						}
						else
						{
							num2 = -636063731;
							num13 = num2;
						}
						continue;
					}
					case 4u:
						num2 = (int)(num3 * 1708819397) ^ -1669250942;
						continue;
					case 13u:
					{
						int num12;
						if (num9 < meshes.Length)
						{
							num2 = -226486520;
							num12 = num2;
						}
						else
						{
							num2 = -371613946;
							num12 = num2;
						}
						continue;
					}
					case 9u:
					{
						int num11;
						if (num5 < array2.Length)
						{
							num2 = -1335651589;
							num11 = num2;
						}
						else
						{
							num2 = -253620007;
							num11 = num2;
						}
						continue;
					}
					case 16u:
						num10 = 0;
						num2 = ((int)num3 * -1736039469) ^ -181213040;
						continue;
					case 10u:
						num6++;
						num2 = ((int)num3 * -750580125) ^ -1198600825;
						continue;
					case 2u:
						_206A_206A_206A_200E_200E_202D_200B_200B_202D_202E_206F_206E_206E_200F_200B_200E_206F_206B_206C_200E_202E_202A_202B_200C_206E_202E_200E_202A_200E_200F_206F_206A_200F_202E_206A_202B_206A_206D_206F_206F_202E(meshes[num9], array);
						num2 = ((int)num3 * -1333142509) ^ -1811532705;
						continue;
					case 22u:
						num8 = 0;
						num2 = ((int)num3 * -363461829) ^ -1882455521;
						continue;
					case 23u:
						num8++;
						num2 = ((int)num3 * -256345060) ^ -957538023;
						continue;
					case 7u:
						array[num4] = array[array4[num8]];
						num2 = -482267795;
						continue;
					case 11u:
					{
						int num7;
						if (num6 >= array3.Length)
						{
							num2 = -78015401;
							num7 = num2;
						}
						else
						{
							num2 = -1307348620;
							num7 = num2;
						}
						continue;
					}
					case 21u:
						array[num4] = array[array2[num5]];
						num2 = -2003185588;
						continue;
					case 17u:
						return;
					}
					break;
				}
			}
		}

		static Vector3[] _200B_200D_202C_202C_202B_200F_202E_206F_200C_206A_206C_200F_206A_202D_200D_206F_206A_202C_200E_206F_202A_202B_202B_206F_202C_206E_206B_206A_202B_200B_202C_206F_202A_200E_206A_206E_200D_202E_200C_206E_202E(Mesh P_0)
		{
			return P_0.vertices;
		}

		static Vector2[] _202D_200C_202B_206D_200C_206A_202E_200C_200B_202E_200E_200F_206A_202A_200E_206D_206A_206C_202B_200B_206B_206E_200E_202E_200E_202C_200C_206D_206E_206C_200C_206A_206C_200B_206C_202D_206F_206D_206D_202E(Mesh P_0)
		{
			return P_0.uv;
		}

		static Vector2[] _200C_200F_200D_202D_206B_200E_200E_202D_206B_202D_202C_202B_200E_206D_202B_206A_200E_202D_206C_202D_206B_202B_202D_200C_200C_200F_202E_200B_200C_202A_202E_202C_202A_202A_200B_202E_202E_202C_202C_206D_202E(Mesh P_0)
		{
			return P_0.uv2;
		}

		static Vector3[] _206F_206B_206D_200B_206D_206D_206F_200B_200E_206B_206D_200C_206B_206F_200C_206C_202A_200D_200C_200B_202D_206E_202E_206F_206E_206C_206F_200C_202B_200F_200E_200D_206C_206B_206E_202D_202E_202D_202B_200D_202E(Mesh P_0)
		{
			return P_0.normals;
		}

		static Vector4[] _202D_200E_206F_202D_200E_206B_202B_200F_206B_206C_206D_200E_202D_206E_202E_200F_200D_200F_200D_202D_200B_206A_206D_206B_206A_202D_200C_200C_202D_202A_206C_202C_206D_202A_200D_202E_206C_200E_202B_200E_202E(Mesh P_0)
		{
			return P_0.tangents;
		}

		static void _206A_206A_206A_200E_200E_202D_200B_200B_202D_202E_206F_206E_206E_200F_200B_200E_206F_206B_206C_200E_202E_202A_202B_200C_206E_202E_200E_202A_200E_200F_206F_206A_200F_202E_206A_202B_206A_206D_206F_206F_202E(Mesh P_0, Vector3[] P_1)
		{
			P_0.vertices = P_1;
		}

		static void _202A_200B_206A_202B_206A_206D_202B_202D_206F_202D_202E_206C_206D_202A_200B_200E_200D_206F_200E_206B_200D_206C_206E_206C_202E_206C_202A_202D_206C_200F_206B_206E_200C_206D_202A_200C_202A_206E_200B_202C_202E(Mesh P_0, Vector2[] P_1)
		{
			P_0.uv = P_1;
		}

		static void _202E_206F_206E_202E_200C_206E_202D_202E_206B_202E_206E_202D_202E_206C_200F_206F_202D_206E_206C_202E_206F_200C_200E_202E_200C_200E_202C_206D_200D_200E_200E_200D_200D_202E_206B_206A_206C_206D_206F_206B_202E(Mesh P_0, Vector2[] P_1)
		{
			P_0.uv2 = P_1;
		}

		static void _200B_200D_206C_200B_206D_200B_200E_200F_200D_200D_206B_202C_206A_200B_202C_206E_200D_202B_200F_206A_202B_206A_202A_206E_206D_202D_206A_206A_206E_206D_206C_202C_202E_202B_206A_202A_200B_206D_206A_202C_202E(Mesh P_0, Vector3[] P_1)
		{
			P_0.normals = P_1;
		}

		static void _206E_202C_200F_200F_200D_200D_206C_206E_202A_202D_202B_200C_202D_202B_200B_206F_206A_202C_202E_202B_206F_206C_200C_206A_202B_200D_202D_200B_200D_206E_202A_200E_202B_202B_200E_206E_200C_202A_202B_200F_202E(Mesh P_0, Vector4[] P_1)
		{
			P_0.tangents = P_1;
		}

		static int _206B_206A_200D_206D_200D_202D_206C_202D_206A_200D_202D_200F_202C_206B_202A_206D_206D_206B_202C_202A_200C_200C_202B_200B_200D_206A_200D_206A_206A_202A_200C_200D_202B_202A_202A_200E_200D_202A_200B_202B_202E(Mesh P_0)
		{
			return P_0.vertexCount;
		}
	}
}
