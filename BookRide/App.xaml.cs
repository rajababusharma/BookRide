using Microsoft.Maui.Controls;

namespace BookRide
{
    public partial class App : Application
    {
        public App(AppShell shell)
        {
            InitializeComponent();
            MainPage = shell;

            // Navigate to Login page on app start
            Shell.Current.GoToAsync($"//{nameof(MainPage)}");
          //  MainPage = new AppShell();
        }
    }
}
