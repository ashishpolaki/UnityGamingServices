using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using System;

namespace UGS
{
    public class Authentication
    {
        public event Action OnSignedInEvent;
        public event Action<string> OnSignInFailed;
        public event Action OnSignedOut;
        public event Action OnSessionExpired;
        public event Action<string> OnPlayerNameChanged;

        public string PlayerID { get; private set; }
        public string PlayerName { get; private set; }

        public Authentication()
        {
            InitializeUnityServices();
        }

        #region Initialize
        private async void InitializeUnityServices()
        {
            try
            {
                await UnityServices.InitializeAsync();
                SubscribeEvents();
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }
        #endregion

        #region PlayerName
        public async Task GenerateRandomPlayerName()
        {
            try
            {
                PlayerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            OnPlayerNameChanged?.Invoke(PlayerName);
        }

        public async Task SetPlayerNameAsync(string _playerName)
        {
            try
            {
                PlayerName = await AuthenticationService.Instance.UpdatePlayerNameAsync(_playerName);
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            OnPlayerNameChanged?.Invoke(PlayerName);
        }
        #endregion

        public bool IsSignInCached()
        {
            // Check if a cached player already exists by checking if the session token exists
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                return true;
            }
            return false;
        }

        public async Task SignUpAsync(string userName, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(userName, password);
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                // Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }

        public async Task SignInWithUsernamePasswordAsync(string username, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.Log(ex.Message);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.Log(ex.Message);
            }
        }

        public async Task SignInAnonymouslyAsync()
        {
            //if session token is available, sign out the player
            if (IsSignInCached())
            {
                AuthenticationService.Instance.ClearSessionToken();
            }

            //Random signin for anonymous user with random username
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }

        public async Task CacheSignInAsync()
        {
            //Random signin for anonymous user with random username
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }

        public void Signout()
        {
            AuthenticationService.Instance.SignOut(true);
            AuthenticationService.Instance.ClearSessionToken();
        }

        #region Subscribe/Desubscribe Methods

        private void SubscribeEvents()
        {
            AuthenticationService.Instance.SignedIn += () => HandleSignInSuccessEvent();

            AuthenticationService.Instance.SignInFailed += (err) => HandleSigninFailedEvent(err);

            AuthenticationService.Instance.SignedOut += () => HandleSignOutEvent();

            AuthenticationService.Instance.Expired += () => HandleSessionExpireEvent();
        }

        public void DeSubscribeEvents()
        {
            AuthenticationService.Instance.SignedIn -= () => HandleSignInSuccessEvent();

            AuthenticationService.Instance.SignInFailed -= (err) => HandleSigninFailedEvent(err);

            AuthenticationService.Instance.SignedOut -= () => HandleSignOutEvent();

            AuthenticationService.Instance.Expired -= () => HandleSessionExpireEvent();
        }

        private async void HandleSignInSuccessEvent()
        {
            PlayerID = AuthenticationService.Instance.PlayerId;
            PlayerName = await AuthenticationService.Instance.GetPlayerNameAsync(false);
            OnSignedInEvent?.Invoke();
        }

        private void HandleSigninFailedEvent(RequestFailedException err)
        {
            OnSignInFailed?.Invoke(err.Message);
        }
        private void HandleSignOutEvent()
        {
            OnSignedOut?.Invoke();
            Debug.Log("Player signed out.");
        }
        private void HandleSessionExpireEvent()
        {
            Debug.Log("Player session could not be refreshed and expired.");
        }
        #endregion
    }
}

