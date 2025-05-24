using AutoOrbit.Api.Application.Abstractions;
using AutoOrbit.Api.Domain.Entities;
using AutoOrbit.Api.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoOrbit.Api.Application.Services;

public class OrganizationService(IAutoOrbitDbContext autoOrbitDbContext) : IOrganizationService
{
    public async Task<ApiBaseResponse> GetOrganizationAsync()
    {
        Organization? organization = await autoOrbitDbContext.Organizations.FirstOrDefaultAsync();

        if (organization is null)
        {
            return new ApiNotFoundResponse("Organization not found");
        }

        return new ApiOkResponse<Organization>(organization);
    }
}
