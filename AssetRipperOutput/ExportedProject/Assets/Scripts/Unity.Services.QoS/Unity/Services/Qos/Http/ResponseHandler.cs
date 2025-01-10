using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Unity.Services.Qos.Models;

namespace Unity.Services.Qos.Http
{
	internal static class ResponseHandler
	{
		private static List<IDeserializable> DeserializeListOfJsonObjects(List<object> objectList)
		{
			List<IDeserializable> list = new List<IDeserializable>();
			foreach (object @object in objectList)
			{
				list.Add(new JsonObject(@object));
			}
			return list;
		}

		public static T TryDeserializeResponse<T>(HttpClientResponse response)
		{
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
			try
			{
				return JsonConvert.DeserializeObject<T>(GetDeserializedJson(response.Data), settings);
			}
			catch (Exception ex)
			{
				throw new ResponseDeserializationException(response, ex, ex.Message);
			}
		}

		public static object TryDeserializeResponse(HttpClientResponse response, Type type)
		{
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
			try
			{
				return JsonConvert.DeserializeObject(GetDeserializedJson(response.Data), type, settings);
			}
			catch (Exception ex)
			{
				throw new ResponseDeserializationException(response, ex, ex.Message);
			}
		}

		private static string GetDeserializedJson(byte[] data)
		{
			return Encoding.UTF8.GetString(data);
		}

		public static void HandleAsyncResponse(HttpClientResponse response, Dictionary<string, Type> statusCodeToTypeMap)
		{
			if (statusCodeToTypeMap.ContainsKey(response.StatusCode.ToString()))
			{
				Type type = statusCodeToTypeMap[response.StatusCode.ToString()];
				if ((type != null && response.IsHttpError) || response.IsNetworkError)
				{
					if (typeof(IOneOf).IsAssignableFrom(type))
					{
						throw CreateOneOfException(response, type);
					}
					throw CreateHttpException(response, type);
				}
				return;
			}
			throw new HttpException(response);
		}

		private static HttpException CreateOneOfException(HttpClientResponse response, Type responseType)
		{
			try
			{
				object obj = TryDeserializeResponse(response, responseType);
				return CreateHttpException(response, ((IOneOf)obj).Type);
			}
			catch (ArgumentException ex)
			{
				throw new ResponseDeserializationException(response, ex, ex.Message);
			}
			catch (MissingFieldException inner)
			{
				throw new ResponseDeserializationException(response, inner, "Discriminator field not found in the parsed json response.");
			}
			catch (ResponseDeserializationException ex2)
			{
				if (ex2.InnerException.GetType() == typeof(MissingFieldException))
				{
					throw new ResponseDeserializationException(response, ex2.InnerException, "Discriminator field not found in the parsed json response.");
				}
				if (ex2.response == null)
				{
					throw new ResponseDeserializationException(response, ex2.Message);
				}
				throw;
			}
			catch (Exception ex3)
			{
				throw new ResponseDeserializationException(response, ex3, ex3.Message);
			}
		}

		private static HttpException CreateHttpException(HttpClientResponse response, Type responseType)
		{
			Type type = typeof(HttpException<>).MakeGenericType(responseType);
			try
			{
				if (responseType == typeof(Stream))
				{
					object obj = ((response.Data == null) ? new MemoryStream() : new MemoryStream(response.Data));
					return (HttpException)Activator.CreateInstance(type, response, obj);
				}
				object obj2 = TryDeserializeResponse(response, responseType);
				return (HttpException)Activator.CreateInstance(type, response, obj2);
			}
			catch (ArgumentException ex)
			{
				throw new ResponseDeserializationException(response, ex, ex.Message);
			}
			catch (MissingFieldException inner)
			{
				throw new ResponseDeserializationException(response, inner, "Discriminator field not found in the parsed json response.");
			}
			catch (ResponseDeserializationException ex2)
			{
				if (ex2.response == null)
				{
					throw new ResponseDeserializationException(response, ex2.Message);
				}
				throw;
			}
			catch (Exception ex3)
			{
				throw new ResponseDeserializationException(response, ex3, ex3.Message);
			}
		}

		public static T HandleAsyncResponse<T>(HttpClientResponse response, Dictionary<string, Type> statusCodeToTypeMap) where T : class
		{
			HandleAsyncResponse(response, statusCodeToTypeMap);
			try
			{
				if (statusCodeToTypeMap[response.StatusCode.ToString()] == typeof(Stream))
				{
					return ((response.Data == null) ? new MemoryStream() : new MemoryStream(response.Data)) as T;
				}
				return TryDeserializeResponse<T>(response);
			}
			catch (ArgumentException ex)
			{
				throw new ResponseDeserializationException(response, ex.Message);
			}
			catch (MissingFieldException inner)
			{
				throw new ResponseDeserializationException(response, inner, "Discriminator field not found in the parsed json response.");
			}
			catch (ResponseDeserializationException ex2)
			{
				if (ex2.response == null)
				{
					throw new ResponseDeserializationException(response, ex2.Message);
				}
				throw;
			}
			catch (Exception ex3)
			{
				throw new ResponseDeserializationException(response, ex3, ex3.Message);
			}
		}
	}
}
