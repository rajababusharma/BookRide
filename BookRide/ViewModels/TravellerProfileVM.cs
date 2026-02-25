using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Services;
using BookRide.Views;
using Microsoft.Maui.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoogleGson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
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
        private ObservableCollection<Drivers> driversList = new();

        [ObservableProperty]
        private Users user;
        [ObservableProperty]
        private string hi="Hi";
        [ObservableProperty]
        private string selectedDistrict = "Select District";
        [ObservableProperty]
        private bool isFeatureEnabled;
        private readonly GeolocationRequest _geolocationRequest;

        private readonly INetworkService _networkService;

        private Location? currentLocation;
       // private string selected_district="";
        partial void OnSelectedDistrictChanged(string value)
        {
            if (value == null) return;
            Console.WriteLine($"Selected: {value}");
            SelectedDistrict = value;
            Console.WriteLine("Loading drivers list by district");
            if (IsBusy)
            {
                IsBusy = false;
            }
            // Fire-and-forget the async load. The async method manages IsBusy and UI updates.
            _ = LoadUsersByDistrictAsync("0");
        }

        // Optional: Logic to run when the value changes
        partial void OnIsFeatureEnabledChanged(bool value)
        {
            SelectedDistrict = "Select District";
            if (IsBusy)
            {
                IsBusy = false;
            }
            // Your logic here (e.g., saving settings)
            if (value)
            {
                _ = LoadUsersByDistrictAsync("1");
            }
            else
            {              
                    _ = LoadUsersByDistrictAsync("0");
                           
            }
        }

        [RelayCommand]
        public async Task WhatsappConnect(string phoneNumber)
        {
            if (phoneNumber != null && phoneNumber.Length == 10)
            {
                try
                {
                     _whatsAppConnect.WhatsappConnect("+91" + phoneNumber, $"Hello, my name is {User.FirstName} " + " and I want to connect with you to book your vehicle for ride.");
                }
                catch (Exception exp)
                {
                    // Handle error
                    await Shell.Current.DisplayAlertAsync("Error", exp.Message, "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Error", "Incorrect phone number", "OK");
            }
        }

        [RelayCommand]
        public async Task Call(string phoneNumber)
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
                    await Shell.Current.DisplayAlertAsync("Error", exp.Message, "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Error", "Incorrect phone number", "OK");
            }
        }

        public TravellerProfileVM(IWhatsAppConnect whatsApp, INetworkService networkService,RealtimeDatabaseService databaseService)
        {
            SelectedDistrict = "Select District";
            _whatsAppConnect = whatsApp;
            Districts = new ObservableCollection<string>(UttarPradeshDistricts.All);
          //  _db = new RealtimeDatabaseService();
            _db = databaseService;
            _geolocationRequest = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            _networkService = networkService;
            IsBusy = false;
        }

        // calling this method on page load to check GPS and location permissions and also to load drivers list
        public async Task InitializeAsync()
        {
            try
            {
                SelectedDistrict = "Select District";
                Console.WriteLine("Checking GPS and Location Permissions...");
                await LocationPermissionHelper.CheckGPSLocationEnableAsync();
                // async work
                await LocationPermissionHelper.HasPermissionsAsync();

                Console.WriteLine("Obtaining current location...");
                currentLocation = await Geolocation.Default.GetLocationAsync(_geolocationRequest);

                Console.WriteLine("Loading drivers list");
                await LoadUsersByDistrictAsync("");
            }
            catch (Exception ex)
            {
               // await LoadUsersByDistrictAsync("");
                // Handle exceptions related to geolocation
                // Console.WriteLine($"Error obtaining location: {ex.Message}");
                await Shell.Current.DisplayAlertAsync(
                              "Error",
                              $"Error obtaining location: {ex.Message}",
                              "OK");
            }
          
            //  await LoadDataAsync();
        }


        public async Task LoadUsersByDistrictAsync(string district)
        {
            if (IsBusy)
            {
                IsBusy = false;
                return;
            }
               

            IsBusy = true; // Shows the spinner
            // check internet connectivity first 
            if (!_networkService.HasInternet())
            {
                await Shell.Current.DisplayAlertAsync("No Internet", "Please check your internet connection and try again.", "OK");
                // ErrorMessage = "No internet connection. Please check your connection and try again.";
                IsBusy = false;
                return;
            }
            try
                {
                // fetch all drivers from firebase and then filter them by district and also check if credit point is greater than 0 and user is active
                var drivers = await _db.GetAllAsync<Drivers>("Drivers");
                if (drivers == null)
                {
                    IsBusy = false;
                    return;
                }
                //deleting all items from the driver list
                DriversList.Clear();
                // Flatten results
                var lists = new List<Drivers>(drivers.Count);
                foreach (var item in drivers)
                    lists.Add(item.Value);
                ObservableCollection<Drivers> result = new ObservableCollection<Drivers>();
                IEnumerable<Drivers> filteredUsers = lists.Where(x => x.CreditPoint > 0 && x.IsActive);
                if (district.Equals("0"))
                {
                    // check if selected district is not "Select District" then filter the list by district
                    if (!SelectedDistrict.Equals("Select District", StringComparison.OrdinalIgnoreCase))
                    {
                            Console.WriteLine($"Filtering drivers by district: {SelectedDistrict}");
                        filteredUsers = filteredUsers.Where(x => string.Equals(x.District, SelectedDistrict, StringComparison.OrdinalIgnoreCase));
                    }

                   
                  
                    foreach(var driver in filteredUsers)
                    {
                        result.Add(driver);
                    }
                }
                else if(district.Equals("1"))
                {
                    // Get filtered list within radius (this method returns a new collection)
                    result = await GetLocationsWithinRadiusAsync(new ObservableCollection<Drivers>(filteredUsers));
                }
                else
                {
                    foreach (var driver in filteredUsers)
                    {
                        result.Add(driver);
                    }
                }
               
               
                  

                    // Ensure we update the observable collection on the main thread
                    MainThread.BeginInvokeOnMainThread(() => DriversList = result);
                

               
              

            }
            catch (Exception ex)
                {
                    // Handle exceptions related to geolocation
                     Console.WriteLine($"Line: 216, TravelerProfileVM Error obtaining drivers list: {ex.Message}");
                    IsBusy = false;
                await Shell.Current.DisplayAlertAsync(
                              "Error",
                              $"Error obtaining location: {ex.Message}",
                              "OK");
                }
            finally
            {
                IsBusy = false;
            }
          
        }

        public async Task<ObservableCollection<Drivers>> GetLocationsWithinRadiusAsync(ObservableCollection<Drivers> drivers)
        {

            // user curret location is null then return the original list without filtering
            if (currentLocation == null)
                    {
                        return drivers;
                    }


                    double radiusKm = Constants.Constants.RADIUS_KM;

                  
                Console.WriteLine("Line: 239 TravelerProfileVM Filtering drivers within radius...");
            try
            {
                await Task.Run(async () =>
                {
                    foreach (var usr in drivers)
                    {
                        // fetch user location from Drivers_Location nodel

                        var _driverLocation = await Task.Run(async()=>
                        {
                            return await _db.GetAsync<Drivers_Location>("Drivers_Location/" + usr.UserId);
                        });
                        // var userDict = _userLoc as IDictionary<string, object>;

                        //Drivers_Location _userLocation =new Drivers_Location
                        //{
                        //    UserId = userDict["UserId"].ToString(),
                        //    Latitude = Convert.ToDouble(userDict["Latitude"]),
                        //    Longitude = Convert.ToDouble(userDict["Longitude"]),
                        //    Altitude = userDict["Altitude"] != null ? Convert.ToDouble(userDict["Altitude"]) : (double?)null,
                        //    Accuracy = userDict["Accuracy"] != null ? Convert.ToDouble(userDict["Accuracy"]) : (double?)null,
                        //    Speed = userDict["Speed"] != null ? Convert.ToDouble(userDict["Speed"]) : (double?)null,
                        //    Course = userDict["Course"] != null ? Convert.ToDouble(userDict["Course"]) : (double?)null,
                        //};

                        // checking _userLocation is null or not and also check if latitude and longitude are not null
                        //if (_driverLocation == null)
                        //{

                        //}

                        if (_driverLocation?.Latitude == null && _driverLocation?.Longitude == null)
                        {
                            DriversList.Add(usr);
                            continue;

                        }
                        else
                        {
                            var lat = _driverLocation.Latitude;
                            var lon = _driverLocation.Longitude;
                            var alt = _driverLocation?.Altitude;
                            var acc = _driverLocation?.Accuracy;
                            var time = _driverLocation?.Timestamp;
                            var vertical = _driverLocation?.Vertical;
                            var speed = _driverLocation?.Speed;
                            var course = _driverLocation?.Course;

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
                                DriversList.Add(usr);
                            }
                        }

                       

                    }
                });
              

                    return DriversList;
                
            }
            catch (Exception ex)
            {
                // Handle exceptions related to geolocation
                Console.WriteLine($"Line: 287 TravelerProfileVM Error obtaining driver list: {ex.Message}");
               
                return drivers; // Return the original list if location cannot be obtained

            }
           
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
           // User = query["CurrentUser"] as Users;
            if (!query.TryGetValue("CurrentUser", out var userObj) || userObj is not Users user)
                return;

            User = user;
        }

        [RelayCommand]
        public async Task UpdateProfileAsync()
        {

            var navigationParameter = new Dictionary<string, object>
                    {
                        { "CurrentUser", User }
                    };
            await Shell.Current.GoToAsync(nameof(UserRegistrationPage), navigationParameter);

        }
    }
}
