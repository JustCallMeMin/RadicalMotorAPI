using RadicalMotor.Models;

namespace RadicalMotorAPI.Repositories
{
	public interface ICustomerRepository
	{
		Task<Customer> GetOrCreateCustomerAsync(string fullName, string phoneNumber);
		Task<List<Customer>> GetAllAsync();
		Task<Customer> GetByPhoneNumberAsync(string phoneNumber);
		Task<Customer> GetByIdAsync(string id);
		Task AddAsync(Customer customer);
		Task UpdateAsync(Customer customer);
		Task DeleteAsync(string id);
	}
}
