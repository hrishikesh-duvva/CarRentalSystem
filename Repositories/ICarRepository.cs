using CarRentalSystem.Models;

namespace CarRentalSystem.Repositories
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAvailableCars();
        Task<Car> GetCarById(int id);
        Task AddCar(Car car);
        Task UpdateCarAvailability(int id, bool isAvailable);
        Task DeleteCar(int id);
    }
}
