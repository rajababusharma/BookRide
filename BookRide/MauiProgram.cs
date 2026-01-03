using BookRide.Interfaces;
using BookRide.Services;
using BookRide.Views;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using CommunityToolkit.Maui;


namespace BookRide
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // Add any additional services or configurations here


            // Register ViewModes here
            builder.Services.AddTransient<ViewModels.MainPageViewModel>();
            builder.Services.AddTransient<ViewModels.RegistrationTypeViewModel>();
            builder.Services.AddTransient<ViewModels.DriverRegistrationVM>();
            builder.Services.AddTransient<ViewModels.TravellerProfileVM>();
            builder.Services.AddTransient<ViewModels.DriverProfileVM>();
            builder.Services.AddTransient<ViewModels.RechargeCreditVM>();

            

            // Register views
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<Views.DriverRegistration>();        
            builder.Services.AddTransient<Views.DriverProfilePage>();
            builder.Services.AddTransient<Views.TravellerProfilePage>();
            builder.Services.AddTransient<Views.RechargeCreditPage>();
          builder.Services.AddTransient<Views.ConfirmPage>();
            builder.Services.AddTransient<Views.TermsConditionsPage>();

            // Register Services
            builder.Services.AddSingleton<ILocation, LocationService>();
            builder.Services.AddSingleton<IUsers, UserService>();
            builder.Services.AddTransient<IWhatsAppConnect, WhatsappConnectService>();
            //#if ANDROID
            // builder.Services.AddTransient<ITest, Platforms.Android.Implementations.Test>();
            //#endif

#if ANDROID
            builder.Services.AddTransient<IUpiPaymentService, Platforms.Android.Implementations.RazorpayUpiService>();
#elif IOS


#elif WINDOWS
                        
                      
#endif


            return builder.Build();
        }
       
     
    }
}
