using FluentValidation;

namespace AutoOrbit.Api.Application.Vehicles.Add;

public class AddVehicleCommand
{
    public string? VIN { get; set; }
    public int? Year { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? Trim { get; set; }
}

public class AddVehicleCommandValidator : AbstractValidator<AddVehicleCommand>
{
    public AddVehicleCommandValidator()
    {
        RuleFor(v => v.VIN).NotEmpty();

        RuleFor(v => v.Year).NotEmpty();

        RuleFor(v => v.Make).NotEmpty();

        RuleFor(v => v.Model).NotEmpty();

        RuleFor(v => v.Trim).NotEmpty();
    }
}