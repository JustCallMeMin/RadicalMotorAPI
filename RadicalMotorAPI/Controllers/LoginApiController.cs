using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadicalMotor.Models;
using RadicalMotorAPI.DTO;
using RadicalMotorAPI.Models;
using RadicalMotorAPI.PasswordHash;
using RadicalMotorAPI.Repositories;
using System.Security.Claims;



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

			if (account != null && loginDTO.Password == account.Password)
			{
				// Create a cookie with user information
				var claims = new List<Claim>
		{
			new Claim("UserId", account.AccountId),
			new Claim(ClaimTypes.Email, account.Email),
            // Add a claim to indicate successful login
            new Claim("LoggedIn", "true")
            // Add more claims as needed
        };
				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				var authProperties = new AuthenticationProperties
				{
					IsPersistent = loginDTO.RememberMe // Set cookie persistence based on rememberMe flag
				};
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

				return Ok(new { Message = "Login successful" });
			}

			return Unauthorized("Invalid credentials");
		}

	}
}
