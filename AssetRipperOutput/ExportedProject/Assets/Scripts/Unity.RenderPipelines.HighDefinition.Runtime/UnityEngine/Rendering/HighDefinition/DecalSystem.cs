using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Jobs;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class DecalSystem
	{
		public enum MaterialDecalPass
		{
			DBufferProjector = 0,
			DecalProjectorForwardEmissive = 1,
			DBufferMesh = 2,
			DecalMeshForwardEmissive = 3
		}

		public class CullResult : IDisposable
		{
			public class Set : IDisposable
			{
				private int m_NumResults;

				private int[] m_ResultIndices;

				public int numResults => m_NumResults;

				public int[] resultIndices => m_ResultIndices;

				public void Dispose()
				{
					Dispose(disposing: true);
				}

				private void Dispose(bool disposing)
				{
					if (disposing)
					{
						Clear();
						m_ResultIndices = null;
					}
				}

				public void Clear()
				{
					m_NumResults = 0;
				}

				public int QueryIndices(int maxLength, CullingGroup cullingGroup)
				{
					if (m_ResultIndices == null || m_ResultIndices.Length < maxLength)
					{
						Array.Resize(ref m_ResultIndices, maxLength);
					}
					m_NumResults = cullingGroup.QueryIndices(visible: true, m_ResultIndices, 0);
					return m_NumResults;
				}
			}

			private Dictionary<int, Set> m_Requests = new Dictionary<int, Set>();

			public Dictionary<int, Set> requests => m_Requests;

			public Set this[int index]
			{
				get
				{
					if (!m_Requests.TryGetValue(index, out var value))
					{
						value = GenericPool<Set>.Get();
						m_Requests.Add(index, value);
					}
					return value;
				}
			}

			public void Clear()
			{
				foreach (KeyValuePair<int, Set> request in m_Requests)
				{
					request.Value.Clear();
					GenericPool<Set>.Release(request.Value);
				}
				m_Requests.Clear();
			}

			public void Dispose()
			{
				Dispose(disposing: true);
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					m_Requests.Clear();
					m_Requests = null;
				}
			}
		}

		public class CullRequest : IDisposable
		{
			public class Set : IDisposable
			{
				private CullingGroup m_CullingGroup;

				public CullingGroup cullingGroup => m_CullingGroup;

				public void Dispose()
				{
					Dispose(disposing: true);
				}

				private void Dispose(bool disposing)
				{
					if (disposing)
					{
						Clear();
					}
				}

				public void Clear()
				{
					if (m_CullingGroup != null)
					{
						CullingGroupManager.instance.Free(m_CullingGroup);
					}
					m_CullingGroup = null;
				}

				public void Initialize(CullingGroup cullingGroup)
				{
					m_CullingGroup = cullingGroup;
				}
			}

			private Dictionary<int, Set> m_Requests = new Dictionary<int, Set>();

			public Set this[int index]
			{
				get
				{
					if (!m_Requests.TryGetValue(index, out var value))
					{
						value = GenericPool<Set>.Get();
						m_Requests.Add(index, value);
					}
					return value;
				}
			}

			public void Clear()
			{
				foreach (KeyValuePair<int, Set> request in m_Requests)
				{
					request.Value.Clear();
					GenericPool<Set>.Release(request.Value);
				}
				m_Requests.Clear();
			}

			public void Dispose()
			{
				Dispose(disposing: true);
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					m_Requests.Clear();
					m_Requests = null;
				}
			}
		}

		public class DecalHandle
		{
			public int m_MaterialID;

			public int m_Index;

			public DecalHandle(int index, int materialID)
			{
				m_MaterialID = materialID;
				m_Index = index;
			}

			public static bool IsValid(DecalHandle handle)
			{
				if (handle == null)
				{
					return false;
				}
				if (handle.m_Index == -1)
				{
					return false;
				}
				return true;
			}
		}

		public class TextureScaleBias : IComparable
		{
			public Texture m_Texture;

			public Vector4 m_ScaleBias = Vector4.zero;

			public int CompareTo(object obj)
			{
				TextureScaleBias textureScaleBias = obj as TextureScaleBias;
				int num = m_Texture.width * m_Texture.height;
				int num2 = textureScaleBias.m_Texture.width * textureScaleBias.m_Texture.height;
				if (num > num2)
				{
					return -1;
				}
				if (num < num2)
				{
					return 1;
				}
				return 0;
			}

			public void Initialize(Texture texture, Vector4 scaleBias)
			{
				m_Texture = texture;
				m_ScaleBias = scaleBias;
			}
		}

		private class DecalSet : IDisposable
		{
			private List<Matrix4x4[]> m_DecalToWorld = new List<Matrix4x4[]>();

			private List<Matrix4x4[]> m_NormalToWorld = new List<Matrix4x4[]>();

			private List<float[]> m_DecalLayerMasks = new List<float[]>();

			private DecalHandle[] m_Handles = new DecalHandle[128];

			private int[] m_ResultIndices = new int[128];

			private int m_NumResults;

			private int m_InstanceCount;

			private int m_DecalsCount;

			private int m_CachedDrawOrder;

			private Vector2[] m_CachedDrawDistances = new Vector2[128];

			private Vector2[] m_CachedAngleFade = new Vector2[128];

			private Vector4[] m_CachedUVScaleBias = new Vector4[128];

			private bool[] m_CachedAffectsTransparency = new bool[128];

			private int[] m_CachedLayerMask = new int[128];

			private DecalLayerEnum[] m_CachedDecalLayerMask = new DecalLayerEnum[128];

			private ulong[] m_CachedSceneLayerMask = new ulong[128];

			private float[] m_CachedFadeFactor = new float[128];

			private Material m_Material;

			private MaterialPropertyBlock m_PropertyBlock = new MaterialPropertyBlock();

			private float m_Blend;

			private Vector4 m_BaseColor;

			private Vector4 m_RemappingAOS;

			private Vector4 m_ScalingBAndRemappingM;

			private Vector3 m_BlendParams;

			private bool m_IsHDRenderPipelineDecal;

			private int m_cachedProjectorPassValue;

			private int m_cachedProjectorEmissivePassValue;

			private TextureScaleBias m_Diffuse = new TextureScaleBias();

			private TextureScaleBias m_Normal = new TextureScaleBias();

			private TextureScaleBias m_Mask = new TextureScaleBias();

			private JobHandle m_UpdateJobHandle;

			private TransformAccessArray m_CachedTransforms = new TransformAccessArray(128);

			private NativeArray<float3> m_Positions = new NativeArray<float3>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private NativeArray<quaternion> m_Rotations = new NativeArray<quaternion>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private NativeArray<float3> m_Scales = new NativeArray<float3>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private NativeArray<float3> m_Sizes = new NativeArray<float3>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private NativeArray<float3> m_Offsets = new NativeArray<float3>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private NativeArray<quaternion> m_ResolvedRotations = new NativeArray<quaternion>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private NativeArray<float3> m_ResolvedScales = new NativeArray<float3>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private NativeArray<float4x4> m_ResolvedSizeOffsets = new NativeArray<float4x4>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private NativeArray<DecalScaleMode> m_ScaleModes = new NativeArray<DecalScaleMode>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private NativeArray<float4x4> m_NormalToWorlds = new NativeArray<float4x4>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private NativeArray<float4x4> m_DecalToWorlds = new NativeArray<float4x4>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private NativeArray<BoundingSphere> m_BoundingSpheres = new NativeArray<BoundingSphere>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private NativeArray<bool> m_Dirty = new NativeArray<bool>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

			private BoundingSphere[] m_CachedBoundingSpheres = new BoundingSphere[128];

			public Material KeyMaterial => m_Material;

			public int Count => m_DecalsCount;

			public bool HasEmissivePass => m_cachedProjectorEmissivePassValue != -1;

			public int DrawOrder => m_CachedDrawOrder;

			internal JobHandle updateJobHandle => m_UpdateJobHandle;

			public void InitializeMaterialValues()
			{
				if (m_Material == null)
				{
					return;
				}
				m_IsHDRenderPipelineDecal = IsHDRenderPipelineDecal(m_Material);
				if (m_IsHDRenderPipelineDecal)
				{
					bool flag = m_Material.GetFloat(HDShaderIDs._AffectNormal) != 0f;
					m_Normal.Initialize(flag ? m_Material.GetTexture("_NormalMap") : null, Vector4.zero);
					bool flag2 = m_Material.GetFloat(HDShaderIDs._AffectMetal) != 0f;
					bool flag3 = m_Material.GetFloat(HDShaderIDs._AffectAO) != 0f;
					bool flag4 = m_Material.GetFloat(HDShaderIDs._AffectSmoothness) != 0f;
					bool flag5 = flag2 || flag3 || flag4;
					m_Mask.Initialize(flag5 ? m_Material.GetTexture("_MaskMap") : null, Vector4.zero);
					float @float = m_Material.GetFloat("_NormalBlendSrc");
					float float2 = m_Material.GetFloat("_MaskBlendSrc");
					bool num = m_Material.GetFloat(HDShaderIDs._AffectAlbedo) != 0f;
					m_Diffuse.Initialize(m_Material.GetTexture("_BaseColorMap"), Vector4.zero);
					m_Blend = m_Material.GetFloat("_DecalBlend");
					m_BaseColor = m_Material.GetVector("_BaseColor");
					m_BlendParams = new Vector3(@float, float2, 0f);
					int num2 = (num ? 1 : 0) | (flag ? 2 : 0) | (flag2 ? 4 : 0) | (flag3 ? 8 : 0) | (flag4 ? 16 : 0);
					m_BlendParams.z = num2;
					m_ScalingBAndRemappingM = new Vector4(0f, m_Material.GetFloat("_DecalMaskMapBlueScale"), 0f, 0f);
					if ((bool)m_Material.GetTexture("_MaskMap"))
					{
						m_RemappingAOS = new Vector4(m_Material.GetFloat("_AORemapMin"), m_Material.GetFloat("_AORemapMax"), m_Material.GetFloat("_SmoothnessRemapMin"), m_Material.GetFloat("_SmoothnessRemapMax"));
						m_ScalingBAndRemappingM.z = m_Material.GetFloat("_MetallicRemapMin");
						m_ScalingBAndRemappingM.w = m_Material.GetFloat("_MetallicRemapMax");
					}
					else
					{
						m_RemappingAOS = new Vector4(m_Material.GetFloat("_AO"), m_Material.GetFloat("_AO"), m_Material.GetFloat("_Smoothness"), m_Material.GetFloat("_Smoothness"));
						m_ScalingBAndRemappingM.z = m_Material.GetFloat("_Metallic");
					}
					m_cachedProjectorPassValue = -1;
					if (m_Material.GetShaderPassEnabled(s_MaterialDecalPassNames[0]))
					{
						m_cachedProjectorPassValue = 0;
					}
					m_cachedProjectorEmissivePassValue = -1;
					if (m_Material.GetShaderPassEnabled(s_MaterialDecalPassNames[1]))
					{
						m_cachedProjectorEmissivePassValue = 1;
					}
				}
				else
				{
					m_Blend = 1f;
					m_cachedProjectorPassValue = m_Material.FindPass(s_MaterialDecalPassNames[0]);
					if (m_cachedProjectorPassValue != -1 && !m_Material.GetShaderPassEnabled(s_MaterialDecalPassNames[0]))
					{
						m_cachedProjectorPassValue = -1;
					}
					m_cachedProjectorEmissivePassValue = m_Material.FindPass(s_MaterialDecalPassNames[1]);
					if (m_cachedProjectorEmissivePassValue != -1 && !m_Material.GetShaderPassEnabled(s_MaterialDecalPassNames[1]))
					{
						m_cachedProjectorEmissivePassValue = -1;
					}
				}
			}

			public void Dispose()
			{
				Dispose(disposing: true);
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					DisposeJobArrays();
				}
			}

			public DecalSet(Material material)
			{
				m_Material = material;
				InitializeMaterialValues();
			}

			public void UpdateCachedData(DecalHandle handle, DecalProjector decalProjector)
			{
				DecalProjector.CachedDecalData cachedDecalData = decalProjector.GetCachedDecalData();
				int index = handle.m_Index;
				m_CachedDrawDistances[index].x = ((cachedDecalData.drawDistance < (float)instance.DrawDistance) ? cachedDecalData.drawDistance : ((float)instance.DrawDistance));
				m_CachedDrawDistances[index].y = cachedDecalData.fadeScale;
				if (cachedDecalData.startAngleFade == 180f)
				{
					m_CachedAngleFade[index].x = 0f;
					m_CachedAngleFade[index].y = 0f;
				}
				else
				{
					float num = cachedDecalData.startAngleFade / 180f;
					float num2 = cachedDecalData.endAngleFade / 180f;
					float num3 = Mathf.Max(0.0001f, num2 - num);
					m_CachedAngleFade[index].x = 2f / 9f / num3;
					m_CachedAngleFade[index].y = (num2 - 0.5f) / num3;
				}
				m_CachedUVScaleBias[index] = cachedDecalData.uvScaleBias;
				m_CachedAffectsTransparency[index] = cachedDecalData.affectsTransparency;
				m_CachedLayerMask[index] = cachedDecalData.layerMask;
				m_CachedSceneLayerMask[index] = cachedDecalData.sceneLayerMask;
				m_CachedFadeFactor[index] = cachedDecalData.fadeFactor;
				m_CachedDecalLayerMask[index] = cachedDecalData.decalLayerMask;
				UpdateCachedDrawOrder();
				UpdateJobArrays(index, decalProjector);
			}

			public void UpdateCachedDrawOrder()
			{
				if (m_Material != null && m_Material.HasProperty(HDShaderIDs._DrawOrder))
				{
					m_CachedDrawOrder = m_Material.GetInt(HDShaderIDs._DrawOrder);
				}
				else
				{
					m_CachedDrawOrder = 0;
				}
			}

			public DecalHandle AddDecal(int materialID, DecalProjector decalProjector)
			{
				if (m_DecalsCount == m_Handles.Length)
				{
					int num = m_DecalsCount + 128;
					m_ResultIndices = new int[num];
					ResizeJobArrays(num);
					ArrayExtensions.ResizeArray(ref m_Handles, num);
					ArrayExtensions.ResizeArray(ref m_CachedDrawDistances, num);
					ArrayExtensions.ResizeArray(ref m_CachedAngleFade, num);
					ArrayExtensions.ResizeArray(ref m_CachedUVScaleBias, num);
					ArrayExtensions.ResizeArray(ref m_CachedAffectsTransparency, num);
					ArrayExtensions.ResizeArray(ref m_CachedLayerMask, num);
					ArrayExtensions.ResizeArray(ref m_CachedSceneLayerMask, num);
					ArrayExtensions.ResizeArray(ref m_CachedDecalLayerMask, num);
					ArrayExtensions.ResizeArray(ref m_CachedFadeFactor, num);
				}
				DecalHandle decalHandle = new DecalHandle(m_DecalsCount, materialID);
				m_Handles[m_DecalsCount] = decalHandle;
				UpdateCachedData(decalHandle, decalProjector);
				m_DecalsCount++;
				return decalHandle;
			}

			public void RemoveDecal(DecalHandle handle)
			{
				int index = handle.m_Index;
				m_Handles[index] = m_Handles[m_DecalsCount - 1];
				m_Handles[index].m_Index = index;
				m_Handles[m_DecalsCount - 1] = null;
				RemoveFromJobArrays(index);
				m_CachedDrawDistances[index] = m_CachedDrawDistances[m_DecalsCount - 1];
				m_CachedAngleFade[index] = m_CachedAngleFade[m_DecalsCount - 1];
				m_CachedUVScaleBias[index] = m_CachedUVScaleBias[m_DecalsCount - 1];
				m_CachedAffectsTransparency[index] = m_CachedAffectsTransparency[m_DecalsCount - 1];
				m_CachedLayerMask[index] = m_CachedLayerMask[m_DecalsCount - 1];
				m_CachedSceneLayerMask[index] = m_CachedSceneLayerMask[m_DecalsCount - 1];
				m_CachedFadeFactor[index] = m_CachedFadeFactor[m_DecalsCount - 1];
				m_DecalsCount--;
				handle.m_Index = -1;
			}

			public void BeginCull(CullRequest.Set cullRequest)
			{
				cullRequest.Clear();
				if (!(m_Material == null))
				{
					if (cullRequest.cullingGroup != null)
					{
						Debug.LogError("Begin/EndCull() called out of sequence for decal projectors.");
					}
					ResolveUpdateJob();
					m_BoundingDistances[0] = instance.DrawDistance;
					m_NumResults = 0;
					CullingGroup cullingGroup = CullingGroupManager.instance.Alloc();
					cullingGroup.targetCamera = instance.CurrentCamera;
					cullingGroup.SetDistanceReferencePoint(cullingGroup.targetCamera.transform.position);
					cullingGroup.SetBoundingDistances(m_BoundingDistances);
					cullingGroup.SetBoundingSpheres(m_CachedBoundingSpheres);
					cullingGroup.SetBoundingSphereCount(m_DecalsCount);
					cullRequest.Initialize(cullingGroup);
				}
			}

			public int QueryCullResults(CullRequest.Set cullRequest, CullResult.Set cullResult)
			{
				if (m_Material == null || cullRequest.cullingGroup == null)
				{
					return 0;
				}
				return cullResult.QueryIndices(m_Handles.Length, cullRequest.cullingGroup);
			}

			private void GetDecalVolumeDataAndBound(Matrix4x4 decalToWorld, Matrix4x4 worldToView)
			{
				Vector4 vector = decalToWorld.GetColumn(0) * 0.5f;
				Vector4 vector2 = decalToWorld.GetColumn(1) * 0.5f;
				Vector4 vector3 = decalToWorld.GetColumn(2) * 0.5f;
				Vector4 column = decalToWorld.GetColumn(3);
				Vector3 vector4 = default(Vector3);
				vector4.x = vector.magnitude;
				vector4.y = vector2.magnitude;
				vector4.z = vector3.magnitude;
				Vector3 vector5 = worldToView.MultiplyVector(vector / vector4.x);
				Vector3 vector6 = worldToView.MultiplyVector(vector2 / vector4.y);
				Vector3 vector7 = worldToView.MultiplyVector(vector3 / vector4.z);
				Vector3 vector8 = worldToView.MultiplyPoint(column);
				m_Bounds[m_DecalDatasCount].center = vector8;
				m_Bounds[m_DecalDatasCount].boxAxisX = vector5 * vector4.x;
				m_Bounds[m_DecalDatasCount].boxAxisY = vector6 * vector4.y;
				m_Bounds[m_DecalDatasCount].boxAxisZ = vector7 * vector4.z;
				m_Bounds[m_DecalDatasCount].scaleXY = 1f;
				m_Bounds[m_DecalDatasCount].radius = vector4.magnitude;
				m_LightVolumes[m_DecalDatasCount].lightCategory = 3u;
				m_LightVolumes[m_DecalDatasCount].lightVolume = 2u;
				m_LightVolumes[m_DecalDatasCount].featureFlags = 32768u;
				m_LightVolumes[m_DecalDatasCount].lightPos = vector8;
				m_LightVolumes[m_DecalDatasCount].lightAxisX = vector5;
				m_LightVolumes[m_DecalDatasCount].lightAxisY = vector6;
				m_LightVolumes[m_DecalDatasCount].lightAxisZ = vector7;
				m_LightVolumes[m_DecalDatasCount].boxInnerDist = vector4 - HDRenderPipeline.k_BoxCullingExtentThreshold;
				m_LightVolumes[m_DecalDatasCount].boxInvRange.Set(1f / HDRenderPipeline.k_BoxCullingExtentThreshold.x, 1f / HDRenderPipeline.k_BoxCullingExtentThreshold.y, 1f / HDRenderPipeline.k_BoxCullingExtentThreshold.z);
			}

			private void AssignCurrentBatches(ref Matrix4x4[] decalToWorldBatch, ref Matrix4x4[] normalToWorldBatch, ref float[] decalLayerMaskBatch, int batchCount)
			{
				if (m_DecalToWorld.Count == batchCount)
				{
					decalToWorldBatch = new Matrix4x4[250];
					m_DecalToWorld.Add(decalToWorldBatch);
					normalToWorldBatch = new Matrix4x4[250];
					m_NormalToWorld.Add(normalToWorldBatch);
					decalLayerMaskBatch = new float[250];
					m_DecalLayerMasks.Add(decalLayerMaskBatch);
				}
				else
				{
					decalToWorldBatch = m_DecalToWorld[batchCount];
					normalToWorldBatch = m_NormalToWorld[batchCount];
					decalLayerMaskBatch = m_DecalLayerMasks[batchCount];
				}
			}

			public bool IsDrawn()
			{
				if (m_Material != null)
				{
					return m_NumResults > 0;
				}
				return false;
			}

			public void CreateDrawData()
			{
				int num = 0;
				int num2 = 0;
				m_InstanceCount = 0;
				Matrix4x4[] decalToWorldBatch = null;
				Matrix4x4[] normalToWorldBatch = null;
				float[] decalLayerMaskBatch = null;
				bool flag = false;
				AssignCurrentBatches(ref decalToWorldBatch, ref normalToWorldBatch, ref decalLayerMaskBatch, num2);
				NativeArray<Matrix4x4> nativeArray = m_DecalToWorlds.Reinterpret<Matrix4x4>();
				NativeArray<Matrix4x4> nativeArray2 = m_NormalToWorlds.Reinterpret<Matrix4x4>();
				Vector3 position = instance.CurrentCamera.transform.position;
				Camera currentCamera = instance.CurrentCamera;
				Matrix4x4 worldToView = HDRenderPipeline.WorldToCamera(currentCamera);
				int cullingMask = currentCamera.cullingMask;
				HDUtils.GetSceneCullingMaskFromCamera(currentCamera);
				for (int i = 0; i < m_NumResults; i++)
				{
					int num3 = m_ResultIndices[i];
					int num4 = 1 << m_CachedLayerMask[num3];
					_ = m_CachedSceneLayerMask[num3];
					bool flag2 = true;
					if (!((cullingMask & num4) != 0 && flag2))
					{
						continue;
					}
					float magnitude = (position - m_CachedBoundingSpheres[num3].position).magnitude;
					float num5 = m_CachedDrawDistances[num3].x + m_CachedBoundingSpheres[num3].radius;
					if (magnitude < num5)
					{
						decalToWorldBatch[num] = nativeArray[num3];
						normalToWorldBatch[num] = nativeArray2[num3];
						float num6 = m_CachedFadeFactor[num3] * Mathf.Clamp((num5 - magnitude) / (num5 * (1f - m_CachedDrawDistances[num3].y)), 0f, 1f);
						normalToWorldBatch[num].m03 = num6 * m_Blend;
						normalToWorldBatch[num].m13 = m_CachedAngleFade[num3].x;
						normalToWorldBatch[num].m23 = m_CachedAngleFade[num3].y;
						normalToWorldBatch[num].SetRow(3, m_CachedUVScaleBias[num3]);
						decalLayerMaskBatch[num] = (float)m_CachedDecalLayerMask[num3];
						if (m_CachedAffectsTransparency[num3])
						{
							m_DecalDatas[m_DecalDatasCount].worldToDecal = decalToWorldBatch[num].inverse;
							m_DecalDatas[m_DecalDatasCount].normalToWorld = normalToWorldBatch[num];
							m_DecalDatas[m_DecalDatasCount].baseColor = new Vector4(Mathf.GammaToLinearSpace(m_BaseColor.x), Mathf.GammaToLinearSpace(m_BaseColor.y), Mathf.GammaToLinearSpace(m_BaseColor.z), m_BaseColor.w);
							m_DecalDatas[m_DecalDatasCount].blendParams = m_BlendParams;
							m_DecalDatas[m_DecalDatasCount].remappingAOS = m_RemappingAOS;
							m_DecalDatas[m_DecalDatasCount].scalingBAndRemappingM = m_ScalingBAndRemappingM;
							m_DecalDatas[m_DecalDatasCount].decalLayerMask = (uint)m_CachedDecalLayerMask[num3];
							m_DiffuseTextureScaleBias[m_DecalDatasCount] = m_Diffuse;
							m_NormalTextureScaleBias[m_DecalDatasCount] = m_Normal;
							m_MaskTextureScaleBias[m_DecalDatasCount] = m_Mask;
							GetDecalVolumeDataAndBound(decalToWorldBatch[num], worldToView);
							m_DecalDatasCount++;
							flag = true;
						}
						num++;
						m_InstanceCount++;
						if (num == 250)
						{
							num = 0;
							num2++;
							AssignCurrentBatches(ref decalToWorldBatch, ref normalToWorldBatch, ref decalLayerMaskBatch, num2);
						}
					}
				}
				if (flag)
				{
					AddToTextureList(ref instance.m_TextureList);
				}
			}

			public void EndCull(CullRequest.Set request)
			{
				if (!(m_Material == null))
				{
					if (request.cullingGroup == null)
					{
						Debug.LogError("Begin/EndCull() called out of sequence for decal projectors.");
					}
					else
					{
						request.Clear();
					}
				}
			}

			public void AddToTextureList(ref List<TextureScaleBias> textureList)
			{
				if (m_Diffuse.m_Texture != null)
				{
					textureList.Add(m_Diffuse);
				}
				if (m_Normal.m_Texture != null)
				{
					textureList.Add(m_Normal);
				}
				if (m_Mask.m_Texture != null)
				{
					textureList.Add(m_Mask);
				}
			}

			public void RenderIntoDBuffer(CommandBuffer cmd)
			{
				if (!(m_Material == null) && m_cachedProjectorPassValue != -1 && m_NumResults != 0)
				{
					int i = 0;
					int num = m_InstanceCount;
					for (; i < m_InstanceCount / 250; i++)
					{
						m_PropertyBlock.SetMatrixArray(HDShaderIDs._NormalToWorldID, m_NormalToWorld[i]);
						m_PropertyBlock.SetFloatArray("_DecalLayerMaskFromDecal", m_DecalLayerMasks[i]);
						cmd.DrawMeshInstanced(m_DecalMesh, 0, m_Material, m_cachedProjectorPassValue, m_DecalToWorld[i], 250, m_PropertyBlock);
						num -= 250;
					}
					if (num > 0)
					{
						m_PropertyBlock.SetMatrixArray(HDShaderIDs._NormalToWorldID, m_NormalToWorld[i]);
						m_PropertyBlock.SetFloatArray("_DecalLayerMaskFromDecal", m_DecalLayerMasks[i]);
						cmd.DrawMeshInstanced(m_DecalMesh, 0, m_Material, m_cachedProjectorPassValue, m_DecalToWorld[i], num, m_PropertyBlock);
					}
				}
			}

			public void RenderForwardEmissive(CommandBuffer cmd)
			{
				if (!(m_Material == null) && m_cachedProjectorEmissivePassValue != -1 && m_NumResults != 0)
				{
					int i = 0;
					int num = m_InstanceCount;
					for (; i < m_InstanceCount / 250; i++)
					{
						m_PropertyBlock.SetMatrixArray(HDShaderIDs._NormalToWorldID, m_NormalToWorld[i]);
						m_PropertyBlock.SetFloatArray("_DecalLayerMaskFromDecal", m_DecalLayerMasks[i]);
						cmd.DrawMeshInstanced(m_DecalMesh, 0, m_Material, m_cachedProjectorEmissivePassValue, m_DecalToWorld[i], 250, m_PropertyBlock);
						num -= 250;
					}
					if (num > 0)
					{
						m_PropertyBlock.SetMatrixArray(HDShaderIDs._NormalToWorldID, m_NormalToWorld[i]);
						m_PropertyBlock.SetFloatArray("_DecalLayerMaskFromDecal", m_DecalLayerMasks[i]);
						cmd.DrawMeshInstanced(m_DecalMesh, 0, m_Material, m_cachedProjectorEmissivePassValue, m_DecalToWorld[i], num, m_PropertyBlock);
					}
				}
			}

			internal void SetCullResult(CullResult.Set value)
			{
				m_NumResults = value.numResults;
				if (m_ResultIndices.Length < m_NumResults)
				{
					Array.Resize(ref m_ResultIndices, m_NumResults);
				}
				Array.Copy(value.resultIndices, m_ResultIndices, m_NumResults);
			}

			public void ResolveUpdateJob()
			{
				m_UpdateJobHandle.Complete();
				m_BoundingSpheres.CopyTo(m_CachedBoundingSpheres);
			}

			private void ResizeJobArrays(int newCapacity)
			{
				m_CachedTransforms.ResizeArray(newCapacity);
				ArrayExtensions.ResizeArray(ref m_Positions, newCapacity);
				ArrayExtensions.ResizeArray(ref m_Rotations, newCapacity);
				ArrayExtensions.ResizeArray(ref m_Scales, newCapacity);
				ArrayExtensions.ResizeArray(ref m_Sizes, newCapacity);
				ArrayExtensions.ResizeArray(ref m_Offsets, newCapacity);
				ArrayExtensions.ResizeArray(ref m_ResolvedRotations, newCapacity);
				ArrayExtensions.ResizeArray(ref m_ResolvedScales, newCapacity);
				ArrayExtensions.ResizeArray(ref m_ResolvedSizeOffsets, newCapacity);
				ArrayExtensions.ResizeArray(ref m_ScaleModes, newCapacity);
				ArrayExtensions.ResizeArray(ref m_NormalToWorlds, newCapacity);
				ArrayExtensions.ResizeArray(ref m_DecalToWorlds, newCapacity);
				ArrayExtensions.ResizeArray(ref m_BoundingSpheres, newCapacity);
				ArrayExtensions.ResizeArray(ref m_Dirty, newCapacity);
				ArrayExtensions.ResizeArray(ref m_CachedBoundingSpheres, newCapacity);
			}

			private void UpdateJobArrays(int index, DecalProjector decalProjector)
			{
				if (index == m_CachedTransforms.length)
				{
					m_CachedTransforms.Add(decalProjector.transform);
				}
				else
				{
					m_CachedTransforms[index] = decalProjector.transform;
				}
				m_Positions[index] = decalProjector.transform.position;
				m_Rotations[index] = decalProjector.transform.rotation;
				m_Scales[index] = decalProjector.transform.lossyScale;
				m_Sizes[index] = decalProjector.size;
				m_Offsets[index] = decalProjector.pivot;
				m_ScaleModes[index] = decalProjector.scaleMode;
				m_Dirty[index] = true;
			}

			private void RemoveFromJobArrays(int removeAtIndex)
			{
				m_CachedTransforms.RemoveAtSwapBack(removeAtIndex);
				m_Positions[removeAtIndex] = m_Positions[m_DecalsCount - 1];
				m_Rotations[removeAtIndex] = m_Rotations[m_DecalsCount - 1];
				m_Scales[removeAtIndex] = m_Scales[m_DecalsCount - 1];
				m_Sizes[removeAtIndex] = m_Sizes[m_DecalsCount - 1];
				m_Offsets[removeAtIndex] = m_Offsets[m_DecalsCount - 1];
				m_ResolvedRotations[removeAtIndex] = m_ResolvedRotations[m_DecalsCount - 1];
				m_ResolvedScales[removeAtIndex] = m_ResolvedScales[m_DecalsCount - 1];
				m_ResolvedSizeOffsets[removeAtIndex] = m_ResolvedSizeOffsets[m_DecalsCount - 1];
				m_ScaleModes[removeAtIndex] = m_ScaleModes[m_DecalsCount - 1];
				m_NormalToWorlds[removeAtIndex] = m_NormalToWorlds[m_DecalsCount - 1];
				m_DecalToWorlds[removeAtIndex] = m_DecalToWorlds[m_DecalsCount - 1];
				m_BoundingSpheres[removeAtIndex] = m_BoundingSpheres[m_DecalsCount - 1];
				m_Dirty[removeAtIndex] = m_Dirty[m_DecalsCount - 1];
				m_CachedBoundingSpheres[removeAtIndex] = m_CachedBoundingSpheres[m_DecalsCount - 1];
			}

			private void DisposeJobArrays()
			{
				m_CachedTransforms.Dispose();
				m_Positions.Dispose();
				m_Rotations.Dispose();
				m_Scales.Dispose();
				m_Sizes.Dispose();
				m_Offsets.Dispose();
				m_ResolvedRotations.Dispose();
				m_ResolvedScales.Dispose();
				m_ResolvedSizeOffsets.Dispose();
				m_ScaleModes.Dispose();
				m_NormalToWorlds.Dispose();
				m_DecalToWorlds.Dispose();
				m_BoundingSpheres.Dispose();
				m_Dirty.Dispose();
				m_CachedBoundingSpheres = null;
			}

			internal void StartUpdateJob()
			{
				m_UpdateJobHandle.Complete();
				UpdateJob updateJob = default(UpdateJob);
				updateJob.positions = m_Positions;
				updateJob.rawRotations = m_Rotations;
				updateJob.rawScales = m_Scales;
				updateJob.resolvedScales = m_ResolvedScales;
				updateJob.resolvedRotations = m_ResolvedRotations;
				updateJob.resolvedSizesOffsets = m_ResolvedSizeOffsets;
				updateJob.dirty = m_Dirty;
				updateJob.rawSizes = m_Sizes;
				updateJob.rawOffsets = m_Offsets;
				updateJob.scaleModes = m_ScaleModes;
				updateJob.normalToWorlds = m_NormalToWorlds;
				updateJob.decalToWorlds = m_DecalToWorlds;
				updateJob.boundingSpheres = m_BoundingSpheres;
				updateJob.minDistance = float.Epsilon;
				UpdateJob jobData = updateJob;
				m_UpdateJobHandle = jobData.Schedule(m_CachedTransforms);
			}
		}

		[BurstCompile]
		internal struct UpdateJob : IJobParallelForTransform
		{
			private static readonly quaternion k_MinusYtoZRotation = quaternion.EulerXYZ(-MathF.PI / 2f, 0f, 0f);

			private static readonly quaternion k_YtoZRotation = quaternion.EulerXYZ(MathF.PI / 2f, 0f, 0f);

			private static readonly float3 sFloat3One = new float3(1f, 1f, 1f);

			public float minDistance;

			public NativeArray<float3> positions;

			public NativeArray<quaternion> rawRotations;

			public NativeArray<float3> rawScales;

			public NativeArray<float3> resolvedScales;

			public NativeArray<quaternion> resolvedRotations;

			public NativeArray<float4x4> resolvedSizesOffsets;

			public NativeArray<bool> dirty;

			[ReadOnly]
			public NativeArray<float3> rawSizes;

			[ReadOnly]
			public NativeArray<float3> rawOffsets;

			[ReadOnly]
			public NativeArray<DecalScaleMode> scaleModes;

			[WriteOnly]
			public NativeArray<float4x4> normalToWorlds;

			[WriteOnly]
			public NativeArray<float4x4> decalToWorlds;

			[WriteOnly]
			public NativeArray<BoundingSphere> boundingSpheres;

			private float DistanceBetweenQuaternions(quaternion a, quaternion b)
			{
				return math.distancesq(a.value, b.value);
			}

			private float3 effectiveScale(int index, in TransformAccess transform)
			{
				if (scaleModes[index] != DecalScaleMode.InheritFromHierarchy)
				{
					return sFloat3One;
				}
				return transform.localToWorldMatrix.lossyScale;
			}

			private float3 resolveDecalSize(int index, float3 scale, in TransformAccess transform)
			{
				if (scale.z < 0f)
				{
					scale.y *= -1f;
				}
				if ((scale.x < 0f) ^ (scale.y < 0f) ^ (scale.z < 0f))
				{
					scale.z *= -1f;
				}
				float3 @float = rawSizes[index];
				return new float3(@float.x * scale.x, @float.z * scale.z, @float.y * scale.y);
			}

			private float3 resolveDecalOffset(int index, float3 scale, in TransformAccess transform)
			{
				if (scale.z < 0f)
				{
					scale.y *= -1f;
					scale.z *= -1f;
				}
				float3 @float = rawOffsets[index];
				return new float3(@float.x * scale.x, (0f - @float.z) * scale.z, @float.y * scale.y);
			}

			private quaternion resolveRotation(int index, in float3 scale, in TransformAccess transform)
			{
				return transform.rotation * ((scale.z >= 0f) ? k_MinusYtoZRotation : k_YtoZRotation);
			}

			public void Execute(int index, TransformAccess transform)
			{
				bool flag = dirty[index];
				bool num = math.distancesq(transform.position, positions[index]) > minDistance;
				if (num)
				{
					positions[index] = transform.position;
				}
				bool flag2 = math.distancesq(transform.localToWorldMatrix.lossyScale, rawScales[index]) > minDistance;
				if (flag2)
				{
					rawScales[index] = transform.localToWorldMatrix.lossyScale;
				}
				if (flag2 || flag)
				{
					resolvedScales[index] = effectiveScale(index, in transform);
				}
				bool flag3 = DistanceBetweenQuaternions(transform.rotation, rawRotations[index]) > minDistance;
				if (flag3)
				{
					rawRotations[index] = transform.rotation;
				}
				if (flag3 || flag)
				{
					ref NativeArray<quaternion> reference = ref resolvedRotations;
					float3 scale = resolvedScales[index];
					reference[index] = resolveRotation(index, in scale, in transform);
				}
				if (num || flag3 || flag2 || flag)
				{
					if (flag || flag3 || flag2)
					{
						resolvedSizesOffsets[index] = math.mul(float4x4.Translate(resolveDecalOffset(index, resolvedScales[index], in transform)), float4x4.Scale(resolveDecalSize(index, resolvedScales[index], in transform)));
					}
					float4x4 value;
					float4x4 a = (value = float4x4.TRS(transform.position, resolvedRotations[index], sFloat3One));
					float4 c = value.c1;
					value.c1 = value.c2;
					value.c2 = c;
					normalToWorlds[index] = value;
					float4x4 b = resolvedSizesOffsets[index];
					float4x4 float4x = math.mul(a, b);
					decalToWorlds[index] = float4x;
					boundingSpheres[index] = GetDecalProjectBoundingSphere(float4x);
					dirty[index] = false;
				}
			}

			private BoundingSphere GetDecalProjectBoundingSphere(Matrix4x4 decalToWorld)
			{
				float4 b = new float4(-0.5f, -0.5f, -0.5f, 1f);
				float4 b2 = new float4(0.5f, 0.5f, 0.5f, 1f);
				b = math.mul(decalToWorld, b);
				b2 = math.mul(decalToWorld, b2);
				float3 xyz = ((b2 + b) / 2f).xyz;
				float radius = math.length(b2 - b) / 2f;
				BoundingSphere result = default(BoundingSphere);
				result.position = xyz;
				result.radius = radius;
				return result;
			}
		}

		public static readonly string[] s_MaterialDecalPassNames = Enum.GetNames(typeof(MaterialDecalPass));

		public static readonly string s_AtlasSizeWarningMessage = "Decal texture atlas out of space, decals on transparent geometry might not render correctly, atlas size can be changed in HDRenderPipelineAsset";

		public const int kInvalidIndex = -1;

		public const int kNullMaterialIndex = int.MaxValue;

		private static DecalSystem m_Instance;

		private const int kDefaultDrawDistance = 1000;

		private const int kDecalBlockSize = 128;

		private const int kDrawIndexedBatchSize = 250;

		private static Vector4 kMin = new Vector4(-0.5f, -0.5f, -0.5f, 1f);

		private static Vector4 kMax = new Vector4(0.5f, 0.5f, 0.5f, 1f);

		public static Mesh m_DecalMesh = null;

		public static DecalData[] m_DecalDatas = new DecalData[128];

		public static SFiniteLightBound[] m_Bounds = new SFiniteLightBound[128];

		public static LightVolumeData[] m_LightVolumes = new LightVolumeData[128];

		public static TextureScaleBias[] m_DiffuseTextureScaleBias = new TextureScaleBias[128];

		public static TextureScaleBias[] m_NormalTextureScaleBias = new TextureScaleBias[128];

		public static TextureScaleBias[] m_MaskTextureScaleBias = new TextureScaleBias[128];

		public static Vector4[] m_BaseColor = new Vector4[128];

		public static int m_DecalDatasCount = 0;

		public static float[] m_BoundingDistances = new float[1];

		private Dictionary<int, DecalSet> m_DecalSets = new Dictionary<int, DecalSet>();

		private List<DecalSet> m_DecalSetsRenderList = new List<DecalSet>();

		private Camera m_Camera;

		public static int m_DecalsVisibleThisFrame = 0;

		private Texture2DAtlas m_Atlas;

		public bool m_AllocationSuccess = true;

		public bool m_PrevAllocationSuccess = true;

		private List<TextureScaleBias> m_TextureList = new List<TextureScaleBias>();

		private const string kIdentifyHDRPDecal = "_Unity_Identify_HDRP_Decal";

		public static DecalSystem instance
		{
			get
			{
				if (m_Instance == null)
				{
					m_Instance = new DecalSystem();
				}
				return m_Instance;
			}
		}

		public int DrawDistance
		{
			get
			{
				HDRenderPipelineAsset currentAsset = HDRenderPipeline.currentAsset;
				if (currentAsset != null)
				{
					return currentAsset.currentPlatformRenderPipelineSettings.decalSettings.drawDistance;
				}
				return 1000;
			}
		}

		public bool perChannelMask
		{
			get
			{
				HDRenderPipelineAsset currentAsset = HDRenderPipeline.currentAsset;
				if (currentAsset != null)
				{
					return currentAsset.currentPlatformRenderPipelineSettings.decalSettings.perChannelMask;
				}
				return false;
			}
		}

		public Camera CurrentCamera
		{
			get
			{
				return m_Camera;
			}
			set
			{
				m_Camera = value;
			}
		}

		public Texture2DAtlas Atlas
		{
			get
			{
				if (m_Atlas == null)
				{
					m_Atlas = new Texture2DAtlas(HDUtils.hdrpSettings.decalSettings.atlasWidth, HDUtils.hdrpSettings.decalSettings.atlasHeight, GraphicsFormat.R8G8B8A8_UNorm);
				}
				return m_Atlas;
			}
		}

		public static bool IsHDRenderPipelineDecal(Shader shader)
		{
			return shader.name == "HDRP/Decal";
		}

		public static bool IsHDRenderPipelineDecal(Material material)
		{
			return material.HasProperty("_Unity_Identify_HDRP_Decal");
		}

		public static bool IsDecalMaterial(Material material)
		{
			string[] array = s_MaterialDecalPassNames;
			foreach (string passName in array)
			{
				if (material.FindPass(passName) != -1)
				{
					return true;
				}
			}
			return false;
		}

		private void SetupMipStreamingSettings(Texture texture, bool allMips)
		{
			if (!texture || texture.dimension != TextureDimension.Tex2D)
			{
				return;
			}
			Texture2D texture2D = texture as Texture2D;
			if ((bool)texture2D)
			{
				if (allMips)
				{
					texture2D.requestedMipmapLevel = 0;
				}
				else
				{
					texture2D.ClearRequestedMipmapLevel();
				}
			}
		}

		private void SetupMipStreamingSettings(Material material, bool allMips)
		{
			if (material != null && IsHDRenderPipelineDecal(material.shader))
			{
				SetupMipStreamingSettings(material.GetTexture("_BaseColorMap"), allMips);
				SetupMipStreamingSettings(material.GetTexture("_NormalMap"), allMips);
				SetupMipStreamingSettings(material.GetTexture("_MaskMap"), allMips);
			}
		}

		public DecalHandle AddDecal(DecalProjector decalProjector)
		{
			Material material = decalProjector.material;
			SetupMipStreamingSettings(material, allMips: true);
			DecalSet value = null;
			int num = ((material != null) ? material.GetInstanceID() : int.MaxValue);
			if (!m_DecalSets.TryGetValue(num, out value))
			{
				value = new DecalSet(material);
				m_DecalSets.Add(num, value);
			}
			return value.AddDecal(num, decalProjector);
		}

		public void RemoveDecal(DecalHandle handle)
		{
			if (!DecalHandle.IsValid(handle))
			{
				return;
			}
			DecalSet value = null;
			int materialID = handle.m_MaterialID;
			if (m_DecalSets.TryGetValue(materialID, out value))
			{
				value.RemoveDecal(handle);
				if (value.Count == 0)
				{
					SetupMipStreamingSettings(value.KeyMaterial, allMips: false);
					value.Dispose();
					m_DecalSets.Remove(materialID);
				}
			}
		}

		public void UpdateCachedData(DecalHandle handle, DecalProjector decalProjector)
		{
			if (DecalHandle.IsValid(handle))
			{
				DecalSet value = null;
				int materialID = handle.m_MaterialID;
				if (m_DecalSets.TryGetValue(materialID, out value))
				{
					value.UpdateCachedData(handle, decalProjector);
				}
			}
		}

		public void BeginCull(CullRequest request)
		{
			request.Clear();
			foreach (KeyValuePair<int, DecalSet> decalSet in m_DecalSets)
			{
				decalSet.Value.BeginCull(request[decalSet.Key]);
			}
		}

		private int QueryCullResults(CullRequest decalCullRequest, CullResult cullResults)
		{
			int num = 0;
			foreach (KeyValuePair<int, DecalSet> decalSet in m_DecalSets)
			{
				num += decalSet.Value.QueryCullResults(decalCullRequest[decalSet.Key], cullResults[decalSet.Key]);
			}
			return num;
		}

		public void EndCull(CullRequest cullRequest, CullResult cullResults)
		{
			m_DecalsVisibleThisFrame = QueryCullResults(cullRequest, cullResults);
			foreach (KeyValuePair<int, DecalSet> decalSet in m_DecalSets)
			{
				decalSet.Value.EndCull(cullRequest[decalSet.Key]);
			}
		}

		public bool HasAnyForwardEmissive()
		{
			foreach (DecalSet decalSetsRender in m_DecalSetsRenderList)
			{
				if (decalSetsRender.HasEmissivePass)
				{
					return true;
				}
			}
			return false;
		}

		public void RenderIntoDBuffer(CommandBuffer cmd)
		{
			if (m_DecalMesh == null)
			{
				m_DecalMesh = CoreUtils.CreateCubeMesh(kMin, kMax);
			}
			foreach (DecalSet decalSetsRender in m_DecalSetsRenderList)
			{
				decalSetsRender.RenderIntoDBuffer(cmd);
			}
		}

		public void RenderForwardEmissive(CommandBuffer cmd)
		{
			if (m_DecalMesh == null)
			{
				m_DecalMesh = CoreUtils.CreateCubeMesh(kMin, kMax);
			}
			foreach (DecalSet decalSetsRender in m_DecalSetsRenderList)
			{
				decalSetsRender.RenderForwardEmissive(cmd);
			}
		}

		public void SetAtlas(CommandBuffer cmd)
		{
			cmd.SetGlobalTexture(HDShaderIDs._DecalAtlas2DID, Atlas.AtlasTexture);
		}

		public void AddTexture(CommandBuffer cmd, TextureScaleBias textureScaleBias)
		{
			if (textureScaleBias.m_Texture != null)
			{
				if (Atlas.IsCached(out textureScaleBias.m_ScaleBias, textureScaleBias.m_Texture))
				{
					Atlas.UpdateTexture(cmd, textureScaleBias.m_Texture, ref textureScaleBias.m_ScaleBias);
				}
				else if (!Atlas.AddTexture(cmd, ref textureScaleBias.m_ScaleBias, textureScaleBias.m_Texture))
				{
					m_AllocationSuccess = false;
				}
			}
			else
			{
				textureScaleBias.m_ScaleBias = Vector4.zero;
			}
		}

		public void UpdateCachedMaterialData()
		{
			m_TextureList.Clear();
			foreach (KeyValuePair<int, DecalSet> decalSet in m_DecalSets)
			{
				decalSet.Value.InitializeMaterialValues();
			}
		}

		private void UpdateDecalDatasWithAtlasInfo()
		{
			for (int i = 0; i < m_DecalDatasCount; i++)
			{
				m_DecalDatas[i].diffuseScaleBias = m_DiffuseTextureScaleBias[i].m_ScaleBias;
				m_DecalDatas[i].normalScaleBias = m_NormalTextureScaleBias[i].m_ScaleBias;
				m_DecalDatas[i].maskScaleBias = m_MaskTextureScaleBias[i].m_ScaleBias;
			}
		}

		public void UpdateTextureAtlas(CommandBuffer cmd)
		{
			m_AllocationSuccess = true;
			foreach (TextureScaleBias texture in m_TextureList)
			{
				AddTexture(cmd, texture);
			}
			if (!m_AllocationSuccess)
			{
				m_TextureList.Sort();
				Atlas.ResetAllocator();
				m_AllocationSuccess = true;
				foreach (TextureScaleBias texture2 in m_TextureList)
				{
					AddTexture(cmd, texture2);
				}
				if (!m_AllocationSuccess && m_PrevAllocationSuccess)
				{
					Debug.LogWarning(s_AtlasSizeWarningMessage);
				}
			}
			m_PrevAllocationSuccess = m_AllocationSuccess;
			UpdateDecalDatasWithAtlasInfo();
		}

		public void CreateDrawData()
		{
			m_DecalDatasCount = 0;
			if (m_DecalsVisibleThisFrame > m_DecalDatas.Length)
			{
				int num = (m_DecalsVisibleThisFrame + 128 - 1) / 128 * 128;
				m_DecalDatas = new DecalData[num];
				m_Bounds = new SFiniteLightBound[num];
				m_LightVolumes = new LightVolumeData[num];
				m_DiffuseTextureScaleBias = new TextureScaleBias[num];
				m_NormalTextureScaleBias = new TextureScaleBias[num];
				m_MaskTextureScaleBias = new TextureScaleBias[num];
				m_BaseColor = new Vector4[num];
			}
			m_DecalSetsRenderList.Clear();
			foreach (KeyValuePair<int, DecalSet> decalSet in m_DecalSets)
			{
				decalSet.Value.UpdateCachedDrawOrder();
				if (decalSet.Value.IsDrawn())
				{
					int i;
					for (i = 0; i < m_DecalSetsRenderList.Count && decalSet.Value.DrawOrder > m_DecalSetsRenderList[i].DrawOrder; i++)
					{
					}
					m_DecalSetsRenderList.Insert(i, decalSet.Value);
				}
			}
			foreach (DecalSet decalSetsRender in m_DecalSetsRenderList)
			{
				decalSetsRender.CreateDrawData();
			}
		}

		public void Cleanup()
		{
			if (m_Atlas != null)
			{
				m_Atlas.Release();
			}
			CoreUtils.Destroy(m_DecalMesh);
			m_DecalMesh = null;
			m_Atlas = null;
		}

		public void RenderDebugOverlay(HDCamera hdCamera, CommandBuffer cmd, int mipLevel, DebugOverlay debugOverlay)
		{
			cmd.SetViewport(debugOverlay.Next());
			HDUtils.BlitQuad(cmd, Atlas.AtlasTexture, new Vector4(1f, 1f, 0f, 0f), new Vector4(1f, 1f, 0f, 0f), mipLevel, bilinear: true);
		}

		public void LoadCullResults(CullResult cullResult)
		{
			using Dictionary<int, CullResult.Set>.Enumerator enumerator = cullResult.requests.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (m_DecalSets.TryGetValue(enumerator.Current.Key, out var value))
				{
					value.SetCullResult(cullResult.requests[enumerator.Current.Key]);
				}
			}
		}

		public bool IsAtlasAllocatedSuccessfully()
		{
			return m_AllocationSuccess;
		}

		public void StartDecalUpdateJobs()
		{
			Dictionary<int, DecalSet>.Enumerator enumerator = m_DecalSets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DecalSet value = enumerator.Current.Value;
				if (value.Count != 0)
				{
					value.updateJobHandle.Complete();
					value.StartUpdateJob();
				}
			}
		}
	}
}
