using FluentValidation;
using System.Xml.Linq;

namespace AutoOrbit.Api.Application.Profiles.Create;

public class AddProfileCommand
{
    public string? Phone { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}

public class CreateProfileCommandValidator : AbstractValidator<AddProfileCommand>
{
    public CreateProfileCommandValidator()
    {
        //RuleFor(p => p.UserId)
        //    .NotEmpty()
        //    .WithMessage("User id is required");

        //RuleFor(p => p.Email)
        //    .NotEmpty()
        //    .WithMessage("Email address is required")
        //    .EmailAddress()
        //    .WithMessage("Email address is not in the correct format");

        RuleFor(p => p.ProfilePicture)
            .Must((profile, context) => profile.ProfilePicture!.ContentType == "image/png")
            .When(p => p.ProfilePicture != null)
            .WithMessage("Content type is not valid");

        RuleFor(x => x).Must((profile, context) =>
        {
            // string representation of hexadecimal signature of an execute file
            var exeSignatures = new List<string> {
                "4D-5A",
                "5A 4D"
            };

            var binary = new BinaryReader(profile.ProfilePicture!.OpenReadStream());
            byte[] bytes = binary.ReadBytes(2); // reading first two bytes
            string fileSequenceHex = BitConverter.ToString(bytes);

            foreach (string exeSignature in exeSignatures)
            {
                if (exeSignature.Equals(fileSequenceHex, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
                
            return true;
        })
            .WithName("FileContent")
            .WithMessage("The file content is not valid")
            .When(p => p.ProfilePicture != null);

        RuleFor(p => p.ProfilePicture!.FileName)
            .Matches("^[A-Za-z0-9_\\-.]*$")
            .WithMessage("The file name is not valid")
            .When(p => p.ProfilePicture != null);
    }
}
