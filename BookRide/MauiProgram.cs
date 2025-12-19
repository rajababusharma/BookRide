using BookRide.Views;
using Microsoft.Extensions.Logging;

namespace BookRide
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // Add any additional services or configurations here
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ViewModels.MainPageViewModel>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<Views.DriverRegistration>();
            builder.Services.AddTransient<ViewModels.RegistrationTypeViewModel>();
            builder.Services.AddTransient<ViewModels.DriverRegistrationVM>();   
            builder.Services.AddTransient<Views.DriverProfilePage>();
            builder.Services.AddTransient<Views.TravellerProfilePage>();

            return builder.Build();
        }
    }
}
