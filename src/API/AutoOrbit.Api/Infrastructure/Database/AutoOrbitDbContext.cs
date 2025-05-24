using AutoOrbit.Api.Application.Abstractions;
using AutoOrbit.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AutoOrbit.Api.Infrastructure.Database;

public class AutoOrbitDbContext: DbContext, IAutoOrbitDbContext
{
    public AutoOrbitDbContext(DbContextOptions<AutoOrbitDbContext> options)
        : base(options)
    {

    }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehiclePart> VehicleParts { get; set; }
    public DbSet<Organization> Organizations { get; set; }
}

public class AutoOrbitDbContextFactory : IDesignTimeDbContextFactory<AutoOrbitDbContext>
{
    public AutoOrbitDbContext CreateDbContext(string[] args)
    {
        string connectionString = "Host=localhost;Port=5432;Database=AutoOrbit;Username=postgres;Password=@dm1n123;Include Error Detail=true";
        var optionsBuilder = new DbContextOptionsBuilder<AutoOrbitDbContext>();
        optionsBuilder
            .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
            .UseSnakeCaseNamingConvention();

        return new AutoOrbitDbContext(optionsBuilder.Options);
    }
}

