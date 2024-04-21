using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadicalMotor.Models;
using RadicalMotorAPI.DTO;
using RadicalMotorAPI.Models;
using RadicalMotorAPI.PasswordHash;
using RadicalMotorAPI.Repositories;



namespace RadicalMotorAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginApiController : ControllerBase
	{
		private readonly IAccountRepository _accountRepository;

		public LoginApiController(IAccountRepository accountRepository)
		{
			_accountRepository = accountRepository;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var account = await _accountRepository.GetByEmailAsync(loginDTO.Email);

			if (account != null && PasswordHasher.VerifyPassword(loginDTO.Password, account.Password))
			{
				var token = await _accountRepository.CreateTokenAsync(account, loginDTO.RememberMe);
				return Ok(new { Token = token, Message = "Login successful" });
			}

			return Unauthorized("Invalid credentials");
		}
	}
}
