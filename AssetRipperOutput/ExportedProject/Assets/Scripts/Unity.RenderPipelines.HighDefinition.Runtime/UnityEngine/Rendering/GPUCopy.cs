namespace UnityEngine.Rendering
{
	internal class GPUCopy
	{
		private ComputeShader m_Shader;

		private int k_SampleKernel_xyzw2x_8;

		private int k_SampleKernel_xyzw2x_1;

		private static readonly int _RectOffset = Shader.PropertyToID("_RectOffset");

		private static readonly int _Result1 = Shader.PropertyToID("_Result1");

		private static readonly int _Source4 = Shader.PropertyToID("_Source4");

		private static int[] _IntParams = new int[2];

		public GPUCopy(ComputeShader shader)
		{
			m_Shader = shader;
			k_SampleKernel_xyzw2x_8 = m_Shader.FindKernel("KSampleCopy4_1_x_8");
			k_SampleKernel_xyzw2x_1 = m_Shader.FindKernel("KSampleCopy4_1_x_1");
		}

		private unsafe void SampleCopyChannel(CommandBuffer cmd, RectInt rect, int _source, RenderTargetIdentifier source, int _target, RenderTargetIdentifier target, int slices, int kernel8, int kernel1)
		{
			RectInt* ptr = stackalloc RectInt[3];
			int num = 0;
			RectInt rectInt = new RectInt(0, 0, 0, 0);
			if (TileLayoutUtils.TryLayoutByTiles(rect, 8u, out var main, out var topRow, out var rightCol, out var topRight))
			{
				if (topRow.width > 0 && topRow.height > 0)
				{
					ptr[num] = topRow;
					num++;
				}
				if (rightCol.width > 0 && rightCol.height > 0)
				{
					ptr[num] = rightCol;
					num++;
				}
				if (topRight.width > 0 && topRight.height > 0)
				{
					ptr[num] = topRight;
					num++;
				}
				rectInt = main;
			}
			else if (rect.width > 0 && rect.height > 0)
			{
				ptr[num] = rect;
				num++;
			}
			cmd.SetComputeTextureParam(m_Shader, kernel8, _source, source);
			cmd.SetComputeTextureParam(m_Shader, kernel1, _source, source);
			cmd.SetComputeTextureParam(m_Shader, kernel8, _target, target);
			cmd.SetComputeTextureParam(m_Shader, kernel1, _target, target);
			if (rectInt.width > 0 && rectInt.height > 0)
			{
				RectInt rectInt2 = rectInt;
				_IntParams[0] = rectInt2.x;
				_IntParams[1] = rectInt2.y;
				cmd.SetComputeIntParams(m_Shader, _RectOffset, _IntParams);
				cmd.DispatchCompute(m_Shader, kernel8, Mathf.Max(rectInt2.width / 8, 1), Mathf.Max(rectInt2.height / 8, 1), slices);
			}
			int i = 0;
			for (int num2 = num; i < num2; i++)
			{
				RectInt rectInt3 = ptr[i];
				_IntParams[0] = rectInt3.x;
				_IntParams[1] = rectInt3.y;
				cmd.SetComputeIntParams(m_Shader, _RectOffset, _IntParams);
				cmd.DispatchCompute(m_Shader, kernel1, Mathf.Max(rectInt3.width, 1), Mathf.Max(rectInt3.height, 1), slices);
			}
		}

		public void SampleCopyChannel_xyzw2x(CommandBuffer cmd, RTHandle source, RTHandle target, RectInt rect)
		{
			SampleCopyChannel(cmd, rect, _Source4, source, _Result1, target, source.rt.volumeDepth, k_SampleKernel_xyzw2x_8, k_SampleKernel_xyzw2x_1);
		}
	}
}
