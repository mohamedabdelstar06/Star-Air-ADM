using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarAirAdm.Application.DTOs.Flights;
using StarAirAdm.Application.Interfaces;

namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlightController : ControllerBase
{
    private readonly IFlightService _flightService;

    public FlightController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateFlight(CreateFlightTripDto dto)
    {
        try
        {
            var result = await _flightService.CreateFlightAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFlight(int id, UpdateFlightTripDto dto)
    {
        try
        {
            var result = await _flightService.UpdateFlightAsync(id, dto);
            if (result == null) return NotFound(new { message = "Flight not found." });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyFlights()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _flightService.GetMyFlightsAsync(userId);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _flightService.GetAllFlightsAsync();
        return Ok(result);
    }

    [HttpPatch("{id}/link")]
    public async Task<IActionResult> LinkAssessments(int id, LinkAssessmentDto dto)
    {
        var result = await _flightService.LinkAssessmentsAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        var result = await _flightService.CompleteFlightAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _flightService.DeleteFlightAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
