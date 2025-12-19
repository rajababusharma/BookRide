using BookRide.ViewModels;

namespace BookRide.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegistrationTypeViewModel register )
	{
        BindingContext = register;
        InitializeComponent();
	
	}

    protected override  bool OnBackButtonPressed()
    {
         Shell.Current.GoToAsync(nameof(MainPage));
        return base.OnBackButtonPressed();
    }
}