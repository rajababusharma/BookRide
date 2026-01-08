using BookRide.eNum;
using BookRide.Interfaces;
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
        private readonly Interfaces.IForegroundService _foregroundService;
       private readonly INetworkService _networkService;
        public MainPageViewModel(Interfaces.IForegroundService foregroundService, INetworkService networkService)
        {
            _foregroundService = foregroundService;
            _db = new RealtimeDatabaseService();
            _networkService = networkService;
            // _db.DeleteAllAsync();


        }
       public async Task StartTracking(Users users)
        {
            try
            {
                if (await LocationPermissionHelper.EnsurePermissionsAsync())
                {
                    // start forground service to decrease credit point for driver daily and to update location
                    if (users.UserType == "Driver" && users.CreditPoint > 0)
                    {
                        try
                        {
                            if (await LocationPermissionHelper.EnsurePermissionsAsync())
                            {
                                _foregroundService.Start(users);
                            }

                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions related to starting the service
                            System.Diagnostics.Debug.WriteLine($"Error starting DriverCreditPointService: {ex.Message}");
                        }
                    }
                    else if (users.UserType == "Driver" && users.CreditPoint <= 0)
                    {
                        _foregroundService.Stop();

                    }
                }
            }
            catch (Exception ex)
            {
                // displaying an error alert message
                await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            }
           
           
                
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            // check internet connectivity first 
           
            if (!_networkService.HasInternet())
            {
                await Shell.Current.DisplayAlert("No Internet", "Please check your internet connection and try again.", "OK");
                IsBusy = false;
                return;
            }

            try
            {
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

                    if (Username != usr.UserId || Password != usr.Password)
                    {
                        ErrorMessage = "Invalid username or password";
                        IsBusy = false;
                        return;
                    }
                    else
                    {
                       

                        if (usr.UserType.Equals(eNumUserType.Driver.ToString()))
                        {
                            // starting a location tracking service
                            await StartTracking(usr);

                            await Shell.Current.GoToAsync(nameof(DriverProfilePage), true, new Dictionary<string, object>
                        {
                            { "CurrentUser", usr }
                        });
                        }
                        else if (usr.UserType.Equals(eNumUserType.Traveler.ToString()))
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
                }
                else
                {
                    ErrorMessage = "User does not exist.";
                    IsBusy = false;
                    return;
                }

                // Navigate to home page
                IsBusy = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                IsBusy = false;
                return;
            }
           
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            await Task.Delay(1500); // simulate API call

            // Navigate to terms and conditions page
            IsBusy = false;
           // await Shell.Current.GoToAsync(nameof(RegisterPage));
            await Shell.Current.GoToAsync(nameof(TermsConditionsPage));
        }

        [RelayCommand]
        private async Task ForgotPasswordAsync()
        {
            await Shell.Current.GoToAsync(nameof(RecoverPasswordPage));
        }

        [RelayCommand]
        public async Task OpenEmbeddedPdfAsync()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("guidance.pdf");

            var filePath = Path.Combine(FileSystem.CacheDirectory, "guidance.pdf");

            using var fileStream = File.Create(filePath);
            await stream.CopyToAsync(fileStream);

            await Launcher.Default.OpenAsync(
                new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
        }

    }
}
