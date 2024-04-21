using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadicalMotor.DTO;
using RadicalMotor.Models;
using RadicalMotor.Repositories;
using System.Collections.Generic;

namespace RadicalMotor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehiclesController(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        // GET: api/Vehicles
        [HttpGet]
        public ActionResult<IEnumerable<VehicleDTO>> GetAllVehicles()
        {
            return Ok(_vehicleRepository.GetAllVehicles());
        }

        // GET: api/Vehicles/5
        [HttpGet("{chassisNumber}")]
        public ActionResult<VehicleDTO> GetVehicle(string chassisNumber)
        {
            var vehicle = _vehicleRepository.GetVehicle(chassisNumber);
            if (vehicle == null)
            {
                return NotFound();
            }
            return vehicle;
        }

        // POST: api/Vehicles
        [HttpPost]
        public ActionResult<VehicleDTO> AddVehicle(VehicleDTO vehicleDto)
        {
            var vehicle = new Vehicle
            {
                ChassisNumber = vehicleDto.ChassisNumber,
                VehicleName = vehicleDto.VehicleName,
                EntryDate = vehicleDto.EntryDate,
                Version = vehicleDto.Version,
                Price = vehicleDto.Price,
                VehicleTypeId = vehicleDto.VehicleTypeId
            };
            var images = vehicleDto.ImageUrls.Select(url => new VehicleImage { ImageUrl = url, ChassisNumber = vehicle.ChassisNumber }).ToList();
            _vehicleRepository.AddVehicle(vehicle, images);

            return CreatedAtAction(nameof(GetVehicle), new { chassisNumber = vehicle.ChassisNumber }, vehicleDto);
        }

        // PUT: api/Vehicles/5
        [HttpPut("{chassisNumber}")]
        public IActionResult UpdateVehicle(string chassisNumber, VehicleDTO vehicleDto)
        {
            if (chassisNumber != vehicleDto.ChassisNumber)
            {
                return BadRequest();
            }

            // Retrieve the existing vehicle using the repository
            var existingVehicle = _vehicleRepository.GetVehicle(chassisNumber);
            if (existingVehicle == null)
            {
                return NotFound();
            }

            // Create an updated vehicle object (or update the existingVehicle fields directly)
            var updatedVehicle = new Vehicle
            {
                ChassisNumber = vehicleDto.ChassisNumber,
                VehicleName = vehicleDto.VehicleName,
                EntryDate = vehicleDto.EntryDate,
                Version = vehicleDto.Version,
				Price = vehicleDto.Price,
				VehicleTypeId = vehicleDto.VehicleTypeId
            };

            var images = vehicleDto.ImageUrls.Select(url => new VehicleImage { ImageUrl = url, ChassisNumber = vehicleDto.ChassisNumber }).ToList();
            _vehicleRepository.UpdateVehicle(updatedVehicle, images);

            return NoContent();
        }



        // DELETE: api/Vehicles/5
        [HttpDelete("{chassisNumber}")]
        public IActionResult DeleteVehicle(string chassisNumber)
        {
            _vehicleRepository.DeleteVehicle(chassisNumber);
            return NoContent();
        }
    }
}
