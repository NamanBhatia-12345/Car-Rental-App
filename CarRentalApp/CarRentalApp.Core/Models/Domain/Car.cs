using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Core.Models.Domain
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        public string Maker { get; set; }
        public string Model { get; set; }
        public int RentalPrice { get; set; }
        public bool AvailabilityStatus { get; set; } = true;
    }
}
