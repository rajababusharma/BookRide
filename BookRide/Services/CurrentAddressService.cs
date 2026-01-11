using BookRide.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Services
{
    public class CurrentAddressService : ICurrentAddress
    {
        public async Task<string> GetCurrentAddressAsync(double lat, double lon)
        {
            try
            {
                var placemarks = await Geocoding.Default
                    .GetPlacemarksAsync(lat, lon);

                var place = placemarks?.FirstOrDefault();

                if (place == null)
                    return "Address not found";

                return $"{place.Thoroughfare}, {place.SubLocality}, " +
                       $"{place.Locality}, {place.AdminArea}, " +
                       $"{place.PostalCode}, {place.CountryName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "null";
            }
        }
    }
}
