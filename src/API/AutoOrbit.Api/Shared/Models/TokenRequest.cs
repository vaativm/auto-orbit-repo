using Refit;

namespace AutoOrbit.Api.Shared.Models;

public class TokenRequest
{
    [AliasAs("grant_type")]
    public string GrantType => "refresh_token";

    [AliasAs("refresh_token")]
    public string? RefreshToken { get; set; }
}
