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
		private readonly ICustomerRepository _customerRepository;

		public AccountsController(IAccountRepository accountRepository, ICustomerRepository customerRepository)
		{
			_accountRepository = accountRepository;
			_customerRepository = customerRepository;
		}

		//GET: api/Accounts
	   [HttpGet]
		public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
		{
			return Ok(await _accountRepository.GetAllAccountsAsync());
		}

		//[HttpGet("testPasswordVerification")]
		//public ActionResult TestPasswordVerification()
		//{
		//	var hash = "$2a$11$F0mOrmyXjLhSSwmafLuKoer9pPpJH/LnKZdN7rCr/ofU8SlM7PJiK";
		//	var password = "tnhminh33";
		//	var isPasswordValid = PasswordHasher.VerifyPassword(password, hash);

		//	return Ok($"Password verification result: {isPasswordValid}");
		//}

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
		public async Task<ActionResult<Account>> PostAccount([FromBody] AccountCreateDTO accountDto)
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
				Password = accountDto.Password,
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
		public async Task<IActionResult> PutAccount(string id, [FromBody] AccountDTO accountDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var account = await _accountRepository.GetByIdAsync(id);
			if (account == null)
			{
				return NotFound("Account not found.");
			}

			if (!string.IsNullOrEmpty(accountDto.PhoneNumber))
			{
				var customer = await _customerRepository.GetByPhoneNumberAsync(accountDto.PhoneNumber);
				if (customer != null)
				{
					account.CustomerId = customer.CustomerId;
				}
				else
				{
					return BadRequest("No customer with the provided phone number exists.");
				}
			}

			account.Username = accountDto.Username;
			account.Email = accountDto.Email;
			account.PhoneNumber = accountDto.PhoneNumber;
			if (!string.IsNullOrEmpty(accountDto.Password))
			{
				account.Password = PasswordHasher.HashPassword(accountDto.Password);
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
