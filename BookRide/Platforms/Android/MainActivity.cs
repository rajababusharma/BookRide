using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;
using BookRide.Interfaces;
using BookRide.Platforms.Android.Implementations;
using Microsoft.Maui.Controls;

namespace BookRide
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        override protected void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Tell Android we want edge-to-edge drawing
            WindowCompat.SetDecorFitsSystemWindows(Window, false);
         
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Display the status code alert


            //MainThread.BeginInvokeOnMainThread(async () =>
            //{
            //    // Access the current main page of the application
                
            //      //  await  DisplayAlert("Result", "An activity result was received.", "OK");
            //      await Shell.Current.DisplayAlert("Result", $"An activity result was received.{resultCode}", "OK");

            //});

            }
    }
}
