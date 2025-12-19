using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Models
{
    public class Drivers
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public int Age { get; set; }
        public required string Address { get; set; }
        public required string Password { get; set; }
        public required string VehicleNo { get; set; }
        public required int AadharCard { get; set; }
        public required string DrivingLicense { get; set; }
    }
}
