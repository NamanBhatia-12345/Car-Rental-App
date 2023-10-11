using CarRentalApp.Core.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Core.Models.DTO
{
    public class CarRentDto
    {
        [Key]
        public int Id { get; set; }
        public int RentalDuration { get; set; }
        public int TotalCost { get; set; } = 0;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        [ForeignKey("CarId")]
        public int CarId { get; set; }
        public string Maker { get; set; }
        public string Model { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Rental must be a whole number.")]
        public int RentalPrice { get; set; }
        public bool AvailabilityStatus { get; set; } = true;
        //Navigation Property
        //public ApplicationUser User { get; set; }

    }

}
