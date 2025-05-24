using AutoOrbit.Api.Application.Abstractions;
using System.Security.Claims;
using FirebaseAdmin.Auth;

namespace AutoOrbit.Api.Shared.Models;

public class CurrentUser(IHttpContextAccessor context) : ICurrentUser
{
    public string UserId  => context.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == "user_id")!.Value;
    public string Email => context.HttpContext!.User.Claims.First(x => x.Type == ClaimTypes.Email).Value;
    public async Task<bool> IsAdmin()
    {
        string? token = GetToken();
        
        if(token is null)
        {
            return false;
        }

        FirebaseToken decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

        if (decoded.Claims.TryGetValue("admin", out object isAdmin))
        {
            if ((bool)isAdmin)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<bool> IsUser()
    {
        string? token = GetToken();

        if (token is null)
        {
            return false;
        }

        FirebaseToken decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

        if (decoded.Claims.TryGetValue("user", out object isUser))
        {
            if ((bool)isUser)
            {
                return true;
            }
        }

        return false;
    }

    private string? GetToken()
    {
        string? token = null;
        IHeaderDictionary headers = context!.HttpContext!.Request.Headers;

        if (headers.ContainsKey("Authorization"))
        {
            string authorizationHeader = headers["Authorization"];

            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                string[] authHeaderParts = authorizationHeader.Split(' ');

                if(authHeaderParts.Length==2 && authHeaderParts[0].ToLower() == "bearer")
                {
                    token = authHeaderParts[1];

                    return token;
                }
            }
        }

        return token;
    }
}
