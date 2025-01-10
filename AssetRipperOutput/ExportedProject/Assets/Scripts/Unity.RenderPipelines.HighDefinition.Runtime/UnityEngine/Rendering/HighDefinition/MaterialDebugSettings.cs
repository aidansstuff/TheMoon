using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Rendering.HighDefinition.Attributes;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class MaterialDebugSettings
	{
		internal class MaterialItem
		{
			public string className;

			public Type surfaceDataType;

			public Type bsdfDataType;
		}

		private static bool isDebugViewMaterialInit;

		internal static GUIContent[] debugViewMaterialStrings;

		internal static int[] debugViewMaterialValues;

		internal static GUIContent[] debugViewEngineStrings;

		internal static int[] debugViewEngineValues;

		internal static GUIContent[] debugViewMaterialVaryingStrings;

		internal static int[] debugViewMaterialVaryingValues;

		internal static GUIContent[] debugViewMaterialPropertiesStrings;

		internal static int[] debugViewMaterialPropertiesValues;

		internal static GUIContent[] debugViewMaterialTextureStrings;

		internal static int[] debugViewMaterialTextureValues;

		public static GUIContent[] debugViewMaterialGBufferStrings;

		public static int[] debugViewMaterialGBufferValues;

		private static Dictionary<MaterialSharedProperty, int[]> s_MaterialPropertyMap;

		public MaterialSharedProperty debugViewMaterialCommonValue;

		public Color materialValidateLowColor = new Color(1f, 0f, 0f);

		public Color materialValidateHighColor = new Color(0f, 0f, 1f);

		public Color materialValidateTrueMetalColor = new Color(1f, 1f, 0f);

		public bool materialValidateTrueMetal;

		private const int kDebugViewMaterialBufferLength = 10;

		private static float[] s_DebugViewMaterialOffsetedBuffer;

		private int[] m_DebugViewMaterial = new int[11];

		private int m_DebugViewEngine;

		private DebugViewVarying m_DebugViewVarying;

		private DebugViewProperties m_DebugViewProperties;

		private int m_DebugViewGBuffer;

		internal int materialEnumIndex;

		public int[] debugViewMaterial
		{
			get
			{
				return m_DebugViewMaterial;
			}
			internal set
			{
				int num = ((value != null) ? value.Length : 0);
				if (num > 10)
				{
					Debug.LogError($"DebugViewMaterialBuffer is cannot handle {num} elements. Only first {10} are kept.");
				}
				int num2 = Mathf.Min(10, num);
				if (num2 == 0)
				{
					m_DebugViewMaterial[0] = 1;
					m_DebugViewMaterial[1] = 0;
					return;
				}
				m_DebugViewMaterial[0] = num2;
				for (int i = 0; i < num2; i++)
				{
					m_DebugViewMaterial[i + 1] = value[i];
				}
			}
		}

		public int debugViewEngine => m_DebugViewEngine;

		public DebugViewVarying debugViewVarying => m_DebugViewVarying;

		public DebugViewProperties debugViewProperties => m_DebugViewProperties;

		public int debugViewGBuffer => m_DebugViewGBuffer;

		static MaterialDebugSettings()
		{
			isDebugViewMaterialInit = false;
			debugViewMaterialStrings = null;
			debugViewMaterialValues = null;
			debugViewEngineStrings = null;
			debugViewEngineValues = null;
			debugViewMaterialVaryingStrings = null;
			debugViewMaterialVaryingValues = null;
			debugViewMaterialPropertiesStrings = null;
			debugViewMaterialPropertiesValues = null;
			debugViewMaterialTextureStrings = null;
			debugViewMaterialTextureValues = null;
			debugViewMaterialGBufferStrings = null;
			debugViewMaterialGBufferValues = null;
			s_MaterialPropertyMap = new Dictionary<MaterialSharedProperty, int[]>();
			s_DebugViewMaterialOffsetedBuffer = new float[11];
			BuildDebugRepresentation();
		}

		private static void FillWithProperties(Type type, ref List<GUIContent> debugViewMaterialStringsList, ref List<int> debugViewMaterialValuesList, string className = "")
		{
			GenerateHLSL customAttribute = type.GetCustomAttribute<GenerateHLSL>();
			if (!customAttribute.needParamDebug)
			{
				return;
			}
			List<(GUIContent, int)> value;
			using (ListPool<(GUIContent, int)>.Get(out value))
			{
				FieldInfo[] fields = type.GetFields();
				int num = 0;
				FieldInfo[] array = fields;
				foreach (FieldInfo fieldInfo in array)
				{
					List<string> list = new List<string>();
					if (Attribute.IsDefined(fieldInfo, typeof(PackingAttribute)))
					{
						PackingAttribute[] array2 = (PackingAttribute[])fieldInfo.GetCustomAttributes(typeof(PackingAttribute), inherit: false);
						foreach (PackingAttribute packingAttribute in array2)
						{
							list.AddRange(packingAttribute.displayNames);
						}
					}
					else
					{
						list.Add(fieldInfo.Name);
					}
					if (Attribute.IsDefined(fieldInfo, typeof(SurfaceDataAttributes)))
					{
						SurfaceDataAttributes[] array3 = (SurfaceDataAttributes[])fieldInfo.GetCustomAttributes(typeof(SurfaceDataAttributes), inherit: false);
						if (array3[0].displayNames.Length != 0 && array3[0].displayNames[0] != "")
						{
							list.Clear();
							list.AddRange(array3[0].displayNames);
						}
					}
					foreach (string item3 in list)
					{
						value.Add((new GUIContent(className + item3), customAttribute.paramDefinesStart + num));
						num++;
					}
				}
				foreach (var (item, item2) in value.OrderBy(((GUIContent, int) t) => t.Item1.text))
				{
					debugViewMaterialStringsList.Add(item);
					debugViewMaterialValuesList.Add(item2);
				}
			}
		}

		private static void FillWithPropertiesEnum(Type type, ref List<GUIContent> debugViewMaterialStringsList, ref List<int> debugViewMaterialValuesList, string prefix)
		{
			string[] names = Enum.GetNames(type);
			int num = 0;
			foreach (object value in Enum.GetValues(type))
			{
				string text = prefix + names[num];
				debugViewMaterialStringsList.Add(new GUIContent(text));
				debugViewMaterialValuesList.Add((int)value);
				num++;
			}
		}

		private static List<MaterialItem> GetAllMaterialDatas()
		{
			List<RenderPipelineMaterial> renderPipelineMaterialList = HDUtils.GetRenderPipelineMaterialList();
			foreach (RenderPipelineMaterial item in renderPipelineMaterialList)
			{
				if (item.IsDefferedMaterial())
				{
					item.GetType().GetNestedType("BSDFData");
				}
			}
			List<MaterialItem> list = new List<MaterialItem>();
			int num = 0;
			int num2 = 0;
			foreach (RenderPipelineMaterial item2 in renderPipelineMaterialList)
			{
				Type type = item2.GetType();
				MaterialItem materialItem = new MaterialItem
				{
					className = type.Name + "/",
					surfaceDataType = type.GetNestedType("SurfaceData"),
					bsdfDataType = type.GetNestedType("BSDFData")
				};
				num += materialItem.surfaceDataType.GetFields().Length;
				num2 += materialItem.bsdfDataType.GetFields().Length;
				list.Add(materialItem);
			}
			return list;
		}

		private static void BuildDebugRepresentation()
		{
			if (isDebugViewMaterialInit)
			{
				return;
			}
			List<MaterialItem> allMaterialDatas = GetAllMaterialDatas();
			FillMaterialsInfos(allMaterialDatas);
			List<GUIContent> debugViewMaterialStringsList = new List<GUIContent>();
			List<int> debugViewMaterialValuesList = new List<int>();
			List<GUIContent> debugViewMaterialStringsList2 = new List<GUIContent>();
			List<int> debugViewMaterialValuesList2 = new List<int>();
			List<GUIContent> debugViewMaterialStringsList3 = new List<GUIContent>();
			List<int> debugViewMaterialValuesList3 = new List<int>();
			List<GUIContent> list = new List<GUIContent>();
			List<int> list2 = new List<int>();
			List<GUIContent> debugViewMaterialStringsList4 = new List<GUIContent>();
			List<int> debugViewMaterialValuesList4 = new List<int>();
			debugViewMaterialStringsList.Add(new GUIContent("None"));
			debugViewMaterialValuesList.Add(0);
			foreach (MaterialItem item in allMaterialDatas)
			{
				FillWithProperties(item.bsdfDataType, ref debugViewMaterialStringsList, ref debugViewMaterialValuesList, item.className);
			}
			FillWithPropertiesEnum(typeof(DebugViewVarying), ref debugViewMaterialStringsList2, ref debugViewMaterialValuesList2, "");
			FillWithPropertiesEnum(typeof(DebugViewProperties), ref debugViewMaterialStringsList3, ref debugViewMaterialValuesList3, "");
			FillWithPropertiesEnum(typeof(DebugViewGbuffer), ref debugViewMaterialStringsList4, ref debugViewMaterialValuesList4, "");
			FillWithProperties(typeof(Lit.BSDFData), ref debugViewMaterialStringsList4, ref debugViewMaterialValuesList4);
			debugViewEngineStrings = debugViewMaterialStringsList.ToArray();
			debugViewEngineValues = debugViewMaterialValuesList.ToArray();
			debugViewMaterialVaryingStrings = debugViewMaterialStringsList2.ToArray();
			debugViewMaterialVaryingValues = debugViewMaterialValuesList2.ToArray();
			debugViewMaterialPropertiesStrings = debugViewMaterialStringsList3.ToArray();
			debugViewMaterialPropertiesValues = debugViewMaterialValuesList3.ToArray();
			debugViewMaterialTextureStrings = list.ToArray();
			debugViewMaterialTextureValues = list2.ToArray();
			debugViewMaterialGBufferStrings = debugViewMaterialStringsList4.ToArray();
			debugViewMaterialGBufferValues = debugViewMaterialValuesList4.ToArray();
			Dictionary<MaterialSharedProperty, List<int>> dictionary = new Dictionary<MaterialSharedProperty, List<int>>
			{
				{
					MaterialSharedProperty.Albedo,
					new List<int>()
				},
				{
					MaterialSharedProperty.Normal,
					new List<int>()
				},
				{
					MaterialSharedProperty.Smoothness,
					new List<int>()
				},
				{
					MaterialSharedProperty.AmbientOcclusion,
					new List<int>()
				},
				{
					MaterialSharedProperty.Metal,
					new List<int>()
				},
				{
					MaterialSharedProperty.Specular,
					new List<int>()
				},
				{
					MaterialSharedProperty.Alpha,
					new List<int>()
				}
			};
			int paramDefinesStart = typeof(Builtin.BuiltinData).GetCustomAttribute<GenerateHLSL>().paramDefinesStart;
			int num = 0;
			FieldInfo[] fields = typeof(Builtin.BuiltinData).GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (Attribute.IsDefined(fieldInfo, typeof(MaterialSharedPropertyMappingAttribute)))
				{
					MaterialSharedPropertyMappingAttribute[] array = (MaterialSharedPropertyMappingAttribute[])fieldInfo.GetCustomAttributes(typeof(MaterialSharedPropertyMappingAttribute), inherit: false);
					dictionary[array[0].property].Add(paramDefinesStart + num);
				}
				SurfaceDataAttributes[] array2 = (SurfaceDataAttributes[])fieldInfo.GetCustomAttributes(typeof(SurfaceDataAttributes), inherit: false);
				if (array2.Length != 0)
				{
					num += array2[0].displayNames.Length;
				}
			}
			foreach (MaterialItem item2 in allMaterialDatas)
			{
				GenerateHLSL customAttribute = item2.surfaceDataType.GetCustomAttribute<GenerateHLSL>();
				paramDefinesStart = customAttribute.paramDefinesStart;
				if (!customAttribute.needParamDebug)
				{
					continue;
				}
				FieldInfo[] fields2 = item2.surfaceDataType.GetFields();
				num = 0;
				fields = fields2;
				foreach (FieldInfo fieldInfo2 in fields)
				{
					if (Attribute.IsDefined(fieldInfo2, typeof(MaterialSharedPropertyMappingAttribute)))
					{
						MaterialSharedPropertyMappingAttribute[] array3 = (MaterialSharedPropertyMappingAttribute[])fieldInfo2.GetCustomAttributes(typeof(MaterialSharedPropertyMappingAttribute), inherit: false);
						dictionary[array3[0].property].Add(paramDefinesStart + num);
					}
					SurfaceDataAttributes[] array4 = (SurfaceDataAttributes[])fieldInfo2.GetCustomAttributes(typeof(SurfaceDataAttributes), inherit: false);
					if (array4.Length != 0)
					{
						num += array4[0].displayNames.Length;
					}
				}
				if (item2.bsdfDataType == null)
				{
					continue;
				}
				GenerateHLSL customAttribute2 = item2.bsdfDataType.GetCustomAttribute<GenerateHLSL>();
				paramDefinesStart = customAttribute2.paramDefinesStart;
				if (!customAttribute2.needParamDebug)
				{
					continue;
				}
				FieldInfo[] fields3 = item2.bsdfDataType.GetFields();
				num = 0;
				fields = fields3;
				foreach (FieldInfo fieldInfo3 in fields)
				{
					if (Attribute.IsDefined(fieldInfo3, typeof(MaterialSharedPropertyMappingAttribute)))
					{
						MaterialSharedPropertyMappingAttribute[] array5 = (MaterialSharedPropertyMappingAttribute[])fieldInfo3.GetCustomAttributes(typeof(MaterialSharedPropertyMappingAttribute), inherit: false);
						dictionary[array5[0].property].Add(paramDefinesStart + num++);
					}
					SurfaceDataAttributes[] array6 = (SurfaceDataAttributes[])fieldInfo3.GetCustomAttributes(typeof(SurfaceDataAttributes), inherit: false);
					if (array6.Length != 0)
					{
						num += array6[0].displayNames.Length;
					}
				}
			}
			foreach (MaterialSharedProperty key in dictionary.Keys)
			{
				s_MaterialPropertyMap[key] = dictionary[key].ToArray();
			}
			isDebugViewMaterialInit = true;
		}

		private static void FillMaterialsInfos(List<MaterialItem> materialItems)
		{
			List<GUIContent> value;
			using (ListPool<GUIContent>.Get(out value))
			{
				List<int> value2;
				using (ListPool<int>.Get(out value2))
				{
					value.Add(new GUIContent("None"));
					value2.Add(0);
					FillWithProperties(typeof(Builtin.BuiltinData), ref value, ref value2, "Common/");
					foreach (MaterialItem materialItem in materialItems)
					{
						FillWithProperties(materialItem.surfaceDataType, ref value, ref value2, materialItem.className);
					}
					debugViewMaterialStrings = value.ToArray();
					debugViewMaterialValues = value2.ToArray();
				}
			}
		}

		internal float[] GetDebugMaterialIndexes()
		{
			int num = m_DebugViewMaterial[0];
			s_DebugViewMaterialOffsetedBuffer[0] = num;
			for (int i = 1; i <= num; i++)
			{
				s_DebugViewMaterialOffsetedBuffer[i] = (int)(m_DebugViewGBuffer + m_DebugViewMaterial[i] + m_DebugViewEngine + m_DebugViewVarying) + (int)m_DebugViewProperties;
			}
			return s_DebugViewMaterialOffsetedBuffer;
		}

		public void DisableMaterialDebug()
		{
			debugViewMaterialCommonValue = MaterialSharedProperty.None;
			m_DebugViewMaterial[0] = 1;
			m_DebugViewMaterial[1] = 0;
			m_DebugViewEngine = 0;
			m_DebugViewVarying = DebugViewVarying.None;
			m_DebugViewProperties = DebugViewProperties.None;
			m_DebugViewGBuffer = 0;
		}

		public void SetDebugViewCommonMaterialProperty(MaterialSharedProperty value)
		{
			if (value != 0)
			{
				DisableMaterialDebug();
				materialEnumIndex = 0;
			}
			debugViewMaterial = ((value == MaterialSharedProperty.None) ? null : s_MaterialPropertyMap[value]);
		}

		public void SetDebugViewMaterial(int value)
		{
			debugViewMaterialCommonValue = MaterialSharedProperty.None;
			if (value != 0)
			{
				DisableMaterialDebug();
				m_DebugViewMaterial[0] = 1;
				m_DebugViewMaterial[1] = value;
			}
			else
			{
				m_DebugViewMaterial[0] = 1;
				m_DebugViewMaterial[1] = 0;
			}
		}

		public void SetDebugViewEngine(int value)
		{
			if (value != 0)
			{
				DisableMaterialDebug();
			}
			m_DebugViewEngine = value;
		}

		public void SetDebugViewVarying(DebugViewVarying value)
		{
			if (value != 0)
			{
				DisableMaterialDebug();
			}
			m_DebugViewVarying = value;
		}

		public void SetDebugViewProperties(DebugViewProperties value)
		{
			if (value != 0)
			{
				DisableMaterialDebug();
			}
			m_DebugViewProperties = value;
		}

		public void SetDebugViewGBuffer(int value)
		{
			if (value != 0)
			{
				DisableMaterialDebug();
			}
			m_DebugViewGBuffer = value;
		}

		public bool IsDebugGBufferEnabled()
		{
			return m_DebugViewGBuffer != 0;
		}

		public bool IsDebugViewMaterialEnabled()
		{
			int[] array = m_DebugViewMaterial;
			int num = ((array != null) ? array[0] : 0);
			bool flag = false;
			for (int i = 1; i <= num; i++)
			{
				flag |= m_DebugViewMaterial[i] != 0;
			}
			return flag;
		}

		public bool IsDebugDisplayEnabled()
		{
			if (m_DebugViewEngine == 0 && !IsDebugViewMaterialEnabled() && m_DebugViewVarying == DebugViewVarying.None && m_DebugViewProperties == DebugViewProperties.None)
			{
				return IsDebugGBufferEnabled();
			}
			return true;
		}
	}
}
