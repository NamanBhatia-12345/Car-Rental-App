using CarRentalApp.Application.IRepositories;
using CarRentalApp.Core.Models.Domain;
using CarRentalApp.Core.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentController : ControllerBase
    {
        private readonly IRentRepository rentRepository;
        private readonly ICarRepository carRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public RentController(IRentRepository rentRepository, ICarRepository carRepository, UserManager<ApplicationUser> userManager)
        {
            this.rentRepository = rentRepository;
            this.carRepository = carRepository;
            this.userManager = userManager;
        }
        [HttpPost]
        [Authorize(Roles = "User")]
        // Doubt in that only user can create the rental agreement or the admin can also
        public async Task<IActionResult> CreateRentalAgreement([FromBody] AddRentDto addRentDto)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (User.Identity.IsAuthenticated)
            {
                string userName = User.Identity.Name;
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                
                if (userId == null)
                {
                    return Unauthorized("The User is not exist!!.Please register");
                }
            }
            var carDomain = await carRepository.GetByIdAsync(addRentDto.CarId); 
            if(carDomain == null)
            {
                return NotFound("The Car Not Found");
            }
            if (carDomain.AvailabilityStatus == false)
            {
                return BadRequest("The Car is not available for rent");
            }
            await carRepository.UpdateAvailabilityStatusAsync(carDomain.Id,carDomain,false);
            // Map/ Convert the Dto to Domain Model
            var rentDomainModel = new Rent
            {
                RentalDuration = addRentDto.RentalDuration,
                RequestForReturn = false,
                InspetionCompleted = false,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(addRentDto.RentalDuration),  
                CarId = addRentDto.CarId,
                UserId = user.Id
            };
            int totalDays = (rentDomainModel.EndDate - rentDomainModel.StartDate).Days;   
            int totalCost = (totalDays * carDomain.RentalPrice);
            rentDomainModel.TotalCost = totalCost;
            await rentRepository.CreateAsync(rentDomainModel);
            var rentDto = new RentDto
            {
                Id = rentDomainModel.Id,
                RentalDuration = rentDomainModel.RentalDuration,
                TotalCost = rentDomainModel.TotalCost,
                StartDate = rentDomainModel.StartDate,
                EndDate = rentDomainModel.EndDate,
                CarId = rentDomainModel.CarId,
                UserId = rentDomainModel.UserId
            };
            //Map Domain Model to Dto
            return CreatedAtAction(nameof(GetRentalAgreementById), new { Id = rentDto.Id }, rentDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRentalAgreementById([FromRoute] int id)
        {
            
            var rentDomain = await rentRepository.GetByIdAsync(id);
            if(rentDomain == null)
            {
                return NotFound($"The Rental Agreement does not exist with id :- {id}");
            }
           
            var rentDto = new RentDto
            {
                Id = rentDomain.Id,
                RentalDuration = rentDomain.RentalDuration,
                TotalCost = rentDomain.TotalCost,
                StartDate = rentDomain.StartDate,
                EndDate = rentDomain.EndDate,   
                CarId = rentDomain.CarId,
                UserId = rentDomain.UserId
            };
            return Ok(rentDto);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRentalAgreement()
        {
            var rents = await rentRepository.GetAllAsync();

            var rentDto = new List<RentDto>();
            foreach (var rent in rents)
            {
                rentDto.Add(new RentDto
                {
                    Id = rent.Id,
                    RentalDuration = rent.RentalDuration,
                    TotalCost = rent.TotalCost,
                    StartDate = rent.StartDate,
                    EndDate = rent.EndDate,
                    CarId = rent.CarId,
                    UserId = rent.UserId
                });
            }
            return Ok(rentDto);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRentalAgreement([FromRoute] int id, [FromBody] UpdateRentDto updateRentDto)
        {
            var rentDomainModel = new Rent
            {
                RentalDuration = updateRentDto.RentalDuration
            };
            rentDomainModel = await rentRepository.UpdateAsync(id, rentDomainModel);
            if(rentDomainModel == null)
            {
                return NotFound($"The Rental Agreement does not exist with id :- {id}");
            }
            var rentDto = new RentDto
            {
                Id = rentDomainModel.Id,
                RentalDuration = rentDomainModel.RentalDuration,
                TotalCost = rentDomainModel.TotalCost,
                StartDate = rentDomainModel.StartDate,
                EndDate = rentDomainModel.EndDate,
                CarId = rentDomainModel.CarId,
                UserId = rentDomainModel.UserId
            };
            return Ok(rentDto);
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRentalAgreement([FromRoute] int id)
        {
            var rentDomainModel = await rentRepository.DeleteAsync(id);
            if (rentDomainModel == null)
            {
                return NotFound($"The Rental Agreement does not exist with id :- {id}");
            }
            var carDomainModel = await carRepository.GetByIdAsync(rentDomainModel.CarId);
            await carRepository.UpdateAvailabilityStatusAsync(carDomainModel.Id, carDomainModel, true);
            var rentDto = new RentDto
            {
                Id = rentDomainModel.Id,
                RentalDuration = rentDomainModel.RentalDuration,
                TotalCost = rentDomainModel.TotalCost,
                StartDate = rentDomainModel.StartDate,
                EndDate = rentDomainModel.EndDate,
                CarId= rentDomainModel.CarId,  
                UserId = rentDomainModel.UserId,
            };
            return Ok(rentDto);
        }
        [HttpGet]
        [Route("Rental-Agreements")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserRentalAgreementDetails()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return Unauthorized("User not authenticated or does not exist.");
            }

            var userRentals = await rentRepository.GetRentalsByUserIdAsync(user.Id);

            // Create a list to store rental agreement DTOs
            var rentalAgreements = new List<CarRentDto>();

            foreach (var rental in userRentals)
            {
                var carDomainModel = await carRepository.GetByIdAsync(rental.CarId);

                // Create a CarRentDto for each rental agreement
                var carRentDto = new CarRentDto
                {
                    Id = rental.Id,
                    RentalDuration = rental.RentalDuration,
                    TotalCost = rental.TotalCost,
                    StartDate = rental.StartDate,
                    EndDate = rental.EndDate,
                    CarId = rental.CarId,
                    Maker = carDomainModel.Maker,
                    Model = carDomainModel.Model,
                    RentalPrice = carDomainModel.RentalPrice,
                    AvailabilityStatus = carDomainModel.AvailabilityStatus
                };

                rentalAgreements.Add(carRentDto);
            }

            // Create a UserDetailsDto to package the response data
            var userDetailsDto = new UserDetailsDto
            {
                UserId = user.Id,
                Username = user.UserName,
                RentalAgreements = rentalAgreements
            };

            return Ok(userDetailsDto);
        }
        [HttpPut]
        [Route("Request-For-Return/{id:int}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RequestForReturn([FromRoute] int id)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (User.Identity.IsAuthenticated == false && user.Id == null)
            {
                return Unauthorized("The User is not exist!!.Please register");
            }
            var rentDomainModel = await rentRepository.GetByIdAsync(id);
            if (rentDomainModel == null)
            {
                return NotFound($"The rent does not exist with rent id :- {id}");
            }
            if (rentDomainModel.UserId != user.Id)
            {
                return BadRequest($"The User with id :- {user.Id} and username :- {user.UserName} does not have rental agreement associated with rental id :- {rentDomainModel.Id}");
            }
            DateTime lastDate = DateTime.Now; 
            var diffDays = (lastDate - rentDomainModel.StartDate).Days;
            if(diffDays != rentDomainModel.RentalDuration)
            {
                rentDomainModel.EndDate = lastDate;
            }
            if (diffDays == 0)
                diffDays += 1;
            var newRentDomain = new Rent
            {
                Id = rentDomainModel.Id,
                RentalDuration = diffDays,
                RequestForReturn = true,
                StartDate = rentDomainModel.StartDate,
                EndDate = rentDomainModel.EndDate,
                CarId = rentDomainModel.CarId,
                UserId = rentDomainModel.UserId
            };
            await rentRepository.UpdateAsync(newRentDomain.Id, newRentDomain);
            return Ok("The Requested for Car return is Accepted");
        }

        [HttpPut]
        [Route("Validate-Return-Car/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Validate([FromRoute] int id)
        {
            var rentDomain = await rentRepository.GetByIdAsync(id);
            if(rentDomain == null)
            {
                return NotFound($"The rent does not exist with {id}");
            }
            if(rentDomain.RequestForReturn == false)
            {
                return NotFound($"The car is not return by the user with carid {rentDomain.CarId}");
            }
            var carDomain = await carRepository.GetByIdAsync(rentDomain.CarId);
            await rentRepository.UpdateInspectionAsync(id, rentDomain);
            await carRepository.UpdateAvailabilityStatusAsync(rentDomain.CarId, carDomain, true);
            return Ok($"The car model {carDomain.Model} and car maker {carDomain.Maker} is validated and return back in a working condition.");
        }
        [HttpGet]
        [Route("GetAllRequest-For-Return-Car")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRentalRequestForReturnCar()
        {
            var rents = await rentRepository.GetAllAsync();
            var rentalRequestForReturn = rents.Where(r => r.RequestForReturn && !r.InspetionCompleted).ToList();
            return Ok(rentalRequestForReturn);
        }

    }
}