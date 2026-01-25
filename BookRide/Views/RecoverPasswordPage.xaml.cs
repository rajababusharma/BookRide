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

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var selected = (sender as RadioButton)?.Content.ToString();
            // DisplayAlert("Selected", selected, "OK");
            var viewModel = BindingContext as RecoverPasswordVM;
            if (viewModel != null)
            {
                viewModel.SelectedUserType = selected;
            }
        }
    }
}