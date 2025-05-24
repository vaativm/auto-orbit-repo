using AutoOrbit.Api.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoOrbit.Api.Controllers;

public class ApiControllerBase : ControllerBase
{
    [HttpGet]
    internal IActionResult ProcessError(ApiBaseResponse baseResponse)
    {
        return baseResponse switch
        {
            ApiNotFoundResponse => NotFound(new ErrorDetails
            {
                Message = ((ApiNotFoundResponse)baseResponse).Message,
                StatusCode = StatusCodes.Status404NotFound
            }),
            _ => throw new NotImplementedException()
        };
    }
}
