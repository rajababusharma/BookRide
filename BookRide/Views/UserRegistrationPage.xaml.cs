using BookRide.ViewModels;

namespace BookRide.Views;

public partial class UserRegistrationPage : ContentPage
{
    public UserRegistrationPage(UserRegistrationVM vM)
    {
        BindingContext = vM;
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("..//..//..");
        return true;
    }
}