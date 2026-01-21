using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Models
{
    [Serializable]
    public class Drivers
    {

        public string UserId { get; set; }
        public string FirstName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string VehicleNo { get; set; }
        public int CreditPoint { get; set; } = 61;
        public string VehicleType { get; set; }
        public string State { get; set; } = "Uttar Pradesh";
        public string District { get; set; } = "Fatehpur";
        public DateTime RegistrationDate { get; set; }
        public string? AadharImageURL { get; set; }
        public string? ProfileImageUrl { get; set; } = "person.png";
        public bool IsActive { get; set; } = true;


       
    }
}
