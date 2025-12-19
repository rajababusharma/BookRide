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
            Routing.RegisterRoute(nameof(DriverRegistration),typeof(Views.DriverRegistration));
            Routing.RegisterRoute(nameof(RegisterPage),typeof(Views.RegisterPage));
            Routing.RegisterRoute(nameof(DriverProfilePage),typeof(DriverProfilePage));
            Routing.RegisterRoute(nameof(TravellerProfilePage),typeof(TravellerProfilePage) );

        }
    }
}
