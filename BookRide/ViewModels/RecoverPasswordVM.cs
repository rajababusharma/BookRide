using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Services;
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

        [ObservableProperty]
        private string selectedUserType;

        private Users users;
        private Drivers drivers;

        private bool isDriver;

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
                if(SelectedUserType==null)
                {
                    await Shell.Current.DisplayAlert("Error", "Please select a user type.", "OK");
                    IsBusy = false;
                    IsReadOnly = false;
                    return;
                }
                // Fetch all users from the database
                if (SelectedUserType == eNum.eNumUserType.Driver.ToString())
                {

                   var drs = await _db.GetAsync<Drivers>($"Drivers/{Mobile}");
                    var userDict = drs as IDictionary<string, object>;

                     drivers = new Drivers
                    {
                        UserId = userDict["UserId"].ToString(),
                        CreditPoint = Convert.ToInt32(userDict["CreditPoint"]),
                        FirstName = userDict["FirstName"].ToString(),
                        Age = Convert.ToInt32(userDict["Age"]),
                        Address = userDict["Address"].ToString(),
                        Mobile = userDict["Mobile"].ToString(),
                        Password = userDict["Password"].ToString(),
                        VehicleNo = userDict["VehicleNo"].ToString(),
                        VehicleType = userDict["VehicleType"].ToString(),
                        State = userDict["State"].ToString(),
                        District = userDict["District"].ToString(),
                        RegistrationDate = Convert.ToDateTime(userDict["RegistrationDate"]),
                        AadharImageURL = userDict["AadharImageURL"]?.ToString(),
                        ProfileImageUrl = userDict["ProfileImageUrl"]?.ToString(),
                        IsActive = Convert.ToBoolean(userDict["IsActive"]),


                        // add other properties as needed
                    };

                    if (drivers != null)
                    {
                        isDriver = true;
                        IsBusy = false;
                        IsPasswordVisible = true;
                        IsReadOnly = true;
                        IsVisible = false;
                        return;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Alert", "Driver with the provided mobile number not found.", "Ok");
                        IsBusy = false;
                        IsReadOnly = false;
                        return;
                    }
                }
               else if(SelectedUserType == eNum.eNumUserType.Traveler.ToString())
                {
                   var usr = await _db.GetAsync<Users>($"Users/{Mobile}");
                    var userDict = usr as IDictionary<string, object>;

                     users = new Users
                    {
                        UserId = userDict["UserId"].ToString(),
                        FirstName = userDict["FirstName"].ToString(),
                        Age = Convert.ToInt32(userDict["Age"]),
                        Address = userDict["Address"].ToString(),
                        Mobile = userDict["Mobile"].ToString(),
                        Password = userDict["Password"].ToString(),
                        State = userDict["State"].ToString(),
                        District = userDict["District"].ToString(),
                        RegistrationDate = Convert.ToDateTime(userDict["RegistrationDate"]),
                        ProfileImageUrl = userDict["ProfileImageUrl"]?.ToString(),
                        IsActive = Convert.ToBoolean(userDict["IsActive"]),


                        // add other properties as needed
                    };
                    if (users != null)
                    {
                        isDriver = false;
                        IsBusy = false;
                        IsPasswordVisible = true;
                        IsReadOnly = true;
                        IsVisible = false;
                        return;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Alert", "User with the provided mobile number not found.", "Ok");
                        IsBusy = false;
                        IsReadOnly = false;
                        return;
                    }
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
           
            if (isDriver)
            {
                drivers.Password = Password;
                var status = await Task.Run(() => _db.SaveAsync<Drivers>($"Drivers/{drivers.UserId}", drivers));

               // await _db.SaveAsync<Drivers>($"Drivers/{drivers.UserId}", drivers);
               if (status)
                {
                    await Shell.Current.DisplayAlert("Success", "Password changed successfully.", "OK");
                    IsBusy = false;
                    await Shell.Current.GoToAsync("//MainPage"); // Navigates to the root of the Page
                    return;                   
                }   
               else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to change password. Please try again.", "OK");
                    IsBusy = false;
                    return;
                }
               
            }
            else
            {
                users.Password = Password;
                var status = await Task.Run(() => _db.SaveAsync<Users>($"Users/{users.UserId}", users));

                // await _db.SaveAsync<Drivers>($"Drivers/{drivers.UserId}", drivers);
                if (status)
                {
                    await Shell.Current.DisplayAlert("Success", "Password changed successfully.", "OK");
                    IsBusy = false;
                    await Shell.Current.GoToAsync("//MainPage"); // Navigates to the root of the Page
                    return;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to change password. Please try again.", "OK");
                    IsBusy = false;
                    return;
                }
               
            }
           
        }
    }
}
