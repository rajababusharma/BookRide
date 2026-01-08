using Android.Content;
using Android.OS;
using BookRide.Interfaces;
using BookRide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookRide.Platforms.Android.Implementations
{
    public class UpdateCreditPointStarter : IForegroundService
    {
        public void Start(Users users)
        {

            var context = Microsoft.Maui.ApplicationModel.Platform.AppContext;



            var intent = new Intent(context, typeof(HourlyLocationService));
            var json = JsonSerializer.Serialize(users);
            intent.PutExtra("USER", json);
            // intent.PutExtra("USER", (Java.IO.ISerializable?)users);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                context.StartForegroundService(intent);
            else
                context.StartService(intent);
        }

        public void Stop()
        {
            var context = Microsoft.Maui.ApplicationModel.Platform.AppContext;
            var intent = new Intent(context, typeof(HourlyLocationService));
            context.StopService(intent);
        }
    }
}
