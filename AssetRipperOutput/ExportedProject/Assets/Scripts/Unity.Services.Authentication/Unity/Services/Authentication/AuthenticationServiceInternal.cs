using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Services.Authentication.Generated;
using Unity.Services.Authentication.Shared;
using Unity.Services.Core;
using Unity.Services.Core.Scheduler.Internal;

namespace Unity.Services.Authentication
{
	internal class AuthenticationServiceInternal : IAuthenticationService
	{
		private const string k_ProfileRegex = "^[a-zA-Z0-9_-]{1,30}$";

		private const string k_IdProviderNameRegex = "^oidc-[a-z0-9-_\\.]{1,15}$";

		private const string k_SteamIdentityRegex = "^[a-zA-Z0-9]{5,30}$";

		private readonly IProfile m_Profile;

		private readonly IJwtDecoder m_JwtDecoder;

		private readonly IAuthenticationCache m_Cache;

		private readonly IActionScheduler m_Scheduler;

		private readonly IAuthenticationMetrics m_Metrics;

		public bool IsSignedIn
		{
			get
			{
				if (State != AuthenticationState.Authorized && State != AuthenticationState.Refreshing)
				{
					return State == AuthenticationState.Expired;
				}
				return true;
			}
		}

		public bool IsAuthorized
		{
			get
			{
				if (State != AuthenticationState.Authorized)
				{
					return State == AuthenticationState.Refreshing;
				}
				return true;
			}
		}

		public bool IsExpired => State == AuthenticationState.Expired;

		public bool SessionTokenExists => !string.IsNullOrEmpty(SessionTokenComponent.SessionToken);

		public string Profile => m_Profile.Current;

		public string AccessToken => AccessTokenComponent.AccessToken;

		public string PlayerId => PlayerIdComponent.PlayerId;

		public string PlayerName => PlayerNameComponent.PlayerName;

		public PlayerInfo PlayerInfo { get; internal set; }

		internal long? ExpirationActionId { get; set; }

		internal long? RefreshActionId { get; set; }

		internal AccessTokenComponent AccessTokenComponent { get; }

		internal EnvironmentIdComponent EnvironmentIdComponent { get; }

		internal PlayerIdComponent PlayerIdComponent { get; }

		internal PlayerNameComponent PlayerNameComponent { get; }

		internal SessionTokenComponent SessionTokenComponent { get; }

		internal AuthenticationState State { get; set; }

		internal IAuthenticationSettings Settings { get; }

		internal IAuthenticationNetworkClient NetworkClient { get; set; }

		internal IPlayerNamesApi PlayerNamesApi { get; set; }

		internal IAuthenticationExceptionHandler ExceptionHandler { get; set; }

		public event Action<RequestFailedException> SignInFailed;

		public event Action SignedIn;

		public event Action SignedOut;

		public event Action Expired;

		public event Action<RequestFailedException> UpdatePasswordFailed;

		internal event Action<AuthenticationState, AuthenticationState> StateChanged;

		internal AuthenticationServiceInternal(IAuthenticationSettings settings, IAuthenticationNetworkClient networkClient, IPlayerNamesApi playerNamesApi, IProfile profile, IJwtDecoder jwtDecoder, IAuthenticationCache cache, IActionScheduler scheduler, IAuthenticationMetrics metrics, AccessTokenComponent accessToken, EnvironmentIdComponent environmentId, PlayerIdComponent playerId, PlayerNameComponent playerName, SessionTokenComponent sessionToken)
		{
			Settings = settings;
			NetworkClient = networkClient;
			PlayerNamesApi = playerNamesApi;
			m_Profile = profile;
			m_JwtDecoder = jwtDecoder;
			m_Cache = cache;
			m_Scheduler = scheduler;
			m_Metrics = metrics;
			ExceptionHandler = new AuthenticationExceptionHandler(m_Metrics);
			AccessTokenComponent = accessToken;
			EnvironmentIdComponent = environmentId;
			PlayerIdComponent = playerId;
			PlayerNameComponent = playerName;
			SessionTokenComponent = sessionToken;
			State = AuthenticationState.SignedOut;
			MigrateCache();
			PlayerIdComponent.PlayerIdChanged += OnPlayerIdChanged;
			Expired += delegate
			{
				m_Metrics.SendExpiredSessionMetric();
			};
		}

