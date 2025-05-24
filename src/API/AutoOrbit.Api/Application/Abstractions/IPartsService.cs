using AutoOrbit.Api.Application.Parts.Add;
using AutoOrbit.Api.Application.Parts.Get;
using AutoOrbit.Api.Shared.Models;

namespace AutoOrbit.Api.Application.Abstractions;

public interface IPartsService
{
    Task<ApiBaseResponse> AddVehiclePartAsync(AddVehiclePartCommand addVehiclePartCommand);
    Task<ApiBaseResponse> GetVehiclePartsAsync(GetPartsQuery query);
    Task<ApiBaseResponse> GetVehiclePartAsync(int id);
}
