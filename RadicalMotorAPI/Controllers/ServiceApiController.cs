using Microsoft.AspNetCore.Mvc;
using RadicalMotor.Models;
using RadicalMotorAPI.Repositories;
using System;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ServiceController : ControllerBase
{
	private readonly IServiceRepository _serviceRepository;

	public ServiceController(IServiceRepository serviceRepository)
	{
		_serviceRepository = serviceRepository;
	}

	// GET: api/Service
	[HttpGet]
	public async Task<IActionResult> GetAllServices()
	{
		try
		{
			var services = await _serviceRepository.GetAllServicesAsync();
			return Ok(services);
		}
		catch (Exception ex)
		{
			// Log the exception details
			return StatusCode(500, "Internal server error: " + ex.Message);
		}
	}

	// GET: api/Service/5
	[HttpGet("{id}")]
	public async Task<IActionResult> GetServiceById(string id)
	{
		try
		{
			var service = await _serviceRepository.GetServiceByIdAsync(id);

			if (service == null)
			{
				return NotFound();
			}

			return Ok(service);
		}
		catch (Exception ex)
		{
			// Log the exception details
			return StatusCode(500, "Internal server error: " + ex.Message);
		}
	}

	// POST: api/Service
	[HttpPost]
	public async Task<IActionResult> CreateService([FromBody] Service service)
	{
		try
		{
			if (service == null)
			{
				return BadRequest("Service object is null");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest("Invalid model object");
			}

			var createdService = await _serviceRepository.AddServiceAsync(service);
			return CreatedAtAction("GetServiceById", new { id = createdService.ServiceId }, createdService);
		}
		catch (Exception ex)
		{
			// Log the exception details
			return StatusCode(500, "Internal server error: " + ex.Message);
		}
	}

	// PUT: api/Service/5
	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateService(string id, [FromBody] Service service)
	{
		try
		{
			if (service == null || id != service.ServiceId)
			{
				return BadRequest("Service object is null or mismatched id");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest("Invalid model object");
			}

			await _serviceRepository.UpdateServiceAsync(service);
			return NoContent();
		}
		catch (Exception ex)
		{
			// Log the exception details
			return StatusCode(500, "Internal server error: " + ex.Message);
		}
	}

	// DELETE: api/Service/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteService(string id)
	{
		try
		{
			await _serviceRepository.DeleteServiceAsync(id);
			return NoContent();
		}
		catch (Exception ex)
		{
			// Log the exception details
			return StatusCode(500, "Internal server error: " + ex.Message);
		}
	}
}
