using BookRide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Interfaces
{
    public interface IForegroundService
    {
        void Start(Users users);
        void Stop();
    }
}
