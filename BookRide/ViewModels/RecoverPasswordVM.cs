using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.ViewModels
{
    public partial class RecoverPasswordVM : ObservableObject
    {
        [ObservableProperty]
        private string mobile;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isReadOnly;

        [ObservableProperty]
        private bool isVisible;

        [ObservableProperty]
        private bool isPasswordVisible;
        private readonly RealtimeDatabaseService _db;

        [ObservableProperty]
        private string password;
        [ObservableProperty]
        private string confirmPassword;

        private Users Users;

        private readonly INetworkService _networkService;

        public RecoverPasswordVM(INetworkService networkService)
        {
            _db = new RealtimeDatabaseService();
            _networkService = networkService;
            isPasswordVisible = false;
            isReadOnly = false;
            IsVisible = true;
        }

        [RelayCommand]
        private async Task RecoverPasswordAsync()
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
                if (string.IsNullOrWhiteSpace(Mobile))
                {
                    // Handle empty mobile number case
                    IsBusy = false;
                    IsReadOnly = false;
                    return;
                }
                // Fetch all users from the database

                Users = await _db.GetAsync<Users>($"Users/{Mobile}");
                // Find the user with the matching mobile number
              //  var user = users.FirstOrDefault(u => u.Mobile == Mobie);
                if (Users != null)
                {
                    // Simulate sending password recovery instructions
                    // In a real application, you would send an email or SMS
                  //  Console.WriteLine($"Password recovery instructions sent to mobile: {Mobile}");
                    IsBusy = false;
                    IsPasswordVisible = true;
                    IsReadOnly = true;
                    IsVisible = false;
                }
                else
                {
                    // Handle case where user is not found
                    Console.WriteLine("User with the provided mobile number not found.");
                    await Shell.Current.DisplayAlert("Alert", "User with the provided mobile number not found.", "Ok");
                    IsBusy = false;
                    IsReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                IsReadOnly = false;
                // Handle exceptions (e.g., network issues)
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            IsBusy = true;
            if (!_networkService.HasInternet())
            {
                await Shell.Current.DisplayAlert("No Internet", "Please check your internet connection and try again.", "OK");
                // ErrorMessage = "No internet connection. Please check your connection and try again.";
                IsBusy = false;
                return;
            }
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                // Handle empty password fields
                await Shell.Current.DisplayAlert("Error", "Password fields cannot be empty.", "OK");
                IsBusy = false;
                return;
            }
            if (Password != ConfirmPassword)
            {
                // Handle password mismatch
                await Shell.Current.DisplayAlert("Error", "Passwords do not match.", "OK");
                IsBusy = false;
                return;
            }   
           
            Users.Password = Password;
            await _db.SaveAsync<Users>($"Users/{Users.UserId}", Users);
            await Shell.Current.DisplayAlert("Success", "Password changed successfully.", "OK");
            IsBusy = false;
            await Shell.Current.GoToAsync("//MainPage"); // Navigates to the root of the Page
        }
    }
}
