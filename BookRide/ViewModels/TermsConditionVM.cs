using BookRide.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.ViewModels
{
    public partial class TermsConditionVM : ObservableObject
    {
        private readonly INetworkService _networkService;
        public TermsConditionVM(INetworkService networkService)
        {
            _networkService = networkService;
        }
        [RelayCommand]
        private async Task OpenWebsiteAsync(string url)
        {
            // check internet connectivity first 
            if (!_networkService.HasInternet())
            {
                await Shell.Current.DisplayAlert("No Internet", "Please check your internet connection and try again.", "OK");
                // ErrorMessage = "No internet connection. Please check your connection and try again.";
               // IsBusy = false;
                return;
            }
            try
            {
                Uri uri = new Uri(url);
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors, such as no browser installed on the device.
                await Shell.Current.DisplayAlert("Error", $"Could not open URL: {ex.Message}", "OK");
            }
        }
    }
}
