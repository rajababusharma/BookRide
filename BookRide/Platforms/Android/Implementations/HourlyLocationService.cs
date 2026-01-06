using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using BookRide.Models;
using BookRide.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookRide.Platforms.Android.Implementations
{
    [Service(
    ForegroundServiceType = ForegroundService.TypeLocation,
    Exported = false
)]
    public class HourlyLocationService : Service
    {
        CancellationTokenSource _cts;

        RealtimeDatabaseService _db;
        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(
            Intent intent,
            StartCommandFlags flags,
            int startId)
        {
          
            StartForeground(1001, CreateNotification());
            _cts = new CancellationTokenSource();
          //  _ = RunHourlyLocationAsync(_cts.Token);
            _ = Task.Run(() => RunHourlyLocationAsync(intent, _cts.Token));
            return StartCommandResult.Sticky;
        }

        async Task RunHourlyLocationAsync(Intent intent, CancellationToken token)
        {
            var json = intent?.GetStringExtra("USER");

           Users user = null;
            if (!string.IsNullOrEmpty(json))
            {
                user = JsonSerializer.Deserialize<Users>(json);
            }
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var request = new GeolocationRequest(
                        GeolocationAccuracy.Medium,
                        TimeSpan.FromSeconds(30));

                    var location = await Geolocation.GetLocationAsync(request);

                    if (location != null)
                    {
                        _db ??= new RealtimeDatabaseService();
                        user.Latitude = location.Latitude;
                        user.Longitude = location.Longitude;
                        user.Location = location;
                        _ = _db.SaveAsync<Users>($"Users/{user.UserId}", user);
                        // TODO: Save or upload location
                        Console.WriteLine(
                            $"Lat:{location.Latitude}, Lng:{location.Longitude}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(50), token);
            }
        }

        public override void OnDestroy()
        {
            _cts?.Cancel();
            base.OnDestroy();
        }

        Notification CreateNotification()
        {
            var channelId = "location_service_channel";
            var manager = (NotificationManager)GetSystemService(NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    channelId,
                    "Location Tracking",
                    NotificationImportance.Low);

                manager.CreateNotificationChannel(channel);
            }

            return new Notification.Builder(this, channelId)
                .SetContentTitle("Location Service Running")
                .SetContentText("Fetching location every hour")
                .SetSmallIcon(Resource.Drawable.notification_bg)
                .SetOngoing(true)
                .Build();
        }
    }
}
