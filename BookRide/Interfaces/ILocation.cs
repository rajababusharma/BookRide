using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Interfaces
{
    public interface ILocation
    {
        Task<Location> GetCurrentLocationAsync();
    }
}
