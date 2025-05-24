namespace AutoOrbit.Api.Shared.Models;

public class FirebaseOptions
{
    public static string SectionName = "Firebase";
    public string? BaseUrl { get; set; }
    public string? ProjectId { get; set; }
    public string? ApiKey { get; set; }
    public string? AuthDomain { get; set; }
    public string? Authority { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string? CredentialsFilePath { get; set; }
}
