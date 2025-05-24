using AutoOrbit.Api.Application.Vehicles.Get;
using AutoOrbit.Api.Domain.Entities;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;

namespace AutoOrbit.Api.Application.Extensions;

public static class VehicleServiceExtensions
{
    public static IQueryable<Vehicle> FilterVehicles(this IQueryable<Vehicle> vehicles, GetVehiclesQuery query)
    {
        vehicles = query.Year is null ? vehicles : vehicles.Where(v => v.Year == query.Year);
        vehicles = query.Make is null ? vehicles : vehicles.Where(v => v.Make == query.Make);
        vehicles = query.Model is null ? vehicles : vehicles.Where(v => v.Model == query.Model);
        vehicles = query.Trim is null ? vehicles : vehicles.Where(v => v.Trim == query.Trim);

        return vehicles;
    }

    public static IQueryable<Vehicle> SearchVehicles(this IQueryable<Vehicle> vehicles, GetVehiclesQuery query)
    {
        if (query.SearchTerm is null)
            return vehicles;

        var lowerCaseTerm = query.SearchTerm.Trim().ToLower();

        return vehicles.Where(v =>
            v.Make!.ToLower().Contains(lowerCaseTerm) ||
            v.Model!.ToLower().Contains(lowerCaseTerm) ||
            v.Trim!.ToLower().Contains(lowerCaseTerm)
        );
    }

    public static IQueryable<Vehicle> Sort(this IQueryable<Vehicle> vehicles, string? orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return vehicles.OrderByDescending(v => v.DateAdded);

        var orderByParams = orderByQueryString.Trim().Split(",");
        var propertiesInfo = typeof(Vehicle).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var orderByStrBuilder = new StringBuilder();

        foreach(var param in orderByParams)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;

            var propertyFromQueryParam = param.Split(" ")[0];
            var objectProperty = propertiesInfo
                .FirstOrDefault(p => p.Name.Equals(propertyFromQueryParam, StringComparison.InvariantCultureIgnoreCase));

            if (objectProperty is null)
                continue;

            var direction = param.EndsWith(" desc") ? "descending" : "ascending";

            orderByStrBuilder.Append($"{objectProperty.Name.ToString()} {direction},");
        }

        var orderByQuery = orderByStrBuilder.ToString().TrimEnd(',', ' ');

        if (string.IsNullOrWhiteSpace(orderByQuery))
            return vehicles.OrderByDescending(v => v.DateAdded);

        return vehicles.OrderBy(orderByQuery);
    }
}
