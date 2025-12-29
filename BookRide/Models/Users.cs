using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Models
{
    public class Users
    {
        
        public string UserId { get; set; }
        public  string FirstName { get; set; }
        public  string LastName { get; set; }
        public int Age { get; set; }
        public  string Address { get; set; }

        public  string Mobile { get; set; }
        public  string Password { get; set; }
        public  string VehicleNo { get; set; }
        public string AadharCard { get; set; }
        public  string DrivingLicense { get; set; }
        public  string UserType { get; set; }
        public  int CreditPoint { get; set; } = 30;
        public string VehicleType { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
