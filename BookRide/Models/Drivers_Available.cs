using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Models
{
    [Serializable]
    public class Drivers_Available
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string Mobile { get; set; }
        public string VehicleNo { get; set; }
        public string VehicleType { get; set; }
        public string State { get; set; } = "Uttar Pradesh";
        public string District { get; set; } = "Fatehpur";
        public string? ProfileImageUrl { get; set; } = "person.png";
        public string? CurrentAddress { get; set; }
    }
}
