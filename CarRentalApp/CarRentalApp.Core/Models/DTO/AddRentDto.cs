using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApp.Core.Models.DTO
{
    public class AddRentDto
    {
        [Required]
        [Range(1,30, ErrorMessage = "Rental duration must be between 1 to 30 days only.")]
        public int RentalDuration { get; set; }
        public int TotalCost { get; set; } = 0;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "CarId is required.")]
        [Range(1,int.MaxValue,ErrorMessage = "CarId must be positive integer.")]
        [ForeignKey("CarId")]
        public int CarId { get; set; }
    }
}
