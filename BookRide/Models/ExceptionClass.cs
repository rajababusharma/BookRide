using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Models
{
    public class ExceptionClass
    {
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}
