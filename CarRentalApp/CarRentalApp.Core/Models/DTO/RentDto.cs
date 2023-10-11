using CarRentalApp.Core.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Core.Models.DTO
{
    public class RentDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1, 30, ErrorMessage = "Rental duration must be between 1 t0 30 days only.")]
        public int RentalDuration { get; set; }
        public int TotalCost { get; set; } = 0;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }

        [ForeignKey("CarId")]
        public int CarId { get; set; }
        [ForeignKey("UserId")]
        public string UserId { get; set; }
    }
}
