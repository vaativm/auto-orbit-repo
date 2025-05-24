namespace AutoOrbit.Api.Application.Profiles.Edit;

public class UpdateProfileCommand
{
    public int? UserProfileId { get; set; }
    public string? Phone { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}
