using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using AndroidX.Core.App;
using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Services;
using Java.Lang;
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
        private string currentloc;
        private bool isServiceRunning = false;
        // Public parameterless constructor required by Android runtime (resolves XA4213)
        public HourlyLocationService()
        {
            _db = new RealtimeDatabaseService();
            _currentAddress=new CurrentAddressService();
          
        }
     
        
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
           
                StartForeground(1001, CreateNotification());
            // If already running, nothing else to do.
            if (isServiceRunning)
            {
                return StartCommandResult.Sticky;
            }
            isServiceRunning = true;
            Task.Run(async () =>
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

            if (string.IsNullOrEmpty(id))
            {
                // No user ID provided, stop the service
                isServiceRunning = false;
                StopForeground(true);
                StopSelf();
                return;
            }

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var request = new GeolocationRequest(
                        GeolocationAccuracy.Medium,
                        TimeSpan.FromSeconds(10));

                 
                   // var user = await _db.GetAsync<Drivers>($"Drivers/{id}");

                    var user = await Task.Run(() =>
                             _db.GetAsync<Drivers>($"Drivers/{id}")
                           );
                   
                    // 
                    // await _db.SaveAsync($"Exceptions/{Guid.NewGuid()}", user);
                    if (user.CreditPoint > 0)
                    {
                       
                        var location = await Geolocation.GetLocationAsync(request);
                        if (location != null)
                        {
                            if (location.Latitude != 0.0 && location.Longitude != 0.0)
                            {

                                var lat = location.Latitude;
                                var lon = location.Longitude;
                                var alt = location?.Altitude;
                                var acc = location?.Accuracy;
                                var time = location?.Timestamp;
                                var vertical = location?.VerticalAccuracy;
                                var speed = location?.Speed;
                                var course = location?.Course;
                                try
                                {
                                    currentloc = await _currentAddress.GetCurrentAddressAsync(lat, lon);
                                  
                                }
                                catch (System.Exception ex)
                                {
                                    currentloc = null;
                                }
                                Drivers_Location drivers_Location = new Drivers_Location
                                {
                                    UserId = user.UserId,
                                    Latitude = lat,
                                    Longitude = lon,
                                    Altitude = alt,
                                    Accuracy = acc,
                                    Timestamp = DateTime.Now,
                                    Vertical = vertical,
                                    Speed = speed,
                                    Course = course,
                                    CurrentAddress = currentloc

                                };
                                
                                // TODO: Save or upload location
                                _ = _db.SaveAsync<Drivers_Location>($"Drivers_Location/{user.UserId}", drivers_Location);
                                
                               
                            }
                        }
                    }
                    else
                    {
                        isServiceRunning = false;
                        // stop service if credit point is zero
                        StopForeground(true);
                        StopSelf();
                        _cts.Cancel();

                    }
                   
                }
                catch (System.OperationCanceledException ex)
                {
                    ExceptionClass excp = new ExceptionClass { Message = ex.Message, StackTrace = ex.StackTrace, OccurredAt = DateTime.Now };
                    await _db.SaveAsync<ExceptionClass>($"Exceptions/{Guid.NewGuid()}", excp);
                    // Cancellation requested — exit cleanly.
                    isServiceRunning = false;
                    StopForeground(true);
                    StopSelf();
                    return;
                }
                catch (System.Exception ex)
                {
                    isServiceRunning = false;
                    // Console.WriteLine(ex.Message);
                    ExceptionClass excp = new ExceptionClass { Message = ex.Message, StackTrace = ex.StackTrace, OccurredAt = DateTime.Now };
                    await _db.SaveAsync<ExceptionClass>($"Exceptions/{Guid.NewGuid()}", excp);
                }
                try
                { 
                await Task.Delay(TimeSpan.FromMinutes(Constants.Constants.LocationService_Timer), token);
                }
                catch (System.OperationCanceledException ex)
                {
                    ExceptionClass excp = new ExceptionClass { Message = ex.Message, StackTrace = ex.StackTrace, OccurredAt = DateTime.Now };
                    await _db.SaveAsync<ExceptionClass>($"Exceptions/{Guid.NewGuid()}", excp);
                    // cancelled while waiting
                    isServiceRunning = false;
                StopForeground(true);
                StopSelf();
                return;
            }
        }
        }


        public override IBinder OnBind(Intent intent) => null;
        public override void OnDestroy()
        {
            isServiceRunning = false;
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
