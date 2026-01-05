using BookRide.ViewModels;

namespace BookRide.Views;

public partial class TermsConditionsPage : ContentPage
{
    public TermsConditionsPage(TermsConditionVM vM)
	{
		BindingContext = vM;
        InitializeComponent();
	}

    private void btnregister_Clicked(object sender, EventArgs e)
    {
		if(chkAgree.IsChecked)
		{
            // Navigate to the registration page
			Shell.Current.GoToAsync(nameof(RegisterPage));
        }
		else
		{
			Shell.Current.DisplayAlert("Alert", "Please select terms and conditions first", "OK");
		}
    }

}