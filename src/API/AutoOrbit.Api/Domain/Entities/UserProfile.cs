namespace AutoOrbit.Api.Domain.Entities;

public class UserProfile
{
    public int UserProfileId { get; set; }
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime DateAdded { get; set; }

}
