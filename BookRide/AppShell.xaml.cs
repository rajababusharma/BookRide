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
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(DriverRegistration),typeof(Views.DriverRegistration));
            Routing.RegisterRoute(nameof(TravellerRegistration), typeof(Views.TravellerRegistration));
        }
    }
}
