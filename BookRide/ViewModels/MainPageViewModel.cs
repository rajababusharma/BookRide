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

        [RelayCommand]
        private async Task LoginAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            await Task.Delay(1500); // simulate API call

            if (Username != "admin" || Password != "1234")
            {
                ErrorMessage = "Invalid username or password";
                IsBusy = false;
                return;
            }

            // Navigate to home page
            IsBusy = false;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            await Task.Delay(1500); // simulate API call



            // Navigate to register page

            await Shell.Current.GoToAsync(nameof(RegisterPage));
            IsBusy = false;
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
