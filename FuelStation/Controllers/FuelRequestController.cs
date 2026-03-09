using FuelStation.BLL.Services.Interfaces;
using FuelStation.Common.Models.DTOs.FuelRequest;
using FuelStation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FuelStation.Controllers;

[ApiController]
[Route("api/fuel-request")]
public class FuelRequestController : ControllerBase
{
    private readonly IFuelRequestService _fuelRequestService;

    public FuelRequestController(IFuelRequestService fuelRequestService)
    {
        _fuelRequestService = fuelRequestService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRequest(CreateFuelRequestDTO dto)
    {
        var userId = HttpContext.GetUserId();
        var result = await _fuelRequestService.CreateAsync(userId, dto);

        return Ok(result);
    }
}