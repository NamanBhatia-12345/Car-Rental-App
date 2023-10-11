using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Core.Models.DTO
{
    public class CarDto
    {
        [Key]
        public int Id { get; set; }
        public string Maker { get; set; }
        public string Model { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Rental must be a whole number.")]
        public int RentalPrice { get; set; }
        public bool AvailabilityStatus { get; set; } = true;
    }
}
