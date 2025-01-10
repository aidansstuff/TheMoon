namespace Unity.Services.Core.Internal.Serialization
{
	internal interface IJsonSerializer
	{
		string SerializeObject<T>(T value);

		T DeserializeObject<T>(string value);
	}
}
