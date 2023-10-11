using CarRentalApp.Core.Models.Domain;

namespace CarRentalApp.Application.IRepositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(ApplicationUser user, string roles);
    }
}
