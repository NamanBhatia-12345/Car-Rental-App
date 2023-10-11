using CarRentalApp.Application.IRepositories;
using CarRentalApp.Core.Models.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRentalApp.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;
        public TokenRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string CreateJWTToken(ApplicationUser user, string roles)
        {
            // Create Claims 
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Role, roles));

            //Include the UserId claims
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

            //Include the aditional user details as claims 
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            // Create a key from your app's secret key (stored securely)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the token
            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
    }
}
