using CarRentalApp.Core.Models.Domain;

namespace CarRentalApp.Application.IRepositories
{
    public interface ICarRepository
    {
        Task<List<Car>> GetAllAsync(string filterOn = null, string filterQuery = null);
        Task<Car> GetByIdAsync(int id);
        Task<Car> CreateAsync(Car car);
        Task<Car> UpdateAsync(int id, Car car);
        Task<Car> DeleteAsync(int id);
        Task<Car> UpdateAvailabilityStatusAsync(int id, Car car, bool flag);
    }
}
