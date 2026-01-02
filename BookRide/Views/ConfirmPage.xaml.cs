namespace BookRide.Views;

public partial class ConfirmPage : ContentPage
{
	public ConfirmPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage"); // Navigates to the root of the Page
    }
}