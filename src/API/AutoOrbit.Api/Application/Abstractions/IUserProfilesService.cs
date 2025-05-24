using AutoOrbit.Api.Application.Profiles.Create;
using AutoOrbit.Api.Application.Profiles.Edit;
using AutoOrbit.Api.Shared.Models;

namespace AutoOrbit.Api.Application.Abstractions;

public interface IUserProfilesService
{
    Task<ApiBaseResponse> GetUserProfileAsync(string userId);
    Task<ApiBaseResponse> GetUsersProfilesAsync();
    Task<ApiBaseResponse> CreateUserProfileAsync(AddProfileCommand addProfileCommand);
    Task<ApiBaseResponse> UpdateProfilePictureAsync(string userId, IFormFile profilePicture);
    Task<ApiBaseResponse> UploadImageAsync(IFormFile profilePicture);
    Task<ApiBaseResponse> GetUserProfileByEmailAsync(string email);
    Task<ApiBaseResponse> EditUserProfileAsync(UpdateProfileCommand updateProfileCommand);
}
