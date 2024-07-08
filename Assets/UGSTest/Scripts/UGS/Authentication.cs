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
        public event Action OnSignInFailed;
        public event Action OnSignedOut;
        public event Action OnSessionExpired;

        public async void InitializeUnityServices()
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

        public async Task<string> GetPlayerNameAsync()
        {
            string playerName = string.Empty;
            try
            {
                playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
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
            return playerName;
        }

        public async void SetPlayerNameAsync(string _playerName)
        {
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(_playerName);
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

        public bool IsSignInCached()
        {
            // Check if a cached player already exists by checking if the session token exists
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                return true;
            }
            return false;
        }

        public async void SignUpAsync(string userName, string playername, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(userName, password);
                SetPlayerNameAsync(playername);
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

        public async void SignInAnonymously()
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

        public void Signout()
        {
            AuthenticationService.Instance.SignOut();
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

            AuthenticationService.Instance.SignedOut -= ()=> HandleSignOutEvent();

            AuthenticationService.Instance.Expired -= () => HandleSessionExpireEvent();
        }

        private void HandleSignInSuccessEvent()
        {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

            OnSignedInEvent?.Invoke();
        }
        
        private void HandleSigninFailedEvent(RequestFailedException err)
        {
            Debug.LogError(err);
        }
        private void HandleSignOutEvent()
        {
            Debug.Log("Player signed out.");
        }
        private void HandleSessionExpireEvent()
        {
            Debug.Log("Player session could not be refreshed and expired.");
        }
        #endregion
    }
}