		private void OnPlayerIdChanged(string playerId)
		{
			PlayerNameComponent.Clear();
		}

		public Task SignInAnonymouslyAsync(SignInOptions options = null)
		{
			if (State == AuthenticationState.SignedOut || State == AuthenticationState.Expired)
			{
				if (SessionTokenExists)
				{
					string sessionToken = SessionTokenComponent.SessionToken;
					if (string.IsNullOrEmpty(sessionToken))
					{
						SessionTokenComponent.Clear();
						RequestFailedException exception = ExceptionHandler.BuildClientSessionTokenNotExistsException();
						SendSignInFailedEvent(exception, forceSignOut: true);
						return Task.FromException(exception);
					}
					return HandleSignInRequestAsync(() => NetworkClient.SignInWithSessionTokenAsync(sessionToken));
				}
				if (options == null || options.CreateAccount)
				{
					return HandleSignInRequestAsync(NetworkClient.SignInAnonymouslyAsync);
				}
				SessionTokenComponent.Clear();
				RequestFailedException exception2 = ExceptionHandler.BuildClientSessionTokenNotExistsException();
				SendSignInFailedEvent(exception2, forceSignOut: true);
				return Task.FromException(exception2);
			}
			RequestFailedException exception3 = ExceptionHandler.BuildClientInvalidStateException(State);
			SendSignInFailedEvent(exception3, forceSignOut: false);
			return Task.FromException(exception3);
		}

		public Task SignInWithAppleAsync(string idToken, SignInOptions options = null)
		{
			return SignInWithExternalTokenAsync("apple.com", new SignInWithExternalTokenRequest
			{
				IdProvider = "apple.com",
				Token = idToken,
				SignInOnly = (options != null && !options.CreateAccount)
			});
		}

		public Task LinkWithAppleAsync(string idToken, LinkOptions options = null)
		{
			return LinkWithExternalTokenAsync("apple.com", new LinkWithExternalTokenRequest
			{
				IdProvider = "apple.com",
				Token = idToken,
				ForceLink = (options?.ForceLink ?? false)
			});
		}

		public Task UnlinkAppleAsync()
		{
			return UnlinkExternalTokenAsync("apple.com");
		}

		public Task SignInWithAppleGameCenterAsync(string signature, string teamPlayerId, string publicKeyURL, string salt, ulong timestamp, SignInOptions options = null)
		{
			return SignInWithExternalTokenAsync("apple-game-center", new SignInWithAppleGameCenterRequest
			{
				IdProvider = "apple-game-center",
				Token = signature,
				AppleGameCenterConfig = new AppleGameCenterConfig
				{
					TeamPlayerId = teamPlayerId,
					PublicKeyURL = publicKeyURL,
					Salt = salt,
					Timestamp = timestamp
				},
				SignInOnly = (options != null && !options.CreateAccount)
			});
		}

		public Task LinkWithAppleGameCenterAsync(string signature, string teamPlayerId, string publicKeyURL, string salt, ulong timestamp, LinkOptions options = null)
		{
			return LinkWithExternalTokenAsync("apple-game-center", new LinkWithAppleGameCenterRequest
			{
				IdProvider = "apple-game-center",
				Token = signature,
				AppleGameCenterConfig = new AppleGameCenterConfig
				{
					TeamPlayerId = teamPlayerId,
					PublicKeyURL = publicKeyURL,
					Salt = salt,
					Timestamp = timestamp
				},
				ForceLink = (options?.ForceLink ?? false)
			});
		}

		public Task UnlinkAppleGameCenterAsync()
		{
			return UnlinkExternalTokenAsync("apple-game-center");
		}

		public Task SignInWithGoogleAsync(string idToken, SignInOptions options = null)
		{
			return SignInWithExternalTokenAsync("google.com", new SignInWithExternalTokenRequest
			{
				IdProvider = "google.com",
				Token = idToken,
				SignInOnly = (options != null && !options.CreateAccount)
			});
		}

