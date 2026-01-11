using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Interfaces
{
    public interface ICurrentAddress
    {
        Task<string> GetCurrentAddressAsync(double lat, double lon);
    }
}
