using Android.App;
using Android.Content;
using BookRide.Platforms.Android.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Platforms.Android.BroadcastReceivers
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceivers : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent.Action == Intent.ActionBootCompleted)
            {
                var serviceIntent = new Intent(context, typeof(HourlyLocationService));
                context.StartForegroundService(serviceIntent);
            }
        }
    }
}
