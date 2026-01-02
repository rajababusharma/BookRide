using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Models
{
    public class OrderResponse
    {
        public string OrderId { get; set; }
        public int Amount { get; set; }
        public string Key { get; set; }
    }
}
