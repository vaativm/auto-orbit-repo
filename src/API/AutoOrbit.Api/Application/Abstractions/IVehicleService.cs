using AutoOrbit.Api.Application.Vehicles.Add;
using AutoOrbit.Api.Application.Vehicles.Get;
using AutoOrbit.Api.Shared.Models;

namespace AutoOrbit.Api.Application.Abstractions;

public interface IVehicleService
{
    Task<ApiBaseResponse> AddVehicleAsync(AddVehicleCommand addVehicleCommand);
    Task<ApiBaseResponse> GetRecentlyAddedVehiclesAsync();
    Task<ApiBaseResponse> GetVehicleAsync(int id);
    Task<ApiBaseResponse> GetVehiclesAsync(GetVehiclesQuery query);
}
