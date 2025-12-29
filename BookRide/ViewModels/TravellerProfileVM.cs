using BookRide.Models;
using BookRide.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookRide.ViewModels
{
    public partial class TravellerProfileVM : ObservableObject, IQueryAttributable
    {
        private readonly RealtimeDatabaseService _db;

        [ObservableProperty]
        public ObservableCollection<Users> usersList = new();

        [ObservableProperty]
        public Users user;
        [ObservableProperty]
        public string hi="Hi";

        [RelayCommand]
        public async void Call(string phoneNumber)
        {
            if (phoneNumber != null && phoneNumber.Length == 10)
            {

                try
                {
                    //  PhoneDialer.Default.Open(phoneNumber);
                    await Launcher.Default.OpenAsync("tel:+91" + phoneNumber);

                }
                catch (Exception exp)
                {
                    // Handle error
                    await Shell.Current.DisplayAlert(
                           "Error",
                           exp.Message,
                           "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert(
                          "Error",
                          "Incorrect phone number",
                          "OK");
            }
        }

        public TravellerProfileVM()
        {
            _db = new RealtimeDatabaseService();
            LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            // check internet connectivity first 
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType == NetworkAccess.Internet)
            {
                // Connection to the internet is available
                Console.WriteLine("Internet is connected!"); try
                {
                    UsersList = await _db.GetAllAsync<Users>("Users").ContinueWith(t =>
                    {
                        var userList = t.Result;
                        return new ObservableCollection<Users>(userList);
                    });
                }
                catch (System.AggregateException exp)
                {
                    // Handle error
                    await Shell.Current.DisplayAlert(
                           "Error",
                           exp.Message,
                           "OK");
                }
                catch (Exception exp)
                {
                    // Handle error
                    await Shell.Current.DisplayAlert(
                           "Error",
                           exp.Message,
                           "OK");
                }
                // Update UI, enable features, etc.
            }
            else if (accessType == NetworkAccess.ConstrainedInternet)
            {
                // Limited internet (e.g., captive portal)
                Console.WriteLine("Internet is limited.");
            }
            else
            {
                // No internet connection
                Console.WriteLine("No internet connection.");
                // Show an alert or disable features
              
                await Shell.Current.DisplayAlert(
                         "Offline",
                         "Please check your internet connection",
                         "OK");
            }
         

        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            User = query["CurrentUser"] as Users;
        }
    }
}
