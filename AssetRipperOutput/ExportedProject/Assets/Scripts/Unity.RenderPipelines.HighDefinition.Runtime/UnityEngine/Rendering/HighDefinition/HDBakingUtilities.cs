using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class HDBakingUtilities
	{
		public enum SceneObjectCategory
		{
			ReflectionProbe = 0
		}

		private const string k_HDProbeAssetFormat = "{0}-{1}.exr";

		private static readonly Regex k_HDProbeAssetRegex = new Regex("(?<type>ReflectionProbe|PlanarProbe)-(?<index>\\d+)\\.exr");

		public static string HDProbeAssetPattern(ProbeSettings.ProbeType type)
		{
			return $"{type}-*.exr";
		}

		public static string GetBakedTextureDirectory(Scene scene)
		{
			string path = scene.path;
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			return Path.Combine(Path.GetDirectoryName(path), fileNameWithoutExtension);
		}

		public static string GetBakedTextureFilePath(HDProbe probe)
		{
			return GetBakedTextureFilePath(probe.settings.type, SceneObjectIDMap.GetOrCreateSceneObjectID(probe.gameObject, SceneObjectCategory.ReflectionProbe), probe.gameObject.scene);
		}

		public static bool TryParseBakedProbeAssetFileName(string filename, out ProbeSettings.ProbeType type, out int index)
		{
			Match match = k_HDProbeAssetRegex.Match(filename);
			if (!match.Success)
			{
				type = ProbeSettings.ProbeType.ReflectionProbe;
				index = 0;
				return false;
			}
			type = (ProbeSettings.ProbeType)Enum.Parse(typeof(ProbeSettings.ProbeType), match.Groups["type"].Value);
			index = int.Parse(match.Groups["index"].Value);
			return true;
		}

		public static string GetBakedTextureFilePath(ProbeSettings.ProbeType probeType, int index, Scene scene)
		{
			return Path.Combine(GetBakedTextureDirectory(scene), $"{probeType}-{index}.exr");
		}

		public static void CreateParentDirectoryIfMissing(string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			if (!fileInfo.Directory.Exists)
			{
				fileInfo.Directory.Create();
			}
		}

		public static bool TrySerializeToDisk<T>(T renderData, string filePath)
		{
			CreateParentDirectoryIfMissing(filePath);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(filePath, FileMode.Create);
				xmlSerializer.Serialize(fileStream, renderData);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return false;
			}
			finally
			{
				fileStream?.Dispose();
			}
			return true;
		}

		public static bool TryDeserializeFromDisk<T>(string filePath, out T renderData)
		{
			if (!File.Exists(filePath))
			{
				renderData = default(T);
				return false;
			}
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(filePath, FileMode.Open);
				renderData = (T)xmlSerializer.Deserialize(fileStream);
				return true;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				renderData = default(T);
				return false;
			}
		}
	}
}
