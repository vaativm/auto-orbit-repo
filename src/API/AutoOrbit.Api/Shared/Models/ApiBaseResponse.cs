namespace AutoOrbit.Api.Shared.Models;

public abstract class ApiBaseResponse
{
    public bool IsSuccess { get; set; }

    protected ApiBaseResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }
}
