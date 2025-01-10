using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Http
{
	[Preserve]
	[JsonConverter(typeof(JsonObjectConverter))]
	internal class JsonObject : IDeserializable
	{
		[Preserve]
		internal object obj;

		[Preserve]
		internal JsonObject(object obj)
		{
			this.obj = obj;
		}

		public string GetAsString()
		{
			try
			{
				if (obj == null)
				{
					return "";
				}
				if (obj.GetType() == typeof(string))
				{
					return obj.ToString();
				}
				return JsonConvert.SerializeObject(obj);
			}
			catch (Exception)
			{
				throw new InvalidOperationException("Failed to convert JsonObject to string.");
			}
		}

		public T GetAs<T>(DeserializationSettings deserializationSettings = null)
		{
			deserializationSettings = deserializationSettings ?? new DeserializationSettings();
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				MissingMemberHandling = ((deserializationSettings.MissingMemberHandling == MissingMemberHandling.Error) ? Newtonsoft.Json.MissingMemberHandling.Error : Newtonsoft.Json.MissingMemberHandling.Ignore)
			};
			try
			{
				T val = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj), settings);
				List<string> list = ValidateObject(val);
				if (list.Count > 0)
				{
					throw new DeserializationException(string.Join("\n", list));
				}
				return val;
			}
			catch (DeserializationException)
			{
				throw;
			}
			catch (JsonSerializationException ex2)
			{
				throw new DeserializationException(ex2.Message);
			}
			catch (Exception)
			{
				throw new DeserializationException("Unable to deserialize object.");
			}
		}

		public T GetAs<T>()
		{
			return GetAs<T>(null);
		}

		public static IDeserializable GetNewJsonObjectResponse(object o)
		{
			return new JsonObject(o);
		}

		public static List<IDeserializable> GetNewJsonObjectResponse(List<object> o)
		{
			return o?.Select((Func<object, IDeserializable>)((object v) => new JsonObject(v))).ToList();
		}

		public static List<List<IDeserializable>> GetNewJsonObjectResponse(List<List<object>> o)
		{
			return o?.Select((List<object> l) => ((IEnumerable<object>)l).Select((Func<object, IDeserializable>)((object v) => (v != null) ? new JsonObject(v) : null)).ToList()).ToList();
		}

		public static Dictionary<string, IDeserializable> GetNewJsonObjectResponse(Dictionary<string, object> o)
		{
			return o?.ToDictionary((Func<KeyValuePair<string, object>, string>)((KeyValuePair<string, object> kv) => kv.Key), (Func<KeyValuePair<string, object>, IDeserializable>)((KeyValuePair<string, object> kv) => new JsonObject(kv.Value)));
		}

		public static Dictionary<string, List<IDeserializable>> GetNewJsonObjectResponse(Dictionary<string, List<object>> o)
		{
			return o?.ToDictionary((KeyValuePair<string, List<object>> kv) => kv.Key, (KeyValuePair<string, List<object>> kv) => GetNewJsonObjectResponse(kv.Value));
		}

		private List<string> ValidateObject<T>(T objectToCheck, List<string> errors = null)
		{
			if (errors == null)
			{
				errors = new List<string>();
			}
			if (objectToCheck != null)
			{
				if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
				{
					foreach (object item in (IEnumerable)(object)objectToCheck)
					{
						ValidateFieldInfos(item, errors);
						ValidatePropertyInfos(item, errors);
					}
				}
				else
				{
					ValidateFieldInfos(objectToCheck, errors);
					ValidatePropertyInfos(objectToCheck, errors);
				}
			}
			return errors;
		}

		private void ValidatePropertyInfos<T>(T objectToCheck, List<string> errors)
		{
			PropertyInfo[] properties = objectToCheck.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.GetIndexParameters().Length != 0)
				{
					for (int j = 0; j < propertyInfo.GetIndexParameters().Length; j++)
					{
						object value = propertyInfo.GetValue(objectToCheck, new object[1] { j });
						string name = propertyInfo.Name;
						string name2 = objectToCheck.GetType().Name;
						ValidateValue(value, name2, "Property", name, errors);
					}
				}
				else
				{
					object value2 = propertyInfo.GetValue(objectToCheck);
					string name3 = propertyInfo.Name;
					string name4 = objectToCheck.GetType().Name;
					ValidateValue(value2, name4, "Property", name3, errors);
				}
			}
		}

		private void ValidateFieldInfos<T>(T objectToCheck, List<string> errors)
		{
			FieldInfo[] fields = objectToCheck.GetType().GetFields();
			foreach (FieldInfo obj in fields)
			{
				object value = obj.GetValue(objectToCheck);
				string name = obj.Name;
				string name2 = objectToCheck.GetType().Name;
				ValidateValue(value, name2, "Field", name, errors);
			}
		}

		private void ValidateValue(object value, string objectName, string memberType, string memberName, List<string> errors)
		{
			if (!(value is ValueType) && !(value is string))
			{
				if (value is JObject)
				{
					errors.Add(memberType + ": \"" + memberName + "\" on Type: \"" + objectName + "\" must not be of type `object` or `dynamic`");
				}
				else
				{
					ValidateObject(value, errors);
				}
			}
		}
	}
}
