using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Core.Models.DTO
{
    public class RegisterRequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Roles { get; set; }
    }
}
