using BookRide.ViewModels;

namespace BookRide.Views;

public partial class RecoverPasswordPage : ContentPage
{
	RecoverPasswordVM PasswordVM;
	public RecoverPasswordPage(RecoverPasswordVM vM)
	{
		BindingContext = vM;
		InitializeComponent();
	}
}