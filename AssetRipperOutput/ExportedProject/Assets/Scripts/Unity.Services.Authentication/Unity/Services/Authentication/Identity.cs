namespace Unity.Services.Authentication
{
	public sealed class Identity
	{
		public string TypeId;

		public string UserId;

		internal Identity(ExternalIdentity externalIdentity)
		{
			if (externalIdentity != null)
			{
				TypeId = externalIdentity.ProviderId;
				UserId = externalIdentity.ExternalId;
			}
		}
	}
}
