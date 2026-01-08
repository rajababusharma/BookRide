using BookRide.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Services
{
    public class NetworkService : INetworkService
    {
        public bool HasInternet() =>
         Connectivity.Current.NetworkAccess == Microsoft.Maui.Networking.NetworkAccess.Internet;
    }
}
