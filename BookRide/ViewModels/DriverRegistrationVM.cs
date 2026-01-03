using BookRide.eNum;
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
        [ObservableProperty]
        private string selectedDistrict;

        private double latitude { get; set; }= 0.0;
        private double longitude { get; set; }= 0.0;


        public DriverRegistrationVM()
        {
            _db = new RealtimeDatabaseService();
            States = new ObservableCollection<string>(IndiaStates.All);
            Districts = new ObservableCollection<string>(UttarPradeshDistricts.All);

           
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
            IsBusy = true;
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

            await Task.Delay(1500); // Simulate API call

            try
            {
              
                var users = new Users
                { FirstName = FirstName, LastName = LastName, Age = int.Parse(Age), Address = Address, Mobile = Mobile, Password = Password, VehicleNo = VehicleNo, AadharCard = AadharCard, DrivingLicense = DrivingLicense, UserType = UserType_para, CreditPoint = CreditPoint, UserId=Mobile, VehicleType=SelectedVehicle,Latitude=latitude,Longitude=longitude,District=SelectedDistrict,RegistrationDate=DateTime.Now };

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
