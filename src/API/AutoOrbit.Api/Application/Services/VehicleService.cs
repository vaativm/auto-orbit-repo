using AutoOrbit.Api.Application.Abstractions;
using AutoOrbit.Api.Application.Extensions;
using AutoOrbit.Api.Application.Vehicles.Add;
using AutoOrbit.Api.Application.Vehicles.Get;
using AutoOrbit.Api.Domain.Entities;
using AutoOrbit.Api.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AutoOrbit.Api.Application.Services;

public class VehicleService(
    IAutoOrbitDbContext autoOrbitDbContext,
    IDateTimeProvider dateTimeProvider) : IVehicleService
{
    public async Task<ApiBaseResponse> AddVehicleAsync(AddVehicleCommand addVehicleCommand)
    {
        EntityEntry<Vehicle> addedVehicle = await autoOrbitDbContext.Vehicles.AddAsync(new Vehicle
        {
            VIN = addVehicleCommand.VIN!,
            Year = addVehicleCommand.Year!.Value,
            Make = addVehicleCommand.Make!,
            Model = addVehicleCommand.Model!,
            Trim = addVehicleCommand.Trim!,
            DateAdded = dateTimeProvider.UtcNow
        });

        await autoOrbitDbContext.SaveChangesAsync(default);

        return new ApiOkResponse<Vehicle>(addedVehicle.Entity);
    }

    public async Task<ApiBaseResponse> GetRecentlyAddedVehiclesAsync()
    {
        List<Vehicle> recentVehicles = await autoOrbitDbContext.Vehicles
            .OrderByDescending(v => v.DateAdded)
            .Take(10)
            .AsNoTracking()
            .ToListAsync();

        return new ApiOkResponse<List<Vehicle>>(recentVehicles);
    }

    public async Task<ApiBaseResponse> GetVehicleAsync(int id)
    {
        Vehicle? vehicle = await autoOrbitDbContext.Vehicles.FirstOrDefaultAsync(p => p.Id == id);

        if (vehicle is null)
        {
            return new ApiNotFoundResponse($"Vehicle with id {id} not found");
        }
            

        return new ApiOkResponse<Vehicle>(vehicle);
    }

    public async Task<ApiBaseResponse> GetVehiclesAsync(GetVehiclesQuery query)
    {
        int totalVehicleCount = await autoOrbitDbContext.Vehicles.CountAsync();

        IQueryable<Vehicle> vehiclesQueryable = autoOrbitDbContext.Vehicles.AsQueryable();
        List<Vehicle> vehicles = await vehiclesQueryable
            .FilterVehicles(query)
            .SearchVehicles(query)
            .Sort(query?.OrderBy)
            .AsNoTracking()
            .OrderByDescending(v => v.DateAdded)
            .Skip((query!.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var pagedList = new PagedList<Vehicle>(vehicles, totalVehicleCount, query.PageNumber, query.PageSize);

        return new ApiOkResponse<PagedList<Vehicle>>(pagedList);
    }
}
