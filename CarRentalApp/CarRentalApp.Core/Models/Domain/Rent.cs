using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApp.Core.Models.Domain
{
    public class Rent
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1, 30, ErrorMessage = "Rental duration must be between 1 t0 30 days only.")]
        public int RentalDuration { get; set; }
        public int TotalCost { get; set; } = 0;
        public bool RequestForReturn { get; set; } = false;
        public bool InspetionCompleted { get; set; } = false;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }

        [ForeignKey("CarId")]
        public int CarId { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
        //Navigation Property
        public Car Car { get; set; }
        public ApplicationUser User { get; set; }
    }
}
