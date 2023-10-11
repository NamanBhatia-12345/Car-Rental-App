using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalApp.Core.Models.DTO
{
    public class UserDetailsDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public List<CarRentDto> RentalAgreements { get; set; }
    }
}
