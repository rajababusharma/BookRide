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
using System.Text.Json;
using Org.Apache.Http.Authentication;
using System.Net;
using AndroidX.Lifecycle;


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
        private readonly FirebaseAuthService _authService;

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
            _authService = new FirebaseAuthService();
            _networkService = networkService;
            //   _creditPointService = creditPointService;
            // _db.DeleteAllAsync();

            // Task.Run(async ()=>
            //{
            //    await _authService.GetTokenAsync(Constants.Constants.Firebase_UserId, Constants.Constants.Firebase_Userpwd);
               
            //    });

        }
        public async Task StartTracking(string userid)
        {
            await LocationPermissionHelper.CheckGPSLocationEnableAsync();
            try
            {
               
                    // start forground service to decrease credit point for driver daily and to update location
                    Console.WriteLine("Starting Foreground Services...");
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
                        Console.WriteLine($"Line: 88 MainPageVM Error starting services: {ex.Message}");
                        // Handle exceptions related to starting the service
                        // System.Diagnostics.Debug.WriteLine($"Error starting DriverCreditPointService: {ex.Message}");
                    }
                    }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Line: 98 MainPageVM Error starting services: {ex.Message}");
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
            await LocationPermissionHelper.CheckGPSLocationEnableAsync();
            await LocationPermissionHelper.HasPermissionsAsync();

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

                // Getting Firebase token
                if (string.IsNullOrEmpty(await SecureStorage.GetAsync(Constants.Constants.Firebase_TokenKeyValue)))
                {
                    Console.WriteLine("Fetching new Firebase token...");
                    await _authService.GetTokenAsync(Constants.Constants.Firebase_UserId, Constants.Constants.Firebase_Userpwd);
                }

                if (SelectedUserType.Equals(eNumUserType.Driver.ToString()))
                {



                    // var drs = await Task.Run(()=> _db.GetAsync<Drivers>($"Drivers/{Username}"));
                    //putting the GetAsync<Drivers>($"Drivers/{Username}") inside try catch to handle exception if user not found
                    var drs = new Drivers();
                    try
                    {
                        drs = await _db.GetAsync<Drivers>($"Drivers/{Username}");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Line: 165 MainPageVM Error fetching driver data: {ex.Message}");
                        await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                    }

                   
                    //     var userDict = drs as IDictionary<string, object>;

                    //Drivers userObj = new Drivers
                    //{
                    //    UserId = userDict["UserId"].ToString(),
                    //    CreditPoint = Convert.ToInt32(userDict["CreditPoint"]),
                    //    FirstName = userDict["FirstName"].ToString(),
                    //    Age = Convert.ToInt32(userDict["Age"]),
                    //    Address = userDict["Address"].ToString(),
                    //    Mobile = userDict["Mobile"].ToString(),
                    //    Password = userDict["Password"].ToString(),
                    //    VehicleNo = userDict["VehicleNo"].ToString(),
                    //    VehicleType = userDict["VehicleType"].ToString(),
                    //    State = userDict["State"].ToString(),
                    //    District = userDict["District"].ToString(),
                    //    RegistrationDate = Convert.ToDateTime(userDict["RegistrationDate"]),
                    //    AadharImageURL = userDict["AadharImageURL"]?.ToString(),
                    //    ProfileImageUrl = userDict["ProfileImageUrl"]?.ToString(),
                    //    IsActive = Convert.ToBoolean(userDict["IsActive"]),


                    //    // add other properties as needed
                    //};

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
                           // await SecureStorage.SetAsync(Constants.Constants.LoggedInUser, drs.UserId);
                           await SecureStorageService.SaveAsync<DateTime>(Constants.Constants.SessionStartTime, DateTime.Now);
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
                    var usr = new Users();
                    try
                    {
                        usr = await _db.GetAsync<Users>($"Users/{Username}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Line: 231 MainPageVM Error fetching users data: {ex.Message}");
                        await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                    }
                    
                    // cast a dictionary to an object of Drivers
                    //var userDict = usr as IDictionary<string, object>;

                    //Users userObj = new Users
                    //{
                    //    UserId = userDict["UserId"].ToString(),
                    //    FirstName = userDict["FirstName"].ToString(),
                    //    Age = Convert.ToInt32(userDict["Age"]),
                    //    Address = userDict["Address"].ToString(),
                    //    Mobile = userDict["Mobile"].ToString(),
                    //    Password = userDict["Password"].ToString(),
                    //    State = userDict["State"].ToString(),
                    //    District = userDict["District"].ToString(),
                    //    RegistrationDate = Convert.ToDateTime(userDict["RegistrationDate"]),
                    //    ProfileImageUrl = userDict["ProfileImageUrl"]?.ToString(),
                    //    IsActive = Convert.ToBoolean(userDict["IsActive"]),


                    //    // add other properties as needed
                    //};
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
                            await SecureStorageService.SaveAsync<Users>(Constants.Constants.LoggedInUser, usr);
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
            // Getting Firebase token
            if (string.IsNullOrEmpty(await SecureStorage.GetAsync(Constants.Constants.Firebase_TokenKeyValue)))
            {
                Console.WriteLine("Fetching new Firebase token...");
                await _authService.GetTokenAsync(Constants.Constants.Firebase_UserId, Constants.Constants.Firebase_Userpwd);
            }
            await Shell.Current.GoToAsync(nameof(RegisterPage));
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
