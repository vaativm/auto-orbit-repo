using AutoOrbit.Api.Shared.Models;
using Refit;

namespace AutoOrbit.Api.Shared.RestClients;

public interface IFirebaseApi
{
    [Post("/token")]
    Task<TokenResponse> GetToken([Body(BodySerializationMethod.UrlEncoded)] TokenRequest tokenRequest, [Query]string key);
}
