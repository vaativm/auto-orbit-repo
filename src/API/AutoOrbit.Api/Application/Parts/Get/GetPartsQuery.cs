using AutoOrbit.Api.Shared.Models;

namespace AutoOrbit.Api.Application.Parts.Get;

public class GetPartsQuery : QueryParameters
{
    public string? PartName { get; set; }
    public DateTime? DateAdded { get; set; }
    public GetPartsQuery() => OrderBy = "dateadded";
    public string? SearchTearm { get; set; }
}
