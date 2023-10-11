using CarRentalApp.Application.IRepositories;
using CarRentalApp.Core.Models.Domain;
using CarRentalApp.Core.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITokenRepository tokenRepository;
        public AuthController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }
        //POST Method
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new ApplicationUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };
            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);
            if (identityResult.Succeeded)
            {
                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    //Add roles to this user
                    identityResult = await userManager.AddToRoleAsync(identityUser, registerRequestDto.Roles);

                    if (identityResult.Succeeded)
                    {
                        return Ok("Registered Successfully!!!");
                    }
                }
            }
            return BadRequest("Something went wrong");
        }


        //POST :- /api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto) 
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);
            if (user != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(user,loginRequestDto.Password);
                if (checkPasswordResult)
                {
                    // Get the roles for this user
                    var roles = await userManager.GetRolesAsync(user);
                    if(roles != null)
                    {
                        var rolesString = string.Join(",", roles);
                        // Create Token
                        var jwtToken = tokenRepository.CreateJWTToken(user, rolesString);

                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };
                        return Ok(new
                        {
                            Message = "Login Successfully!!",
                            Token = response
                        });
                    }
                }   
            }
            return BadRequest("Username or password incorrect");
        }
        [HttpGet]
        [Route("GetUserDetails")]
        public async Task<IActionResult> GetDetailsOfLoggedInUser()
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return Unauthorized("Please login first!!");
            }
            var user = await userManager.GetUserAsync(HttpContext.User);
            var roles = await userManager.GetRolesAsync(user);
            if (user == null || roles == null)
            {
                return BadRequest("Not Found!!");
            }
            var rolesString = string.Join(",", roles);
            var getLoggedInUserDetails = new GetLoggedInUserDetails
            {
                UserId = user.Id,
                Username = user.UserName,
                Roles = rolesString
            };
            return Ok(getLoggedInUserDetails);
        }
    }
}
