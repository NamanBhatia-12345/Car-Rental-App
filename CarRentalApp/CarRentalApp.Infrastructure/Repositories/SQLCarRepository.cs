using CarRentalApp.Application.IRepositories;
using CarRentalApp.Core.Models.Domain;
using CarRentalApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Infrastructure.Repositories
{
    public class SQLCarRepository : ICarRepository
    {
        private readonly CarRentalDbContext dbContext;
        public SQLCarRepository(CarRentalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Car> CreateAsync(Car car)
        {
            await dbContext.Cars.AddAsync(car);
            await dbContext.SaveChangesAsync();
            return car;
        }
        public async Task<Car> DeleteAsync(int id)
        {
            var car = await dbContext.Cars.FirstOrDefaultAsync(x => x.Id == id);
            if (car == null)
            {
                return null;
            }
            dbContext.Cars.Remove(car);
            await dbContext.SaveChangesAsync();
            return car;
        }

        public async Task<List<Car>> GetAllAsync(string filterOn = null, string filterQuery = null)
        {
            var cars = dbContext.Cars.AsQueryable();
            //Filtering:-
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                cars = cars.Where(x => x.AvailabilityStatus == true);
                if (filterOn.Equals("Maker", StringComparison.OrdinalIgnoreCase))
                {
                    cars = cars.Where(x => x.Maker.Contains(filterQuery));
                }
                if (filterOn.Equals("Model", StringComparison.OrdinalIgnoreCase))
                {
                    cars = cars.Where(x => x.Model.Contains(filterQuery));
                }
            }
            return await cars.ToListAsync();
        }

        public async Task<Car> GetByIdAsync(int id)
        {
            var car = await dbContext.Cars.FirstOrDefaultAsync(x => x.Id == id);
            return car;
        }

        public async Task<Car> UpdateAsync(int id, Car car)
        {
            var carDomainModel = await dbContext.Cars.FirstOrDefaultAsync(x => x.Id == id);
            if (carDomainModel == null)
            {
                return null;
            }
            carDomainModel.Maker = car.Maker;
            carDomainModel.Model = car.Model;
            carDomainModel.RentalPrice = car.RentalPrice;
            carDomainModel.AvailabilityStatus = car.AvailabilityStatus;
            await dbContext.SaveChangesAsync();
            return carDomainModel;
        }
        public async Task<Car> UpdateAvailabilityStatusAsync(int id, Car car, bool flag)
        {
            var carDomainModel = await dbContext.Cars.FirstOrDefaultAsync(x => x.Id == id);
            if (carDomainModel == null)
            {
                return null;
            }
            carDomainModel.AvailabilityStatus = flag;
            await dbContext.SaveChangesAsync();
            return carDomainModel;
        }
    }
}
