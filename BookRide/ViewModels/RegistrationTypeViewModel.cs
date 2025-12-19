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
    public partial class RegistrationTypeViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task DriverAsync()
        {
            // Navigate to Driver Registration Page
            await Shell.Current.GoToAsync(nameof(DriverRegistration));
        }
        [RelayCommand]
        private async Task TravellerAsync()
        {
            // Navigate to Traveller Registration Page
            await Shell.Current.GoToAsync(nameof(TravellerRegistration));
        }
    }
}
