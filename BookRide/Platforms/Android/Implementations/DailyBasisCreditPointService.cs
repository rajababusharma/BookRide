using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.Util;
using Android.OS;
using BookRide.Models;
using BookRide.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BookRide.Platforms.Android.Implementations
{
    [Service(Name = "com.intellics.bookride.DailyBasisCreditPointService",
        ForegroundServiceType = ForegroundService.TypeDataSync,
        Exported = true)]
    public class DailyBasisCreditPointService : Service
    {
        CancellationTokenSource _cts;
        private bool isServiceRunning = false;

        RealtimeDatabaseService _db;
        public DailyBasisCreditPointService()
        {
            _db = new RealtimeDatabaseService();
        }

        public override void OnCreate()
        {
            base.OnCreate();
            _cts = new CancellationTokenSource();
        }

        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(
            Intent intent,
            StartCommandFlags flags,
            int startId)
        {
           // Android.Util.Log.Debug("DailyBasisCreditPointService", $"OnStartCommand called. intent={(intent == null ? "null" : "present")}, startId={startId}");
            // Must call StartForeground quickly after startForegroundService() to avoid ANR.
            StartForeground(1002, CreateNotification());

            // If already running, nothing else to do.
            if (isServiceRunning)
            {
                return StartCommandResult.Sticky;
            }

            // Mark as running immediately, then kick off background work.
            isServiceRunning = true;

            // Ensure we have a cancellation token source.
            if (_cts == null || _cts.IsCancellationRequested)
                _cts = new CancellationTokenSource();

            // Run background loop without blocking the caller.
            _ = Task.Run(async () =>
            {
                await RunHourlyCreditPointAsync(intent, _cts.Token);
            });

            return StartCommandResult.Sticky;
        }

        async Task RunHourlyCreditPointAsync(Intent intent, CancellationToken token)
        {
            var id = intent?.GetStringExtra("USERID");
            if (string.IsNullOrWhiteSpace(id))
            {
               // Android.Util.Log.Warn("DailyBasisCreditPointService", "RunHourlyCreditPointAsync: USERID is missing from intent. Stopping service.");
                // No user ID provided — stop service gracefully.
                StopForeground(true);
                StopSelf();
                isServiceRunning = false;
                return;
            }

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var user = await Task.Run(() =>
                              _db.GetAsync<Drivers>($"Drivers/{id}")
                           );

                    if (user == null)
                    {
                       // Android.Util.Log.Warn("DailyBasisCreditPointService", $"No driver found for id={id}. Stopping service.");
                        // No user found — stop service gracefully.
                        StopForeground(true);
                        StopSelf();
                        isServiceRunning = false;
                        return;
                    }

                    // Deduct one credit point
                    if (!string.IsNullOrWhiteSpace(user.FirstName) &&
                        !string.IsNullOrWhiteSpace(user.UserId) &&
                        user.CreditPoint > 0 &&
                        !string.IsNullOrWhiteSpace(user.Mobile))
                    {
                        Drivers drivers = new Drivers
                        {
                            UserId = user.UserId,
                            FirstName = user.FirstName,
                            Age = user.Age,
                            Address = user.Address,
                            Mobile = user.Mobile,
                            Password = user.Password,
                            VehicleNo = user.VehicleNo,
                            CreditPoint = user.CreditPoint - 1,
                            VehicleType = user.VehicleType,
                            State = user.State,
                            District = user.District,
                            RegistrationDate = user.RegistrationDate,
                            AadharImageURL = user.AadharImageURL,
                            ProfileImageUrl = user.ProfileImageUrl,
                            IsActive = user.IsActive
                        };
                       // user.CreditPoint -= 1;
                        _ = _db.SaveAsync<Drivers>($"Drivers/{user.UserId}", drivers);
                    }
                    else
                    {
                       // Android.Util.Log.Info("DailyBasisCreditPointService", $"User {id} has no credits or invalid data. Stopping service.");
                        // stop service if credit point is zero
                        isServiceRunning = false;
                        StopForeground(true);
                        StopSelf();
                        return;
                    }
                }
                catch (System.OperationCanceledException)
                {
                    // Cancellation requested — exit cleanly.
                   // Android.Util.Log.Info("DailyBasisCreditPointService", "Operation canceled, stopping service.");
                    isServiceRunning = false;
                    StopForeground(true);
                    StopSelf();
                    return;
                }
                catch (Exception ex)
                {
                   // Android.Util.Log.Error("DailyBasisCreditPointService", $"Unhandled exception: {ex}");
                    isServiceRunning = false;
                    ExceptionClass excp = new ExceptionClass { Message = ex.Message, StackTrace = ex.StackTrace, OccurredAt = DateTime.Now };
                    await _db.SaveAsync<ExceptionClass>($"Exceptions/{Guid.NewGuid()}", excp);

                    // Stop to avoid repeated failures; caller can restart if needed.
                    StopForeground(true);
                    StopSelf();
                    return;
                }

                // Wait for the configured interval or until cancelled.
                try
                {
                    await Task.Delay(TimeSpan.FromHours(Constants.Constants.CreditPointService_Timer), token);
                }
                catch (System.OperationCanceledException ex)
                {
                   // Android.Util.Log.Info("DailyBasisCreditPointService", $"Delay cancelled: {ex.Message}");
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

        public override void OnDestroy()
        {
            isServiceRunning = false;
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
                .SetSmallIcon(Resource.Drawable.car)
                .SetOngoing(true)
                .Build();
        }

        void StopService()
        {
            _cts?.Cancel();
            StopForeground(true); // remove notification
            StopSelf();
        }
    }
}
