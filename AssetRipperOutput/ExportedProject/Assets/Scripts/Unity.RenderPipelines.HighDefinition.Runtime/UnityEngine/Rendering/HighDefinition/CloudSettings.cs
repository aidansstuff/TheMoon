using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	public abstract class CloudSettings : VolumeComponent
	{
		private static Dictionary<Type, int> cloudUniqueIDs = new Dictionary<Type, int>();

		public virtual int GetHashCode(Camera camera)
		{
			return GetHashCode();
		}

		public static int GetUniqueID<T>()
		{
			return GetUniqueID(typeof(T));
		}

		public static int GetUniqueID(Type type)
		{
			if (!cloudUniqueIDs.TryGetValue(type, out var value))
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(CloudUniqueID), inherit: false);
				value = ((customAttributes.Length == 0) ? (-1) : ((CloudUniqueID)customAttributes[0]).uniqueID);
				cloudUniqueIDs[type] = value;
			}
			return value;
		}

		public abstract Type GetCloudRendererType();
	}
}
