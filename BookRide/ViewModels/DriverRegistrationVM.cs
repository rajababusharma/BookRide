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
        private readonly FirebaseAuthService _authService;
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty] private string firstName;
        [ObservableProperty] private string age;
        [ObservableProperty] private string address;
        [ObservableProperty] private string mobile;
        [ObservableProperty] private string password;
        [ObservableProperty] private string emailAddress;

        [ObservableProperty] private string confirmPassword;
      
        [ObservableProperty] private string  vehicleNo;
        //[ObservableProperty] private string aadharCard;
        //[ObservableProperty] private string drivingLicense;

        [ObservableProperty] private string userType;
        [ObservableProperty] private int creditPoint = 61;
        [ObservableProperty] private string userType_para;

        [ObservableProperty] private string errorMessage;
        [ObservableProperty] private bool isDriver;

        [ObservableProperty] private string selectedVehicle;

        [ObservableProperty] private string selectedDistrict;
        [ObservableProperty] private string aadharImagePath;
        [ObservableProperty] private string aadharImageURL;
        [ObservableProperty] private string profileImageUrl;
        [ObservableProperty] private bool isTermsAccepted;
        // private string aadharImageURL;


        private readonly INetworkService _networkService;

        private readonly IFirebaseUpload _firebaseUpload;
        public DriverRegistrationVM(INetworkService networkService,ICurrentAddress currentAddress,IFirebaseUpload firebaseUpload)
        {
            _db = new RealtimeDatabaseService();
            _authService = new FirebaseAuthService();
          //  States = new ObservableCollection<string>(IndiaStates.All);
            Districts = new ObservableCollection<string>(UttarPradeshDistricts.All);
      
            _networkService = networkService;
            _firebaseUpload = firebaseUpload;
            IsBusy = false;

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
            await LocationPermissionHelper.HasPermissionsAsync();
            if (!IsTermsAccepted)
            {
                Shell.Current.DisplayAlert("Alert", "Please select terms and conditions first", "OK");
                IsBusy = false;
                return;
            }
           
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

                if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(Age) ||
                string.IsNullOrWhiteSpace(Address) ||
                string.IsNullOrWhiteSpace(Mobile) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword) ||
                string.IsNullOrWhiteSpace(SelectedDistrict) ||
                string.IsNullOrWhiteSpace(VehicleNo) ||
                string.IsNullOrWhiteSpace(AadharImagePath) ||
                string.IsNullOrWhiteSpace(SelectedVehicle))
                {
                    ErrorMessage = "All fields are required";
                    IsBusy = false;
                    return;
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

                    var drivers = new Drivers
                    {
                        FirstName = FirstName,
                        Age = int.Parse(Age),
                        Address = Address,
                        Mobile = Mobile,
                        EmailAddress=EmailAddress,
                        Password = Password,
                        VehicleNo = VehicleNo,
                        CreditPoint = CreditPoint,
                        UserId = Mobile,
                        VehicleType = SelectedVehicle,
                        District = SelectedDistrict,
                        RegistrationDate = DateTime.Now,
                        AadharImageURL = AadharImageURL,
                        ProfileImageUrl = ProfileImageUrl
                    };

                //var result = await _authService.RegisterAsync(drivers.DEmail, drivers.Password);
                //if (result == null)
                //{
                //    ErrorMessage = $"Authentication registration failed";
                //    IsBusy = false;
                //    return;
                //}
                // var status = await Task.Run(() =>

              var status=  await _db.SaveAsync<Drivers>($"Drivers/{drivers.UserId}", drivers);
                    

               // );
               // await _db.SaveAsync<Drivers>($"Drivers/{drivers.UserId}", drivers);
               if (!status)
                {
                   
                    Console.WriteLine("Failed to save driver data to the database.");
                }
                else
                {
                  
                    Console.WriteLine("Driver registration data saved successfully.");
                    await Shell.Current.DisplayAlert(
                      "Success",
                      "Registration completed successfully",
                      "OK");
                }
                Console.WriteLine("Driver registration data saved to Realtime Database.");
              
                Console.WriteLine("Starting foreground services for location updates and credit point management.");
                try
                    {

                        System.Diagnostics.Trace.WriteLine("Starting DriverCreditPointService for driver: " + drivers.UserId);             
                        // Start foreground services here
#if ANDROID
                        var intent_loc = new Intent(Application.Context, typeof(HourlyLocationService));
                        intent_loc.PutExtra("USERID", drivers.UserId);
                        // intent_loc.PutExtra("ISSERVICE1", System.Text.Json.JsonSerializer.Serialize(users));

                        Application.Context.StartForegroundService(intent_loc);
#endif

#if ANDROID
                        var intent_credit = new Intent(Application.Context, typeof(DailyBasisCreditPointService));
                        intent_credit.PutExtra("USERID", drivers.UserId);
                        Application.Context.StartForegroundService(intent_credit);
#endif
                        // await LocationPermissionHelper.HasPermissionsAsync();

                        // _foregroundService.Start(users.Mobile);
               

                }
                    catch (Exception ex)
                    {
                        // Handle exceptions related to starting the service
                          System.Diagnostics.Trace.WriteLine($"Error starting DriverCreditPointService: {ex.Message}");
                    }
                

                IsBusy = false;

                // start forground service to decrease credit point for driver daily and to update location

               

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

        [RelayCommand]
        private async Task OpenTermsConditionAsync()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("termsncondition.pdf");

            var filePath = Path.Combine(FileSystem.CacheDirectory, "termsncondition.pdf");

            using var fileStream = File.Create(filePath);
            await stream.CopyToAsync(fileStream);

            await Launcher.Default.OpenAsync(
                new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
           var user = query["CurrentUser"] as Drivers;
            if (user == null)
            {
                return;
            }

            //  Drivers user = usr as Drivers;
            Task.Run(() =>
            {
                Mobile = user.Mobile;
                EmailAddress = user.EmailAddress;
                FirstName = user.FirstName;
                Age = user.Age.ToString();
                Address = user.Address;
                VehicleNo = user.VehicleNo;
                SelectedDistrict = user.District;
                SelectedVehicle = user.VehicleType;
                Password = user.Password;
                ConfirmPassword = user.Password;
                AadharImagePath = user.AadharImageURL;
                AadharImageURL = user.AadharImageURL;
                CreditPoint = user.CreditPoint;
                ProfileImageUrl = user.ProfileImageUrl;
            });
                
            
        }

        [RelayCommand]
        public async Task UploadPhotoAsync()
        {
            try
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
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to upload image: {ex.Message}", "OK");
               
            }
        }

    }
}
