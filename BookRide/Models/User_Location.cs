using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Models
{
    [Serializable]
    public class User_Location
    {
        public string UserId { get; set; }
        public string? CurrentAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public double? Accuracy { get; set; }
        public DateTime Timestamp { get; set; }
        public double? Vertical { get; set; }
        public double? Speed { get; set; }
        public double? Course { get; set; }

    }
}
