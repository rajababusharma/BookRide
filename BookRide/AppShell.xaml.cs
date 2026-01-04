using BookRide.Views;

namespace BookRide
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Register routes for navigation
            Routing.RegisterRoute(nameof(MainPage),typeof(MainPage));
          
            Routing.RegisterRoute(nameof(RegisterPage),typeof(Views.RegisterPage));
            Routing.RegisterRoute(nameof(DriverRegistration), typeof(Views.DriverRegistration));
            Routing.RegisterRoute(nameof(DriverProfilePage),typeof(DriverProfilePage));
            Routing.RegisterRoute(nameof(TravellerProfilePage),typeof(TravellerProfilePage) );
            Routing.RegisterRoute(nameof(RechargeCreditPage), typeof(RechargeCreditPage));
            Routing.RegisterRoute(nameof(ConfirmPage), typeof(ConfirmPage) );
            Routing.RegisterRoute(nameof(TermsConditionsPage), typeof(TermsConditionsPage) );
            Routing.RegisterRoute(nameof(RecoverPasswordPage), typeof(RecoverPasswordPage));

        }
    }
}
