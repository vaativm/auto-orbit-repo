using AutoOrbit.Api.Shared.Models;

namespace AutoOrbit.Api.Application.Vehicles.Get;

public class GetVehiclesQuery : QueryParameters
{
    public GetVehiclesQuery() => OrderBy = "dateadded";
    public string? SearchTerm { get; set; }
    public int? Year { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? Trim { get; set; }
}
