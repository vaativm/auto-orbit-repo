using AutoOrbit.Api.Application.Abstractions;
using AutoOrbit.Api.Domain.Entities;
using AutoOrbit.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoOrbit.Api.Controllers;

[Route("api/organization")]
[ApiController]
[Authorize]
public class OrganizationController(IOrganizationService organizationService) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(Organization), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrganization()
    {
        var organizationResult = await organizationService.GetOrganizationAsync();

        if (!organizationResult.IsSuccess)
            return ProcessError(organizationResult);

        var organization = organizationResult.GetResult<Organization>();

        return Ok(organization);
    }
}
