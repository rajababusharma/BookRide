using Android.Locations;
using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Platforms.Android.Implementations;
using BookRide.Services;
using BookRide.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliJ.Lang.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.ViewModels
{
    public partial class UserRegistrationVM : ObservableObject,IQueryAttributable
    {
        public ObservableCollection<string> States { get; }
        public ObservableCollection<string> Districts { get; }
        private readonly RealtimeDatabaseService _db;
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty] private string firstName;
        [ObservableProperty] private string age;
        [ObservableProperty] private string address;
        [ObservableProperty] private string mobile;
        [ObservableProperty] private string password;

        [ObservableProperty] private string confirmPassword;   

        [ObservableProperty] private string errorMessage;

        [ObservableProperty] private string selectedDistrict;
        // private string aadharImageURL;


        private readonly INetworkService _networkService;

        private readonly IFirebaseUpload _firebaseUpload;
        public UserRegistrationVM(INetworkService networkService)
        {
            _db = new RealtimeDatabaseService();
            States = new ObservableCollection<string>(IndiaStates.All);
            Districts = new ObservableCollection<string>(UttarPradeshDistricts.All);

            _networkService = networkService;

        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

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

                if (string.IsNullOrWhiteSpace(FirstName) ||
               string.IsNullOrWhiteSpace(Age) ||
               string.IsNullOrWhiteSpace(Address) ||
               string.IsNullOrWhiteSpace(Mobile))
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

                    var users = new Users
                    {
                        FirstName = FirstName,
                        Age = int.Parse(Age),
                        Address = Address,
                        Mobile = Mobile,
                        Password = Password,
                        UserId = Mobile,
                        District = SelectedDistrict,
                        RegistrationDate = DateTime.Now
                    };

                    await _db.SaveAsync<Users>($"Users/{users.UserId}", users);
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
           
            if (query.TryGetValue("USERS", out var usr))
            {
                Users user = usr as Users;
                Mobile = user.Mobile;
                FirstName = user.FirstName;
                Age = user.Age.ToString();
                Address = user.Address;
                SelectedDistrict = user.District;
                Password = user.Password;
                ConfirmPassword = user.Password;
            }
        }

    }
}
