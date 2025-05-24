using AutoOrbit.Api.Application.Abstractions;
using AutoOrbit.Api.Application.Vehicles.Add;
using AutoOrbit.Api.Application.Vehicles.Get;
using AutoOrbit.Api.Domain.Entities;
using AutoOrbit.Api.Extensions;
using AutoOrbit.Api.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AutoOrbit.Api.Controllers;

[Route("api/vehicles")]
[ApiController]
[Authorize]
public class VehiclesController(IVehicleService vehicleService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetVehicles([FromQuery] GetVehiclesQuery query)
    {
        var getVehiclesResult = await vehicleService.GetVehiclesAsync(query);
        var pagedVehicleList = getVehiclesResult.GetResult<PagedList<Vehicle>>();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedVehicleList.PagingMetaData));

        return Ok(pagedVehicleList);
    }

    [HttpGet("recent")]
    public async Task<IActionResult> RecentlyAddedVehicles()
    {
        var recentVehiclesResult = await vehicleService.GetRecentlyAddedVehiclesAsync();

        return Ok(recentVehiclesResult.GetResult<List<Vehicle>>());
    }

    [HttpGet("{id}", Name = "VehicleById")]
    public async Task<IActionResult> VehiclePartById(int id)
    {
        var vehicleResult = await vehicleService.GetVehicleAsync(id);

        if (!vehicleResult.IsSuccess)
            return ProcessError(vehicleResult);

        return Ok(vehicleResult.GetResult<Vehicle>());
    }

    [HttpPost]
    public async Task<IActionResult> AddVehicle(AddVehicleCommand addVehicleCommand)
    {
        var addedVehicleResult = await vehicleService.AddVehicleAsync(addVehicleCommand);
        var addedVehicle = addedVehicleResult.GetResult<Vehicle>();

        return CreatedAtRoute("VehicleById", new { id = addedVehicle.Id }, addedVehicle);
    }
}
