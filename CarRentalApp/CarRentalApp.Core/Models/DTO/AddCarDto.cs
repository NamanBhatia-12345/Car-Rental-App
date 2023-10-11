using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Core.Models.DTO
{
    public class AddCarDto
    {
        [Required]
        [StringLength(20, ErrorMessage = "Car Maker must be atmost 20 characters long")]
        public string Maker { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Car Model must be atmost 20 characters long")]
        public string Model { get; set; }
        [Required]
        [Range(50,10000,ErrorMessage = "Rental Price must be between Rs.50 and Rs.10,000.")]
        [RegularExpression(@"^\d+$",ErrorMessage = "Rental must be a whole number.")]
        public int RentalPrice { get; set; }
        public bool AvailabilityStatus { get; set; } = true;
    }
}
