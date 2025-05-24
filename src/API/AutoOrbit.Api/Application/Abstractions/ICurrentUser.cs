
namespace AutoOrbit.Api.Application.Abstractions;

public interface ICurrentUser
{
    string UserId { get; }
    string Email { get; }
    Task<bool> IsAdmin();
    Task<bool> IsUser();
}
