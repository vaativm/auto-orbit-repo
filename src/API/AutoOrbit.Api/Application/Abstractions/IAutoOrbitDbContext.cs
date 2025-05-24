using AutoOrbit.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoOrbit.Api.Application.Abstractions;

public interface IAutoOrbitDbContext
{
    DbSet<UserProfile> UserProfiles { get; set; }
    DbSet<Vehicle> Vehicles { get; set; }
    DbSet<VehiclePart> VehicleParts { get; set; }
    DbSet<Organization> Organizations { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}