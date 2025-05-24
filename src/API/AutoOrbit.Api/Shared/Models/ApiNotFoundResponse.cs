namespace AutoOrbit.Api.Shared.Models;

public class ApiNotFoundResponse : ApiBaseResponse
{
    public ApiNotFoundResponse(string message)
        : base(false)
    {
        Message = message;
    }

    public string Message { get; set; }
}
