using AutoOrbit.Api.Shared.Models;

namespace AutoOrbit.Api.Extensions;

public static class ApiBaseResponseExtensions
{
    public static TResultType GetResult<TResultType>(this ApiBaseResponse apiBaseResponse)
        => ((ApiOkResponse<TResultType>)apiBaseResponse).Result;
}

