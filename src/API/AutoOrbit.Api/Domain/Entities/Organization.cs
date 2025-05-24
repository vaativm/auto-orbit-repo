namespace AutoOrbit.Api.Domain.Entities;

public class Organization
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? LogoUrl { get; set; }
}