		public Task LinkWithGoogleAsync(string idToken, LinkOptions options = null)
		{
			return LinkWithExternalTokenAsync("google.com", new LinkWithExternalTokenRequest
			{
				IdProvider = "google.com",
				Token = idToken,
				ForceLink = (options?.ForceLink ?? false)
			});
		}

		public Task UnlinkGoogleAsync()
		{
			return UnlinkExternalTokenAsync("google.com");
		}

		public Task SignInWithGooglePlayGamesAsync(string authCode, SignInOptions options = null)
		{
			return SignInWithExternalTokenAsync("google-play-games", new SignInWithExternalTokenRequest
			{
				IdProvider = "google-play-games",
				Token = authCode,
				SignInOnly = (options != null && !options.CreateAccount)
			});
		}

		public Task LinkWithGooglePlayGamesAsync(string authCode, LinkOptions options = null)
		{
			return LinkWithExternalTokenAsync("google-play-games", new LinkWithExternalTokenRequest
			{
				IdProvider = "google-play-games",
				Token = authCode,
				ForceLink = (options?.ForceLink ?? false)
			});
		}

		public Task UnlinkGooglePlayGamesAsync()
		{
			return UnlinkExternalTokenAsync("google-play-games");
		}

		public Task SignInWithFacebookAsync(string accessToken, SignInOptions options = null)
		{
			return SignInWithExternalTokenAsync("facebook.com", new SignInWithExternalTokenRequest
			{
				IdProvider = "facebook.com",
				Token = accessToken,
				SignInOnly = (options != null && !options.CreateAccount)
			});
		}

		public Task LinkWithFacebookAsync(string accessToken, LinkOptions options = null)
		{
			return LinkWithExternalTokenAsync("facebook.com", new LinkWithExternalTokenRequest
			{
				IdProvider = "facebook.com",
				Token = accessToken,
				ForceLink = (options?.ForceLink ?? false)
			});
		}

		public Task UnlinkFacebookAsync()
		{
			return UnlinkExternalTokenAsync("facebook.com");
		}

		[Obsolete("This method is deprecated as of version 2.7.1. Please use the SignInWithSteamAsync method with the 'identity' parameter for better security.")]
		public Task SignInWithSteamAsync(string sessionTicket, SignInOptions options = null)
		{
			return SignInWithExternalTokenAsync("steampowered.com", new SignInWithSteamRequest
			{
				IdProvider = "steampowered.com",
				Token = sessionTicket,
				SignInOnly = (options != null && !options.CreateAccount)
			});
		}

		[Obsolete("This method is deprecated as of version 2.7.1. Please use the LinkWithSteamAsync method with the 'identity' parameter for better security.")]
		public Task LinkWithSteamAsync(string sessionTicket, LinkOptions options = null)
		{
			return LinkWithExternalTokenAsync("steampowered.com", new LinkWithSteamRequest
			{
				IdProvider = "steampowered.com",
				Token = sessionTicket,
				ForceLink = (options?.ForceLink ?? false)
			});
		}

		public Task SignInWithSteamAsync(string sessionTicket, string identity, SignInOptions options = null)
		{
			ValidateSteamIdentity(identity);
			return SignInWithExternalTokenAsync("steampowered.com", new SignInWithSteamRequest
			{
				IdProvider = "steampowered.com",
				Token = sessionTicket,
				SteamConfig = new SteamConfig
				{
					identity = identity
				},
				SignInOnly = (options != null && !options.CreateAccount)
			});
		}

		public Task LinkWithSteamAsync(string sessionTicket, string identity, LinkOptions options = null)
		{
			ValidateSteamIdentity(identity);
			return LinkWithExternalTokenAsync("steampowered.com", new LinkWithSteamRequest
			{
				IdProvider = "steampowered.com",
				Token = sessionTicket,
				SteamConfig = new SteamConfig
				{
					identity = identity
				},
				ForceLink = (options?.ForceLink ?? false)
			});
		}

