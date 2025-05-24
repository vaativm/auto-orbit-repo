using AutoOrbit.Api.Application.Abstractions;
using AutoOrbit.Api.Application.Profiles.Create;
using AutoOrbit.Api.Application.Profiles.Edit;
using AutoOrbit.Api.Domain.Entities;
using AutoOrbit.Api.Extensions;
using AutoOrbit.Api.Shared.Models;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;

namespace AutoOrbit.Api.Application.Services;

public class UserProfilesService(
    IAutoOrbitDbContext autoOrbitDbContext,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IOptionsMonitor<ImageLocationOptions> options) : IUserProfilesService
{
    public async Task<ApiBaseResponse> CreateUserProfileAsync(
        AddProfileCommand addProfileCommand)
    {
        ApiBaseResponse? imageUploadResult = null;
        
        if(addProfileCommand.ProfilePicture is not null)
        {
            imageUploadResult = await UploadImageAsync(addProfileCommand.ProfilePicture);

            if (!imageUploadResult.IsSuccess)
            {
                return imageUploadResult;
            }
        }

        EntityEntry<UserProfile> userProfileCreated = await autoOrbitDbContext.UserProfiles.AddAsync(new UserProfile
        {
            UserId = currentUser.UserId!,
            Email = currentUser.Email,
            FirstName = addProfileCommand.FirstName,
            LastName = addProfileCommand.LastName,
            Phone = addProfileCommand.Phone,
            ProfilePictureUrl = imageUploadResult is null ? "" : imageUploadResult.GetResult<string>(),
            DateAdded = dateTimeProvider.UtcNow
        });
  
        await autoOrbitDbContext.SaveChangesAsync(default);

        return new ApiOkResponse<UserProfile>(userProfileCreated.Entity);
    }

    public async Task<ApiBaseResponse> EditUserProfileAsync(
        UpdateProfileCommand updateProfileCommand)
    {
        ApiBaseResponse? imageUploadResult = null;

        UserProfile userProfileToUpdate = await autoOrbitDbContext.UserProfiles
           .FirstOrDefaultAsync(p => p.UserProfileId == updateProfileCommand.UserProfileId);

        if(userProfileToUpdate is null)
        {
            return new ApiNotFoundResponse("Profile not found");
        }

        userProfileToUpdate.FirstName = updateProfileCommand.FirstName;
        userProfileToUpdate.LastName = updateProfileCommand.LastName;
        userProfileToUpdate.Phone = updateProfileCommand.Phone;

        if (updateProfileCommand.ProfilePicture is not null)
        {
            
            imageUploadResult = await UploadImageAsync(updateProfileCommand.ProfilePicture);

            if (!imageUploadResult.IsSuccess)
            {
                return imageUploadResult;
            }

            if (!string.IsNullOrWhiteSpace(userProfileToUpdate.ProfilePictureUrl))
            {
                DeleteImage(userProfileToUpdate.ProfilePictureUrl);
            }

            userProfileToUpdate.ProfilePictureUrl = imageUploadResult.GetResult<string>();
        }


        await autoOrbitDbContext.SaveChangesAsync(default);

        return new ApiOkResponse<UserProfile>(userProfileToUpdate);
    }

    public async Task<ApiBaseResponse> GetUserProfileAsync(string userId)
    {
        UserProfile? profile = await autoOrbitDbContext.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile is null)
        {
            return new ApiNotFoundResponse($"Profile with {userId}: not found");
        }

        var result = new ApiOkResponse<UserProfile>(profile);

        return result;
    }

    public async Task<ApiBaseResponse> GetUserProfileByEmailAsync(string email)
    {
        UserProfile? profile = await autoOrbitDbContext.UserProfiles
            .FirstOrDefaultAsync(p => p.Email == email);

        if (profile is null)
        {
            return new ApiNotFoundResponse($"Profile with email {email}: not found");
        }
            

        var result = new ApiOkResponse<UserProfile>(profile);

        return result;
    }

    public async Task<ApiBaseResponse> GetUsersProfilesAsync()
    {
        List<UserProfile> userProfiles = await autoOrbitDbContext.UserProfiles
            .AsNoTracking()
            .ToListAsync();

        return new ApiOkResponse<List<UserProfile>>(userProfiles);
    }

    public async Task<ApiBaseResponse> UpdateProfilePictureAsync(
        string userId, 
        IFormFile profilePicture)
    {
        UserProfile? profile = autoOrbitDbContext.UserProfiles.FirstOrDefault(p => p.UserId == userId);
        
        if (profile is null)
        {
            return new ApiNotFoundResponse($"User with id {userId} not found");
        }

        ApiBaseResponse imageUploadResult = await UploadImageAsync(profilePicture);

        if (!string.IsNullOrWhiteSpace(profile.ProfilePictureUrl))
        {
            DeleteImage(profile.ProfilePictureUrl);
        }

        profile.ProfilePictureUrl = imageUploadResult.GetResult<string>();

        await autoOrbitDbContext.SaveChangesAsync(default);

        return new ApiOkResponse<UserProfile>(profile);
    }

    public async Task<ApiBaseResponse> UploadImageAsync(
        IFormFile profilePicture)
    {
        string path = Path.Combine(options.CurrentValue.Path);

        string fileSaveName = Guid.NewGuid().ToString("N") + Path.GetExtension(profilePicture.FileName);

        string filePath = Path.Combine(path, fileSaveName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await profilePicture.CopyToAsync(stream);
        }

        return new ApiOkResponse<string>(fileSaveName);
    }

    private  bool DeleteImage(string imageName)
    {
        bool isSuccess = false;
        string path = Path.Combine(options.CurrentValue.Path);

        string filePath = Path.Combine(path, imageName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);

            isSuccess = true;
        }

        return isSuccess;
    }
}
