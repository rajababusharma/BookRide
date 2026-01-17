using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Services;
using BookRide.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Storage;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
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
        [ObservableProperty] private string age;
        [ObservableProperty] private string address;
        [ObservableProperty] private string mobile;
        [ObservableProperty] private string password;

        [ObservableProperty] private string confirmPassword;
      
        [ObservableProperty] private string  vehicleNo;
        //[ObservableProperty] private string aadharCard;
        //[ObservableProperty] private string drivingLicense;

        [ObservableProperty] private string userType;
        [ObservableProperty] private int creditPoint = 30;
        [ObservableProperty] private string userType_para;

        [ObservableProperty] private string errorMessage;
        [ObservableProperty] private bool isDriver;

        [ObservableProperty] private string selectedVehicle;

        [ObservableProperty] private string selectedDistrict;
        [ObservableProperty] private string aadharImagePath;
        [ObservableProperty] private string aadharImageURL; 
       // private string aadharImageURL;
      

        private readonly INetworkService _networkService;

        private readonly IFirebaseUpload _firebaseUpload;
        public DriverRegistrationVM(INetworkService networkService,ICurrentAddress currentAddress,IFirebaseUpload firebaseUpload)
        {
            _db = new RealtimeDatabaseService();
            States = new ObservableCollection<string>(IndiaStates.All);
            Districts = new ObservableCollection<string>(UttarPradeshDistricts.All);
      
            _networkService = networkService;
            _firebaseUpload = firebaseUpload;

        }

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
                string.IsNullOrWhiteSpace(AadharImagePath) ||
                string.IsNullOrWhiteSpace(SelectedVehicle))
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

                var users = new Users
                { FirstName = FirstName, Age = int.Parse(Age), Address = Address, Mobile = Mobile, 
                    Password = Password, VehicleNo = VehicleNo,
                    UserType = UserType_para, CreditPoint = CreditPoint, 
                    UserId=Mobile, VehicleType=SelectedVehicle,
                    District =SelectedDistrict,
                    RegistrationDate=DateTime.Now,
                AadharImageURL= AadharImageURL};
           
                await _db.SaveAsync<Users>($"Users/{users.UserId}", users);
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
                        intent_loc.PutExtra("USERID", users.UserId);
                       // intent_loc.PutExtra("ISSERVICE1", System.Text.Json.JsonSerializer.Serialize(users));
                       
                        Application.Context.StartForegroundService(intent_loc);
#endif

#if ANDROID
                        var intent_credit = new Intent(Application.Context, typeof(DailyBasisCreditPointService));
                        intent_credit.PutExtra("USERID", users.UserId);
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
            if (query.TryGetValue("USER", out var usr))
            {
                Users user = usr as Users;
                Mobile = user.Mobile;
                FirstName = user.FirstName;
                Age = user.Age.ToString();
                Address = user.Address;
                VehicleNo = user.VehicleNo;
                SelectedDistrict = user.District;
                SelectedVehicle = user.VehicleType;
                UserType= user.UserType;
                Password = user.Password;
                ConfirmPassword= user.Password;
                UserType_para =user.UserType;
                AadharImagePath= user.AadharImageURL;
                AadharImageURL = user.AadharImageURL;
                if (UserType_para == "Driver")
                {
                    IsDriver = true;
                }
                else
                {
                    IsDriver = false;
                }
            }
        }

        [RelayCommand]
        public async Task UploadPhotoAsync()
        {
           
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Aadhar Card Image",
                    FileTypes = FilePickerFileType.Images
                });
                if (result == null)
                    return;
               
                    AadharImagePath = result.FileName;
                var imageStream = await result.OpenReadAsync();
                var imageurl = await _firebaseUpload.UploadAadharImagesToCloud(imageStream, Mobile);
                AadharImageURL = imageurl;         
        }

    }
}
