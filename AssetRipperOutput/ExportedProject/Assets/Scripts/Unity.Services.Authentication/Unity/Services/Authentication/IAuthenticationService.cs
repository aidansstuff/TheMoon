using System;
using System.Threading.Tasks;
using Unity.Services.Core;

namespace Unity.Services.Authentication
{
	public interface IAuthenticationService
	{
		bool IsSignedIn { get; }

		bool IsAuthorized { get; }

		bool IsExpired { get; }

		string AccessToken { get; }

		string PlayerId { get; }

		string PlayerName { get; }

		string Profile { get; }

		bool SessionTokenExists { get; }

		PlayerInfo PlayerInfo { get; }

		event Action SignedIn;

		event Action SignedOut;

		event Action Expired;

		event Action<RequestFailedException> SignInFailed;

		Task SignInAnonymouslyAsync(SignInOptions options = null);

		Task SignInWithAppleAsync(string idToken, SignInOptions options = null);

		Task LinkWithAppleAsync(string idToken, LinkOptions options = null);

		Task UnlinkAppleAsync();

		Task SignInWithAppleGameCenterAsync(string signature, string teamPlayerId, string publicKeyURL, string salt, ulong timestamp, SignInOptions options = null);

		Task LinkWithAppleGameCenterAsync(string signature, string teamPlayerId, string publicKeyURL, string salt, ulong timestamp, LinkOptions options = null);

		Task UnlinkAppleGameCenterAsync();

		Task SignInWithGoogleAsync(string idToken, SignInOptions options = null);

		Task LinkWithGoogleAsync(string idToken, LinkOptions options = null);

		Task UnlinkGoogleAsync();

		Task SignInWithGooglePlayGamesAsync(string authCode, SignInOptions options = null);

		Task LinkWithGooglePlayGamesAsync(string authCode, LinkOptions options = null);

		Task UnlinkGooglePlayGamesAsync();

		Task SignInWithFacebookAsync(string accessToken, SignInOptions options = null);

		Task LinkWithFacebookAsync(string accessToken, LinkOptions options = null);

		Task UnlinkFacebookAsync();

		Task SignInWithSteamAsync(string sessionTicket, string identity, SignInOptions options = null);

		[Obsolete("This method is deprecated as of version 2.7.1. Please use the SignInWithSteamAsync method with the 'identity' parameter for better security.")]
		Task SignInWithSteamAsync(string sessionTicket, SignInOptions options = null);

		Task LinkWithSteamAsync(string sessionTicket, string identity, LinkOptions options = null);

		[Obsolete("This method is deprecated as of version 2.7.1. Please use the LinkWithSteamAsync method with the 'identity' parameter for better security.")]
		Task LinkWithSteamAsync(string sessionTicket, LinkOptions options = null);

		Task UnlinkSteamAsync();

		Task SignInWithOculusAsync(string nonce, string userId, SignInOptions options = null);

		Task LinkWithOculusAsync(string nonce, string userId, LinkOptions options = null);

		Task UnlinkOculusAsync();

		Task SignInWithOpenIdConnectAsync(string idProviderName, string idToken, SignInOptions options = null);

		Task LinkWithOpenIdConnectAsync(string idProviderName, string idToken, LinkOptions options = null);

		Task UnlinkOpenIdConnectAsync(string idProviderName);

		Task SignInWithUnityAsync(string token, SignInOptions options = null);

		Task LinkWithUnityAsync(string token, LinkOptions options = null);

		Task UnlinkUnityAsync();

		Task SignInWithUsernamePasswordAsync(string username, string password);

		Task SignUpWithUsernamePasswordAsync(string username, string password);

		Task AddUsernamePasswordAsync(string username, string password);

		Task UpdatePasswordAsync(string currentPassword, string newPassword);

		Task DeleteAccountAsync();

		Task<PlayerInfo> GetPlayerInfoAsync();

		Task<string> GetPlayerNameAsync();

		Task<string> UpdatePlayerNameAsync(string name);

		void SignOut(bool clearCredentials = false);

		void SwitchProfile(string profile);

		void ClearSessionToken();
	}
}
