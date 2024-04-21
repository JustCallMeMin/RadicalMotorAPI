using RadicalMotor.Models;

namespace RadicalMotorAPI.Repositories
{
    public interface IVehicleTypeRepository
    {
        VehicleType GetVehicleType(string vehicleTypeId);
        IEnumerable<VehicleType> GetAllVehicleTypes();
        VehicleType AddVehicleType(VehicleType vehicleType);
        VehicleType UpdateVehicleType(VehicleType vehicleType);
        void DeleteVehicleType(string vehicleTypeId);
    }
}
