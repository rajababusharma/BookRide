using BookRide.eNum;
using BookRide.Models;
using BookRide.Services;
using BookRide.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookRide.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly RealtimeDatabaseService _db;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
        public bool IsNotBusy => !IsBusy;

        public MainPageViewModel()
        {
            _db = new RealtimeDatabaseService();
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            await Task.Delay(1500); // simulate API call

            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Username and Password are required";
                IsBusy = false;
                return;
            }

            var usr = await _db.GetAsync<Users>($"Users/{Username}");
            if (usr != null)
            {

                if (Username != usr.UserId|| Password != usr.Password)
                {
                    ErrorMessage = "Invalid username or password";
                    IsBusy = false;
                    return;
                }
            }

            // Navigate to home page
            IsBusy = false;

            if(usr.UserType.Equals(eNumUserType.Driver.ToString()))
            {
                    await Shell.Current.GoToAsync(nameof(DriverProfilePage), true, new Dictionary<string, object>
                {
                    { "CurrentUser", usr }
                });
            }
            else if (usr.UserType.Equals(eNumUserType.Traveller.ToString()))
            {
                await Shell.Current.GoToAsync(nameof(TravellerProfilePage), true, new Dictionary<string, object>
                {
                    { "CurrentUser", usr }
                });
            }
            else
            {
                ErrorMessage = "Unknown user type";
            }
          
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            await Task.Delay(1500); // simulate API call



            // Navigate to register page
            IsBusy = false;
            await Shell.Current.GoToAsync(nameof(RegisterPage));

          //  await Shell.Current.GoToAsync(nameof(RegistrationConfirmationPage));

        }

        [RelayCommand]
        private async Task ForgotPasswordAsync()
        {
            await Shell.Current.DisplayAlert(
                "Forgot Password",
                "Password recovery flow here",
                "OK");
        }
    }
}
