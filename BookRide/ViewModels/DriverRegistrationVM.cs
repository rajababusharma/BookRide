using BookRide.Models;
using BookRide.Services;
using BookRide.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.ViewModels
{
    public partial  class DriverRegistrationVM : ObservableObject,IQueryAttributable
    {
        private readonly RealtimeDatabaseService _db;
       
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

        public DriverRegistrationVM()
        {
            _db = new RealtimeDatabaseService();
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        [RelayCommand]
        private async Task RegisterAsync()
        {
            ErrorMessage = string.Empty;

            if(UserType_para=="Driver")
            {
                if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(Age) ||
                string.IsNullOrWhiteSpace(Address) ||
                string.IsNullOrWhiteSpace(Mobile) ||
                string.IsNullOrWhiteSpace(VehicleNo) ||
                string.IsNullOrWhiteSpace(DrivingLicense))
                {
                    ErrorMessage = "All fields are required";

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

                    return;
                }
            }

           

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                return;
            }

            await Task.Delay(1500); // Simulate API call

            try
            {
                var users = new Users
                { FirstName = FirstName, LastName = LastName, Age = int.Parse(Age), Address = Address, Mobile = Mobile, Password = Password, VehicleNo = VehicleNo, AadharCard = AadharCard, DrivingLicense = DrivingLicense, UserType = UserType_para, CreditPoint = CreditPoint, UserId=Mobile, VehicleType=SelectedVehicle };

                var usr=await _db.GetAsync<Users>($"Users/{users.UserId}");
                if(usr!=null)
                {
                    ErrorMessage = "User with this mobile number already exists";
                    await Shell.Current.DisplayAlert(
                  "Alert",
                  "User with this mobile number already exists",
                  "OK");
                    return;
                }   

                await _db.SaveAsync($"Users/{users.UserId}", users);
                await Shell.Current.DisplayAlert(
                    "Success",
                    "Registration completed successfully",
                    "OK");
                await Shell.Current.GoToAsync(nameof(RegistrationConfirmationPage));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Registration failed: {ex.Message}";
                return;
            }

           

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
