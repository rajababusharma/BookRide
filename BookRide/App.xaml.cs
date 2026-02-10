using BookRide.Models;
using BookRide.Services;
using BookRide.Views;
using Microsoft.Maui.Controls;
using System.Net;

namespace BookRide
{
    public partial class App : Application
    {
       private readonly RealtimeDatabaseService _db;
        private readonly FirebaseAuthService _authService;
        public App(AppShell shell,FirebaseAuthService firebaseAuth, RealtimeDatabaseService databaseService)
        {
            InitializeComponent();
            // MainPage = shell;
            _authService = firebaseAuth;
            _db = databaseService;
           // _db = new RealtimeDatabaseService();
            // Navigate to Login page on app start
            //  Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            // 1. Catch unhandled exceptions from the main application domain
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // 2. Catch unobserved exceptions from tasks (e.g., async operations without await)
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

//#if ANDROID
//            DependencyService.Register<Platforms.Android.Implementations.Test>();
//#endif

            MainPage = new AppShell();
           // SetStartPage();
        }

        private async void SetStartPage()
        {
            var session_dr = await SecureStorageService.GetAsync<Drivers>(Constants.Constants.LoggedInUser);
            var session_tr = await SecureStorageService.GetAsync<Users>(Constants.Constants.LoggedInUser);

            if (session_dr == null && session_tr==null)
            {
                // 🔥 User already logged in → Home
                await Shell.Current.GoToAsync("//MainPage");
            }
            else if (session_dr != null)
            {
                // ✅ Logged in as Driver → Driver Home
              //  await Shell.Current.GoToAsync("//DriverProfilePage");
                await Shell.Current.GoToAsync("//DriverProfilePage", true, new Dictionary<string, object>
                            {
                                { "CurrentUser", session_dr }
                            });
            }
            else if (session_tr != null)
            {
                // ✅ Logged in as Traveler → Traveler Home
              //  await Shell.Current.GoToAsync("//TravellerProfilePage");
                await Shell.Current.GoToAsync("//TravellerProfilePage", true, new Dictionary<string, object>
                            {
                                { "CurrentUser", session_tr }
                            });
            }
            else
            {
                // ❌ Not logged in → Login
                await Shell.Current.GoToAsync("//MainPage");
            }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            var exception = e.Exception;
            // Log the exception details
            // Mark the exception as observed to prevent the app from crashing immediately
            e.SetObserved();
          
          
            // Optionally, display a user-friendly alert
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                ExceptionClass excp = new ExceptionClass { Message = exception.Message, StackTrace = exception.StackTrace, OccurredAt = DateTime.Now };
                await _db.SaveAsync($"Exceptions/{Guid.NewGuid()}", excp);
                //  await MainPage.DisplayAlert("Error", "An error occurred in a background task.", "OK");
                //  await Shell.Current.DisplayAlert("Error", "An error occurred in a background task.", "OK");

                //  await Shell.Current.DisplayAlert("Error", exception.Message, "OK");
            });
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            // Log the exception details using a logging service (e.g., Application Insights, Sentry)
            System.Diagnostics.Debug.WriteLine($"Unhandled AppDomain Exception: {exception}");
           
            // Display a user-friendly alert on the UI thread
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                ExceptionClass excp = new ExceptionClass { Message = exception.Message, StackTrace = exception.StackTrace, OccurredAt = DateTime.Now };
                await _db.SaveAsync($"Exceptions/{Guid.NewGuid()}", excp);
                // await Shell.Current.DisplayAlert("Error", exception.Message, "OK");
                //  await Shell.Current.DisplayAlert("Error", "Something unexpected happened. The app might need to close.", "OK");
            });

            // Note: The app may still terminate after this handler runs.
        }

        override async protected void OnStart()
        {
            // Handle when your app starts
            try
            {
                var token = await _authService.GetValidTokenAsync();

                // Token valid → go to Home Page
            }
            catch
            {
                // Refresh failed → go to Login Page
            }

        }

    }
}
