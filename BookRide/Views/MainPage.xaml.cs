using BookRide.ViewModels;
using BookRide.Views;

namespace BookRide
{
    public partial class MainPage : ContentPage
    {

        public MainPage(MainPageViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }


        private async void toolbar_guide_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(UserGuidePage));
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
