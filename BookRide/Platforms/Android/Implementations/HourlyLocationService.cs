using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using AndroidX.Core.App;
using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Android.Graphics.ImageDecoder;

namespace BookRide.Platforms.Android.Implementations
{
    [Service
        ( Name = "com.companyname.bookride.HourlyLocationService",
    ForegroundServiceType = ForegroundService.TypeLocation,
    Exported = false
)]
    public class HourlyLocationService : Service
    {
        CancellationTokenSource _cts;
        public const string ACTION_STOP_LOCATION = "ACTION_STOP_LOCATION_SERVICE";

        private readonly RealtimeDatabaseService _db;
       
        private ICurrentAddress? _currentAddress;

        // Public parameterless constructor required by Android runtime (resolves XA4213)
        public HourlyLocationService()
        {
            _db = new RealtimeDatabaseService();
            _currentAddress=new CurrentAddressService();
          
        }
        //public HourlyLocationService(ICurrentAddress currentAddress)
        //{
           
        //   // _currentAddress = currentAddress;
        //}
        
        public override void OnCreate()
        {
            base.OnCreate();
            _cts = new CancellationTokenSource();
         
        }
      

        public override StartCommandResult OnStartCommand(
            Intent intent,
            StartCommandFlags flags,
            int startId)
        {
            //if (intent?.Action == ACTION_STOP_LOCATION)
            //{
            //    StopService();
            //    return StartCommandResult.NotSticky;
            //}
            StartForeground(1001, CreateNotification());
          //  _cts = new CancellationTokenSource();
     
          //  _ = Task.Run(() => RunHourlyLocationAsync(intent, _cts.Token));
          Task.Run(async() =>
          { 
              await RunHourlyLocationAsync(intent, _cts.Token); 
          });

            return StartCommandResult.Sticky;
        }

        async Task RunHourlyLocationAsync(Intent intent, CancellationToken token)
        {
            var id = intent?.GetStringExtra("USERID");

            // if intent value is a model object use this
           // var json = intent?.GetStringExtra("USER");
            //Users user = null;
            //if (!string.IsNullOrEmpty(json))
            //{
            //    user = JsonSerializer.Deserialize<Users>(json);
            //}

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var request = new GeolocationRequest(
                        GeolocationAccuracy.Medium,
                        TimeSpan.FromSeconds(60));

                 
                    var user = await _db.GetAsync<Users>($"Users/{id}");
                    if (user.CreditPoint > 0)
                    {
                        var location = await Geolocation.GetLocationAsync(request);
                        if (location != null)
                        {

                            var lat = location.Latitude;
                            var lon = location.Longitude;
                            var alt = location?.Altitude;
                            var acc = location?.Accuracy;
                            var time = location?.Timestamp;
                            var vertical = location?.VerticalAccuracy;
                            var speed = location?.Speed;
                            var course = location?.Course;

                            var currentloc = await _currentAddress.GetCurrentAddressAsync(lat, lon);

                            user.Latitude = lat;
                            user.Longitude = lon;
                            user.Altitude = alt;
                            user.Accuracy = acc;
                            user.Timestamp = DateTime.Now;
                            user.Vertical = vertical;
                            user.Speed = speed;
                            user.Course = course;

                            user.CurrentAddress = currentloc;



                            _ = _db.SaveAsync<Users>($"Users/{user.UserId}", user);
                            // TODO: Save or upload location
                            Console.WriteLine(
                                $"Lat:{location.Latitude}, Lng:{location.Longitude}");
                        }

                    }
                    else
                    {
                        // stop service if credit point is zero
                        StopForeground(true);

                    }
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                await Task.Delay(TimeSpan.FromMinutes(Constants.Constants.LocationService_Timer), token);
            }
        }


        public override IBinder OnBind(Intent intent) => null;
        public override void OnDestroy()
        {
            _cts?.Cancel();
            base.OnDestroy();
        }

       private Notification CreateNotification()
        {
            var channelId = "location_service_channel";
           
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    channelId,
                    "Background Service",
                    NotificationImportance.Low);

                var manager = GetSystemService(NotificationService) as NotificationManager;
                manager.CreateNotificationChannel(channel);
            }

           
            return new NotificationCompat.Builder(this, channelId)
            .SetContentTitle("Location Service Running")
            .SetContentText("Background task is active")
            .SetSmallIcon(Resource.Drawable.car)
            .Build();
        }

        void StopService()
        {
            _cts.Cancel();
            StopForeground(true); // remove notification
            StopSelf();
        }
    }
}
