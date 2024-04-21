using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RadicalMotor.Models;
using RadicalMotorAPI.PasswordHash;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RadicalMotorAPI.Repositories
{
	public class AccountRepository : IAccountRepository
	{
		private readonly ApplicationDbContext _context;

		public AccountRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<Account>> GetAllAsync()
		{
			return await _context.Accounts.ToListAsync();
		}

		public async Task<Account> GetByIdAsync(string id)
		{
			return await _context.Accounts.FindAsync(id);
		}

		public async Task<Account> AddAsync(Account account)
		{
			account.Password = PasswordHasher.HashPassword(account.Password); // Hashing the password
			_context.Accounts.Add(account);
			await _context.SaveChangesAsync();
			return account;
		}

		public async Task UpdateAsync(Account account)
		{
			_context.Entry(account).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(string id)
		{
			var account = await _context.Accounts.FindAsync(id);
			if (account != null)
			{
				_context.Accounts.Remove(account);
				await _context.SaveChangesAsync();
			}
		}
		public async Task<AccountType> GetDefaultAccountTypeAsync(string typeName)
		{
			return await _context.AccountTypes.FirstOrDefaultAsync(at => at.TypeName == typeName);
		}
		public async Task<Account> GetByEmailAsync(string email)
		{
			return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
		}
		public async Task<IEnumerable<Account>> GetAllAccountsAsync()
		{
			return await _context.Accounts.ToListAsync();
		}
		public async Task<string> CreateTokenAsync(Account account, bool rememberMe)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("FA3B2ED187A2AF32ECEBC9B2B87C40B8C887F3A1E8A671ADB86D7E0A2C6A405E"));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, account.Email),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
		};

			var token = new JwtSecurityToken(
				issuer: "https://localhost:44301/",
				audience: "https://localhost:44301/",
				claims: claims,
				expires: DateTime.Now.AddMinutes(rememberMe ? 1440 : 30),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
