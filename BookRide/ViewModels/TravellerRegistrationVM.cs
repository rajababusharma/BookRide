using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookRide.Models;
using BookRide.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookRide.ViewModels
{

  public  partial class TravellerRegistrationVM : ObservableObject
    {
        private readonly RealtimeDatabaseService _db;
        public TravellerRegistrationVM()
        {
            _db = new RealtimeDatabaseService();
        }
        [ObservableProperty] private string firstName;
        [ObservableProperty] private string lastName;
        [ObservableProperty] private string age;
        [ObservableProperty] private string address;
        [ObservableProperty] private string mobile;
        [ObservableProperty] private string password;
        [ObservableProperty] private string confirmPassword;
        [ObservableProperty] private string errorMessage;

      
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        [RelayCommand]
        private async Task RegisterAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Age) ||
                string.IsNullOrWhiteSpace(Address) ||
                string.IsNullOrWhiteSpace(Mobile))
            {
                ErrorMessage = "All fields are required";
             
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                return;
            }

            await Task.Delay(1500); // Simulate API call

            var traveller=new Travellers {Id= int.Parse(Mobile), Address= Address, Age = int.Parse(Age), FirstName = FirstName, LastName = LastName, Password = Password };

            await _db.SaveAsync($"Travellers/{traveller.Id}", traveller);
            await Shell.Current.DisplayAlert(
                "Success",
                "Registration completed successfully",
                "OK");

            // Navigate to Login
           // await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
