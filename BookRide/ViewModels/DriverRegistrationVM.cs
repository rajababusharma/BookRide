using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Services;
using BookRide.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
#if ANDROID
using Android.Content;
using Application = Android.App.Application;
using BookRide.Platforms.Android.Implementations;
#endif

namespace BookRide.ViewModels
{
    public partial  class DriverRegistrationVM : ObservableObject,IQueryAttributable
    {
      
        public ObservableCollection<string> States { get; }
        public ObservableCollection<string> Districts { get; }
        private readonly ITest _test;
        private readonly RealtimeDatabaseService _db;
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty] private string firstName;
        [ObservableProperty] private string lastName;
        [ObservableProperty] private string age;
        [ObservableProperty] private string address;
        [ObservableProperty] private string mobile;
        [ObservableProperty] private string password;

        [ObservableProperty] private string confirmPassword;
      
        [ObservableProperty] private string  vehicleNo;
        [ObservableProperty] private string aadharCard;
        [ObservableProperty] private string drivingLicense;

        [ObservableProperty] private string userType;
        [ObservableProperty] private int creditPoint = 30;
        [ObservableProperty] private string userType_para;

        [ObservableProperty] private string errorMessage;
        [ObservableProperty] private bool isDriver;

        [ObservableProperty] private string selectedVehicle;

        [ObservableProperty] private string selectedDistrict;
        [ObservableProperty] private string aadharImagePath;
       
        private readonly GeolocationRequest _geolocationRequest;

        private readonly INetworkService _networkService;

        private readonly ICurrentAddress _currentAddress;
        public DriverRegistrationVM(INetworkService networkService,ICurrentAddress currentAddress)
        {
            _db = new RealtimeDatabaseService();
            States = new ObservableCollection<string>(IndiaStates.All);
            Districts = new ObservableCollection<string>(UttarPradeshDistricts.All);
            _geolocationRequest = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
      
            _networkService = networkService;
            _currentAddress = currentAddress;

        }

        //public DriverRegistrationVM(ITest service)
        //{
        //    _test = service;

        //    _db = new RealtimeDatabaseService();

        //}

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        [RelayCommand]
        private async Task Pay()
        {

            var data = _test.GetValue();
            ErrorMessage = data;

        }

        [RelayCommand]
        private async Task RegisterAsync()
        {


            await LocationPermissionHelper.CheckGPSLocationEnableAsync();

            IsBusy = true;
            // check internet connectivity first 
            if (!_networkService.HasInternet())
            {
                await Shell.Current.DisplayAlert("No Internet", "Please check your internet connection and try again.", "OK");
                // ErrorMessage = "No internet connection. Please check your connection and try again.";
                IsBusy = false;
                return;
            }
            ErrorMessage = string.Empty;

            if(UserType_para=="Driver")
            {
                if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(Age) ||
                string.IsNullOrWhiteSpace(Address) ||
                string.IsNullOrWhiteSpace(Mobile) ||
                string.IsNullOrWhiteSpace(VehicleNo) ||
                string.IsNullOrWhiteSpace(DrivingLicense) || string.IsNullOrWhiteSpace(SelectedVehicle))
                {
                    ErrorMessage = "All fields are required";
                    IsBusy = false;
                    return;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(FirstName) ||
               string.IsNullOrWhiteSpace(Age) ||
               string.IsNullOrWhiteSpace(Address) ||
               string.IsNullOrWhiteSpace(Mobile))
                {
                    ErrorMessage = "All fields are required";
                    IsBusy = false;
                    return;
                }
            }

           

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                IsBusy = false;
                return;
            }

          //  await Task.Delay(1500); // Simulate API call
    
            try
            {
              var location = await Geolocation.Default.GetLocationAsync(_geolocationRequest);


                var lat = location.Latitude;
                var lon = location.Longitude;
                var alt = location?.Altitude;
                var acc = location?.Accuracy;
                var time = location?.Timestamp;
                var vertical = location?.VerticalAccuracy;
                var speed = location?.Speed;
                var course = location?.Course;

                var currentloc = await _currentAddress.GetCurrentAddressAsync(lat, lon);

                var users = new Users
                { FirstName = FirstName, LastName = LastName, Age = int.Parse(Age), Address = Address, Mobile = Mobile, 
                    Password = Password, VehicleNo = VehicleNo, AadharCard = AadharCard, DrivingLicense = DrivingLicense, 
                    UserType = UserType_para, CreditPoint = CreditPoint, UserId=Mobile, VehicleType=SelectedVehicle,CurrentAddress=currentloc,
                    Latitude = lat, 
                    Longitude= lon, 
                    Altitude = alt,
                    Accuracy =acc,
                    Timestamp = time?.DateTime ?? DateTime.Now,
                    Vertical= vertical,
                    Speed= speed,
                    Course= course,
                    District =SelectedDistrict,
                    RegistrationDate=DateTime.Now,
                AadharImageURL= AadharImagePath};

                var usr=await _db.GetAsync<Users>($"Users/{users.UserId}");
                if(usr!=null)
                {
                    ErrorMessage = "User with this mobile number already exists";
                    await Shell.Current.DisplayAlert(
                  "Alert",
                  "User with this mobile number already exists",
                  "OK");
                    IsBusy = false;
                    return;
                }
           
                await _db.SaveAsync($"Users/{users.UserId}", users);
                await Shell.Current.DisplayAlert(
                    "Success",
                    "Registration completed successfully",
                    "OK");
                IsBusy = false;

                // start forground service to decrease credit point for driver daily and to update location
                if (UserType_para == "Driver")
                 {
                    
                    try
                      {
#if ANDROID
                        var intent_loc = new Intent(Application.Context, typeof(HourlyLocationService));
                        intent_loc.PutExtra("USERID", users.Mobile);
                        Application.Context.StartForegroundService(intent_loc);
#endif

#if ANDROID
                        var intent_credit = new Intent(Application.Context, typeof(DailyBasisCreditPointService));
                        intent_credit.PutExtra("USERID", users.Mobile);
                        Application.Context.StartForegroundService(intent_credit);
#endif
                        // await LocationPermissionHelper.HasPermissionsAsync();

                        // _foregroundService.Start(users.Mobile);


                    }
                    catch (Exception ex)
                      {
                            // Handle exceptions related to starting the service
                          //  System.Diagnostics.Debug.WriteLine($"Error starting DriverCreditPointService: {ex.Message}");
                      }
                }
               

                //  await Shell.Current.GoToAsync(nameof(RegistrationConfirmationPage));

                await Shell.Current.GoToAsync(nameof(ConfirmPage));



            }
            catch (Exception ex)
            {
                ErrorMessage = $"Registration failed: {ex.Message}";
                IsBusy = false;
                return;
            }


            IsBusy = false;
            // Navigate to Login
            //await Shell.Current.GoToAsync("..");
            //await Shell.Current.GoToAsync(nameof(MainPage));
        }

        [RelayCommand]
        private async Task GoToLoginAsync()
        {
           // await Shell.Current.GoToAsync("..");
            //await Shell.Current.GoToAsync(nameof(MainPage));
            await Shell.Current.GoToAsync("//MainPage"); // Navigates to the root of the Page
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("UserType", out var userTypes))
            {
                UserType_para = userTypes as string;
                if(UserType_para=="Driver")
                {
                    IsDriver = true;
                }
                else
                {
                    IsDriver = false;
                }
                   
            }
        }

    }
}
