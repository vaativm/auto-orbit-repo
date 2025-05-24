using AutoOrbit.Api.Application.Abstractions;
using AutoOrbit.Api.Application.Parts.Add;
using AutoOrbit.Api.Application.Parts.Get;
using AutoOrbit.Api.Domain.Entities;
using AutoOrbit.Api.Extensions;
using AutoOrbit.Api.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AutoOrbit.Api.Controllers;

[Route("api/parts")]
[ApiController]
[Authorize]
public class PartsController(IPartsService partsService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetParts([FromQuery] GetPartsQuery query)
    {
        var getPartsResult = await partsService.GetVehiclePartsAsync(query);
        var pagedVehiclePartsList = getPartsResult.GetResult<PagedList<VehiclePart>>();

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedVehiclePartsList.PagingMetaData));

        return Ok(pagedVehiclePartsList);
    }

    [HttpGet("{id}", Name = "VehiclePartById")]
    public async Task<IActionResult> VehiclePartById(int id)
    {
        var partResult = await partsService.GetVehiclePartAsync(id);

        if (!partResult.IsSuccess)
            return ProcessError(partResult);

        return Ok(partResult.GetResult<VehiclePart>());
    }

    [HttpPost]
    public async Task<IActionResult> AddPart(AddVehiclePartCommand addVehiclePartCommand)
    {
        var addedPartResult = await partsService.AddVehiclePartAsync(addVehiclePartCommand);
        var addedPart = addedPartResult.GetResult<VehiclePart>();

        return CreatedAtRoute("VehiclePartById", new { id=addedPart.Id }, addedPart);
    }
}
