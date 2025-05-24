using Refit;

namespace AutoOrbit.Api.Shared.Models;

public class TokenResponse
{
    [AliasAs("id_token")]
    public string? IdToken { get; set; }

    [AliasAs("user_id")]
    public string? UserId { get; set; }

    [AliasAs("refresh_token")]
    public string? RefreshToken { get; set; }

    [AliasAs("expires_in")]
    public string ExpiresIn { get; set; }
}
