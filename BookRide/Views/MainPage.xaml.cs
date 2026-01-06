using BookRide.ViewModels;
using BookRide.Views;
using System.Threading.Tasks;

namespace BookRide
{
    public partial class MainPage : ContentPage
    {

        public MainPage(MainPageViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }

        override protected async void OnAppearing()
        {
            
            base.OnAppearing();
        }


        private void OnTogglePasswordClicked(object sender, EventArgs e)
        {
            // Toggle the IsPassword property
            passwordEntry.IsPassword = !passwordEntry.IsPassword;

            // Change the icon based on the visibility state
            if (passwordEntry.IsPassword)
            {
                toggleIcon.Source = "eye_off";
            }
            else
            {
                toggleIcon.Source = "eye.png";
            }
        }

    }

}
