namespace BookRide.Views;

public partial class ConfirmPage : ContentPage
{
	public ConfirmPage()
	{
		InitializeComponent();

	}

    private void Button_Clicked(object sender, EventArgs e)
    {
       // await Shell.Current.GoToAsync("//MainPage"); // Navigates to the root of the Page
        Shell.Current.GoToAsync("..//..");
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("..//..");
        return true;
    }
}