using AutoOrbit.Api.Application.Parts.Get;
using AutoOrbit.Api.Domain.Entities;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;

namespace AutoOrbit.Api.Application.Extensions;

public static class PartsServiceExtensions
{
    public static IQueryable<VehiclePart> FilterVehicleParts(this IQueryable<VehiclePart> vehicleParts, GetPartsQuery query)
    {
        vehicleParts = query.PartName is null ? vehicleParts : vehicleParts.Where(v => v.PartName == query.PartName);
        vehicleParts = query.DateAdded is null ? vehicleParts : vehicleParts.Where(v => v.DateAdded == query.DateAdded);

        return vehicleParts;
    }

    public static IQueryable<VehiclePart> SearchVehicleParts(this IQueryable<VehiclePart> vehicleParts, GetPartsQuery query)
    {
        if (query.SearchTearm is null)
            return vehicleParts;

        var lowerCaseTerm = query.SearchTearm.Trim().ToLower();

        return vehicleParts.Where(p => p.PartName.ToLower().Contains(lowerCaseTerm));
    }

    public static IQueryable<VehiclePart> Sort(this IQueryable<VehiclePart> vehicleParts, string? orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return vehicleParts.OrderByDescending(v => v.DateAdded);

        var orderByParams = orderByQueryString.Trim().Split(",");
        var propertiesInfo = typeof(VehiclePart).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var orderByStrBuilder = new StringBuilder();

        foreach (var param in orderByParams)
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
            return vehicleParts.OrderByDescending(v => v.DateAdded);

        return vehicleParts.OrderBy(orderByQuery);
    }
}
