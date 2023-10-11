using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Core.Models.DTO
{
    public class UpdateRentDto
    {
        [Required]
        [Range(1, 30, ErrorMessage = "Rental duration must be between 1 t0 30 days only.")]
        public int RentalDuration { get; set; }
    }
}
