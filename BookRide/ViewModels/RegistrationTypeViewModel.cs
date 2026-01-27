using BookRide.Models;
using BookRide.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BookRide.ViewModels
{
    public partial class RegistrationTypeViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task DriverAsync()
        {
            // Navigate to Driver Registration Page
            //  await Shell.Current.GoToAsync(nameof(DriverRegistration));

            Drivers user = new Drivers();
            user = null;

            await Shell.Current.GoToAsync(nameof(DriverRegistration),
                    new Dictionary<string, object>
                    {
                        ["CurrentUser"] = user
                    });
        }
        [RelayCommand]
        private async Task TravellerAsync()
        {
            // Navigate to Traveler Registration Page
            // await Shell.Current.GoToAsync(nameof(UserRegistrationPage));
            Users user = new Users();
            user = null;

            await Shell.Current.GoToAsync(nameof(UserRegistrationPage),
                    new Dictionary<string, object>
                    {
                        ["CurrentUser"] = user
                    });
        }

    }
}
