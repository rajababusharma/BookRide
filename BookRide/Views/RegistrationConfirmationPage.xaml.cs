using BookRide.ViewModels;

namespace BookRide.Views;

public partial class RegistrationConfirmationPage : ContentPage
{
	public RegistrationConfirmationPage(ConfirmRegistrationVM vM)
	{
		BindingContext = vM;
        InitializeComponent();
	}
}