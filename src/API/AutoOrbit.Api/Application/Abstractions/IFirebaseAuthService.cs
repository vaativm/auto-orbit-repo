using AutoOrbit.Api.Shared.Models;

namespace AutoOrbit.Api.Application.Abstractions;

public interface IFirebaseAuthService
{
    Task<ApiBaseResponse> SignUpAsync(string email, string password, Role role);
    Task<ApiBaseResponse> LoginAsync(string email, string password);
    Task ResetPasswordAsync(string email);
    void SignOut();
}
