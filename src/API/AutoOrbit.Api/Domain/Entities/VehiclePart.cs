namespace AutoOrbit.Api.Domain.Entities;

public class VehiclePart
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public required string PartName { get; set; }
    public DateTime DateAdded { get; set; }

    public Vehicle? Vehicle { get; set; }
}
