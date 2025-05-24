using AutoOrbit.Api.Application.Abstractions;
using AutoOrbit.Api.Extensions;
using AutoOrbit.Api.Shared.Models;
using AutoOrbit.Api.Shared.RestClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AutoOrbit.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(
    IFirebaseAuthService firebaseAuthService, 
    IFirebaseApi firebaseApi,
    IOptionsMonitor<FirebaseOptions> firebaseOptions) : ApiControllerBase
{
    [HttpPost]
    [Route("signup")]
    public async Task<IActionResult> SignUp(string email, string password, Role role)
    {
        ApiBaseResponse baseResult = await firebaseAuthService.SignUpAsync(email, password, role);

        if (!baseResult.IsSuccess)
        {
            return ProcessError(baseResult);
        }

        TokenResponse token = baseResult.GetResult<TokenResponse>();

        return Ok(token);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        ApiBaseResponse baseResult = await firebaseAuthService.LoginAsync(email, password);

        if (!baseResult.IsSuccess)
        {
            return ProcessError(baseResult);
        }

        TokenResponse token = baseResult.GetResult<TokenResponse>();

        return Ok(token);
    }

    [HttpPost]
    [Route("reset-password")]
    [Authorize]
    public async Task<IActionResult> ResetPassword(string email)
    {
        await firebaseAuthService.ResetPasswordAsync(email);

        return Ok();
    }

    [HttpPost]
    [Route("signout")]
    [Authorize]
    public IActionResult LogOut()
    {
        firebaseAuthService.SignOut();

        return Ok();
    }

    [HttpPost]
    [Route("refresh-token")]
    [Authorize]
    public async Task<IActionResult> GetToken(TokenRequest tokenRequest)
    {
        FirebaseOptions options = firebaseOptions.CurrentValue;

        TokenResponse response = await firebaseApi.GetToken(tokenRequest, options?.ApiKey!);

        return Ok(response);
    }

}

