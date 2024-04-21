using RadicalMotor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RadicalMotorAPI.Repositories
{
	public interface IAppointmentRepository
	{
		Task<AppointmentDetail> GetAppointmentDetailByIdAsync(string appointmentId, string serviceId);
		Task<IEnumerable<AppointmentDetail>> GetAppointmentDetailsByAppointmentIdAsync(string appointmentId);
		Task AddAppointmentAsync(Appointment appointment);
		Task AddAppointmentDetailAsync(AppointmentDetail appointmentDetail);
		Task UpdateAppointmentDetailAsync(AppointmentDetail appointmentDetail);
		Task DeleteAppointmentDetailAsync(string appointmentId, string serviceId);
	}
}
