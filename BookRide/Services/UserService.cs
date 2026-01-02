using BookRide.Interfaces;
using BookRide.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Services
{
    public class UserService : IUsers
    {
        private ObservableCollection<Users> UsersList;
        private RealtimeDatabaseService _db;

        public async Task<ObservableCollection<Users>> GetUsers()
        {
            _db = new RealtimeDatabaseService();
            // check internet connectivity first 
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType == NetworkAccess.Internet)
            {
                // Connection to the internet is available
                Console.WriteLine("Internet is connected!");
                try
                {
                    UsersList = await _db.GetAllAsync<Users>("Users").ContinueWith(t =>
                    {
                        var userList = t.Result.Where<Users>(x => x.CreditPoint > 0);

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
                    return null;
                }
                // Update UI, enable features, etc.
            }
            else if (accessType == NetworkAccess.ConstrainedInternet)
            {
                // Limited internet (e.g., captive portal)
                Console.WriteLine("Internet is limited.");
                return null;
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
                return null;
            }
            return UsersList;
        }
    }
}
