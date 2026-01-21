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
using Android.Views.ContentCaptures;


#if ANDROID
using Android.Content;
using Application = Android.App.Application;
using BookRide.Platforms.Android.Implementations;
#endif
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

        [ObservableProperty]
        private string selectedUserType;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
        public bool IsNotBusy => !IsBusy;

        private readonly INetworkService _networkService;
        public MainPageViewModel(INetworkService networkService)
        {
           // _foregroundService = foregroundService;
            _db = new RealtimeDatabaseService();
            _networkService = networkService;
         //   _creditPointService = creditPointService;
            // _db.DeleteAllAsync();


        }
        public async Task StartTracking(string userid)
        {
            await LocationPermissionHelper.CheckGPSLocationEnableAsync();
            try
            {
               
                    // start forground service to decrease credit point for driver daily and to update location
                    if (SelectedUserType.Equals(eNum.eNumUserType.Driver.ToString()))
                    {
                        try
                        {
                        //_creditPointService.Start(users.Mobile);
                        //_foregroundService.Start(users.Mobile);
#if ANDROID
                        var intent_loc = new Intent(Application.Context, typeof(HourlyLocationService));
                        intent_loc.PutExtra("USERID", userid);
                        Application.Context.StartForegroundService(intent_loc);
#endif

#if ANDROID
                        var intent_credit = new Intent(Application.Context, typeof(DailyBasisCreditPointService));
                        intent_credit.PutExtra("USERID", userid);
                        Application.Context.StartForegroundService(intent_credit);
#endif

                    }
                    catch (Exception ex)
                        {
                            // Handle exceptions related to starting the service
                           // System.Diagnostics.Debug.WriteLine($"Error starting DriverCreditPointService: {ex.Message}");
                        }
                    }


            }
            catch (Exception ex)
            {
                // displaying an error alert message
              //  await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
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

            if (string.IsNullOrWhiteSpace(SelectedUserType))
            {
                ErrorMessage = "Please select user type";
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
              
                if(SelectedUserType.Equals(eNumUserType.Driver.ToString()))
                {
                    var drs = await _db.GetAsync<Drivers>($"Drivers/{Username}");
                    if (drs != null)
                    {

                        if (Username != drs.UserId || Password != drs.Password)
                        {
                            ErrorMessage = "Invalid username or password";
                            IsBusy = false;
                            return;
                        }
                        else
                        {
                            await LocationPermissionHelper.HasPermissionsAsync();
                            // starting a location tracking service
                            await StartTracking(drs.UserId);

                            await Shell.Current.GoToAsync(nameof(DriverProfilePage), true, new Dictionary<string, object>
                            {
                                { "CurrentUser", drs }
                            });
                        }
                    }
                    else
                    {
                        ErrorMessage = "User does not exist.";
                        IsBusy = false;
                        return;
                    }
                }
                else
                {
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

                                await Shell.Current.GoToAsync(nameof(TravellerProfilePage), true, new Dictionary<string, object>
                                {
                                    { "CurrentUser", usr }
                                });

                        }
                    }
                    else
                    {
                        ErrorMessage = "User does not exist.";
                        IsBusy = false;
                        return;
                    }
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
