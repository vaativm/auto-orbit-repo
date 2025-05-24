using AutoOrbit.Api.Application.Abstractions;
using AutoOrbit.Api.Application.Extensions;
using AutoOrbit.Api.Application.Parts.Add;
using AutoOrbit.Api.Application.Parts.Get;
using AutoOrbit.Api.Domain.Entities;
using AutoOrbit.Api.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AutoOrbit.Api.Application.Services;

public class PartsService(
    IAutoOrbitDbContext autoOrbitDbContext,
    IDateTimeProvider dateTimeProvider) : IPartsService
{
    public async Task<ApiBaseResponse> AddVehiclePartAsync(AddVehiclePartCommand addVehiclePartCommand)
    {
        EntityEntry<VehiclePart> partAdded = await autoOrbitDbContext.VehicleParts.AddAsync(new VehiclePart
        {
            VehicleId = addVehiclePartCommand.VehicleId!.Value,
            PartName = addVehiclePartCommand.PartName!,
            DateAdded = dateTimeProvider.UtcNow
        });

        await autoOrbitDbContext.SaveChangesAsync(default);

        return new ApiOkResponse<VehiclePart>(partAdded.Entity);
    }

    public async Task<ApiBaseResponse> GetVehiclePartAsync(int id)
    {
        VehiclePart? vehiclePart = await autoOrbitDbContext.VehicleParts.FirstOrDefaultAsync(p => p.Id == id);

        if (vehiclePart is null)
        {
            return new ApiNotFoundResponse($"Part with id {id} not found");
        }
            

        return new ApiOkResponse<VehiclePart>(vehiclePart);
    }

    public async Task<ApiBaseResponse> GetVehiclePartsAsync(GetPartsQuery query)
    {
        int totalVehiclePartsCount = await autoOrbitDbContext.Vehicles.CountAsync();

        IQueryable<VehiclePart> partsQueryable = autoOrbitDbContext.VehicleParts.AsQueryable();

        List<VehiclePart> parts = await partsQueryable
            .FilterVehicleParts(query)
            .SearchVehicleParts(query)
            .Sort(query.OrderBy)
            .Take(20)
            .AsNoTracking()
            .ToListAsync();

        var pagedList = new PagedList<VehiclePart>(parts, totalVehiclePartsCount, query.PageNumber, query.PageSize);

        return new ApiOkResponse<PagedList<VehiclePart>>(pagedList);
    }
}
