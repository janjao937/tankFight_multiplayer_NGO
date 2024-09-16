using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> GetAuth(int maxTries = 5)
    {
        if (AuthState == AuthState.Authenticating)
        {
            return AuthState;
        }
        if(AuthState == AuthState.Authenticating){
            Debug.LogWarning("Already Authenticating");
            await Authenticating();
            return AuthState;
        }
        await SignInAnonymouslyAsync(maxTries);

        return AuthState;
    }
    private static async Task<AuthState> Authenticating(){
        while(AuthState == AuthState.Authenticating|| AuthState ==AuthState.NotAuthenticated){
            await Task.Delay(200);
        }
        return AuthState;
    }
    private static async Task SignInAnonymouslyAsync(int maxRetries)
    {

        AuthState = AuthState.Authenticating;
        int retries = 0;
        while (AuthState == AuthState.Authenticating && retries < maxRetries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException auEx)
            {
                Debug.LogError(auEx);
                AuthState = AuthState.Error;
            }
            catch(RequestFailedException rfEx){
                Debug.LogError(rfEx);
                AuthState = AuthState.Error;
            }

            retries++;
            await Task.Delay(1000);//fail => delay1000ms =>again
        }
        if(AuthState!=AuthState.Authenticated){
            Debug.LogWarning($"Player was not signed in successfully after {retries} re-tries");
            AuthState = AuthState.TimeOut;
        }
    }
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut,
}
