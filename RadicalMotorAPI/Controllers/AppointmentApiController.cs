using Microsoft.AspNetCore.Mvc;
using RadicalMotor.Models;
using RadicalMotorAPI.DTO;
using RadicalMotorAPI.Repositories;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RadicalMotorAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AppointmentDetailsController : ControllerBase
	{
		private readonly IAppointmentRepository _appointmentRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IServiceRepository _serviceRepository;
		private readonly IAccountRepository _accountRepository;

		public AppointmentDetailsController(
			IAppointmentRepository appointmentRepository,
			ICustomerRepository customerRepository,
			IServiceRepository serviceRepository,
			IAccountRepository accountRepository)
		{
			_appointmentRepository = appointmentRepository;
			_customerRepository = customerRepository;
			_serviceRepository = serviceRepository;
			_accountRepository = accountRepository;
		}

		[HttpGet("{appointmentId}/{serviceId}")]
		public async Task<IActionResult> GetAppointmentDetail(string appointmentId, string serviceId)
		{
			var appointmentDetail = await _appointmentRepository.GetAppointmentDetailByIdAsync(appointmentId, serviceId);
			if (appointmentDetail == null)
			{
				return NotFound();
			}
			return Ok(appointmentDetail);
		}

		[HttpGet("byAppointment/{appointmentId}")]
		public async Task<IActionResult> GetAppointmentDetailsByAppointmentId(string appointmentId)
		{
			var appointmentDetails = await _appointmentRepository.GetAppointmentDetailsByAppointmentIdAsync(appointmentId);
			if (appointmentDetails == null)
			{
				return NotFound();
			}
			return Ok(appointmentDetails);
		}

		[HttpPost("book")]
		public async Task<IActionResult> BookService([FromBody] AppointmentDTO appointmentDto)
		{
			var loggedInAccountId = Request.Cookies["accountId"];
			if (string.IsNullOrEmpty(loggedInAccountId))
			{
				return Unauthorized("Account not found");
			}

            var isLoggedInCookie = Request.Cookies["isLoggedIn"];

            // Check if the 'isLoggedIn' cookie is set to 'true'
            if (isLoggedInCookie != "true")
            {
                return Unauthorized("User is not logged in.");
            }

            // Create and save the appointment
            var appointment = new Appointment
			{
				AccountId = loggedInAccountId,
				DateCreated = DateTime.UtcNow
			};
			await _appointmentRepository.AddAppointmentAsync(appointment); // Ensure this method exists and is implemented correctly

			if (!DateTime.TryParse(appointmentDto.ServiceDate, out var parsedServiceDate))
			{
				return BadRequest("Invalid date format");
			}

			var appointmentDetail = new AppointmentDetail
			{
				AppointmentId = appointment.AppointmentId,
				ServiceId = appointmentDto.ServiceId,
				ServiceDate = parsedServiceDate,
				Notes = appointmentDto.Notes
			};
			await _appointmentRepository.AddAppointmentDetailAsync(appointmentDetail); // Ensure this method exists and is implemented correctly

			return CreatedAtAction("GetAppointmentDetail", new { appointmentId = appointment.AppointmentId, serviceId = appointmentDto.ServiceId }, appointmentDetail);
		}

		[HttpPut("{appointmentId}/{serviceId}")]
		public async Task<IActionResult> UpdateAppointmentDetail(string appointmentId, string serviceId, [FromBody] AppointmentDetail appointmentDetail)
		{
			if (appointmentId != appointmentDetail.AppointmentId || serviceId != appointmentDetail.ServiceId)
			{
				return BadRequest("Mismatched ID in request");
			}

			await _appointmentRepository.UpdateAppointmentDetailAsync(appointmentDetail);
			return NoContent();
		}

		[HttpDelete("{appointmentId}/{serviceId}")]
		public async Task<IActionResult> DeleteAppointmentDetail(string appointmentId, string serviceId)
		{
			var detailExists = await _appointmentRepository.GetAppointmentDetailByIdAsync(appointmentId, serviceId);
			if (detailExists == null)
			{
				return NotFound();
			}

			await _appointmentRepository.DeleteAppointmentDetailAsync(appointmentId, serviceId);
			return NoContent();
		}
	}
}
