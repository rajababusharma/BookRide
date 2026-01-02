using BookRide.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Services
{
    public class LocationService : ILocation
    {
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
        public async Task<Location> GetCurrentLocationAsync()
        {
            Location? location = null;
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                    //  latitude = location.Latitude;
                    // longitude = location.Longitude;

                  //  Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                return location;

            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (Exception ex)
            {
                // Unable to get location
                Console.WriteLine($"Unable to get location: {ex.Message}");
                await Shell.Current.DisplayAlert(
                    "Error",
                   $"Unable to get location. Please ensure location services are enabled and permissions are granted.{ex.Message}",
                    "OK");
               // return location;
            }
            finally
            {
                _isCheckingLocation = false;

            }
            return location;
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }
    }
}
