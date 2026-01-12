using BookRide.Models;
using BookRide.Services;
using Microsoft.Maui.Controls;

namespace BookRide
{
    public partial class App : Application
    {
       private readonly RealtimeDatabaseService _db;
        public App(AppShell shell)
        {
            InitializeComponent();
            // MainPage = shell;
            _db = new RealtimeDatabaseService();
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
        }

        private async void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            var exception = e.Exception;
            // Log the exception details
            // Mark the exception as observed to prevent the app from crashing immediately
            e.SetObserved();
            ExceptionClass excp = new ExceptionClass { Message= exception.Message, StackTrace= exception.StackTrace, OccurredAt= DateTime.Now};
            await _db.SaveAsync($"Exceptions/{Guid.NewGuid()}", excp);
          
            // Optionally, display a user-friendly alert
            MainThread.BeginInvokeOnMainThread(async () =>
            {
              //  await MainPage.DisplayAlert("Error", "An error occurred in a background task.", "OK");
              //  await Shell.Current.DisplayAlert("Error", "An error occurred in a background task.", "OK");

                await Shell.Current.DisplayAlert("Error", exception.Message, "OK");
            });
        }

        private async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            // Log the exception details using a logging service (e.g., Application Insights, Sentry)
            System.Diagnostics.Debug.WriteLine($"Unhandled AppDomain Exception: {exception}");
            ExceptionClass excp = new ExceptionClass { Message = exception.Message, StackTrace = exception.StackTrace, OccurredAt = DateTime.Now };
            await _db.SaveAsync($"Exceptions/{Guid.NewGuid()}", excp);
            // Display a user-friendly alert on the UI thread
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Error", exception.Message, "OK");
                //  await Shell.Current.DisplayAlert("Error", "Something unexpected happened. The app might need to close.", "OK");
            });

            // Note: The app may still terminate after this handler runs.
        }

        
    }
}
