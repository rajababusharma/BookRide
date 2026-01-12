using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookRide.ViewModels
{
    public partial class TravellerProfileVM : ObservableObject, IQueryAttributable
    {
        private RealtimeDatabaseService _db;
      
        private readonly IWhatsAppConnect _whatsAppConnect;
        public ObservableCollection<string> Districts { get; }
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        public ObservableCollection<Users> usersList = new();

        [ObservableProperty]
        public Users user;
        [ObservableProperty]
        public string hi="Hi";
        [ObservableProperty]
        public string selectedDistrict;
        private readonly GeolocationRequest _geolocationRequest;

        private readonly INetworkService _networkService;

        private Location currentLocation;
        partial void  OnSelectedDistrictChanged(string value)
        {
            if (value == null) return;
            Console.WriteLine($"Selected: {value}");

            LoadUsersByDistrictAsync(value);
        }
        [RelayCommand]
        public async void WhatsappConnect(string phoneNumber)
        {
            if (phoneNumber != null && phoneNumber.Length == 10)
            {
                try
                {
                     _whatsAppConnect.WhatsappConnect("+91" + phoneNumber, $"Hello, my name is {User.FirstName} " + " and I want to connect with you.");
                }
                catch (Exception exp)
                {
                    // Handle error
                    await Shell.Current.DisplayAlert(
                           "Error",
                           exp.Message,
                           "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert(
                          "Error",
                          "Incorrect phone number",
                          "OK");
            }
        }

        [RelayCommand]
        public async void Call(string phoneNumber)
        {
            if (phoneNumber != null && phoneNumber.Length == 10)
            {

                try
                {
                    //  PhoneDialer.Default.Open(phoneNumber);
                    await Launcher.Default.OpenAsync("tel:+91" + phoneNumber);

                }
                catch (Exception exp)
                {
                    // Handle error
                    await Shell.Current.DisplayAlert(
                           "Error",
                           exp.Message,
                           "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert(
                          "Error",
                          "Incorrect phone number",
                          "OK");
            }
        }

        public TravellerProfileVM(IWhatsAppConnect whatsApp, INetworkService networkService)
        {
            _whatsAppConnect = whatsApp;
            Districts = new ObservableCollection<string>(UttarPradeshDistricts.All);
            _db = new RealtimeDatabaseService();
            _geolocationRequest = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            _networkService = networkService;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await LocationPermissionHelper.CheckGPSLocationEnableAsync();
                // async work
                await LocationPermissionHelper.HasPermissionsAsync();

                currentLocation = await Geolocation.Default.GetLocationAsync(_geolocationRequest);

                await LoadUsersByDistrictAsync("");
            }
            catch (Exception ex)
            {
                await LoadUsersByDistrictAsync("");
                // Handle exceptions related to geolocation
                // Console.WriteLine($"Error obtaining location: {ex.Message}");
                await Shell.Current.DisplayAlert(
                              "Error",
                              $"Error obtaining location: {ex.Message}",
                              "OK");
            }
          
            //  await LoadDataAsync();
        }

        public async Task LoadUsersByDistrictAsync(string district)
        {
            IsBusy = true;
            // check internet connectivity first 
            if (!_networkService.HasInternet())
            {
                await Shell.Current.DisplayAlert("No Internet", "Please check your internet connection and try again.", "OK");
                // ErrorMessage = "No internet connection. Please check your connection and try again.";
                IsBusy = false;
                return;
            }
            try
                {
                UsersList.Clear();
                if (string.IsNullOrEmpty(district))

                {
                    Task.Run(async () =>
                    {
                        var users = await _db.GetAllAsync<Users>("Users").ContinueWith(t =>
                        {
                            var userList = t.Result.Where<Users>(x => x.CreditPoint > 0 && x.IsActive && x.UserType.Equals(eNum.eNumUserType.Driver.ToString()));
                            return new ObservableCollection<Users>(userList);
                        });

                        UsersList = await GetLocationsWithinRadiusAsync(users);
                    }).GetAwaiter().GetResult();
                }
                else
                {
                    Task.Run(async () =>
                    {
                        var users = await _db.GetAllAsync<Users>("Users").ContinueWith(t =>
                        {
                            var userList = t.Result.Where<Users>(x => x.CreditPoint > 0 && x.UserType.Equals(eNum.eNumUserType.Driver.ToString()) && x.District.Equals(district));
                            return new ObservableCollection<Users>(userList);
                        });
                        UsersList = await GetLocationsWithinRadiusAsync(users);
                    }).GetAwaiter().GetResult();
                }

                IsBusy = false;

            }
            catch (Exception ex)
                {
                    // Handle exceptions related to geolocation
                    // Console.WriteLine($"Error obtaining location: {ex.Message}");
                    IsBusy = false;
                await Shell.Current.DisplayAlert(
                              "Error",
                              $"Error obtaining location: {ex.Message}",
                              "OK");
                }
          
        }

        public async Task<ObservableCollection<Users>> GetLocationsWithinRadiusAsync(ObservableCollection<Users> users)
        {
            try
            {
                 
                    if (currentLocation == null)
                    {
                        return users;
                    }


                    double radiusKm = 5.0;

                    foreach (var usr in users)
                    {
                        if (usr.Latitude != null && usr.Longitude!=null)
                        {
                        var lat = usr.Latitude;
                        var lon = usr.Longitude;
                        var alt = usr?.Altitude;
                        var acc = usr?.Accuracy;
                        var time = usr?.Timestamp;
                        var vertical = usr?.Vertical;
                        var speed = usr?.Speed;
                        var course = usr?.Course;

                        // Create a Location object for the user's location
                        Location driverLocation = new Location();
                        driverLocation.Latitude = lat;
                        driverLocation.Longitude = lon;
                        driverLocation.Altitude = alt ?? double.NaN;
                        driverLocation.Accuracy = acc ?? double.NaN;
                        driverLocation.Timestamp = DateTimeOffset.UtcNow;
                        driverLocation.Speed = speed ?? double.NaN;
                        driverLocation.Course = course ?? double.NaN;
                        driverLocation.VerticalAccuracy = vertical ?? double.NaN;

                        // Calculate the distance in kilometers
                        double distance = currentLocation.CalculateDistance(driverLocation, DistanceUnits.Kilometers);
                            if (distance <= radiusKm)
                            {
                                UsersList.Add(usr);
                            }
                         }
                        else
                        {
                        UsersList.Add(usr);
                        }
                    }

                    return UsersList;
                
            }
            catch (Exception ex)
            {
                // Handle exceptions related to geolocation
               // Console.WriteLine($"Error obtaining location: {ex.Message}");
               
                return users; // Return the original list if location cannot be obtained

            }
           
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            User = query["CurrentUser"] as Users;
        }
    }
}
