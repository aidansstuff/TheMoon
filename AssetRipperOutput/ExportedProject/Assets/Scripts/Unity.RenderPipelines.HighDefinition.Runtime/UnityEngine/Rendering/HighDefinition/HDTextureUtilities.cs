using System;
using System.IO;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class HDTextureUtilities
	{
		public static void WriteTextureFileToDisk(Texture target, string filePath)
		{
			RenderTexture renderTexture = target as RenderTexture;
			Cubemap cubemap = target as Cubemap;
			if (renderTexture != null)
			{
				byte[] bytes = CopyRenderTextureToTexture2D(renderTexture).EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
				HDBakingUtilities.CreateParentDirectoryIfMissing(filePath);
				File.WriteAllBytes(filePath, bytes);
				return;
			}
			if (cubemap != null)
			{
				Texture2D texture2D = new Texture2D(cubemap.width * 6, cubemap.height, GraphicsFormat.R16G16B16A16_SFloat, TextureCreationFlags.None);
				CommandBuffer commandBuffer = new CommandBuffer
				{
					name = "CopyCubemapToTexture2D"
				};
				for (int i = 0; i < 6; i++)
				{
					commandBuffer.CopyTexture(cubemap, i, 0, 0, 0, cubemap.width, cubemap.height, texture2D, 0, 0, cubemap.width * i, 0);
				}
				Graphics.ExecuteCommandBuffer(commandBuffer);
				byte[] bytes2 = texture2D.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
				HDBakingUtilities.CreateParentDirectoryIfMissing(filePath);
				File.WriteAllBytes(filePath, bytes2);
				return;
			}
			throw new ArgumentException();
		}

		public static Texture2D CopyRenderTextureToTexture2D(RenderTexture source)
		{
			GraphicsFormat graphicsFormat = source.graphicsFormat;
			switch (source.dimension)
			{
			case TextureDimension.Cube:
			{
				int width2 = source.width;
				RenderTexture temporary = RenderTexture.GetTemporary(width2 * 6, width2, 0, source.format);
				CommandBuffer commandBuffer = new CommandBuffer();
				for (int i = 0; i < 6; i++)
				{
					commandBuffer.CopyTexture(source, i, 0, 0, 0, width2, width2, temporary, 0, 0, i * width2, 0);
				}
				Graphics.ExecuteCommandBuffer(commandBuffer);
				Texture2D texture2D2 = new Texture2D(width2 * 6, width2, graphicsFormat, TextureCreationFlags.None);
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = temporary;
				texture2D2.ReadPixels(new Rect(0f, 0f, 6 * width2, width2), 0, 0, recalculateMipMaps: false);
				RenderTexture.active = active;
				RenderTexture.ReleaseTemporary(temporary);
				return texture2D2;
			}
			case TextureDimension.Tex2D:
			{
				int width = source.width;
				Texture2D texture2D = new Texture2D(width, width, graphicsFormat, TextureCreationFlags.None);
				Graphics.SetRenderTarget(source, 0);
				texture2D.ReadPixels(new Rect(0f, 0f, width, width), 0, 0);
				texture2D.Apply();
				Graphics.SetRenderTarget(null);
				return texture2D;
			}
			default:
				throw new ArgumentException();
			}
		}
	}
}
