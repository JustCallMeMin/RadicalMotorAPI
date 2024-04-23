using Microsoft.AspNetCore.Mvc;
using RadicalMotorAPI.Repositories;
using RadicalMotorAPI.DTO;
using System.Threading.Tasks;

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
				var response = new LoginResponseDTO
				{
					AccountId = account.AccountId
				};

				return Ok(response);
			}
			else
			{
				return Unauthorized("Invalid credentials");
			}
		}
	}
}
