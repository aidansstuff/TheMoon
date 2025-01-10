using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Unity.Services.Authentication
{
	public sealed class PlayerInfo
	{
		private const string k_OpenIdConnectPrefix = "oidc-";

		private const string k_IdProviderNameRegex = "^oidc-[a-z0-9-_\\.]{1,15}$";

		public string Id { get; }

		public DateTime? CreatedAt { get; }

		public List<Identity> Identities { get; }

		[CanBeNull]
		public string Username { get; internal set; }

		[CanBeNull]
		public DateTime? LastPasswordUpdate { get; internal set; }

		internal PlayerInfo(string playerId)
		{
			Id = playerId;
			Identities = new List<Identity>();
		}

		internal PlayerInfo(PlayerInfoResponse response)
			: this(response.Id, response.CreatedAt, response.ExternalIds, response.UsernamePassword?.Username, response.UsernamePassword?.PasswordUpdatedAt)
		{
		}

		internal PlayerInfo(User user)
			: this(user.Id, user.CreatedAt, user.ExternalIds, user.UsernameInfo?.Username ?? user.Username, user.UsernameInfo?.PasswordUpdatedAt)
		{
		}

		internal PlayerInfo(string playerId, string createdAt, List<ExternalIdentity> externalIdentities, string username, string lastPasswordUpdate)
		{
			Id = playerId;
			Identities = new List<Identity>();
			if (double.TryParse(createdAt, out var result))
			{
				CreatedAt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(result);
			}
			if (externalIdentities != null)
			{
				foreach (ExternalIdentity externalIdentity in externalIdentities)
				{
					Identities.Add(new Identity(externalIdentity));
				}
			}
			Username = username;
			if (double.TryParse(lastPasswordUpdate, out var result2))
			{
				LastPasswordUpdate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(result2);
			}
		}

		public string GetFacebookId()
		{
			return GetIdentityId("facebook.com");
		}

		public string GetSteamId()
		{
			return GetIdentityId("steampowered.com");
		}

		public string GetGoogleId()
		{
			return GetIdentityId("google.com");
		}

		public string GetGooglePlayGamesId()
		{
			return GetIdentityId("google-play-games");
		}

		public string GetAppleId()
		{
			return GetIdentityId("apple.com");
		}

		public string GetAppleGameCenterId()
		{
			return GetIdentityId("apple-game-center");
		}

		public string GetOculusId()
		{
			return GetIdentityId("oculus");
		}

		public string GetOpenIdConnectId(string idProviderName)
		{
			if (!ValidateOpenIdConnectIdProviderName(idProviderName))
			{
				return null;
			}
			return GetIdentityId(idProviderName);
		}

		public string GetUnityId()
		{
			return GetIdentityId("unity");
		}

		public List<Identity> GetOpenIdConnectIdProviders()
		{
			return Identities?.FindAll((Identity id) => id.TypeId.StartsWith("oidc-"));
		}

		internal string GetIdentityId(string typeId)
		{
			return Identities?.FirstOrDefault((Identity x) => x.TypeId == typeId)?.UserId;
		}

		internal void AddExternalIdentity(ExternalIdentity externalId)
		{
			if (externalId != null)
			{
				Identities.Add(new Identity(externalId));
			}
		}

		internal void RemoveIdentity(string typeId)
		{
			Identities?.RemoveAll((Identity x) => x.TypeId == typeId);
		}

		private bool ValidateOpenIdConnectIdProviderName(string idProviderName)
		{
			if (!string.IsNullOrEmpty(idProviderName))
			{
				return Regex.Match(idProviderName, "^oidc-[a-z0-9-_\\.]{1,15}$").Success;
			}
			return false;
		}
	}
}
