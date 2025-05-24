using AutoOrbit.Api.Application.Abstractions;
using AutoOrbit.Api.Application.Profiles.Create;
using AutoOrbit.Api.Application.Profiles.Edit;
using AutoOrbit.Api.Domain.Entities;
using AutoOrbit.Api.Extensions;
using AutoOrbit.Api.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoOrbit.Api.Controllers;

[Route("api/profiles")]
[ApiController]
[Authorize]
public class ProfilesController(IUserProfilesService userProfilesService) : ApiControllerBase
{

    [HttpGet]
    [ProducesResponseType(typeof(List<UserProfile>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetProfiles()
    {
        ApiBaseResponse usersProfilesResult = await userProfilesService.GetUsersProfilesAsync();
        List<UserProfile> usersProfiles = usersProfilesResult.GetResult<List<UserProfile>>();

        return Ok(usersProfiles);
    }

    [HttpGet("{userId}", Name = "ProfileByUserId")]
    [ProducesResponseType(typeof(UserProfile), 200)]
    [ProducesResponseType(typeof(NotFoundResult), 404)]
    [ProducesResponseType(401)]

    public async Task<IActionResult> GetProfileBy([FromRoute] string userId)
    {
        ApiBaseResponse userProfileResult = await userProfilesService.GetUserProfileAsync(userId);
        UserProfile userProfile = userProfileResult.GetResult<UserProfile>();

        return Ok(userProfile);
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> CreateProfile(AddProfileCommand addProfileCommand)
    {
        ApiBaseResponse createUserProfileResult = await userProfilesService.CreateUserProfileAsync(addProfileCommand);

        if (!createUserProfileResult.IsSuccess)
        {
            return ProcessError(createUserProfileResult);
        }
           

        UserProfile createdProfile = createUserProfileResult.GetResult<UserProfile>();

        return CreatedAtRoute("ProfileByUserId", new { userId = createdProfile.UserId }, createdProfile);
    }

    [HttpPatch("{userId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> UpdateProfilePicture(string userId, IFormFile profilePicture)
    {
        ApiBaseResponse updatePictureResult = await userProfilesService.UpdateProfilePictureAsync(userId, profilePicture);

        if (!updatePictureResult.IsSuccess)
        {
            return ProcessError(updatePictureResult);
        }
            
        return NoContent();
    }

    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> UpdateProfile(UpdateProfileCommand updateProfileCommand)
    {
        ApiBaseResponse updateProfileResult = await userProfilesService.EditUserProfileAsync(updateProfileCommand);

        if (!updateProfileResult.IsSuccess)
        {
            return ProcessError(updateProfileResult);
        }

        return NoContent();
    }
}
