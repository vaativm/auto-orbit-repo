using FluentValidation;

namespace AutoOrbit.Api.Application.Parts.Add;

public class AddVehiclePartCommand
{
    public int? VehicleId { get; set; }
    public string? PartName { get; set; }
}

public class AddVehiclePartCommandValidator : AbstractValidator<AddVehiclePartCommand>
{
    public AddVehiclePartCommandValidator()
    {
        RuleFor(p => p.VehicleId).NotEmpty();

        RuleFor(p => p.PartName).NotEmpty();
    }
}