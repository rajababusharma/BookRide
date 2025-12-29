using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Models
{
    public class UpiPaymentResult
    {
        public bool IsSuccess { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public string Response { get; set; }
    }
}
