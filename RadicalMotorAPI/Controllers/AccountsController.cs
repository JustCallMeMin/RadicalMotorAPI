using Microsoft.AspNetCore.Mvc;
using RadicalMotor.Models;
using RadicalMotorAPI.DTO;
using RadicalMotorAPI.PasswordHash;
using RadicalMotorAPI.Repositories;

namespace RadicalMotor.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController : ControllerBase
	{
		private readonly IAccountRepository _accountRepository;

		public AccountsController(IAccountRepository accountRepository)
		{
			_accountRepository = accountRepository;
		}
		// GET: api/Accounts
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
		{
			return Ok(await _accountRepository.GetAllAccountsAsync());
		}

		// GET: api/Accounts/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Account>> GetAccount(string id)
		{
			var account = await _accountRepository.GetByIdAsync(id);
			if (account == null)
			{
				return NotFound();
			}
			return account;
		}

		// POST: api/Accounts
		[HttpPost]
		public async Task<ActionResult<Account>> PostAccount([FromBody] AccountDTO accountDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var existingAccount = await _accountRepository.GetByEmailAsync(accountDto.Email);
			if (existingAccount != null)
			{
				return Conflict(new { message = "An account with the given email already exists." });
			}

			var defaultAccountType = await _accountRepository.GetDefaultAccountTypeAsync("Member"); // Example usage
			if (defaultAccountType == null)
			{
				return BadRequest("Default account type not found.");
			}

			var account = new Account
			{
				Username = accountDto.Username,
				Email = accountDto.Email,
				Password = PasswordHasher.HashPassword(accountDto.Password),
				AccountTypeId = defaultAccountType.AccountTypeId,
				AccountCreationDate = DateTime.UtcNow
			};

			var createdAccount = await _accountRepository.AddAsync(account);
			if (createdAccount == null)
			{
				return BadRequest("Unable to create account due to an error.");
			}

			return CreatedAtAction(nameof(GetAccount), new { id = createdAccount.AccountId }, createdAccount);
		}

		// PUT: api/Accounts/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> PutAccount(string id, [FromBody] Account account)
		{
			if (id != account.AccountId)
			{
				return BadRequest();
			}

			await _accountRepository.UpdateAsync(account);

			return NoContent();
		}

		// DELETE: api/Accounts/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAccount(string id)
		{
			var account = await _accountRepository.GetByIdAsync(id);
			if (account == null)
			{
				return NotFound();
			}

			await _accountRepository.DeleteAsync(id);

			return NoContent();
		}

		// This utility method should also use the repository
		private async Task<bool> AccountExists(string id) =>
			await _accountRepository.GetByIdAsync(id) != null;
	}
}
