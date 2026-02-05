using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace BookRide.Services
{
    public static class LocationPermissionHelper
    {
        //public static async Task<bool> EnsurePermissionsAsync()
        //{
        //    var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
        //
        //    if (status != PermissionStatus.Granted)
        //        status = await Permissions.RequestAsync<Permissions.LocationAlways>();
        //
        //    return status == PermissionStatus.Granted;
        //}

        public static async Task CheckGPSLocationEnableAsync()
        {
            bool isLocationEnabled = IsLocationEnabled();

            if (!isLocationEnabled)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Location Disabled",
                    "Please enable location services.",
                    "Open Settings");

                OpenLocationSettings();
                return;
            }
        }

        public static bool IsLocationEnabled()
        {
#if ANDROID
            var context = Android.App.Application.Context;
            var locationManager = (Android.Locations.LocationManager)context.GetSystemService(Android.Content.Context.LocationService);
            if (locationManager == null)
                return false;

            return locationManager.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider)
                || locationManager.IsProviderEnabled(Android.Locations.LocationManager.NetworkProvider);
#elif IOS
            return CoreLocation.CLLocationManager.LocationServicesEnabled;
#else
            // For other platforms assume enabled (or implement platform-specific checks if needed)
            return true;
#endif
        }

        public static void OpenLocationSettings()
        {
        #if ANDROID
                var intent = new Android.Content.Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                intent.SetFlags(Android.Content.ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
        #elif IOS
                // On iOS you can open app settings; user must enable Location Services in Settings
                var url = new Foundation.NSUrl("App-Prefs:root=Privacy&path=LOCATION");
                if (UIKit.UIApplication.SharedApplication.CanOpenUrl(url))
                    UIKit.UIApplication.SharedApplication.OpenUrl(url);
        #endif
        }

        public static async Task HasPermissionsAsync()
        {
           // var alwaysinuse = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            var alwaysinuse = await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                return await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            });

            if (alwaysinuse != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert(
                 "Permission Required",
                 "This app collects location data to enable real-time ride tracking, driver matching, and trip safety features even when the app is closed or not in use.\n Your location is used to: \n" + "1.>" + "Find nearby drivers faster.\n" + "2.>" + " Track rides in real time. \n" + "3.>" + " Improve pickup accuracy",
                 "OK");

                alwaysinuse = await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    return await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                });
              //  alwaysinuse = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (alwaysinuse == PermissionStatus.Granted)
                {
                #if ANDROID
                   await MainThread.InvokeOnMainThreadAsync(async () =>
                                    await Permissions.RequestAsync<Permissions.LocationAlways>());
                #endif
                }
            }

        }
    }
}
