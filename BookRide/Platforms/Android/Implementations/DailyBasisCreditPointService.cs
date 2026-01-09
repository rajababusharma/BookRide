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
    [Service(Name = "com.companyname.bookride.DailyBasisCreditPointService",
    ForegroundServiceType = ForegroundService.TypeDataSync,
    Exported = false
)]
    public class DailyBasisCreditPointService : Service
    {
        CancellationTokenSource _cts;
        public const string ACTION_STOP_CREDIT = "ACTION_STOP_CREDIT_SERVICE";

        RealtimeDatabaseService _db;
        public DailyBasisCreditPointService()
        {
            _db = new RealtimeDatabaseService();
        }

        public override IBinder OnBind(Intent intent) => null;
        public override StartCommandResult OnStartCommand(
            Intent intent,
            StartCommandFlags flags,
            int startId)
        {
            if (intent?.Action == ACTION_STOP_CREDIT)
            {
                StopService();
                return StartCommandResult.NotSticky;
            }
            StartForeground(1002, CreateNotification());
            
            _cts = new CancellationTokenSource();
            //  _ = RunHourlyLocationAsync(_cts.Token);
            _ = Task.Run(() => RunHourlyCreditPointAsync(intent, _cts.Token));
            return StartCommandResult.Sticky;
        }

        async Task RunHourlyCreditPointAsync(Intent intent, CancellationToken token)
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
                   

                   // _db ??= new RealtimeDatabaseService();
                    var user = await _db.GetAsync<Users>($"Users/{id}");
                    user.CreditPoint -= 1;
                    // TODO: Save or upload user credit point
                    _ = _db.SaveAsync<Users>($"Users/{user.UserId}", user);

                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                await Task.Delay(TimeSpan.FromMinutes(2), token);
            }
        }

        public override void OnDestroy()
        {
            _cts?.Cancel();
            base.OnDestroy();
        }

        Notification CreateNotification()
        {
            var channelId = "creditpoint_service_channel";
            var manager = (NotificationManager)GetSystemService(NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    channelId,
                    "Credit Point Update",
                    NotificationImportance.Low);

                manager.CreateNotificationChannel(channel);
            }

            return new Notification.Builder(this, channelId)
                .SetContentTitle("Credit Point Service Running")
                .SetContentText("Updating credit point every day")
                .SetSmallIcon(Resource.Drawable.notification_bg)
                .SetOngoing(true)
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
