using System.Collections.Generic;
using UnityEngine;

namespace AmazingAssets.TerrainToMesh
{
	public class TreePrototypesData
	{
		public GameObject prefab;

		public int prototypeIndex;

		public List<Vector3> position;

		public List<Vector3> surfaceNormal;

		public List<Vector3> scale;

		public TreePrototypesData(GameObject P_0, int P_1, Vector3 P_2, Vector3 P_3, Vector3 P_4)
		{
			while (true)
			{
				int num = 1651615935;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0x1E9DA1E4u) % 7)
					{
					case 0u:
						break;
					default:
						return;
					case 2u:
						prefab = P_0;
						prototypeIndex = P_1;
						num = ((int)num2 * -219310472) ^ -729260577;
						continue;
					case 5u:
						Add(P_2, P_3, P_4);
						num = (int)((num2 * 252976185) ^ 0x3CF57CC4);
						continue;
					case 6u:
						position = new List<Vector3>();
						num = ((int)num2 * -399273516) ^ 0x67D060C3;
						continue;
					case 4u:
						scale = new List<Vector3>();
						num = (int)((num2 * 1536241507) ^ 0x735531C9);
						continue;
					case 3u:
						surfaceNormal = new List<Vector3>();
						num = (int)(num2 * 1077546839) ^ -955277830;
						continue;
					case 1u:
						return;
					}
					break;
				}
			}
		}

		public void Add(Vector3 position, Vector3 surfaceNormal, Vector3 scale)
		{
			this.position.Add(position);
			while (true)
			{
				int num = -789720570;
				while (true)
				{
					uint num2;
					switch ((num2 = (uint)num ^ 0xAB88472Du) % 3)
					{
					case 0u:
						break;
					case 2u:
						goto IL_002e;
					default:
						this.scale.Add(scale);
						return;
					}
					break;
					IL_002e:
					this.surfaceNormal.Add(surfaceNormal);
					num = (int)((num2 * 256304674) ^ 0x7FDF1E0C);
				}
			}
		}

		public string GetName()
		{
			if (!_202E_206F_200E_202E_202B_200E_200B_200E_202A_202B_202B_206B_200D_200F_202C_200C_200B_202E_200D_202D_202B_202B_200F_206B_206D_202C_200F_206D_202A_202C_206E_202A_200D_206D_200B_200E_206A_200E_206E_200C_202E(_200D_200C_206D_200C_206A_206B_206E_200E_206D_202B_200D_200D_206B_206B_206A_202C_206D_202D_200B_200C_200B_202C_206B_206A_206F_206F_202C_202B_206C_202D_206F_206E_200B_202E_206B_206A_200F_206F_200B_206E_202E((Object)prefab)))
			{
				while (true)
				{
					int num = -1669712464;
					while (true)
					{
						uint num2;
						switch ((num2 = (uint)num ^ 0xCFCB1955u) % 4)
						{
						case 0u:
							break;
						case 1u:
						{
							int num3;
							int num4;
							if (_202E_206F_200E_202E_202B_200E_200B_200E_202A_202B_202B_206B_200D_200F_202C_200C_200B_202E_200D_202D_202B_202B_200F_206B_206D_202C_200F_206D_202A_202C_206E_202A_200D_206D_200B_200E_206A_200E_206E_200C_202E(_206C_206C_200E_200B_206F_206F_202C_206F_206E_202C_206F_200E_202A_206F_202E_202B_202D_200C_200F_202D_202B_206A_202E_200F_206D_206F_202D_206F_202C_200B_206B_202B_202B_200F_200F_202A_200E_206B_202C_206B_202E(_200D_200C_206D_200C_206A_206B_206E_200E_206D_202B_200D_200D_206B_206B_206A_202C_206D_202D_200B_200C_200B_202C_206B_206A_206F_206F_202C_202B_206C_202D_206F_206E_200B_202E_206B_206A_200F_206F_200B_206E_202E((Object)prefab))))
							{
								num3 = -2050662171;
								num4 = num3;
							}
							else
							{
								num3 = -1904449648;
								num4 = num3;
							}
							num = num3 ^ (int)(num2 * 895567126);
							continue;
						}
						case 3u:
							return _200D_200C_206D_200C_206A_206B_206E_200E_206D_202B_200D_200D_206B_206B_206A_202C_206D_202D_200B_200C_200B_202C_206B_206A_206F_206F_202C_202B_206C_202D_206F_206E_200B_202E_206B_206A_200F_206F_200B_206E_202E((Object)prefab);
						default:
							goto end_IL_0012;
						}
						break;
					}
					continue;
					end_IL_0012:
					break;
				}
			}
			return _206C_200C_202B_200D_202C_200F_202E_202B_206E_202A_200D_200E_202A_202A_200F_202A_206C_206B_206C_200C_206C_206A_206E_200F_206E_200D_206D_200C_206F_200B_200F_206A_206B_206B_202C_202A_202E_200D_202E_206D_202E("ID ", _206F_200B_206D_202C_206A_200B_200D_200E_206B_200C_200E_206C_202C_200C_206A_200E_202B_200F_206E_200E_206F_200B_206D_200B_200C_200B_202A_206E_200B_206F_206D_200F_200B_200E_200F_206B_206B_202A_200D_206C_202E((Object)prefab).ToString());
		}

		static string _200D_200C_206D_200C_206A_206B_206E_200E_206D_202B_200D_200D_206B_206B_206A_202C_206D_202D_200B_200C_200B_202C_206B_206A_206F_206F_202C_202B_206C_202D_206F_206E_200B_202E_206B_206A_200F_206F_200B_206E_202E(Object P_0)
		{
			return P_0.name;
		}

		static bool _202E_206F_200E_202E_202B_200E_200B_200E_202A_202B_202B_206B_200D_200F_202C_200C_200B_202E_200D_202D_202B_202B_200F_206B_206D_202C_200F_206D_202A_202C_206E_202A_200D_206D_200B_200E_206A_200E_206E_200C_202E(string P_0)
		{
			return string.IsNullOrEmpty(P_0);
		}

		static string _206C_206C_200E_200B_206F_206F_202C_206F_206E_202C_206F_200E_202A_206F_202E_202B_202D_200C_200F_202D_202B_206A_202E_200F_206D_206F_202D_206F_202C_200B_206B_202B_202B_200F_200F_202A_200E_206B_202C_206B_202E(string P_0)
		{
			return P_0.Trim();
		}

		static int _206F_200B_206D_202C_206A_200B_200D_200E_206B_200C_200E_206C_202C_200C_206A_200E_202B_200F_206E_200E_206F_200B_206D_200B_200C_200B_202A_206E_200B_206F_206D_200F_200B_200E_200F_206B_206B_202A_200D_206C_202E(Object P_0)
		{
			return P_0.GetInstanceID();
		}

		static string _206C_200C_202B_200D_202C_200F_202E_202B_206E_202A_200D_200E_202A_202A_200F_202A_206C_206B_206C_200C_206C_206A_206E_200F_206E_200D_206D_200C_206F_200B_200F_206A_206B_206B_202C_202A_202E_200D_202E_206D_202E(string P_0, string P_1)
		{
			return P_0 + P_1;
		}
	}
}
