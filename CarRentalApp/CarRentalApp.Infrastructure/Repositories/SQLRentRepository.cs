using CarRentalApp.Application.IRepositories;
using CarRentalApp.Core.Models.Domain;
using CarRentalApp.Core.Models.DTO;
using CarRentalApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Infrastructure.Repositories
{
    public class SQLRentRepository : IRentRepository
    {
        private readonly CarRentalDbContext dbContext;
        public SQLRentRepository(CarRentalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Rent> CreateAsync(Rent rent)
        {
            await dbContext.Rents.AddAsync(rent);
            await dbContext.SaveChangesAsync();
            return rent;
        }
        public async Task<Rent> DeleteAsync(int id)
        {
            var rent = await dbContext.Rents.FirstOrDefaultAsync(x => x.Id == id);
            if (rent == null)
            {
                return null;
            }
            dbContext.Rents.Remove(rent);
            await dbContext.SaveChangesAsync();
            return rent;
        }

        public async Task<List<Rent>> GetAllAsync()
        {
            var rents = await dbContext.Rents.ToListAsync();
            return rents;
        }

        public async Task<Rent> GetByIdAsync(int id)
        {
            var rent = await dbContext.Rents.FirstOrDefaultAsync(x => x.Id == id);
            return rent;
        }

        public async Task<List<Rent>> GetRentalsByUserIdAsync(string userId)
        {
            var rentals = await dbContext.Rents
             .Where(r => r.UserId == userId)
             .ToListAsync();

            return rentals;
        }

        public async Task<Rent> UpdateAsync(int id, Rent rent)
        {
            int totalDays = 0;
            var rentDomainModel = await dbContext.Rents.FirstOrDefaultAsync(x => x.Id == id);
            if (rentDomainModel == null)
            {
                return null;
            }
            var carDomainModel = await dbContext.Cars.FirstOrDefaultAsync(x => x.Id == rentDomainModel.CarId);
            rentDomainModel.RentalDuration = rent.RentalDuration;
            rentDomainModel.EndDate = rentDomainModel.StartDate.AddDays(rent.RentalDuration);
            totalDays = (rentDomainModel.EndDate - rentDomainModel.StartDate).Days;    
            int totalCost = (totalDays * carDomainModel.RentalPrice);
            rentDomainModel.TotalCost = totalCost;
            rentDomainModel.RequestForReturn = rent.RequestForReturn;
            rentDomainModel.InspetionCompleted = rent.InspetionCompleted;
            await dbContext.SaveChangesAsync();
            return rentDomainModel;
        }

        public async Task<Rent> UpdateInspectionAsync(int id, Rent inspection)
        {
            var rentDomainModel = await dbContext.Rents.FirstOrDefaultAsync(x => x.Id == id);
            rentDomainModel.InspetionCompleted = true;
            await dbContext.SaveChangesAsync();     
            return rentDomainModel;
        }
    }

}
