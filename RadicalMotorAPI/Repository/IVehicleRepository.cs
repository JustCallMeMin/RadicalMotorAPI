using RadicalMotor.Models;
using System.Collections.Generic;

namespace RadicalMotor.Repositories
{
    public interface IVehicleRepository
    {
        VehicleDTO GetVehicle(string chassisNumber);
        IEnumerable<VehicleDTO> GetAllVehicles();
        Vehicle AddVehicle(Vehicle vehicle, List<VehicleImage> images);
        Vehicle UpdateVehicle(Vehicle vehicle, List<VehicleImage> images);
        void DeleteVehicle(string chassisNumber);
    }
}