		private void ValidateSteamIdentity(string identity)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw ExceptionHandler.BuildUnknownException("Identity cannot be null or empty.");
			}
			if (!Regex.IsMatch(identity, "^[a-zA-Z0-9]{5,30}$"))
			{
				throw ExceptionHandler.BuildUnknownException("The provided identity must only contain alphanumeric characters and be between 5 and 30 characters in length.");
			}
		}

		public Task UnlinkSteamAsync()
		{
			return UnlinkExternalTokenAsync("steampowered.com");
		}

		public Task SignInWithOculusAsync(string nonce, string userId, SignInOptions options = null)
		{
			return SignInWithExternalTokenAsync("oculus", new SignInWithOculusRequest
			{
				IdProvider = "oculus",
				Token = nonce,
				OculusConfig = new OculusConfig
				{
					UserId = userId
				},
				SignInOnly = (options != null && !options.CreateAccount)
			});
		}

		public Task LinkWithOculusAsync(string nonce, string userId, LinkOptions options = null)
		{
			return LinkWithExternalTokenAsync("oculus", new LinkWithOculusRequest
			{
				IdProvider = "oculus",
				Token = nonce,
				OculusConfig = new OculusConfig
				{
					UserId = userId
				},
				ForceLink = (options?.ForceLink ?? false)
			});
		}

		public Task UnlinkOculusAsync()
		{
			return UnlinkExternalTokenAsync("oculus");
		}

		public Task SignInWithOpenIdConnectAsync(string idProviderName, string idToken, SignInOptions options = null)
		{
			if (!ValidateOpenIdConnectIdProviderName(idProviderName))
			{
				throw ExceptionHandler.BuildInvalidIdProviderNameException();
			}
			return SignInWithExternalTokenAsync(idProviderName, new SignInWithExternalTokenRequest
			{
				IdProvider = idProviderName,
				Token = idToken,
				SignInOnly = (options != null && !options.CreateAccount)
			});
		}

		public Task LinkWithOpenIdConnectAsync(string idProviderName, string idToken, LinkOptions options = null)
		{
			if (!ValidateOpenIdConnectIdProviderName(idProviderName))
			{
				throw ExceptionHandler.BuildInvalidIdProviderNameException();
			}
			return LinkWithExternalTokenAsync(idProviderName, new LinkWithExternalTokenRequest
			{
				IdProvider = idProviderName,
				Token = idToken,
				ForceLink = (options?.ForceLink ?? false)
			});
		}

		public Task UnlinkOpenIdConnectAsync(string idProviderName)
		{
			if (!ValidateOpenIdConnectIdProviderName(idProviderName))
			{
				throw ExceptionHandler.BuildInvalidIdProviderNameException();
			}
			return UnlinkExternalTokenAsync(idProviderName);
		}

		public Task SignInWithUsernamePasswordAsync(string username, string password)
		{
			return SignInWithUsernamePasswordRequestAsync(BuildUsernamePasswordRequest(username, password));
		}

		public Task SignUpWithUsernamePasswordAsync(string username, string password)
		{
			return SignUpWithUsernamePasswordRequestAsync(BuildUsernamePasswordRequest(username, password));
		}

		public Task AddUsernamePasswordAsync(string username, string password)
		{
			return AddUsernamePasswordRequestAsync(BuildUsernamePasswordRequest(username, password));
		}

		public Task UpdatePasswordAsync(string currentPassword, string newPassword)
		{
			if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
			{
				throw ExceptionHandler.BuildInvalidCredentialsException();
			}
			return UpdatePasswordRequestAsync(new UpdatePasswordRequest
			{
				Password = currentPassword,
				NewPassword = newPassword
			});
		}

		public Task SignInWithUnityAsync(string token, SignInOptions options = null)
		{
			return SignInWithExternalTokenAsync("unity", new SignInWithExternalTokenRequest
			{
				IdProvider = "unity",
				Token = token,
				SignInOnly = (options != null && !options.CreateAccount)
			});
		}

		public Task LinkWithUnityAsync(string token, LinkOptions options = null)
		{
			return LinkWithExternalTokenAsync("unity", new LinkWithExternalTokenRequest
			{
				IdProvider = "unity",
				Token = token,
				ForceLink = (options?.ForceLink ?? false)
			});
		}

		public Task UnlinkUnityAsync()
		{
			return UnlinkExternalTokenAsync("unity");
		}

		public async Task DeleteAccountAsync()
		{
			if (IsAuthorized)
			{
				try
				{
					await NetworkClient.DeleteAccountAsync(PlayerId);
					SignOut(clearCredentials: true);
					return;
				}
				catch (WebRequestException exception)
				{
					throw ExceptionHandler.ConvertException(exception);
				}
			}
			throw ExceptionHandler.BuildClientInvalidStateException(State);
		}

		public void SignOut(bool clearCredentials = false)
		{
			AccessTokenComponent.Clear();
			PlayerInfo = null;
			if (clearCredentials)
			{
				SessionTokenComponent.Clear();
				PlayerIdComponent.Clear();
				PlayerNameComponent.Clear();
			}
			CancelScheduledRefresh();
			CancelScheduledExpiration();
			ChangeState(AuthenticationState.SignedOut);
		}

		public void SwitchProfile(string profile)
		{
			if (State == AuthenticationState.SignedOut)
			{
				if (!string.IsNullOrEmpty(profile) && Regex.Match(profile, "^[a-zA-Z0-9_-]{1,30}$").Success)
				{
					m_Profile.Current = profile;
					return;
				}
				throw ExceptionHandler.BuildClientInvalidProfileException();
			}
			throw ExceptionHandler.BuildClientInvalidStateException(State);
		}

		public void ClearSessionToken()
		{
			if (State == AuthenticationState.SignedOut)
			{
				SessionTokenComponent.Clear();
				return;
			}
			throw ExceptionHandler.BuildClientInvalidStateException(State);
		}

		public async Task<PlayerInfo> GetPlayerInfoAsync()
		{
			if (IsAuthorized)
			{
				try
				{
					PlayerInfo = new PlayerInfo(await NetworkClient.GetPlayerInfoAsync(PlayerId));
					return PlayerInfo;
				}
				catch (WebRequestException exception)
				{
					throw ExceptionHandler.ConvertException(exception);
				}
			}
			throw ExceptionHandler.BuildClientInvalidStateException(State);
		}

		public async Task<string> GetPlayerNameAsync()
		{
			if (IsAuthorized)
			{
				try
				{
					PlayerNamesApi.Configuration.AccessToken = AccessTokenComponent.AccessToken;
					Player data = (await PlayerNamesApi.GetNameAsync(PlayerId)).Data;
					PlayerNameComponent.PlayerName = data.Name;
					return data.Name;
				}
				catch (ApiException ex)
				{
					if (ex.Response.StatusCode == 404)
					{
						PlayerNameComponent.Clear();
						return null;
					}
					throw ExceptionHandler.ConvertException(ex);
				}
				catch (Exception ex2)
				{
					throw ExceptionHandler.BuildUnknownException(ex2.Message);
				}
			}
			throw ExceptionHandler.BuildClientInvalidStateException(State);
		}

		public async Task<string> UpdatePlayerNameAsync(string playerName)
		{
			if (IsAuthorized)
			{
				if (string.IsNullOrWhiteSpace(playerName) || playerName.Any(char.IsWhiteSpace))
				{
					throw ExceptionHandler.BuildInvalidPlayerNameException();
				}
				try
				{
					PlayerNamesApi.Configuration.AccessToken = AccessTokenComponent.AccessToken;
					string text = (await PlayerNamesApi.UpdateNameAsync(PlayerId, new UpdateNameRequest(playerName))).Data?.Name;
					if (string.IsNullOrWhiteSpace(text))
					{
						throw ExceptionHandler.BuildUnknownException("Invalid player name response");
					}
					PlayerNameComponent.PlayerName = text;
					return text;
				}
				catch (ApiException exception)
				{
					throw ExceptionHandler.ConvertException(exception);
				}
				catch (Exception ex)
				{
					throw ExceptionHandler.BuildUnknownException(ex.Message);
				}
			}
			throw ExceptionHandler.BuildClientInvalidStateException(State);
		}

		internal Task SignInWithExternalTokenAsync(string idProvider, SignInWithExternalTokenRequest request, bool enableRefresh = true)
		{
			if (State == AuthenticationState.SignedOut || State == AuthenticationState.Expired)
			{
				return HandleSignInRequestAsync(() => NetworkClient.SignInWithExternalTokenAsync(idProvider, request), enableRefresh);
			}
			RequestFailedException exception = ExceptionHandler.BuildClientInvalidStateException(State);
			SendSignInFailedEvent(exception, forceSignOut: false);
			return Task.FromException(exception);
		}

		internal async Task LinkWithExternalTokenAsync(string idProvider, LinkWithExternalTokenRequest request)
		{
			if (IsAuthorized)
			{
				try
				{
					LinkResponse linkResponse = await NetworkClient.LinkWithExternalTokenAsync(idProvider, request);
					PlayerInfo?.AddExternalIdentity(linkResponse.User?.ExternalIds?.FirstOrDefault((ExternalIdentity x) => x.ProviderId == request.IdProvider));
					return;
				}
				catch (WebRequestException exception)
				{
					throw ExceptionHandler.ConvertException(exception);
				}
			}
			throw ExceptionHandler.BuildClientInvalidStateException(State);
		}

		internal async Task UnlinkExternalTokenAsync(string idProvider)
		{
			if (IsAuthorized)
			{
				string text = PlayerInfo?.GetIdentityId(idProvider);
				if (text == null)
				{
					throw ExceptionHandler.BuildClientUnlinkExternalIdNotFoundException();
				}
				try
				{
					await NetworkClient.UnlinkExternalTokenAsync(idProvider, new UnlinkRequest
					{
						IdProvider = idProvider,
						ExternalId = text
					});
					PlayerInfo.RemoveIdentity(idProvider);
					return;
				}
				catch (WebRequestException exception)
				{
					throw ExceptionHandler.ConvertException(exception);
				}
			}
			throw ExceptionHandler.BuildClientInvalidStateException(State);
		}

		internal Task SignInWithUsernamePasswordRequestAsync(UsernamePasswordRequest request, bool enableRefresh = true)
		{
			if (State == AuthenticationState.SignedOut || State == AuthenticationState.Expired)
			{
				return HandleSignInRequestAsync(() => NetworkClient.SignInWithUsernamePasswordAsync(request), enableRefresh);
			}
			RequestFailedException exception = ExceptionHandler.BuildClientInvalidStateException(State);
			SendSignInFailedEvent(exception, forceSignOut: false);
			return Task.FromException(exception);
		}

		internal Task SignUpWithUsernamePasswordRequestAsync(UsernamePasswordRequest request, bool enableRefresh = true)
		{
			if (State == AuthenticationState.SignedOut || State == AuthenticationState.Expired)
			{
				return HandleSignInRequestAsync(() => NetworkClient.SignUpWithUsernamePasswordAsync(request), enableRefresh);
			}
			RequestFailedException exception = ExceptionHandler.BuildClientInvalidStateException(State);
			SendSignInFailedEvent(exception, forceSignOut: false);
			return Task.FromException(exception);
		}

		internal async Task AddUsernamePasswordRequestAsync(UsernamePasswordRequest request)
		{
			if (IsAuthorized)
			{
				try
				{
					SignInResponse signInResponse = await NetworkClient.AddUsernamePasswordAsync(request);
					PlayerInfo.Username = signInResponse.User?.Username;
					return;
				}
				catch (WebRequestException exception)
				{
					throw ExceptionHandler.ConvertException(exception);
				}
				catch (Exception ex)
				{
					throw ExceptionHandler.BuildUnknownException(ex.Message);
				}
			}
			throw ExceptionHandler.BuildClientInvalidStateException(State);
		}

		internal Task UpdatePasswordRequestAsync(UpdatePasswordRequest request, bool enableRefresh = true)
		{
			if (IsAuthorized)
			{
				return HandleUpdatePasswordRequestAsync(() => NetworkClient.UpdatePasswordAsync(request), enableRefresh);
			}
			return Task.FromException(ExceptionHandler.BuildClientInvalidStateException(State));
		}

		internal Task RefreshAccessTokenAsync()
		{
			if (IsSignedIn)
			{
				if (State == AuthenticationState.Expired)
				{
					return Task.CompletedTask;
				}
				string sessionToken = SessionTokenComponent.SessionToken;
				if (string.IsNullOrEmpty(sessionToken))
				{
					return Task.CompletedTask;
				}
				return StartRefreshAsync(sessionToken);
			}
			return Task.CompletedTask;
		}

		internal async Task HandleSignInRequestAsync(Func<Task<SignInResponse>> signInRequest, bool enableRefresh = true)
		{
			try
			{
				ChangeState(AuthenticationState.SigningIn);
				CompleteSignIn(await signInRequest(), enableRefresh);
			}
			catch (RequestFailedException exception)
			{
				SendSignInFailedEvent(exception, forceSignOut: true);
				throw;
			}
			catch (WebRequestException exception2)
			{
				RequestFailedException ex = ExceptionHandler.ConvertException(exception2);
				if (ex.ErrorCode == AuthenticationErrorCodes.InvalidSessionToken)
				{
					SessionTokenComponent.Clear();
					Logger.Log("The session token is invalid and has been cleared. The associated account is no longer accessible through this login method.");
				}
				SendSignInFailedEvent(ex, forceSignOut: true);
				throw ex;
			}
		}

		internal async Task HandleUpdatePasswordRequestAsync(Func<Task<SignInResponse>> updatePasswordRequest, bool enableRefresh = true)
		{
			try
			{
				CompleteSignIn(await updatePasswordRequest(), enableRefresh);
			}
			catch (RequestFailedException exception)
			{
				SendUpdatePasswordFailedEvent(exception, forceSignOut: false);
				throw;
			}
			catch (WebRequestException exception2)
			{
				RequestFailedException ex = ExceptionHandler.ConvertException(exception2);
				if (ex.ErrorCode == AuthenticationErrorCodes.InvalidSessionToken)
				{
					SessionTokenComponent.Clear();
					Logger.Log("The session token is invalid and has been cleared. The associated account is no longer accessible through this login method.");
				}
				SendUpdatePasswordFailedEvent(ex, forceSignOut: false);
				throw ex;
			}
		}

		internal async Task StartRefreshAsync(string sessionToken)
		{
			ChangeState(AuthenticationState.Refreshing);
			try
			{
				CompleteSignIn(await NetworkClient.SignInWithSessionTokenAsync(sessionToken));
			}
			catch (RequestFailedException)
			{
				Logger.LogWarning("The access token is not valid. Retry and refresh again.");
				if (State != AuthenticationState.Expired)
				{
					Expire();
				}
			}
			catch (WebRequestException)
			{
				if (State == AuthenticationState.Refreshing)
				{
					Logger.LogWarning("Failed to refresh access token due to network error or internal server error, will retry later.");
					ChangeState(AuthenticationState.Authorized);
					ScheduleRefresh(Settings.RefreshAttemptFrequency);
				}
			}
		}

		internal void CompleteSignIn(SignInResponse response, bool enableRefresh = true)
		{
			try
			{
				AccessToken accessToken = m_JwtDecoder.Decode<AccessToken>(response.IdToken);
				if (accessToken == null)
				{
					throw AuthenticationException.Create(51, "Failed to decode and verify access token.");
				}
				AccessTokenComponent.AccessToken = response.IdToken;
				if (accessToken.Audience != null)
				{
					EnvironmentIdComponent.EnvironmentId = accessToken.Audience.FirstOrDefault((string s) => s.StartsWith("envId:"))?.Substring(6);
				}
				PlayerInfo = ((response.User != null) ? new PlayerInfo(response.User) : new PlayerInfo(response.UserId));
				PlayerIdComponent.PlayerId = response.UserId;
				SessionTokenComponent.SessionToken = response.SessionToken;
				int num = response.ExpiresIn - Settings.AccessTokenRefreshBuffer;
				int num2 = response.ExpiresIn - Settings.AccessTokenExpiryBuffer;
				AccessTokenComponent.ExpiryTime = DateTime.UtcNow.AddSeconds(num2);
				if (enableRefresh)
				{
					ScheduleRefresh(num);
				}
				ScheduleExpiration(num2);
				ChangeState(AuthenticationState.Authorized);
			}
			catch (AuthenticationException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw AuthenticationException.Create(0, "Unknown error completing sign-in.", innerException);
			}
		}

		internal void ScheduleRefresh(double delay)
		{
			CancelScheduledRefresh();
			DateTime value = DateTime.UtcNow.AddSeconds(delay);
			DateTime? expiryTime = AccessTokenComponent.ExpiryTime;
			if (value < expiryTime)
			{
				RefreshActionId = m_Scheduler.ScheduleAction(ExecuteScheduledRefresh, delay);
			}
		}

		internal void ScheduleExpiration(double delay)
		{
			CancelScheduledExpiration();
			ExpirationActionId = m_Scheduler.ScheduleAction(ExecuteScheduledExpiration, delay);
		}

		internal void ExecuteScheduledRefresh()
		{
			RefreshActionId = null;
			RefreshAccessTokenAsync();
		}

		internal void ExecuteScheduledExpiration()
		{
			ExpirationActionId = null;
			Expire();
		}

		internal void CancelScheduledRefresh()
		{
			if (RefreshActionId.HasValue)
			{
				m_Scheduler.CancelAction(RefreshActionId.Value);
				RefreshActionId = null;
			}
		}

		internal void CancelScheduledExpiration()
		{
			if (ExpirationActionId.HasValue)
			{
				m_Scheduler.CancelAction(ExpirationActionId.Value);
				ExpirationActionId = null;
			}
		}

		internal void Expire()
		{
			AccessTokenComponent.Clear();
			CancelScheduledRefresh();
			CancelScheduledExpiration();
			ChangeState(AuthenticationState.Expired);
		}

		internal void MigrateCache()
		{
			try
			{
				SessionTokenComponent.Migrate();
			}
			catch (Exception exception)
			{
				Logger.LogException(exception);
			}
		}

		private void ChangeState(AuthenticationState newState)
		{
			if (State != newState)
			{
				AuthenticationState state = State;
				State = newState;
				HandleStateChanged(state, newState);
			}
		}

		private void HandleStateChanged(AuthenticationState oldState, AuthenticationState newState)
		{
			this.StateChanged?.Invoke(oldState, newState);
			switch (newState)
			{
			case AuthenticationState.Authorized:
				if (oldState != AuthenticationState.Refreshing)
				{
					this.SignedIn?.Invoke();
				}
				break;
			case AuthenticationState.SignedOut:
				if (oldState != AuthenticationState.SigningIn)
				{
					this.SignedOut?.Invoke();
				}
				break;
			case AuthenticationState.Expired:
				this.Expired?.Invoke();
				break;
			case AuthenticationState.SigningIn:
			case AuthenticationState.Refreshing:
				break;
			}
		}

		private void SendSignInFailedEvent(RequestFailedException exception, bool forceSignOut)
		{
			this.SignInFailed?.Invoke(exception);
			if (forceSignOut)
			{
				SignOut();
			}
		}

		private void SendUpdatePasswordFailedEvent(RequestFailedException exception, bool forceSignOut)
		{
			this.UpdatePasswordFailed?.Invoke(exception);
			if (forceSignOut)
			{
				SignOut();
			}
		}

		private bool ValidateOpenIdConnectIdProviderName(string idProviderName)
		{
			if (!string.IsNullOrEmpty(idProviderName))
			{
				return Regex.Match(idProviderName, "^oidc-[a-z0-9-_\\.]{1,15}$").Success;
			}
			return false;
		}

		private UsernamePasswordRequest BuildUsernamePasswordRequest(string username, string password)
		{
			if (!ValidateCredentials(username, password))
			{
				throw ExceptionHandler.BuildInvalidCredentialsException();
			}
			return new UsernamePasswordRequest
			{
				Username = username,
				Password = password
			};
		}

		private bool ValidateCredentials(string username, string password)
		{
			if (!string.IsNullOrEmpty(username))
			{
				return !string.IsNullOrEmpty(password);
			}
			return false;
		}
	}
}
