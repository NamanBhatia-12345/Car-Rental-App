using CarRentalApp.Application.IRepositories;
using CarRentalApp.Core.Models.Domain;
using CarRentalApp.Core.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarRepository carRepository;
        public CarController(ICarRepository carRepository)
        {
            this.carRepository = carRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        //GET :- api/Car?filterOn=Name&filterQuery=Track
        public async Task<IActionResult> GetAll([FromQuery] string filterOn, [FromQuery] string filterQuery)
        {
            //Get Data from the Database
            //var cars = await dbContext.Cars.ToListAsync();
            var cars = await carRepository.GetAllAsync(filterOn, filterQuery);

            var carDto = new List<CarDto>();
            foreach (var car in cars)
            {
                carDto.Add(new CarDto
                {
                    Id = car.Id,
                    Maker = car.Maker,
                    Model = car.Model,
                    RentalPrice = car.RentalPrice,
                    AvailabilityStatus = car.AvailabilityStatus
                });
            }
            return Ok(carDto);
        }
        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var carDomain = await carRepository.GetByIdAsync(id);
            if(carDomain == null)
            {
                return NotFound();
            }
            // Map/Convert Domain Model to DTO
            var carDto = new CarDto
            {
                Id = carDomain.Id,
                Maker = carDomain.Maker,
                Model = carDomain.Model,
                RentalPrice = carDomain.RentalPrice,
                AvailabilityStatus = carDomain.AvailabilityStatus   
            };
            return Ok(carDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] AddCarDto addCarDto)
        {
            // Map / Convert the Dto to Domain Model
            var carDomainModel = new Car
            {
                Maker = addCarDto.Maker,
                Model = addCarDto.Model,
                RentalPrice = addCarDto.RentalPrice,
                AvailabilityStatus = addCarDto.AvailabilityStatus
            };

            // Use Domain Model to create Car
            await carRepository.CreateAsync(carDomainModel);
            // Map Domain Model to Dto
            var carDto = new CarDto
            {
                Id = carDomainModel.Id,
                Maker = carDomainModel.Maker,
                Model = carDomainModel.Model,
                RentalPrice = carDomainModel.RentalPrice,
                AvailabilityStatus = carDomainModel.AvailabilityStatus
            };
            return CreatedAtAction(nameof(GetById),new { id = carDto.Id}, carDto);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        // If suppose car is rented then it cannot be updated first because it is already rented by the user.
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCarDto updateCarDto)
        {
            var carModel = await carRepository.GetByIdAsync(id);
            if (carModel != null && carModel.AvailabilityStatus == false)
            {
                return NotFound($"Car with maker :- {carModel.Maker} and its model :- {carModel.Model} cannot be updated because it is already rented by the user.");
            }
            var carDomainModel = new Car
            {
                Maker = updateCarDto.Maker,
                Model = updateCarDto.Model,
                RentalPrice = updateCarDto.RentalPrice,
                AvailabilityStatus = updateCarDto.AvailabilityStatus
            };
            carDomainModel = await carRepository.UpdateAsync(id, carDomainModel);
            if(carDomainModel == null)
            {
                return NotFound("The Car does not exist");
            }
            var carDto = new CarDto
            {
                Id = carDomainModel.Id,
                Maker = carDomainModel.Maker,
                Model = carDomainModel.Model,
                RentalPrice = carDomainModel.RentalPrice,
                AvailabilityStatus = carDomainModel.AvailabilityStatus
            };
            return Ok(carDto);
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var carModel = await carRepository.GetByIdAsync(id);
            if(carModel != null && carModel.AvailabilityStatus == false) 
            {
                return NotFound($"Car with maker :- {carModel.Maker} and its model :- {carModel.Model} cannot be deleted from database because it is already rented by the user.");
            }
            var carDomainModel = await carRepository.DeleteAsync(id);
            if(carDomainModel == null)
            {
                return NotFound();
            }
            var carDto = new CarDto 
            {
                Id = carDomainModel.Id,
                Maker = carDomainModel.Maker,
                Model = carDomainModel.Model,
                RentalPrice = carDomainModel.RentalPrice,
                AvailabilityStatus = carDomainModel.AvailabilityStatus
            };
            return Ok(carDto);
        }
    }
}