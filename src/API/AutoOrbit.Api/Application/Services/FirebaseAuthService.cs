using AutoOrbit.Api.Application.Abstractions;
using AutoOrbit.Api.Shared.Models;
using Firebase.Auth;
using FirebaseAdmin.Auth;

namespace AutoOrbit.Api.Application.Services;

public class FirebaseAuthService(FirebaseAuthClient firebaseAuth) : IFirebaseAuthService
{
    public async Task<ApiBaseResponse> SignUpAsync(string email, string password, Role role)
    {
        UserCredential? userCredentials = await firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password);

        if (userCredentials is not null)
        {
        
            string uid = userCredentials.User.Uid;
            
            var claims = new Dictionary<string, object>()
            {
                {role.ToString(), true }
            };

            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);

            var tokenResponse = new TokenResponse
            {
                IdToken = await userCredentials.User.GetIdTokenAsync(),
                UserId = userCredentials.User.Info.Uid,
                RefreshToken = userCredentials.User.Credential.RefreshToken,
                ExpiresIn = userCredentials.User.Credential.ExpiresIn.ToString(),
            };

            return new ApiOkResponse<TokenResponse>(tokenResponse);
        }

        return new ApiNotFoundResponse("User not found");
    }

    public async Task<ApiBaseResponse> LoginAsync(string email, string password)
    {
        UserCredential? userCredentials = await firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);

        if (userCredentials is not null)
        {
            var tokenResponse = new TokenResponse
            {
                IdToken = await userCredentials.User.GetIdTokenAsync(),
                UserId = userCredentials.User.Info.Uid,
                RefreshToken = userCredentials.User.Credential.RefreshToken,
                ExpiresIn = userCredentials.User.Credential.ExpiresIn.ToString(),
            };

            return new ApiOkResponse<TokenResponse>(tokenResponse);
        }

        return new ApiNotFoundResponse("User not found");
    }

    public async Task ResetPasswordAsync(string email)
    {
        await firebaseAuth.ResetEmailPasswordAsync(email);
    }

    public void SignOut() => firebaseAuth.SignOut();

}

