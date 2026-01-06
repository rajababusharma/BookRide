using Android.Content;
using Android.OS;
using BookRide.Interfaces;
using BookRide.Models;
using System.Text.Json;

namespace BookRide.Platforms.Android.Implementations
{

  
    public class LocationServiceStarter : ILocationService
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
