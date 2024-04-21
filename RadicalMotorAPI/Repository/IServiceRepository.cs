using RadicalMotor.Models;

namespace RadicalMotorAPI.Repositories
{
	public interface IServiceRepository
	{
		Task<Service> GetServiceByIdAsync(string serviceId);
		Task<IEnumerable<Service>> GetAllServicesAsync();
		Task<Service> AddServiceAsync(Service service);
		Task UpdateServiceAsync(Service service);
		Task DeleteServiceAsync(string serviceId);
	}

}
