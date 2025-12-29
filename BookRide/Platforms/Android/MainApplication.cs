using Android.App;
using Android.Runtime;

namespace BookRide
{
    [Application]
    public class MainApplication : MauiApplication
    {
        [assembly: UsesPermission(Android.Manifest.Permission.AccessCoarseLocation)]
        [assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
        [assembly: UsesFeature("android.hardware.location", Required = false)]
        [assembly: UsesFeature("android.hardware.location.gps", Required = false)]
        [assembly: UsesFeature("android.hardware.location.network", Required = false)]
        [assembly: UsesPermission(Android.Manifest.Permission.AccessBackgroundLocation)]
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
