using AutoOrbit.Api.Shared.Models;

namespace AutoOrbit.Api.Application.Abstractions;

public interface IOrganizationService
{
    Task<ApiBaseResponse> GetOrganizationAsync();
}
