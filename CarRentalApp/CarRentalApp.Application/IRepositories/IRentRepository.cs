using CarRentalApp.Core.Models.Domain;

namespace CarRentalApp.Application.IRepositories
{
    public interface IRentRepository
    {
        Task<List<Rent>> GetAllAsync();
        Task<Rent> GetByIdAsync(int id);
        Task<Rent> CreateAsync(Rent rent);
        Task<Rent> UpdateAsync(int id, Rent rent);
        Task<Rent> DeleteAsync(int id);
        Task<List<Rent>> GetRentalsByUserIdAsync(string userId);
        Task<Rent> UpdateInspectionAsync(int id, Rent inspection);
    }
}
